import test from "node:test";
import assert from "node:assert/strict";
import { planRetry } from "../src/retry-policy.js";

test("el primer fallo transitorio reintenta a los 5 segundos", () => {
  assert.deepEqual(planRetry(0, new Error("timeout")), {
    action: "retry",
    attempt: 1,
    routingKey: "retry.5s",
    delayMs: 5000,
  });
});

test("el segundo fallo transitorio reintenta a 1 minuto", () => {
  assert.equal(planRetry(1, new Error("timeout")).routingKey, "retry.1m");
});

test("el tercer fallo transitorio reintenta a 10 minutos", () => {
  assert.equal(planRetry(2, new Error("timeout")).routingKey, "retry.10m");
});

test("los intentos agotados van a la DLQ", () => {
  assert.deepEqual(planRetry(3, new Error("timeout")), {
    action: "dead",
    reason: "attempts_exhausted",
  });
});

test("un error permanente no se reintenta", () => {
  const error = Object.assign(new Error("pedido inexistente"), { permanent: true });
  assert.deepEqual(planRetry(0, error), { action: "dead", reason: "permanent" });
});

test("un intento inválido no se reintenta", () => {
  assert.deepEqual(planRetry(-1, new Error("x")), { action: "dead", reason: "invalid_attempt" });
  assert.deepEqual(planRetry("x", new Error("x")), { action: "dead", reason: "invalid_attempt" });
});
