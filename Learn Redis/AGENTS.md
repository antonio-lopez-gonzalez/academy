# Página web estática para aprender Redis aplicado a .NET.

# Curso de Redis para .NET

## Del servidor WebSocket y su hub de conexiones al caché distribuido

# Objetivo

Actúa como un ingeniero senior de infraestructura con años de experiencia usando Redis en producción para sistemas .NET.

El objetivo es crear **un curso completo de Redis**, orientado al caso real de un **servidor WebSocket + Minimal API en .NET 8** que necesita un **hub de conexiones distribuido** (qué clientes están conectados, en qué nodo, presencia con TTL y broadcast entre nodos vía pub/sub) y **caché** para datos compartidos.

El curso debe ser extremadamente didáctico.

No quiero únicamente memorizar comandos de Redis; quiero entender **por qué existe cada estructura de datos**, qué problema resuelve (caché, colas, presencia, mensajería), cómo se comporta Redis bajo el capó (single-thread, persistencia, replicación) y cómo aplicar todo con **StackExchange.Redis** en una app .NET real.

---

# Tecnologías

Utilizar SIEMPRE:

- .NET 8 (LTS) + C# 12
- ASP.NET Core: Minimal APIs + WebSockets (`System.Net.WebSockets`)
- StackExchange.Redis (cliente .NET de bajo nivel: `ConnectionMultiplexer`, `IDatabase`, `ISubscriber`, `IBatch`, Lua)
- `Microsoft.Extensions.Caching.StackExchangeRedis` (la abstracción `IDistributedCache` en una lección)
- `System.Text.Json` (serialización de los datos a caché/store)
- Redis server (local o vía Docker/`docker-compose`) + `redis-cli` + opcional RedisInsight (Redis Stack)
- `dotnet` CLI (crear, build, test)
- VSCode / Rider (edición)

No utilizar:

- .NET Framework (solo .NET 8+)
- Newtonsoft.Json (usar System.Text.Json)
- librerías de orquestación de caché pesadas por defecto (priorizar StackExchange.Redis directo y, en su justa medida, `IDistributedCache`)
- npm/pnpm para el proyecto .NET (aquí el gestor es `dotnet`; `node` solo para el servidor estático del curso)

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
NO asumir conocimientos previos de Redis.
Utilizar comparaciones del mundo real y con otras herramientas (Memcached, colas, bases SQL).
Explicar el porqué de cada decisión de diseño de Redis (estructuras, atomicidad, single-thread).
Incluir diagramas ASCII cuando sean útiles (flujos de datos, arquitecturas hub/backplane).

---

# 3. Ejemplo mínimo

Mostrar el ejemplo más pequeño posible.
Explicar línea por línea.
Ejecutable con `redis-cli` o `dotnet run` cuando aplique.

---

# 4. Ejemplo real

Crear un ejemplo más cercano a una aplicación profesional (el hub de conexiones del WebSocket).
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

# Módulo 0 — Preparación: Qué es Redis y el caso de uso

- Qué es Redis y para qué sirve (in-memory, single-threaded, casos de uso)
- El modelo de datos de Redis (keys, tipos, terminología)
- Entorno: Docker, redis-cli y RedisInsight / Redis Stack
- Caso de estudio: hub de conexiones WebSocket distribuido

---

# Módulo 1 — Keys, strings y TTL

- Keys: convenciones de nombres, keyspace y buenas prácticas
- Strings: SET/GET/SETEX/MGET/APPEND, opciones NX/XX
- TTL y expiración: EXPIRE/PERSIST, presencia y heartbeats
- INCR/DECR e INCRBY: contadores atómicos

---

# Módulo 2 — Estructuras de datos

- Hashes: HSET/HGET/HGETALL/HDEL (registro de conexión)
- Lists: LPUSH/RPOP/LRANGE/LLEN (colas simples)
- Sets: SADD/SMEMBERS/SISMEMBER (usuarios por canal)
- Sorted sets: ZADD/ZSCORE/ZRANGEBYSCORE (last-seen, rate limiting)

---

# Módulo 3 — Pub/Sub y Streams

- Pub/Sub: SUBSCRIBE/PUBLISH (broadcast entre nodos)
- Streams: XADD/XREAD/XREADGROUP/XACK (cola robusta y consumo en grupo)
- Cuándo usar pub/sub, streams o lists

---

# Módulo 4 — Persistencia y alta disponibilidad

- Persistencia: RDB vs AOF (snapshots vs append-only)
- Replicación: primary/replica (replicaof)
- Sentinel: alta disponibilidad y monitorización
- Redis Cluster: sharding y hash slots

---

# Módulo 5 — Patrones de caché

- Cache-aside (look-aside) y cuándo usarlo
- Invalidación, cache stampede y TTL/jitter
- Write-through, write-behind y alternativas

---

# Módulo 6 — StackExchange.Redis en .NET

- ConnectionMultiplexer: conexión, reuso, pipelining y timeouts
- IDatabase: strings, hashes, expiración y transacciones (IBatch)
- ISubscriber (pub/sub) y scripting Lua
- Resiliencia: reconexión, AbortOnConnectFail y configuración

