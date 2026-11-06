# Curso Completo de AI-DLC con Cursor y Claude

## Ciclo de vida auditable para desarrollo asistido por IA

---

# 1. Objetivo

Actúa como alguien que lleva años entregando software con agentes de IA y ha sufrido el paso de la demo al equipo. El objetivo es crear **un curso completo de AI-DLC** (AI-Driven Development Life Cycle) orientado a los dos entornos que más se usan hoy: **Cursor** y **Claude Code**.

El curso enseña el **por qué**:

- Un chat que genera código no es un proceso de entrega: sin estado, sin gates y sin auditoría, la intención se pierde.
- AI-DLC no sustituye al equipo: convierte al humano en el que decide y al agente en el que ejecuta dentro de un ciclo verificable.
- El mismo motor corre en Cursor y en Claude Code; lo que cambia es el *shell* (dónde viven las reglas, cómo se invoca, cómo se renderizan las aprobaciones).
- Un perfil de workflow no es una plantilla de prompt: es una ruta de etapas con profundidad, estrategia de pruebas y techo de revisión.
- La auditoría y los sensores no son burocracia: son lo que permite reconstruir qué pasó y mejorar el proceso.

El hilo conductor es **un equipo pequeño que construye una API de inventario** y aprende a operarla con AI-DLC primero en Claude Code, luego en Cursor y finalmente en ambos a la vez.

> Stack fijado: **AI-DLC 2.x** (repositorio `awslabs/aidlc-workflows`, licencia MIT-0), **Claude Code** como harness CLI y **Cursor** como harness IDE. Node.js 22 LTS para el proyecto ejecutable. No se enseña a modificar el motor interno de AI-DLC; se enseña a usarlo y a entender su arquitectura.
>
> **Versión fijada:** línea estable **2.11.3** en el momento de la redacción. Los conceptos (5 fases, 33 etapas, 14 agentes, 11 perfiles, gates, sensores, audit trail) son los de 2.x; los detalles que cambian entre releases menores se señalan en la lección correspondiente. `aidlc-lite` es un motor didáctico inspirado en esta mecánica, no un clon de AI-DLC 2.x.

---

# 2. Audiencia y requisitos previos

- **Saber programar** y usar una terminal.
- Node.js 22+ para el proyecto `aidlc-lite`.
- Una cuenta de **Cursor** (plan de pago recomendado para modelos con nombre y agent mode).
- **Claude Code** instalado y autenticado (suscripción o API key de Anthropic).
- No se asume experiencia previa con AI-DLC. Sí se agradece haber visto agentes, herramientas y MCP (`Aprende IA`) o especificación y ciclos de vida (`Aprende SDD`).
- Ganas de entender el proceso, no solo de pedir código.

---

# 3. Hilo conductor: el equipo de inventario

Un equipo que crece con el curso:

1. **Una idea**: «necesitamos una API REST de inventario».
2. **Un perfil**: el equipo elige `feature` o `mvp` y recorre sus etapas.
3. **Dos entornos**: el mismo trabajo se ejecuta en Claude Code y en Cursor para comparar.
4. **Un gate**: cada etapa termina con revisión y aprobación humana.
5. **Una auditoría**: todo queda registrado en `aidlc/`.
6. **Un motor propio**: el proyecto final reconstruye la mecánica en pequeño.

El proyecto ejecutable vive en `proyectos/aidlc-lite/` y se ejecuta con Node.js, sin API de IA ni servicios externos.

---

# 4. Tecnologías

## Utilizar SIEMPRE

- **AI-DLC 2.x** instalado con el instalador nativo oficial.
- **Claude Code** (`aidlc config --harness claude`) como entorno CLI de referencia.
- **Cursor** (`aidlc config --harness cursor`) como entorno IDE de referencia.
- **Node.js 22 LTS**, módulos ESM, `node --test` para el proyecto.
- **Git** en el proyecto: Cursor puede omitir hooks fuera de un repositorio Git.
- **Markdown** para reglas, specs y conocimiento del equipo.
- **JSON/JSONL** para estado y auditoría del proyecto didáctico.

