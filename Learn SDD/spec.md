# Página web estática para aprender Spec-Driven Development.

# Curso de Spec-Driven Development (SDD)

## De la intención al código — specs que un humano y un agente pueden ejecutar

---

# 1. Objetivo

Actúa como un ingeniero senior de producto/plataforma que lleva años escribiendo especificaciones que sobreviven al chat, al ticket y al agente de turno.

El objetivo es crear **un curso completo de Spec-Driven Development**, pensado para un **desarrollador que ya programa** (y probablemente ya usa un agente de código: OpenCode, Cursor, Copilot, Claude Code…) pero quiere dejar de “pedir cosas en el chat” y pasar a **trabajar con una spec como fuente de verdad**.

El curso debe ser **extremadamente didáctico** y orientado al **por qué**.

No quiero un recetario de prompts. Quiero entender:

- **qué es una spec** (y qué no es: no es un ticket, no es un README, no es un prompt de una sola vez);
- **qué problema resuelve** ahora que el código lo escribe a menudo un agente;
- **cómo se escribe** una spec ejecutable (objetivos, no-objetivos, restricciones, criterios de aceptación);
- **cómo se usa** en el ciclo specify → plan → implement → verify;
- **cómo se mantiene viva** cuando el producto cambia;
- **cuándo no especificar** (over-spec, teatro de documentación).

El alumno debe terminar el curso **sabiendo escribir una spec que otro agente (u otro humano) pueda implementar sin adivinar**.

---

# 2. Audiencia y requisitos previos

- Desarrolladores/as que ya programan en cualquier lenguaje.
- Se asume: Git básico, Markdown, una terminal, haber usado (aunque sea poco) un asistente de código.
- **No** se asume: haber escrito specs formales, conocer OpenAPI, ADRs, BDD/Gherkin ni herramientas concretas (Spec Kit, OpenSpec, Kiro…).
- Toda la teoría se acompaña de **specs reales** y de un producto de ejemplo que se especifica e implementa de forma incremental.

---

# 3. Hilo conductor: Lumen

Todo el curso gira alrededor de **un caso de estudio real** que se construye spec-first:

> **Lumen** — una bandeja de notas mínima (inbox) con tres piezas:
>
> 1. `spec/` — las especificaciones del producto (fuente de verdad).
> 2. `cli` — una CLI en Node.js que crea, lista y cierra notas (archivo JSON local).
> 3. `web` — una UI estática mínima que lee el mismo almacén (opcional en el proyecto final).

El alumno verá el mismo sistema evolucionar:

- **Módulo 0–1:** se escribe la spec de Lumen (objetivo, no-objetivos, glosario, aceptación).
- **Módulo 2–3:** se convierte esa spec en un plan y en instrucciones para un agente (`AGENTS.md`, skills).
- **Módulo 4:** se añaden contratos (esquema JSON de una nota, ADR de almacenamiento).
- **Módulo 5:** se especifica un **cambio** (no greenfield): etiquetas y búsqueda.
- **Módulo 6:** se revisan anti-patrones sobre la propia spec de Lumen.
- **Módulo 7:** proyecto final: spec + plan + implementación verificable de Lumen.

Poder ver el mismo producto pasar de “párrafo en un chat” a “spec versionada + código que la cumple” cierra el círculo.

**Meta-lección permanente:** esta Academy *ya* es Spec-Driven (`spec.md` / `AGENTS.md` / skills). El curso debe señalarlo cuando ayude, sin volverse autorreferencial en exceso.

---

# 4. Tecnologías y herramientas

## Utilizar SIEMPRE

