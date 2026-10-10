# Academy — Especificación general (spec unificada)

## Centro de conocimiento: cursos `Learn *` bajo un mismo contrato

---

# 1. Objetivo

Este repositorio es la **Academy**: una colección de cursos independientes, cada uno en su carpeta
`Learn <Tema>/`, que comparten **una misma forma de construirse y entregarse**. La portada
`index.html` (raíz) es el punto de entrada único: lista un curso por tarjeta y cada tarjeta enlaza a
`Learn <Tema>/web/index.html`.

Este documento es la **spec general unificada**: define el contrato común a todos los cursos (qué es
un curso, cómo se estructura, cómo se navega, cómo se verifica). **No sustituye** a la spec de cada
curso: el *qué enseñar* (temario, stack, proyecto, tono pedagógico) vive en la spec propia de cada
curso y en su `AGENTS.md`.

- Contrato mecánico compartido (esta spec + skill `academy-course`).
- Contenido de cada curso → `<curso>/specs.md` \| `<curso>/spec.md` \| `<curso>/AGENTS.md`.
- Skills reutilizables → `Academy/.agents/skills/`.

---

# 2. Audiencia y requisitos previos

- El alumno de cada curso **ya sabe programar** en algún lenguaje; cada curso declara sus propios
  requisitos previos en su spec.
- Quien construye o edita un curso (humano o agente) debe leer **primero** este documento, la spec
  del curso afectado y la skill `academy-course` antes de escribir HTML.

---

# 3. Los cursos (índice de la Academy)

| Curso | Tema | Spec del curso | Claves localStorage | Módulos | Lecciones | Proyecto de ejemplo |
|---|---|---|---|---|---|---|
| `Learn Astro` | Astro 5+, islands, content-driven | `AGENTS.md` | `astro-theme/progress` | 10 | 39 | — |
| `Learn Docker` | Docker y Kubernetes (kind, Helm) | `specs.md` | `dk-theme/progress` | 10 | 43 | `proyectos/harbortasks` |
| `Learn Dotnet` | .NET 8 + C# 12 (de cero a arquitecto) | `AGENTS.md` | `dn8-theme/progress` | 19 | 128 | `proyectos/m*-cms-*` (CMS) |
| `Learn Javascript` | JavaScript moderno (ES2024+) | `AGENTS.md` | `js-theme/progress` | 10 | 44 | — |
| `Learn NewRelic` | Observabilidad New Relic para .NET | `AGENTS.md` | `nr-theme/progress` | 9 | 35 | `proyectos/newrelic-ws-demo` |
| `Learn Python` | Python 3.12+ (de cero a paquete) | `specs.md` | `py-theme/progress` | 10 | 45 | `proyectos/biblioteca` |
| `Learn React` | React 19 + Vite | `agents.md` | `rc19-theme/progress` | 16 | 102 | `proyectos/m*`, `proyecto-final/` |
| `Learn Redis` | Redis para .NET (hub + caché) | `AGENTS.md` | `redis-theme/progress` | 9 | 34 | `proyectos/redis-ws-hub` |
| `Learn SDD` | Spec-Driven Development | `spec.md` | `sdd-theme/progress` | 8 | 32 | `proyectos/lumen` |
| `Learn Tailwind` | Tailwind CSS v4 | `AGENTS.md` | `tw-theme/progress` | 10 | 43 | `proyectos/m06-landing-completa`, `m09-dashboard` |
| `Learn TypeScript` | TypeScript profesional | `AGENTS.md` | `ts-theme/progress` | 10 | 39 | `proyectos/08-typescript-react`, `09-proyecto-final` |

> Los conteos de módulos/lecciones son los de `web/modulos/`. Cada curso añade además `web/recursos/`
> (cheatsheet, glosario, FAQ). Las claves se escriben completas como `<clave>-theme` y `<clave>-progress`.

Cursos con proyecto ejecutable real: Docker, Dotnet, NewRelic, Python, React, Redis, SDD, Tailwind,
TypeScript. Astro y Javascript son solo curso (web).

---

# 4. Especificación común (contrato compartido)

