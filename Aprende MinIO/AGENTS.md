# Aprende MinIO — instrucciones para agentes

Este `AGENTS.md` es un **puntero corto**. La fuente de verdad del curso es **`spec.md`** (mismo directorio). Léelo entero antes de tocar nada.

Las skills están **centralizadas en la raíz de la Academy** (`Academy/.agents/skills/`). Desde este directorio: `../.agents/skills/`.

## Orden de lectura obligatorio

1. `spec.md` — spec del curso (modelo S3, seguridad, lifecycle, proyecto document-store).
2. `../.agents/skills/academy-course/SKILL.md` — mecánica de la web.
3. `../.agents/skills/academy-verify/SKILL.md` — verificación.
4. `../.agents/skills/documentation/SKILL.md` — contrastar MinIO, `mc`, `minio-js` y S3 vía Context7.
5. `../.agents/skills/frontend-design/SKILL.md` — identidad visual propia.

Referencia de chrome (estilos/scripts): `Aprende Git/`, `Aprende Cloud/`.

## Reglas

- No contradigas `spec.md`. Si choca con una skill, gana la spec.
- Lecciones en HTML hecho a mano. `course-data.js` es la única fuente de navegación.
- Contenido y UI en español; comandos e identificadores en inglés.
- Bucket ≠ carpeta: explica objeto y prefijo con precisión.
- Root solo para arranque; aplicación con usuario de mínimo privilegio y presigned URLs acotadas.
- Compose es solo para un nodo; el modo distribuido se explica con Kubernetes Operator.
- Claves de localStorage: `minio-theme`, `minio-progress`.
- Verifica: `node .agents/skills/academy-verify/scripts/verify.mjs "Aprende MinIO"`, `node --check`, HTTP 200, tarjeta en `Academy/index.html`.
- El proyecto `proyectos/document-store/` se levanta con Docker Compose y se prueba con `node --test`.
- Icono oficial de MinIO (Simple Icons) en favicon y topbar.