## No utilizar

- Prometer ejecución totalmente autónoma sin gates humanos.
- Enseñar a editar `dist/` o el motor interno de AI-DLC como si fuera código de aplicación.
- Depender de una API de IA para el proyecto del curso.
- Presentar AI-DLC como sustituto de tests, revisión de código o criterio de ingeniería.
- Comparaciones superficiales con «solo prompting» que confundan el objetivo.

---

# 5. Forma de enseñar

Cada lección sigue EXACTAMENTE las 10 secciones del contrato Academy:

1. **Introducción** — qué, por qué existe, qué problema resuelve, cuándo se usa, errores típicos.
2. **Teoría** — fases, etapas, perfiles, agentes, gates, sensores; diagramas ASCII.
3. **Ejemplo mínimo** — el comando, archivo o flujo más pequeño posible, explicado.
4. **Ejemplo real** — el equipo de inventario o un escenario profesional.
5. **Buenas prácticas** — qué hacer / qué evitar / por qué (tabla cuando aplique).
6. **Errores habituales** — incorrecto, por qué está mal y versión correcta.
7. **Ejercicio guiado** — paso a paso, resuelto, con resultado esperado.
8. **Reto** — reto adicional **sin resolver** (solo pista).
9. **Resumen** — bullets + conceptos clave.
10. **Preguntas frecuentes** — FAQ (3–5 `<details>`).

## Reglas de contenido

- Distingue siempre **metodología** (fases, etapas, gates) de **harness** (Claude Code, Cursor).
- Cuando algo difiera entre Cursor y Claude Code, dilo explícitamente y muestra ambos caminos.
- No inventes flags, comandos ni rutas: contrasta con la documentación oficial.
- Los diagramas de flujo se dibujan en ASCII (`idea → gate → etapa → artefacto`).
- El reto nunca se resuelve en el HTML.

---

# 6. Multiplataforma

