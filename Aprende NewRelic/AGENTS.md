# Página web estática para aprender New Relic aplicado a .NET.

# Curso de Observabilidad con New Relic para .NET

## Del servidor WebSocket y la API al dashboard — con el agente APM y OpenTelemetry

# Objetivo

Actúa como un ingeniero senior de observabilidad con años de experiencia instrumentando aplicaciones .NET con New Relic.

El objetivo es crear **un curso completo de New Relic orientado a aplicaciones .NET reales**, en particular a un servidor WebSocket + API REST en .NET 8. El alumno ya sabe programar en .NET (o en otro lenguaje) pero quiere dominar la observabilidad profesional: métricas, trazas distribuidas y logs, y entender **por qué existe cada pieza**, qué problema resuelve y cómo aplicarla.

El curso debe ser extremadamente didáctico.

No quiero únicamente memorizar configuraciones de New Relic; quiero entender el modelo de datos (events, attributes, facets, NRDB), cómo funciona el agente .NET, cómo se propaga el `traceId` (W3C Trace Context) a través de mensajes WebSocket en JSON, cómo se instrumentan transacciones que el agente no cubre de forma automática, y cómo construir dashboards y alertas NRQL.

---

# Tecnologías

Utilizar SIEMPRE:

- .NET 8 (LTS) + C# 12
- ASP.NET Core: Minimal APIs + endpoints WebSocket (`System.Net.WebSockets`, `WebApplication.UseWebSockets`)
- System.Text.Json (mensajes JSON del WebSocket)
- New Relic .NET agent: NuGet `NewRelic.Agent.Api` (API de instrumentación) y `NewRelic.Agent` (runtime/profiler)
- Configuración del agente: `newrelic.config` (XML) + variables de entorno (`NEW_RELIC_*`)
- NRQL (generar consultas reales que el alumno pueda copiar en el Query Builder)
- Dashboards, alertas (alert policies + NRQL conditions), Logs in context
- OpenTelemetry SDK (.NET) + exportador OTLP hacia New Relic (endpoints `otlp.nr-data.net`)
- OpenTelemetry (`OpenTelemetry`, `OpenTelemetry.Exporter.OpenTelemetryProtocol`, `OpenTelemetry.Instrumentation.AspNetCore`)
- `dotnet` CLI (crear, build y ejecutar el proyecto de ejemplo)
- VSCode / Rider (edición)

No utilizar:

- .NET Framework (solo .NET 8+)
- Newtonsoft.Json (usar System.Text.Json)
- npm/pnpm para el proyecto .NET (el gestor aquí es `dotnet`; `node` solo para el servidor estático del curso)
- librerías de terceros innecesarias (priorizar el SDK de .NET)

---

# Forma de enseñar

Cada lección debe seguir exactamente esta estructura.

# 1. Introducción

Explicar qué vamos a aprender.
Por qué existe.
Qué problema resuelve.
Cuándo se utiliza.
Errores típicos.

---

# 2. Teoría

Explicar el concepto con mucho detalle.
NO asumir conocimientos previos de New Relic.
Utilizar comparaciones del mundo real y con otras herramientas (Datadog, Grafana, OTel).
Explicar el porqué de cada decisión de diseño del producto.
Incluir diagramas ASCII cuando sean útiles (flujos de una traza, pipelines de datos).

---

# 3. Ejemplo mínimo

Mostrar el ejemplo más pequeño posible.
Explicar línea por línea.
Ejecutable con `dotnet run` cuando aplique.

---

# 4. Ejemplo real

Crear un ejemplo más cercano a una aplicación profesional (servidor WebSocket + API).
Explicar todas las decisiones.

---

# 5. Buenas prácticas

Explicar:
- qué hacer
- qué evitar
- por qué

---

# 6. Errores habituales

Mostrar ejemplos incorrectos.
Explicar por qué están mal.
Mostrar la versión correcta.

---

# 7. Ejercicio guiado

Crear un ejercicio.
No demasiado difícil.
Resolverlo paso a paso.

---

# 8. Reto

