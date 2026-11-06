# Curso Completo de RabbitMQ

## Mensajería fiable: de AMQP a colas que aguantan producción

---

# 1. Objetivo

Actúa como un ingeniero de plataforma que lleva años operando RabbitMQ en producción. El objetivo es crear **un curso completo de RabbitMQ**, pensado para quien **ya sabe programar** y quiere entender la mensajería asíncrona de verdad: no solo publicar y consumir, sino **diseñar rutas**, **garantizar entregas**, **sobrevivir a fallos** y **operar el broker** sin sorpresas.

El curso enseña el **por qué**:

- Una cola no es una base de datos: es un **contrato de entrega** con garantías que hay que elegir.
- Un exchange no «envía a una cola»: **enruta** según bindings y routing keys; el desacoplamiento es el objetivo.
- `ack`, `nack`, `prefetch`, confirms y DLQ existen porque **el fallo es normal**, no la excepción.
- Un mensaje puede entregarse **más de una vez**; la idempotencia es responsabilidad del consumidor.
- Un clúster no arregla un diseño mal pensado; las **quorum queues** y las políticas sí cambian las garantías.

El hilo conductor es **un taller de pedidos** que empieza publicando un evento y termina con una **cola de trabajos** real: API, workers, reintentos, DLQ y pruebas.

> Stack fijado: **RabbitMQ 4.4.x** (imagen `rabbitmq:4-management`), **AMQP 0-9-1** y **Node.js 22 + amqplib**. Se menciona AMQP 1.0 solo como alternativa, no como base del curso.

---

# 2. Audiencia y requisitos previos

- **Saber programar** y usar una terminal.
- Node.js 22+ y Docker (Docker Desktop o Docker Engine + Compose).
- No se asume experiencia previa con mensajería. Sí se agradece haber visto contenedores (`Aprende Docker`).
- Ganas de entender garantías y fallos: el curso no es un recetario de `send`/`receive`.

---

# 3. Hilo conductor: el taller de pedidos

Un sistema que crece con el curso:

1. **Un mensaje**: el taller publica `pedido.creado` y un consumidor lo imprime.
2. **Varios consumidores**: facturación e inventario reciben el mismo evento con fanout.
3. **Rutas**: topic exchange con `pedido.*`, `pedido.creado` y `pedido.cancelado`.
4. **Fiabilidad**: confirms de publicación, `ack` manual, reintentos y DLQ.
5. **Patrones**: work queue para generar facturas, RPC para consultar stock.
6. **Operación**: quorum queues, alarmas, métricas y política de reintentos.
7. **Proyecto final**: la **job queue** del taller, con API HTTP, workers y observabilidad.

El proyecto ejecutable vive en `proyectos/job-queue/` y se levanta con Docker Compose + Node.js.

---

# 4. Tecnologías

## Utilizar SIEMPRE

- **RabbitMQ 4.4.x** con la imagen oficial `rabbitmq:4-management` (management UI en `15672`).
- **AMQP 0-9-1** como protocolo del curso; `amqplib` como cliente de Node.js.
- **Node.js 22 LTS**, módulos ESM, `node --test` para pruebas.
- **Quorum queues** para colas durables de trabajo; `classic` solo para casos justificados.
- **Publisher confirms** y **consumer acknowledgements** en todo ejemplo de producción.
- **Docker Compose** para levantar el broker local.
- **Idempotencia** explícita en los consumidores (clave de negocio, no `messageId` a ciegas).

## No utilizar

- Colas clásicas **mirrored** como solución de alta disponibilidad (fueron retiradas; se explica por qué).
- `autoAck: true` en ejemplos de producción.
- `basic.qos` global como sustituto de un diseño de prefetch por consumidor.
- Reintentos infinitos sin DLQ ni límite.
- La management UI como única forma de declarar infraestructura: todo debe poder reproducirse con código.
- Comparaciones superficiales con Kafka que confundan modelos (se menciona solo para contextualizar).

---

# 5. Forma de enseñar

Cada lección sigue EXACTAMENTE las 10 secciones del contrato Academy:

1. **Introducción** — qué, por qué existe, qué problema resuelve, cuándo se usa, errores típicos.
2. **Teoría** — AMQP, exchanges, colas, garantías; diagramas ASCII del flujo.
3. **Ejemplo mínimo** — el código más pequeño posible, explicado línea a línea.
4. **Ejemplo real** — el taller de pedidos o un escenario profesional.
5. **Buenas prácticas** — qué hacer / qué evitar / por qué (tabla cuando aplique).
6. **Errores habituales** — incorrecto, por qué está mal y versión correcta.
7. **Ejercicio guiado** — paso a paso, resuelto, con resultado esperado.
8. **Reto** — reto adicional **sin resolver** (solo pista).
9. **Resumen** — bullets + conceptos clave.
10. **Preguntas frecuentes** — FAQ (3–5 `<details>`).

