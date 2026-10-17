# Aprende SDD — instrucciones para agentes

Este `AGENTS.md` es un **puntero corto**. La fuente de verdad del curso es **`spec.md`** (mismo directorio). Léelo entero antes de tocar nada.

## Orden de lectura obligatorio

Las skills ya no viven en el curso: están **centralizadas en la raíz de la Academy** (`Academy/.agents/skills/`). Desde este directorio se referencian con `../.agents/skills/`.

1. `spec.md` — spec completa del curso (contenido, índice, diseño, checklist).
2. `../.agents/skills/academy-course/SKILL.md` — **cómo** se construye una web de Academy (contrato mecánico: mapa, anatomía HTML, SITE_BASE, verificación).
3. `../.agents/skills/spec-driven-development/SKILL.md` — **qué** enseñar (vocabulario SDD, ciclo, plantillas, anti-patrones).
4. `../.agents/skills/documentation/SKILL.md` — consultar docs oficiales (Spec Kit, JSON Schema, OpenAPI) vía Context7 antes de afirmar comandos o versiones.
5. `../.agents/skills/frontend-design/SKILL.md` — identidad visual propia (prohibidos los defaults genéricos de IA).

Referencia de implementación para copiar (chrome, estilos, scripts): `Aprende Docker/` (más reciente), alternativa `Aprende Javascript/`.

## Reglas

- No contradigas `spec.md`. Si la spec y una skill chocan, gana la spec y lo señalas.
- No reescribas la spec al implementar; implementa. Solo corrección puntual con justificación.
- Lecciones en HTML hecho a mano (sin Markdown, sin build). `course-data.js` es la única fuente de verdad de navegación.
- Contenido, comentarios y UI en español; identificadores y comandos en inglés.
- Verifica siempre antes de dar por hecho: `node --check` de los JS, chequeo estructural de las 43→32 lecciones (ids, data-attributes, SITE_BASE, `sdd-theme`), servidor estático con HTTP 200, tarjeta en `Academy/index.html`.
- El proyecto `proyectos/lumen/` debe ejecutarse con `node` de verdad.
