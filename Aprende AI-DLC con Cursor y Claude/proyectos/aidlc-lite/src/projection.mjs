/**
 * projection.mjs — proyecta las reglas fusionadas a cada harness.
 *
 * El bloque gestionado va entre marcadores. Todo lo que el usuario escriba
 * fuera de los marcadores se conserva: aidlc-lite solo es dueño de su bloque.
 */
import fs from "node:fs";
import path from "node:path";

export const START = "<!-- aidlc-lite:start -->";
export const END = "<!-- aidlc-lite:end -->";

function managedBlock(merged) {
  return `${START}\n${merged}\n${END}`;
}

export function replaceManagedBlock(existing, block) {
  const start = existing.indexOf(START);
  const end = existing.indexOf(END);
  if (start !== -1 && end !== -1 && end > start) {
    return existing.slice(0, start) + block + existing.slice(end + END.length);
  }
  const separator = existing.endsWith("\n") || existing === "" ? "" : "\n";
  return `${existing}${separator}\n${block}\n`;
}

export function projectClaude(dir, merged) {
  const file = path.join(dir, ".claude", "CLAUDE.md");
  fs.mkdirSync(path.dirname(file), { recursive: true });
  const existing = fs.existsSync(file) ? fs.readFileSync(file, "utf8") : "";
  const header = existing ? "" : "# Instrucciones del proyecto\n\n";
  fs.writeFileSync(file, replaceManagedBlock(existing || header, managedBlock(merged)), "utf8");
  return file;
}

export function projectCursor(dir, merged) {
  const file = path.join(dir, ".cursor", "rules", "aidlc-team.mdc");
  fs.mkdirSync(path.dirname(file), { recursive: true });
  const frontmatter =
    "---\ndescription: Reglas de equipo proyectadas por aidlc-lite\nalwaysApply: true\n---\n";
  const existing = fs.existsSync(file) ? fs.readFileSync(file, "utf8") : "";
  const body = existing || frontmatter;
  fs.writeFileSync(file, replaceManagedBlock(body, managedBlock(merged)), "utf8");
  return file;
}

export function projectAll(dir, merged, harnesses) {
  const written = [];
  if (harnesses.includes("claude")) written.push(projectClaude(dir, merged));
  if (harnesses.includes("cursor")) written.push(projectCursor(dir, merged));
  return written;
}

export function hasManagedBlock(file) {
  if (!fs.existsSync(file)) return false;
  const text = fs.readFileSync(file, "utf8");
  return text.includes(START) && text.includes(END);
}
