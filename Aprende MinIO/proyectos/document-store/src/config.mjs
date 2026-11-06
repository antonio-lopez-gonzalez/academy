import path from "node:path";
import { fileURLToPath } from "node:url";

const projectRoot = fileURLToPath(new URL("..", import.meta.url));

export const config = {
  endpoint: process.env.MINIO_ENDPOINT ?? "127.0.0.1",
  port: Number(process.env.MINIO_PORT ?? 9000),
  useSSL: process.env.MINIO_USE_SSL === "true",
  accessKey: process.env.MINIO_ACCESS_KEY ?? "minioadmin",
  secretKey: process.env.MINIO_SECRET_KEY ?? "minioadmin123",
  bucket: process.env.MINIO_BUCKET ?? "documentos",
  apiPort: Number(process.env.API_PORT ?? 7072),
  maxUploadBytes: Number(process.env.MAX_UPLOAD_BYTES ?? 25 * 1024 * 1024),
  presignExpirySeconds: Number(process.env.PRESIGN_EXPIRY_SECONDS ?? 15 * 60),
  webhookUrl: process.env.WEBHOOK_URL ?? "",
  webhookToken: process.env.MINIO_WEBHOOK_TOKEN ?? "dev-token",
  catalogPath: process.env.CATALOG_PATH ?? path.join(projectRoot, "data", "catalog.json"),
};
