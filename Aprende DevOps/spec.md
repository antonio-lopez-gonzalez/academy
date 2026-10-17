# Curso Completo de DevOps y CI/CD

## Del commit al despliegue — pipelines que publican de verdad

---

# 1. Objetivo

Actúa como un ingeniero senior de plataforma/DevOps con años llevando software a producción. El objetivo es crear **un curso completo de CI/CD y DevOps**, pensado para un **desarrollador que ya programa** (y sabe Docker/K8s por el curso `Aprende Docker`) y quiere dominar el ciclo de entrega: de un `git push` a un **pipeline que prueba, construye, despliega y observa** — de forma reproducible y segura.

El curso es didáctico y orientado al **por qué**:

- **cómo funciona** la integración y la entrega continua por dentro (jobs, runners, artefactos, entornos);
- **qué problema resuelve** cada pieza (CI, artefactos inmutables, gates, IaC, estrategias de despliegue);
- **cómo se escribe un pipeline** mantenible (reutilización, caché, matrices, secretos);
- **cómo se despliega con seguridad** (blue/green, canary, rollback, migraciones) y se controla la **cadena de suministro**.

El hilo conductor es **Publicar el Copiloto**: llevar la API del curso `Aprende FastAPI` del repositorio a producción con un pipeline completo.

---

# 2. Audiencia y requisitos previos

- **Saber programar** y usar Git. Se recomienda haber hecho `Aprende Docker` (contenedores/K8s).
- Ganas de tocar la terminal y GitHub. No se asume experiencia previa en CI/CD.
- GitHub (Actions) como plataforma principal; se comparan GitLab CI y Azure DevOps.

---

# 3. Hilo conductor: Publicar el Copiloto

**Publicar el Copiloto** — el pipeline de la `Copiloto API` (curso `Aprende FastAPI`):

1. **CI**: lint + tests + cobertura en cada push/PR.
2. **Build**: imagen Docker multi-stage, etiquetada e inmutable; SBOM.
3. **CD**: despliegue a *staging* automático y a *producción* tras aprobación.
4. **Estrategias**: blue/green y canary con rollback.
5. **IaC**: el entorno se define como código (Terraform) y se versiona.
6. **Observabilidad**: el pipeline emite métricas de duración/éxito.

Evolución: Módulos 0-4 (CI y artefactos), 5 (estrategias), 6 (IaC), 7 (proyecto end-to-end).

---

# 4. Tecnologías

## Utilizar SIEMPRE

- **Git** + **GitHub** (Actions, Environments, Packages).
- **GitHub Actions** (workflows YAML, reusable workflows, matrices, caché, artifacts).
- **Docker** y **GitHub Container Registry** (GHCR).
- **Terraform / OpenTofu** para IaC.
- **`act`** (ejecutar Actions en local) y **`gh`** CLI.
- **Trunk-based development** y **Conventional Commits** + SemVer.

## No utilizar

- Pipelines en la interfaz gráfica como única vía (todo como **código**).
- Secretos en el repositorio o en texto plano en los workflows.
- Deploys manuales "por SSH y a rezar" (todo reproducible).
- Un proveedor cloud concreto como requisito (se ilustra con ejemplos neutros/Terraform).

---

# 5. Forma de enseñar

Cada lección sigue EXACTAMENTE esta estructura.

## 1. Introducción
Qué, por qué existe, qué problema resuelve, cuándo se usa, errores típicos.

## 2. Teoría
Con detalle, sin asumir CI/CD previo. Comparaciones del mundo real (una cadena de montaje; un control de calidad). Diagramas ASCII (commit → CI → artefacto → entorno). Explicar el **por qué**.

## 3. Ejemplo mínimo
El workflow / comando más pequeño posible, explicado línea por línea.

## 4. Ejemplo real
Aplicado a **Publicar el Copiloto** o a un escenario profesional.

## 5. Buenas prácticas
Qué hacer, qué evitar, por qué. Tabla cuando aplique.

## 6. Errores habituales
Ejemplos incorrectos, por qué están mal, versión correcta.

## 7. Ejercicio guiado
Paso a paso, resuelto al detalle, con resultado esperado.

## 8. Reto
Reto adicional **sin resolver** (solo pista).

## 9. Resumen
Bullets + conceptos clave. Tabla "concepto → YAML" cuando aplique.

