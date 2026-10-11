# Learn FastAPI — instrucciones para agentes

Este `AGENTS.md` es un **puntero corto**. La fuente de verdad del curso es **`spec.md`** (mismo directorio). Léelo entero antes de tocar nada.

Las skills están **centralizadas en la raíz de la Academy** (`Academy/.agents/skills/`). Desde este directorio se referencian con `../.agents/skills/`.

## Orden de lectura obligatorio

1. `spec.md` — spec completa del curso (objetivo, temario, proyecto Copiloto API, diseño, checklist).
2. `../.agents/skills/academy-course/SKILL.md` — **cómo** se construye una web de Academy (mapa, anatomía HTML, SITE_BASE, verificación).
3. `../.agents/skills/documentation/SKILL.md` — consultar docs oficiales (FastAPI, Pydantic v2, SQLModel, Alembic, uvicorn) vía Context7 antes de afirmar APIs o versiones.
4. `../.agents/skills/frontend-design/SKILL.md` — identidad visual propia (prohibidos los defaults genéricos de IA).

Referencia de implementación para copiar (chrome, estilos, scripts): `Learn Docker/` (más reciente), `Learn Python/`/`Learn IA/` (mismo runtime).

## Reglas

- No contradigas `spec.md`. Si la spec y una skill chocan, gana la spec y lo señalas.
- No reescribas la spec al implementar; implementa. Solo corrección puntual con justificación.
- Lecciones en HTML hecho a mano (sin Markdown, sin build). `course-data.js` es la única fuente de verdad de navegación.
- Contenido, comentarios y UI en español; identificadores y comandos en inglés.
- Python 3.12+ con `uv`. Separa **sync vs async** con criterio.
- Nada de claves ni secretos en el repositorio (`.env` / variables de entorno).
- Claves de localStorage del curso: `fapi-theme`, `fapi-progress`.
- Verifica siempre antes de dar por hecho: `node --check` de los JS, chequeo estructural de las 32 lecciones (ids, data-attributes, SITE_BASE, `fapi-theme`), servidor estático con HTTP 200, tarjeta en `Academy/index.html`.
- El proyecto `proyectos/copiloto-api/` debe ejecutarse con Python 3.12+ de verdad (`uv run`).