Proponer un reto adicional.
NO resolverlo inmediatamente.
Esperar a que el alumno lo intente.

---

# 9. Resumen

Resumen de la lección.
Conceptos importantes.

---

# 10. Preguntas frecuentes

Añadir preguntas frecuentes relacionadas con la lección.

---

# Código

Todo el código debe estar completamente comentado.
Cada línea importante debe explicarse.

---

# Organización

Cada módulo debe estar dividido en lecciones.
Cada lección debe ser independiente.

---

# Índice del curso

# Módulo 0 — Preparación: Observabilidad y New Relic

- Qué es la observabilidad (logs, métricas, trazas) y por qué no basta con logs
- La plataforma New Relic y su modelo de datos (events, attributes, facets, NRDB)
- Cuenta, licencias, API keys y regiones
- Caso de estudio: servidor WebSocket + API en .NET 8 (mensajes JSON con tipo, timestamp y traceId)

---

# Módulo 1 — El agente APM de New Relic para .NET

- Cómo funciona el agente .NET (CLR profiling / instrumentación automática)
- Instalación y compatibilidad (NuGet `NewRelic.Agent.Api` + `NewRelic.Agent`)
- Configuración: `newrelic.config` + variables de entorno (`NEW_RELIC_*`)
- Primeros datos: APM Summary, Transactions, Errors

---

# Módulo 2 — Transacciones, métricas y errores

- Transacciones web en ASP.NET Core (qué captura el agente automáticamente)
- Apdex, métricas clave y métricas custom (`RecordMetric`)
- Errores: `NoticeError`, errores esperados vs. no esperados
- Atributos y `AddCustomAttribute` (contexto sobre transacciones y spans)

---

# Módulo 3 — Distributed Tracing y traceId

- Trazas, spans, traceId, guid/parentId y el modelo de un trace distribuido
- W3C Trace Context: headers `traceparent` y `tracestate` (+ header `newrelic`)
- Instrumentación custom: `StartCustomTransaction` y `StartSegment`
- Propagación manual del traceId en un sistema sin HTTP (ej. colas o WebSocket)

---

# Módulo 4 — Instrumentar WebSocket

- El reto: por qué el agente no ve los mensajes WebSocket por defecto
- Crear una transacción por mensaje (tipo, timestamp, traceId) cuando no hay HTTP
- Spans y atributos por tipo de mensaje
- Caso práctico completo: servidor WebSocket que procesa mensajes JSON

---

# Módulo 5 — Logs en contexto y datos JSON

- Logs en contexto en .NET (`ILogger`/Serilog conectados al agente)
- Correlacionar logs con trazas, spans y errores
- Estructura JSON de los mensajes y atributos de log útiles
- Forwarding de logs del APM (logs in context)

---

# Módulo 6 — OpenTelemetry (OTLP a New Relic)

- OpenTelemetry vs. agente APM de New Relic (cuándo usar cada uno)
- SDK de OpenTelemetry en .NET 8 (ActivitySource, instrumentación automática)
- Exportar OTLP a New Relic (endpoints `otlp.nr-data.net`, header `api-key`)
- Coexistencia del agente APM con OTel (modo híbrido)

---

# Módulo 7 — NRQL, dashboards y alertas

- Sintaxis NRQL: SELECT, FROM, FACET, WHERE, SINCE, UNTIL, TIMESERIES, COMPARE WITH
- Consultar Transactions, Spans, Logs y custom events
- Dashboards y charts (widgets, variables, versiones)
- Alert policies y condiciones NRQL (y alertas mult-condición)

---

# Módulo 8 — Proyecto final: instrumentar el servidor WebSocket + API

- Arquitectura de la app: WebSocket + Minimal API + cola de mensajes
- Configurar agente + distributed tracing + custom events/attributes/métricas
- Logs en contexto, dashboard y alerta
- Verificación end-to-end en la UI de New Relic (trace, errores, logs, métricas)

---

# Buenas prácticas

Explicar siempre:

- cuándo utilizar cada solución (agente APM vs OTel, custom event vs métrica vs span)
- ventajas
- inconvenientes
- alternativas
- coste (consumo de datos y unidades, sampling)
- mantenibilidad