Todos los cursos son el **mismo artefacto**: una **web estática hecha a mano, sin paso de build**,
cuyo chrome (topbar, sidebar, navegación anterior/siguiente, progreso, tema claro/oscuro y buscador)
se genera en tiempo de ejecución a partir de **un único fichero mapa**.

## 4.1. Layout de un curso

```text
Learn <Curso>/
├── specs.md | spec.md        ← spec de contenido del curso (fuente de verdad)
├── AGENTS.md | agents.md     ← puntero corto a la spec + reglas para agentes
├── package.json              ← "serve": "node scripts/serve-web.mjs"
├── pnpm-workspace.yaml       ← packages: ['proyectos/*'] (si hay proyectos Vite)
├── .gitignore / .gitattributes
├── web/                      ← ★ EL CURSO
│   ├── index.html            ← portada + hero + #module-list
│   ├── assets/{css,js,fonts,favicon.svg}
│   ├── modulos/XX-slug/      ← una carpeta por módulo, NN-leccion.html
│   └── recursos/             ← cheatsheet.html, glosario.html, faq.html
├── scripts/serve-web.mjs
└── proyectos/<proyecto>/     ← proyecto ejecutable real (si aplica)
```

Los proyectos de `proyectos/` (Vite/TS/JS, .NET, Python, …) **sí** pueden tener build propio; lo que
nunca tiene build es la web del curso (`web/`).

## 4.2. `course-data.js` es la ley

`web/assets/js/course-data.js` define `window.COURSE = { title, subtitle, resources[], modules[] }` y
alimenta el sidebar, la navegación, el índice de la portada, el buscador y el progreso.

- Toda lección listada **existe** en disco; toda lección en disco **está listada**. Sin huérfanos.
- Módulo: `{ id: "00", dir: "modulos/00-slug", title, description, lessons: [{ file, title, minutes }] }`.
- `id` es cadena con cero a la izquierda; `file` termina en `.html`.
- `data-module` = `id`; `data-lesson` = `file` sin `.html`.

## 4.3. Anatomía exacta de una lección

```html
<!DOCTYPE html>
<html lang="es" data-theme="dark" data-module="00" data-lesson="00-ejemplo">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Título — Módulo N · Curso <Curso></title>
  <script>document.documentElement.dataset.theme = localStorage.getItem('<clave>-theme') || 'dark';</script>
  <link rel="stylesheet" href="../../assets/css/site.css">
  <link rel="icon" href="../../assets/favicon.svg" type="image/svg+xml">
</head>
<body>
  <main><article class="lesson">
    <header class="lesson-header"><h1>…</h1><div class="lesson-header__meta">…</div></header>
    <section id="introduccion"><h2><span class="section-number">1 ·</span> Introducción</h2>…</section>
    <!-- secciones 2..9 en orden -->
    <section id="faq"><h2><span class="section-number">10 ·</span> Preguntas frecuentes</h2>…</section>
  </article></main>
  <script>window.SITE_BASE = '../../';</script>
  <script src="../../assets/js/course-data.js"></script>
  <script src="../../assets/js/highlight.js"></script>
  <script src="../../assets/js/site.js"></script>
</body>
</html>
```

Las 10 secciones, en orden y con estos `id` exactos: `introduccion`, `teoria`, `ejemplo-minimo`,
`ejemplo-real`, `buenas-practicas`, `errores`, `ejercicio`, `reto`, `resumen`, `faq`.
El **reto nunca se resuelve** en el HTML (solo pista, `.challenge`).

## 4.4. `SITE_BASE` y claves de almacenamiento

| Ubicación | `SITE_BASE` | Prefijo de assets |
|---|---|---|
| `web/index.html` | `'./'` | `./assets/…` |
| `web/recursos/*.html` | `'../'` | `../assets/…` |
| `web/modulos/*/*.html` | `'../../'` | `../../assets/…` |

- Las claves de `localStorage` (`<clave>-theme`, `<clave>-progress`) son **exclusivas de cada curso**;
  nunca se reutilizan entre cursos. El `<script>` inline de `<head>` usa la misma clave de tema.

## 4.5. Clases CSS compartidas (no inventar sistemas paralelos)