## Reglas de contenido

- Antes de un ejemplo, di **qué garantía** ofrece (at-most-once, at-least-once) y **quién** la sostiene.
- Distingue siempre **publicar** (exchange) de **consumir** (cola).
- Todo ejemplo de producción declara colas durables, mensajes persistentes y `ack` manual.
- Los diagramas de routing se dibujan en ASCII (`exchange → binding → queue`).
- El reto nunca se resuelve en el HTML.

---

# 6. Multiplataforma

- **Windows:** Docker Desktop + PowerShell. Shell canónico de los ejemplos: **bash**, con callout cuando PowerShell cambie variables o redirecciones.
- **macOS / Linux:** Docker Engine + Compose.
- Todos los comandos de broker se ejecutan dentro del contenedor (`docker exec`) o con `rabbitmqadmin`; no se asume instalación nativa.
- Puertos por defecto: `5672` (AMQP) y `15672` (management). Si están ocupados, se documenta cómo cambiarlos.

---

# 7. Índice del curso

## Módulo 0 — Fundamentos
- **01-que-es-una-cola.html** — Qué resuelve la mensajería, cuándo usarla y cuándo no.
- **02-modelo-amqp.html** — Conexión, canal, exchange, cola, binding y routing key.
- **03-instalacion-y-management.html** — Docker, management UI, `rabbitmqctl` y `rabbitmqadmin`.
- **04-primer-mensaje.html** — Productor y consumidor mínimos con `amqplib`.

## Módulo 1 — Productores y consumidores
- **01-publicar-y-consumir.html** — `publish`, `consume`, `deliveryTag` y ciclo de vida.
- **02-acks-y-prefetch.html** — `ack`, `nack`, `reject`, `basic.qos` y consumidores lentos.
- **03-durabilidad-y-persistencia.html** — Colas durables, mensajes persistentes y qué sobrevive a un reinicio.
- **04-competing-consumers.html** — Varios workers, reparto justo y orden por cola.

## Módulo 2 — Exchanges y routing
- **01-direct-y-default.html** — Routing key exacta y default exchange.
- **02-fanout.html** — Publicar a todos los suscriptores; el patrón pub/sub.
- **03-topic.html** — Comodines `*` y `#`, jerarquía de routing keys.
- **04-headers-y-alternate.html** — Matching por cabeceras y alternate exchange.

## Módulo 3 — Fiabilidad
- **01-publisher-confirms.html** — Confirmas publicaciones; `mandatory` y `basic.return`.
- **02-dead-letter-exchange.html** — DLX, motivos y cola de mensajes fallidos.
- **03-ttl-y-limites.html** — TTL por mensaje/cola, `max-length` y overflow.
- **04-idempotencia-y-reintentos.html** — Reintentos con backoff, deduplicación y límites.

## Módulo 4 — Patrones
- **01-work-queues.html** — Cola de trabajo con reparto y `prefetch`.
- **02-pub-sub.html** — Eventos de dominio y múltiples consumidores independientes.
- **03-rpc.html** — `reply_to`, `correlation_id` y timeouts.
- **04-prioridad-y-retraso.html** — Prioridades, TTL + DLX y delayed message plugin.

## Módulo 5 — Operación
- **01-quorum-queues.html** — Raft, replicación y límites frente a classic queues.
- **02-clustering-y-alta-disponibilidad.html** — Clúster, nodos, particiones y recuperación.
- **03-memoria-disco-y-flow-control.html** — Alarmas, paging, credit flow y bloqueo de publicadores.
- **04-observabilidad.html** — Management API, Prometheus, logs y alertas clave.

## Módulo 6 — Seguridad
- **01-usuarios-vhosts-permisos.html** — Usuarios, vhosts, roles y mínimo privilegio.
- **02-tls.html** — Certificados, `amqps`, verificación y rotación.
- **03-politicas-y-cuotas.html** — Policies, operator policies y límites por vhost.
- **04-endurecimiento-y-auditoria.html** — Superficie de ataque, credenciales y auditoría.

