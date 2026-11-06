#!/usr/bin/env node
/**
 * cli.mjs — interfaz de línea de comandos de aidlc-lite.
 *
 * Comandos: init, run, status, approve, reject, sync, doctor, sensors, profiles.
 */
import path from "node:path";
import { listProfiles } from "./profiles.mjs";
import { runSensors } from "./sensors.mjs";
import {
  approve,
  doctor,
  initProject,
  reject,
  startIntent,
  status,
  syncProjections,
} from "./engine.mjs";

const HELP = `aidlc-lite — motor de workflows didáctico inspirado en AI-DLC

Uso:
  node src/cli.mjs <comando> [opciones]

Comandos:
  init        Crea configuración, reglas y proyecciones
  run         Crea una intención y la deja en la primera etapa
  status      Muestra el estado de la intención activa
  approve     Aprueba el gate actual y avanza
  reject      Rechaza el gate actual y registra el motivo
  sync        Reprojecta las reglas a los harnesses configurados
  doctor      Valida configuración, estado, reglas y proyecciones
  sensors     Ejecuta sensores sobre un artefacto Markdown
  profiles    Lista los perfiles disponibles
  help        Muestra esta ayuda

Opciones:
  --dir <ruta>        Directorio del proyecto (por defecto: .)
  --harness <valor>   claude | cursor | both (por defecto: claude)
  --profile <nombre>  Perfil de workflow (por defecto: classic)
  --intent <texto>    Intención del trabajo
  --note <texto>      Nota para approve/reject
  --artifact <ruta>   Artefacto para sensors

Ejemplos:
  node src/cli.mjs init --dir .demo --harness both
  node src/cli.mjs run --dir .demo --profile feature --intent "API de inventario"
  node src/cli.mjs status --dir .demo
  node src/cli.mjs approve --dir .demo --note "revisado"
  node src/cli.mjs sensors --dir .demo --artifact .demo/artifacts/intent.md
`;

function parseArgs(argv) {
  const args = argv.slice(2);
  const command = args[0] && !args[0].startsWith("--") ? args[0] : "help";
  const flags = {};
  for (let index = command === "help" ? 0 : 1; index < args.length; index += 1) {
    const token = args[index];
    if (!token.startsWith("--")) continue;
    const key = token.slice(2);
    const next = args[index + 1];
    if (next && !next.startsWith("--")) {
      flags[key] = next;
      index += 1;
    } else {
      flags[key] = true;
    }
  }
  return { command, flags };
}

function resolveDir(flags) {
  return path.resolve(flags.dir && flags.dir !== true ? flags.dir : ".");
}

function printState(state) {
  const stage = state.stages[state.currentStage];
  console.log(`Intención : ${state.intent}`);
  console.log(`Perfil    : ${state.profile}`);
  console.log(`Estado    : ${state.status}`);
  console.log(
    `Etapa     : ${stage ? `${stage.id} — ${stage.title}` : "—"} (${state.currentStage + 1}/${state.stages.length})`
  );
  if (state.status !== "completed") {
    console.log("Gate      : pendiente (approve | reject)");
  } else {
    console.log("Gate      : completado");
  }
}

function printSummary(summary) {
  const number = summary.status === "completed" ? summary.total : summary.done + 1;
  console.log(`Intención : ${summary.intent}`);
  console.log(`Perfil    : ${summary.profile}`);
  console.log(`Estado    : ${summary.status}`);
  console.log(
    `Etapa     : ${summary.stage} — ${summary.stageTitle} (${number}/${summary.total})`
  );
  console.log(
    summary.pendingGate ? "Gate      : pendiente (approve | reject)" : "Gate      : completado"
  );
}

function main() {
  const { command, flags } = parseArgs(process.argv);
  const dir = resolveDir(flags);

  try {
    switch (command) {
      case "help":
        console.log(HELP);
        return 0;

      case "profiles": {
        for (const profile of listProfiles()) {
          console.log(
            `${profile.id.padEnd(16)} ${String(profile.stages).padStart(2)} etapas  ${profile.description}`
          );
        }
        return 0;
      }

      case "init": {
        const result = initProject(dir, { harnesses: flags.harness ?? "claude" });
        console.log(`✓ proyecto inicializado en ${result.dir}`);
        console.log(`  harnesses: ${result.config.harnesses.join(", ")}`);
        for (const file of result.written) {
          console.log(`  → ${path.relative(dir, file).replaceAll("\\", "/")}`);
        }
        return 0;
      }

      case "run": {
        if (!flags.intent || flags.intent === true) {
          throw new Error("run requiere --intent \"<texto>\"");
        }
        const state = startIntent(dir, {
          profile: flags.profile,
          intent: flags.intent,
        });
        console.log("✓ intención creada");
        printState(state);
        return 0;
      }

      case "status": {
        printSummary(status(dir));
        return 0;
      }

      case "approve": {
        const state = approve(dir, { note: flags.note === true ? "" : flags.note ?? "" });
        console.log("✓ gate aprobado");
        printState(state);
        return 0;
      }

      case "reject": {
        const state = reject(dir, { note: flags.note === true ? "" : flags.note ?? "" });
        console.log("✗ gate rechazado; la etapa sigue activa");
        printState(state);
        return 0;
      }

      case "sync": {
        const result = syncProjections(dir);
        console.log("✓ reglas proyectadas");
        for (const file of result.written) {
          console.log(`  → ${path.relative(dir, file).replaceAll("\\", "/")}`);
        }
        return 0;
      }

      case "doctor": {
        const report = doctor(dir);
        for (const problem of report.problems) console.log(`  ✗ ${problem}`);
        for (const warning of report.warnings) console.log(`  ! ${warning}`);
        console.log(
          `${report.ok ? "✓" : "✗"} doctor: ${report.problems.length} problemas, ${report.warnings.length} avisos, ${report.auditEvents} eventos de auditoría`
        );
        return report.ok ? 0 : 1;
      }

      case "sensors": {
        if (!flags.artifact || flags.artifact === true) {
          throw new Error("sensors requiere --artifact <ruta>");
        }
        const report = runSensors(path.resolve(flags.artifact));
        for (const finding of report.findings) {
          console.log(`  ! ${finding.code}: ${finding.message}`);
        }
        console.log(
          `${report.ok ? "✓" : "!"} sensors: ${report.findings.length} hallazgos`
        );
        return report.ok ? 0 : 2;
      }

      default:
        console.error(`comando desconocido: ${command}`);
        console.error(HELP);
        return 1;
    }
  } catch (error) {
    console.error(`error: ${error.message}`);
    return 1;
  }
}

process.exitCode = main();
