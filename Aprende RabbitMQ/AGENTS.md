# Aprende RabbitMQ — instrucciones para agentes

Este `AGENTS.md` es un **puntero corto**. La fuente de verdad del curso es **`spec.md`** (mismo directorio). Léelo entero antes de tocar nada.

Las skills están **centralizadas en la raíz de la Academy** (`Academy/.agents/skills/`). Desde este directorio: `../.agents/skills/`.

## Orden de lectura obligatorio

1. `spec.md` — spec del curso (garantías, temario, proyecto job-queue, diseño).
2. `../.agents/skills/academy-course/SKILL.md` — mecánica de la web.
3. `../.agents/skills/academy-verify/SKILL.md` — verificación.
4. `../.agents/skills/documentation/SKILL.md` — contrastar RabbitMQ 4.4 y `amqplib` vía Context7.
5. `../.agents/skills/frontend-design/SKILL.md` — identidad visual propia.

Referencia de chrome (estilos/scripts): `Aprende Git/`, `Aprende Cloud/`.

## Reglas

- No contradigas `spec.md`. Si choca con una skill, gana la spec.
- Lecciones en HTML hecho a mano. `course-data.js` es la única fuente de navegación.
- Contenido y UI en español; comandos e identificadores en inglés.
- Toda garantía de entrega se explica y se justifica (at-most-once, at-least-once, idempotencia).
- Producción: publisher confirms, `ack` manual, colas durables, quorum queues y DLQ.
- Claves de localStorage: `rmq-theme`, `rmq-progress`.
- Verifica: `node .agents/skills/academy-verify/scripts/verify.mjs "Aprende RabbitMQ"`, `node --check`, HTTP 200, tarjeta en `Academy/index.html`.
- El proyecto `proyectos/job-queue/` se levanta con Docker Compose y se prueba con `node --test`.
- Icono oficial de RabbitMQ (Simple Icons) en favicon y topbar.
