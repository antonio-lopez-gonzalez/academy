/**
 * state.mjs — máquina de estados de una intención.
 *
 * Un intent avanza etapa a etapa. Cada etapa tiene un gate: el humano aprueba
 * o rechaza. El estado es un objeto serializable; la persistencia vive en
 * engine.mjs.
 */
import { getProfile } from "./profiles.mjs";

export function createIntent({ profile, intent, now = new Date() }) {
  const definition = getProfile(profile);
  if (!intent || !intent.trim()) {
    throw new Error("la intención no puede estar vacía");
  }

  const timestamp = now.toISOString();
  return {
    version: 1,
    intent: intent.trim(),
    profile,
    createdAt: timestamp,
    updatedAt: timestamp,
    currentStage: 0,
    status: "in_progress",
    stages: definition.stages.map((stage, index) => ({
      ...stage,
      status: index === 0 ? "in_progress" : "pending",
      approvedAt: null,
      note: null,
    })),
    history: [],
  };
}

function touch(state, now) {
  state.updatedAt = now.toISOString();
}

export function approveCurrent(state, { note = "", now = new Date() } = {}) {
  if (state.status === "completed") {
    throw new Error("la intención ya está completada");
  }

  const stage = state.stages[state.currentStage];
  if (!stage) throw new Error("no hay una etapa activa");

  stage.status = "approved";
  stage.approvedAt = now.toISOString();
  stage.note = note || null;

  state.history.push({
    at: now.toISOString(),
    type: "gate.approved",
    stage: stage.id,
    note: note || null,
  });

  state.currentStage += 1;
  if (state.currentStage >= state.stages.length) {
    state.currentStage = state.stages.length - 1;
    state.status = "completed";
    state.stages[state.currentStage].status = "approved";
  } else {
    state.stages[state.currentStage].status = "in_progress";
  }

  touch(state, now);
  return state;
}

export function rejectCurrent(state, { note = "", now = new Date() } = {}) {
  if (state.status === "completed") {
    throw new Error("la intención ya está completada");
  }
  const stage = state.stages[state.currentStage];
  if (!stage) throw new Error("no hay una etapa activa");

  stage.note = note || null;
  state.history.push({
    at: now.toISOString(),
    type: "gate.rejected",
    stage: stage.id,
    note: note || null,
  });

  touch(state, now);
  return state;
}

export function currentStage(state) {
  return state.stages[state.currentStage] ?? null;
}

export function progress(state) {
  const done = state.stages.filter((stage) => stage.status === "approved").length;
  return { done, total: state.stages.length };
}

export function validateState(state) {
  const errors = [];
  if (!state || typeof state !== "object") {
    return ["el estado no es un objeto"];
  }
  if (!state.intent) errors.push("falta intent");
  if (!state.profile) errors.push("falta profile");
  if (!Array.isArray(state.stages) || state.stages.length === 0) {
    errors.push("stages debe ser un array no vacío");
  }
  if (typeof state.currentStage !== "number") {
    errors.push("currentStage debe ser un número");
  }
  if (state.stages && state.currentStage >= state.stages.length) {
    errors.push("currentStage está fuera de rango");
  }
  return errors;
}
