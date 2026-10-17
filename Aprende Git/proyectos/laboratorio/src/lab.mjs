#!/usr/bin/env node
/**
 * lab.mjs — laboratorio de escenarios de Git del curso "Aprende Git".
 *
 * Crea, en un directorio temporal FUERA del repositorio, pequeños repositorios
 * con situaciones reales (conflicto, historia sucia, commit perdido, bisect,
 * remoto divergente, repo desordenado) para practicar sin miedo.
 *
 * No necesita red y no toca el repositorio de la Academy.
 *
 * Uso:
 *   node src/lab.mjs list
 *   node src/lab.mjs <escenario>      conflicto | historia | perdido | bisect | equipo | auditoria
 *   node src/lab.mjs clean
 *
 * Programático (tests):
 *   import { build } from "./src/lab.mjs"; const r = build("conflicto");
 */

import { execFileSync } from "node:child_process";
import fs from "node:fs";
import os from "node:os";
import path from "node:path";

export const LAB_ROOT = path.join(os.tmpdir(), "aprende-git-lab");

const USER = { name: "Taller Git", email: "taller@example.com" };

/* ------------------------------------------------------------------ shell */

function git(args, cwd) {
  return execFileSync("git", args, {
    cwd,
    encoding: "utf8",
    stdio: ["ignore", "pipe", "pipe"],
    env: {
      ...process.env,
      GIT_AUTHOR_NAME: USER.name,
      GIT_AUTHOR_EMAIL: USER.email,
      GIT_COMMITTER_NAME: USER.name,
      GIT_COMMITTER_EMAIL: USER.email,
      GIT_CONFIG_NOSYSTEM: "1",
      HOME: os.tmpdir(),
      XDG_CONFIG_HOME: path.join(os.tmpdir(), "aprende-git-lab-home"),
    },
  });
}

function tryGit(args, cwd) {
  try {
    return { ok: true, out: git(args, cwd) };
  } catch (e) {
    return { ok: false, out: (e.stdout || "") + (e.stderr || "") };
  }
}

function freshDir(...parts) {
  const dir = path.join(LAB_ROOT, ...parts);
  fs.rmSync(dir, { recursive: true, force: true });
  fs.mkdirSync(dir, { recursive: true });
  return dir;
}

function write(dir, rel, content) {
  const p = path.join(dir, rel);
  fs.mkdirSync(path.dirname(p), { recursive: true });
  fs.writeFileSync(p, content);
}

function init(dir) {
  git(["init", "-b", "main"], dir);
  git(["config", "user.name", USER.name], dir);
  git(["config", "user.email", USER.email], dir);
  git(["config", "commit.gpgsign", "false"], dir);
}

function commit(dir, msg) {
  git(["add", "-A"], dir);
  git(["commit", "-m", msg], dir);
}

function head(dir) {
  return git(["rev-parse", "HEAD"], dir).trim();
}

/* --------------------------------------------------------------- escenarios */

/** main y feature/busqueda chocan en la misma línea; queda un merge a medias. */
function conflicto() {
  const dir = freshDir("conflicto");
  init(dir);
  write(dir, "notas.md", "# Notas\n\n- idea uno\n- idea dos\n- idea tres\n");
  commit(dir, "docs: notas iniciales");

  git(["switch", "-c", "feature/busqueda"], dir);
  write(dir, "notas.md", "# Notas\n\n- idea uno\n- BUSCAR: índice de notas\n- idea tres\n");
  commit(dir, "feat: indice de busqueda");

  git(["switch", "main"], dir);
  write(dir, "notas.md", "# Notas\n\n- idea uno\n- idea dos (revisada)\n- idea tres\n");
  commit(dir, "docs: revisar la idea dos");

  const merge = tryGit(["merge", "feature/busqueda"], dir);
  return { name: "conflicto", dir, conflicted: !merge.ok, mergeOutput: merge.out };
}