- **Markdown** como formato canónico de specs (`spec.md`, `AGENTS.md`, `SKILL.md`, ADRs).
- **Git** para versionar specs igual que el código (la spec vive en el repo).
- **Node.js (LTS)** + JavaScript vanilla para el CLI de Lumen (sin frameworks).
- Un **agente de código** genérico: los ejemplos se escriben para **OpenCode** (porque es el entorno de esta Academy) y se comparan con el mismo patrón en otros agentes (`AGENTS.md` ≈ `CLAUDE.md` ≈ reglas de Cursor).
- **JSON Schema** (o un `schema.json` mínimo) como contrato de datos de Lumen.
- **ADRs** cortos en `spec/adr/`.
- VSCode / el editor que ya use el alumno.

## Mencionar (comparativa, no requisito)

- GitHub **Spec Kit** (`specify` / `plan` / `tasks` / `implement`).
- **OpenSpec** y herramientas similares de “spec como artefacto”.
- **OpenAPI** como spec ejecutable de APIs (una lección, no un curso de OpenAPI).
- **TDD** y **BDD/Gherkin** como *primos*: tests y escenarios también son specs; SDD no los sustituye, los ordena.
- IDEs spec-first (Kiro y similares): existían al escribir el curso; no bloquear el contenido a un producto concreto.

## No utilizar

- Un SaaS de pago como requisito para seguir el curso.
- Un LLM concreto como único camino (los ejemplos deben funcionar descritos en Markdown aunque el alumno no tenga API key).
- Frameworks frontend pesados para Lumen (nada de React/Vue; HTML+JS vanilla si hay UI).
- npm como gestor preferido de la *web del curso* (usar **pnpm**, igual que el resto de Academy). El CLI de Lumen puede usar el `package.json` mínimo que haga falta.
- Prompts mágicos de una línea como “la spec”.

---

# 5. Forma de enseñar

Cada lección debe seguir **exactamente** esta estructura (numeración visible en el HTML con la clase `section-number`).

## 1. Introducción
Qué vamos a aprender. Por qué existe. Qué problema resuelve. Cuándo se utiliza. Errores típicos de esa pieza.

## 2. Teoría
Explicar el concepto con mucho detalle, **sin asumir conocimientos previos de SDD**. Comparaciones del mundo real (planos de un edificio vs “dile al albañil en el pasillo”; receta vs “hazme la cena”; contrato vs apretón de manos). Explicar el **porqué**. Diagramas ASCII del ciclo specify → plan → implement → verify cuando ayuden.

## 3. Ejemplo mínimo
La spec o fragmento más pequeño posible (10–20 líneas de Markdown, un criterio de aceptación, un ADR de 8 líneas). Explicar **línea por línea**.

## 4. Ejemplo real
El mismo concepto aplicado a **Lumen** o a un escenario profesional (un cambio en producción, un agente que se desvía, una spec ambigua). Explicar todas las decisiones.

## 5. Buenas prácticas
Qué hacer / qué evitar / por qué. Formato tabla cuando aplique.

## 6. Errores habituales
Ejemplos incorrectos (specs vagas, prompts disfrazados de spec, specs que no se pueden verificar). Por qué están mal. Versión correcta.

## 7. Ejercicio guiado
Ejercicio paso a paso, no demasiado difícil, resuelto al detalle, **con el resultado esperado** (un fichero `spec/` o un criterio comprobable).

## 8. Reto
Reto adicional **sin resolverlo** en el HTML (pista incluida).

## 9. Resumen
Bullets + conceptos clave. Tabla “Artefacto | Para qué sirve” cuando aplique.

## 10. Preguntas frecuentes
3–5 `<details>` relacionados con la lección.

## Reglas de contenido
- Toda spec de ejemplo debe ser **lo bastante concreta como para implementar** (verbos observables, no “que quede bonito”).
- Cuando se muestre un prompt a un agente, mostrar **también** la spec de la que sale. El prompt no sustituye a la spec.
- Código (CLI Lumen, JSON Schema, HTML) **comentado en español**; identificadores en inglés.
- Si un concepto depende de otro, enlazar a la lección.

---

# 6. Multiplataforma

SDD es sobre todo Markdown + Git: **cambia poco entre OS**. Aun así:

