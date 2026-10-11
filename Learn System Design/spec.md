# Curso Completo de System Design

## Diseña sistemas a escala — de los requisitos a la arquitectura con trade-offs

---

# 1. Objetivo

Actúa como un arquitecto de software senior con años diseñando sistemas distribuidos a escala. El objetivo es crear **un curso completo de System Design**, pensado para un **desarrollador con experiencia** (que ya programa y ha tocado backend, bases de datos y despliegue) y quiere dar el salto a **diseñar sistemas**: de "¿qué debo construir?" a una **arquitectura justificada con trade-offs**.

El curso es didáctico y orientado al **por qué** y al **trade-off**:

- **cómo se piensa** un sistema (requisitos, escala, límites);
- **qué opciones** existen para cada capa (cómputo, datos, caché, mensajería);
- **cómo se justifica** una decisión (capacidad, latencia, consistencia, coste);
- **cómo se comunica** (diagramas, estimaciones, ADRs, revisión).

El hilo conductor es un **cuaderno de arquitectura**: resolver, con método, varios **casos clásicos** (URL shortener, rate limiter, feed, chat) y cerrar con **un diseño propio completo**.

> Curso **conceptual y práctico**: se diseña con papel, números y diagramas; el "proyecto" es un conjunto de **diseños y ADRs** versionados, no código desplegable.

---

# 2. Audiencia y requisitos previos

- **Saber programar** y entender lo básico de backend, HTTP, bases de datos y contenedores
  (idealmente tras `Learn DotNet`/`Learn FastAPI`, `Learn Docker`, `Learn Cloud`).
- No se necesitan herramientas específicas: lápiz, números y criterio.

---

# 3. Hilo conductor: el cuaderno de arquitectura

Se aplican siempre los mismos pasos (el **método**), sobre casos cada vez más grandes:

1. **Requisitos** funcionales y no funcionales; supuestos y no-objetivos.
2. **Estimación de capacidad** (QPS, almacenamiento, ancho de banda, latencia).
3. **Diseño de alto nivel** (componentes y flujos).
4. **Profundización** (datos, caché, particionado, consistencia).
5. **Trade-offs y ADRs** (qué se gana y qué se pierde).
6. **Operación** (SLOs, resiliencia, observabilidad, coste).

Casos: URL shortener · rate limiter · feed de noticias · chat/colaborativo · **proyecto final**.

---

# 4. Tecnologías y conceptos

## Utilizar SIEMPRE

- Diagramas **ASCII** claros y notación consistente (cajas y flechas).
- **Estimaciones numéricas** (back-of-the-envelope) con unidades y supuestos explícitos.
- Conceptos: latencia/throughput, SLI/SLO/SLA, CAP, teoremas de consistencia, idempotencia, CQRS/event sourcing, backpressure, sharding, replicación.
- **ADRs** para cada decisión relevante.

## No utilizar

- "Balas de plata" ni modas: toda decisión se justifica con el caso.
- Complejidad por defecto (microservicios sin motivo, colas donde basta una llamada).
- Números inventados sin supuestos (una estimación sin base no vale).

---

# 5. Forma de enseñar

Cada lección sigue EXACTAMENTE esta estructura.

## 1. Introducción
Qué, por qué existe, qué problema resuelve, cuándo se usa, errores típicos.

## 2. Teoría
Con detalle, sin asumir experiencia en diseño de sistemas. Analogías (una ciudad y su tráfico; un aeropuerto). Diagramas ASCII. Explicar el **por qué** de cada opción.

## 3. Ejemplo mínimo
El caso más pequeño que ilustra el concepto, con números.

## 4. Ejemplo real
Aplicado al **caso clásico** correspondiente o a un sistema profesional; decisiones explicadas.

## 5. Buenas prácticas
Qué hacer, qué evitar, por qué. Tabla cuando aplique (comparación de opciones).

## 6. Errores habituales
Qué se hace mal y por qué, y cómo se corrige.

## 7. Ejercicio guiado
Paso a paso, resuelto, con resultado esperado (estimaciones, diagrama, decisión).

## 8. Reto
Reto adicional **sin resolver** (solo pista).

## 9. Resumen
Bullets + conceptos clave. Tabla "opción → cuándo usarla".

## 10. Preguntas frecuentes
FAQ (3-5 `<details>`).

## Reglas de contenido

- **Define requisitos y supuestos antes** de diseñar.
- **Estima con números** (no "mucho": dí cuánto y por qué).
- **Siempre trade-offs**: ninguna decisión es gratis.
- Piensa en **fallo**: qué pasa si esto cae.

