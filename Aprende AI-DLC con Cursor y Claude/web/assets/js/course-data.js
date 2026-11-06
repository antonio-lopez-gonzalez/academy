/**
 * course-data.js — MAPA COMPLETO DEL CURSO AI-DLC CON CURSOR Y CLAUDE
 *
 * Única fuente de verdad para:
 *  - el sidebar (árbol de módulos y lecciones)
 *  - la navegación anterior / siguiente
 *  - el índice de la portada
 *  - el cálculo de progreso
 *
 * Cada lección apunta a un archivo HTML real dentro de web/modulos/.
 * Si añades o renombras una lección, edita SOLO este archivo.
 */
window.COURSE = {
  title: "Aprende AI-DLC con Cursor y Claude",
  subtitle: "Ciclo de vida auditable para desarrollo asistido por IA",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de AI-DLC" },
    { file: "recursos/glosario.html", title: "Glosario de términos" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-fundamentos",
      title: "Fundamentos",
      description: "Qué es AI-DLC, por qué existe, sus fases, sus etapas y sus perfiles de workflow.",
      lessons: [
        { file: "01-que-es-aidlc.html", title: "Qué es AI-DLC", minutes: 30 },
        { file: "02-vibe-coding-vs-aidlc.html", title: "Vibe coding vs AI-DLC", minutes: 30 },
        { file: "03-fases-y-etapas.html", title: "Las 5 fases y 33 etapas", minutes: 35 },
        { file: "04-perfiles-de-workflow.html", title: "Los 11 perfiles de workflow", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-initialization",
      title: "Initialization",
      description: "La fase de arranque: configurar el proyecto, crear el estado, entender spaces e intents y elegir harness.",
      lessons: [
        { file: "01-que-es-initialization.html", title: "Qué es Initialization", minutes: 30 },
        { file: "02-configurar-el-proyecto.html", title: "Configurar el proyecto", minutes: 35 },
        { file: "03-spaces-e-intents.html", title: "Spaces, intents y estado", minutes: 35 },
        { file: "04-claude-vs-cursor.html", title: "Claude Code vs Cursor", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-claude-code",
      title: "Claude Code",
      description: "Instalar Claude Code, configurar AI-DLC, entender su estructura y ejecutar el primer workflow.",
      lessons: [
        { file: "01-instalar-claude-code.html", title: "Instalar Claude Code", minutes: 30 },
        { file: "02-instalar-aidlc-para-claude.html", title: "Instalar AI-DLC para Claude", minutes: 35 },
        { file: "03-estructura-y-doctor.html", title: "Estructura del proyecto y doctor", minutes: 35 },
        { file: "04-primer-workflow-en-claude.html", title: "El primer workflow en Claude", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-cursor",
      title: "Cursor",
      description: "Cursor IDE y CLI, configuración de AI-DLC, reglas, hooks y primer workflow en el editor.",
      lessons: [
        { file: "01-cursor-ide-y-cli.html", title: "Cursor IDE y Cursor CLI", minutes: 30 },
        { file: "02-instalar-aidlc-para-cursor.html", title: "Instalar AI-DLC para Cursor", minutes: 35 },
        { file: "03-reglas-y-hooks-en-cursor.html", title: "Reglas, hooks y subagentes", minutes: 35 },
        { file: "04-primer-workflow-en-cursor.html", title: "El primer workflow en Cursor", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-ideation",
      title: "Ideation",
      description: "De la intención a una idea definida: captura, viabilidad, alternativas, riesgos y gate de ideación.",
      lessons: [
        { file: "01-intent-capture.html", title: "Intent capture", minutes: 35 },
        { file: "02-viabilidad-y-restricciones.html", title: "Viabilidad y restricciones", minutes: 35 },
        { file: "03-alternativas-y-riesgos.html", title: "Alternativas, riesgos y supuestos", minutes: 35 },
        { file: "04-gate-de-ideation.html", title: "Gate de Ideation", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-inception",
      title: "Inception",
      description: "De la idea a la spec y al plan: requisitos, diseño, unidades, trazabilidad y gate de Inception.",
      lessons: [
        { file: "01-requisitos-y-nfrs.html", title: "Requisitos y NFRs", minutes: 35 },
        { file: "02-diseno-y-plan.html", title: "Diseño y plan", minutes: 40 },
        { file: "03-plan-unidades-y-trazabilidad.html", title: "Plan, unidades y trazabilidad", minutes: 40 },
        { file: "04-gate-de-inception.html", title: "Gate de Inception", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-construction",
      title: "Construction",
      description: "Generación de código, pruebas, checkpoints, revisores, sensores, guardas y unidades de trabajo.",
      lessons: [
        { file: "01-code-generation.html", title: "Code generation", minutes: 40 },
        { file: "02-tests-y-checkpoints.html", title: "Tests y checkpoints", minutes: 35 },
        { file: "03-revisores-y-evidencias.html", title: "Revisores, sensores y evidencias", minutes: 35 },
        { file: "04-units-y-swarm.html", title: "Unidades, swarm y guard policy", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-operation",
      title: "Operation",
      description: "Desplegar, verificar, escuchar el feedback, gestionar incidentes y convertir lo aprendido en reglas.",
      lessons: [
        { file: "01-deploy-y-verificacion.html", title: "Deploy y verificación", minutes: 35 },
        { file: "02-feedback-e-incidentes.html", title: "Feedback e incidentes", minutes: 35 },
        { file: "03-auditoria-y-reconstruccion.html", title: "Auditoría y reconstrucción", minutes: 35 },
        { file: "04-learnings-y-mejora-continua.html", title: "Learnings y mejora continua", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 8 */
    {
      id: "08",
      dir: "modulos/08-proyecto-final",
      title: "Proyecto final",
      description: "Construir aidlc-lite: un motor de workflows con estado, gates, reglas y proyección a ambos harnesses.",
      lessons: [
        { file: "01-arquitectura-aidlc-lite.html", title: "Arquitectura de aidlc-lite", minutes: 40, project: true },
        { file: "02-estado-y-gates.html", title: "Estado y gates", minutes: 45, project: true },
        { file: "03-reglas-y-proyeccion.html", title: "Reglas y proyección", minutes: 40, project: true },
        { file: "04-pruebas-y-operacion.html", title: "Pruebas y operación", minutes: 40, project: true },
      ],
    },
  ],
};
