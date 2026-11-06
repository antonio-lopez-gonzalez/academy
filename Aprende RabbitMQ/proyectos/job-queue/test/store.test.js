import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs/promises";
import os from "node:os";
import path from "node:path";
import { JobStore } from "../src/store.js";

async function withStore(run) {
  const dir = await fs.mkdtemp(path.join(os.tmpdir(), "job-store-"));
  try {
    await run(new JobStore(path.join(dir, "jobs.json")));
  } finally {
    await fs.rm(dir, { recursive: true, force: true });
  }
}

function job(overrides = {}) {
  return {
    jobId: "6f1c2a9e-4b7d-4c1e-9f30-2a7d5e8b1c44",
    type: "invoice.generate",
    payload: { orderId: "PED-1042" },
    status: "queued",
    attempts: 0,
    createdAt: "2026-11-05T10:00:00.000Z",
    ...overrides,
  };
}

test("crea, lee y marca publicado", async () => {
  await withStore(async (store) => {
    await store.create(job());
    assert.equal((await store.get(job().jobId)).status, "queued");

    const published = await store.markPublished(job().jobId);
    assert.ok(published.publishedAt);
  });
});

test("claim es atómico: solo un worker reclama", async () => {
  await withStore(async (store) => {
    await store.create(job());
    const now = new Date("2026-11-05T10:00:00.000Z");

    const first = await store.claim(job().jobId, { leaseMs: 60_000, now });
    assert.equal(first.status, "running");
    assert.ok(first.leaseUntil);

    const second = await store.claim(job().jobId, { leaseMs: 60_000, now });
    assert.equal(second, null);
  });
});

test("un lease vencido permite reclamar de nuevo", async () => {
  await withStore(async (store) => {
    await store.create(job());
    const now = new Date("2026-11-05T10:00:00.000Z");

    await store.claim(job().jobId, { leaseMs: 1_000, now });
    const after = new Date(now.getTime() + 2_000);
    const reclaimed = await store.claim(job().jobId, { leaseMs: 60_000, now: after });
    assert.equal(reclaimed.status, "running");
  });
});

test("retry, éxito y fallo actualizan el estado correctamente", async () => {
  await withStore(async (store) => {
    await store.create(job());

    const retry = await store.markRetry(job().jobId, 1, 5_000, "timeout");
    assert.equal(retry.status, "retry_scheduled");
    assert.equal(retry.attempts, 1);
    assert.ok(retry.nextAttemptAt);

    const succeeded = await store.markSucceeded(job().jobId, { invoiceId: "F-1" });
    assert.equal(succeeded.status, "succeeded");
    assert.equal(succeeded.result.invoiceId, "F-1");

    const failed = await store.markFailed(job().jobId, "permanent", "dato inválido");
    assert.equal(failed.status, "failed");
    assert.equal(failed.failureReason, "permanent");
  });
});

test("serializa escrituras concurrentes sin perder jobs", async () => {
  await withStore(async (store) => {
    await Promise.all(
      Array.from({ length: 20 }, (_, i) =>
        store.create(job({ jobId: `job-${i}`, createdAt: new Date(2026, 0, i + 1).toISOString() })),
      ),
    );
    assert.equal((await store.list()).length, 20);
  });
});
