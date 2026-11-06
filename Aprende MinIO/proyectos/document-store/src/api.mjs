import http from "node:http";
import crypto from "node:crypto";
import { config } from "./config.mjs";
import { Catalog } from "./catalog.mjs";
import {
  assertContentType,
  assertSize,
  assertTenant,
  documentKey,
} from "./keys.mjs";
import {
  createStorage,
  listObjects,
  presignedGet,
  presignedPut,
  removeObject,
  statObject,
} from "./storage.mjs";

class HttpError extends Error {
  constructor(status, message) {
    super(message);
    this.status = status;
  }
}

function send(res, status, body) {
  if (status === 204) {
    res.writeHead(204, { "Access-Control-Allow-Origin": "*" });
    res.end();
    return;
  }
  const payload = JSON.stringify(body, null, 2);
  res.writeHead(status, {
    "Content-Type": "application/json; charset=utf-8",
    "Content-Length": Buffer.byteLength(payload),
    "Access-Control-Allow-Origin": "*",
  });
  res.end(payload);
}

async function readJson(req) {
  const chunks = [];
  let size = 0;
  for await (const chunk of req) {
    size += chunk.length;
    if (size > 1_000_000) throw new HttpError(413, "cuerpo demasiado grande");
    chunks.push(chunk);
  }
  if (chunks.length === 0) return {};
  try {
    return JSON.parse(Buffer.concat(chunks).toString("utf8"));
  } catch {
    throw new HttpError(400, "JSON inválido");
  }
}

async function notify(payload) {
  if (!config.webhookUrl) return;
  try {
    const response = await fetch(config.webhookUrl, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
      signal: AbortSignal.timeout(3000),
    });
    if (!response.ok) console.warn("[webhook]", response.status);
  } catch (error) {
    console.warn("[webhook] no se pudo entregar:", error.message);
  }
}

const catalog = new Catalog(config.catalogPath);
const client = await createStorage(config);

const server = http.createServer(async (req, res) => {
  try {
    const url = new URL(req.url, "http://localhost");

    if (req.method === "GET" && url.pathname === "/health") {
      const ok = await client.bucketExists(config.bucket);
      return send(res, ok ? 200 : 503, { status: ok ? "ok" : "degraded", bucket: config.bucket });
    }

    if (req.method === "GET" && url.pathname === "/documents") {
      const tenant = url.searchParams.get("tenant") ?? undefined;
      if (tenant) assertTenant(tenant);
      return send(res, 200, { documents: await catalog.list({ tenant }) });
    }

    const match = url.pathname.match(/^\/documents\/([^/]+)(\/complete)?$/);
    if (match) {
      const [, id, action] = match;

      if (req.method === "POST" && action === "/complete") {
        const doc = await catalog.get(id);
        if (!doc) throw new HttpError(404, "documento no encontrado");

        const stat = await statObject(client, config, doc.key).catch(() => null);
        if (!stat) throw new HttpError(409, "el objeto todavía no se ha subido");

        if (stat.size > config.maxUploadBytes) {
          throw new HttpError(413, "el objeto supera el tamaño máximo");
        }

        const updated = await catalog.update(id, {
          status: "stored",
          size: stat.size,
          etag: stat.etag,
          completedAt: new Date().toISOString(),
        });

        await notify({
          event: "document.stored",
          id,
          key: doc.key,
          size: stat.size,
          etag: stat.etag,
          tenant: doc.tenant,
        });

        return send(res, 200, updated);
      }

      if (req.method === "GET") {
        const doc = await catalog.get(id);
        if (!doc) throw new HttpError(404, "documento no encontrado");
        const downloadUrl = await presignedGet(client, config, doc.key);
        return send(res, 200, { ...doc, downloadUrl, expiresIn: config.presignExpirySeconds });
      }

      if (req.method === "DELETE") {
        const doc = await catalog.get(id);
        if (!doc) throw new HttpError(404, "documento no encontrado");
        // Con versionado activo, esto crea un delete marker (recuperable).
        await removeObject(client, config, doc.key);
        const updated = await catalog.update(id, {
          status: "deleted",
          deletedAt: new Date().toISOString(),
        });
        await notify({ event: "document.deleted", id, key: doc.key, tenant: doc.tenant });
        return send(res, 200, updated);
      }
    }

    if (req.method === "POST" && url.pathname === "/documents") {
      const body = await readJson(req);
      const tenant = assertTenant(body.tenant);
      const filename = body.filename;
      const contentType = assertContentType(body.contentType);
      const size = assertSize(body.size, config.maxUploadBytes);

      const id = crypto.randomUUID();
      const key = documentKey({ tenant, filename, id });
      const doc = {
        id,
        tenant,
        filename,
        key,
        contentType,
        size,
        status: "pending",
        createdAt: new Date().toISOString(),
      };

      await catalog.create(doc);
      const uploadUrl = await presignedPut(client, config, key);

      return send(res, 201, {
        id,
        key,
        uploadUrl,
        expiresIn: config.presignExpirySeconds,
        next: `PUT ${uploadUrl} y luego POST /documents/${id}/complete`,
      });
    }

    // Receptor de ejemplo para notificaciones reales de MinIO (mc event add).
    if (req.method === "POST" && url.pathname === "/webhooks/minio") {
      const expected = config.webhookToken ? `Bearer ${config.webhookToken}` : null;
      if (expected && req.headers.authorization !== expected) {
        throw new HttpError(401, "unauthorized");
      }
      const event = await readJson(req);
      console.log("[minio-event]", event.EventName ?? "evento sin nombre");
      for (const record of event.Records ?? []) {
        console.log("[minio-event]", record.s3?.object?.key ?? "objeto sin clave");
      }
      return send(res, 204);
    }

    // Listado directo del bucket (no depende del catálogo).
    if (req.method === "GET" && url.pathname === "/objects") {
      const prefix = url.searchParams.get("prefix") ?? "";
      return send(res, 200, { objects: await listObjects(client, config, prefix) });
    }

    throw new HttpError(404, "ruta no encontrada");
  } catch (error) {
    const status = error instanceof HttpError ? error.status : 500;
    if (status === 500) console.error("[api]", error);
    if (!res.headersSent) send(res, status, { error: error.message });
  }
});

server.listen(config.apiPort, () => {
  console.log(`document-store API → http://localhost:${config.apiPort}`);
  console.log(`MinIO → ${config.useSSL ? "https" : "http"}://${config.endpoint}:${config.port}`);
});

let closing = false;
function shutdown(signal) {
  if (closing) return;
  closing = true;
  console.log(`\n[api] ${signal}: cerrando…`);
  server.close(() => process.exit(0));
  setTimeout(() => process.exit(0), 2000).unref();
}
process.on("SIGINT", () => shutdown("SIGINT"));
process.on("SIGTERM", () => shutdown("SIGTERM"));