- Callouts: `callout callout--why | --tip | --note | --warning | --analogy` (+ `callout__title`).
- `table` (thead/tbody) para comparativas y buenas prácticas.
- `ol.steps > li > strong` para ejercicios guiados.
- `.challenge` (+ `h3` + pista) para el reto.
- `.faq > details > summary + div` para FAQ.
- `pre > code.language-*` para código (botón copiar y resaltado automáticos).
- `dl.glossary` para el glosario.
- Tokens `t-kw t-str t-num t-pun t-fn t-bt t-com t-tag t-attr` solo en la firma de portada.

## 4.6. Verificación (siempre antes de dar por hecho)

1. `node --check web/assets/js/site.js && node --check web/assets/js/course-data.js`.
2. Chequeo estructural sobre `window.COURSE`: cada archivo listado existe; cada archivo tiene
   `data-module`, `data-lesson`, los 10 `id` de sección, `SITE_BASE` correcto, la clave de tema del
   curso y los tres scripts.
3. Servir con `node scripts/serve-web.mjs` y pedir `/`, una lección por módulo y todos los recursos
   → HTTP 200.
4. En `Academy/index.html`, la tarjeta del curso enlaza a `Learn <Curso>/web/index.html` y el
   contador de cursos del pie es correcto.

---

# 5. Skills (centralizadas)

Las skills son **reutilizables y compartidas**; viven en la raíz, no duplicadas por curso:

```text
Academy/.agents/skills/
├── academy-course/           ← mecánica común de cualquier curso Academy (esta spec en forma de how-to)
├── documentation/            ← consultar documentación oficial vía Context7 antes de afirmar APIs
├── frontend-design/          ← identidad visual propia (prohibidos los defaults genéricos de IA)
├── python-course/            ← dominio del curso Learn Python (contenido fiel)
└── spec-driven-development/  ← dominio del curso Learn SDD (contenido fiel)
```

- Cada curso referencia las skills por ruta relativa: `../.agents/skills/<skill>/SKILL.md`.
- Skills de **mecánica/transversales**: `academy-course`, `documentation`, `frontend-design`.
- Skills de **dominio**: solo tienen sentido para su curso (`python-course` → Learn Python,
  `spec-driven-development` → Learn SDD). El resto de cursos vuelcan su dominio en su propia spec.
- Si una skill de dominio y la spec del curso discrepan, **gana la spec del curso** y se señala el choque.

---

# 6. Forma de enseñar (idéntica en todos los cursos)

Cada lección sigue **exactamente** estas 10 secciones:

1. **Introducción** — qué, por qué existe, qué problema resuelve, cuándo se usa, errores típicos.
2. **Teoría** — con detalle, sin asumir el tema; comparaciones; el *por qué* de cada decisión.
3. **Ejemplo mínimo** — lo más pequeño posible, explicado línea a línea.
4. **Ejemplo real** — el mismo concepto en el hilo conductor del curso o un escenario profesional.
5. **Buenas prácticas** — qué hacer / qué evitar / por qué (tabla cuando aplique).
6. **Errores habituales** — ejemplo incorrecto, por qué está mal y la versión correcta.
7. **Ejercicio guiado** — paso a paso, resuelto al detalle con verificación.
8. **Reto** — reto adicional **sin resolver** (pista incluida).
9. **Resumen** — bullets + conceptos clave.
10. **Preguntas frecuentes** — FAQ (3–5 `<details>`).

Reglas de contenido transversales:

- Contenido, comentarios y UI en **español**; identificadores y comandos en **inglés**.
- Todo el código debe **ejecutarse** con las versiones que el curso fija.
- Cada curso fija versiones y stack en su propia spec («Utilizar SIEMPRE» / «No utilizar»).
- No reescribir la spec del curso al implementar: se implementa; solo corrección puntual justificada.

---

# 7. Estructura del repositorio (raíz)

```text
Academy/
├── specs.md                  ← ★ ESTE DOCUMENTO (spec general unificada)
├── AGENTS.md                 ← puntero corto a specs.md + reglas para agentes
├── opencode.json             ← Context7 MCP (único para toda la Academy)
├── index.html                ← portada: tarjetas de acceso a cada curso
├── .gitignore                ← node_modules/, dist/, bin/, obj/, __pycache__/, .env, …
├── .agents/skills/           ← skills compartidas (ver §5)
└── Learn <Curso>/            ← un curso por carpeta (ver §4.1)
```