- **Windows:** PowerShell para instalar AI-DLC (`irm ... | iex`) y comandos `aidlc`.
- **macOS / Linux / WSL:** `curl ... | sh`.
- Claude Code se ejecuta en terminal; Cursor puede usarse como IDE o con su CLI `agent`.
- El proyecto `aidlc-lite` funciona en cualquier sistema con Node.js 22.
- Rutas de ejemplo con separadores `/`; se avisa cuando Windows use `\`.

---

# 7. Índice del curso

## Módulo 0 — Fundamentos
- **01-que-es-aidlc.html** — Qué es AI-DLC, qué problema resuelve y qué no es.
- **02-vibe-coding-vs-aidlc.html** — Del chat que improvisa al ciclo de vida auditable.
- **03-fases-y-etapas.html** — Las 5 fases y las 33 etapas; inicialización, ideación, inception, construction y operation.
- **04-perfiles-de-workflow.html** — Los 11 perfiles: classic, express, feature, enterprise, mvp, poc, bugfix, refactor, infra, security-patch y workshop.

## Módulo 1 — Initialization
- **01-que-es-initialization.html** — La fase de arranque: setup del proyecto y estado inicial del intent.
- **02-configurar-el-proyecto.html** — `aidlc config`, qué escribe, qué commitear y cómo refrescar.
- **03-spaces-e-intents.html** — `aidlc/`, spaces, intents, `aidlc-state.md` y estado compartido.
- **04-claude-vs-cursor.html** — Comparativa de harnesses, equipos híbridos y sincronización.

## Módulo 2 — Claude Code
- **01-instalar-claude-code.html** — Requisitos, instalación y autenticación de Claude Code.
- **02-instalar-aidlc-para-claude.html** — Instalador nativo y `aidlc config --harness claude`.
- **03-estructura-y-doctor.html** — `.claude/`, `aidlc/`, `CLAUDE.md` y `aidlc doctor`.
- **04-primer-workflow-en-claude.html** — `/aidlc`, propuesta de plan y primer gate.

## Módulo 3 — Cursor
- **01-cursor-ide-y-cli.html** — Cursor IDE, CLI `agent`, requisitos y plan de pago.
- **02-instalar-aidlc-para-cursor.html** — `git init`, `aidlc config --harness cursor` y hooks.
- **03-reglas-y-hooks-en-cursor.html** — `.cursor/rules`, `.cursor/hooks.json` y subagentes.
- **04-primer-workflow-en-cursor.html** — `/aidlc`, `/aidlc-status` y diferencias de renderizado.

## Módulo 4 — Ideation
- **01-intent-capture.html** — De la petición al enunciado de intención.
- **02-viabilidad-y-restricciones.html** — Veredicto de viabilidad, evidencia y restricciones.
- **03-alternativas-y-riesgos.html** — Opciones, trade-offs, riesgos, supuestos y decisiones descartadas.
- **04-gate-de-ideation.html** — El gate que autoriza a pasar a Inception.

## Módulo 5 — Inception
- **01-requisitos-y-nfrs.html** — Requisitos funcionales, NFRs, criterios de aceptación y trazabilidad.
- **02-diseno-y-plan.html** — Diseño, ADR, unidades, plan y fingerprint.
- **03-plan-unidades-y-trazabilidad.html** — Unidades, dependencias, depth, test strategy y trazabilidad.
- **04-gate-de-inception.html** — Aprobar, cambiar, saltar o rechazar la spec y el plan.

## Módulo 6 — Construction
- **01-code-generation.html** — Generación de código bajo plan approval.
- **02-tests-y-checkpoints.html** — Pruebas, verificación y checkpoints por unidad.
- **03-revisores-y-evidencias.html** — Agentes revisores, sensores y evidencia ligada a fuentes.
- **04-units-y-swarm.html** — Unidades, orden stage-major/unit-major, swarm y guard policy.

## Módulo 7 — Operation
- **01-deploy-y-verificacion.html** — Despliegue, smoke tests, SLOs, rollback y gate de operación.
- **02-feedback-e-incidentes.html** — Telemetría, soporte, incidentes y postmortem.
- **03-auditoria-y-reconstruccion.html** — Estado, audit trail y reconstrucción de incidentes.
- **04-learnings-y-mejora-continua.html** — De la corrección a la regla, el sensor o el cambio de perfil.

## Módulo 8 — Proyecto final
- **01-arquitectura-aidlc-lite.html** — Diseño del motor didáctico. *(project)*
- **02-estado-y-gates.html** — Máquina de estados y gates. *(project)*
- **03-reglas-y-proyeccion.html** — Reglas por capas y proyección a Cursor/Claude. *(project)*
- **04-pruebas-y-operacion.html** — CLI, pruebas y operación. *(project)*

## Recursos
- **cheatsheet.html** — Comandos, perfiles, rutas y diferencias entre harnesses.
- **glosario.html** — Stage, gate, scope, sensor, guard, audit, harness…
- **faq.html** — FAQ general del curso.

**Total: 9 módulos (0-8), 36 lecciones + 3 recursos.**

---

# 8. Estructura

```text
Aprende AI-DLC con Cursor y Claude/
├── spec.md
├── AGENTS.md
├── package.json
├── pnpm-workspace.yaml
├── .gitattributes
├── web/
│   ├── index.html
│   ├── assets/{css,js,fonts,favicon.svg}
│   ├── modulos/00-fundamentos/ … 08-proyecto-final/
│   ├── recursos/{cheatsheet,glosario,faq}.html
│   └── descargas/Aprende-AI-DLC-con-Cursor-y-Claude.zip
├── scripts/serve-web.mjs
└── proyectos/aidlc-lite/       ← motor de workflows didáctico
```

---

# 9. Formato de entrega

Web estática sin build. Claves: `aidlc-theme`, `aidlc-progress`. `SITE_BASE` según el contrato (`./`, `../`, `../../`).

- Bloques de código: `language-bash` (instalación y CLI), `language-text` (salidas de AI-DLC y diagramas), `language-json` (estado/auditoría), `language-markdown` (reglas y specs), `language-javascript` (proyecto).
- El proyecto usa Node.js ESM y `node --test`; no requiere build.

---

# 10. Diseño y marca

- Paleta propia: **índigo de motor** (`#6366F1`) con acento **terracota Claude** (`#D97757`) sobre fondo oscuro azulado.
- Firma de portada: una sesión de terminal donde `/aidlc` propone un plan, recorre etapas y se detiene en un gate de aprobación; al lado, la misma etapa en Cursor con `/aidlc-status`.
- Logo del topbar y favicon: marca propia de **grafo de etapas** (tres nodos conectados), no un icono de terceros.
- Tema oscuro por defecto, toggle a claro, `prefers-reduced-motion` respetado.

