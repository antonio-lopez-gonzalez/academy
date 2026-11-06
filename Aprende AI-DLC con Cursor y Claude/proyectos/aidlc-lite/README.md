# aidlc-lite

Motor de workflows **didáctico** inspirado en AI-DLC. Reproduce en pequeño la
mecánica que enseña el curso:

- fases y etapas con perfil de workflow;
- gates de aprobación humana;
- estado persistente y audit trail en JSONL;
- reglas por capas (`org`, `team`, `project`);
- proyección de reglas a **Claude Code** y **Cursor**;
- sensores deterministas sobre artefactos.

> No es un reemplazo de AI-DLC, no llama a un LLM y no pretende usarse en
> producción. Sirve para entender el motor y practicar sin coste de tokens.

## Requisitos

- Node.js 22 LTS.
- Sin dependencias: no hace falta `npm install`.

## Uso rápido

```bash
# Desde esta carpeta
node src/cli.mjs --help

# Inicializar un proyecto de ejemplo
node src/cli.mjs init --dir .demo --harness both

# Crear una intención y ver el plan
node src/cli.mjs run --dir .demo --profile feature --intent "API de inventario"

# Consultar el estado
node src/cli.mjs status --dir .demo

# Aprobar la etapa actual y avanzar
node src/cli.mjs approve --dir .demo --note "revisado"

# Rechazar y volver atrás
node src/cli.mjs reject --dir .demo --note "faltan NFRs"

# Reprojectar reglas a los dos harnesses
node src/cli.mjs sync --dir .demo

# Comprobar la instalación
node src/cli.mjs doctor --dir .demo

# Ejecutar sensores sobre un artefacto (incluye un ejemplo listo)
node src/cli.mjs sensors --dir .demo --artifact examples/intent.md
```

## Estructura que crea

```text
<dir>/
├── .aidlc-lite/
│   ├── config.json          # harnesses, perfil por defecto
│   ├── state.json           # intención activa y etapas
│   └── audit.jsonl          # eventos append-only
├── rules/
│   ├── org.md               # reglas de la organización
│   ├── team.md              # reglas del equipo
│   └── project.md           # reglas del proyecto
├── artifacts/               # artefactos de ejemplo
├── .claude/CLAUDE.md        # proyección para Claude Code
└── .cursor/rules/aidlc-team.mdc  # proyección para Cursor
```

Código fuente:

```text
src/
├── cli.mjs          # comandos
├── engine.mjs       # operaciones de alto nivel
├── state.mjs        # máquina de estados
├── gates.mjs        # decisiones approve/request_changes/skip/reject
├── profiles.mjs     # rutas por perfil
├── rules.mjs        # reglas org/team/project
├── projection.mjs   # proyección a Claude y Cursor
├── sensors.mjs      # checks deterministas
├── audit.mjs        # JSONL append-only
└── config.mjs       # configuración
```

## Comandos

| Comando | Qué hace |
|---|---|
| `init` | Crea configuración, reglas y proyecciones vacías. |
| `run` | Crea una intención y la deja en la primera etapa. |
| `status` | Muestra perfil, etapa actual y gates pendientes. |
| `approve` | Aprueba el gate actual y avanza a la siguiente etapa. |
| `reject` | Rechaza el gate actual y deja constancia del motivo. |
| `sync` | Vuelve a proyectar las reglas a los harnesses configurados. |
| `doctor` | Valida configuración, estado, reglas y proyecciones. |
| `sensors` | Ejecuta sensores sobre un artefacto Markdown. |

## Perfiles incluidos

`classic`, `express`, `feature`, `enterprise`, `mvp`, `poc`, `bugfix`,
`refactor`, `infra`, `security-patch` y `workshop`.

Las rutas son **simplificadas**: cada perfil declara una secuencia corta de
etapas para que el proyecto sea legible y testeable. El curso explica que el
motor real de AI-DLC usa 30 etapas después del setup y ajusta profundidad,
estrategia de pruebas y techo de revisión.

## Pruebas

```bash
node --test
```

Cubren la máquina de estados, los gates, la fusión de reglas, la proyección
idempotente y los sensores.

## Relación con el curso

El curso `Aprende AI-DLC con Cursor y Claude` explica la metodología completa.
Este proyecto es su laboratorio: no necesitas Cursor ni Claude para ejecutarlo,
pero sí para probar los workflows reales de AI-DLC.
