/**
 * build-zips.mjs — genera el ZIP de código de cada curso.
 *
 * Deja en `Aprende <Curso>/web/descargas/Aprende-<Curso>.zip` los proyectos de
 * muestra: SOLO `proyectos/` y `ejemplos/` (nunca las lecciones), excluyendo
 * artefactos (`node_modules`, `obj`, `bin`, `dist`, `__pycache__`, …).
 *
 * Los ZIP se commitean en el repositorio y se sirven/enlazan directamente
 * desde la portada de cada curso (`./descargas/<slug>.zip`).
 *
 * Sin dependencias: el escritor ZIP usa `node:zlib`. Funciona en Windows,
 * macOS y Linux.
 *
 * Uso:  node tools/build-zips.mjs
 */

import fs from "node:fs";
import path from "node:path";
import zlib from "node:zlib";
import { execFileSync } from "node:child_process";

const ROOT = process.cwd();

// Nunca entran en los ZIP (código fuente sí, artefactos de build no).
const ZIP_SKIP_DIRS = new Set([
  "node_modules", "obj", "bin", "dist", "build", "out", ".git",
  "__pycache__", ".venv", "venv", ".vs", "TestResults", "coverage",
  ".pytest_cache", ".mypy_cache", ".ruff_cache", ".next", ".astro",
]);
const STAMP = new Date(0); // fecha fija -> ZIPs reproducibles

// ---------------------------------------------------------------- ZIP writer

const CRC_TABLE = new Uint32Array(256);
for (let n = 0; n < 256; n++) {
  let c = n;
  for (let k = 0; k < 8; k++) c = c & 1 ? 0xedb88320 ^ (c >>> 1) : c >>> 1;
  CRC_TABLE[n] = c >>> 0;
}

function crc32(buf) {
  let c = 0xffffffff;
  for (let i = 0; i < buf.length; i++) c = CRC_TABLE[(c ^ buf[i]) & 0xff] ^ (c >>> 8);
  return (c ^ 0xffffffff) >>> 0;
}

function dosDateTime(d) {
  const year = Math.max(1980, d.getFullYear());
  return {
    time: (d.getHours() << 11) | (d.getMinutes() << 5) | Math.floor(d.getSeconds() / 2),
    date: ((year - 1980) << 9) | ((d.getMonth() + 1) << 5) | d.getDate(),
  };
}

/** entries: [{ name, data?, isDir? }] — `name` siempre con `/` */
function makeZip(entries) {
  const locals = [];
  const centrals = [];
  let offset = 0;

  for (const e of entries) {
    const nameBuf = Buffer.from(e.name, "utf8");
    const data = e.isDir ? Buffer.alloc(0) : e.data;
    const comp = e.isDir ? Buffer.alloc(0) : zlib.deflateRawSync(data, { level: 9 });
    const crc = e.isDir ? 0 : crc32(data);
    const method = e.isDir ? 0 : 8;
    const { time, date } = dosDateTime(STAMP);

    const local = Buffer.alloc(30);
    local.writeUInt32LE(0x04034b50, 0);
    local.writeUInt16LE(20, 4);
    local.writeUInt16LE(0x0800, 6); // nombres en UTF-8
    local.writeUInt16LE(method, 8);
    local.writeUInt16LE(time, 10);
    local.writeUInt16LE(date, 12);
    local.writeUInt32LE(crc, 14);
    local.writeUInt32LE(comp.length, 18);
    local.writeUInt32LE(data.length, 22);
    local.writeUInt16LE(nameBuf.length, 26);
    local.writeUInt16LE(0, 28);
    locals.push(local, nameBuf, comp);

    const central = Buffer.alloc(46);
    central.writeUInt32LE(0x02014b50, 0);
    central.writeUInt16LE(20, 4);
    central.writeUInt16LE(20, 6);
    central.writeUInt16LE(0x0800, 8);
    central.writeUInt16LE(method, 10);
    central.writeUInt16LE(time, 12);
    central.writeUInt16LE(date, 14);
    central.writeUInt32LE(crc, 16);
    central.writeUInt32LE(comp.length, 20);
    central.writeUInt32LE(data.length, 24);
    central.writeUInt16LE(nameBuf.length, 28);
    central.writeUInt16LE(0, 30);
    central.writeUInt16LE(0, 32);
    central.writeUInt16LE(0, 34);
    central.writeUInt16LE(0, 36);
    central.writeUInt32LE(e.isDir ? 0x10 : 0, 38);
    central.writeUInt32LE(offset, 42);
    centrals.push(central, nameBuf);

    offset += local.length + nameBuf.length + comp.length;
  }

  const centralBuf = Buffer.concat(centrals);
  const eocd = Buffer.alloc(22);
  eocd.writeUInt32LE(0x06054b50, 0);
  eocd.writeUInt16LE(0, 4);
  eocd.writeUInt16LE(0, 6);
  eocd.writeUInt16LE(entries.length, 8);
  eocd.writeUInt16LE(entries.length, 10);
  eocd.writeUInt32LE(centralBuf.length, 12);
  eocd.writeUInt32LE(offset, 16);
  eocd.writeUInt16LE(0, 20);

  return Buffer.concat([...locals, centralBuf, eocd]);
}

/** Entradas del ZIP a partir de rutas relativas a `baseAbs` (con `/`). */
function entriesFromFiles(files, baseAbs) {
  const entries = [];
  const dirs = new Set();
  for (const rel of files) {
    const parts = rel.split("/");
    for (let i = 1; i < parts.length; i++) {
      const d = parts.slice(0, i).join("/") + "/";
      if (!dirs.has(d)) {
        dirs.add(d);
        entries.push({ name: d, isDir: true });
      }
    }
    entries.push({ name: rel, data: fs.readFileSync(path.join(baseAbs, rel)) });
  }
  return entries;
}

const hasSkippedSegment = (p) => p.split("/").some((seg) => ZIP_SKIP_DIRS.has(seg));

// ---------------------------------------------------------------- build

const tracked = execFileSync("git", ["ls-files", "-z"], { cwd: ROOT, encoding: "utf8" })
  .split("\0")
  .filter(Boolean)
  .map((f) => f.replace(/\\/g, "/"));

if (tracked.length === 0) {
  console.error("No hay archivos en git (¿estás dentro del repositorio?).");
  process.exit(1);
}

const courses = [...new Set(tracked.filter((f) => f.startsWith("Aprende ")).map((f) => f.split("/")[0]))].sort();

let zips = 0;
for (const course of courses) {
  const code = tracked.filter(
    (f) =>
      (f.startsWith(`${course}/proyectos/`) || f.startsWith(`${course}/ejemplos/`)) &&
      !hasSkippedSegment(f),
  );
  if (code.length === 0) continue;

  const rels = code.map((f) => f.slice(course.length + 1)).sort();
  const slug = course.replace(/\s+/g, "-");
  const outDir = path.join(ROOT, course, "web", "descargas");
  fs.mkdirSync(outDir, { recursive: true });
  const file = path.join(outDir, `${slug}.zip`);
  fs.writeFileSync(file, makeZip(entriesFromFiles(rels, path.join(ROOT, course))));

  const kb = (fs.statSync(file).size / 1024).toFixed(0);
  console.log(`✓ ${course}/web/descargas/${slug}.zip  (${rels.length} archivos, ${kb} KB)`);
  zips++;
}

console.log(`\n${zips} ZIPs generados en <curso>/web/descargas/`);