/** Historia sucia: mensajes pobres y commits diminutos para practicar rebase -i. */
function historia() {
  const dir = freshDir("historia");
  init(dir);
  write(dir, "README.md", "# Taller\n");
  commit(dir, "init");
  write(dir, "README.md", "# Taller\n\nUtilidades de notas.\n");
  commit(dir, "cambio");
  write(dir, "notas.md", "# Notas\n");
  commit(dir, "wip");
  write(dir, "notas.md", "# Notas\n\n- a\n");
  commit(dir, "mas");
  write(dir, "notas.md", "# Notas\n\n- a\n- b\n");
  commit(dir, "fix typo");
  write(dir, "LICENSE", "MIT\n");
  commit(dir, "añade licencia");
  return { name: "historia", dir, commits: Number(git(["rev-list", "--count", "HEAD"], dir).trim()) };
}

/** Hay un commit importante que se pierde tras un `reset --hard`; solo vive en el reflog. */
function perdido() {
  const dir = freshDir("perdido");
  init(dir);
  write(dir, "notas.md", "# Notas\n\n- a\n");
  commit(dir, "docs: notas iniciales");
  write(dir, "notas.md", "# Notas\n\n- a\n- b\n");
  commit(dir, "feat: segunda nota");
  write(dir, "informe.md", "# Informe\n\nEl contenido que NO se puede perder.\n");
  commit(dir, "feat: informe importante");
  const lost = head(dir);
  git(["reset", "--hard", "HEAD~1"], dir);
  return { name: "perdido", dir, lostCommit: lost };
}

/** Un bug entra en algún commit; se encuentra con `bisect run`. */
function bisect() {
  const dir = freshDir("bisect");
  init(dir);
  write(dir, "estado.txt", "bueno\n");
  write(dir, "verificar.mjs", `import fs from "node:fs";
const v = fs.readFileSync(new URL("./estado.txt", import.meta.url), "utf8");
process.exit(v.includes("malo") ? 1 : 0);
`);
  commit(dir, "chore: proyecto de ejemplo");

  const total = 10;
  const firstBadIndex = 6; // el commit 6 introduce el fallo
  let firstBad = null;
  for (let i = 2; i <= total; i++) {
    const malo = i >= firstBadIndex;
    write(dir, "estado.txt", malo ? "malo\n" : "bueno\n");
    write(dir, `src/modulo-${i}.txt`, `modulo ${i}\n`);
    commit(dir, `feat: modulo ${i}`);
    if (malo && !firstBad) firstBad = head(dir);
  }
  return { name: "bisect", dir, firstBad, commits: total };
}

/** Remoto local (bare) y dos clones divergentes para practicar fetch/pull/push. */
function equipo() {
  freshDir("equipo");
  const origin = path.join(LAB_ROOT, "equipo", "origen.git");
  fs.mkdirSync(origin, { recursive: true });
  git(["init", "--bare", "-b", "main"], origin);

  const trabajo = freshDir("equipo", "trabajo");
  git(["clone", origin, "."], trabajo);
  git(["config", "user.name", USER.name], trabajo);
  git(["config", "user.email", USER.email], trabajo);
  write(trabajo, "notas.md", "# Notas\n");
  commit(trabajo, "docs: notas iniciales");
  git(["push", "-u", "origin", "main"], trabajo);

  const companero = freshDir("equipo", "companero");
  git(["clone", origin, "."], companero);
  git(["config", "user.name", "Companero"], companero);
  git(["config", "user.email", "companero@example.com"], companero);
  write(companero, "README.md", "# Taller\n");
  commit(companero, "docs: readme");
  git(["push", "origin", "main"], companero);

  // `trabajo` se actualiza del remoto y luego crea un commit local distinto:
  // la historia queda divergente con origin/main.
  git(["fetch", "origin"], trabajo);
  write(trabajo, "local.txt", "solo local\n");
  commit(trabajo, "feat: cambio local");

  return { name: "equipo", dir: trabajo, origin, companero, work: trabajo };
}

