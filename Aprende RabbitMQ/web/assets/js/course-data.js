/**
 * course-data.js — MAPA COMPLETO DEL CURSO RABBITMQ
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
  title: "Aprende RabbitMQ",
  subtitle: "Mensajería fiable: de AMQP a colas que aguantan producción",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de RabbitMQ" },
    { file: "recursos/glosario.html", title: "Glosario de términos" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-fundamentos",
      title: "Fundamentos",
      description: "Qué resuelve una cola de mensajes, el modelo AMQP, RabbitMQ en Docker y el primer mensaje.",
      lessons: [
        { file: "01-que-es-una-cola.html", title: "Qué es una cola de mensajes", minutes: 30 },
        { file: "02-modelo-amqp.html", title: "El modelo AMQP 0-9-1", minutes: 35 },
        { file: "03-instalacion-y-management.html", title: "Instalación y management UI", minutes: 30 },
        { file: "04-primer-mensaje.html", title: "El primer mensaje", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-productores-consumidores",
      title: "Productores y consumidores",
      description: "Publicar, consumir, acknowledgements, prefetch, durabilidad y consumidores en competencia.",
      lessons: [
        { file: "01-publicar-y-consumir.html", title: "Publicar y consumir", minutes: 35 },
        { file: "02-acks-y-prefetch.html", title: "acks y prefetch", minutes: 35 },
        { file: "03-durabilidad-y-persistencia.html", title: "Durabilidad y persistencia", minutes: 30 },
        { file: "04-competing-consumers.html", title: "Consumidores en competencia", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-exchanges-routing",
      title: "Exchanges y routing",
      description: "Direct, fanout, topic y headers: enrutar mensajes sin acoplar productores y consumidores.",
      lessons: [
        { file: "01-direct-y-default.html", title: "Direct y default exchange", minutes: 30 },
        { file: "02-fanout.html", title: "Fanout: publicar a todos", minutes: 30 },
        { file: "03-topic.html", title: "Topic: rutas con comodines", minutes: 35 },
        { file: "04-headers-y-alternate.html", title: "Headers y alternate exchange", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-fiabilidad",
      title: "Fiabilidad",
      description: "Confirms, dead-letter exchanges, TTL, límites de cola, idempotencia y reintentos.",
      lessons: [
        { file: "01-publisher-confirms.html", title: "Publisher confirms", minutes: 35 },
        { file: "02-dead-letter-exchange.html", title: "Dead-letter exchange", minutes: 40 },
        { file: "03-ttl-y-limites.html", title: "TTL y límites de cola", minutes: 35 },
        { file: "04-idempotencia-y-reintentos.html", title: "Idempotencia y reintentos", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-patrones",
      title: "Patrones",
      description: "Work queues, pub/sub, RPC y mensajes con prioridad o entrega retardada.",
      lessons: [
        { file: "01-work-queues.html", title: "Work queues", minutes: 35 },
        { file: "02-pub-sub.html", title: "Pub/sub", minutes: 30 },
        { file: "03-rpc.html", title: "RPC sobre mensajería", minutes: 40 },
        { file: "04-prioridad-y-retraso.html", title: "Prioridad y entrega retardada", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-operacion",
      title: "Operación",
      description: "Quorum queues, clústeres, alarms de memoria y disco, flow control y observabilidad.",
      lessons: [
        { file: "01-quorum-queues.html", title: "Quorum queues", minutes: 40 },
        { file: "02-clustering-y-alta-disponibilidad.html", title: "Clústeres y alta disponibilidad", minutes: 40 },
        { file: "03-memoria-disco-y-flow-control.html", title: "Memoria, disco y flow control", minutes: 35 },
        { file: "04-observabilidad.html", title: "Observabilidad y alertas", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-seguridad",
      title: "Seguridad",
      description: "Usuarios, vhosts, permisos, TLS, cuotas y endurecimiento del broker.",
      lessons: [
        { file: "01-usuarios-vhosts-permisos.html", title: "Usuarios, vhosts y permisos", minutes: 35 },
        { file: "02-tls.html", title: "TLS en el broker", minutes: 35 },
        { file: "03-politicas-y-cuotas.html", title: "Políticas y cuotas", minutes: 30 },
        { file: "04-endurecimiento-y-auditoria.html", title: "Endurecimiento y auditoría", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-proyecto-final",
      title: "Proyecto final",
      description: "Construir una cola de trabajos real: API, workers, reintentos, DLQ y pruebas.",
      lessons: [
        { file: "01-arquitectura-job-queue.html", title: "Arquitectura de la job queue", minutes: 40, project: true },
        { file: "02-api-y-worker.html", title: "API y worker", minutes: 45, project: true },
        { file: "03-reintentos-y-dlq.html", title: "Reintentos y DLQ", minutes: 40, project: true },
        { file: "04-operacion-y-pruebas.html", title: "Operación y pruebas", minutes: 40, project: true },
      ],
    },
  ],
};
