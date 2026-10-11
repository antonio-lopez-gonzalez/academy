/**
 * course-data.js — MAPA COMPLETO DEL CURSO SYSTEM DESIGN
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
  title: "Curso System Design",
  subtitle: "Disena sistemas a escala — de los requisitos a la arquitectura",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de System Design" },
    { file: "recursos/glosario.html", title: "Glosario de terminos" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MODULO 0 */
    {
      id: "00",
      dir: "modulos/00-fundamentos",
      title: "Fundamentos",
      description: "El metodo de diseno, requisitos funcionales y no funcionales, estimacion de capacidad y SLOs.",
      lessons: [
        { file: "01-que-es-system-design.html", title: "Que es System Design", minutes: 30 },
        { file: "02-requisitos-y-no-funcionales.html", title: "Requisitos y no funcionales", minutes: 35 },
        { file: "03-estimacion-de-capacidad.html", title: "Estimacion de capacidad", minutes: 35 },
        { file: "04-latencia-disponibilidad-slo.html", title: "Latencia, disponibilidad y SLO", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 1 */
    {
      id: "01",
      dir: "modulos/01-escalabilidad",
      title: "Escalabilidad",
      description: "Escalado vertical y horizontal, balanceo de carga, servicios sin estado y CDN.",
      lessons: [
        { file: "01-escalado-vertical-horizontal.html", title: "Escalado vertical y horizontal", minutes: 30 },
        { file: "02-balanceo-de-carga.html", title: "Balanceo de carga", minutes: 35 },
        { file: "03-stateless-y-sesiones.html", title: "Servicios sin estado y sesiones", minutes: 30 },
        { file: "04-cdn-y-edge.html", title: "CDN y computo en el borde", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 2 */
    {
      id: "02",
      dir: "modulos/02-datos",
      title: "Datos",
      description: "SQL vs NoSQL, particionado y sharding, replicacion y consistencia, indices y modelado.",
      lessons: [
        { file: "01-sql-vs-nosql.html", title: "SQL vs NoSQL", minutes: 35 },
        { file: "02-particionado-y-sharding.html", title: "Particionado y sharding", minutes: 35 },
        { file: "03-replicacion-y-consistencia.html", title: "Replicacion y consistencia", minutes: 35 },
        { file: "04-indices-y-modelado.html", title: "Indices y modelado a escala", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 3 */
    {
      id: "03",
      dir: "modulos/03-cache-consistencia",
      title: "Cache y consistencia",
      description: "Estrategias de cache, invalidacion y TTL, teorema CAP y consistencia eventual.",
      lessons: [
        { file: "01-estrategias-de-cache.html", title: "Estrategias de cache", minutes: 35 },
        { file: "02-invalidacion-y-ttl.html", title: "Invalidacion y TTL", minutes: 30 },
        { file: "03-teorema-cap.html", title: "El teorema CAP", minutes: 35 },
        { file: "04-consistencia-eventual.html", title: "Consistencia eventual", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 4 */
    {
      id: "04",
      dir: "modulos/04-mensajeria-asincronia",
      title: "Mensajeria y asincronia",
      description: "Colas vs streams, patrones de mensajeria, idempotencia y CQRS/event sourcing.",
      lessons: [
        { file: "01-colas-vs-streams.html", title: "Colas vs streams", minutes: 35 },
        { file: "02-patrones-de-mensajeria.html", title: "Patrones de mensajeria", minutes: 35 },
        { file: "03-idempotencia-exactly-once.html", title: "Idempotencia y exactly-once", minutes: 30 },
        { file: "04-cqrs-event-sourcing.html", title: "CQRS y event sourcing", minutes: 35 },
      ],
    },
    /* ============================================================ MODULO 5 */
    {
      id: "05",
      dir: "modulos/05-resiliencia",
      title: "Resiliencia",
      description: "Timeouts y reintentos, cortacircuitos, degradacion elegante y observabilidad con SLOs.",
      lessons: [
        { file: "01-timeouts-retries-backoff.html", title: "Timeouts, reintentos y backoff", minutes: 30 },
        { file: "02-circuit-breaker-bulkhead.html", title: "Cortacircuitos y bulkhead", minutes: 35 },
        { file: "03-degradacion-graceful.html", title: "Degradacion elegante", minutes: 30 },
        { file: "04-observabilidad-y-slos.html", title: "Observabilidad y SLOs", minutes: 35 },
      ],
    },
    /* ============================================================ MODULO 6 */
    {
      id: "06",
      dir: "modulos/06-casos-clasicos",
      title: "Casos clasicos",
      description: "Disenos completos: URL shortener, rate limiter, feed de noticias y chat en tiempo real.",
      lessons: [
        { file: "01-url-shortener.html", title: "Caso: URL shortener", minutes: 40 },
        { file: "02-rate-limiter.html", title: "Caso: rate limiter", minutes: 35 },
        { file: "03-feed-de-noticias.html", title: "Caso: feed de noticias", minutes: 40 },
        { file: "04-chat-colaborativo.html", title: "Caso: chat colaborativo", minutes: 40 },
      ],
    },
    /* ============================================================ MODULO 7 */
    {
      id: "07",
      dir: "modulos/07-proyecto-final",
      title: "Proyecto final",
      description: "Aplica el metodo a un sistema completo: diseno, trade-offs/ADRs y revision critica.",
      lessons: [
        { file: "01-metodologia-de-diseno.html", title: "Metodologia de diseno", minutes: 40, project: true },
        { file: "02-diseno-de-un-sistema.html", title: "Diseno de un sistema", minutes: 45, project: true },
        { file: "03-trade-offs-y-adr.html", title: "Trade-offs y ADRs", minutes: 40, project: true },
        { file: "04-revision-y-presentacion.html", title: "Revision y presentacion", minutes: 40, project: true },
      ],
    },
  ],
};
