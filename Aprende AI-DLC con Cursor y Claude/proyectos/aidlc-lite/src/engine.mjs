/**
 * engine.mjs — operaciones de alto nivel del motor.
 *
 * Aquí se juntan configuración, estado, auditoría, reglas y proyección. La CLI
 * solo traduce argumentos a llamadas de este módulo.
 */
import fs from "node:fs";
import path from "node:path";
import { appendAudit, readAudit } from "./audit.mjs";
import {
  configFile,
  defaultConfig,
  loadConfig,
  normalizeHarnesses,
  saveConfig,
  validateConfig,
} from "./config.mjs";
import { applyGate } from "./gates.mjs";
import { getProfile } from "./profiles.mjs";
import { projectAll, hasManagedBlock } from "./projection.mjs";
import { loadRules, validateRules, writeDefaultRules } from "./rules.mjs";
import { createIntent, currentStage, progress, validateState } from "./state.mjs";

export const STATE_PATH = [".aidlc-lite", "state.json"];

export function stateFile(dir) {
  return path.join(dir, ...STATE_PATH);
}

export function saveState(dir, state) {
  const file = stateFile(dir);
  fs.mkdirSync(path.dirname(file), { recursive: true });
  fs.writeFileSync(file, `${JSON.stringify(state, null, 2)}\n`, "utf8");
  return file;
}

export function loadState(dir) {
  const file = stateFile(dir);
  if (!fs.existsSync(file)) {
    throw new Error(`no hay ninguna intención activa en ${dir}. Ejecuta run primero.`);
  }
  return JSON.parse(fs.readFileSync(file, "utf8"));
}

export function initProject(dir, { harnesses = "claude" } = {}) {
  fs.mkdirSync(dir, { recursive: true });
  const normalized = normalizeHarnesses(harnesses);
  const config = defaultConfig(normalized);

  if (!fs.existsSync(configFile(dir))) saveConfig(dir, config);
  writeDefaultRules(dir);

  fs.mkdirSync(path.join(dir, "artifacts"), { recursive: true });
  const artifactReadme = path.join(dir, "artifacts", "README.md");
  if (!fs.existsSync(artifactReadme)) {
    fs.writeFileSync(
      artifactReadme,
      "# Artefactos\n\nAñade aquí los documentos de cada etapa: intent, requisitos, diseño, plan.\n",
      "utf8"
    );
  }

  const { merged } = loadRules(dir);
  const written = projectAll(dir, merged, normalized);
  appendAudit(dir, {
    type: "project.initialized",
    harnesses: normalized,
    files: written.map((file) => path.relative(dir, file).replaceAll("\\", "/")),
  });

  return { dir, config, written };
}

export function startIntent(dir, { profile, intent }) {
  const config = loadConfig(dir);
  const chosen = profile || config.defaultProfile;
  getProfile(chosen);
  const state = createIntent({ profile: chosen, intent });
  // La auditoría se escribe antes que el estado: si el proceso muere entre
  // ambas, el evento permite reconstruir la intención.
  appendAudit(dir, {
    type: "intent.started",
    intent: state.intent,
    profile: chosen,
    stages: state.stages.length,
  });
  saveState(dir, state);
  return state;
}

export function approve(dir, { note = "" } = {}) {
  const state = loadState(dir);
  const before = currentStage(state);
  const result = applyGate(state, "approve", { note });
  appendAudit(dir, {
    type: "gate.approved",
    intent: result.state.intent,
    stage: before?.id ?? null,
    note: note || null,
    status: result.state.status,
  });
  saveState(dir, result.state);
  return result.state;
}

export function reject(dir, { note = "" } = {}) {
  const state = loadState(dir);
  const stage = currentStage(state);
  const result = applyGate(state, "reject", { note });
  appendAudit(dir, {
    type: "gate.rejected",
    intent: result.state.intent,
    stage: stage?.id ?? null,
    note: note || null,
  });
  saveState(dir, result.state);
  return result.state;
}

export function syncProjections(dir) {
  const config = loadConfig(dir);
  const { sources, merged } = loadRules(dir);
  const written = projectAll(dir, merged, config.harnesses);
  appendAudit(dir, {
    type: "rules.projected",
    sources: sources.map((source) => source.file),
    files: written.map((file) => path.relative(dir, file).replaceAll("\\", "/")),
  });
  return { written, sources };
}

export function status(dir) {
  const state = loadState(dir);
  const stage = currentStage(state);
  const counts = progress(state);
  return {
    intent: state.intent,
    profile: state.profile,
    status: state.status,
    stage: stage?.id ?? null,
    stageTitle: stage?.title ?? null,
    done: counts.done,
    total: counts.total,
    pendingGate: state.status !== "completed",
  };
}

export function doctor(dir) {
  const problems = [];
  const warnings = [];

  if (!fs.existsSync(configFile(dir))) {
    problems.push("falta .aidlc-lite/config.json (ejecuta init)");
  } else {
    try {
      const config = loadConfig(dir);
      problems.push(...validateConfig(config));
      for (const harness of config.harnesses) {
        const file =
          harness === "claude"
            ? path.join(dir, ".claude", "CLAUDE.md")
            : path.join(dir, ".cursor", "rules", "aidlc-team.mdc");
        if (!hasManagedBlock(file)) {
          problems.push(`falta la proyección gestionada para ${harness}: ${file}`);
        }
      }
    } catch (error) {
      problems.push(error.message);
    }
  }

  problems.push(...validateRules(dir));

  if (fs.existsSync(stateFile(dir))) {
    try {
      const state = loadState(dir);
      problems.push(...validateState(state));
    } catch (error) {
      problems.push(error.message);
    }
  } else {
    warnings.push("todavía no hay una intención activa; ejecuta run");
  }

  const audit = readAudit(dir);
  return { ok: problems.length === 0, problems, warnings, auditEvents: audit.length };
}
