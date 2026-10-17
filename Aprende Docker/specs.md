# Página web estática para aprender Docker y Kubernetes.

# Curso Completo de Docker y Kubernetes

## Del contenedor local al clúster — con proyectos reales

---

# 1. Objetivo

Actúa como un ingeniero senior de plataforma (Platform Engineering) con muchos años de experiencia llevando aplicaciones a producción con contenedores y orquestación.

El objetivo es crear **un curso completo de Docker y Kubernetes**, pensado para un **desarrollador que ya sabe programar** pero quiere dominar la contenerización y la orquestación en profundidad: de "¿qué es un contenedor?" a desplegar un stack completo multi-servicio con Helm en un clúster local.

El curso debe ser **extremadamente didáctico** y orientado al **por qué**, no a memorizar comandos.

No quiero únicamente memorizar `docker run` o `kubectl apply`; quiero entender **por qué existe cada abstracción** (imagen, capa, volumen, pod, deployment, service…), **qué problema resuelve**, cómo se comportan Docker y Kubernetes bajo el capó y **cómo trabajar de verdad en producción** (persistencia, red, configuración, secretos, seguridad y operación).

---

# 2. Audiencia y requisitos previos

- Desarrolladores/as que ya programan en cualquier lenguaje (Node, .NET, Java, Python…).
- Se asume: saber usar una terminal, Git, variables de entorno y una API HTTP básica.
- **No** se asume ningún conocimiento previo de Docker, Kubernetes, contenedores ni virtualización.
- Toda la teoría se acompaña de la práctica real ejecutable en el sistema del alumno.

---

# 3. Hilo conductor del curso: HarborTasks

Todo el curso gira alrededor de **un caso de estudio real** que se construye de forma incremental:

> **HarborTasks** — un gestor de tareas (to-do) en miniatura compuesto por tres piezas:
>
> 1. `web` — interfaz estática servida por **nginx** (HTML/CSS/JS vanilla).
> 2. `api` — **API REST** en Node.js (Express) que expone el CRUD de tareas.
> 3. `postgres` — base de datos **PostgreSQL** que persiste las tareas.

El alumno verá el mismo stack evolucionar:

- **Módulo 1–3:** cada pieza se conteneriza individualmente en Docker.
- **Módulo 4:** el stack completo se orquesta **localmente con Docker Compose**.
- **Módulo 5–8:** el mismo stack se traslada a **Kubernetes** (kind) con Deployment, Service, ConfigMap, Secret, PVC e Ingress.
- **Módulo 9:** el despliegue final se empaqueta con un **chart de Helm** reutilizable.

Poder ver el mismo sistema pasar de "comando suelto" a "clúster orquestado" cierra el círculo de aprendizaje y justifica cada nueva abstracción.

---

# 4. Tecnologías

## Utilizar SIEMPRE

- **Docker Engine + Docker CLI** (motor de contenedores) y **Buildx** (builder multi-plataforma).
- **Dockerfile** multi-stage y `.dockerignore`.
- **Docker Compose** (`compose.yaml`, comando `docker compose`).
- **kind** (Kubernetes in Docker) como clúster local de Kubernetes + **kubectl**.
- **Helm** (charts y `values.yaml`).
- **Kubernetes nativo** (manifiestos YAML, sin abstracciones tipo kustomize en profundidad; Helm sí).
- Registros de imágenes: **Docker Hub** y **GitHub Container Registry (GHCR)**.
- Aplicación de ejemplo: **Node.js (Express)** para la API, **nginx** para la web, **PostgreSQL** para datos.
- PowerShell / bash según el sistema, con **WSL2 en Windows** como entorno recomendado.
- VSCode con extensiones: **Docker**, **Kubernetes**, **Remote - WSL**.

## No utilizar

