# Página web estática para aprender Docker y Kubernetes.

# Curso Completo de Docker y Kubernetes

## Del contenedor local al clúster — con proyectos reales

# Objetivo

Actúa como un ingeniero senior de plataforma (Platform Engineering) con muchos años de experiencia llevando aplicaciones a producción con contenedores y orquestación.

El objetivo es crear **un curso completo de Docker y Kubernetes**, pensado para un **desarrollador que ya sabe programar** pero quiere dominar la contenerización y la orquestación en profundidad: de "¿qué es un contenedor?" a desplegar un stack completo multi-servicio con Helm en un clúster local.

El curso debe ser **extremadamente didáctico** y orientado al **por qué**, no a memorizar comandos. No quiero únicamente memorizar `docker run` o `kubectl apply`; quiero entender **por qué existe cada abstracción** (imagen, capa, volumen, pod, deployment, service…), **qué problema resuelve**, cómo se comportan Docker y Kubernetes bajo el capó y **cómo trabajar de verdad en producción** (persistencia, red, configuración, secretos, seguridad y operación).

---

# Hilo conductor: HarborTasks

**HarborTasks** — un gestor de tareas (to-do) en miniatura compuesto por:

1. `web` — interfaz estática servida por **nginx**.
2. `api` — **API REST** en Node.js (Express).
3. `postgres` — base de datos **PostgreSQL**.

Evolución del aprendizaje:

- Módulo 1–3: cada pieza se conteneriza individualmente en Docker.
- Módulo 4: el stack completo se orquesta **localmente con Docker Compose**.
- Módulo 5–8: el mismo stack se traslada a **Kubernetes** (kind) con Deployment, Service, ConfigMap, Secret, PVC e Ingress.
- Módulo 9: el despliegue final se empaqueta con un **chart de Helm** reutilizable.

---

# Tecnologías

Utilizar SIEMPRE:

- Docker Engine + Docker CLI + Buildx
- Dockerfile multi-stage y `.dockerignore`
- Docker Compose (`compose.yaml`, comando `docker compose`)
- kind (Kubernetes in Docker) + kubectl
- Helm (charts y `values.yaml`)
- Kubernetes nativo (manifiestos YAML)
- Registros: Docker Hub y GitHub Container Registry (GHCR)
- App de ejemplo: Node.js (Express) para la API, nginx para la web, PostgreSQL para datos
- WSL2 en Windows (recomendado) / bash en Linux y macOS
- VSCode con extensiones Docker, Kubernetes y Remote - WSL

No utilizar:

- EKS/AKS/GKE como requisito (solo comparativas)
- Minikube (usar kind; mencionar en comparativas)
- Rancher, OpenShift, Docker Swarm (solo mencionados)
- Istio, service meshes, Operators (solo mencionados como camino de futuro)
- AWS/Azure/GCP a fondo

---

# Multiplataforma: Linux / macOS / Windows (+ WSL2)

Las diferencias entre sistemas operativos **son contenido del curso**, no notas al margen.

