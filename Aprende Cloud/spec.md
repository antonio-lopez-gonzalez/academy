# Curso Completo de Cloud (Azure / AWS)

## Despliega en la nube de verdad — de la región al Copiloto en producción

---

# 1. Objetivo

Actúa como un ingeniero de cloud/plataforma con años llevando aplicaciones a producción en Azure y AWS. El objetivo es crear **un curso completo de Cloud**, pensado para un **desarrollador que ya programa** (y sabe Docker/K8s e IaC por `Aprende Docker` y `Aprende DevOps`) y quiere dominar el despliegue en la nube: de "¿qué es una región?" a un **Copiloto funcionando en producción** con contenedores gestionados, base de datos, secretos, observabilidad y control de coste.

El curso es didáctico y orientado al **por qué**:

- **cómo funciona** la nube por dentro a nivel práctico (regiones, red, identidad, cómputo, almacenamiento);
- **qué problema resuelve** cada servicio (y cuándo elegir uno u otro);
- **cómo se asegura** un despliegue (IAM, secretos, red, sin claves de larga vida);
- **cómo se opera** de verdad (observabilidad, alta disponibilidad, coste).

El hilo conductor es **Copiloto en la nube**: desplegar la `Copiloto API` (curso `Aprende FastAPI`) con Terraform, contenedores gestionados y buenas prácticas.

> Curso **provider-neutral** en el discurso (Azure como principal, AWS en comparación) y **concreto** en el código (Terraform).

---

# 2. Audiencia y requisitos previos

- **Saber programar** y haber hecho (idealmente) `Aprende Docker` y `Aprende DevOps`.
- Ganas de tocar la terminal y Terraform. No se asume experiencia previa en la nube.
- Una cuenta de cloud (capa gratuita basta); nada de secretos en el repositorio.

---

# 3. Hilo conductor: Copiloto en la nube

1. **Fundamentos**: cuenta, regiones, coste y CLI.
2. **Red y cómputo**: VNet/VPC, registros de contenedores y cómputo.
3. **Identidad y secretos**: IAM, Key Vault/Secrets Manager, red segura, OIDC.
4. **Datos**: almacenamiento de objetos, base de datos gestionada, caché/cola.
5. **Contenedores gestionados**: App Service / Container Apps (Azure) y AKS/EKS.
6. **Serverless**: Functions y eventos.
7. **Operación**: logs/métricas/trazas, coste y DR.
8. **Proyecto final**: el Copiloto desplegado con Terraform y CI/CD.

---

# 4. Tecnologías

## Utilizar SIEMPRE

- **Terraform / OpenTofu** (provider `azurerm`, con equivalentes `aws`).
- **Azure CLI (`az`)** y **AWS CLI (`aws`)**.
- **Azure**: Resource Groups, VNet, ACR, Container Apps / App Service, Key Vault, Azure Database for PostgreSQL, Azure Monitor, Entra ID.
- **AWS** (comparación): VPC, ECR, App Runner/ECS, Secrets Manager, RDS, CloudWatch, IAM.
- **GitHub OIDC** para desplegar sin claves de larga vida.
- **Docker** para empaquetar.

## No utilizar

- Consolas web como única vía (todo como **código**).
- Secretos en el repositorio o en la imagen.
- Recursos sin etiquetas de coste ni límites de gasto.
- Un único proveedor como dogma (se comparan Azure y AWS).

---

# 5. Forma de enseñar

Cada lección sigue EXACTAMENTE esta estructura.

## 1. Introducción
Qué, por qué existe, qué problema resuelve, cuándo se usa, errores típicos.

## 2. Teoría
Con detalle, sin asumir nube. Comparaciones (la nube como un edificio con plantas y conserje). Diagramas ASCII (región → VNet → subred → recurso). Explicar el **por qué**.

## 3. Ejemplo mínimo
El recurso / comando más pequeño posible, explicado línea a línea.

## 4. Ejemplo real
Aplicado a **Copiloto en la nube** o a un escenario profesional.

## 5. Buenas prácticas
Qué hacer, qué evitar, por qué. Tabla cuando aplique.

## 6. Errores habituales
Incorrecto, por qué, versión correcta.

## 7. Ejercicio guiado
Paso a paso, resuelto, con resultado esperado.

## 8. Reto
Reto adicional **sin resolver** (solo pista).

## 9. Resumen
Bullets + conceptos clave. Tabla "concepto → recurso Azure/AWS" cuando aplique.

## 10. Preguntas frecuentes
FAQ (3-5 `<details>`).

## Reglas de contenido

