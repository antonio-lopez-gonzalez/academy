/**
 * course-data.js — MAPA COMPLETO DEL CURSO REDIS PARA .NET
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
  title: "Redis para .NET",
  subtitle: "Caché y hub de conexiones WebSocket · StackExchange.Redis",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de comandos Redis" },
    { file: "recursos/glosario.html", title: "Glosario de Redis" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Qué es Redis y el caso de uso",
      description: "Redis desde cero: qué es, su modelo de datos, el entorno con Docker y el caso de estudio del hub de conexiones.",
      lessons: [
        { file: "01-que-es-redis-y-para-que.html", title: "Qué es Redis y para qué sirve", minutes: 30 },
        { file: "02-modelo-de-datos.html", title: "El modelo de datos de Redis", minutes: 35 },
        { file: "03-entorno-docker-redis-cli.html", title: "Entorno: Docker, redis-cli y RedisInsight", minutes: 35 },
        { file: "04-caso-de-estudio-hub-conexiones.html", title: "Caso de estudio: hub de conexiones WebSocket", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-keys-strings-ttl",
      title: "Keys, Strings y TTL",
      description: "Las keys, los strings como caché básica, la expiración (TTL) y los contadores atómicos.",
      lessons: [
        { file: "01-keys-y-namespaces.html", title: "Keys: convenciones de nombres", minutes: 30 },
        { file: "02-strings-set-get.html", title: "Strings: SET/GET/SETEX/MGET y opciones NX/XX", minutes: 35 },
        { file: "03-ttl-y-expiracion.html", title: "TTL y expiración: EXPIRE y presencia", minutes: 35 },
        { file: "04-incrementos-y-contadores.html", title: "INCR/DECR y contadores atómicos", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-estructuras-de-datos",
      title: "Estructuras de Datos",
      description: "Hashes, lists, sets y sorted sets: cuándo usar cada una y sus comandos esenciales.",
      lessons: [
        { file: "01-hashes.html", title: "Hashes: el registro de una conexión", minutes: 35 },
        { file: "02-lists.html", title: "Lists: colas simples", minutes: 30 },
        { file: "03-sets.html", title: "Sets: usuarios por canal", minutes: 30 },
        { file: "04-sorted-sets.html", title: "Sorted sets: last-seen y rate limiting", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-pubsub-y-streams",
      title: "Pub/Sub y Streams",
      description: "Mensajería push entre nodos (pub/sub) y colas robustas con grupos de consumo (streams).",
      lessons: [
        { file: "01-pubsub.html", title: "Pub/Sub: SUBSCRIBE/PUBLISH y el backplane del hub", minutes: 35 },
        { file: "02-streams.html", title: "Streams: XADD/XREAD/XREADGROUP y XACK", minutes: 40 },
        { file: "03-pubsub-vs-streams-vs-lists.html", title: "Cuándo usar pub/sub, streams o lists", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-persistencia-y-alta-disponibilidad",
      title: "Persistencia y Alta Disponibilidad",
      description: "RDB vs AOF, replicación, Sentinel y Redis Cluster para sistemas que no pueden perder datos.",
      lessons: [
        { file: "01-rdb-y-aof.html", title: "Persistencia: RDB vs AOF", minutes: 35 },
        { file: "02-replicacion.html", title: "Replicación: primary/replica", minutes: 35 },
        { file: "03-sentinel.html", title: "Sentinel: alta disponibilidad", minutes: 30 },
        { file: "04-cluster.html", title: "Redis Cluster: sharding y hash slots", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-patrones-de-cache",
      title: "Patrones de Caché",
      description: "Cache-aside, invalidación, cache stampede, TTL/jitter y las estrategias de escritura.",
      lessons: [
        { file: "01-cache-aside.html", title: "Cache-aside (look-aside)", minutes: 35 },
        { file: "02-invalidacion-y-cache-stampede.html", title: "Invalidación, cache stampede y TTL/jitter", minutes: 40 },
        { file: "03-write-through-write-behind.html", title: "Write-through, write-behind y cuándo cachear", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-stackexchange-redis",
      title: "StackExchange.Redis en .NET",
      description: "El cliente .NET de Redis: conexión, operaciones, pub/sub, transacciones, Lua y resiliencia.",
      lessons: [
        { file: "01-connectionmultiplexer.html", title: "ConnectionMultiplexer: conexión y reuso", minutes: 40 },
        { file: "02-idatabase-operaciones.html", title: "IDatabase: strings, hashes y expiración", minutes: 40 },
        { file: "03-pubsub-transacciones-y-lua.html", title: "ISubscriber, transacciones y scripting Lua", minutes: 40 },
        { file: "04-resiliencia-y-monitorizacion.html", title: "Resiliencia, reconexión y configuración", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-hub-conexiones-websocket",
      title: "El Hub de Conexiones WebSocket con Redis",
      description: "Registro distribuido de conexiones, presencia con TTL y broadcast entre nodos vía backplane.",
      lessons: [
        { file: "01-arquitectura-del-hub.html", title: "Arquitectura: registro + backplane", minutes: 40 },
        { file: "02-registro-y-presencia.html", title: "Registro de conexiones y presencia", minutes: 40 },
        { file: "03-broadcast-y-envio-dirigido.html", title: "Broadcast vía pub/sub y envío dirigido", minutes: 40 },
        { file: "04-fallover-y-recuperacion.html", title: "Desconexiones, heartbeats y fallover", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 8 */
    {
      id: "08",
      dir: "modulos/08-proyecto-final",
      title: "Proyecto Final: Hub Distribuido",
      description: "Construir e implementar el hub de conexiones distribuido end-to-end con Redis y verificar su escalado.",
      lessons: [
        { file: "01-arquitectura-del-proyecto.html", title: "Arquitectura y setup del proyecto", minutes: 45, project: true },
        { file: "02-registro-y-pubsub.html", title: "Implementar registro y pub/sub", minutes: 45, project: true },
        { file: "03-presencia-y-escalado.html", title: "Presencia, TTL y escalado multi-nodo", minutes: 45, project: true },
        { file: "04-verificacion-y-produccion.html", title: "Verificación, docker-compose y puesta en marcha", minutes: 45, project: true },
      ],
    },
  ],
};