---

# 8. Formato de entrega

- **La web del curso**: HTML + CSS + JavaScript vanilla, **sin Markdown y sin build**. Cada lección
  es un `.html` completo; el chrome lo inyecta `web/assets/js/site.js` desde `course-data.js`.
- **Los proyectos** de `proyectos/`: aplicaciones **reales y ejecutables** (Vite+TS/JS, .NET, Python,
  Compose+kind+Helm…), con su propio build y su propio README.
- Fuente de verdad de navegación: `web/assets/js/course-data.js`. Fuente de verdad de diseño visual:
  `frontend-design` + la spec de marca de cada curso.

---

# 9. Diseño y marca

- Tema oscuro por defecto, con toggle a claro y respeto de `prefers-reduced-motion`.
- Cada curso tiene **identidad propia** (paleta, tipografía, firma de portada, `favicon.svg`): no se
  reutilizan los distintivos de otro curso. Prohibidos los defaults genéricos de IA (ver skill
  `frontend-design`).
- La portada de cada curso incluye la firma específica del curso (REPL, terminal, documento de spec…).

---

# 10. Convenciones de contenido

1. Español en contenido/comentarios/UI; inglés en identificadores y comandos.
2. Cada lección usa SIEMPRE las 10 secciones.
3. Canvas navegable = HTML hecho a mano; `course-data.js` es la única fuente de navegación.
4. Los retos no se resuelven en el HTML.
5. Rutas y comandos canónicos del curso, con callouts para diferencias por SO cuando aplique.
6. Progresión lógica; si un concepto depende de otro, se enlaza la lección correspondiente.

---

# 11. Documentación actualizada

Antes de afirmar una API, versión, flag o comando, consultar la **documentación oficial** y Context7
(mediante la skill `documentation`). Preferir la **última versión estable**. Cada curso fija su stack y
sus versiones en su propia spec.

El **MCP de Context7** se declara una sola vez, en `Academy/opencode.json` (servidor local
`npx -y @upstash/context7-mcp`); los cursos no llevan copia propia de la configuración.

---

# 12. Página principal (`index.html` de la raíz)

- Portada tipo grid de `.card`; cada tarjeta enlaza a `Learn <Curso>/web/index.html`.
- `<footer>` indica el número de cursos disponibles (hoy **11**).
- Al añadir un curso, añadir su tarjeta y actualizar el contador.

---

# 13. Checklist para añadir o editar un curso

- Fase 0 — **Spec y skills**: escribir/actualizar la spec del curso (`specs.md`/`spec.md`) y su
  `AGENTS.md`; usar las skills de `Academy/.agents/skills/` (no duplicarlas).
- Fase 1 — **Scaffold**: `package.json` (serve), `pnpm-workspace.yaml` (si aplica),
  `.gitignore`, `.gitattributes` (LF), `scripts/serve-web.mjs`. (El MCP de Context7 ya está en la raíz.)
- Fase 2 — **Lecciones**: `web/modulos/XX-slug/NN-leccion.html` con las 10 secciones y `course-data.js`
  como mapa.
- Fase 3 — **Recursos**: `web/recursos/{cheatsheet,glosario,faq}.html`.
- Fase 4 — **Proyecto**: `proyectos/<proyecto>/` real y ejecutable (si el curso lo pide).
- Fase 5 — **Academy**: tarjeta en la portada y contador del pie.
- Verificación: §4.6.

---

# 14. Reglas importantes

1. Este contrato común es de **mecánica**; el contenido se rige por la spec de cada curso.
2. Nunca duplicar skills por curso: viven en `Academy/.agents/skills/`.
3. La web del curso nunca lleva build ni Markdown; los proyectos de `proyectos/` sí pueden llevarlo.
4. `course-data.js` y el disco no pueden divergir (sin lecciones huérfanas en ningún sentido).
5. Claves de `localStorage` y firma de portada: exclusivas de cada curso.
6. Todo código mostrado debe ejecutarse con las versiones fijadas por el curso.
7. Antes de dar una lección por terminada, ejecutar la verificación §4.6.
8. Si una skill de dominio choca con la spec del curso, gana la spec y se señala el conflicto.
