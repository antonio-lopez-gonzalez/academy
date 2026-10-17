# Aprende DevOps — instrucciones para agentes

Este `AGENTS.md` es un **puntero corto**. La fuente de verdad del curso es **`spec.md`** (mismo directorio). Léelo entero antes de tocar nada.

Las skills están **centralizadas en la raíz de la Academy** (`Academy/.agents/skills/`). Desde este directorio se referencian con `../.agents/skills/`.

## Orden de lectura obligatorio

1. `spec.md` — spec completa del curso (objetivo, temario, proyecto Publicar el Copiloto, diseño, checklist).
2. `../.agents/skills/academy-course/SKILL.md` — **cómo** se construye una web de Academy (mapa, anatomía HTML, SITE_BASE, verificación).
3. `../.agents/skills/documentation/SKILL.md` — consultar docs oficiales (GitHub Actions, Docker/Buildx, Terraform/OpenTofu, GHCR) vía Context7 antes de afirmar sintaxis o versiones.
4. `../.agents/skills/frontend-design/SKILL.md` — identidad visual propia (prohibidos los defaults genéricos de IA).

Referencia de implementación para copiar (chrome, estilos, scripts): `Aprende Docker/` (más reciente), `Aprende IA/`/`Aprende FastAPI/`.

## Reglas

- No contradigas `spec.md`. Si la spec y una skill chocan, gana la spec y lo señalas.
- No reescribas la spec al implementar; implementa. Solo corrección puntual con justificación.
- Lecciones en HTML hecho a mano (sin Markdown, sin build). `course-data.js` es la única fuente de verdad de navegación.
- Contenido, comentarios y UI en español; identificadores y comandos en inglés.
- Todo **como código** (workflows, IaC); los **secretos** nunca en el repositorio.
- Claves de localStorage del curso: `devops-theme`, `devops-progress`.
- Verifica siempre antes de dar por hecho: `node --check` de los JS, chequeo estructural de las 32 lecciones (ids, data-attributes, SITE_BASE, `devops-theme`), servidor estático con HTTP 200, tarjeta en `Academy/index.html`.
- El proyecto `proyectos/pipeline/` describe CI/CD real (GitHub Actions) y debe ser coherente y revisable.
