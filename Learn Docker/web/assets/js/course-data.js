/**
 * course-data.js — MAPA COMPLETO DEL CURSO DOCKER Y KUBERNETES
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
  title: "Docker y Kubernetes",
  subtitle: "Del contenedor local al clúster · HarborTasks + kind + Helm",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet Docker / kubectl / Helm" },
    { file: "recursos/glosario.html", title: "Glosario de Docker y Kubernetes" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Preparación: contenedores y WSL2",
      description: "Qué es un contenedor, la arquitectura de Docker, cómo montar el entorno en Linux, macOS y Windows (WSL2), y el caso de estudio HarborTasks.",
      lessons: [
        { file: "01-que-es-un-contenedor.html", title: "¿Qué es un contenedor?", minutes: 30 },
        { file: "02-arquitectura-docker.html", title: "Arquitectura Docker: daemon, imagen y registry", minutes: 35 },
        { file: "03-entorno-linux-macos.html", title: "Entorno en Linux y macOS", minutes: 30 },
        { file: "04-entorno-windows-wsl2.html", title: "Windows, WSL2 y Docker Desktop", minutes: 40 },
        { file: "05-caso-de-estudio-harbortasks.html", title: "Caso de estudio: HarborTasks", minutes: 25 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-imagenes-y-dockerfile",
      title: "Imágenes y Dockerfile",
      description: "Imágenes, capas y copy-on-write; el Dockerfile paso a paso, CMD vs ENTRYPOINT, multi-stage builds, Buildx y publicación de imágenes.",
      lessons: [
        { file: "01-imagenes-y-capas.html", title: "Imágenes, capas y Union FS", minutes: 30 },
        { file: "02-dockerfile-basico.html", title: "Dockerfile básico: FROM, RUN, COPY, CMD", minutes: 35 },
        { file: "03-cmd-vs-entrypoint.html", title: "CMD vs ENTRYPOINT", minutes: 30 },
        { file: "04-multi-stage-buildx.html", title: "Multi-stage, .dockerignore y Buildx", minutes: 35 },
        { file: "05-publicar-imagenes.html", title: "Etiquetar y publicar imágenes", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-contenedores",
      title: "Contenedores en ejecución",
      description: "docker run, ps, logs y exec; puertos y variables de entorno; ciclo de vida, señales y restart policies; límites de recursos y healthchecks.",
      lessons: [
        { file: "01-docker-run.html", title: "docker run, ps, logs y exec", minutes: 30 },
        { file: "02-puertos-y-env.html", title: "Puertos y variables de entorno", minutes: 30 },
        { file: "03-ciclo-de-vida-restart.html", title: "Ciclo de vida y restart policies", minutes: 30 },
        { file: "04-recursos-healthcheck.html", title: "Límites de recursos y HEALTHCHECK", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-datos-y-redes",
      title: "Datos y redes Docker",
      description: "Volúmenes vs bind mounts vs tmpfs; redes bridge, host y none con DNS interno; el caso real de un Postgres persistente.",
      lessons: [
        { file: "01-volumes-y-bind-mounts.html", title: "Volúmenes, bind mounts y tmpfs", minutes: 30 },
        { file: "02-redes-docker.html", title: "Redes bridge, host y DNS interno", minutes: 30 },
        { file: "03-postgres-persistente.html", title: "Caso: Postgres persistente en Docker", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-docker-compose",
      title: "Docker Compose",
      description: "Qué es Compose y por qué existe; el archivo compose.yaml; depends_on y healthchecks entre servicios; el stack completo de HarborTasks.",
      lessons: [
        { file: "01-introduccion-compose.html", title: "Qué es Compose y por qué existe", minutes: 30 },
        { file: "02-compose-yaml.html", title: "compose.yaml: services, ports, env, volumes", minutes: 35 },
        { file: "03-dependencias-y-healthchecks.html", title: "depends_on y healthchecks entre servicios", minutes: 30 },
        { file: "04-stack-harbortasks.html", title: "Stack Compose completo de HarborTasks", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-de-docker-a-kubernetes",
      title: "De Docker a Kubernetes",
      description: "Qué no resuelve Compose, la arquitectura del clúster, el primer contacto con kind y kubectl, y el modelo declarativo en YAML.",
      lessons: [
        { file: "01-por-que-kubernetes.html", title: "Qué no resuelve Compose y qué aporta Kubernetes", minutes: 30 },
        { file: "02-arquitectura-kubernetes.html", title: "Control plane, nodos y etcd", minutes: 35 },
        { file: "03-kind-y-kubectl.html", title: "kind y kubectl: primer contacto", minutes: 35 },
        { file: "04-yaml-declarativo.html", title: "YAML declarativo y reconciliación", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-workloads",
      title: "Workloads: Pods, Deployments y más",
      description: "Pods, Deployments con rolling updates, Services y DNS, Jobs y CronJobs, y las probes de liveness, readiness y startup.",
      lessons: [
        { file: "01-pods.html", title: "Pods: la unidad mínima", minutes: 30 },
        { file: "02-deployments.html", title: "Deployments y rolling updates", minutes: 35 },
        { file: "03-services-y-dns.html", title: "Services: ClusterIP, NodePort, LoadBalancer", minutes: 35 },
        { file: "04-jobs-y-cronjobs.html", title: "Jobs y CronJobs", minutes: 25 },
        { file: "05-probes.html", title: "Probes: liveness, readiness, startup", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-configuracion-y-almacenamiento",
      title: "Configuración y almacenamiento",
      description: "ConfigMaps, Secrets, PersistentVolumes con PVCs, e Ingress como puerta de entrada al clúster.",
      lessons: [
        { file: "01-configmaps.html", title: "ConfigMaps", minutes: 30 },
        { file: "02-secrets.html", title: "Secrets", minutes: 30 },
        { file: "03-persistent-volumes.html", title: "PersistentVolumes y PVCs", minutes: 35 },
        { file: "04-ingress.html", title: "Ingress y entrada al clúster", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 8 */
    {
      id: "08",
      dir: "modulos/08-helm-seguridad-operacion",
      title: "Helm, seguridad y operación",
      description: "Helm y los charts, SecurityContext y RBAC básico, límites con HPA, y logs con kubectl debug.",
      lessons: [
        { file: "01-helm-basico.html", title: "Helm: charts, values y releases", minutes: 35 },
        { file: "02-helm-harbortasks.html", title: "Chart de HarborTasks", minutes: 35 },
        { file: "03-seguridad-rbac.html", title: "SecurityContext, no-root y RBAC básico", minutes: 30 },
        { file: "04-limites-y-hpa.html", title: "Límites de recursos y HPA", minutes: 30 },
        { file: "05-logs-y-troubleshooting.html", title: "Logs, kubectl debug y troubleshooting", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 9 */
    {
      id: "09",
      dir: "modulos/09-proyecto-final",
      title: "Proyecto final: HarborTasks en Kubernetes",
      description: "Arquitectura del despliegue final, la API y la web en el clúster, Postgres con Ingress, y la verificación end-to-end con checklist de producción.",
      lessons: [
        { file: "01-arquitectura-despliegue.html", title: "Arquitectura del despliegue final", minutes: 30 },
        { file: "02-desplegar-api-y-web.html", title: "Desplegar API y web", minutes: 35 },
        { file: "03-postgres-y-ingress.html", title: "Postgres en clúster + Ingress", minutes: 35 },
        { file: "04-verificacion-produccion.html", title: "Verificación y checklist de producción", minutes: 30 },
      ],
    },
  ],
};