import test from "node:test";
import assert from "node:assert/strict";
import { execFileSync } from "node:child_process";
import fs from "node:fs";
import path from "node:path";
import { build } from "../src/lab.mjs";

function git(args, cwd) {
  return execFileSync("git", args, {
    cwd,
    encoding: "utf8",
    env: { ...process.env, GIT_CONFIG_NOSYSTEM: "1", HOME: process.env.TEMP },
  });
}

test("conflicto: deja un merge sin resolver con marcadores", () => {
  const r = build("conflicto");
  const status = git(["status", "--porcelain"], r.dir);
  assert.match(status, /^UU /m);
  assert.ok(fs.readFileSync(path.join(r.dir, "notas.md"), "utf8").includes("<<<<<<<"));
});

test("historia: varios commits con mensajes pobres", () => {
  const r = build("historia");
  assert.ok(r.commits >= 6);
  assert.match(git(["log", "--format=%s"], r.dir), /^wip$/m);
});

test("perdido: el commit solo vive en el reflog", () => {
  const r = build("perdido");
  const short = r.lostCommit.slice(0, 7);
  assert.ok(!git(["log", "--oneline", "--all"], r.dir).includes(short));
  const reflog = git(["reflog", "--format=%H %gs"], r.dir);
  assert.ok(reflog.includes("informe importante"));
});

test("bisect: el primer commit malo es detectable", () => {
  const r = build("bisect");
  assert.equal(git(["show", `${r.firstBad}:estado.txt`], r.dir).trim(), "malo");
  assert.equal(git(["show", `${r.firstBad}~1:estado.txt`], r.dir).trim(), "bueno");
});

test("equipo: los clones divergen respecto a origin", () => {
  const r = build("equipo");
  const counts = git(["rev-list", "--left-right", "--count", "main...origin/main"], r.dir).trim();
  const [ahead, behind] = counts.split(/\s+/).map(Number);
  assert.ok(ahead >= 1 && behind >= 1, `esperaba divergencia, obtuve ${counts}`);
});

test("auditoria: binario y node_modules versionados", () => {
  const r = build("auditoria");
  const files = git(["ls-files"], r.dir);
  assert.match(files, /node_modules\/dep\/index\.js/);
  assert.match(files, /logo\.png/);
  assert.match(git(["log", "--format=%s"], r.dir), /^update$/m);
});
