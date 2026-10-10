#!/usr/bin/env node
// cli.js — CLI de Lumen (bandeja de notas mínima).
// Implementa spec/lumen.md: add | list | close | --help sobre data/notes.json.
// Cero dependencias: solo node:fs y node:path. Mensajes y ayuda en español.

import fs from "node:fs";
import path from "node:path";

// Almacén por defecto, relativo al directorio de trabajo (donde se ejecuta).
const DEFAULT_STORE = path.join("data", "notes.json");

// Muestra el error en español y sale con código != 0.
function fail(message) {
  console.error(`lumen: ${message}`);
  process.exit(1);
}

// Lee el almacén; si no existe, devuelve []. Falla si el JSON es inválido.
function loadNotes(storePath) {
  try {
    const raw = fs.readFileSync(storePath, "utf8");
    const parsed = JSON.parse(raw);
    if (!Array.isArray(parsed)) fail(`el almacén ${storePath} no contiene un array JSON.`);
    return parsed;
  } catch (err) {
    // Fichero inexistente = almacén vacío (se crea al escribir).
    if (err.code === "ENOENT") return [];
    if (err instanceof SyntaxError) fail(`el almacén ${storePath} no es JSON válido.`);
    throw err;
  }
}

// Escribe el array de notas (crea la carpeta data/ si hace falta).
function saveNotes(storePath, notes) {
  fs.mkdirSync(path.dirname(storePath), { recursive: true });
  fs.writeFileSync(storePath, JSON.stringify(notes, null, 2) + "\n", "utf8");
}

// Genera un id único corto y legible (base36 de tiempo + aleatorio).
function newId() {
  return Date.now().toString(36) + Math.random().toString(36).slice(2, 6);
}

// Extrae --store <ruta> y --all de los argumentos; devuelve el resto.
function parseFlags(args) {
  const rest = [];
  let store = DEFAULT_STORE;
  let all = false;
  for (let i = 0; i < args.length; i++) {
    if (args[i] === "--store") {
      const value = args[++i];
      if (!value) fail("falta el valor de --store.");
      store = value;
    } else if (args[i] === "--all") {
      all = true;
    } else {
      rest.push(args[i]);
    }
  }
  return { rest, store, all };
}

function printHelp() {
  console.log(`lumen — bandeja de notas mínima

Uso:
  node src/cli.js add "<título>" [--store <ruta>]   Crea una nota
  node src/cli.js list [--all] [--store <ruta>]     Muestra el inbox (o todo con --all)
  node src/cli.js close <id> [--store <ruta>]       Cierra una nota
  node src/cli.js --help                            Esta ayuda

Almacén por defecto: data/notes.json`);
}

// add "<título>": crea la nota y muestra su id.
function cmdAdd(title, store) {
  if (!title || !title.trim()) fail("falta el título: usa add \"<título>\".");
  const notes = loadNotes(store);
  const note = {
    id: newId(),
    title: title.trim(),
    done: false,
    createdAt: new Date().toISOString(),
  };
  notes.push(note);
  saveNotes(store, notes);
  console.log(`nota creada: ${note.id} ${note.title}`);
}

// list: una línea por nota "<id> <título>"; por defecto solo abiertas.
function cmdList(store, all) {
  const notes = loadNotes(store);
  const visible = all ? notes : notes.filter((n) => !n.done);
  for (const note of visible) {
    const mark = note.done ? "[x]" : "[ ]";
    console.log(`${mark} ${note.id} ${note.title}`);
  }
}

// close <id>: marca done:true; falla si el id no existe.
function cmdClose(id, store) {
  if (!id) fail("falta el id: usa close <id>.");
  const notes = loadNotes(store);
  const note = notes.find((n) => n.id === id);
  if (!note) fail(`no existe ninguna nota con id "${id}".`);
  note.done = true;
  saveNotes(store, notes);
  console.log(`nota cerrada: ${note.id}`);
}

function main() {
  const [, , command, ...restArgs] = process.argv;
  if (!command || command === "--help" || command === "-h") {
    printHelp();
    return;
  }
  const { rest, store, all } = parseFlags(restArgs);
  if (command === "add") cmdAdd(rest[0], store);
  else if (command === "list") cmdList(store, all);
  else if (command === "close") cmdClose(rest[0], store);
  else fail(`comando desconocido "${command}". Usa --help.`);
}

main();