- Kubernetes de pago o gestionado (EKS/AKS/GKE) como requisito del curso (solo comparativas).
- Minikube (usar **kind**, más ligero y reproducible); se menciona en comparativas.
- Rancher, OpenShift, Docker Swarm (solo mencionados como alternativas del ecosistema).
- Istio, service meshes, Operators (solo mencionados como camino de futuro).
- AWS / Azure / GCP a fondo (solo comparativas de despliegue en la nube).
- `sudo` en los ejemplos de Docker (el alumno configura su grupo `docker` o usa Docker Desktop).

---

# 5. Forma de enseñar

Cada lección debe seguir **exactamente** esta estructura (numeración visible en el HTML con la clase `section-number`).

## 1. Introducción
Qué vamos a aprender. Por qué existe. Qué problema resuelve. Cuándo se utiliza. Errores típicos de esa pieza.

## 2. Teoría
Explicar el concepto con mucho detalle, **sin asumir conocimientos previos** de Docker/K8s. Usar comparaciones del mundo real (cargas en un barco, envíos postales, recetas de cocina, alquiler de apartamentos vs vivienda completa…). Explicar el **porqué** de cada decisión de diseño. Incluir **diagramas ASCII** cuando ayuden (capas de imagen, arquitecturas, flujos de peticiones, planos de control).

## 3. Ejemplo mínimo
El ejemplo más pequeño posible (un comando, un Dockerfile de 8 líneas, un Pod de 15 líneas). Explicar **línea por línea/comando por comando**.

## 4. Ejemplo real
El mismo concepto aplicado a **HarborTasks** o a un escenario profesional. Explicar todas las decisiones tomadas.

## 5. Buenas prácticas
Qué hacer / qué evitar / por qué. Formato tabla cuando aplique.

## 6. Errores habituales
Ejemplos incorrectos, por qué están mal, y la versión correcta.

## 7. Ejercicio guiado
Ejercicio paso a paso, no demasiado difícil, resuelto al detalle (con el resultado esperado y verificación).

## 8. Reto
Reto adicional **sin resolverlo inmediatamente** (pista incluida, debe intentarlo el alumno).

## 9. Resumen
Resumen con bullets + conceptos clave. Puede incluir la "tabla de comandos clave" de la lección.

## 10. Preguntas frecuentes
FAQ relacionada con la lección (3–5 `<details>`).

## Reglas de código
- Todo comando, Dockerfile, manifiesto YAML y fragmento de código debe estar **completamente comentado**.
- Todo código de la lección debe **funcionar** con las versiones indicadas.
- Cuando haya varias formas de hacer algo (CLI vs flag vs config), mostrar la recomendada y mencionar las alternativas.

---

# 6. Multiplataforma: Linux / macOS / Windows (+ WSL2)

Las diferencias entre sistemas operativos **son contenido del curso**, no notas al margen. Todo lo que cambie según el sistema se explica con variantes. Convenciones fijas:

## Shell canónico
- Ejemplos en **bash** (Linux, macOS y WSL2).
- Callouts con el equivalente en **PowerShell** cuando la sintaxis cambie (`$env:VAR` vs `$VAR`, comillas, escapes, heredoc `@'...'@`).

## Recomendación Windows
- **Trabajar dentro de WSL2 con una distro Ubuntu**, no Docker "a medias" desde PowerShell. Se explica el porqué (ver M0-04).
- En macOS y Linux se trabaja en la terminal nativa.

## Tabla de diferencias clave que deben aparecer en cada lección afectada

| Tema | Linux | macOS | Windows |
|---|---|---|---|
| Motor Docker | Docker Engine nativo (o Docker Desktop) | Docker Desktop (hypervisor) o Colima/OrbStack | Docker Desktop + WSL2 |
| Ruta del proyecto | `/home/usuario/proyectos/harbortasks` | `/Users/usuario/proyectos/harbortasks` | **En WSL:** `~/proyectos/...` (FS ext4, rápido) · **En Windows:** `C:\Users\...` · mapeo `\\wsl$\Ubuntu\...` |
| `/mnt/c` | n/a | n/a | accesible desde WSL PERO lento para bind mounts |
| `network_mode: host` | funciona | **no es el host real** (VM) | no aplica igual (VM/WSL2) |
| `localhost` | host real | puerto publicado por la VM lo redirige | WSL2 redirige localhost; a veces hay que usar `--network=host` políticas |
| Montaje LF/CRLF | LF | LF | cuidar CRLF en Dockerfiles y scripts (`Añadir .gitattributes`) |
| Grupo del usuario `docker` | `usermod -aG docker $USER` | n/a (Desktop no lo necesita) | n/a |
| kubectl / kind / helm | binario nativo | binario nativo | **instalar dentro de WSL** (Linux binaries) |