## 10. Preguntas frecuentes
FAQ (3-5 `<details>`).

## Reglas de contenido

- Todo **como código** (pipelines, infra, config) y versionado.
- Los **secretos** fuera del repo (Secrets/Vault/OIDC).
- Artefactos **inmutables** y trazables (el mismo que se prueba es el que se despliega).
- Migraciones y despliegues **seguros** (compatibles hacia atrás, reversibles).

---

# 6. Multiplataforma: Linux / macOS / Windows

Los runners suelen ser Linux. Shell canónico: **bash**, con callouts de **PowerShell** cuando el alumno ejecute en local. GitHub Actions/`gh`/`act` son multiplataforma.

---

# 7. Índice del curso

## Módulo 0 — Preparación
- **01-cicd-y-cultura.html** — Qué es CI/CD y la cultura DevOps (por qué "romper el silencio").
- **02-proyecto-conductor.html** — Publicar el Copiloto: qué construiremos de punta a punta.
- **03-entorno-gh-act.html** — Entorno: Git, `gh`, `act`, Docker local; primer workflow.
- **04-anatomia-workflow.html** — Anatomía de un workflow de GitHub Actions (triggers, jobs, steps, runners).

## Módulo 1 — Git, ramas y versiones
- **01-estrategias-de-ramas.html** — GitFlow vs trunk-based; por qué CI favorece ramas cortas.
- **02-pull-requests-y-review.html** — PRs, revisión, checks obligatorios y merge.
- **03-versionado-semantico.html** — SemVer y su relación con CI/CD.
- **04-conventional-commits-changelog.html** — Conventional Commits y changelog automático.

## Módulo 2 — Integración continua
- **01-pipeline-ci.html** — El pipeline de CI: eventos, jobs y dependencias.
- **02-lint-test-build.html** — Lint + tests + build en CI (rápido y fiable).
- **03-matrices-y-cache.html** — Matrices (varias versiones/SO) y caché de dependencias.
- **04-artefactos-y-cobertura.html** — Publicar cobertura y artefactos; gates de calidad.

## Módulo 3 — Build y artefactos
- **01-docker-en-ci.html** — Construir la imagen Docker en CI (multi-stage, Buildx).
- **02-registries-y-tagging.html** — Publicar en un registry (GHCR) y etiquetar inmutablemente.
- **03-sbom-y-provenance.html** — SBOM y procedencia del artefacto (supply chain).
- **04-cache-de-builds.html** — Caché de capas de Docker en CI (rápido y reproducible).

## Módulo 4 — Entrega continua
- **01-entornos-y-config.html** — Entornos (staging/prod) y configuración por entorno.
- **02-secretos-y-variables.html** — Secrets, variables y OIDC (sin claves de larga vida).
- **03-gates-y-aprobaciones.html** — Puertas de aprobación, protección de entorno y promoción.
- **04-migraciones-en-cd.html** — Migraciones de BD en CD (compatibles, reversibles).

## Módulo 5 — Estrategias de despliegue
- **01-rolling.html** — Despliegue rolling y health checks.
- **02-blue-green.html** — Blue/green: cambiar tráfico sin downtime.
- **03-canary-y-feature-flags.html** — Canary + feature flags (desplegar ≠ publicar).
- **04-rollback.html** — Rollback seguro y qué lo hace posible.

## Módulo 6 — Infraestructura como código
- **01-que-es-iac.html** — IaC: por qué la infra se versiona como el código.
- **02-terraform-basico.html** — Terraform/OpenTofu: providers, recursos, plan/apply.
- **03-estado-y-modulos.html** — Estado remoto y módulos reutilizables.
- **04-entornos-con-iac.html** — Entornos con IaC (workspaces/var-files) y CI para IaC.

## Módulo 7 — Proyecto final: el pipeline del Copiloto
- **01-pipeline-completo.html** — CI → build → CD end-to-end. *(project)*
- **02-cd-y-entornos.html** — Staging automático y producción con aprobación. *(project)*
- **03-observabilidad-pipeline.html** — Métricas y avisos del pipeline. *(project)*
- **04-seguridad-supply-chain.html** — Seguridad de la cadena de suministro (pinning, scanning, permisos). *(project)*

