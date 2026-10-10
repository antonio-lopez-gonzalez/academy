/**
 * course-data.js — MAPA COMPLETO DEL CURSO NEW RELIC PARA .NET
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
  title: "New Relic para .NET",
  subtitle: "Observabilidad de tu WebSocket + API · agente APM + OpenTelemetry",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet NRQL + agente .NET" },
    { file: "recursos/glosario.html", title: "Glosario de observabilidad y New Relic" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Observabilidad y New Relic",
      description: "Qué es la observabilidad, la plataforma New Relic, su modelo de datos y el caso de estudio .NET.",
      lessons: [
        { file: "01-que-es-observabilidad.html", title: "Qué es la observabilidad (logs, métricas, trazas)", minutes: 30 },
        { file: "02-plataforma-y-modelo-de-datos.html", title: "La plataforma New Relic y el modelo de datos", minutes: 35 },
        { file: "03-cuenta-licencias-api-keys-regiones.html", title: "Cuenta, licencias, API keys y regiones", minutes: 30 },
        { file: "04-caso-de-estudio-websocket-api-dotnet.html", title: "Caso de estudio: servidor WebSocket + API en .NET", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-agente-apm",
      title: "El agente APM de New Relic para .NET",
      description: "Cómo funciona el agente .NET, instalación por NuGet, configuración y primeros datos en APM.",
      lessons: [
        { file: "01-como-funciona-el-agente-dotnet.html", title: "Cómo funciona el agente .NET (CLR profiling)", minutes: 35 },
        { file: "02-instalacion-y-compatibilidad.html", title: "Instalación y compatibilidad (NuGet)", minutes: 35 },
        { file: "03-configuracion-newrelic-config-y-env.html", title: "Configuración: newrelic.config y variables de entorno", minutes: 35 },
        { file: "04-primeros-datos-en-apm.html", title: "Primeros datos: APM Summary, Transactions y Errors", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-transacciones-metricas-errores",
      title: "Transacciones, Métricas y Errores",
      description: "Transacciones web, Apdex, métricas custom, captura de errores y atributos.",
      lessons: [
        { file: "01-transacciones-web-aspnet-core.html", title: "Transacciones web en ASP.NET Core", minutes: 30 },
        { file: "02-apdex-metricas-y-recordmetric.html", title: "Apdex, métricas clave y RecordMetric", minutes: 35 },
        { file: "03-errores-notice-error-y-mas.html", title: "Errores: NoticeError, esperados vs. no esperados", minutes: 30 },
        { file: "04-atributos-y-addcustomattribute.html", title: "Atributos y AddCustomAttribute", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-distributed-tracing",
      title: "Distributed Tracing y traceId",
      description: "Trazas y spans, W3C Trace Context, instrumentación custom y propagación manual del traceId.",
      lessons: [
        { file: "01-traces-spans-y-traceid.html", title: "Trazas, spans, traceId, guid y parentId", minutes: 35 },
        { file: "02-w3c-trace-context.html", title: "W3C Trace Context: traceparent y tracestate", minutes: 35 },
        { file: "03-instrumentacion-custom-transacciones-segmentos.html", title: "Instrumentación custom: atributos [Transaction] y [Trace]", minutes: 40 },
        { file: "04-propagacion-manual-del-traceid.html", title: "Propagación manual del traceId sin HTTP", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-websocket",
      title: "Instrumentar WebSocket",
      description: "El agente no ve los WebSocket por defecto: creamos transacciones, spans y atributos por mensaje.",
      lessons: [
        { file: "01-el-reto-instrumentar-websocket.html", title: "El reto: por qué el agente no ve los WebSocket", minutes: 30 },
        { file: "02-transaccion-por-mensaje-json.html", title: "Crear una transacción por mensaje JSON", minutes: 40 },
        { file: "03-spans-y-atributos-por-tipo-de-mensaje.html", title: "Spans y atributos por tipo de mensaje", minutes: 35 },
        { file: "04-caso-practico-websocket-completo.html", title: "Caso práctico: servidor WebSocket instrumentado", minutes: 45, project: true },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-logs-en-contexto",
      title: "Logs en Contexto y Datos JSON",
      description: "Logs conectados a trazas y errores desde .NET, estructura JSON y forwarding.",
      lessons: [
        { file: "01-logs-en-contexto-dotnet.html", title: "Logs en contexto en .NET", minutes: 30 },
        { file: "02-correlacion-logs-trazas-errores.html", title: "Correlacionar logs con trazas y errores", minutes: 30 },
        { file: "03-estructura-json-mensajes-y-log-forwarding.html", title: "Estructura JSON de los mensajes y forwarding", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-opentelemetry",
      title: "OpenTelemetry (OTLP a New Relic)",
      description: "OpenTelemetry vs. agente APM, SDK en .NET 8, exportador OTLP y coexistencia.",
      lessons: [
        { file: "01-opentelemetry-vs-agente-apm.html", title: "OpenTelemetry vs. agente APM", minutes: 30 },
        { file: "02-sdk-opentelemetry-dotnet-8.html", title: "SDK de OpenTelemetry en .NET 8", minutes: 40 },
        { file: "03-exportar-otlp-a-new-relic.html", title: "Exportar OTLP a New Relic", minutes: 35 },
        { file: "04-coexistir-agente-y-otel.html", title: "Coexistencia del agente APM con OTel", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-nrql-dashboards-alertas",
      title: "NRQL, Dashboards y Alertas",
      description: "El lenguaje de consulta de New Relic: NRQL, dashboards y alertas de condiciones.",
      lessons: [
        { file: "01-sintaxis-nrql-basica.html", title: "Sintaxis NRQL: SELECT, FROM, WHERE, SINCE, TIMESERIES", minutes: 40 },
        { file: "02-consultar-transacciones-spans-logs.html", title: "Consultar Transactions, Spans, Logs y custom events", minutes: 30 },
        { file: "03-dashboards-y-charts.html", title: "Dashboards y charts", minutes: 35 },
        { file: "04-alertas-y-nrql-conditions.html", title: "Alert policies y NRQL conditions", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 8 */
    {
      id: "08",
      dir: "modulos/08-proyecto-final",
      title: "Proyecto Final: WebSocket + API",
      description: "Instrumentar de extremo a extremo el servidor WebSocket + Minimal API y verificarlo en la UI.",
      lessons: [
        { file: "01-arquitectura-websocket-minimal-api.html", title: "Arquitectura: WebSocket + Minimal API", minutes: 40, project: true },
        { file: "02-agente-y-distributed-tracing.html", title: "Agente, distributed tracing y traceId", minutes: 45, project: true },
        { file: "03-custom-events-metricas-y-logs.html", title: "Custom events, métricas y logs en contexto", minutes: 45, project: true },
        { file: "04-dashboard-alerta-y-verificacion.html", title: "Dashboard, alerta y verificación end-to-end", minutes: 45, project: true },
      ],
    },
  ],
};