## Lección dedicada: WSL2 (Módulo 0, lección 04)
Explicar a fondo:
1. Qué es WSL y **por qué existe** (Windows necesita un kernel Linux para contenedores).
2. **WSL1 vs WSL2**: traducción de syscalls vs VM ligera con kernel Linux real.
3. Por qué **los contenedores Linux no corren en Windows**: usan el kernel del host; Docker Desktop levanta una VM WSL2 y ese es el "host" de los contenedores.
4. Contenedores **Linux vs Windows containers** → este curso usa **solo Linux containers**.
5. Instalación: `wsl --install`, distro **Ubuntu** (o Debian), **Windows Terminal**.
6. Integración Docker Desktop ↔ WSL: *Settings → Resources → WSL Integration*, marcar la distro.
7. Filesystem: `\\wsl$\Ubuntu\home\...` y `/mnt/c/...`; por qué **el proyecto debe vivir en el FS de Linux** (rendimiento de I/O).
8. Red: cómo Docker publica `localhost` en Windows; cuándo usar la IP de WSL.
9. VSCode **Remote - WSL** (abrir la carpeta del proyecto dentro de WSL).
10. Errores típicos: *daemon not running*, distro sin integración activada, CRLF en Dockerfile, rutas de Compose mezcladas entre Windows y Linux.

## Convención de rutas en el curso
- Las rutas de ejemplo se escriben en **formato Linux** (canónico) y se añade el callout del equivalente macOS/Windows cuando difiera.
- En Windows se da **siempre** la alternativa que funciona: desarrollar dentro de WSL2 en `~/...`.

---

# 7. Índice del curso

## Módulo 0 — Preparación
- **01-que-es-un-contenedor.html** — ¿Qué es un contenedor? Contenedores vs máquinas virtuales (aislamiento, namespace, cgroups, el "por qué" histórico).
- **02-arquitectura-docker.html** — Arquitectura Docker: daemon, CLI, cliente-servidor, imágenes, registros, runtime containerd.
- **03-entorno-linux-macos.html** — Entorno Linux y macOS: Docker Engine nativo vs Docker Desktop; verificación (`docker version`, `docker info`, `docker run hello-world`).
- **04-entorno-windows-wsl2.html** — **Windows, WSL2 y Docker Desktop** (lección propia; contenidos de la sección anterior).
- **05-caso-de-estudio-harbortasks.html** — Caso de estudio HarborTasks: arquitectura, repositorio, plan del curso.

## Módulo 1 — Imágenes y Dockerfile
- **01-imagenes-y-capas.html** — Imágenes, capas, Union FS y copy-on-write. `docker pull`, `docker images`, `docker inspect`.
- **02-dockerfile-basico.html** — Dockerfile: `FROM`, `RUN`, `COPY`, `WORKDIR`, `EXPOSE`, `CMD`. Buenas prácticas de orden y caché.
- **03-cmd-vs-entrypoint.html** — `CMD` vs `ENTRYPOINT`, `exec form` vs `shell form`, argumentos en `docker run`.
- **04-multi-stage-y-buildx.html** — Multi-stage builds (compilar en una etapa, producir la imagen final), `.dockerignore`, `docker buildx` y tamaño de imagen.
- **05-publicar-imagen.html** — Etiquetado (`tag`), publicar en Docker Hub y GHCR, `docker push/pull`, inmutabilidad de imágenes.