## Recursos
- **cheatsheet.html** — Triggers, sintaxis de Actions, `gh`, `act`, comandos Terraform.
- **glosario.html** — CI, CD, runner, job, artefacto, gate, canary, IaC, SBOM, OIDC…
- **faq.html** — FAQ general del curso.

**Total: 8 módulos (0-7), 32 lecciones + 3 recursos.**

---

# 8. Estructura de repositorio

```text
Aprende DevOps/
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
└── proyectos/pipeline/          ← CI/CD real de la Copiloto API
    ├── .github/workflows/{ci,publish,deploy}.yml
    ├── Dockerfile
    ├── infra/                   ← Terraform
    └── README.md
```

---

# 9. Formato de entrega

Web estática (HTML + CSS + JS vanilla, sin Markdown ni build). Chrome por `site.js` desde `course-data.js`. Proyecto: YAML/HCL/scripts reales y revisables.

- `SITE_BASE` = `'../../'` en lecciones, `'./'` en portada, `'../'` en `web/recursos/`.
- Claves de localStorage: `devops-theme`, `devops-progress`.
- Bloques: `callout`, `challenge`, `faq`, `table`, `ol.steps`, `pre > code.language-*` (`yaml`, `bash`, `dockerfile`, `hcl`).

---

# 10. Diseño y marca

- Tema oscuro por defecto, toggle a claro.
- Paleta propia: **naranja** (`#f97316`) con acento ámbar (`#fbbf24`) sobre fondo profundo.
- Firma de portada: un **pipeline que se ejecuta** (jobs iluminándose: lint → test → build → deploy).
- Identificadores en inglés; contenido, comentarios y UI en español.

---

# 11. Convenciones del curso

1. Idioma: contenido, comentarios y UI en español; identificadores/comandos en inglés.
2. Todo como código (workflows, infra), versionado.
3. Proyecto conductor: `proyectos/pipeline/`.
4. Cada lección usa SIEMPRE las 10 secciones.
5. Retos sin resolver en el HTML (solo pista).

---

# 12. Documentación actualizada

Consultar la documentación oficial (GitHub Actions, Docker/Buildx, Terraform/OpenTofu, GHCR) vía Context7 antes de afirmar sintaxis, actions o versiones. Preferir la versión estable más reciente.

---

# 13. Página principal (`web/index.html`)

Portada con hero, firma (pipeline que se ejecuta), estadísticas e `<div id="module-list">` alimentado por `course-data.js`.

---

# 14. Checklist de implementación

## Fase 0 — Spec y AGENTS ✅
- [x] `spec.md` (este documento).
- [x] `AGENTS.md` (puntero corto). Skills centralizadas en `Academy/.agents/skills/`.

## Fase 1 — Scaffold ✅
- [x] `package.json`, `pnpm-workspace.yaml`, `.gitignore`, `.gitattributes` (LF), `scripts/serve-web.mjs`.
- [x] Assets re-tematizados (naranja) y `course-data.js` completo.
- [x] Portada `web/index.html` + firma.

## Fase 2 — Lecciones (web/modulos/) ✅
- [x] 00-preparacion … 07-proyecto-final (32 lecciones).

## Fase 3 — Recursos ✅
- [x] `web/recursos/{cheatsheet,glosario,faq}.html`.

## Fase 4 — Proyecto pipeline (CI/CD real) ✅
- [x] `proyectos/pipeline/` con `.github/workflows`, Dockerfile e IaC.

## Fase 5 — Academy ✅
- [x] Tarjeta en `Academy/index.html` + contador + fila en `specs.md` §3 y `README.md`.

---

# 15. Reglas importantes

1. Nunca des por sentado un concepto: CI, artefacto y entorno se explican desde cero.
2. Explica siempre el "por qué" (por qué ramas cortas, por qué artefactos inmutables).
3. Todo pipeline debe poder ejecutarse (y probarse con `act` en local).
4. Los secretos nunca en el repositorio.
5. El artefacto que se prueba es el que se despliega (trazabilidad).
6. Sigue las buenas prácticas actuales de GitHub Actions y DevOps.
7. No omitas pasos de configuración.
8. Si un concepto depende de otro, enlaza la lección correspondiente.
9. Prioriza la comprensión profunda frente a la velocidad.
10. No conviertas el curso en un tutorial exclusivo de una plataforma.
