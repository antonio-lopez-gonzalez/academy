#!/usr/bin/env node
// validate-notes.mjs — valida un almacén de notas contra schema/note.schema.json
// Uso: node scripts/validate-notes.mjs data/notes.json schema/note.schema.json
// Sin dependencias: implementa el subconjunto de JSON Schema usado por Lumen
//   (type, properties, required, additionalProperties) y localiza la fila rota.
// Código de salida: 0 = válido, 1 = inválido.

import fs from "node:fs";

// --- Argumentos -------------------------------------------------------------
const [storeArg, schemaArg] = process.argv.slice(2);
if (!storeArg || !schemaArg) {
  console.error("uso: node scripts/validate-notes.mjs <almacén.json> <esquema.json>");
  process.exit(2);
}

// --- Lectura ----------------------------------------------------------------
let data, schema;
try {
  data = JSON.parse(fs.readFileSync(storeArg, "utf8"));
} catch (err) {
  console.error(`el almacén ${storeArg} no es JSON válido: ${err.message}`);
  process.exit(1);
}
try {
  schema = JSON.parse(fs.readFileSync(schemaArg, "utf8"));
} catch (err) {
  console.error(`el esquema ${schemaArg} no es JSON válido: ${err.message}`);
  process.exit(2);
}

// --- Validación (subset suficiente para Lumen) -------------------------------
function typeName(value) {
  if (Array.isArray(value)) return "array";
  if (value === null) return "null";
  return typeof value;
}

// Comprueba UN valor contra UN objeto de esquema. Devuelve array de errores.
function checkValue(value, subschema, path) {
  const errors = [];
  if (subschema.type !== undefined && typeName(value) !== subschema.type) {
    errors.push(`${path}: se esperaba ${subschema.type}, se encontró ${typeName(value)}`);
    return errors; // sin el tipo correcto el resto no aplica
  }
  if (subschema.type === "array") {
    if (subschema.items) {
      value.forEach((item, i) => {
        errors.push(...checkValue(item, subschema.items, `${path}[${i}]`));
      });
    }
    return errors;
  }
  if (subschema.type === "object" && value !== null && typeof value === "object") {
    if (subschema.required) {
      for (const key of subschema.required) {
        if (value[key] === undefined) {
          errors.push(`${path}: falta el campo obligatorio "${key}"`);
        }
      }
    }
    const props = subschema.properties || {};
    for (const [key, propSchema] of Object.entries(props)) {
      if (value[key] !== undefined) {
        errors.push(...checkValue(value[key], propSchema, `${path}.${key}`));
      }
    }
    if (subschema.additionalProperties === false) {
      for (const key of Object.keys(value)) {
        if (!(key in props)) {
          errors.push(`${path}: el campo "${key}" no está permitido (additionalProperties: false)`);
        }
      }
    }
  }
  return errors;
}

// --- Ejecución --------------------------------------------------------------
// Espera un array (el almacén de Lumen es un array de notas).
if (!Array.isArray(data)) {
  console.error("el almacén debe ser un array de notas (JSON Array).");
  process.exit(1);
}

// En lugar del $schema raíz, se valida cada elemento contra el object de la Nota.
const noteSchema = schema.type === "array" ? schema.items : schema;
const failedRows = [];
data.forEach((note, i) => {
  const errs = checkValue(note, noteSchema, `notas[${i}]`);
  if (errs.length) {
    failedRows.push({ index: i, errors: errs });
  }
});

if (failedRows.length) {
  console.error(`✗ ${failedRows.length} nota(s) con errores:`);
  for (const row of failedRows) {
    console.error(`nota ${row.index}:`);
    row.errors.forEach((e) => console.error(`  - ${e}`));
  }
  process.exit(1);
}
console.log("✓ todas las notas válidas contra el esquema");
process.exit(0);