## Módulo 2 — Contenedores en ejecución
- **01-docker-run.html** — `docker run`, `docker ps`, `docker logs`, `docker exec`, `docker rm/stop/start`, flags esenciales.
- **02-puertos-y-env.html** — Publicación de puertos (`-p`), variables de entorno (`-e`, `--env-file`), redes externas.
- **03-ciclo-de-vida-y-restart.html** — Ciclo de vida, PID 1 y señales, `--restart`, `stop --time`, entradas y salidas del contenedor.
- **04-recursos-y-healthcheck.html** — Límites de CPU/memoria (cgroups), `--memory`, `--cpus`, `--pids-limit`, `HEALTHCHECK` y `docker stats`.

## Módulo 3 — Datos y redes Docker
- **01-volumes-y-bind-mounts.html** — Volúmenes, bind mounts y tmpfs; persistencia, permisos, `--mount` vs `-v`.
- **02-redes-docker.html** — Redes bridge, host y none; DNS interno, `--network`, comunicación entre contenedores, `docker network`.
- **03-postgres-persistente.html** — Caso: Postgres contenerizado con volumen, variables de entorno y datos persistentes (aplica directo a HarborTasks).

## Módulo 4 — Docker Compose
- **01-introduccion-compose.html** — Qué es Compose y qué problema resuelve (multi-contenedor como una unidad).
- **02-compose-yaml.html** — `compose.yaml`: services, image/build, ports, env, volumes. `docker compose up/down/logs/config`.
- **03-dependencias-y-healthchecks.html** — `depends_on` (con condición), healthchecks entre servicios, orden de arranque.
- **04-stack-harbortasks.html** — Stack Compose completo de HarborTasks (web+api+postgres). Perfiles/dev vs prod. **Fin de bloque Docker local.**

## Módulo 5 — De Docker a Kubernetes
- **01-por-que-kubernetes.html** — Qué NO resuelve Compose (escalado, alta disponibilidad, auto-reparación, multi-host) y qué aporta Kubernetes.
- **02-arquitectura-kubernetes.html** — Control plane (kube-apiserver, etcd, scheduler, controller-manager) y nodos (kubelet, kube-proxy, runtime).
- **03-kind-y-kubectl.html** — Clúster local con **kind** (Kubernetes in Docker), `kubectl` (context, get, describe, apply, delete, logs, port-forward). Uso con WSL.
- **04-yaml-declarativo.html** — El modelo declarativo de Kubernetes, YAML, `apiVersion/kind/metadata/spec`, idempotencia y reconciliación.

## Módulo 6 — Workloads
- **01-pods.html** — Pods: la unidad mínima; contenedores, initContainers, ciclo de vida de un Pod, `kubectl exec`.
- **02-deployments.html** — Deployments y ReplicaSets: `kubectl create deployment`, replicas, rolling updates, `kubectl rollout`.
- **03-services-y-dns.html** — Services: ClusterIP, NodePort, LoadBalancer; DNS de clúster, `kubectl port-forward`, `kubectl expose`.
- **04-jobs-y-cronjobs.html** — Jobs y CronJobs: tareas puntuales y programadas; backoffLimit, paralelismo.
- **05-probes.html** — Probes: liveness, readiness, startup; `kubectl describe pod`, reintentos y consecuencias.

## Módulo 7 — Configuración y almacenamiento en K8s
- **01-configmaps.html** — ConfigMaps: configuración declarativa, env y volúmenes, inmutabilidad.
- **02-secrets.html** — Secrets: Base64, mejores prácticas, `--from-literal`, `--from-env-file`, Secret vs ConfigMap.
- **03-persistent-volumes.html** — PersistentVolumes, PersistentVolumeClaims, StorageClass y accessModes.
- **04-ingress.html** — Ingress y controlador (ingress-nginx en kind), reglas de host/path, HTTPS/TLS introduction.

