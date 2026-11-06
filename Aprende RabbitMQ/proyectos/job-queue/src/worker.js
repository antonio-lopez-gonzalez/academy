import amqp from "amqplib";
import { config } from "./config.js";
import { JobStore } from "./store.js";
import { planRetry } from "./retry-policy.js";
import {
  DEAD_ROUTING_KEY,
  DLX,
  RETRY_EXCHANGE,
  WORK_QUEUE,
  declareTopology,
} from "./topology.js";

const store = new JobStore(config.storePath);
const sleep = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

let connection = null;
let channel = null;
let confirm = null;
let consumerTag = null;
let stopping = false;
const inFlight = new Set();

/** Simula el trabajo real. El payload permite forzar fallos para la demo. */
async function processJob(job) {
  await sleep(200 + Math.floor(Math.random() * 300));

  if (job.payload?.failPermanent === true) {
    const error = new Error("fallo permanente simulado por payload.failPermanent");
    error.permanent = true;
    throw error;
  }
  if (job.payload?.fail === true) {
    throw new Error("fallo transitorio simulado por payload.fail");
  }

  return {
    processedAt: new Date().toISOString(),
    workerPid: process.pid,
  };
}

function retryHeaders(msg, attempt, error) {
  return {
    ...msg.properties.headers,
    "x-attempt": attempt,
    "x-error": String(error?.message ?? "error desconocido").slice(0, 500),
    "x-failed-at": new Date().toISOString(),
  };
}

async function publishRetry(msg, job, plan, attempt, error) {
  confirm.publish(RETRY_EXCHANGE, plan.routingKey, msg.content, {
    persistent: true,
    messageId: job.jobId,
    contentType: "application/json",
    headers: retryHeaders(msg, plan.attempt, error),
  });
  await confirm.waitForConfirms();
  await store.markRetry(job.jobId, plan.attempt, plan.delayMs, error.message);
  channel.ack(msg);
  console.log(
    JSON.stringify({
      event: "job.retry_scheduled",
      jobId: job.jobId,
      attempt: plan.attempt,
      delayMs: plan.delayMs,
      traceId: job.traceId,
    }),
  );
}

async function publishDead(msg, job, plan, attempt, error) {
  confirm.publish(DLX, DEAD_ROUTING_KEY, msg.content, {
    persistent: true,
    messageId: job.jobId,
    contentType: "application/json",
    headers: {
      ...retryHeaders(msg, attempt, error),
      "x-dead-reason": plan.reason,
    },
  });
  await confirm.waitForConfirms();
  await store.markFailed(job.jobId, plan.reason, error.message);
  channel.ack(msg);
  console.log(
    JSON.stringify({
      event: "job.failed",
      jobId: job.jobId,
      attempt,
      reason: plan.reason,
      traceId: job.traceId,
    }),
  );
}

async function handle(msg) {
  if (!msg) return;

  let job;
  try {
    job = JSON.parse(msg.content.toString());
  } catch {
    confirm.publish(DLX, DEAD_ROUTING_KEY, msg.content, {
      persistent: true,
      contentType: "text/plain",
      headers: {
        "x-attempt": 0,
        "x-dead-reason": "invalid_message",
        "x-error": "JSON inválido",
      },
    });
    await confirm.waitForConfirms();
    channel.ack(msg);
    console.error(JSON.stringify({ event: "job.invalid", reason: "invalid_message" }));
    return;
  }

  const claimed = await store.claim(job.jobId, { leaseMs: config.leaseMs });
  if (!claimed) {
    channel.ack(msg);
    console.log(JSON.stringify({ event: "job.skipped", jobId: job.jobId, reason: "not_claimable" }));
    return;
  }

  const attempt = Number(msg.properties.headers?.["x-attempt"] ?? 0);

  try {
    const result = await processJob(claimed);
    await store.markSucceeded(job.jobId, result);
    channel.ack(msg);
    console.log(JSON.stringify({ event: "job.succeeded", jobId: job.jobId, attempt, traceId: job.traceId }));
  } catch (error) {
    const plan = planRetry(attempt, error);
    try {
      if (plan.action === "retry") {
        await publishRetry(msg, job, plan, attempt, error);
      } else {
        await publishDead(msg, job, plan, attempt, error);
      }
    } catch (publishError) {
      console.error(
        JSON.stringify({
          event: "job.publish_failed",
          jobId: job.jobId,
          error: publishError.message,
        }),
      );
      // Sin confirm no se hace ack: el original vuelve a la cola de trabajo.
      channel.nack(msg, false, true);
    }
  }
}

async function run() {
  while (!stopping) {
    try {
      connection = await amqp.connect(config.url);
      connection.on("error", (error) =>
        console.error(JSON.stringify({ event: "amqp.connection.error", error: error.message })),
      );
      const closed = new Promise((resolve) => {
        connection.once("close", resolve);
      });

      channel = await connection.createChannel();
      confirm = await connection.createConfirmChannel();
      channel.on("error", (error) =>
        console.error(JSON.stringify({ event: "amqp.channel.error", error: error.message })),
      );
      confirm.on("error", (error) =>
        console.error(JSON.stringify({ event: "amqp.confirm.error", error: error.message })),
      );

      await declareTopology(channel);
      await channel.prefetch(config.prefetch);

      ({ consumerTag } = await channel.consume(
        WORK_QUEUE,
        (msg) => {
          const task = handle(msg).catch((error) => {
            console.error(JSON.stringify({ event: "handler.crashed", error: error.message }));
            try {
              channel.nack(msg, false, true);
            } catch {
              /* canal cerrado */
            }
          });
          inFlight.add(task);
          task.finally(() => inFlight.delete(task));
        },
        { noAck: false },
      ));

      console.log(JSON.stringify({ event: "worker.listening", queue: WORK_QUEUE, prefetch: config.prefetch }));
      await closed;
    } catch (error) {
      console.error(JSON.stringify({ event: "worker.connect_failed", error: error.message }));
    }

    connection = null;
    channel = null;
    confirm = null;
    consumerTag = null;

    if (!stopping) {
      await sleep(3000);
    }
  }
}

async function shutdown(signal) {
  if (stopping) return;
  stopping = true;
  console.log(JSON.stringify({ event: "worker.shutdown", signal }));

  try {
    if (channel && consumerTag) await channel.cancel(consumerTag);
    await Promise.allSettled([...inFlight]);
    await channel?.close().catch(() => {});
    await confirm?.close().catch(() => {});
    await connection?.close().catch(() => {});
  } catch (error) {
    console.error(JSON.stringify({ event: "worker.shutdown_error", error: error.message }));
  }
  process.exitCode = 0;
}

process.on("SIGINT", () => void shutdown("SIGINT"));
process.on("SIGTERM", () => void shutdown("SIGTERM"));

await run();
