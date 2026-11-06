/**
 * config.mjs — configuración del proyecto didáctico.
 *
 * Solo hay dos decisiones: qué harnesses se proyectan y qué perfil se usa por
 * defecto. El resto lo deduce el motor.
 */
import fs from "node:fs";
import path from "node:path";

export const CONFIG_PATH = [".aidlc-lite", "config.json"];
export const VALID_HARNESSES = ["claude", "cursor"];

export function defaultConfig(harnesses = ["claude"]) {
  return {
    version: 1,
    harnesses,
    defaultProfile: "classic",
  };
}

export function normalizeHarnesses(input) {
  if (!input) return ["claude"];
  if (input === "both") return [...VALID_HARNESSES];
  const list = String(input)
    .split(",")
    .map((item) => item.trim())
    .filter(Boolean);
  for (const harness of list) {
    if (!VALID_HARNESSES.includes(harness)) {
      throw new Error(
        `harness desconocido: ${harness}. Válidos: ${VALID_HARNESSES.join(", ")} o "both"`
      );
    }
  }
  return [...new Set(list)];
}

export function configFile(dir) {
  return path.join(dir, ...CONFIG_PATH);
}

export function loadConfig(dir) {
  const file = configFile(dir);
  if (!fs.existsSync(file)) {
    throw new Error(`no existe ${file}. Ejecuta primero: aidlc-lite init --dir ${dir}`);
  }
  return JSON.parse(fs.readFileSync(file, "utf8"));
}

export function saveConfig(dir, config) {
  const file = configFile(dir);
  fs.mkdirSync(path.dirname(file), { recursive: true });
  fs.writeFileSync(file, `${JSON.stringify(config, null, 2)}\n`, "utf8");
  return file;
}

export function validateConfig(config) {
  const errors = [];
  if (!config || typeof config !== "object") return ["la configuración no es un objeto"];
  if (!Array.isArray(config.harnesses) || config.harnesses.length === 0) {
    errors.push("harnesses debe ser un array no vacío");
  } else {
    for (const harness of config.harnesses) {
      if (!VALID_HARNESSES.includes(harness)) {
        errors.push(`harness inválido: ${harness}`);
      }
    }
  }
  if (!config.defaultProfile) errors.push("falta defaultProfile");
  return errors;
}