## Módulo 8 — Helm, seguridad y operación
- **01-helm-basico.html** — Helm: por qué existe, charts, `helm create`, `helm install/upgrade/rollback`, values.yaml.
- **02-helm-harbortasks.html** — Chart completo de HarborTasks (Deployment + Service + ConfigMap + Secret + PVC + Ingress) con plantillas y values.
- **03-seguridad-rbac.html** — SecurityContext, non-root, `runAsNonRoot`, capabilities; RBAC básico (ServiceAccount, Role, RoleBinding) y namespace aislado.
- **04-limites-y-hpa.html** — Requests/limits de recursos, `kubectl top`, HorizontalPodAutoscaler y `kubectl scale`.
- **05-logs-y-troubleshooting.html** — `kubectl logs`, `kubectl describe`, `kubectl debug`/ephemeral containers, patrones de diagnóstico (CrashLoopBackOff, ImagePullBackOff, Pending).

## Módulo 9 — Proyecto final
- **01-arquitectura-despliegue.html** — Arquitectura del despliegue final: namespace, componentes y flujo de datos del stack HarborTasks en K8s.
- **02-desplegar-api-y-web.html** — Desplegar API y web: Deployment, Service, ConfigMap, Secret, probes y rollout.
- **03-postgres-y-ingress.html** — PostgreSQL en K8s (PVC, Secret, headless/Service, init) e Ingress con paths a api y web.
- **04-verificacion-y-produccion.html** — Verificación end-to-end, checklist de producción, próximos pasos (CI/CD, observabilidad).

## Recursos
- **cheatsheet.html** — Cheatsheet Docker / kubectl / Helm (comandos clave por tema).
- **glosario.html** — Glosario de Docker y Kubernetes (imagen, capa, volumen, pod, deployment, service…).
- **faq.html** — FAQ general del curso.

**Total: 10 módulos, 43 lecciones + 3 recursos.**

---

# 8. Estructura de repositorio

```text
Aprende Docker/
│
├── specs.md                     ← ★ ESTE DOCUMENTO
├── AGENTS.md                    ← misma spec, en formato de instrucciones para agentes
├── package.json                 ← script de servidor estático
├── pnpm-workspace.yaml          ← proyectos/* (si hay paquetes pnpm opcionales)
├── .gitignore
├── .gitattributes               ← LF para repo (evita CRLF en Dockerfiles)
│
├── web/                         ← ★ EL CURSO (web estática, sin build)
│   ├── index.html               ← portada + índice completo
│   ├── assets/
│   │   ├── css/site.css         ← tema claro/oscuro, identidad Docker/K8s
│   │   ├── js/course-data.js    ← mapa del curso (única fuente de verdad)
│   │   ├── js/highlight.js      ← resaltado de sintaxis
│   │   ├── js/site.js           ← chrome: sidebar, topbar, progreso, tema
│   │   ├── fonts/               ← JetBrains Mono (woff2)
│   │   └── favicon.svg
│   ├── modulos/
│   │   ├── 00-preparacion/
│   │   ├── 01-imagenes-y-dockerfile/
│   │   ├── 02-contenedores/
│   │   ├── 03-datos-y-redes/
│   │   ├── 04-docker-compose/
│   │   ├── 05-de-docker-a-kubernetes/
│   │   ├── 06-workloads/
│   │   ├── 07-configuracion-y-almacenamiento/
│   │   ├── 08-helm-seguridad-operacion/
│   │   └── 09-proyecto-final/
│   └── recursos/                ← cheatsheet.html, glosario.html, faq.html
│
├── scripts/
│   └── serve-web.mjs            ← servidor estático (node)
│
├── ejemplos/                    ← demos HTML planos y ejecutables (opcional)
│
└── proyectos/
    └── harbortasks/             ← ★ PROYECTO FINAL
        ├── README.md
        ├── compose.yaml         ← stack local (web + api + postgres)
        ├── api/
        │   ├── Dockerfile       ← multi-stage Node
        │   ├── package.json
        │   └── src/index.js     ← API Express + /healthz
        ├── web/
        │   ├── Dockerfile       ← nginx
        │   ├── nginx.conf       ← proxy /api → api:3000
        │   └── index.html       ← SPA
        ├── k8s/                 ← manifiestos Kubernetes "a mano"
        │   ├── namespace.yaml
        │   ├── secrets.yaml
        │   ├── api/ (deployment, service, configmap, hpa)
        │   ├── web/ (deployment, service, configmap)
        │   └── postgres/ (deployment, service, pvc, configmap)
        └── helm/harbortasks/    ← chart de Helm
            ├── Chart.yaml
            ├── values.yaml
            └── templates/ (deployment-api.yaml, deployment-web.yaml,
                            deployment-postgres.yaml, postgres-pvc.yaml,
                            postgres-configmap.yaml, service-*.yaml,
                            configmap.yaml, secret.yaml, ingress.yaml, hpa.yaml)
```