## Módulo 7 — Proyecto final
- **01-arquitectura-job-queue.html** — Diseño de la cola de trabajos y sus garantías. *(project)*
- **02-api-y-worker.html** — API HTTP, publicación fiable y worker con `ack`. *(project)*
- **03-reintentos-y-dlq.html** — Backoff, límite de intentos y DLQ operativa. *(project)*
- **04-operacion-y-pruebas.html** — Métricas, graceful shutdown y pruebas con `node --test`. *(project)*

## Recursos
- **cheatsheet.html** — Comandos, APIs y patrones por intención.
- **glosario.html** — Exchange, binding, quorum, prefetch, DLX, confirm…
- **faq.html** — FAQ general del curso.

**Total: 8 módulos (0-7), 32 lecciones + 3 recursos.**

---

# 8. Estructura

```text
Aprende RabbitMQ/
├── spec.md
├── AGENTS.md
├── package.json / pnpm-workspace.yaml
├── .gitattributes
├── web/
│   ├── index.html
│   ├── assets/{css,js,fonts,favicon.svg}
│   ├── modulos/00-fundamentos/ … 07-proyecto-final/
│   ├── recursos/{cheatsheet,glosario,faq}.html
│   └── descargas/Aprende-RabbitMQ.zip
├── scripts/serve-web.mjs
└── proyectos/job-queue/       ← API + worker + Docker Compose
```

---

# 9. Formato de entrega

Web estática sin build. Claves: `rmq-theme`, `rmq-progress`. `SITE_BASE` según el contrato (`./`, `../`, `../../`).

- Bloques de código: `language-javascript` (Node/amqplib), `language-bash` (Docker/CLI), `language-json`, `language-yaml`.
- Salidas de broker y diagramas de routing: `language-text`.
- El proyecto usa Node.js ESM y `node --test`; no requiere build.

---

# 10. Diseño y marca

- Paleta propia: **naranja RabbitMQ** (`#FF6600`) con acento **turquesa de mensaje** (`#2DD4BF`) sobre fondo oscuro.
- Firma de portada: un mensaje que recorre **exchange → dos colas → dos `ack`**, escrito en una consola.
- Logo del topbar y favicon: **icono oficial de RabbitMQ** (Simple Icons).
- Tema oscuro por defecto, toggle a claro, `prefers-reduced-motion` respetado.

---

# 11. Convenciones

1. Contenido, comentarios y UI en español; identificadores y comandos en inglés.
2. Toda afirmación sobre garantías se acompaña de **quién la sostiene** (productor, broker o consumidor).
3. Los ejemplos de producción usan confirms, `ack` manual, durabilidad e idempotencia.
4. Proyecto conductor: `proyectos/job-queue/`.
5. Cada lección usa SIEMPRE las 10 secciones; el reto no se resuelve.
6. Los enlaces entre lecciones usan rutas relativas del curso.

---

# 12. Documentación actualizada

Consultar la documentación oficial de RabbitMQ (versión 4.4) y de `amqplib` vía Context7 antes de afirmar APIs, flags o comportamientos. Preferir la versión estable más reciente. No inventar métodos: si `amqplib` no expone algo, se dice.

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
- [x] Assets re-tematizados, `course-data.js` completo, favicon oficial.
- [x] Portada `web/index.html` + firma.

## Fase 2 — Lecciones ✅
- [x] 00-fundamentos … 07-proyecto-final (32 lecciones).

## Fase 3 — Recursos ✅
- [x] `web/recursos/{cheatsheet,glosario,faq}.html`.

## Fase 4 — Proyecto job-queue ✅
- [x] `proyectos/job-queue/` con Compose, API, worker, reintentos, DLQ y tests.

## Fase 5 — Academy ✅
- [x] Tarjeta, contadores, filas en `specs.md` y `README.md`, ZIP.

---

# 15. Reglas importantes

1. No des por sabido AMQP: explica exchange, cola, binding y canal desde cero.
2. Toda garantía se elige y se justifica; no existe «entrega exactamente una vez» gratis.
3. El fallo es el caso normal: diseña para reintentar e idempotizar.
4. Nunca `autoAck` en producción; nunca reintentos infinitos.
5. Las quorum queues son el valor por defecto para trabajo durable; justifica cualquier excepción.
6. Sin secretos en el repositorio; credenciales de desarrollo solo en `.env.example`.
7. Todo comando debe poder ejecutarse en Windows, macOS y Linux con Docker.
8. Si un concepto depende de otro, enlaza la lección correspondiente.
9. Prioriza comprensión y operación frente a memorizar la management UI.
10. El proyecto debe arrancar con un solo `docker compose up` y pasar sus pruebas.
