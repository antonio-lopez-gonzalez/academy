#!/usr/bin/env node
// Verificador estructural de cursos de la Academy.
// Uso: node verify.mjs ["Aprende X"] [--root <raiz>]
import fs from "node:fs";
import path from "node:path";

const SECTIONS = [
  "introduccion", "teoria", "ejemplo-minimo", "ejemplo-real", "buenas-practicas",
  "errores", "ejercicio", "reto", "resumen", "faq",
];

const args = process.argv.slice(2);
let root = process.cwd();
const only = [];
for (let i = 0; i < args.length; i++) {
  if (args[i] === "--root") root = args[++i];
  else if (args[i].startsWith("--")) { /* ignora */ }
  else only.push(args[i]);
}

const courses = fs
  .readdirSync(root, { withFileTypes: true })
  .filter((d) => d.isDirectory() && d.name.startsWith("Aprende "))
  .map((d) => d.name)
  .filter((n) => only.length === 0 || only.includes(n));

function readIf(p) {
  return fs.existsSync(p) ? fs.readFileSync(p, "utf8") : "";
}

function checkCourse(name) {
  const errors = [];
  const warns = [];
  const base = path.join(root, name);
  const webIndex = path.join(base, "web", "index.html");
  const dataPath = path.join(base, "web", "assets", "js", "course-data.js");
  const sitePath = path.join(base, "web", "assets", "js", "site.js");

  if (!fs.existsSync(webIndex)) errors.push("falta web/index.html");
  if (!fs.existsSync(dataPath)) {
    errors.push("falta web/assets/js/course-data.js");
    return { errors, warns, lessons: 0, key: null };
  }

  const data = fs.readFileSync(dataPath, "utf8");
  const site = readIf(sitePath);
  const keyMatch = site.match(/theme:\s*"([a-z0-9-]+)-theme"/);
  const key = keyMatch ? keyMatch[1] : null;
  if (!key) warns.push("no se detectó la clave de tema en site.js");

  const modRe = /dir:\s*"([^"]+)"[\s\S]*?lessons:\s*\[([\s\S]*?)\n\s*\]/g;
  const mapped = new Set();
  let lessons = 0;
  let m;
  while ((m = modRe.exec(data))) {
    const dir = m[1];
    const fileRe = /file:\s*"([^"]+)"/g;
    let fm;
    while ((fm = fileRe.exec(m[2]))) {
      lessons++;
      const rel = `${dir}/${fm[1]}`.replace(/\\/g, "/");
      mapped.add(rel);
      const abs = path.join(base, "web", rel);
      if (!fs.existsSync(abs)) {
        errors.push(`lección del mapa sin archivo: ${rel}`);
        continue;
      }
      const t = fs.readFileSync(abs, "utf8");
      const lessonId = fm[1].replace(/\.html$/, "");
      if (!t.includes(`data-lesson="${lessonId}"`)) errors.push(`${rel}: data-lesson no coincide`);
      if (!t.includes("highlight.js")) errors.push(`${rel}: sin highlight.js`);
      if (!t.includes("section-number")) errors.push(`${rel}: sin numeración de secciones`);
      if (key && !t.includes(`'${key}-theme'`)) errors.push(`${rel}: no usa la clave de tema '${key}-theme'`);
      for (const s of SECTIONS) {
        if (!t.includes(`id="${s}"`)) {
          errors.push(`${rel}: falta la sección id="${s}"`);
          break;
        }
      }
    }
  }

  // Huérfanas: lecciones en disco que no están en el mapa.
  const modDir = path.join(base, "web", "modulos");
  if (fs.existsSync(modDir)) {
    const walk = (d) => {
      for (const e of fs.readdirSync(d, { withFileTypes: true })) {
        const p = path.join(d, e.name);
        if (e.isDirectory()) walk(p);
        else if (e.name.endsWith(".html")) {
          const rel = path.relative(path.join(base, "web"), p).replace(/\\/g, "/");
          if (!mapped.has(rel)) warns.push(`huérfana (en disco, no en el mapa): ${rel}`);
        }
      }
    };
    walk(modDir);
  }

  return { errors, warns, lessons, key };
}

let totalErrors = 0;
for (const c of courses) {
  const r = checkCourse(c);
  totalErrors += r.errors.length;
  console.log(`${r.errors.length ? "✗" : "✓"} ${c}  (${r.lessons} lecciones${r.key ? `, tema ${r.key}` : ""})`);
  for (const e of r.errors.slice(0, 40)) console.log(`   ERROR  ${e}`);
  for (const w of r.warns.slice(0, 20)) console.log(`   warn   ${w}`);
}
console.log(`\n${courses.length} cursos · ${totalErrors} errores`);
process.exit(totalErrors ? 1 : 0);
