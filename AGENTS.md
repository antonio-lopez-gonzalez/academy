# Academy — instrucciones para agentes

Este `AGENTS.md` es un **puntero corto**. La fuente de verdad común es **`specs.md`** (raíz del repositorio). Léelo entero antes de tocar nada. El *contenido* de cada curso vive en su propia spec (`Learn <Curso>/specs.md` | `spec.md` | `AGENTS.md`).

## Qué es este repositorio

La **Academy**: cursos independientes en `Learn <Curso>/`, todos construidos como el mismo artefacto (web estática sin build, navegada desde `web/assets/js/course-data.js`). La portada es `index.html` (raíz).

## Orden de lectura obligatorio

1. `specs.md` — spec general unificada: catálogo de cursos, contrato común, verificación y reglas.
2. `.agents/skills/academy-course/SKILL.md` — **cómo** se construye cualquier web de Academy (mapa, anatomía HTML de 10 secciones, SITE_BASE, claves de tema, verificación).
3. `.agents/skills/documentation/SKILL.md` — consultar documentación oficial vía Context7 antes de afirmar APIs, comandos o versiones.
4. `.agents/skills/frontend-design/SKILL.md` — identidad visual propia (prohibidos los defaults genéricos de IA).
5. `.agents/skills/academy-verify/SKILL.md` — **verificar** que un curso cumple el contrato antes de terminarlo (script `verify.mjs`).
6. `.agents/skills/academy-course-add/SKILL.md` — **playbook** para añadir un curso nuevo de punta a punta.
7. La spec del curso que vayas a tocar (`Learn <Curso>/specs.md` | `spec.md` | `AGENTS.md`).
8. Skills de dominio cuando existan: `.agents/skills/python-course/SKILL.md` (Learn Python), `.agents/skills/spec-driven-development/SKILL.md` (Learn SDD).

## Estructura

```text
Academy/
├── specs.md            ← spec general unificada (este contrato)
├── AGENTS.md           ← este archivo
├── opencode.json       ← Context7 MCP (único en la raíz)
├── index.html          ← portada: una tarjeta por curso
├── .gitignore
├── .agents/skills/     ← skills COMPARTIDAS (no duplicar por curso)
└── Learn <Curso>/      ← un curso por carpeta (web/, proyectos/, scripts/, su spec y su AGENTS.md)
```

## Reglas

- No contradigas `specs.md` ni la spec del curso. Si una skill y una spec chocan, **gana la spec** y se señala el conflicto.
- Las skills viven solo en `.agents/skills/`. **No** crear copias dentro de los cursos.
- La web del curso (`web/`) es HTML hecho a mano, **sin Markdown y sin build**. Los proyectos de `proyectos/` sí pueden tener su propio build.
- `web/assets/js/course-data.js` es la única fuente de verdad de navegación; no puede divergir de los archivos en disco (sin huérfanos en ningún sentido).
- Cada lección usa SIEMPRE las 10 secciones (`introduccion`, `teoria`, `ejemplo-minimo`, `ejemplo-real`, `buenas-practicas`, `errores`, `ejercicio`, `reto`, `resumen`, `faq`). El reto nunca se resuelve en el HTML.
- Contenido, comentarios y UI en **español**; identificadores y comandos en **inglés**.
- Claves de `localStorage` (`<clave>-theme`, `<clave>-progress`) y firma de portada: exclusivas de cada curso.
- No reescribas las specs al implementar; implementa. Solo corrección puntual con justificación.
- Verifica siempre antes de dar por hecho: `node --check` de los JS, chequeo estructural, servidor estático con HTTP 200 y tarjeta correcta en `Academy/index.html` (ver `specs.md` §4.6).
- Al añadir o retirar un curso, actualiza la tabla de `specs.md` §3, la tarjeta de `index.html` y el contador del pie.