---

# 6. Multiplataforma: Linux / macOS / Windows

Es un curso conceptual: independiente del SO. Shell canónico **bash** si se ilustran comandos.

---

# 7. Índice del curso

## Módulo 0 — Fundamentos
- **01-que-es-system-design.html** — Qué es diseñar sistemas y el método de 6 pasos.
- **02-requisitos-y-no-funcionales.html** — Funcionales vs no funcionales; supuestos y no-objetivos.
- **03-estimacion-de-capacidad.html** — Back-of-the-envelope: QPS, almacenamiento, ancho de banda.
- **04-latencia-disponibilidad-slo.html** — Latencia (percentiles) y disponibilidad (SLI/SLO/SLA, "nueves").

## Módulo 1 — Escalabilidad
- **01-escalado-vertical-horizontal.html** — Vertical vs horizontal; por qué el horizontal gana a escala.
- **02-balanceo-de-carga.html** — Balanceadores, algoritmos, health checks y sesiones.
- **03-stateless-y-sesiones.html** — Servicios sin estado; dónde va la sesión.
- **04-cdn-y-edge.html** — CDN, caché en el borde, anycast y latencia percibida.

## Módulo 2 — Datos
- **01-sql-vs-nosql.html** — Relacional vs no relacional: elige por acceso, no por moda.
- **02-particionado-y-sharding.html** — Particionado, sharding, claves de partición y rebalanceo.
- **03-replicacion-y-consistencia.html** — Réplicas líder/seguidor, lag y lecturas consistentes.
- **04-indices-y-modelado.html** — Índices a escala, desnormalización y modelado por consulta.

## Módulo 3 — Caché y consistencia
- **01-estrategias-de-cache.html** — Cache-aside, read/write-through, write-behind; dónde cachear.
- **02-invalidacion-y-ttl.html** — TTL, invalidación, stampede y cache penetration.
- **03-teorema-cap.html** — CAP y PACELC: qué sacrificas y cuándo (CP vs AP).
- **04-consistencia-eventual.html** — Consistencia fuerte vs eventual y cómo convivir con ella.

## Módulo 4 — Mensajería y asincronía
- **01-colas-vs-streams.html** — Colas (una vez) vs streams (log); cuándo cada una.
- **02-patrones-de-mensajeria.html** — Pub/sub, fan-out, dead-letter, backpressure.
- **03-idempotencia-exactly-once.html** — Entrega at-least-once, idempotencia y "exactly-once" real.
- **04-cqrs-event-sourcing.html** — CQRS y event sourcing: cuándo aportan y su coste.

## Módulo 5 — Resiliencia
- **01-timeouts-retries-backoff.html** — Timeouts, reintentos con backoff y jitter.
- **02-circuit-breaker-bulkhead.html** — Cortacircuitos, bulkhead y aislamiento de fallos.
- **03-degradacion-graceful.html** — Degradación elegante y backpressure.
- **04-observabilidad-y-slos.html** — Observabilidad (métricas/trazas/logs) y SLOs/error budget.

## Módulo 6 — Casos clásicos
- **01-url-shortener.html** — Diseño completo de un acortador (hash, KV, redirección).
- **02-rate-limiter.html** — Diseño de un limitador distribuido (algoritmos, Redis, precisión).
- **03-feed-de-noticias.html** — Feed a escala: fan-out on write vs on read, timeline.
- **04-chat-colaborativo.html** — Mensajería en tiempo real: WebSockets, presencia, orden.

## Módulo 7 — Proyecto final
- **01-metodologia-de-diseno.html** — El método aplicado de principio a fin. *(project)*
- **02-diseno-de-un-sistema.html** — Diseña un sistema completo (a elegir). *(project)*
- **03-trade-offs-y-adr.html** — Documenta decisiones y trade-offs (ADRs). *(project)*
- **04-revision-y-presentacion.html** — Revisión crítica y presentación del diseño. *(project)*

## Recursos
- **cheatsheet.html** — Fórmulas de estimación, tabla "opción → cuándo", recordatorio CAP/PACELC, plantilla de ADR.
- **glosario.html** — Latencia, throughput, QPS, SLO, CAP, PACELC, sharding, idempotencia, CQRS…
- **faq.html** — FAQ general del curso.

**Total: 8 módulos (0-7), 32 lecciones + 3 recursos.**

---

# 8. Estructura de repositorio

