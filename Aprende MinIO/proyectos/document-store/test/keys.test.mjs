import test from "node:test";
import assert from "node:assert/strict";
import {
  ALLOWED_CONTENT_TYPES,
  assertContentType,
  assertSize,
  assertTenant,
  documentKey,
  sanitizeFilename,
} from "../src/keys.mjs";

test("assertTenant acepta tenants válidos y rechaza rutas", () => {
  assert.equal(assertTenant("acme"), "acme");
  assert.equal(assertTenant("acme-2026"), "acme-2026");
  assert.throws(() => assertTenant("../etc"), /tenant inválido/);
  assert.throws(() => assertTenant("ACME"), /tenant inválido/);
  assert.throws(() => assertTenant("a"), /tenant inválido/);
});

test("sanitizeFilename elimina rutas, acentos y caracteres peligrosos", () => {
  assert.equal(sanitizeFilename("../../etc/passwd"), "passwd");
  assert.equal(sanitizeFilename("Informe Año.pdf"), "Informe-Ano.pdf");
  assert.equal(sanitizeFilename("factura (copia).pdf"), "factura-copia.pdf");
  assert.throws(() => sanitizeFilename(".."), /filename inválido/);
  assert.throws(() => sanitizeFilename(""), /filename es obligatorio/);
});

test("assertContentType normaliza el charset y aplica la lista blanca", () => {
  assert.equal(assertContentType("application/pdf"), "application/pdf");
  assert.equal(assertContentType("text/plain; charset=utf-8"), "text/plain");
  assert.ok(ALLOWED_CONTENT_TYPES.has("application/pdf"));
  assert.throws(() => assertContentType("application/x-msdownload"), /no permitido/);
});

test("assertSize exige entero positivo y respeta el máximo", () => {
  assert.equal(assertSize(1024, 2048), 1024);
  assert.throws(() => assertSize(0, 2048), /entero positivo/);
  assert.throws(() => assertSize(4096, 2048), /supera el máximo/);
  assert.throws(() => assertSize(10.5, 2048), /entero positivo/);
});

test("documentKey genera una clave estable sin traversal", () => {
  const key = documentKey({
    tenant: "acme",
    filename: "../../Informe Año.pdf",
    id: "123e4567-e89b-12d3-a456-426614174000",
    date: new Date("2026-11-05T12:00:00Z"),
  });
  assert.equal(
    key,
    "acme/2026/11/123e4567-e89b-12d3-a456-426614174000-Informe-Ano.pdf",
  );
  assert.ok(!key.includes(".."));
});

test("documentKey valida id y tenant", () => {
  assert.throws(
    () => documentKey({ tenant: "acme", filename: "a.pdf", id: "x" }),
    /id inválido/,
  );
  assert.throws(
    () => documentKey({ tenant: "A", filename: "a.pdf", id: "123e4567-e89b" }),
    /tenant inválido/,
  );
});
