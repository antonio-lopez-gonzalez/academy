import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs/promises";
import os from "node:os";
import path from "node:path";
import { Catalog } from "../src/catalog.mjs";

async function withCatalog(run) {
  const dir = await fs.mkdtemp(path.join(os.tmpdir(), "doc-catalog-"));
  try {
    await run(new Catalog(path.join(dir, "catalog.json")));
  } finally {
    await fs.rm(dir, { recursive: true, force: true });
  }
}

test("crea, actualiza y lista documentos", async () => {
  await withCatalog(async (catalog) => {
    await catalog.create({
      id: "d1",
      tenant: "acme",
      status: "pending",
      createdAt: "2026-01-01T00:00:00.000Z",
    });
    assert.equal((await catalog.get("d1")).status, "pending");

    const updated = await catalog.update("d1", { status: "stored", etag: "abc" });
    assert.equal(updated.status, "stored");
    assert.equal(updated.etag, "abc");
    assert.ok(updated.updatedAt);
  });
});

test("filtra por tenant y ordena por fecha descendente", async () => {
  await withCatalog(async (catalog) => {
    await catalog.create({ id: "a", tenant: "acme", createdAt: "2026-01-01T00:00:00.000Z" });
    await catalog.create({ id: "b", tenant: "acme", createdAt: "2026-02-01T00:00:00.000Z" });
    await catalog.create({ id: "c", tenant: "globex", createdAt: "2026-03-01T00:00:00.000Z" });

    assert.deepEqual((await catalog.list({ tenant: "acme" })).map((d) => d.id), ["b", "a"]);
    assert.equal((await catalog.list()).length, 3);
  });
});

test("devuelve null al actualizar un id inexistente", async () => {
  await withCatalog(async (catalog) => {
    assert.equal(await catalog.update("nope", { status: "stored" }), null);
  });
});
