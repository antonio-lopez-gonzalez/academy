import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs";
import os from "node:os";
import path from "node:path";
import { execFileSync } from "node:child_process";
import { fileURLToPath } from "node:url";

const projectRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const cli = path.join(projectRoot, "src", "cli.mjs");

function run(args, cwd = projectRoot) {
  return execFileSync(process.execPath, [cli, ...args], {
    cwd,
    encoding: "utf8",
    stdio: ["ignore", "pipe", "pipe"],
  });
}

function tempDir() {
  return fs.mkdtempSync(path.join(os.tmpdir(), "aidlc-cli-"));
}

test("el CLI muestra la ayuda sin fallar", () => {
  const output = run(["--help"]);
  assert.match(output, /aidlc-lite/);
  assert.match(output, /init/);
});

test("el CLI lista los perfiles", () => {
  const output = run(["profiles"]);
  assert.match(output, /classic/);
  assert.match(output, /security-patch/);
});

test("flujo init -> run -> status -> approve -> doctor", () => {
  const dir = tempDir();
  run(["init", "--dir", dir, "--harness", "both"]);
  run(["run", "--dir", dir, "--profile", "express", "--intent", "demo"]);
  assert.match(run(["status", "--dir", dir]), /intent — Intent capture/);
  run(["approve", "--dir", dir, "--note", "ok"]);
  assert.match(run(["status", "--dir", dir]), /build — Code generation/);
  assert.match(run(["doctor", "--dir", dir]), /0 problemas/);
});

test("el CLI devuelve error con un perfil desconocido", () => {
  const dir = tempDir();
  run(["init", "--dir", dir]);
  assert.throws(() =>
    execFileSync(process.execPath, [cli, "run", "--dir", dir, "--profile", "nope", "--intent", "x"], {
      cwd: projectRoot,
      encoding: "utf8",
      stdio: ["ignore", "pipe", "pipe"],
    })
  );
});
