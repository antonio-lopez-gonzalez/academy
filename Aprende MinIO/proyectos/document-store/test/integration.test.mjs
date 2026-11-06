import test from "node:test";
import assert from "node:assert/strict";
import crypto from "node:crypto";

const BASE = process.env.BASE_URL;

test("crear, subir por presigned URL y completar", { skip: !BASE }, async () => {
  const tenant = `test-${crypto.randomUUID().slice(0, 8)}`;
  const create = await fetch(`${BASE}/documents`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      tenant,
      filename: "prueba.pdf",
      contentType: "application/pdf",
      size: 4,
    }),
  });

  assert.equal(create.status, 201);
  const doc = await create.json();
  assert.ok(doc.uploadUrl);
  assert.match(doc.key, new RegExp(`^${tenant}/\\d{4}/\\d{2}/`));

  const put = await fetch(doc.uploadUrl, {
    method: "PUT",
    headers: { "Content-Type": "application/pdf" },
    body: Buffer.from("%PDF"),
  });
  assert.equal(put.status, 200);

  const complete = await fetch(`${BASE}/documents/${doc.id}/complete`, { method: "POST" });
  assert.equal(complete.status, 200);
  assert.equal((await complete.json()).status, "stored");
});