---

# 11. Convenciones

1. Contenido, comentarios y UI en español; identificadores y comandos en inglés.
2. Toda afirmación sobre AI-DLC se contrasta con la documentación oficial; si cambia entre versiones, se indica.
3. Cursor y Claude Code se tratan como iguales: nunca se asume que uno es «el bueno».
4. El proyecto conductor es `proyectos/aidlc-lite/` y no depende de APIs de IA.
5. Cada lección usa SIEMPRE las 10 secciones; el reto no se resuelve.
6. Los enlaces entre lecciones usan rutas relativas del curso.

---

# 12. Documentación actualizada

Consultar la documentación oficial de AI-DLC (`awslabs/aidlc-workflows`, guía de usuario y guías de harness de Claude Code y Cursor) vía Context7 o el repositorio antes de afirmar comandos, flags, rutas o comportamientos. Preferir la versión estable más reciente. No inventar subcomandos ni rutas: si algo no está documentado, se dice.

---

# 13. Página principal (`web/index.html`)

Hero con eyebrow, firma animada, lead, botones (empezar, índice, cheatsheet, descarga del ZIP), estadísticas calculadas por `site.js`, callouts de «cómo funciona» e `<div id="module-list">`.

---

# 14. Checklist de implementación

## Fase 0 — Spec y AGENTS ✅
- [x] `spec.md` (este documento).
- [x] `AGENTS.md` (puntero corto).

## Fase 1 — Scaffold ✅
- [x] `package.json`, `pnpm-workspace.yaml`, `.gitattributes`, `scripts/serve-web.mjs`.
- [x] Assets re-tematizados, `course-data.js` completo, favicon propio.
- [x] Portada `web/index.html` + firma.

## Fase 2 — Lecciones ✅
- [x] 00-fundamentos … 08-proyecto-final (36 lecciones).

## Fase 3 — Recursos ✅
- [x] `web/recursos/{cheatsheet,glosario,faq}.html`.

## Fase 4 — Proyecto aidlc-lite ✅
- [x] `proyectos/aidlc-lite/` con motor, gates, reglas, proyección y tests.

## Fase 5 — Academy ✅
- [x] Tarjeta, contadores, filas en `specs.md` y `README.md`, ZIP.

---

# 15. Reglas importantes

1. No des por sabido AI-DLC: explica fase, etapa, gate y perfil desde cero.
2. Metodología y harness son cosas distintas; no las mezcles.
3. Cursor y Claude Code tienen diferencias reales: nómbralas y muestra ambos caminos.
4. Los gates son del humano; ningún agente se autoaprueba.
5. La auditoría no es opcional: es lo que hace reconstruible el trabajo.
6. Sin secretos en el repositorio; credenciales solo en `.env.example` cuando aplique.
7. Todo comando debe poder ejecutarse en Windows, macOS y Linux.
8. Si un concepto depende de otro, enlaza la lección correspondiente.
9. Prioriza comprensión y operación frente a memorizar la CLI.
10. El proyecto debe arrancar con `node src/cli.mjs --help` y pasar sus pruebas con `node --test`.
