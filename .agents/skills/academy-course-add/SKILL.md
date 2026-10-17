---
name: academy-course-add
description: Use when adding a new `Aprende *` course to the Academy, or scaffolding/registering one. Playbook for the full lifecycle (Fase 0 spec → Fase 5 registration), the files to create, the per-course theme key and identity, and the steps to wire it into the landing and docs. Pairs with `academy-course` (mechanics) and `academy-verify` (verification).
---

# Academy — añadir un curso nuevo

Playbook para crear un curso `Aprende <Tema>` de punta a punta de forma consistente.
Cada curso es el **mismo artefacto** (web estática sin build, navegada desde
`course-data.js`). Detalles mecánicos en `academy-course`; verificación en `academy-verify`.

## Antes de empezar

1. Lee `Academy/specs.md` (contrato general) y elige la **identidad del curso**:
   - **carpeta**: `Aprende <Tema>` (sin `/` en el nombre).
   - **clave de progreso**: corta y única, p. ej. `py`, `dk`, `fastapi` → `<clave>-theme`, `<clave>-progress`.
   - **color/paleta** propia (no reutilices la de otro curso).
   - **firma de portada** específica (terminal, conversación, pipeline, despliegue…).
   - **proyecto conductor** en `proyectos/` (runnable) o justifícalo si no aplica.
2. Copia como base un curso **reciente** con el mismo runtime: `Aprende Docker/`,
   `Aprende Python/`, `Aprende IA/`, `Aprende DevOps/` o `Aprende Cloud/`.

## Fases

### Fase 0 — Spec y AGENTS
- `spec.md` — spec completa del curso (la plantilla habitual de 15 secciones:
  objetivo, audiencia, hilo conductor, tecnologías, forma de enseñar, multiplataforma,
  índice de módulos/lecciones, estructura, formato, diseño, convenciones, docs, portada,
  checklist de fases, reglas).
- `AGENTS.md` — puntero corto a `spec.md` + reglas (orden de lectura de las skills,
  claves de localStorage, verificación).

### Fase 1 — Scaffold
- `package.json` (`"serve": "node scripts/serve-web.mjs"`), `pnpm-workspace.yaml` (si hay proyectos), `.gitignore`, `.gitattributes` (LF), `scripts/serve-web.mjs`.
- `web/assets/`: copia `highlight.js` (verbatim), `fonts/`; **re-tematiza** `site.css`
  (accent/acento secundario del curso) y `site.js` (clave de tema, logo del topbar,
  firma de portada `buildRepl`).
- `web/assets/favicon.svg` con la identidad del curso.
- `web/index.html` (portada + firma) y `web/recursos/{cheatsheet,glosario,faq}.html`.

### Fase 2 — Lecciones (`web/modulos/XX-slug/NN-leccion.html`)
- Mapea primero en `web/assets/js/course-data.js` (módulos + lecciones con `file`, `title`, `minutes`).
- Cada lección: `<html data-module data-lesson>`, clave de tema, `SITE_BASE`, los 3 scripts,
  y las **10 secciones** con sus `id` y numeración (ver `academy-course`).
- Para volumen, reparte por módulos entre varios agentes, con la plantilla y las reglas exactas.

### Fase 3 — Recursos (`web/recursos/…`)
- Cheatsheet, glosario (`dl.glossary`) y FAQ del curso.

### Fase 4 — Proyecto (`proyectos/<nombre>/`)
- Aplicación real y ejecutable (con su `README` y `.gitignore` propios).

### Fase 5 — Registro en la Academy
- Tarjeta en `Academy/index.html` con `data-key="<clave>" data-lessons="<n>"`
  (insertada donde corresponda) y `--brand` con el color del curso.
- Fila en `specs.md` §3 y en `README.md`.
- Actualiza los **totales** de la portada (cursos, módulos, lecciones, recursos) y el conteo de `specs.md` §12.
- Marca las fases ✅ en el `spec.md` del curso.

## Verificación (obligatoria)

```bash
node .agents/skills/academy-verify/scripts/verify.mjs   # 0 errores
```
Más los chequeos manuales de `academy-verify` (node --check, HTTP 200, registro).

## Convenciones

- Contenido/comentarios/UI en **español**; identificadores/comandos en **inglés**.
- Commits **conventional** (`feat(<scope>): …`).
- Sin secretos en el repositorio.
- No dupliques skills por curso: viven en `.agents/skills/`.