```text
Learn System Design/
├── spec.md                      ← ★ ESTE DOCUMENTO (fuente de verdad del curso)
├── AGENTS.md                    ← puntero corto a spec.md + reglas para agentes
├── package.json / pnpm-workspace.yaml
├── .gitignore / .gitattributes
├── web/                         ← ★ EL CURSO (web estática, sin build)
│   ├── index.html
│   ├── assets/{css,js,fonts,favicon.svg}
│   ├── modulos/00-fundamentos/ … 07-proyecto-final/
│   └── recursos/
├── scripts/serve-web.mjs
└── proyectos/case-studies/      ← diseños y ADRs (documentos versionados)
    ├── README.md
    ├── url-shortener/           ← diseño de ejemplo
    └── diseno-final/            ← plantilla de diseño + ADRs
```

---

# 9. Formato de entrega

Web estática (HTML + CSS + JS vanilla, sin Markdown ni build). Chrome por `site.js` desde `course-data.js`. El "proyecto" son **documentos de diseño** (Markdown) + diagramas ASCII, versionados en `proyectos/case-studies/`.

- `SITE_BASE` = `'../../'` en lecciones, `'./'` en portada, `'../'` en `web/recursos/`.
- Claves de localStorage: `sysd-theme`, `sysd-progress`.
- Bloques: `callout`, `challenge`, `faq`, `table`, `ol.steps`, `pre > code.language-*` (`text` para diagramas, `bash`/`json`/`sql`/`yaml` cuando aplique).

---

# 10. Diseño y marca

- Tema oscuro por defecto, toggle a claro.
- Paleta propia: **esmeralda** (`#10b981`) con acento lima (`#84cc16`) sobre fondo profundo.
- Firma de portada: un **diseño que se escribe** solo (requisitos → componentes → trade-off).
- Identificadores en inglés; contenido, comentarios y UI en español.

---

# 11. Convenciones del curso

1. Idioma: contenido, comentarios y UI en español; identificadores/comandos en inglés.
2. Todo diseño parte de **requisitos + supuestos** y termina en **trade-offs**.
3. Proyecto conductor: `proyectos/case-studies/`.
4. Cada lección usa SIEMPRE las 10 secciones.
5. Retos sin resolver en el HTML (solo pista).

---

# 12. Documentación actualizada

Consultar fuentes de referencia (artículos clásicos de System Design, docs de teoremas de consistencia) cuando se afirmen detalles concretos. Priorizar el razonamiento sobre la memorización.

---

# 13. Página principal (`web/index.html`)

Portada con hero, firma (diseño que se escribe), estadísticas e `<div id="module-list">` alimentado por `course-data.js`.

---

# 14. Checklist de implementación

## Fase 0 — Spec y AGENTS ✅
- [x] `spec.md` (este documento).
- [x] `AGENTS.md` (puntero corto). Skills centralizadas en `Academy/.agents/skills/`.

## Fase 1 — Scaffold ✅
- [x] `package.json`, `pnpm-workspace.yaml`, `.gitignore`, `.gitattributes` (LF), `scripts/serve-web.mjs`.
- [x] Assets re-tematizados (esmeralda) y `course-data.js` completo.
- [x] Portada `web/index.html` + firma.

## Fase 2 — Lecciones (web/modulos/) ✅
- [x] 00-fundamentos … 07-proyecto-final (32 lecciones).

## Fase 3 — Recursos ✅
- [x] `web/recursos/{cheatsheet,glosario,faq}.html`.

## Fase 4 — Proyecto case-studies (diseños + ADRs) ✅
- [x] `proyectos/case-studies/` con diseño de ejemplo y plantilla.

## Fase 5 — Academy ✅
- [x] Tarjeta en `Academy/index.html` + contador + fila en `specs.md` §3 y `README.md`.

---

# 15. Reglas importantes

1. Nunca des por sentado un concepto: latencia, CAP e idempotencia se explican desde cero.
2. Explica siempre el "por qué" y el **trade-off**.
3. Toda estimación lleva **supuestos** y unidades.
4. Piensa en **fallos**: qué se rompe y cómo se degrada.
5. No compliques por defecto: la solución más simple que cumple los requisitos.
6. Sigue las buenas prácticas de arquitectura actuales.
7. No omitas el paso de requisitos.
8. Si un concepto depende de otro, enlaza la lección correspondiente.
9. Prioriza la comprensión profunda frente a la velocidad.
10. No conviertas el curso en un catálogo de productos comerciales.
