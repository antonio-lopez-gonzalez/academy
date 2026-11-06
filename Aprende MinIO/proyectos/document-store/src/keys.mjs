/**
 * Reglas de claves y validación, sin dependencias de MinIO.
 * Se prueban con `node --test`.
 */

const TENANT_RE = /^[a-z0-9][a-z0-9-]{1,30}$/;

export const ALLOWED_CONTENT_TYPES = new Set([
  "application/pdf",
  "image/png",
  "image/jpeg",
  "image/webp",
  "text/plain",
  "text/markdown",
  "application/zip",
  "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
  "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
]);

export function assertTenant(tenant) {
  if (typeof tenant !== "string" || !TENANT_RE.test(tenant)) {
    throw new Error("tenant inválido: usa minúsculas, números y guiones (2–31 caracteres)");
  }
  return tenant;
}

/** Limpia un nombre de archivo y elimina cualquier intento de ruta. */
export function sanitizeFilename(filename) {
  if (typeof filename !== "string" || filename.trim() === "") {
    throw new Error("filename es obligatorio");
  }

  const base = filename
    .replace(/\\/g, "/")
    .split("/")
    .pop()
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/[^a-zA-Z0-9._-]+/g, "-")
    .replace(/-+/g, "-")
    .replace(/-+\./g, ".")
    .replace(/^[-.]+|[-.]+$/g, "")
    .slice(0, 120);

  if (base === "" || base === "." || base === "..") {
    throw new Error("filename inválido");
  }
  return base;
}

export function assertContentType(contentType) {
  if (typeof contentType !== "string" || contentType === "") {
    throw new Error("contentType es obligatorio");
  }
  const normalized = contentType.split(";")[0].trim().toLowerCase();
  if (!ALLOWED_CONTENT_TYPES.has(normalized)) {
    throw new Error(`contentType no permitido: ${normalized}`);
  }
  return normalized;
}

export function assertSize(size, maxBytes) {
  if (!Number.isInteger(size) || size <= 0) {
    throw new Error("size debe ser un entero positivo");
  }
  if (size > maxBytes) {
    throw new Error(`size supera el máximo de ${maxBytes} bytes`);
  }
  return size;
}

/**
 * Clave final: tenant/AAAA/MM/<id>-<nombre>.
 * El id lo genera el servidor para que el cliente no pueda elegir la ruta.
 */
export function documentKey({ tenant, filename, id, date = new Date() }) {
  assertTenant(tenant);
  if (typeof id !== "string" || !/^[a-f0-9-]{8,}$/i.test(id)) {
    throw new Error("id inválido");
  }
  const safe = sanitizeFilename(filename);
  const year = String(date.getUTCFullYear());
  const month = String(date.getUTCMonth() + 1).padStart(2, "0");
  return `${tenant}/${year}/${month}/${id}-${safe}`;
}
