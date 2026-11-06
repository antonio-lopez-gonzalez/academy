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
      dir: "modulos/01-claude-code",
      title: "Claude Code",
      description: "Instalar Claude Code, configurar AI-DLC, entender su estructura y ejecutar el primer workflow.",
      lessons: [
        { file: "01-instalar-claude-code.html", title: "Instalar Claude Code", minutes: 30 },
        { file: "02-instalar-aidlc-para-claude.html", title: "Instalar AI-DLC para Claude", minutes: 35 },
        { file: "03-estructura-y-doctor.html", title: "Estructura del proyecto y doctor", minutes: 35 },
        { file: "04-primer-workflow-en-claude.html", title: "El primer workflow en Claude", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-cursor",
      title: "Cursor",
      description: "Cursor IDE y CLI, configuración de AI-DLC, reglas, hooks y primer workflow en el editor.",
      lessons: [
        { file: "01-cursor-ide-y-cli.html", title: "Cursor IDE y Cursor CLI", minutes: 30 },
        { file: "02-instalar-aidlc-para-cursor.html", title: "Instalar AI-DLC para Cursor", minutes: 35 },
        { file: "03-reglas-y-hooks-en-cursor.html", title: "Reglas, hooks y subagentes", minutes: 35 },
        { file: "04-primer-workflow-en-cursor.html", title: "El primer workflow en Cursor", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-perfiles-y-comparativa",
      title: "Perfiles y comparativa",
      description: "Elegir el perfil correcto, ajustar profundidad y comparar Cursor con Claude Code.",
      lessons: [
        { file: "01-elegir-perfil.html", title: "Elegir el perfil correcto", minutes: 35 },
        { file: "02-depth-y-test-strategy.html", title: "Profundidad y estrategia de pruebas", minutes: 35 },
        { file: "03-claude-vs-cursor.html", title: "Claude Code vs Cursor", minutes: 35 },
        { file: "04-equipos-hibridos.html", title: "Equipos híbridos y sincronización", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-inception",
      title: "Inception",
      description: "De la intención al enunciado, la viabilidad, el diseño y el plan aprobado.",
      lessons: [
        { file: "01-intent-capture.html", title: "Intent capture", minutes: 35 },
        { file: "02-feasibility-y-requisitos.html", title: "Viabilidad y requisitos", minutes: 35 },
        { file: "03-diseno-y-plan.html", title: "Diseño y plan", minutes: 40 },
        { file: "04-gates-de-aprobacion.html", title: "Gates de aprobación", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-construction",
      title: "Construction",
      description: "Generación de código, pruebas, checkpoints, revisores y unidades de trabajo.",
      lessons: [
        { file: "01-code-generation.html", title: "Code generation", minutes: 40 },
        { file: "02-tests-y-checkpoints.html", title: "Tests y checkpoints", minutes: 35 },
        { file: "03-revisores-y-evidencias.html", title: "Revisores y evidencias", minutes: 35 },
        { file: "04-units-y-swarm.html", title: "Unidades y swarm", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-sensores-y-operacion",
      title: "Sensores y operación",
      description: "Sensores deterministas, guard policy, audit trail, despliegue y feedback.",
      lessons: [
        { file: "01-sensores-deterministas.html", title: "Sensores deterministas", minutes: 35 },
        { file: "02-guard-policy.html", title: "Guard policy", minutes: 35 },
        { file: "03-audit-trail.html", title: "Audit trail y estado", minutes: 35 },
        { file: "04-operacion-y-feedback.html", title: "Operación y feedback", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-proyecto-final",
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
