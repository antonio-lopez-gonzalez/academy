import amqp from "amqplib";
import { declareTopology, EXCHANGE, JOB_ROUTING_KEY } from "./topology.js";

const sleep = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

/**
 * Publicador de la API con reconexión.
 *
 * Si el broker está caído, `publishJob` lanza y la API responde 503. Un bucle
 * en segundo plano reintenta la conexión; mientras tanto los jobs quedan
 * persistidos como `queued` y el sweeper los republica.
 */
export function createPublisher(config) {
  let connection = null;
  let channel = null;
  let connecting = null;
  let closed = false;

  async function connect() {
    if (channel) return;
    if (connecting) return connecting;

    connecting = (async () => {
      const conn = await amqp.connect(config.url);
      const ch = await conn.createConfirmChannel();
      await declareTopology(ch);

      conn.on("error", (error) => console.error(JSON.stringify({ event: "amqp.connection.error", error: error.message })));
      ch.on("error", (error) => console.error(JSON.stringify({ event: "amqp.channel.error", error: error.message })));
      conn.on("close", () => {
        if (connection === conn) {
          connection = null;
          channel = null;
          if (!closed) void retryLoop();
        }
      });

      connection = conn;
      channel = ch;
      console.log(JSON.stringify({ event: "broker.connected" }));
    })().finally(() => {
      connecting = null;
    });

    return connecting;
  }

  async function retryLoop() {
    while (!closed && !channel) {
      try {
        await connect();
      } catch (error) {
        console.error(JSON.stringify({ event: "broker.connect_failed", error: error.message }));
        await sleep(3000);
      }
    }
  }

  return {
    async start() {
      try {
        await connect();
      } catch (error) {
        console.error(JSON.stringify({ event: "broker.connect_failed", error: error.message }));
        void retryLoop();
      }
    },

    isReady() {
      return Boolean(channel);
    },

    async publishJob(job) {
      if (!channel) throw new Error("broker no disponible");

      const body = Buffer.from(
        JSON.stringify({
          jobId: job.jobId,
          type: job.type,
          schemaVersion: 1,
          payload: job.payload,
          enqueuedAt: job.createdAt,
          traceId: job.traceId,
        }),
      );

      channel.publish(EXCHANGE, JOB_ROUTING_KEY, body, {
        persistent: true,
        messageId: job.jobId,
        contentType: "application/json",
        headers: { "x-attempt": 0, "x-trace-id": job.traceId },
      });
      await channel.waitForConfirms();
    },

    async close() {
      closed = true;
      await channel?.close().catch(() => {});
      await connection?.close().catch(() => {});
      channel = null;
      connection = null;
    },
  };
}
