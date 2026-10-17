# Academy

Centro de conocimiento: **cursos independientes** (`Aprende <Curso>/`) que comparten una misma forma
de construirse y entregarse. La portada [`index.html`](./index.html) es el punto de entrada único.

- **Contrato común (mecánica):** [`specs.md`](./specs.md) + la skill `academy-course`.
- **Contenido de cada curso:** su propia spec (`Aprende <Curso>/specs.md` | `spec.md`) y su `AGENTS.md`.
- **Instrucciones para agentes:** [`AGENTS.md`](./AGENTS.md).
- **MCP de Context7:** [`opencode.json`](./opencode.json) (único, en la raíz).

---

## ¿Qué es la Academy?

Una colección de cursos en español, pensados para quien **ya sabe programar** y quiere dominar un tema
en profundidad. Cada curso es a la vez:

1. Una **web estática navegable** (`web/`): HTML + CSS + JS vanilla, **sin Markdown y sin paso de
   build**. El chrome (topbar, sidebar, navegación anterior/siguiente, progreso, tema claro/oscuro y
   buscador) lo genera en tiempo de ejecución `web/assets/js/site.js` a partir del mapa
   `web/assets/js/course-data.js` (única fuente de verdad de la navegación).
2. Un curso con **proyecto(s) real(es) y ejecutable(s)** en `proyectos/` (Vite, .NET, Python, Docker,
   etc.), que sí pueden tener su propio build.

Todas las lecciones siguen **10 secciones fijas**: introducción, teoría, ejemplo mínimo, ejemplo real,
buenas prácticas, errores habituales, ejercicio guiado, reto, resumen y preguntas frecuentes.

---

## Cursos

| Curso | Tema | Módulos | Lecciones | Proyecto de ejemplo |
|---|---|---|---|---|
| `Aprende Astro` | Astro 5+, islands, content-driven | 10 | 39 | — |
| `Aprende Docker` | Docker y Kubernetes (kind, Helm) | 10 | 43 | `proyectos/harbortasks` |
| `Aprende Dotnet` | .NET 8 + C# 12 (de cero a arquitecto) | 19 | 128 | `proyectos/m*-cms-*` (CMS) |
| `Aprende Javascript` | JavaScript moderno (ES2024+) | 10 | 44 | — |
| `Aprende NewRelic` | Observabilidad New Relic para .NET | 9 | 35 | `proyectos/newrelic-ws-demo` |
| `Aprende Python` | Python 3.12+ (de cero a paquete) | 10 | 45 | `proyectos/biblioteca` |
| `Aprende React` | React 19 + Vite | 16 | 102 | `proyectos/m*`, `proyecto-final/` |
| `Aprende Redis` | Redis para .NET (hub + caché) | 9 | 34 | `proyectos/redis-ws-hub` |
| `Aprende SDD` | Spec-Driven Development | 8 | 32 | `proyectos/lumen` |
| `Aprende Tailwind` | Tailwind CSS v4 | 10 | 43 | `proyectos/m06-landing-completa`, `m09-dashboard` |
| `Aprende TypeScript` | TypeScript profesional | 13 | 51 | `proyectos/08-typescript-react`, `09-proyecto-final` |
| `Aprende IA` | IA aplicada (LLM, RAG, agentes, MCP) | 9 | 36 | `proyectos/copiloto` |
| `Aprende FastAPI` | FastAPI (APIs modernas en Python) | 8 | 32 | `proyectos/copiloto-api` |
| `Aprende DevOps` | CI/CD y DevOps (Actions, IaC) | 8 | 32 | `proyectos/pipeline` |
| `Aprende Cloud` | Cloud (Azure / AWS, Terraform) | 8 | 32 | `proyectos/infra-cloud` |
| `Aprende System Design` | System Design (arquitectura a escala) | 8 | 32 | `proyectos/case-studies` |

> Conteos de `web/modulos/`. Cada curso añade además `web/recursos/` (cheatsheet, glosario, FAQ).

---

## Descargas (ZIP de proyectos)

Cada curso con `proyectos/` y/o `ejemplos/` ofrece en su portada un botón **«⬇ Descargar código (.zip)»**
con **solo** ese código (nunca las lecciones):

- El ZIP está **commiteado** en `Aprende <Curso>/web/descargas/Aprende-<Curso>.zip` y se descarga
  directamente desde la portada del curso.
- Se regenera con `node tools/build-zips.mjs` (desde la raíz) después de cambiar el código.
- Se sirve como cualquier archivo estático (GitHub Pages o `npm run serve` del curso).

---

## Estructura del repositorio

```text
Academy/
├── specs.md            ← spec general unificada (contrato común de todos los cursos)
├── AGENTS.md           ← puntero corto para agentes
├── README.md           ← este archivo
├── opencode.json       ← MCP de Context7 (único para toda la Academy)
├── index.html          ← portada: una tarjeta por curso
├── .gitignore / .nojekyll
├── tools/build-zips.mjs           ← regenera los ZIP de proyectos/ejemplos
├── .agents/skills/     ← skills COMPARTIDAS (no duplicar por curso)
│   ├── academy-course/           mecánica de cualquier curso Academy
│   ├── documentation/            consultar docs oficiales vía Context7
│   ├── frontend-design/          identidad visual propia
│   ├── python-course/            dominio de Aprende Python
│   └── spec-driven-development/  dominio de Aprende SDD
└── Aprende <Curso>/
    ├── specs.md | spec.md        spec de contenido (fuente de verdad)
    ├── AGENTS.md | agents.md     puntero corto a la spec + reglas
    ├── package.json              "serve": node scripts/serve-web.mjs (cuando aplica)
    ├── pnpm-workspace.yaml       packages: ['proyectos/*'] (cuando aplica)
    ├── web/                      ★ el curso (web estática, sin build)
    │   ├── index.html
    │   ├── assets/{css,js,fonts,favicon.svg}
    │   ├── modulos/XX-slug/NN-leccion.html
    │   ├── recursos/{cheatsheet,glosario,faq}.html
    │   └── descargas/<slug>.zip  ZIP del proyecto (descarga directa)
    ├── scripts/serve-web.mjs     servidor estático local (cuando aplica)
    └── proyectos/<proyecto>/     proyecto(s) real(es) y ejecutable(s)
```

