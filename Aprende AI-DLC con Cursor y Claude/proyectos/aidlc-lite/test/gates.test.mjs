import test from "node:test";
import assert from "node:assert/strict";
import { applyGate, GATE_DECISIONS } from "../src/gates.mjs";
import { createIntent } from "../src/state.mjs";

test("approve avanza la etapa", () => {
  const state = createIntent({ profile: "express", intent: "demo" });
  const result = applyGate(state, "approve", { note: "ok" });
  assert.equal(result.advanced, true);
  assert.equal(state.currentStage, 1);
});

test("skip también avanza y queda registrado", () => {
  const state = createIntent({ profile: "express", intent: "demo" });
  const result = applyGate(state, "skip", { note: "no aplica" });
  assert.equal(result.advanced, true);
  assert.equal(state.history[0].type, "gate.approved");
});

test("request_changes mantiene la etapa activa", () => {
  const state = createIntent({ profile: "express", intent: "demo" });
  const result = applyGate(state, "request_changes", { note: "faltan datos" });
  assert.equal(result.advanced, false);
  assert.equal(state.currentStage, 0);
  assert.equal(state.history[0].type, "gate.rejected");
});

test("reject mantiene la etapa activa", () => {
  const state = createIntent({ profile: "express", intent: "demo" });
  const result = applyGate(state, "reject", { note: "no va" });
  assert.equal(result.advanced, false);
  assert.equal(state.currentStage, 0);
});

test("una decisión desconocida falla", () => {
  const state = createIntent({ profile: "express", intent: "demo" });
  assert.throws(() => applyGate(state, "maybe"), /desconocida/);
  assert.deepEqual(GATE_DECISIONS, ["approve", "request_changes", "skip", "reject"]);
});
