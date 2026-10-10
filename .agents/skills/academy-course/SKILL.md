---
name: academy-course
description: Use when building or editing ANY Academy course website (Learn Astro, Docker, Dotnet, Javascript, NewRelic, Python, React, Redis, SDD, Tailwind, TypeScript). Covers the static-web contract that every Academy course shares: course-data.js as the single source of truth, lesson HTML anatomy, SITE_BASE rules, per-course storage keys, shared CSS classes, scaffold layout, and how to verify before finishing. This is the MECHANICS skill; each course's own spec and AGENTS.md define the CONTENT.
---

# Academy Course Construction (shared skill)

Every `Learn *` folder in this Academy is the **same kind of artefact**: a hand-written static
website with **no build step**, whose chrome (topbar, sidebar, prev/next, progress, light/dark theme,
search) is generated at runtime from a single map file. This skill is the source of truth for the
**mechanics**. Each course's own spec (`<course>/specs.md` or `<course>/spec.md`) and its
`AGENTS.md` are the source of truth for the **content** (what to teach, module list, stack, project).
When this skill and a course spec disagree, the course spec wins — flag the conflict, don't guess.

> This skill lives at `Academy/.agents/skills/academy-course/` and is shared by **all** courses.
> Per-course domain skills (e.g. `python-course`, `spec-driven-development`) also live here now.

## 1. Repository layout (target, per course)

```text
Learn <Course>/
├── specs.md | spec.md         ← content spec (DO NOT contradict it)
├── AGENTS.md | agents.md      ← short pointer to the spec + agent rules
├── package.json               ← "serve": "node scripts/serve-web.mjs"
├── pnpm-workspace.yaml        ← packages: ['proyectos/*']   (only if there are Vite projects)
├── .gitignore                 ← node_modules/, dist/, .vite/, … (see root .gitignore)
├── .gitattributes             ← "* text=auto eol=lf" (+ binaries) when LF matters
├── web/
│   ├── index.html             ← cover + hero + #module-list (see §5)
│   ├── assets/
│   │   ├── css/site.css       ← copied from a newer course, re-themed per course
│   │   ├── js/course-data.js  ← ★ THE MAP (single source of truth)
│   │   ├── js/site.js         ← chrome (copy + adapt keys/logo/signature)
│   │   ├── js/highlight.js    ← copy verbatim, do not modify
│   │   ├── fonts/             ← JetBrains Mono woff2 + fonts.css
│   │   └── favicon.svg        ← per-course identity
│   ├── modulos/XX-slug/       ← one folder per module, NN-file.html per lesson
│   └── recursos/              ← cheatsheet.html, glosario.html, faq.html
├── scripts/serve-web.mjs      ← copy, change the log line to the course name
└── proyectos/<sample>/        ← real, runnable sample project (when the course has one)
```

Reference implementation to copy from: `Learn Docker/` (newest), fallback `Learn Javascript/`.
The skills themselves are **centralised at the Academy root** (`.agents/skills/`), not per course.
The `opencode.json` with the Context7 MCP is also **unique and lives at the Academy root**
(`Academy/opencode.json`); courses do not ship their own copy.

## 2. course-data.js is the law

`web/assets/js/course-data.js` defines `window.COURSE = { title, subtitle, resources[], modules[] }`.
It drives the sidebar, prev/next nav, cover index, search and progress. Rules:

- Every lesson listed **must** exist at `web/<dir>/<file>`; every lesson file **must** be listed.
  No orphans either way.
- Module object: `{ id: "00", dir: "modulos/00-preparacion", title, description, lessons: [{ file, title, minutes }] }`.
- `id` is a zero-padded string (`"00"`…`"NN"`); `file` ends in `.html`.
- `data-module` in the HTML equals the module `id`; `data-lesson` equals `file` **without** `.html`.
- After adding/renaming anything, re-run the structural check (see §7).

## 3. Lesson HTML anatomy (exact)

```html
<!DOCTYPE html>
<html lang="es" data-theme="dark" data-module="05" data-lesson="04-ejemplo">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Título de la lección — Módulo N · Curso <Curso></title>
  <script>document.documentElement.dataset.theme = localStorage.getItem('<key>-theme') || 'dark';</script>
  <link rel="stylesheet" href="../../assets/css/site.css">
  <link rel="icon" href="../../assets/favicon.svg" type="image/svg+xml">
</head>
<body>
  <main>
    <article class="lesson">
      <header class="lesson-header">
        <h1>Título de la lección</h1>
        <div class="lesson-header__meta">
          <span class="badge">Módulo N · Nombre del módulo</span>
          <span>⏱ 30 minutos</span>
        </div>
      </header>
      <section id="introduccion"><h2><span class="section-number">1 ·</span> Introducción</h2>…</section>
      <!-- secciones 2..9 en orden -->
      <section id="faq"><h2><span class="section-number">10 ·</span> Preguntas frecuentes</h2>…</section>
    </article>
  </main>
  <script>window.SITE_BASE = '../../';</script>
  <script src="../../assets/js/course-data.js"></script>
  <script src="../../assets/js/highlight.js"></script>
  <script src="../../assets/js/site.js"></script>
</body>
</html>
```

