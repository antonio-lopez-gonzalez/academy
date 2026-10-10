# HarborTasks

Proyecto final del curso **Docker y Kubernetes** — un gestor de tareas en miniatura compuesto por:

| Servicio | Tecnología | Función |
|---|---|---|
| `web` | nginx `1.27-alpine` | Interfaz estática (HTML/CSS/JS vanilla) que sirve el SPA y proxya `/api` |
| `api` | Node.js 20 + Express | API REST con CRUD de tareas |
| `postgres` | PostgreSQL `16-alpine` | Persistencia de las tareas |

El mismo stack se ejecuta de tres formas, en orden de complejidad (es el hilo conductor del curso):

1. **Docker Compose** — stack local completo con `compose.yaml`.
2. **Kubernetes (kind)** — manifiestos declarativos en `k8s/`.
3. **Helm** — chart reutilizable en `helm/harbortasks` (despliegue recomendado).

---

## Requisitos

- Docker Engine + Compose v2 (en Windows: Docker Desktop + WSL2, lección M0-04).
- kind + kubectl (Kubernetes 1.31+) — para las opciones 2 y 3.
- Helm 3 — para la opción 3.
- Para construir la imagen de la API: Node 20 LTS (opcional; el Dockerfile multi-stage lo hace dentro del build).

---

## Opción 1 · Docker Compose (desarrollo local)

```bash
# Desde esta carpeta
docker compose up -d --build

# Verificación
docker compose ps
curl http://localhost:3000/api/tasks
open http://localhost:8080   # la web

# Logs y parada
docker compose logs -f api
docker compose down          # conserva el volumen pgdata
docker compose down -v       # borra también los datos de postgres
```

- API → `http://localhost:3000`
- Web → `http://localhost:8080`
- Postgres → `localhost:5432` (BD `harbortasks`, user `harbor`, pass `harborpass` — **solo dev**)

---

## Opción 2 · Kubernetes con kind (manifiestos a mano)

```bash
# 1. Crear el clúster
kind create cluster --name curso

# 2. Cargar las imágenes construidas localmente (sin registry)
docker build -t harbortasks-api:1.0.0 -f api/Dockerfile ./api
docker build -t harbortasks-web:1.0.0 -f web/Dockerfile ./web
kind load docker-image harbortasks-api:1.0.0 harbortasks-web:1.0.0

# 3. Aplicar manifiestos en orden
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/api/ k8s/web/ k8s/postgres/ k8s/secrets.yaml

# 4. Verificación
kubectl get pods -n harbortasks -w
kubectl port-forward -n harbortasks svc/api 3000:3000
curl http://localhost:3000/api/tasks      # en otra terminal
kubectl port-forward -n harbortasks svc/web 8080:80

# 5. Limpieza
kind delete cluster --name curso
```

> Con Ingress (lección M7-04 / M9-03) se sustituyen los `port-forward` por el Ingress controller de kind y el host `harbor.test`.

---

## Opción 3 · Helm (despliegue recomendado)

```bash
# El chart empaqueta namespace + deployment + service + configmap + secret + pvc + ingress
helm lint helm/harbortasks
helm install harbortasks helm/harbortasks --namespace harbortasks --create-namespace

# Verificación
helm list -n harbortasks
kubectl get pods -n harbortasks
kubectl port-forward -n harbortasks svc/api 3000:3000

# Actualizar con otros valores (réplicas, tag, host)
helm upgrade harbortasks helm/harbortasks --set api.replicaCount=3

# Desinstalar
helm uninstall harbortasks -n harbortasks
```

---

## Estructura

```text
harbortasks/
├── README.md
├── compose.yaml                 # stack local (web + api + postgres)
├── api/
│   ├── Dockerfile               # multi-stage Node (deps → build → runtime no-root)
│   ├── package.json
│   └── src/index.js             # API Express con CRUD + /healthz
├── web/
│   ├── Dockerfile               # nginx que sirve el SPA
│   ├── nginx.conf               # proxy /api → api:3000, SPA en /
│   └── index.html               # SPA de ejemplo
├── k8s/
│   ├── namespace.yaml
│   ├── secrets.yaml             # Secret compartido (database-url + postgres-password)
│   ├── api/                     # deployment, service, configmap, hpa
│   ├── web/                     # deployment, service, configmap (nginx proxy)
│   └── postgres/                # deployment, service, pvc, configmap (init.sql)
└── helm/harbortasks/
    ├── Chart.yaml
    ├── values.yaml
    └── templates/               # namespace, serviceaccount, deployment-api,
                                 # deployment-web, deployment-postgres,
                                 # postgres-pvc, postgres-configmap, service-*.yaml,
                                 # configmap, secret, ingress, hpa, _helpers.tpl
```

> ⚠️ Las credenciales del `Secret` están en Base64 como ejemplo didáctico. En producción: gestores externos (Sealed Secrets, External Secrets, KMS) y rotación.