- Shell canónico de los ejemplos: **bash** (Linux, macOS y WSL2).
- Callouts con el equivalente en **PowerShell** cuando la sintaxis cambie.
- Windows: **trabajar dentro de WSL2 con Ubuntu** + Docker Desktop; explicar el porqué.
- Lección 04 del módulo 0 dedicada a WSL2 (qué es, WSL1 vs WSL2, por qué los contenedores Linux no corren en Windows, integración Docker Desktop ↔ WSL, filesystem `\\wsl$\` vs `/mnt/c`, red, VSCode Remote - WSL, errores típicos).
- Tabla de diferencias por tema dentro de `specs.md` (rutas, `network_mode: host`, `localhost`, LF/CRLF, grupo docker, instalación de kubectl/kind/helm).

---

# Forma de enseñar

Cada lección debe seguir exactamente esta estructura.

# 1. Introducción
Qué vamos a aprender. Por qué existe. Qué problema resuelve. Cuándo se utiliza. Errores típicos.

# 2. Teoría
Concepto con mucho detalle, sin asumir conocimientos previos. Comparaciones del mundo real (barcos, envíos, recetas, alquiler). Explicar el porqué de cada decisión de diseño. Diagramas ASCII cuando ayuden.

# 3. Ejemplo mínimo
El ejemplo más pequeño posible. Explicar línea por línea / comando por comando.

# 4. Ejemplo real
El mismo concepto aplicado a HarborTasks o a un escenario profesional. Explicar todas las decisiones.

# 5. Buenas prácticas
Qué hacer / qué evitar / por qué. Formato tabla cuando aplique.

# 6. Errores habituales
Ejemplos incorrectos, por qué están mal, y la versión correcta.

# 7. Ejercicio guiado
Ejercicio paso a paso (no demasiado difícil), resuelto al detalle con verificación.

# 8. Reto
Reto adicional sin resolverlo inmediatamente (pista incluida).

# 9. Resumen
Bullets + conceptos clave. Incluir tabla de comandos clave (`Comando | Qué hace`).

# 10. Preguntas frecuentes
FAQ (3–5 `<details>`).

---

# Código

Todo comando, Dockerfile, manifiesto YAML y fragmento de código debe estar **completamente comentado** y **funcionar** con las versiones indicadas.

---

# Organización

Cada módulo está dividido en lecciones. Cada lección es independiente pero enlaza a las anteriores cuando un concepto depende de otro.

---

# Índice del curso

## Módulo 0 — Preparación
- 01-que-es-un-contenedor.html — ¿Qué es un contenedor? Contenedores vs VMs
- 02-arquitectura-docker.html — Arquitectura Docker: daemon, CLI, imagen, registry
- 03-entorno-linux-macos.html — Entorno Linux y macOS
- 04-entorno-windows-wsl2.html — Windows, WSL2 y Docker Desktop (lección propia)
- 05-caso-de-estudio-harbortasks.html — Caso de estudio HarborTasks

## Módulo 1 — Imágenes y Dockerfile
- 01-imagenes-y-capas.html — Imágenes, capas y Union FS
- 02-dockerfile-basico.html — Dockerfile: FROM, RUN, COPY, WORKDIR, CMD
- 03-cmd-vs-entrypoint.html — CMD vs ENTRYPOINT, exec form vs shell form
- 04-multi-stage-buildx.html — Multi-stage builds, .dockerignore y buildx
- 05-publicar-imagenes.html — Etiquetar y publicar imágenes (Docker Hub / GHCR)

## Módulo 2 — Contenedores en ejecución
- 01-docker-run.html — docker run, ps, logs y exec
- 02-puertos-y-env.html — Puertos y variables de entorno
- 03-ciclo-de-vida-restart.html — Ciclo de vida, PID 1, señales y restart policies
- 04-recursos-healthcheck.html — Límites de CPU/memoria y HEALTHCHECK

## Módulo 3 — Datos y redes Docker
- 01-volumes-y-bind-mounts.html — Volúmenes, bind mounts y tmpfs
- 02-redes-docker.html — Redes bridge, host, none y DNS interno
- 03-postgres-persistente.html — Caso: Postgres persistente en Docker

## Módulo 4 — Docker Compose
- 01-introduccion-compose.html — Qué es Compose y por qué existe
- 02-compose-yaml.html — compose.yaml: services, ports, env, volumes
- 03-dependencias-y-healthchecks.html — depends_on y healthchecks entre servicios
- 04-stack-harbortasks.html — Stack Compose completo de HarborTasks

## Módulo 5 — De Docker a Kubernetes
- 01-por-que-kubernetes.html — Qué no resuelve Compose y qué aporta K8s
- 02-arquitectura-kubernetes.html — Control plane, nodos, etcd, kubelet
- 03-kind-y-kubectl.html — kind, kubectl y primer contacto
- 04-yaml-declarativo.html — Manifiestos YAML y modelo declarativo

## Módulo 6 — Workloads
- 01-pods.html — Pods: la unidad mínima
- 02-deployments.html — Deployments y rolling updates
- 03-services-y-dns.html — Services: ClusterIP, NodePort, LoadBalancer
- 04-jobs-y-cronjobs.html — Jobs y CronJobs
- 05-probes.html — Probes: liveness, readiness, startup

## Módulo 7 — Configuración y almacenamiento
- 01-configmaps.html — ConfigMaps
- 02-secrets.html — Secrets
- 03-persistent-volumes.html — PersistentVolumes y PVCs
- 04-ingress.html — Ingress y entrada al clúster

## Módulo 8 — Helm, seguridad y operación
- 01-helm-basico.html — Helm: charts, values y releases
- 02-helm-harbortasks.html — Chart de HarborTasks
- 03-seguridad-rbac.html — SecurityContext, no-root y RBAC básico
- 04-limites-y-hpa.html — Límites de recursos y HPA
- 05-logs-y-troubleshooting.html — Logs, kubectl debug y troubleshooting

## Módulo 9 — Proyecto final
- 01-arquitectura-despliegue.html — Arquitectura del despliegue final
- 02-desplegar-api-y-web.html — Desplegar API y web
- 03-postgres-y-ingress.html — Postgres en clúster + Ingress
- 04-verificacion-produccion.html — Verificación y checklist de producción

## Recursos
- cheatsheet.html — Cheatsheet Docker / kubectl / Helm
- glosario.html — Glosario
- faq.html — FAQ general

**Total: 10 módulos, 43 lecciones + 3 recursos.**

---

# Formato de entrega

Web estática (HTML + CSS + JavaScript vanilla, sin Markdown ni build). La carpeta `web/` contiene todo el contenido navegable. Cada lección es un archivo HTML completo. El chrome (cabecera, sidebar, progreso, tema claro/oscuro) lo inyecta `web/assets/js/site.js` a partir del mapa en `web/assets/js/course-data.js` (única fuente de verdad). El proyecto final vive en `proyectos/harbortasks/` y es ejecutable de verdad (Compose + kind + Helm).

Estructura:

```text
Aprende Docker/
├── specs.md                    ← spec maestra
├── AGENTS.md
├── package.json
├── pnpm-workspace.yaml
├── .gitignore
├── .gitattributes              ← LF (evita CRLF en Dockerfiles)
├── web/
│   ├── index.html              ← portada + índice
│   ├── assets/{css,js,fonts}   ← site.css, course-data.js, site.js, highlight.js
│   ├── modulos/00-preparacion/ … 09-proyecto-final/
│   └── recursos/               ← cheatsheet, glosario, faq
├── scripts/serve-web.mjs
└── proyectos/harbortasks/
    ├── README.md
    ├── compose.yaml
    ├── api/{Dockerfile, package.json, src/index.js}
    ├── web/{Dockerfile, nginx.conf, index.html}
    ├── k8s/{namespace.yaml, secrets.yaml, api/, web/, postgres/}    └── helm/harbortasks/{Chart.yaml, values.yaml, templates/}
```

Anatomía de una lección (idéntica a la familia Academy):

```html
<!DOCTYPE html>
<html lang="es" data-theme="dark" data-module="04" data-lesson="02-compose-yaml">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>… — Módulo N · Curso Docker y Kubernetes</title>
  <script>document.documentElement.dataset.theme = localStorage.getItem('dk-theme') || 'dark';</script>
  <link rel="stylesheet" href="../../assets/css/site.css">
  <link rel="icon" href="../../assets/favicon.svg" type="image/svg+xml">
</head>
<body>
  <main><article class="lesson">
    <header class="lesson-header">…</header>
    <section id="introduccion">…</section>
    … 10 secciones …
  </article></main>
  <script>window.SITE_BASE = '../../';</script>
  <script src="../../assets/js/course-data.js"></script>
  <script src="../../assets/js/highlight.js"></script>
  <script src="../../assets/js/site.js"></script>
</body>
</html>
```

Notas:
- `SITE_BASE` = `'../../'` en lecciones (`modulos/XX/`), `'./'` en portada, `'../'` en `web/recursos/`.
- Claves de localStorage: `dk-theme`, `dk-progress`.
- Bloques disponibles: `callout callout--why/--tip/--note/--warning/--analogy`, `challenge`, `faq details`, `table`, `ol.steps`, `pre > code.language-*`, tokens `t-kw t-str t-num t-pun t-fn t-bt t-com t-tag t-attr`.

---

# Diseño y marca

- Tema oscuro por defecto, toggle a claro.
- Paleta: Docker azul `#2496ED`, Kubernetes azul `#326CE5`, fondo `#0d1117`.
- Firma de portada: sesión de terminal con `docker`/`kubectl`/`helm`.
- Identificadores en inglés; UI y comentarios en español.

---

# Convenciones del curso

1. Idioma: contenido, comentarios y UI en español; identificadores/comandos en inglés.
2. Clúster local: kind, siempre. Minikube solo en comparativa.
3. Proyecto de muestra: `harbortasks`, namespace `harbortasks`.
4. Cada lección usa SIEMPRE las 10 secciones.
5. Rutas canónicas en Linux con callouts macOS/Windows.
6. Retos sin resolver en el HTML (solo pista).
7. En el Resumen, tabla de comandos clave.

---

# Documentación actualizada

Consultar la documentación oficial (docs.docker.com, kubernetes.io/docs, helm.sh/docs) vía Context7 o web. Preferir la versión estable más reciente: Docker Engine 28.x / Compose v2 / Buildx, Kubernetes 1.31+ / kind 0.2x, Helm 3.x, Node LTS + Express 4/5, nginx 1.27+, PostgreSQL 16/17.

---

# Reglas importantes

1. Nunca des por sentado un concepto.
2. Explica siempre el "por qué" de cada abstracción.
3. Todo código debe ejecutarse con las versiones indicadas.
4. Usa `docker` y `kubectl`/`helm` en los comandos de ejemplo; nada de `sudo` como requisito.
5. Sigue las versiones oficiales actuales.
6. No omitas pasos de configuración (instalar Docker Desktop, activar WSL, kind, kubectl, helm).
7. Mantén una progresión lógica entre lecciones.
8. Si un concepto depende de otro, enlaza a la lección correspondiente.
9. Prioriza la comprensión profunda frente a la velocidad.
10. En Windows, la vía recomendada es WSL2; añade el callout correspondiente cuando la lección difiera.