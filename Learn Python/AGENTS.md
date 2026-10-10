# Learn Python — instrucciones para agentes

Este `AGENTS.md` es un **puntero corto**. La fuente de verdad del curso es **`specs.md`** (mismo directorio). Léelo entero antes de tocar nada.

## Orden de lectura obligatorio

Las skills ya no viven en el curso: están **centralizadas en la raíz de la Academy** (`Academy/.agents/skills/`). Desde este directorio se referencian con `../.agents/skills/`.

1. `specs.md` — spec completa del curso (objetivo, temario, diseño, checklist).
2. `../.agents/skills/academy-course/SKILL.md` — **cómo** se construye una web de Academy (mapa, anatomía HTML, SITE_BASE, verificación).
3. `../.agents/skills/python-course/SKILL.md` — **qué** enseñar (postura pedagógica, por módulo, contrato Biblioteca, anti-patrones).
4. `../.agents/skills/documentation/SKILL.md` — consultar documentación oficial (docs.python.org, pytest, uv, ruff) vía Context7 antes de afirmar APIs o versiones.
5. `../.agents/skills/frontend-design/SKILL.md` — identidad visual propia (prohibidos los defaults genéricos de IA).

Referencia de implementación para copiar (chrome, estilos, scripts): `Learn Docker/` (más reciente), alternativa `Learn Javascript/`.

## Reglas

- No contradigas `specs.md`. Si la spec y una skill chocan, gana la spec y lo señalas.
- No reescribas la spec al implementar; implementa. Solo corrección puntual con justificación.
- Lecciones en HTML hecho a mano (sin Markdown, sin build). `course-data.js` es la única fuente de verdad de navegación.
- Contenido, comentarios y UI en español; identificadores y comandos en inglés.
- Python 3.12+, biblioteca estándar primero; código de ejemplo ejecutable y con salida esperada.
- Claves de localStorage del curso: `py-theme`, `py-progress`.
- Verifica siempre antes de dar por hecho: `node --check` de los JS, chequeo estructural de las 45 lecciones (ids, data-attributes, SITE_BASE, `py-theme`), servidor estático con HTTP 200, tarjeta en `Academy/index.html`.
- El proyecto `proyectos/biblioteca/` debe ejecutarse con Python 3.12+ de verdad (`pip`/`uv`).