- Rutas de ejemplo en formato Linux (`spec/lumen.md`); callout Windows (`spec\lumen.md`) cuando se peguen en el Explorador.
- Git y editores: LF (`.gitattributes` `* text=auto eol=lf`) para que las specs no se llenen de CRLF.
- El CLI de Lumen se ejecuta con `node` en Linux, macOS y Windows (PowerShell o WSL). Si un comando difiere (`$env:VAR` vs `$VAR`), callout.
- No hace falta Docker ni WSL para este curso. Si el alumno está en Windows, Node nativo basta.

---

# 7. Índice del curso

## Módulo 0 — Preparación: por qué SDD ahora
- **01-que-es-sdd.html** — Qué es Spec-Driven Development. El problema: el chat se evapora, el código queda, la intención se pierde. SDD = la intención vive en un artefacto versionado.
- **02-specs-vs-prompts-vs-tickets.html** — Spec vs prompt vs ticket vs README vs “lo hablamos”. Tabla de diferencias. Cuándo cada uno basta.
- **03-entorno-git-agente.html** — Entorno: Git, Markdown, un agente de código (OpenCode como ejemplo), `AGENTS.md` como puerta de entrada.
- **04-caso-lumen.html** — Caso de estudio Lumen: qué vamos a especificar, qué queda fuera, cómo se verá al final.

## Módulo 1 — Anatomía de una spec
- **01-objetivo-y-no-objetivos.html** — Objetivo, audiencia, alcance y **no-objetivos** (lo más infravalorado).
- **02-restricciones-y-glosario.html** — Restricciones técnicas/de negocio, glosario (lenguaje ubicuo), supuestos explícitos.
- **03-criterios-de-aceptacion.html** — Criterios de aceptación verificables (Given/When/Then *sin* exigir Gherkin; “se puede demostrar que…”).
- **04-que-no-va-en-una-spec.html** — Diseño de implementación prematuro, prosa de relleno, secretos, decisiones sin dueño.

## Módulo 2 — El ciclo specify → plan → implement → verify
- **01-el-ciclo-sdd.html** — Las cuatro fases, quién decide qué, por qué no se salta el plan.
- **02-la-spec-como-fuente-de-verdad.html** — La spec gana al chat y al “el código es la spec”. Cómo se detecta divergencia.
- **03-plan-antes-de-codigo.html** — Del spec al plan (pasos, riesgos, orden, fuera de alcance). Plan mode vs implementar.
- **04-verificar-contra-la-spec.html** — Definition of done = la spec, no “compila”. Checklist de verificación.

## Módulo 3 — Specs para agentes
- **01-agents-md.html** — `AGENTS.md` / equivalentes: instrucciones persistentes vs el hilo de hoy.
- **02-skills.html** — Skills (`SKILL.md`): specs reutilizables de *cómo* hacer un tipo de trabajo. Relación skill ↔ spec de producto.
- **03-contexto-y-limites.html** — Qué meter en contexto (y qué no). Alucinaciones por spec ausente vs spec contradictoria.
- **04-humano-en-el-bucle.html** — Dónde se revisa (spec, plan, diff). El agente no es el product owner.

## Módulo 4 — Contratos y specs técnicas
- **01-contratos-de-datos.html** — JSON Schema (Lumen: una Nota). La spec de datos se puede validar.
- **02-adr.html** — Architecture Decision Records: una decisión, un contexto, consecuencias. Cuándo abrir un ADR.
- **03-tdd-bdd-y-sdd.html** — Relación con TDD y BDD. Los tests son specs ejecutables; no sustituyen el “qué y por qué”.
- **04-openapi-como-spec.html** — OpenAPI en una lección: contrato de API como spec que genera clientes y valida. Comparativa, no tutorial completo.

