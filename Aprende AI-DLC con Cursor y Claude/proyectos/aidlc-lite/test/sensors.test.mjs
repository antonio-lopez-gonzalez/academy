import test from "node:test";
import assert from "node:assert/strict";
import { DEFAULT_REQUIRED_SECTIONS, scanMarkdown } from "../src/sensors.mjs";

const GOOD = `# Intent

## Contexto

Una API de inventario para el equipo de operaciones.

## Requisitos

- Listar stock por almacén.
- Reservar unidades.

## Riesgos

- Datos desactualizados.
`;

test("un artefacto completo no genera hallazgos", () => {
  const report = scanMarkdown(GOOD);
  assert.equal(report.ok, true);
  assert.deepEqual(report.findings, []);
});

test("detecta una sección requerida ausente", () => {
  const report = scanMarkdown(GOOD.replace("## Riesgos", "## Otros"));
  assert.equal(report.ok, false);
  assert.ok(report.findings.some((finding) => finding.code === "missing-section"));
});

test("detecta placeholders", () => {
  const report = scanMarkdown(`${GOOD}\nTODO: completar\n`);
  assert.ok(report.findings.some((finding) => finding.code === "placeholder"));
});

test("detecta artefactos demasiado cortos", () => {
  const report = scanMarkdown("# X\n", { requiredSections: [] });
  assert.ok(report.findings.some((finding) => finding.code === "too-short"));
});

test("DEFAULT_REQUIRED_SECTIONS incluye contexto, requisitos y riesgos", () => {
  assert.deepEqual(DEFAULT_REQUIRED_SECTIONS, ["Contexto", "Requisitos", "Riesgos"]);
});