- Todo **como código** (Terraform) y versionado; la consola solo para explorar.
- **Sin secretos** en el repo; identidad gestionada / OIDC.
- **Coste** siempre presente (etiquetas, límites, apagado de lo que no se usa).
- Seguridad por defecto (red privada, mínimo privilegio).

---

# 6. Multiplataforma: Linux / macOS / Windows

Las CLIs (`az`, `aws`, `terraform`) son multiplataforma. Shell canónico: **bash**, con callouts de **PowerShell**.

---

# 7. Índice del curso

## Módulo 0 — Preparación
- **01-que-es-la-nube.html** — Qué es la nube, regiones/zonas, y por qué "el datacenter de otro".
- **02-modelos-de-servicio.html** — IaaS/PaaS/SaaS/PaaS de contenedores: qué gestionas tú y qué no.
- **03-proveedores-y-cuenta.html** — Azure vs AWS: cuenta, suscripción, organización y CLI.
- **04-regiones-y-coste.html** — Regiones, latencia, y lo primero: entender y controlar el coste.

## Módulo 1 — Red y cómputo
- **01-redes-vnet-vpc.html** — VNet/VPC, subredes, grupos de seguridad y por qué importa la red.
- **02-computo.html** — VM vs contenedor vs serverless: el espectro de cómputo y cuándo cada uno.
- **03-registros-de-imagenes.html** — Registros (ACR/ECR), autenticación y por qué un registro gestionado.
- **04-eligiendo-computo.html** — Cómo elegir cómputo (decisión con criterio) y su impacto en coste/operación.

## Módulo 2 — Identidad y seguridad
- **01-iam.html** — IAM / Entra ID: identidades, roles y mínimo privilegio.
- **02-secretos.html** — Key Vault / Secrets Manager e identidad gestionada (sin secretos en el repo).
- **03-red-segura.html** — Red privada, puntos de servicio/endpoints, firewall y exposición mínima.
- **04-oidc-sin-claves.html** — OIDC: desplegar desde CI/CD sin claves de larga vida.

## Módulo 3 — Almacenamiento y datos
- **01-almacenamiento-objeto.html** — Blob/S3: objetos, clases de acceso y casos de uso.
- **02-bases-de-datos.html** — Bases gestionadas (PostgreSQL/MySQL/RDS): qué te quitan de encima.
- **03-cache-y-cola.html** — Caché gestionada y colas/eventos (y su relación con Redis/Kafka).
- **04-backups-y-retencion.html** — Copias de seguridad, retención y recuperación.

## Módulo 4 — Contenedores gestionados
- **01-container-apps.html** — Azure Container Apps / App Runner: contenedores sin gestionar clúster.
- **02-kubernetes-gestionado.html** — AKS/EKS: Kubernetes gestionado y qué NO es gratis.
- **03-escalado.html** — Escalado automático (horizontal) y sus reglas.
- **04-desplegar-el-copiloto.html** — Desplegar Copiloto API en contenedores gestionados.

## Módulo 5 — Serverless y eventos
- **01-functions.html** — Funciones serverless (Azure Functions / Lambda).
- **02-eventos-y-colas.html** — Eventos/colas gestionadas y arquitecturas desacopladas.
- **03-api-gateway.html** — API Management / API Gateway como puerta de entrada.
- **04-cuando-serverless.html** — Cuándo serverless y cuándo no (límites, cold start, coste).

## Módulo 6 — Observabilidad y operación
- **01-logs-y-metricas.html** — Logs y métricas del cloud (Azure Monitor / CloudWatch).
- **02-trazas-y-apm.html** — Trazas distribuidas y APM (enlace con `Aprende NewRelic`).
- **03-coste-y-presupuesto.html** — Presupuestos, alertas de gasto y optimización de coste.
- **04-alta-disponibilidad-y-dr.html** — Zonas, réplicas y recuperación ante desastres.

## Módulo 7 — Proyecto final: Copiloto en la nube
- **01-arquitectura-cloud.html** — Arquitectura final en Azure (con alternativa AWS). *(project)*
- **02-terraform-azure.html** — Terraform que crea toda la infraestructura. *(project)*
- **03-despliegue-y-cicd.html** — Despliegue con GitHub Actions + OIDC. *(project)*
- **04-coste-y-observabilidad.html** — Coste, alertas y observabilidad en producción. *(project)*

## Recursos
- **cheatsheet.html** — Comandos `az`/`aws`/`terraform` y tabla de recursos Azure↔AWS.
- **glosario.html** — Región, zona, VNet, IAM, ACR, PaaS, serverless, OIDC, DR…
- **faq.html** — FAQ general del curso.

