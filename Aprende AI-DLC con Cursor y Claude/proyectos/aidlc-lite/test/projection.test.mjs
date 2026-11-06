import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs";
import os from "node:os";
import path from "node:path";
import {
  END,
  START,
  hasManagedBlock,
  projectAll,
  projectClaude,
  projectCursor,
  replaceManagedBlock,
} from "../src/projection.mjs";

function tempDir() {
  return fs.mkdtempSync(path.join(os.tmpdir(), "aidlc-projection-"));
}

test("projectClaude crea el archivo con el bloque gestionado", () => {
  const dir = tempDir();
  const file = projectClaude(dir, "# Reglas\n- una regla");
  assert.ok(fs.existsSync(file));
  const text = fs.readFileSync(file, "utf8");
  assert.ok(text.includes(START));
  assert.ok(text.includes(END));
  assert.ok(text.includes("una regla"));
  assert.equal(hasManagedBlock(file), true);
});

test("projectClaude es idempotente y conserva el contenido del usuario", () => {
  const dir = tempDir();
  const file = path.join(dir, ".claude", "CLAUDE.md");
  fs.mkdirSync(path.dirname(file), { recursive: true });
  fs.writeFileSync(file, "# Mi proyecto\n\nContenido propio.\n", "utf8");

  projectClaude(dir, "v1");
  projectClaude(dir, "v2");

  const text = fs.readFileSync(file, "utf8");
  assert.match(text, /Contenido propio/);
  assert.match(text, /v2/);
  assert.doesNotMatch(text, /v1/);
  assert.equal(text.split(START).length, 2);
});

test("projectCursor escribe frontmatter y bloque", () => {
  const dir = tempDir();
  const file = projectCursor(dir, "# Reglas de equipo");
  const text = fs.readFileSync(file, "utf8");
  assert.match(text, /alwaysApply: true/);
  assert.ok(text.includes(START));
  assert.ok(text.includes(END));
});

test("replaceManagedBlock sustituye solo el bloque existente", () => {
  const original = `antes\n${START}\nviejo\n${END}\ndespués\n`;
  const result = replaceManagedBlock(original, `${START}\nnuevo\n${END}`);
  assert.match(result, /antes/);
  assert.match(result, /nuevo/);
  assert.match(result, /después/);
  assert.doesNotMatch(result, /viejo/);
});

test("projectAll proyecta a los harnesses indicados", () => {
  const dir = tempDir();
  const written = projectAll(dir, "# Reglas", ["claude", "cursor"]);
  assert.equal(written.length, 2);
  assert.ok(fs.existsSync(path.join(dir, ".claude", "CLAUDE.md")));
  assert.ok(fs.existsSync(path.join(dir, ".cursor", "rules", "aidlc-team.mdc")));
});