The 10 sections, in order, with these exact `id`s: `introduccion`, `teoria`, `ejemplo-minimo`,
`ejemplo-real`, `buenas-practicas`, `errores`, `ejercicio`, `reto`, `resumen`, `faq`.
Headings numbered `1 ·` … `10 ·` with `<span class="section-number">`.
**Never solve the `reto`** in the HTML (only a hint, `.challenge`).

## 4. SITE_BASE and storage keys (per-course!)

| File location | `SITE_BASE` | Asset prefix |
|---|---|---|
| `web/index.html` | `'./'` | `./assets/…` |
| `web/recursos/*.html` | `'../'` | `../assets/…` |
| `web/modulos/*/*.html` | `'../../'` | `../../assets/…` |

- `site.js` storage keys **must** be namespaced per course and never reused across courses.
- The inline `<script>` in `<head>` that sets `data-theme` must use the same theme key.
- Topbar logo text/initials and the cover signature block must be re-themed per course — never ship
  another course's leftovers. The signature is course-specific (a REPL for Python, a terminal for
  Docker, a spec document for SDD, …).

Per-course keys and specs (keep this table in sync when a course is added):

| Course | Theme / progress key | Spec file | Sample project |
|---|---|---|---|
| Learn Astro | `astro-*` | `AGENTS.md` | — |
| Learn Docker | `dk-*` | `specs.md` | `proyectos/harbortasks` |
| Learn Dotnet | `dn8-*` | `AGENTS.md` | `proyectos/m*-cms-*` (CMS) |
| Learn Javascript | `js-*` | `AGENTS.md` | — |
| Learn NewRelic | `nr-*` | `AGENTS.md` | `proyectos/newrelic-ws-demo` |
| Learn Python | `py-*` | `specs.md` | `proyectos/biblioteca` |
| Learn React | `rc19-*` | `agents.md` | `proyectos/m*`, `proyecto-final/` |
| Learn Redis | `redis-*` | `AGENTS.md` | `proyectos/redis-ws-hub` |
| Learn SDD | `sdd-*` | `spec.md` | `proyectos/lumen` |
| Learn Tailwind | `tw-*` | `AGENTS.md` | `proyectos/m06-landing-completa`, `m09-dashboard` |
| Learn TypeScript | `ts-*` | `AGENTS.md` | `proyectos/08-typescript-react`, `09-proyecto-final` |

## 5. Cover (web/index.html)

Adapt a newer course's cover; do not redesign from scratch:

- `<body data-page="home">`, hero eyebrow, `h1`, signature block (`<div class="console">` +
  `<div class="console__body" data-repl>` — filled by `site.js:buildRepl`), lead paragraph, action
  buttons incl. `[data-continue]`, `#home-stats`, "Cómo funciona" callouts,
  `<h2>Índice del curso</h2>` + `<div id="module-list"></div>`.
- `buildRepl` in `site.js` must be rewritten for the course signature; keep the
  `prefers-reduced-motion` branch.

## 6. Shared CSS classes (do not invent parallel systems)

- Callouts: `callout callout--why | --tip | --note | --warning | --analogy` (+ `callout__title`).
- `table` with `thead/tbody` for comparisons and good-practice tables.
- `ol.steps > li > strong` for guided exercises.
- `.challenge` (+ `h3` + hint) for the reto — **never solve the reto in the HTML**.
- `.faq > details > summary + div` for FAQs.
- `pre > code.language-*` for code (copy button + highlight are automatic). Common values:
  `language-javascript`, `language-typescript`, `language-python`, `language-csharp`, `language-html`,
  `language-css`, `language-bash`, `language-json`, `language-yaml`, `language-dockerfile`,
  `language-toml`, `language-text`.
- `dl.glossary` for the glossary resource.
- Tokens `t-kw t-str t-num t-pun t-fn t-bt t-com t-tag t-attr` only inside the cover signature block.

Language: UI, prose and comments in **Spanish**; identifiers and commands in **English**.
Escape `<` as `&lt;` inside code blocks (`a < b`, generics, etc.).

## 7. Verification (run every time before declaring done)

1. `node --check web/assets/js/site.js && node --check web/assets/js/course-data.js`
2. Structural check over `window.COURSE`: every mapped file exists; every file has `data-module`,
   `data-lesson`, all 10 section ids, correct `SITE_BASE`, the course theme key, and the three scripts.
3. Serve: `node scripts/serve-web.mjs` (ports are assigned per course, e.g. Python 3001) and fetch
   `/`, one lesson per module, and all `recursos/` — expect HTTP 200.
4. `Academy/index.html`: the course card links to `Learn <Course>/web/index.html`; the footer course
   count is correct.

## 8. What NOT to do

- Do not put lesson content in Markdown — lessons are hand-written HTML.
- Do not add a build step to the course site itself (no Vite/Next/Astro for `web/`). Vite is only for
  `proyectos/` sample apps where the course calls for it.
- Do not modify `highlight.js`.
- Do not invent a new sidebar/nav system — `site.js` + `course-data.js` already do it.
- Do not rewrite a course spec while implementing; follow it, and flag contradictions explicitly
  instead of silently diverging.
- Do not duplicate skills per course — they live at the Academy root (`.agents/skills/`).
