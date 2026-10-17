# Aprende System Design — instrucciones para agentes

Este `AGENTS.md` es un **puntero corto**. La fuente de verdad del curso es **`spec.md`** (mismo directorio). Léelo entero antes de tocar nada.

Las skills están **centralizadas en la raíz de la Academy** (`Academy/.agents/skills/`). Desde este directorio se referencian con `../.agents/skills/`.

## Orden de lectura obligatorio

1. `spec.md` — spec completa del curso (objetivo, temario, el cuaderno de arquitectura, diseño, checklist).
2. `../.agents/skills/academy-course/SKILL.md` — **cómo** se construye una web de Academy (mapa, anatomía HTML, SITE_BASE, verificación).
3. `../.agents/skills/academy-verify/SKILL.md` — **verificar** el curso (script `verify.mjs`).
4. `../.agents/skills/academy-course-add/SKILL.md` — playbook por si se amplía el temario.
5. `../.agents/skills/frontend-design/SKILL.md` — identidad visual propia.

Referencia de implementación para copiar (chrome, estilos, scripts): `Aprende Docker/` (más reciente), `Aprende Cloud/`.

## Reglas

- No contradigas `spec.md`. Si la spec y una skill chocan, gana la spec y lo señalas.
- No reescribas la spec al implementar; implementa. Solo corrección puntual con justificación.
- Lecciones en HTML hecho a mano (sin Markdown, sin build). `course-data.js` es la única fuente de verdad de navegación.
- Contenido, comentarios y UI en español; identificadores y terminología técnica en inglés.
- Todo diseño parte de **requisitos + supuestos** y termina en **trade-offs**; las estimaciones llevan números y unidades.
- Claves de localStorage del curso: `sysd-theme`, `sysd-progress`.
- Verifica siempre antes de dar por hecho: `node .agents/skills/academy-verify/scripts/verify.mjs "Aprende System Design"`, `node --check` de los JS, HTTP 200, tarjeta en `Academy/index.html`.
- El proyecto `proyectos/case-studies/` son **documentos de diseño** (Markdown + diagramas ASCII), no código.