/** Repositorio desordenado para la auditoría del proyecto final. */
function auditoria() {
  const dir = freshDir("auditoria");
  init(dir);
  write(dir, "app.js", "console.log('hola');\n");
  write(dir, "node_modules/dep/index.js", "module.exports = 1;\n");
  commit(dir, "update");
  write(dir, "app.js", "console.log('hola mundo');\n");
  commit(dir, "cosas");
  // Un binario colado en el control de versiones.
  const bin = Buffer.from([0x89, 0x50, 0x4e, 0x47, 0x00, 0x01, 0x02, 0x03, 0x00]);
  write(dir, "logo.png", bin);
  commit(dir, "wip");
  git(["branch", "experimento-viejo"], dir);
  write(dir, "app.js", "console.log('hola mundo!');\n");
  commit(dir, "mas cosas");
  return { name: "auditoria", dir };
}

const SCENARIOS = { conflicto, historia, perdido, bisect, equipo, auditoria };

export function build(name) {
  const fn = SCENARIOS[name];
  if (!fn) throw new Error(`Escenario desconocido: ${name}`);
  fs.mkdirSync(LAB_ROOT, { recursive: true });
  return fn();
}

/* -------------------------------------------------------------------- CLI */

function clean() {
  fs.rmSync(LAB_ROOT, { recursive: true, force: true });
}

function list() {
  console.log("Escenarios disponibles:\n");
  console.log("  conflicto   merge a medias con conflictos en notas.md");
  console.log("  historia    commits diminutos y mensajes pobres (rebase -i)");
  console.log("  perdido     un commit importante solo recuperable por reflog");
  console.log("  bisect      un bug escondido (usa: git bisect run node verificar.mjs)");
  console.log("  equipo      remoto bare + dos clones divergentes");
  console.log("  auditoria   repositorio desordenado para el proyecto final");
}

function main() {
  const [cmd] = process.argv.slice(2);
  if (!cmd || cmd === "list") return list();
  if (cmd === "clean") {
    clean();
    console.log(`Laboratorio eliminado: ${LAB_ROOT}`);
    return;
  }
  if (!SCENARIOS[cmd]) {
    console.error(`Escenario desconocido: ${cmd}\n`);
    list();
    process.exit(1);
  }
  const r = build(cmd);
  console.log(`✓ Escenario «${cmd}» creado en:\n  ${r.dir}\n`);
  if (cmd === "conflicto") {
    console.log("Hay un merge sin terminar. Resuelve los conflictos de notas.md, haz `git add` y `git commit`,");
    console.log("o aborta con `git merge --abort`.");
  } else if (cmd === "historia") {
    console.log("Usa `git rebase -i HEAD~6` para agrupar y reescribir los mensajes.");
  } else if (cmd === "perdido") {
    console.log("El commit perdido: " + r.lostCommit);
    console.log("Recupéralo con `git reflog` y `git switch -c rescate <hash>`.");
  } else if (cmd === "bisect") {
    console.log("Empieza con: git bisect start && git bisect bad HEAD && git bisect good <primer-commit>");
    console.log("Luego: git bisect run node verificar.mjs");
  } else if (cmd === "equipo") {
    console.log("Repositorio bare: " + r.origin);
    console.log("Tu copia (divergente con origin/main): " + r.work);
    console.log("La otra copia: " + r.companero);
    console.log("Practica `git fetch`, `git log --oneline --all --graph` y `git pull --rebase`.");
  } else if (cmd === "auditoria") {
    console.log("Audita este repositorio (mensajes, binarios, ramas muertas) — ver Módulo 7 del curso.");
  }
}

if (import.meta.url === `file://${process.argv[1]}` || process.argv[1]?.endsWith("lab.mjs")) {
  main();
}