---

# 9. Formato de entrega

El curso se entrega como **web estática** (HTML + CSS + JavaScript vanilla, **sin Markdown y sin paso de build**), exactamente igual que el resto de cursos de la Academy (`Aprende Javascript`, `Aprende Redis`, etc.).

- `web/` contiene todo el contenido navegable.
- Cada lección es un **archivo HTML completo**.
- El chrome (cabecera, sidebar, progreso, tema claro/oscuro) lo inyecta `web/assets/js/site.js` a partir del mapa del curso en `web/assets/js/course-data.js` (**única fuente de verdad** para navegación, enlaces, progreso y búsqueda).
- El proyecto final vive en `proyectos/harbortasks/` y es **ejecutable de verdad** con Docker/kind/Helm (no maquetas).

## Anatomía de una página de lección (idéntica a los otros cursos)

```html
<!DOCTYPE html>
<html lang="es" data-theme="dark" data-module="04" data-lesson="02-compose-yaml">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Lección — Módulo N · Curso Docker y Kubernetes</title>
  <script>document.documentElement.dataset.theme = localStorage.getItem('dk-theme') || 'dark';</script>
  <link rel="stylesheet" href="../../assets/css/site.css">
  <link rel="icon" href="../../assets/favicon.svg" type="image/svg+xml">
</head>
<body>
  <main>
    <article class="lesson">
      <header class="lesson-header">
        <h1>Título de la lección</h1>
        <div class="lesson-header__meta">
          <span class="badge">Módulo N · Nombre módulo</span>
          <span>⏱ 30 minutos</span>
        </div>
      </header>
      <section id="introduccion"><h2><span class="section-number">1 ·</span> Introducción</h2>…</section>
      <section id="teoria"><h2><span class="section-number">2 ·</span> Teoría</h2>…</section>
      <section id="ejemplo-minimo"><h2><span class="section-number">3 ·</span> Ejemplo mínimo</h2>…</section>
      <section id="ejemplo-real"><h2><span class="section-number">4 ·</span> Ejemplo real</h2>…</section>
      <section id="buenas-practicas"><h2><span class="section-number">5 ·</span> Buenas prácticas</h2>…</section>
      <section id="errores"><h2><span class="section-number">6 ·</span> Errores habituales</h2>…</section>
      <section id="ejercicio"><h2><span class="section-number">7 ·</span> Ejercicio guiado</h2>…</section>
      <section id="reto"><h2><span class="section-number">8 ·</span> Reto</h2>…</section>
      <section id="resumen"><h2><span class="section-number">9 ·</span> Resumen</h2>…</section>
      <section id="faq"><h2><span class="section-number">10 ·</span> Preguntas frecuentes</h2>…</section>
    </article>
  </main>
  <script>window.SITE_BASE = '../../';</script>
  <script src="../../assets/js/course-data.js"></script>
  <script src="../../assets/js/highlight.js"></script>
  <script src="../../assets/js/site.js"></script>
</body>
</html>
```

**Nota importantísima:** `window.SITE_BASE` cambia según la profundidad del archivo:
- `'./'` en `web/index.html` (portada) → rutas `./assets/...`, `./modulos/...`
- `'../'` en `web/recursos/*.html` (a un nivel de `web/`) → rutas `../assets/...`
- `'../../'` en `web/modulos/XX/*.html` (a dos niveles) → rutas `../../assets/...`

