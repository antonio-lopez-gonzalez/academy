/**
 * course-data.js — MAPA COMPLETO DEL CURSO SPEC-DRIVEN DEVELOPMENT
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
  title: "Spec-Driven Development",
  subtitle: "De la intención al código · Lumen + agentes + specs versionadas",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet: plantillas SDD" },
    { file: "recursos/glosario.html", title: "Glosario de Spec-Driven Development" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Preparación: por qué SDD ahora",
      description: "Qué es Spec-Driven Development, en qué se diferencia de prompts y tickets, el entorno de trabajo y el caso de estudio Lumen.",
      lessons: [
        { file: "01-que-es-sdd.html", title: "Qué es Spec-Driven Development", minutes: 30 },
        { file: "02-specs-vs-prompts-vs-tickets.html", title: "Spec vs prompt vs ticket vs README", minutes: 30 },
        { file: "03-entorno-git-agente.html", title: "Entorno: Git, Markdown y un agente", minutes: 25 },
        { file: "04-caso-lumen.html", title: "Caso de estudio: Lumen", minutes: 25 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-anatomia-de-una-spec",
      title: "Anatomía de una spec",
      description: "Objetivo y no-objetivos, restricciones y glosario, criterios de aceptación verificables y lo que nunca va en una spec.",
      lessons: [
        { file: "01-objetivo-y-no-objetivos.html", title: "Objetivo, audiencia y no-objetivos", minutes: 30 },
        { file: "02-restricciones-y-glosario.html", title: "Restricciones, glosario y supuestos", minutes: 30 },
        { file: "03-criterios-de-aceptacion.html", title: "Criterios de aceptación verificables", minutes: 35 },
        { file: "04-que-no-va-en-una-spec.html", title: "Lo que no va en una spec", minutes: 25 },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-ciclo-sdd",
      title: "El ciclo specify → plan → implement → verify",
      description: "Las cuatro fases del ciclo, la spec como fuente de verdad, el plan antes del código y la verificación contra la spec.",
      lessons: [
        { file: "01-el-ciclo-sdd.html", title: "Las cuatro fases del ciclo", minutes: 30 },
        { file: "02-la-spec-como-fuente-de-verdad.html", title: "La spec como fuente de verdad", minutes: 30 },
        { file: "03-plan-antes-de-codigo.html", title: "El plan antes del código", minutes: 30 },
        { file: "04-verificar-contra-la-spec.html", title: "Verificar contra la spec", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-specs-para-agentes",
      title: "Specs para agentes",
      description: "AGENTS.md, skills reutilizables, qué meter en contexto y dónde se queda el humano en el bucle.",
      lessons: [
        { file: "01-agents-md.html", title: "AGENTS.md: la puerta del repo", minutes: 30 },
        { file: "02-skills.html", title: "Skills: el cómo reutilizable", minutes: 30 },
        { file: "03-contexto-y-limites.html", title: "Contexto y límites del agente", minutes: 30 },
        { file: "04-humano-en-el-bucle.html", title: "El humano en el bucle", minutes: 25 },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-contratos-y-specs-tecnicas",
      title: "Contratos y specs técnicas",
      description: "Contratos de datos con JSON Schema, ADRs, la relación con TDD/BDD y OpenAPI como spec de APIs.",
      lessons: [
        { file: "01-contratos-de-datos.html", title: "Contratos de datos con JSON Schema", minutes: 35 },
        { file: "02-adr.html", title: "ADRs: una decisión, un contexto", minutes: 30 },
        { file: "03-tdd-bdd-y-sdd.html", title: "TDD, BDD y SDD: primos, no rivales", minutes: 30 },
        { file: "04-openapi-como-spec.html", title: "OpenAPI como spec en una lección", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-cambios",
      title: "Cambios: el 90 % del trabajo real",
      description: "Specs de delta, compatibilidad y roturas, cómo revisar una spec y qué hacer cuando la spec está mal.",
      lessons: [
        { file: "01-specs-de-delta.html", title: "Specs de delta: estado actual → deseado", minutes: 30 },
        { file: "02-compatibilidad-y-roturas.html", title: "Compatibilidad y roturas", minutes: 30 },
        { file: "03-review-de-specs.html", title: "Revisar una spec antes de implementar", minutes: 30 },
        { file: "04-cuando-la-spec-esta-mal.html", title: "Cuando la spec está mal", minutes: 25 },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-oficio-y-anti-patrones",
      title: "Oficio y anti-patrones",
      description: "Calibrar cuánta spec basta, vibe-coding con criterio, spec theatre y convenciones de equipo humano-agente.",
      lessons: [
        { file: "01-over-spec-y-under-spec.html", title: "Over-spec y under-spec", minutes: 30 },
        { file: "02-vibe-coding-vs-sdd.html", title: "Vibe-coding vs SDD", minutes: 30 },
        { file: "03-spec-theatre.html", title: "Spec theatre: documentos que nadie lee", minutes: 25 },
        { file: "04-equipo-humano-agente.html", title: "Equipo humano-agente: convenciones", minutes: 25 },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-proyecto-final",
      title: "Proyecto final: Lumen end-to-end",
      description: "Spec completa de Lumen, plan de implementación, implementación verificada y un cambio con retrospectiva.",
      lessons: [
        { file: "01-escribir-la-spec.html", title: "Escribir la spec de Lumen", minutes: 35 },
        { file: "02-plan-de-implementacion.html", title: "Plan de implementación", minutes: 30 },
        { file: "03-implementar-y-verificar.html", title: "Implementar y verificar", minutes: 35 },
        { file: "04-cambio-y-cierre.html", title: "Un cambio y cierre del curso", minutes: 30 },
      ],
    },
  ],
};