import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs";
import os from "node:os";
import path from "node:path";
import { readAudit } from "../src/audit.mjs";
import {
  approve,
  doctor,
  initProject,
  reject,
  startIntent,
  status,
  syncProjections,
} from "../src/engine.mjs";

function tempDir() {
  return fs.mkdtempSync(path.join(os.tmpdir(), "aidlc-engine-"));
}

test("init crea config, reglas y proyecciones", () => {
  const dir = tempDir();
  const result = initProject(dir, { harnesses: "both" });
  assert.deepEqual(result.config.harnesses, ["claude", "cursor"]);
  assert.ok(fs.existsSync(path.join(dir, ".aidlc-lite", "config.json")));
  assert.ok(fs.existsSync(path.join(dir, "rules", "org.md")));
  assert.ok(fs.existsSync(path.join(dir, ".claude", "CLAUDE.md")));
  assert.ok(fs.existsSync(path.join(dir, ".cursor", "rules", "aidlc-team.mdc")));
});

test("run crea una intención y status la muestra", () => {
  const dir = tempDir();
  initProject(dir, { harnesses: "claude" });
  startIntent(dir, { profile: "feature", intent: "API de inventario" });
  const current = status(dir);
  assert.equal(current.profile, "feature");
  assert.equal(current.stage, "intent");
  assert.equal(current.done, 0);
  assert.equal(current.total, 7);
  assert.equal(current.pendingGate, true);
});

test("approve avanza y reject registra el motivo", () => {
  const dir = tempDir();
  initProject(dir, { harnesses: "claude" });
  startIntent(dir, { profile: "express", intent: "demo" });

  approve(dir, { note: "ok" });
  assert.equal(status(dir).stage, "build");

  reject(dir, { note: "faltan pruebas" });
  assert.equal(status(dir).stage, "build");

  const events = readAudit(dir);
  assert.ok(events.some((event) => event.type === "gate.approved"));
  assert.ok(events.some((event) => event.type === "gate.rejected" && event.note === "faltan pruebas"));
});

test("sync reprojecta las reglas y conserva las fuentes", () => {
  const dir = tempDir();
  initProject(dir, { harnesses: "both" });
  const result = syncProjections(dir);
  assert.equal(result.sources.length, 3);
  assert.equal(result.written.length, 2);
});

test("doctor pasa en un proyecto inicializado", () => {
  const dir = tempDir();
  initProject(dir, { harnesses: "both" });
  startIntent(dir, { profile: "classic", intent: "demo" });
  const report = doctor(dir);
  assert.equal(report.ok, true);
  assert.deepEqual(report.problems, []);
  assert.ok(report.auditEvents >= 2);
});

test("doctor detecta un proyecto sin inicializar", () => {
  const dir = tempDir();
  const report = doctor(dir);
  assert.equal(report.ok, false);
  assert.ok(report.problems.some((problem) => problem.includes("config.json")));
});
