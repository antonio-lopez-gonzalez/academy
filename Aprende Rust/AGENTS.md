# Aprende Rust — instrucciones para agentes

Este `AGENTS.md` es un **puntero corto**. La fuente de verdad del curso es **`spec.md`** (mismo directorio). Léelo entero antes de tocar nada.

Las skills están **centralizadas en la raíz de la Academy** (`Academy/.agents/skills/`). Desde este directorio: `../.agents/skills/`.

## Orden de lectura obligatorio

1. `spec.md` — spec del curso (índice de módulos, stack, reglas, proyecto urlshort).
2. `../.agents/skills/academy-course/SKILL.md` — mecánica de la web (anatomía de lección, SITE_BASE, claves).
3. `../.agents/skills/academy-verify/SKILL.md` — verificación.
4. `../.agents/skills/documentation/SKILL.md` — contrastar APIs de Rust, `clap`, `tokio`, `axum` vía Context7.
5. `../.agents/skills/frontend-design/SKILL.md` — identidad visual propia.

Referencia de chrome (estilos/scripts): `Aprende MinIO/`, `Aprende Git/`.

## Reglas

- No contradigas `spec.md`. Si choca con una skill, gana la spec.
- Lecciones en HTML hecho a mano. `course-data.js` es la única fuente de navegación.
- Contenido y UI en español; comandos e identificadores en inglés.
- Claves de localStorage: `rust-theme`, `rust-progress`.
- Edición 2021 en todo el código; la 2024 solo se documenta como migración.
- Nada de `unwrap()` en ejemplos que simulan producción: usa `Result` y `?`.
- No silencies al borrow checker con `clone()`: explica el préstamo correcto.
- Cada error del compilador se cita con su código (`E0382`, `E0502`, `E0308`…).
- `<` y `>` en los bloques de código van escapados como `&lt;` y `&gt;`.
- El reto nunca se resuelve en el HTML.
- Verifica: `node .agents/skills/academy-verify/scripts/verify.mjs "Aprende Rust"`, `node --check` de los JS, HTTP 200 y tarjeta en `Academy/index.html`.
- El proyecto `proyectos/urlshort/` es un crate real: `cargo test` debe pasar cuando haya toolchain disponible.
- Icono: engranaje de Rust en `favicon.svg` y en la topbar.
