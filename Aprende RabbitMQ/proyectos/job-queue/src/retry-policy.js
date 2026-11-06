/**
 * Política de reintentos pura, sin broker ni almacén.
 * Se prueba con `node --test` en milisegundos.
 */
export const TIERS = [
  { attempt: 1, routingKey: "retry.5s", delayMs: 5_000 },
  { attempt: 2, routingKey: "retry.1m", delayMs: 60_000 },
  { attempt: 3, routingKey: "retry.10m", delayMs: 600_000 },
];

/**
 * Traduce «intento actual + clasificación del error» en una acción.
 * - error.permanent → DLQ inmediata.
 * - sin siguiente nivel → DLQ por intentos agotados.
 * - resto → reintento en la cola del nivel correspondiente.
 */
export function planRetry(attempt, error) {
  if (error?.permanent) return { action: "dead", reason: "permanent" };

  const current = Number(attempt);
  if (!Number.isInteger(current) || current < 0) {
    return { action: "dead", reason: "invalid_attempt" };
  }

  const next = current + 1;
  const tier = TIERS.find((candidate) => candidate.attempt === next);
  if (!tier) return { action: "dead", reason: "attempts_exhausted" };

  return { action: "retry", ...tier };
}
