import * as Minio from "minio";

/**
 * Envoltorio del SDK de MinIO. El bucket se prepara de forma idempotente:
 * versionado activo y lifecycle para temporales y versiones antiguas.
 */
export async function createStorage(config) {
  const client = new Minio.Client({
    endPoint: config.endpoint,
    port: config.port,
    useSSL: config.useSSL,
    accessKey: config.accessKey,
    secretKey: config.secretKey,
  });

  await ensureBucket(client, config);
  return client;
}

export async function ensureBucket(client, config) {
  const exists = await client.bucketExists(config.bucket);
  if (!exists) {
    await client.makeBucket(config.bucket, "us-east-1");
    console.log(`bucket creado: ${config.bucket}`);
  }

  try {
    await client.setBucketVersioning(config.bucket, { Status: "Enabled" });
  } catch (error) {
    console.warn("[minio] no se pudo activar el versionado:", error.message);
  }

  try {
    await client.setBucketLifecycle(config.bucket, {
      Rule: [
        {
          ID: "expire-tmp",
          Status: "Enabled",
          Filter: { Prefix: "tmp/" },
          Expiration: { Days: 1 },
        },
        {
          ID: "expire-noncurrent",
          Status: "Enabled",
          Filter: { Prefix: "" },
          NoncurrentVersionExpiration: { NoncurrentDays: 30 },
        },
      ],
    });
  } catch (error) {
    console.warn("[minio] no se pudo aplicar el lifecycle:", error.message);
  }
}

export function presignedPut(client, config, key) {
  return client.presignedPutObject(config.bucket, key, config.presignExpirySeconds);
}

export function presignedGet(client, config, key) {
  return client.presignedGetObject(config.bucket, key, config.presignExpirySeconds);
}

export function statObject(client, config, key) {
  return client.statObject(config.bucket, key);
}

export async function listObjects(client, config, prefix = "") {
  return new Promise((resolve, reject) => {
    const objects = [];
    const stream = client.listObjectsV2(config.bucket, prefix, true);
    stream.on("data", (obj) => objects.push(obj));
    stream.on("error", reject);
    stream.on("end", () => resolve(objects));
  });
}

export function removeObject(client, config, key) {
  return client.removeObject(config.bucket, key);
}
