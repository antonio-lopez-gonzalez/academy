/**
 * gates.mjs — política de decisiones de un gate.
 *
 * Un gate no es un booleano: el humano puede aprobar, pedir cambios, saltar
 * o rechazar. En este motor didáctico, aprobar y saltar avanzan; pedir cambios
 * y rechazar mantienen la etapa activa. La diferencia queda en la auditoría.
 */
import { approveCurrent, rejectCurrent } from "./state.mjs";

export const GATE_DECISIONS = ["approve", "request_changes", "skip", "reject"];

const ADVANCE = new Set(["approve", "skip"]);
const HOLD = new Set(["request_changes", "reject"]);

export function applyGate(state, decision, { note = "", now = new Date() } = {}) {
  if (ADVANCE.has(decision)) {
    return { state: approveCurrent(state, { note, now }), advanced: true };
  }
  if (HOLD.has(decision)) {
    return { state: rejectCurrent(state, { note, now }), advanced: false };
  }
  throw new Error(
    `decisión de gate desconocida: ${decision}. Válidas: ${GATE_DECISIONS.join(", ")}`
  );
}

export { approveCurrent, rejectCurrent };
