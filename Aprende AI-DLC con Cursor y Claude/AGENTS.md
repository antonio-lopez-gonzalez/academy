# Aprende AI-DLC con Cursor y Claude — instrucciones para agentes

Este `AGENTS.md` es un **puntero corto**. La fuente de verdad del curso es **`spec.md`** (mismo directorio). Léelo entero antes de tocar nada.

Las skills están **centralizadas en la raíz de la Academy** (`Academy/.agents/skills/`). Desde este directorio: `../.agents/skills/`.

## Orden de lectura obligatorio

1. `spec.md` — spec del curso (fases, etapas, perfiles, proyecto aidlc-lite, diseño).
2. `../.agents/skills/academy-course/SKILL.md` — mecánica de la web.
3. `../.agents/skills/academy-verify/SKILL.md` — verificación.
4. `../.agents/skills/documentation/SKILL.md` — contrastar AI-DLC, Claude Code y Cursor vía Context7 o el repositorio oficial.
5. `../.agents/skills/frontend-design/SKILL.md` — identidad visual propia.

Referencia de chrome (estilos/scripts): `Aprende RabbitMQ/`, `Aprende MinIO/`.

## Reglas

- No contradigas `spec.md`. Si choca con una skill, gana la spec.
- Lecciones en HTML hecho a mano. `course-data.js` es la única fuente de navegación.
- Contenido y UI en español; comandos e identificadores en inglés.
- Metodología (fases, etapas, gates) y harness (Claude Code, Cursor) se explican por separado.
- Nunca inventes flags, rutas ni subcomandos de `aidlc`: contrasta con la documentación oficial.
- Claves de localStorage: `aidlc-theme`, `aidlc-progress`.
- Verifica: `node .agents/skills/academy-verify/scripts/verify.mjs "Aprende AI-DLC con Cursor y Claude"`, `node --check`, HTTP 200, tarjeta en `Academy/index.html`.
- El proyecto `proyectos/aidlc-lite/` se ejecuta con Node.js y se prueba con `node --test`; no requiere API de IA.
- Icono propio de grafo de etapas en favicon y topbar.
