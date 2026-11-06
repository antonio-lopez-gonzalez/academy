/**
 * audit.mjs — registro append-only de eventos en JSONL.
 *
 * La auditoría no se reescribe: se añade. Así se puede reconstruir qué pasó
 * aunque el estado actual se haya modificado después.
 */
import fs from "node:fs";
import path from "node:path";

export function auditPath(dir) {
  return path.join(dir, ".aidlc-lite", "audit.jsonl");
}

export function appendAudit(dir, event, now = new Date()) {
  const file = auditPath(dir);
  fs.mkdirSync(path.dirname(file), { recursive: true });
  const line = JSON.stringify({ at: now.toISOString(), ...event });
  fs.appendFileSync(file, `${line}\n`, "utf8");
  return line;
}

export function readAudit(dir) {
  const file = auditPath(dir);
  if (!fs.existsSync(file)) return [];
  return fs
    .readFileSync(file, "utf8")
    .split(/\r?\n/)
    .filter(Boolean)
    .map((line) => {
      try {
        return JSON.parse(line);
      } catch {
        return { at: null, type: "audit.parse_error", raw: line };
      }
    });
}
