/**
 * course-data.js — MAPA COMPLETO DEL CURSO MINIO
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
  title: "Aprende MinIO",
  subtitle: "Almacenamiento de objetos S3: de un bucket a producción",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de MinIO" },
    { file: "recursos/glosario.html", title: "Glosario de términos" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-fundamentos",
      title: "Fundamentos",
      description: "Object storage, la API S3, MinIO en Docker, la consola y el cliente mc.",
      lessons: [
        { file: "01-que-es-object-storage.html", title: "Qué es el almacenamiento de objetos", minutes: 30 },
        { file: "02-s3-api-y-minio.html", title: "La API S3 y MinIO", minutes: 35 },
        { file: "03-instalacion-y-console.html", title: "Instalación y consola web", minutes: 30 },
        { file: "04-mc-y-primer-bucket.html", title: "mc y el primer bucket", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-objetos",
      title: "Objetos y buckets",
      description: "Buckets, prefijos, metadatos, multipart, versionado y object locking.",
      lessons: [
        { file: "01-buckets-y-prefijos.html", title: "Buckets y prefijos", minutes: 30 },
        { file: "02-put-get-y-metadatos.html", title: "PUT, GET y metadatos", minutes: 35 },
        { file: "03-multipart-y-streaming.html", title: "Multipart y streaming", minutes: 35 },
        { file: "04-versionado-y-object-locking.html", title: "Versionado y object locking", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-sdk",
      title: "SDK y aplicación",
      description: "El SDK de Node.js: cliente, subidas directas, errores, checksums y descargas.",
      lessons: [
        { file: "01-cliente-node.html", title: "El cliente de Node.js", minutes: 35 },
        { file: "02-subidas-directas.html", title: "Subidas directas con presigned URLs", minutes: 40 },
        { file: "03-errores-reintentos-y-checksums.html", title: "Errores, reintentos y checksums", minutes: 35 },
        { file: "04-descargas-y-transformaciones.html", title: "Descargas y transformaciones", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-seguridad",
      title: "Seguridad",
      description: "Credenciales root, usuarios y políticas, presigned URLs, TLS y cifrado.",
      lessons: [
        { file: "01-credenciales-root.html", title: "Credenciales root", minutes: 30 },
        { file: "02-usuarios-y-politicas.html", title: "Usuarios y políticas", minutes: 40 },
        { file: "03-presigned-urls.html", title: "Presigned URLs", minutes: 35 },
        { file: "04-tls-y-cifrado.html", title: "TLS y cifrado", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-ciclo-de-vida",
      title: "Ciclo de vida",
      description: "Lifecycle e ILM, réplica entre sitios, notificaciones de eventos y backups.",
      lessons: [
        { file: "01-lifecycle-e-ilm.html", title: "Lifecycle e ILM", minutes: 35 },
        { file: "02-replicacion.html", title: "Réplica entre sitios", minutes: 35 },
        { file: "03-notificaciones-eventos.html", title: "Notificaciones de eventos", minutes: 40 },
        { file: "04-backups-y-retencion.html", title: "Backups y retención", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-operacion",
      title: "Operación",
      description: "Erasure coding, modo distribuido, monitorización, capacidad y actualizaciones.",
      lessons: [
        { file: "01-erasure-coding.html", title: "Erasure coding", minutes: 35 },
        { file: "02-modo-distribuido.html", title: "Modo distribuido", minutes: 40 },
        { file: "03-monitorizacion-y-alertas.html", title: "Monitorización y alertas", minutes: 35 },
        { file: "04-capacidad-y-actualizaciones.html", title: "Capacidad y actualizaciones", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-integracion",
      title: "Integración",
      description: "Compatibilidad con AWS SDK, Kubernetes, procesado de objetos y CDN.",
      lessons: [
        { file: "01-s3-compatible-aws-sdk.html", title: "Compatibilidad con AWS SDK", minutes: 35 },
        { file: "02-kubernetes-operator.html", title: "Kubernetes Operator", minutes: 35 },
        { file: "03-procesado-y-antivirus.html", title: "Procesado y antivirus", minutes: 35 },
        { file: "04-cdn-y-cache.html", title: "CDN y caché", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-proyecto-final",
      title: "Proyecto final",
      description: "Construir un almacén de documentos: API, presigned uploads, metadatos y eventos.",
      lessons: [
        { file: "01-arquitectura-document-store.html", title: "Arquitectura del document store", minutes: 40, project: true },
        { file: "02-api-y-presigned-uploads.html", title: "API y presigned uploads", minutes: 45, project: true },
        { file: "03-metadatos-versionado-y-lifecycle.html", title: "Metadatos, versionado y lifecycle", minutes: 40, project: true },
        { file: "04-eventos-y-pruebas.html", title: "Eventos y pruebas", minutes: 40, project: true },
      ],
    },
  ],
};