## Módulo 5 — Cambios (el 90 % del trabajo real)
- **01-specs-de-delta.html** — Specs de cambio: estado actual, estado deseado, impacto, migración. No reescribir el mundo.
- **02-compatibilidad-y-roturas.html** — Compatibilidad hacia atrás, flags, “esto rompe X”. Cómo se escribe en la spec.
- **03-review-de-specs.html** — Revisar una spec (ambigüedad, no-verificable, alcance hinchado) antes de implementar.
- **04-cuando-la-spec-esta-mal.html** — Actualizar la spec vs parchear el código a ciegas. El historial de Git de la spec.

## Módulo 6 — Oficio y anti-patrones
- **01-over-spec-y-under-spec.html** — Demasiada spec (parálisis) vs demasiado poca (el agente inventa). Cómo calibrar.
- **02-vibe-coding-vs-sdd.html** — Exploración rápida vs entrega. SDD no mata el prototipo; acota cuándo se promociona.
- **03-spec-theatre.html** — Documentos que nadie lee, specs de 40 páginas, “aprobado” sin criterios. Cómo detectarlo.
- **04-equipo-humano-agente.html** — Convenciones de equipo: quién escribe la spec, quién implementa, quién verifica.

## Módulo 7 — Proyecto final: Lumen end-to-end
- **01-escribir-la-spec.html** — Spec completa de Lumen (inbox de notas): objetivo, no-objetivos, modelo, CLI, aceptación.
- **02-plan-de-implementacion.html** — Plan a partir de la spec: orden, ficheros, riesgos, fuera de alcance.
- **03-implementar-y-verificar.html** — Implementar el CLI (y UI mínima si cabe) contra la spec. Verificar cada criterio.
- **04-cambio-y-cierre.html** — Spec de delta (etiquetas o búsqueda), re-plan, re-implementar, retrospectiva SDD.

## Recursos
- **cheatsheet.html** — Plantilla de spec, plantilla de ADR, plantilla de skill, ciclo SDD.
- **glosario.html** — Spec, no-objetivo, criterio de aceptación, ADR, skill, fuente de verdad, delta…
- **faq.html** — FAQ general del curso.

**Total: 8 módulos (0–7), 32 lecciones + 3 recursos.**

---

# 8. Estructura de repositorio (la que debe crear el agente de implementación)

```text
Learn SDD/
│
├── spec.md                      ← ★ ESTE DOCUMENTO (fuente de verdad del curso)
├── AGENTS.md                    ← puntero corto a spec.md + reglas para agentes
├── package.json                 ← script serve
├── pnpm-workspace.yaml
├── .gitignore
├── .gitattributes               ← LF
│
│   (skills centralizadas en `Academy/.agents/skills/`: documentation,
│    frontend-design, academy-course, spec-driven-development)
│
├── web/                         ← ★ EL CURSO (web estática, sin build)
│   ├── index.html
│   ├── assets/css/site.css      ← identidad SDD (ver diseño)
│   ├── assets/js/course-data.js ← mapa único
│   ├── assets/js/highlight.js
│   ├── assets/js/site.js        ← chrome; localStorage `sdd-theme` / `sdd-progress`
│   ├── assets/fonts/            ← JetBrains Mono (copiar de Learn Javascript)
│   ├── assets/favicon.svg
│   ├── modulos/
│   │   ├── 00-preparacion/
│   │   ├── 01-anatomia-de-una-spec/
│   │   ├── 02-ciclo-sdd/
│   │   ├── 03-specs-para-agentes/
│   │   ├── 04-contratos-y-specs-tecnicas/
│   │   ├── 05-cambios/
│   │   ├── 06-oficio-y-anti-patrones/
│   │   └── 07-proyecto-final/
│   └── recursos/
│
├── scripts/serve-web.mjs
│
└── proyectos/
    └── lumen/                   ← ★ PRODUCTO DEL CURSO
        ├── README.md
        ├── spec/
        │   ├── lumen.md         ← spec de producto
        │   ├── changelog.md     ← deltas
        │   └── adr/
        │       └── 0001-almacen-json.md
        ├── schema/note.schema.json
        ├── scripts/
        │   ├── validate-notes.mjs  ← valida el almacén contra el esquema
        │   └── validate-api.mjs    ← valida un contrato OpenAPI mínimo
        ├── src/                 ← CLI Node (implementación que CUMPLE la spec)
        └── package.json
```

