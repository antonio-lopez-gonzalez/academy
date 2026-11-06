import test from "node:test";
import assert from "node:assert/strict";
import crypto from "node:crypto";

const URL = process.env.RABBITMQ_URL;

test("publicar y consumir con ack borra el mensaje", { skip: !URL }, async (t) => {
  const { default: amqp } = await import("amqplib");
  const suffix = crypto.randomUUID().slice(0, 8);
  const queue = `test.jobs.${suffix}`;
  const connection = await amqp.connect(URL);
  const channel = await connection.createConfirmChannel();

  t.after(async () => {
    await channel.deleteQueue(queue).catch(() => {});
    await channel.close().catch(() => {});
    await connection.close().catch(() => {});
  });

  await channel.assertQueue(queue, { durable: false, autoDelete: true });
  channel.sendToQueue(queue, Buffer.from("hola"), { persistent: false });
  await channel.waitForConfirms();

  const message = await new Promise((resolve, reject) => {
    const timeout = setTimeout(() => reject(new Error("timeout esperando el mensaje")), 5_000);
    channel.consume(
      queue,
      (msg) => {
        if (!msg) return;
        clearTimeout(timeout);
        channel.ack(msg);
        resolve(msg);
      },
      { noAck: false },
    );
  });

  assert.equal(message.content.toString(), "hola");
  await new Promise((resolve) => setTimeout(resolve, 100));
  const state = await channel.checkQueue(queue);
  assert.equal(state.messageCount, 0);
});
