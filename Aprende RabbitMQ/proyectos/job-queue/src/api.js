import http from "node:http";
import crypto from "node:crypto";
import { config } from "./config.js";
import { JobStore } from "./store.js";
import { createPublisher } from "./publisher.js";

class HttpError extends Error {
  constructor(status, message, extra = {}) {
    super(message);
    this.status = status;
    this.extra = extra;
  }
}

function send(res, status, body) {
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
    if (size > 1024 * 1024) throw new HttpError(413, "payload_too_large");
    chunks.push(chunk);
  }
  if (chunks.length === 0) return {};
  try {
    return JSON.parse(Buffer.concat(chunks).toString("utf8"));
  } catch {
    throw new HttpError(400, "invalid_json");
  }
}

function validateJob(body) {
  if (typeof body.type !== "string" || body.type.trim() === "") {
    throw new HttpError(400, "invalid_job");
  }
  if (
    body.payload === undefined ||
    body.payload === null ||
    typeof body.payload !== "object" ||
    Array.isArray(body.payload)
  ) {
    throw new HttpError(400, "invalid_job");
  }
  return { type: body.type.trim(), payload: body.payload };
}

function publicJob(job) {
  return {
    id: job.jobId,
    jobId: job.jobId,
    type: job.type,
    status: job.status,
    attempts: job.attempts,
    result: job.result ?? null,
    lastError: job.lastError ?? null,
    nextAttemptAt: job.nextAttemptAt ?? null,
    traceId: job.traceId,
    createdAt: job.createdAt,
    publishedAt: job.publishedAt ?? null,
  };
}

const store = new JobStore(config.storePath);
const publisher = createPublisher(config);
await publisher.start();

const server = http.createServer(async (req, res) => {
  try {
    const url = new URL(req.url, "http://localhost");

    if (req.method === "GET" && url.pathname === "/health/live") {
      return send(res, 200, { status: "ok" });
    }

    if (req.method === "GET" && url.pathname === "/health") {
      const ready = publisher.isReady();
      return send(res, ready ? 200 : 503, {
        status: ready ? "ok" : "degraded",
        broker: ready ? "connected" : "unavailable",
      });
    }

    if (req.method === "GET" && url.pathname === "/jobs") {
      return send(res, 200, { jobs: (await store.list()).map(publicJob) });
    }

    const match = url.pathname.match(/^\/jobs\/([\w-]+)$/);
    if (req.method === "GET" && match) {
      const job = await store.get(match[1]);
      if (!job) throw new HttpError(404, "not_found");
      return send(res, 200, publicJob(job));
    }

    if (req.method === "POST" && url.pathname === "/jobs") {
      const input = validateJob(await readJson(req));
      const job = {
        jobId: crypto.randomUUID(),
        type: input.type,
        payload: input.payload,
        traceId: String(req.headers["x-trace-id"] ?? crypto.randomUUID()),
        status: "queued",
        attempts: 0,
        createdAt: new Date().toISOString(),
      };

      await store.create(job);

      try {
        await publisher.publishJob(job);
        await store.markPublished(job.jobId);
      } catch (error) {
        console.error(JSON.stringify({ event: "publish_failed", jobId: job.jobId, error: error.message }));
        throw new HttpError(503, "broker_unavailable", { jobId: job.jobId });
      }

      res.setHeader("Location", `/jobs/${job.jobId}`);
      return send(res, 202, { id: job.jobId, status: "queued" });
    }

    throw new HttpError(404, "not_found");
  } catch (error) {
    const status = error instanceof HttpError ? error.status : 500;
    if (status === 500) console.error(JSON.stringify({ event: "api.error", error: error.message }));
    if (!res.headersSent) {
      const body = { error: error.message, ...(error.extra ?? {}) };
      send(res, status, body);
    }
  }
});

/** Republica jobs que quedaron persistidos sin publicar por una caída del broker. */
async function sweepUnpublished() {
  if (!publisher.isReady()) return;
  const jobs = await store.list();
  for (const job of jobs) {
    if (job.status !== "queued" || job.publishedAt) continue;
    try {
      await publisher.publishJob(job);
      await store.markPublished(job.jobId);
      console.log(JSON.stringify({ event: "job.republished", jobId: job.jobId }));
    } catch (error) {
      console.error(JSON.stringify({ event: "job.republish_failed", jobId: job.jobId, error: error.message }));
      return;
    }
  }
}

const sweeper = setInterval(() => {
  sweepUnpublished().catch((error) =>
    console.error(JSON.stringify({ event: "sweeper.error", error: error.message })),
  );
}, 15_000);
sweeper.unref();

server.listen(config.apiPort, () => {
  console.log(JSON.stringify({ event: "api.listening", port: config.apiPort }));
  console.log(JSON.stringify({ event: "api.store", path: config.storePath }));
});

let closing = false;
async function shutdown(signal) {
  if (closing) return;
  closing = true;
  console.log(JSON.stringify({ event: "api.shutdown", signal }));
  clearInterval(sweeper);
  server.close();
  await publisher.close();
  process.exitCode = 0;
}

process.on("SIGINT", () => void shutdown("SIGINT"));
process.on("SIGTERM", () => void shutdown("SIGTERM"));