---

# 9. Formato de entrega

Igual que el resto de Academy: **web estática** (HTML + CSS + JS vanilla, **sin Markdown en las lecciones y sin build**).

- `web/` es lo navegable.
- Cada lección = un HTML completo.
- El chrome lo inyecta `web/assets/js/site.js` desde `web/assets/js/course-data.js` (**única fuente de verdad** de navegación).
- Copiar chrome de `Learn Javascript` o `Learn Docker` y adaptar identidad + claves de storage.
- El proyecto `proyectos/lumen/` es **ejecutable de verdad** (`node` CLI).

## Anatomía de una lección

```html
<!DOCTYPE html>
<html lang="es" data-theme="dark" data-module="01" data-lesson="03-criterios-de-aceptacion">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Criterios de aceptación — Módulo 1 · Curso Spec-Driven Development</title>
  <script>document.documentElement.dataset.theme = localStorage.getItem('sdd-theme') || 'dark';</script>
  <link rel="stylesheet" href="../../assets/css/site.css">
  <link rel="icon" href="../../assets/favicon.svg" type="image/svg+xml">
</head>
<body>
  <main>
    <article class="lesson">
      <header class="lesson-header">
        <h1>Criterios de aceptación</h1>
        <div class="lesson-header__meta">
          <span class="badge">Módulo 1 · Anatomía de una spec</span>
          <span>⏱ 30 minutos</span>
        </div>
      </header>
      <section id="introduccion">…</section>
      <!-- 10 secciones con ids fijos: introduccion, teoria, ejemplo-minimo,
           ejemplo-real, buenas-practicas, errores, ejercicio, reto, resumen, faq -->
    </article>
  </main>
  <script>window.SITE_BASE = '../../';</script>
  <script src="../../assets/js/course-data.js"></script>
  <script src="../../assets/js/highlight.js"></script>
  <script src="../../assets/js/site.js"></script>
</body>
</html>
```

**SITE_BASE:** `'./'` en portada · `'../'` en `web/recursos/` · `'../../'` en lecciones.

**data-lesson** = nombre de archivo **sin** `.html`.

Clases: `callout callout--why/--tip/--note/--warning/--analogy`, `challenge`, `faq details`, `table`, `ol.steps`, `pre > code.language-*`.

---

# 10. Diseño y marca

- Tema oscuro por defecto, toggle a claro.
- **No** copiar el azul Docker ni el look genérico “IA” (crema + serif, negro + verde ácido, periódico). Ver skill `frontend-design`.
- Identidad propuesta (el agente de implementación puede afinarla, no sustituirla por un default):
  - Acento: tinta de plano / ámbar de resaltador de spec `#d97706` sobre fondo `#111114`.
  - Firma de portada: un **documento spec que se escribe solo** (líneas `# Objetivo`, `- No-objetivo:`, `Dado/Cuando/Entonces`) — equivalente a la consola JS o la terminal Docker.
- Tipografía: sans de sistema + JetBrains Mono (código y chrome), como el resto de Academy.
- localStorage: `sdd-theme`, `sdd-progress`.
- UI y comentarios en **español**; identificadores en **inglés**.

---

# 11. Convenciones

1. Idioma: contenido, comentarios y UI en español.
2. Identificadores, nombres de ficheros de producto y comandos en inglés.
3. Producto de muestra: **Lumen**; CLI `lumen`; almacén `data/notes.json`.
4. Cada lección usa SIEMPRE las 10 secciones.
5. Los retos no se resuelven en el HTML.
6. En el Resumen, tabla “Artefacto | Para qué sirve” o “Comando | Qué hace”.
7. No vender un vendor: SDD es el método; las herramientas son ejemplos.
8. Consultar docs oficiales (Spec Kit, OpenAPI, JSON Schema) vía Context7 / skill `documentation` antes de afirmar flags o versiones.
9. Priorizar comprensión profunda frente a velocidad.

