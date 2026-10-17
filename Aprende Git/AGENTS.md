# Aprende Git — instrucciones para agentes

Este `AGENTS.md` es un **puntero corto**. La fuente de verdad del curso es **`spec.md`**. Léelo entero antes de tocar nada.

Las skills están **centralizadas en la raíz de la Academy** (`Academy/.agents/skills/`). Desde este directorio: `../.agents/skills/`.

## Orden de lectura obligatorio

1. `spec.md` — spec del curso (modelo, índice, laboratorio).
2. `../.agents/skills/academy-course/SKILL.md` — mecánica de la web.
3. `../.agents/skills/academy-verify/SKILL.md` — verificación.
4. `../.agents/skills/documentation/SKILL.md` — contrastar flags con la documentación de Git antes de afirmarlos.
5. `../.agents/skills/frontend-design/SKILL.md` — identidad visual propia.

Referencia de chrome (estilos/scripts): `Aprende System Design/`, `Aprende Docker/`.

## Reglas

- No contradigas `spec.md`. Si choca con una skill, gana la spec.
- Lecciones en HTML hecho a mano. `course-data.js` es la única fuente de navegación.
- Contenido y UI en español; comandos e identificadores en inglés.
- Comandos modernos: `git switch` y `git restore` (Git 2.23+). No enseñes `git push --force` a `main`.
- Claves de localStorage: `git-theme`, `git-progress`.
- Verifica: `node .agents/skills/academy-verify/scripts/verify.mjs "Aprende Git"`, `node --check`, HTTP 200, tarjeta en `Academy/index.html`.
- El proyecto `proyectos/laboratorio/` se ejecuta con `node --test` y no necesita red.
