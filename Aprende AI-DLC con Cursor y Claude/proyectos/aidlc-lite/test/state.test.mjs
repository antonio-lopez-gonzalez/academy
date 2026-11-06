import test from "node:test";
import assert from "node:assert/strict";
import {
  approveCurrent,
  createIntent,
  currentStage,
  progress,
  rejectCurrent,
  validateState,
} from "../src/state.mjs";

test("crea una intención con la primera etapa en curso", () => {
  const state = createIntent({ profile: "classic", intent: "API de inventario" });
  assert.equal(state.status, "in_progress");
  assert.equal(state.currentStage, 0);
  assert.equal(currentStage(state).id, "intent");
  assert.equal(state.stages[0].status, "in_progress");
  assert.equal(state.stages[1].status, "pending");
  assert.deepEqual(validateState(state), []);
});

test("crear una intención vacía falla", () => {
  assert.throws(() => createIntent({ profile: "classic", intent: "   " }), /vacía/);
});

test("aprobar avanza a la siguiente etapa", () => {
  const state = createIntent({ profile: "express", intent: "demo" });
  approveCurrent(state, { note: "ok" });
  assert.equal(state.currentStage, 1);
  assert.equal(state.stages[0].status, "approved");
  assert.equal(state.stages[1].status, "in_progress");
  assert.equal(state.history.length, 1);
  assert.equal(state.history[0].type, "gate.approved");
});

test("aprobar la última etapa completa la intención", () => {
  const state = createIntent({ profile: "poc", intent: "demo" });
  for (let index = 0; index < state.stages.length; index += 1) {
    approveCurrent(state);
  }
  assert.equal(state.status, "completed");
  assert.equal(state.currentStage, state.stages.length - 1);
  assert.equal(progress(state).done, state.stages.length);
});

test("rechazar mantiene la etapa activa y registra el motivo", () => {
  const state = createIntent({ profile: "feature", intent: "demo" });
  rejectCurrent(state, { note: "faltan NFRs" });
  assert.equal(state.status, "in_progress");
  assert.equal(state.currentStage, 0);
  assert.equal(state.history[0].type, "gate.rejected");
  assert.equal(state.history[0].note, "faltan NFRs");
});

test("no se puede aprobar una intención completada", () => {
  const state = createIntent({ profile: "poc", intent: "demo" });
  for (let index = 0; index < state.stages.length; index += 1) {
    approveCurrent(state);
  }
  assert.throws(() => approveCurrent(state), /completada/);
});

test("validateState detecta currentStage fuera de rango", () => {
  const state = createIntent({ profile: "poc", intent: "demo" });
  state.currentStage = 99;
  assert.match(validateState(state).join(" "), /fuera de rango/);
});