---

## Cómo navegar

### Opción rápida (sin instalar nada)

Abre la portada y navega por los cursos:

```bash
# Abre el índice general en el navegador
start index.html          # Windows (PowerShell)
# open index.html         # macOS
# xdg-open index.html     # Linux
```

> La portada enlaza con rutas relativas a `Aprende <Curso>/web/index.html`. Si tu navegador bloquea
> recursos locales, usa un servidor estático (abajo).

### Servir un curso con su propio servidor

Cada curso que lo necesita incluye `scripts/serve-web.mjs`. Desde la carpeta del curso:

```bash
cd "Aprende Docker"
npm run serve             # o: node scripts/serve-web.mjs
# → http://localhost:3001
```

Todos los servidores usan el puerto **3001** por defecto. Para servir dos a la vez, cambia el puerto:

```bash
# PowerShell
$env:PORT=3002; npm run serve
# bash
PORT=3002 npm run serve
```

### Alternativa genérica

```bash
npx serve "Aprende Python/web"
```

---

## Requisitos

- **Node.js 20+** y **pnpm 11** (para los proyectos Vite/TS/JS y los servidores estáticos).
- **.NET 8 SDK** para `Aprende Dotnet`, `Aprende NewRelic`, `Aprende Redis` y el backend de `Aprende React`.
- **Python 3.12+** para `Aprende Python`.
- **Docker / kind / kubectl / Helm** para `Aprende Docker`.
- Context7 MCP para agentes: se instala solo (`npx -y @upstash/context7-mcp`) al arrancar OpenCode.

---

## Contrato común de un curso (resumen)

Definido en [`specs.md`](./specs.md) y en la skill `academy-course`:

- **Navegación:** `web/assets/js/course-data.js` es la ley; no puede divergir de los archivos en disco.
- **Anatomía de lección** y 10 `id` de sección exactos.
- **`SITE_BASE`:** `'./'` en la portada, `'../'` en `recursos/`, `'../../'` en `modulos/`.
- **Claves `localStorage` por curso:** `<clave>-theme` / `<clave>-progress` (p. ej. `py-theme`, `dk-theme`).
- **Clases CSS compartidas:** callouts, `table`, `ol.steps`, `.challenge`, `.faq`, `pre > code.language-*`,
  `dl.glossary`.
- **Idioma:** contenido, comentarios y UI en **español**; identificadores y comandos en **inglés**.
- **El reto nunca se resuelve** en el HTML (solo pista).

### Verificación (antes de dar algo por terminado)

1. `node --check web/assets/js/site.js && node --check web/assets/js/course-data.js`
2. Chequeo estructural sobre `window.COURSE` (archivos existentes, `data-module`, `data-lesson`, 10
   secciones, `SITE_BASE`, clave de tema y los tres scripts).
3. `node scripts/serve-web.mjs` y pedir `/`, una lección por módulo y todos los recursos → HTTP 200.
4. Tarjeta correcta del curso en `Academy/index.html` y contador del pie actualizado.

---

## Skills y MCP

- **Skills compartidas** en [`.agents/skills/`](./.agents/skills). No se duplican por curso.
  - `academy-course` — mecánica común.
  - `documentation` — consultar documentación oficial **vía Context7** antes de afirmar APIs/versiones.
  - `frontend-design` — identidad visual propia.
  - `python-course`, `spec-driven-development` — skills de dominio de sus cursos.
- **MCP de Context7** en [`opencode.json`](./opencode.json): un único servidor local
  `npx -y @upstash/context7-mcp` para toda la Academy.

---

## Añadir o editar un curso

Checklist completo en [`specs.md`](./specs.md) §13. En resumen:

1. **Spec y skills:** escribir/actualizar la spec del curso y su `AGENTS.md` (usa las skills de la raíz).
2. **Scaffold:** `package.json` (serve), `pnpm-workspace.yaml` (si aplica), `.gitignore`, `.gitattributes` (LF),
   `scripts/serve-web.mjs`.
3. **Lecciones:** `web/modulos/XX-slug/NN-leccion.html` con las 10 secciones y `course-data.js` como mapa.
4. **Recursos:** `web/recursos/{cheatsheet,glosario,faq}.html`.
5. **Proyecto:** `proyectos/<proyecto>/` real y ejecutable (si aplica).
6. **Academy:** tarjeta en `index.html` y contador del pie; añade la fila a la tabla de `specs.md` §3.

---

## Convenciones de commit

Este repositorio usa **[Conventional Commits](https://www.conventionalcommits.org/)**:

```text
<tipo>(<ámbito>): <descripción>

feat(docker): add module 05 — de Docker a Kubernetes
fix(python): correct SITE_BASE in lesson 07
docs(sdd): clarify acceptance criteria template
chore(academy): centralise skills under .agents/skills
```

Tipos habituales: `feat`, `fix`, `docs`, `chore`, `refactor`, `test`, `ci`.

---

## Licencia

Contenido del curso con fines educativos. Sin licencia de uso comercial (ver cada proyecto).