**Total: 8 módulos (0-7), 32 lecciones + 3 recursos.**

---

# 8. Estructura de repositorio

```text
Aprende Cloud/
├── spec.md                      ← ★ ESTE DOCUMENTO (fuente de verdad del curso)
├── AGENTS.md                    ← puntero corto a spec.md + reglas para agentes
├── package.json / pnpm-workspace.yaml
├── .gitignore / .gitattributes
├── web/                         ← ★ EL CURSO (web estática, sin build)
│   ├── index.html
│   ├── assets/{css,js,fonts,favicon.svg}
│   ├── modulos/00-preparacion/ … 07-proyecto-final/
│   └── recursos/
├── scripts/serve-web.mjs
└── proyectos/infra-cloud/       ← Terraform de la infraestructura en Azure
    ├── versions.tf / main.tf / variables.tf / outputs.tf
    ├── .github/workflows/deploy.yml
    └── README.md
```

---

# 9. Formato de entrega

Web estática (HTML + CSS + JS vanilla, sin Markdown ni build). Chrome por `site.js` desde `course-data.js`. Proyecto: HCL/YAML reales y revisables.

- `SITE_BASE` = `'../../'` en lecciones, `'./'` en portada, `'../'` en `web/recursos/`.
- Claves de localStorage: `cloud-theme`, `cloud-progress`.
- Bloques: `callout`, `challenge`, `faq`, `table`, `ol.steps`, `pre > code.language-*` (`hcl`, `bash`, `yaml`, `json`).

---

# 10. Diseño y marca

- Tema oscuro por defecto, toggle a claro.
- Paleta propia: **azul cielo** (`#38bdf8`) con acento violeta (`#a78bfa`) sobre fondo profundo.
- Firma de portada: un **despliegue en la nube** que se ejecuta solo (`terraform apply`, recursos creándose).
- Identificadores en inglés; contenido, comentarios y UI en español.

---

# 11. Convenciones del curso

1. Idioma: contenido, comentarios y UI en español; identificadores/comandos en inglés.
2. Todo como código (Terraform); la consola solo para explorar.
3. Proyecto conductor: `proyectos/infra-cloud/`.
4. Cada lección usa SIEMPRE las 10 secciones.
5. Retos sin resolver en el HTML (solo pista).

---

# 12. Documentación actualizada

Consultar la documentación oficial (Azure, AWS, Terraform/azurerm/aws providers) vía Context7 antes de afirmar recursos, atributos o versiones. Preferir la versión estable más reciente.

---

# 13. Página principal (`web/index.html`)

Portada con hero, firma (despliegue en la nube), estadísticas e `<div id="module-list">` alimentado por `course-data.js`.

---

# 14. Checklist de implementación

## Fase 0 — Spec y AGENTS ✅
- [x] `spec.md` (este documento).
- [x] `AGENTS.md` (puntero corto). Skills centralizadas en `Academy/.agents/skills/`.

## Fase 1 — Scaffold ✅
- [x] `package.json`, `pnpm-workspace.yaml`, `.gitignore`, `.gitattributes` (LF), `scripts/serve-web.mjs`.
- [x] Assets re-tematizados (azul cielo) y `course-data.js` completo.
- [x] Portada `web/index.html` + firma.

## Fase 2 — Lecciones (web/modulos/) ✅
- [x] 00-preparacion … 07-proyecto-final (32 lecciones).

## Fase 3 — Recursos ✅
- [x] `web/recursos/{cheatsheet,glosario,faq}.html`.

## Fase 4 — Proyecto infra-cloud (Terraform) ✅
- [x] `proyectos/infra-cloud/` con Terraform y workflow de despliegue.

## Fase 5 — Academy ✅
- [x] Tarjeta en `Academy/index.html` + contador + fila en `specs.md` §3 y `README.md`.

---

# 15. Reglas importantes

1. Nunca des por sentado un concepto: región, red e identidad se explican desde cero.
2. Explica siempre el "por qué" y la comparación Azure↔AWS.
3. Todo el código debe ser Terraform válido y revisable.
4. Nunca secretos en el repositorio; identidad gestionada/OIDC.
5. El coste se controla desde el minuto uno (etiquetas, límites, alertas).
6. Sigue las buenas prácticas actuales de Azure/AWS.
7. No omitas pasos de configuración (cuenta, CLI, login).
8. Si un concepto depende de otro, enlaza la lección correspondiente.
9. Prioriza la comprensión profunda frente a la velocidad.
10. No conviertas el curso en un tutorial exclusivo de un proveedor.
