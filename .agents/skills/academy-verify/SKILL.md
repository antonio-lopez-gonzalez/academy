---
name: academy-verify
description: Use when you have edited or added any Academy course and need to check it conforms to the shared contract before finishing. Runs a fast structural verification (lesson anatomy, section ids, theme key, scripts, orphans) across one or all `Aprende *` courses, plus the manual checks (node --check, HTTP 200, landing registration).
---

# Academy — verificación de un curso

Comprueba que un curso `Aprende *` cumple el **contrato** de la Academy (ver
`Academy/specs.md` y la skill `academy-course`). Úsala siempre antes de dar por
terminado un cambio en cualquier curso.

## 1. Verificación automática (un comando)

```bash
# Todos los cursos
node .agents/skills/academy-verify/scripts/verify.mjs

# Un curso concreto (por nombre de carpeta)
node .agents/skills/academy-verify/scripts/verify.mjs "Aprende FastAPI"

# Si lo ejecutas desde otra ruta
node .agents/skills/academy-verify/scripts/verify.mjs --root "C:/Desarrollo/Academy"
```

Sale código de salida **0** si todo está bien y **1** si hay errores. Informa por curso:

- `web/index.html` y `course-data.js` presentes.
- Cada lección del mapa **existe** en disco (sin huérfanos en ningún sentido).
- Cada lección tiene: `data-lesson` correcto, `highlight.js`, numeración
  (`section-number`) y las **10 secciones** con sus `id` exactos
  (`introduccion`, `teoria`, `ejemplo-minimo`, `ejemplo-real`, `buenas-practicas`,
  `errores`, `ejercicio`, `reto`, `resumen`, `faq`).
- Detecta lecciones en disco que no están en el mapa (huérfanas).

## 2. Verificación manual (lo que el script no cubre)

1. **Sintaxis de los JS**:
   ```bash
   node --check web/assets/js/site.js && node --check web/assets/js/course-data.js
   ```
2. **Servir y pedir HTTP 200** (portada, una lección por módulo y todos los recursos):
   ```bash
   node scripts/serve-web.mjs     # y pide /, /modulos/…, /recursos/…
   ```
   También comprueba que `SITE_BASE` es correcto (`./`, `../`, `../../`).
3. **Registro en la Academy**: la tarjeta del curso en `Academy/index.html`
   (con `data-key` y `data-lessons`), la tabla de `specs.md` §3 y de `README.md`,
   y el contador del pie.
4. **Proyecto** (`proyectos/…`): que compile o sea coherente y revisable.

## 3. Qué hace "pasar" la verificación

- 0 errores del script.
- `node --check` OK.
- Todas las rutas sirven 200.
- El mapa y el disco coinciden (sin huérfanas).
- El curso está registrado en la portada y en las tablas.

Si algo falla, corrige y vuelve a ejecutar. No declares un curso terminado con
errores pendientes.