---

# Comparaciones

Cuando aparezca una característica nueva, compararla con:

- Cómo se hacía antes (los "logs a pelo")
- Diferencias con otras plataformas (Datadog, Grafana Stack, self-hosted Prometheus)
- Alternativas dentro del ecosistema New Relic (APM agent vs OTel vs telemetry SDK)

---

# Diagramas

Cuando sea útil, utilizar diagramas ASCII para explicar flujos de trazas, propagación de context, pipelines de datos (agente → collector → NRDB) y arquitecturas.

---

# Estilo

El estilo debe ser:

- Muy didáctico.
- Muy detallado.
- Fácil de leer.
- Profesional.
- Sin asumir conocimientos previos de New Relic ni de observabilidad.
- Explicar siempre el "por qué".
- Utilizar analogías cuando ayuden a comprender.

---

# Documentación actualizada

Antes de afirmar datos de configuración, versiones de paquetes, nombres de variables de entorno, endpoints OTLP o sintaxis NRQL, consultar la documentación oficial de New Relic y las herramientas implicadas (Context7 / docs.newrelic.com). Preferir la versión estable más reciente.

---

# Formato de entrega

El curso se entrega como **web estática** (HTML + CSS + JavaScript vanilla, sin Markdown y sin paso de build). La carpeta `web/` contiene todo el contenido navegable: cada lección es un archivo HTML completo y el chrome (cabecera, sidebar, progreso, tema claro/oscuro) lo inyecta `web/assets/js/site.js` a partir del mapa del curso definido en `web/assets/js/course-data.js` (única fuente de verdad para navegación y enlaces).

Los proyectos de fin de módulo en `proyectos/` son aplicaciones .NET 8 reales y ejecutables con `dotnet`.

Estructura real del repositorio:

```text
Aprende NewRelic/
│
├── AGENTS.md
├── package.json               ← script de servidor estático (node)
├── web/                       ← ★ EL CURSO (web estática, sin build)
│   ├── index.html             ← portada + índice completo
│   ├── assets/css/            ← site.css (tema claro/oscuro)
│   ├── assets/js/             ← course-data.js, highlight.js, site.js
│   ├── modulos/
│   │   ├── 00-preparacion/
│   │   ├── 01-agente-apm/
│   │   ├── 02-transacciones-metricas-errores/
│   │   ├── 03-distributed-tracing/
│   │   ├── 04-websocket/
│   │   ├── 05-logs-en-contexto/
│   │   ├── 06-opentelemetry/
│   │   ├── 07-nrql-dashboards-alertas/
│   │   └── 08-proyecto-final/
│   └── recursos/              ← cheatsheet (NRQL), glosario, FAQ
│
├── scripts/serve-web.mjs      ← servidor estático
└── proyectos/
    └── newrelic-ws-demo/      ← .NET 8: servidor WebSocket + API instrumentable
        ├── src/NewRelicWsDemo/
        └── tests/NewRelicWsDemo.Tests/
```

# Convenciones del curso

- Idioma: contenido, comentarios y UI en español.
- Identificadores de código en inglés (estándar profesional).
- La web del curso usa tema oscuro por defecto con toggle a claro.
- Acento de marca: teal de New Relic (`#00ceb9`).
- Cada lección sigue SIEMPRE las 10 secciones definidas en "Forma de enseñar".

---

# Reglas importantes

1. Nunca des por sentado un concepto.
2. Explica siempre el "por qué".
3. Todo el código debe ejecutarse (verifica builds con `dotnet build`).
4. Usa únicamente `dotnet` para el proyecto .NET y `node` para el servidor estático.
5. Sigue las versiones actuales y oficiales de New Relic (.NET agent, OTel, NRQL).
6. No omitas pasos de configuración (licencia, API keys, regiones).
7. Mantén una progresión lógica entre las lecciones.
8. Cada módulo que lo requiera debe terminar con código aplicable al proyecto real.
9. Si un concepto depende de otro, enlaza a la lección correspondiente.
10. Prioriza la comprensión profunda frente a la velocidad.