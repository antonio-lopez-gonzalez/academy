import path from "node:path";
import { fileURLToPath } from "node:url";

const projectRoot = fileURLToPath(new URL("..", import.meta.url));

export const config = {
  url:
    process.env.RABBITMQ_URL ??
    process.env.AMQP_URL ??
    "amqp://taller-api:taller-api-secret@localhost:5672/taller",
  apiPort: Number(process.env.API_PORT ?? 3000),
  prefetch: Number(process.env.PREFETCH ?? 10),
  leaseMs: Number(process.env.JOB_LEASE_MS ?? 60_000),
  storePath: process.env.JOB_STORE ?? path.join(projectRoot, "data", "jobs.json"),
};
