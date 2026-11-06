import fs from "node:fs/promises";
import path from "node:path";

/**
 * Almacén de estado de los jobs.
 *
 * En el curso es un JSON para no añadir una base de datos; en producción sería
 * PostgreSQL. La parte importante es la reclamación atómica con lease:
 *
 *   queued | retry_scheduled  → running (leaseUntil)
 *   running con lease vencido → running (nuevo lease)
 *   running con lease vivo    → no se puede reclamar
 *   succeeded | failed        → terminal
 */
export class JobStore {
  constructor(file) {
    this.file = file;
    this.chain = Promise.resolve();
  }

  async #read() {
    try {
      return JSON.parse(await fs.readFile(this.file, "utf8"));
    } catch (error) {
      if (error.code === "ENOENT") return {};
      throw error;
    }
  }

  async #write(data) {
    await fs.mkdir(path.dirname(this.file), { recursive: true });
    const tmp = `${this.file}.${process.pid}.tmp`;
    await fs.writeFile(tmp, JSON.stringify(data, null, 2));
    await fs.rename(tmp, this.file);
  }

  #mutate(mutator) {
    const run = this.chain.then(async () => {
      const data = await this.#read();
      const result = mutator(data);
      await this.#write(data);
      return result;
    });
    this.chain = run.catch(() => {});
    return run;
  }

  create(job) {
    return this.#mutate((data) => {
      data[job.jobId] = job;
      return job;
    });
  }

  async get(jobId) {
    return (await this.#read())[jobId] ?? null;
  }

  async list() {
    const data = await this.#read();
    return Object.values(data).sort((a, b) => b.createdAt.localeCompare(a.createdAt));
  }

  markPublished(jobId) {
    return this.update(jobId, { publishedAt: new Date().toISOString() });
  }

  /**
   * Reclama un job para un worker. Devuelve el job si la reclamación es válida
   * y null si otro worker lo tiene o ya terminó.
   */
  claim(jobId, { leaseMs = 60_000, now = new Date() } = {}) {
    return this.#mutate((data) => {
      const job = data[jobId];
      if (!job) return null;

      const leaseExpired =
        job.status === "running" &&
        (!job.leaseUntil || new Date(job.leaseUntil).getTime() <= now.getTime());

      const claimable = job.status === "queued" || job.status === "retry_scheduled";
      if (!claimable && !leaseExpired) return null;

      job.status = "running";
      job.leaseUntil = new Date(now.getTime() + leaseMs).toISOString();
      job.startedAt = now.toISOString();
      job.updatedAt = now.toISOString();
      return job;
    });
  }

  markSucceeded(jobId, result) {
    return this.update(jobId, {
      status: "succeeded",
      result,
      completedAt: new Date().toISOString(),
      leaseUntil: null,
    });
  }

  markRetry(jobId, attempt, delayMs, error) {
    return this.update(jobId, {
      status: "retry_scheduled",
      attempts: attempt,
      nextAttemptAt: new Date(Date.now() + delayMs).toISOString(),
      lastError: String(error).slice(0, 500),
      leaseUntil: null,
    });
  }

  markFailed(jobId, reason, error) {
    return this.update(jobId, {
      status: "failed",
      failureReason: reason,
      lastError: String(error).slice(0, 500),
      failedAt: new Date().toISOString(),
      leaseUntil: null,
    });
  }

  markQueued(jobId) {
    return this.update(jobId, {
      status: "queued",
      attempts: 0,
      nextAttemptAt: null,
      lastError: null,
      leaseUntil: null,
    });
  }

  update(jobId, patch) {
    return this.#mutate((data) => {
      const job = data[jobId];
      if (!job) return null;
      data[jobId] = { ...job, ...patch, updatedAt: new Date().toISOString() };
      return data[jobId];
    });
  }
}