---

# 12. Documentación actualizada

Antes de afirmar flujos de Spec Kit, campos de OpenAPI o JSON Schema, consultar la documentación oficial vía **Context7** (skill `documentation`). Preferir la versión estable más reciente. Si una herramienta cambió de nombre o se abandonó, decirlo y enseñar el patrón (spec versionada + plan + verify), no el binario.

---

# 13. Página principal (`web/index.html`)

Adaptar la plantilla de JavaScript/Docker:

- Hero + elemento firma (spec que se teclea).
- Lead: curso en español de SDD para quien ya programa y usa agentes.
- “Cómo funciona este curso” (10 secciones, Lumen, sin nube de pago).
- Estadísticas vía `site.js`.
- Índice `#module-list`.
- Botón continuar.

---

# 14. Checklist de implementación (para el agente que construya el curso)

Este `spec.md` y las skills **ya están hechos**. El siguiente agente NO debe reescribir la spec salvo corrección puntual; debe **implementar**.

## Fase 0 — Spec y skills (este paso) ✅
- [x] `spec.md`
- [x] Skills: `documentation`, `frontend-design`, `academy-course`, `spec-driven-development` (centralizadas en `Academy/.agents/skills/`)
- [x] `AGENTS.md` corto que apunta aquí
- [x] Context7 MCP: `opencode.json` en la raíz de la Academy

## Fase 1 — Scaffold ✅
- [x] `package.json`, `pnpm-workspace.yaml`, `.gitignore`, `.gitattributes`
- [x] Copiar `scripts/serve-web.mjs` (mensaje “Curso Spec-Driven Development”)
- [x] Copiar fonts + highlight.js desde Learn Javascript
- [x] `course-data.js` con el mapa de la sección 7
- [x] `site.js` con `sdd-theme` / `sdd-progress` y firma de portada tipo spec
- [x] `site.css` con identidad de la sección 10
- [x] `web/index.html` + favicon

## Fase 2 — 32 lecciones HTML ✅
- [x] Módulos 00–07 según índice, 10 secciones cada una, Lumen como hilo
- [x] Verificación estructural 32/32 (data-module, data-lesson, 10 ids, SITE_BASE, `sdd-theme`) + smoke HTTP 17/17

## Fase 3 — Recursos ✅
- [x] cheatsheet, glosario, faq

## Fase 4 — Proyecto Lumen ✅
- [x] `proyectos/lumen/spec/lumen.md` alineado con las lecciones
- [x] ADR + JSON Schema
- [x] CLI Node ejecutable que cumple la spec (verificado: add/list/close/--help + 2 casos de error)
- [x] README de verificación (`node src/cli.js --help`, crear/listar/cerrar nota)

## Fase 5 — Academy ✅
- [x] Tarjeta en `Academy/index.html`
- [x] Footer N cursos (9 → 10)

---

# 15. Reglas importantes

1. Nunca des por sentado un concepto.
2. Explica siempre el “por qué” de cada artefacto (spec, plan, skill, ADR).
3. Toda spec de ejemplo debe poder implementarse; todo código de Lumen debe poder ejecutarse con Node.
4. La spec del **curso** es este archivo; no la contradigas en las lecciones.
5. No conviertas el curso en un tutorial de un único producto comercial.
6. No omitas el ciclo de verificación: una spec sin “cómo sé que está hecho” está incompleta.
7. Progresión lógica: anatomía → ciclo → agentes → contratos → cambios → oficio → proyecto.
8. Si un concepto depende de otro, enlaza.
9. Prioriza profundidad frente a velocidad.
10. El agente de implementación debe leer las skills `academy-course` y `spec-driven-development` (en `Academy/.agents/skills/`) **antes** de escribir HTML.
