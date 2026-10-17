# Aprende Cloud — instrucciones para agentes

Este `AGENTS.md` es un **puntero corto**. La fuente de verdad del curso es **`spec.md`** (mismo directorio). Léelo entero antes de tocar nada.

Las skills están **centralizadas en la raíz de la Academy** (`Academy/.agents/skills/`). Desde este directorio se referencian con `../.agents/skills/`.

## Orden de lectura obligatorio

1. `spec.md` — spec completa del curso (objetivo, temario, proyecto Copiloto en la nube, diseño, checklist).
2. `../.agents/skills/academy-course/SKILL.md` — **cómo** se construye una web de Academy (mapa, anatomía HTML, SITE_BASE, verificación).
3. `../.agents/skills/documentation/SKILL.md` — consultar docs oficiales (Azure, AWS, Terraform) vía Context7 antes de afirmar recursos, atributos o versiones.
4. `../.agents/skills/frontend-design/SKILL.md` — identidad visual propia (prohibidos los defaults genéricos de IA).

Referencia de implementación para copiar (chrome, estilos, scripts): `Aprende Docker/` (más reciente), `Aprende DevOps/`.

## Reglas

- No contradigas `spec.md`. Si la spec y una skill chocan, gana la spec y lo señalas.
- No reescribas la spec al implementar; implementa. Solo corrección puntual con justificación.
- Lecciones en HTML hecho a mano (sin Markdown, sin build). `course-data.js` es la única fuente de verdad de navegación.
- Contenido, comentarios y UI en español; identificadores y comandos en inglés.
- Todo **como código** (Terraform); **sin secretos** en el repo (identidad gestionada/OIDC).
- Claves de localStorage del curso: `cloud-theme`, `cloud-progress`.
- Verifica siempre antes de dar por hecho: `node --check` de los JS, chequeo estructural de las 32 lecciones (ids, data-attributes, SITE_BASE, `cloud-theme`), servidor estático con HTTP 200, tarjeta en `Academy/index.html`.
- El proyecto `proyectos/infra-cloud/` debe ser Terraform válido y revisable.
