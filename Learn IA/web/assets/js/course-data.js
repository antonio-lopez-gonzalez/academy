/**
 * course-data.js — MAPA COMPLETO DEL CURSO IA
 *
 * Unica fuente de verdad para:
 *  - el sidebar (arbol de modulos y lecciones)
 *  - la navegacion anterior / siguiente
 *  - el indice de la portada
 *  - el calculo de progreso
 *
 * Cada leccion apunta a un archivo HTML real dentro de web/modulos/.
 * Si anades o renombras una leccion, edita SOLO este archivo.
 */
window.COURSE = {
  title: "Curso IA",
  subtitle: "Del LLM al agente en produccion",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de IA" },
    { file: "recursos/glosario.html", title: "Glosario de terminos" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MODULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Preparacion",
      description: "Que es un LLM, tokens y contexto, el ecosistema y el entorno de trabajo con uv y Ollama.",
      lessons: [
        { file: "01-que-es-un-llm.html", title: "Que es (y que no es) un LLM", minutes: 30 },
        { file: "02-tokens-contexto-sampling.html", title: "Tokens, contexto y sampling", minutes: 35 },
        { file: "03-ecosistema-y-entorno.html", title: "Ecosistema y entorno (uv + Ollama)", minutes: 30 },
        { file: "04-primera-llamada.html", title: "Primera llamada al modelo", minutes: 35 },
      ],
    },
    /* ============================================================ MODULO 1 */
    {
      id: "01",
      dir: "modulos/01-prompting-ingenieria",
      title: "Prompting de ingenieria",
      description: "Plantillas, few-shot, salida estructurada con pydantic y seguridad frente a la inyeccion de prompt.",
      lessons: [
        { file: "01-prompting-estructurado.html", title: "Prompting estructurado", minutes: 30 },
        { file: "02-few-shot-y-roles.html", title: "Few-shot y roles", minutes: 30 },
        { file: "03-structured-output.html", title: "Salida estructurada", minutes: 35 },
        { file: "04-seguridad-de-prompt.html", title: "Seguridad de prompt", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 2 */
    {
      id: "02",
      dir: "modulos/02-apis-sdks",
      title: "APIs y SDKs",
      description: "Llamadas y streaming, tool calling, multiproveedor, manejo de errores y control de coste y latencia.",
      lessons: [
        { file: "01-llamadas-y-streaming.html", title: "Llamadas y streaming", minutes: 30 },
        { file: "02-tool-function-calling.html", title: "Tool / function calling", minutes: 35 },
        { file: "03-multiproveedor-y-errores.html", title: "Multiproveedor y errores", minutes: 30 },
        { file: "04-coste-latencia-tokens.html", title: "Coste, latencia y tokens", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 3 */
    {
      id: "03",
      dir: "modulos/03-embeddings-busqueda",
      title: "Embeddings y busqueda semantica",
      description: "Vectores, similitud, chunking de documentos y bases vectoriales (pgvector y sqlite-vec).",
      lessons: [
        { file: "01-que-son-los-embeddings.html", title: "Que son los embeddings", minutes: 30 },
        { file: "02-similitud-y-vectores.html", title: "Similitud y vectores", minutes: 30 },
        { file: "03-chunking.html", title: "Chunking de documentos", minutes: 35 },
        { file: "04-bases-vectoriales.html", title: "Bases vectoriales", minutes: 35 },
      ],
    },
    /* ============================================================ MODULO 4 */
    {
      id: "04",
      dir: "modulos/04-rag",
      title: "RAG",
      description: "Pipeline de RAG, retrieval y reranking, citas y fidelidad, y evaluacion de la recuperacion.",
      lessons: [
        { file: "01-pipeline-rag.html", title: "El pipeline de RAG", minutes: 35 },
        { file: "02-retrieval-y-reranking.html", title: "Retrieval y reranking", minutes: 35 },
        { file: "03-citas-y-fidelidad.html", title: "Citas y fidelidad", minutes: 30 },
        { file: "04-evaluacion-de-rag.html", title: "Evaluacion de RAG", minutes: 35 },
      ],
    },
    /* ============================================================ MODULO 5 */
    {
      id: "05",
      dir: "modulos/05-agentes-tools",
      title: "Agentes y tools",
      description: "El bucle agentic, diseno de tools, planificacion y memoria, sandboxing y coste del bucle.",
      lessons: [
        { file: "01-bucle-agentic.html", title: "El bucle agentic", minutes: 35 },
        { file: "02-tools-y-handlers.html", title: "Tools y handlers", minutes: 35 },
        { file: "03-planificacion-y-memoria.html", title: "Planificacion y memoria", minutes: 35 },
        { file: "04-sandboxing-y-coste.html", title: "Sandboxing y coste del bucle", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 6 */
    {
      id: "06",
      dir: "modulos/06-extensibilidad",
      title: "Extensibilidad: MCP, skills y plugins",
      description: "Servidores MCP, tools/resources/prompts, skills (SKILL.md) y plugins con permisos.",
      lessons: [
        { file: "01-mcp-servidores.html", title: "Servidores MCP", minutes: 40 },
        { file: "02-tools-resources-prompts.html", title: "Tools, resources y prompts", minutes: 35 },
        { file: "03-skills-skill-md.html", title: "Skills (SKILL.md)", minutes: 30 },
        { file: "04-plugins-y-permisos.html", title: "Plugins y permisos", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 7 */
    {
      id: "07",
      dir: "modulos/07-evals-produccion",
      title: "Evals, calidad y produccion",
      description: "Datasets de evaluacion, LLM-as-judge, observabilidad con trazas y guardrails, coste y versionado.",
      lessons: [
        { file: "01-datasets-y-evals.html", title: "Datasets y evals", minutes: 35 },
        { file: "02-llm-as-judge.html", title: "LLM-as-judge", minutes: 35 },
        { file: "03-observabilidad-y-trazas.html", title: "Observabilidad y trazas", minutes: 35 },
        { file: "04-guardrails-coste.html", title: "Guardrails, coste y versionado", minutes: 35 },
      ],
    },
    /* ============================================================ MODULO 8 */
    {
      id: "08",
      dir: "modulos/08-proyecto-final",
      title: "Proyecto final: Copiloto de la Academy",
      description: "Arquitectura del Copiloto, RAG sobre los cursos reales, agente con tools y evals, y despliegue.",
      lessons: [
        { file: "01-arquitectura.html", title: "Arquitectura del Copiloto", minutes: 40, project: true },
        { file: "02-rag-sobre-los-cursos.html", title: "RAG sobre los cursos", minutes: 45, project: true },
        { file: "03-tools-y-agente.html", title: "El agente con tools", minutes: 45, project: true },
        { file: "04-evals-y-despliegue.html", title: "Evals y despliegue", minutes: 45, project: true },
      ],
    },
  ],
};
