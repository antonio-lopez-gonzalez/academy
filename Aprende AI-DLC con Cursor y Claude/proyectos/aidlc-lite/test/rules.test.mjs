import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs";
import os from "node:os";
import path from "node:path";
import { loadRules, mergeRules, validateRules, writeDefaultRules } from "../src/rules.mjs";

function tempDir() {
  return fs.mkdtempSync(path.join(os.tmpdir(), "aidlc-rules-"));
}

test("mergeRules conserva el orden y anota la fuente", () => {
  const merged = mergeRules([
    { file: "org.md", text: "# Org\n- a" },
    { file: "team.md", text: "# Team\n- b" },
  ]);
  assert.ok(merged.indexOf("org.md") < merged.indexOf("team.md"));
  assert.match(merged, /source: org\.md/);
  assert.match(merged, /source: team\.md/);
});

test("writeDefaultRules crea las tres capas sin sobrescribir", () => {
  const dir = tempDir();
  writeDefaultRules(dir);
  assert.deepEqual(validateRules(dir), []);

  const project = path.join(dir, "rules", "project.md");
  fs.writeFileSync(project, "# Regla propia\n- no tocar\n", "utf8");
  writeDefaultRules(dir);
  assert.match(fs.readFileSync(project, "utf8"), /no tocar/);
});

test("loadRules fusiona solo los archivos presentes", () => {
  const dir = tempDir();
  writeDefaultRules(dir);
  fs.rmSync(path.join(dir, "rules", "team.md"));
  const { sources, merged } = loadRules(dir);
  assert.deepEqual(
    sources.map((source) => source.file),
    ["org.md", "project.md"]
  );
  assert.match(merged, /Reglas del proyecto/);
});

test("validateRules detecta un directorio incompleto", () => {
  const dir = tempDir();
  assert.match(validateRules(dir).join(" "), /no existe/);
});
