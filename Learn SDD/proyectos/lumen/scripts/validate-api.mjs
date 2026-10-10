#!/usr/bin/env node
// validate-api.mjs — valida un documento OpenAPI contra su estructura mínima
// Uso: node scripts/validate-api.mjs lumen.openapi.json
// Sin dependencias: comprueba los tres bloques estables del patrón
//   (openapi, info, paths) y que cada ruta declare al menos un metodo.
// Código de salida: 0 = válido, 1 = inválido.

import fs from "node:fs";

const [fileArg] = process.argv.slice(2);
if (!fileArg) {
  console.error("uso: node scripts/validate-api.mjs <api.openapi.json>");
  process.exit(2);
}

let doc;
try {
  doc = JSON.parse(fs.readFileSync(fileArg, "utf8"));
} catch (err) {
  console.error(`el contrato ${fileArg} no es JSON válido: ${err.message}`);
  process.exit(1);
}

const errors = [];

// openapi: cadena con la versión del formato (3.x en este curso)
if (typeof doc.openapi !== "string" || !/^3\./.test(doc.openapi)) {
  errors.push('falta "openapi" (cadena "3.x.y")');
}

// info: objeto con title
if (!doc.info || typeof doc.info.title !== "string") {
  errors.push('falta "info.title" (cadena)');
}

// paths: objeto con al menos una ruta; cada ruta con, como mínimo, un método
if (!doc.paths || typeof doc.paths !== "object" || Array.isArray(doc.paths)) {
  errors.push('falta "paths" (objeto de rutas)');
} else if (Object.keys(doc.paths).length === 0) {
  errors.push('"paths" no declara ninguna ruta');
} else {
  for (const [route, methods] of Object.entries(doc.paths)) {
    const keys = Object.keys(methods || {});
    const httpKeys = keys.filter((k) => /^(get|post|put|patch|delete|head|options)$/.test(k));
    if (httpKeys.length === 0) {
      errors.push(`la ruta "${route}" no declara ningún método HTTP (get/post/...)`);
    }
  }
}

if (errors.length) {
  console.error(`✗ el contrato no es un OpenAPI válido para este curso:`);
  errors.forEach((e) => console.error(`  - ${e}`));
  process.exit(1);
}
console.log("✓ contrato OpenAPI válido (openapi, info, paths presentes)");
process.exit(0);