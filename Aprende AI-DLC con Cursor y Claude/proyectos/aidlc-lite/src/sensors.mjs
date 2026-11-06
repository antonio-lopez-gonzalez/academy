/**
 * sensors.mjs — sensores deterministas sobre artefactos Markdown.
 *
 * Un sensor no llama a un modelo: aplica una regla fija. Si el artefacto no
 * cumple, devuelve un hallazgo. Es advisory por defecto; el motor del curso
 * podría hacerlo bloqueante en un gate.
 */
import fs from "node:fs";

export const DEFAULT_REQUIRED_SECTIONS = ["Contexto", "Requisitos", "Riesgos"];

function headings(text) {
  return new Set(
    text
      .split(/\r?\n/)
      .map((line) => line.match(/^#{1,3}\s+(.+?)\s*$/))
      .filter(Boolean)
      .map((match) => match[1].trim().toLowerCase())
  );
}

export function scanMarkdown(text, { requiredSections = DEFAULT_REQUIRED_SECTIONS } = {}) {
  const found = headings(text);
  const findings = [];

  for (const section of requiredSections) {
    if (!found.has(section.toLowerCase())) {
      findings.push({
        code: "missing-section",
        severity: "warning",
        message: `falta la sección "## ${section}"`,
      });
    }
  }

  if (/TODO|FIXME|TBD/.test(text)) {
    findings.push({
      code: "placeholder",
      severity: "warning",
      message: "el artefacto contiene TODO, FIXME o TBD",
    });
  }

  if (text.trim().length < 80) {
    findings.push({
      code: "too-short",
      severity: "warning",
      message: "el artefacto es demasiado corto para revisarlo",
    });
  }

  return { ok: findings.length === 0, findings };
}

export function runSensors(file, options = {}) {
  const text = fs.readFileSync(file, "utf8");
  return scanMarkdown(text, options);
}