## Bloques HTML disponibles (heredados del sistema de diseño de la Academy)
- `callout callout--why / --tip / --note / --warn / --bad / --analogy` (destacados pedagógicos).
- `challenge` (reto, con `h3` y pista).
- `faq > details > summary` (FAQ).
- `table` con `thead/tbody` (comparativas y buenas prácticas).
- `ol.steps > li > strong` (ejercicios guiados paso a paso).
- `pre > code.language-*` (bloques de código con botón copiar; highlight.js).
- Tokens de sintaxis para código coloreado: `t-kw`, `t-str`, `t-num`, `t-pun`, `t-fn`, `t-bt`, `t-com`, `t-tag`, `t-attr`.

---

# 10. Diseño y marca

- **Tema oscuro por defecto**, toggle a claro (botón en topbar, prefijo `dk-` en localStorage: `dk-theme`, `dk-progress`).
- **Paleta Docker y Kubernetes**:
  - Acento Docker: azul `#2496ED`.
  - Acento Kubernetes: azul `#326CE5`.
  - Fondo oscuro tipo terminal: `#0d1117` (heredado de la familia Academy, azul oscuro).
  - En la portada, elemento firma: **una sesión de terminal** en la que el curso "se presenta a sí mismo" con `docker run` / `kubectl` / `helm` (equivalente a la consola REPL del curso de JavaScript, pero con prompt `$` y comandos de Docker).
- Tipografía: misma familia que el resto (sans del sistema + **JetBrains Mono** para código y chrome).
- Identificadores de código en inglés; **UI y comentarios en español**.

---

# 11. Convenciones del curso

1. Idioma: contenido, comentarios y UI en **español**.
2. Identificadores, nombres de servicios, manifests y comandos en **inglés** (estándar profesional) con comentarios en español.
3. Clúster local: **kind**, siempre. `minikube` solo en comparativa. Uso en Windows a través de WSL2.
4. Nombres y etiquetas: el proyecto de muestra se llama `harbortasks` y su namespace de K8s es `harbortasks`.
5. Cada lección usa SIEMPRE las 10 secciones de "Forma de enseñar".
6. Rutas canónicas en Linux con callouts macOS/Windows.
7. En las lecciones de AWS/cloud NO se pide crear cuentas de pago; se describe conceptualmente.
8. Los retos quedan **sin resolver** en el HTML (solo pista).
9. Incluir una "tabla de comandos" en el Resumen de las lecciones con comandos (tabla `Comando | Qué hace`).

---

# 12. Documentación actualizada

Antes de afirmar versiones de comandos, imágenes Docker o recursos, consultar la documentación oficial (docs.docker.com, kubernetes.io/docs, helm.sh/docs) vía **Context7** o web. Preferir la versión estable más reciente:
- Docker Engine 28.x / Compose v2, Buildx.
- Kubernetes 1.31+ / kind 0.2x.
- Helm 3.x.
- Node LTS + Express 4/5, nginx 1.27+, PostgreSQL 16/17.

---

# 13. Página principal (web/index.html)

Adaptar la plantilla del curso de JavaScript:

- **Hero**: elemento firma "terminal" (`$`, `>`) con líneas como:
  ```
  $ docker run -it --rm harbor-cli intro
  ✓ 10 módulos · 39 lecciones · ≈25 horas · Docker + Kubernetes
  $ kubectl get curso
  NAME     STATUS    READY
  docker   Running   1/1
  kubernetes Ready    1/1
  ```
- Bloque "Cómo funciona este curso" (10 secciones, ejecutable local, kind/Helm).
- Estadísticas calculadas por `site.js` (módulos, lecciones, horas).
- Índice de módulos generado (`#module-list`).
- Botón "Continuar donde lo dejaste".

---

# 14. Checklist de implementación (TODO ejecutable)

## Fase 0 — Este documento (specs.md) ✅
- [x] Redactar `specs.md` con contenido, multi-OS/WSL, índice y checklist.