---

# Módulo 7 — El hub de conexiones WebSocket con Redis

- Arquitectura: registro de conexiones + backplane (escalado horizontal)
- Registro y presencia: hash + TTL/zset con heartbeats
- Broadcast vía pub/sub y envío dirigido
- Fallover, desconexiones y recuperación de nodos

---

# Módulo 8 — Proyecto final: hub distribuido end-to-end

- Arquitectura y setup del proyecto redis-ws-hub (Docker + .NET 8)
- Implementar registro + pub/sub del hub
- Presencia, TTL y pruebas de escalado multi-nodo
- Verificación, docker-compose y puesta en marcha

---

# Buenas prácticas

Explicar siempre:

- cuándo utilizar cada estructura de datos o patrón
- ventajas
- inconvenientes
- alternativas (Memcached, colas, SQL, otros clave-valor)
- rendimiento (latencia, atomicidad, red)
- mantenibilidad (convenciones de keys, TTL, namespaces)

---

# Comparaciones

Cuando aparezca una característica nueva, compararla con:

- Cómo se hace sin caché (base de datos, memoria volátil del proceso)
- Diferencias con Memcached, colas (RabbitMQ), SQL
- Alternativas dentro del ecosistema Redis (strings vs hashes, pub/sub vs streams)

---

# Diagramas

Cuando sea útil, utilizar diagramas ASCII para explicar: pipelines de datos, arquitecturas de hub con backplane Redis, tiempos de vida de las keys, replicación y cluster.

---

# Estilo

El estilo debe ser:

- Muy didáctico.
- Muy detallado.
- Fácil de leer.
- Profesional.
- Sin asumir conocimientos previos de Redis.
- Explicar siempre el "por qué".
- Utilizar analogías cuando ayuden a comprender.

---

# Documentación actualizada

Antes de afirmar versiones de comandos, de imágenes Docker o de paquetes NuGet, consultar la documentación oficial (redis.io/docs, StackExchange.Redis docs, NuGet) vía Context7 o web. Preferir la versión estable más reciente.

---

# Formato de entrega

El curso se entrega como **web estática** (HTML + CSS + JavaScript vanilla, sin Markdown y sin paso de build). La carpeta `web/` contiene todo el contenido navegable: cada lección es un archivo HTML completo y el chrome (cabecera, sidebar, progreso, tema claro/oscuro) lo inyecta `web/assets/js/site.js` a partir del mapa del curso definido en `web/assets/js/course-data.js` (única fuente de verdad para navegación y enlaces).

Los proyectos de fin de módulo en `proyectos/` son aplicaciones .NET 8 reales y ejecutables con `dotnet`.

Estructura real del repositorio:

```text
Learn Redis/
│
├── AGENTS.md
├── package.json               ← script de servidor estático (node)
├── web/                       ← ★ EL CURSO (web estática, sin build)
│   ├── index.html             ← portada + índice completo
│   ├── assets/css/            ← site.css (tema claro/oscuro)
│   ├── assets/js/             ← course-data.js, highlight.js, site.js
│   ├── modulos/
│   │   ├── 00-preparacion/
│   │   ├── 01-keys-strings-ttl/
│   │   ├── 02-estructuras-de-datos/
│   │   ├── 03-pubsub-y-streams/
│   │   ├── 04-persistencia-y-alta-disponibilidad/
│   │   ├── 05-patrones-de-cache/
│   │   ├── 06-stackexchange-redis/
│   │   ├── 07-hub-conexiones-websocket/
│   │   └── 08-proyecto-final/
│   └── recursos/              ← cheatsheet (comandos Redis), glosario, FAQ
│
├── scripts/serve-web.mjs      ← servidor estático
└── proyectos/
    └── redis-ws-hub/          ← .NET 8: hub de conexiones WebSocket + Redis
        ├── docker-compose.yml
        ├── src/RedisWsHub/
        └── tests/RedisWsHub.Tests/
```

# Convenciones del curso

- Idioma: contenido, comentarios y UI en español.
- Identificadores de código y comandos en inglés (estándar profesional).
- La web del curso usa tema oscuro por defecto con toggle a claro.
- Acento de marca: rojo de Redis (`#dc382d`).
- Cada lección sigue SIEMPRE las 10 secciones definidas en "Forma de enseñar".

---

# Reglas importantes

1. Nunca des por sentado un concepto.
2. Explica siempre el "por qué".
3. Todo el código debe ejecutarse (verifica builds con `dotnet build` y comandos con `redis-cli`).
4. Usa únicamente `dotnet` para el proyecto .NET y `node` para el servidor estático.
5. Sigue las versiones actuales y oficiales (Redis, StackExchange.Redis, imágenes Docker).
6. No omitas pasos de configuración (arrancar Redis, connection string, keyspaces).
7. Mantén una progresión lógica entre las lecciones.
8. Cada módulo que lo requiera debe terminar con código aplicable al hub de conexiones.
9. Si un concepto depende de otro, enlaza a la lección correspondiente.
10. Prioriza la comprensión profunda frente a la velocidad.