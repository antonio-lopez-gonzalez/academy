/**
 * Topología del taller.
 *
 *   API ──job.run──▶ taller.jobs (direct) ──▶ taller.jobs.work (quorum)
 *                                                   │
 *                                     fallo transitorio: publish + ack
 *                                                   ▼
 *                                        taller.jobs.retry (direct)
 *                                   retry.5s | retry.1m | retry.10m
 *                                        (TTL, sin consumidores)
 *                                                   │
 *                                     al expirar → taller.jobs / job.run
 *
 *   fallo permanente o intentos agotados
 *              └──▶ taller.dlx ──job.dead──▶ taller.jobs.dead (quorum)
 */
export const EXCHANGE = "taller.jobs";
export const RETRY_EXCHANGE = "taller.jobs.retry";
export const DLX = "taller.dlx";

export const WORK_QUEUE = "taller.jobs.work";
export const DEAD_QUEUE = "taller.jobs.dead";
export const JOB_ROUTING_KEY = "job.run";
export const DEAD_ROUTING_KEY = "job.dead";

export const RETRY_TIERS = [
  { tier: "5s", ttl: 5_000 },
  { tier: "1m", ttl: 60_000 },
  { tier: "10m", ttl: 600_000 },
];

/**
 * Declara exchanges, colas y bindings. Es idempotente: se ejecuta en cada
 * arranque de la API y del worker. Los argumentos de una cola son inmutables.
 */
export async function declareTopology(channel) {
  await channel.assertExchange(EXCHANGE, "direct", { durable: true });
  await channel.assertExchange(RETRY_EXCHANGE, "direct", { durable: true });
  await channel.assertExchange(DLX, "direct", { durable: true });

  await channel.assertQueue(WORK_QUEUE, {
    durable: true,
    arguments: {
      "x-queue-type": "quorum",
      "x-delivery-limit": 5,
      "x-dead-letter-exchange": DLX,
      "x-dead-letter-routing-key": DEAD_ROUTING_KEY,
    },
  });
  await channel.bindQueue(WORK_QUEUE, EXCHANGE, JOB_ROUTING_KEY);

  for (const { tier, ttl } of RETRY_TIERS) {
    const queue = `${RETRY_EXCHANGE}.${tier}`;
    await channel.assertQueue(queue, {
      durable: true,
      arguments: {
        "x-queue-type": "quorum",
        "x-message-ttl": ttl,
        "x-dead-letter-exchange": EXCHANGE,
        "x-dead-letter-routing-key": JOB_ROUTING_KEY,
      },
    });
    await channel.bindQueue(queue, RETRY_EXCHANGE, `retry.${tier}`);
  }

  await channel.assertQueue(DEAD_QUEUE, {
    durable: true,
    arguments: { "x-queue-type": "quorum" },
  });
  await channel.bindQueue(DEAD_QUEUE, DLX, DEAD_ROUTING_KEY);
}