## Fase 1 — Scaffold del repositorio ✅
- [x] Crear `Aprende Docker/` con estructura de la sección 8.
- [x] Copiar de `Aprende Javascript/`: `.agents/skills/documentation` y `frontend-design` (SKILL.md + LICENSE.txt).
- [x] Context7 MCP: `opencode.json` en la raíz de la Academy (movido allí al unificar el repositorio).
- [x] Crear `package.json` (script `serve`), `pnpm-workspace.yaml`, `.gitignore`, `.gitattributes` (LF).
- [x] Copiar/adaptar `scripts/serve-web.mjs` (cambiar mensaje de puerto y curso).
- [x] Copiar assets: `web/assets/fonts/` (woff2 + fonts.css), `web/assets/favicon.svg` (adaptado a identidad Docker).
- [x] Escribir `web/assets/js/course-data.js` (mapa completo de la sección 7).
- [x] Escribir `web/assets/js/site.js` adaptado: claves `dk-theme`/`dk-progress`, logo "DK", firma terminal en portada.
- [x] Copiar `web/assets/js/highlight.js`.
- [x] Escribir `web/assets/css/site.css` con paleta Docker/K8s.
- [x] Escribir `web/index.html` (portada con terminal + índice).
- [x] Escribir `AGENTS.md` derivado de `specs.md`.

## Fase 2 — Lecciones (web/modulos/XX)
- [x] Módulo 0 (5 lecciones).
- [x] Módulo 1 (5 lecciones).
- [x] Módulo 2 (4 lecciones).
- [x] Módulo 3 (3 lecciones).
- [x] Módulo 4 (4 lecciones).
- [x] Módulo 5 (4 lecciones).
- [x] Módulo 6 (5 lecciones).
- [x] Módulo 7 (4 lecciones).
- [x] Módulo 8 (5 lecciones).
- [x] Módulo 9 (4 lecciones).
- [x] Cada lección: 10 secciones, código comentado y ejecutable, diagramas ASCII, callouts multi-OS.

## Fase 3 — Recursos ✅
- [x] `web/recursos/cheatsheet.html` (Docker, kubectl, Helm).
- [x] `web/recursos/glosario.html`.
- [x] `web/recursos/faq.html`.

## Fase 4 — Proyecto final ejecutable (proyectos/harbortasks) ✅
- [x] `compose.yaml` completo y funcional (web + api + postgres).
- [x] `api/Dockerfile` multi-stage y `web/Dockerfile` + `web/nginx.conf`.
- [x] Manifiestos `k8s/` correctos y aplicables con kind (namespace, secrets, api, web, postgres).
- [x] Chart Helm `helm/harbortasks` con templates y values.
- [x] `README.md` con instrucciones de verificación.
- [ ] Probar que `docker compose up`, `kind create cluster`, `helm install` funcionan (requiere Docker instalado en esta máquina; si no, se ha dejado el código revisado y documentado paso a paso).

## Fase 5 — Integración con la Academy ✅
- [x] Añadir tarjeta `Aprende Docker` en `Academy/index.html` (logo, descripción, enlace a `web/index.html`).
- [x] Actualizar el footer "N cursos disponibles" (8 → 9).

---

# 15. Reglas importantes

1. Nunca des por sentado un concepto.
2. Explica siempre el "por qué" de cada abstracción.
3. Todo código/mani Esto debe funcionar con las versiones indicadas (verifica lo que se pueda; documenta pasos si no hay Docker en el entorno de build).
4. Usa `docker` y `kubectl`/`helm` en los comandos de ejemplo; nada de `sudo` como requisito.
5. Sigue las versiones oficiales actuales (Docker, kind, Helm, Node LTS, nginx, PostgreSQL).
6. No omitas pasos de configuración (instalar Docker Desktop, activar WSL, integración, kind, kubectl, helm).
7. Mantén una progresión lógica: cada lección apoya a la siguiente (M1–M4 Docker, M5–M9 Kubernetes).
8. Si un concepto depende de otro, enlaza a la lección correspondiente.
9. Prioriza la comprensión profunda frente a la velocidad.
10. En Windows, la vía recomendada es WSL2; si una lección de red/volúmenes difiere en Windows, añade el callout correspondiente.