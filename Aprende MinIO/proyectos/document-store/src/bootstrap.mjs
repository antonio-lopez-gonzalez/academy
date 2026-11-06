import { config } from "./config.mjs";
import { createStorage } from "./storage.mjs";

const client = await createStorage(config);
const versioning = await client.getBucketVersioning(config.bucket).catch(() => null);
const lifecycle = await client.getBucketLifecycle(config.bucket).catch(() => null);

console.log(`bucket ${config.bucket} listo en ${config.endpoint}:${config.port}`);
console.log("versionado:", versioning?.Status ?? "desconocido");
console.log("reglas de lifecycle:", lifecycle?.Rule?.map((r) => r.ID).join(", ") ?? "ninguna");
