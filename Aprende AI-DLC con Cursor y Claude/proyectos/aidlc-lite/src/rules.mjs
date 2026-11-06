/**
 * rules.mjs — reglas por capas: organización, equipo y proyecto.
 *
 * El orden importa: project puede refinar team, y team puede refinar org.
 * La fusión es texto plano con encabezados para que sea legible y diffeable.
 */
import fs from "node:fs";
import path from "node:path";

export const RULE_FILES = ["org.md", "team.md", "project.md"];

export const DEFAULT_RULES = {
  "org.md": `# Reglas de la organización

- Todo cambio pasa por un gate humano.
- Ningún agente se autoaprueba.
- Los secretos no se escriben en artefactos ni en reglas.
`,
  "team.md": `# Reglas del equipo

- Una intención, un responsable humano.
- Los artefactos se revisan antes de aprobar.
- Las decisiones se registran en el audit trail.
`,
  "project.md": `# Reglas del proyecto

- Node.js 22, ESM y sin dependencias en aidlc-lite.
- Los tests se ejecutan con \`node --test\`.
- La proyección a Cursor y Claude se regenera con \`sync\`.
`,
};

export function rulesDir(dir) {
  return path.join(dir, "rules");
}

export function loadRules(dir) {
  const sources = [];
  for (const file of RULE_FILES) {
    const full = path.join(rulesDir(dir), file);
    if (!fs.existsSync(full)) continue;
    sources.push({ file, text: fs.readFileSync(full, "utf8").trim() });
  }
  return { sources, merged: mergeRules(sources) };
}

export function mergeRules(sources) {
  return sources
    .map(({ file, text }) => `<!-- source: ${file} -->\n${text}`)
    .join("\n\n");
}

export function writeDefaultRules(dir) {
  fs.mkdirSync(rulesDir(dir), { recursive: true });
  for (const file of RULE_FILES) {
    const full = path.join(rulesDir(dir), file);
    if (!fs.existsSync(full)) {
      fs.writeFileSync(full, DEFAULT_RULES[file], "utf8");
    }
  }
}

export function validateRules(dir) {
  const errors = [];
  if (!fs.existsSync(rulesDir(dir))) {
    return ["no existe el directorio rules/"];
  }
  for (const file of RULE_FILES) {
    const full = path.join(rulesDir(dir), file);
    if (!fs.existsSync(full)) errors.push(`falta rules/${file}`);
  }
  return errors;
}
