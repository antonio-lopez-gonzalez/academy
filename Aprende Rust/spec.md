# Curso Completo de Rust

## Del préstamo de datos a un CLI y un servidor en producción

---

# 1. Objetivo

Actúa como un ingeniero de plataforma que escribe software de sistemas y de servicios en Rust. El objetivo es crear **un curso completo de Rust en español**, pensado para quien **ya sabe programar** en otro lenguaje y quiere dominar el lenguaje de verdad: no memorizar `match` y `unwrap`, sino **entender por qué existe cada regla** (propiedad, préstamos, vidas, traits, async) y **cómo se escribe software que se mantiene** (tests, errores, documentación, CI, publicación).

El curso enseña el **por qué**:

- La **propiedad** no es una molestia: es el mecanismo que sustituye al recolector de basura sin perder seguridad.
- Los **préstamos** codifican quién lee y quién escribe, y el compilador demuestra que no se solapan.
- Las **vidas (lifetimes)** no son ruido: son la forma de decir cuánto vive un préstamo cuando el compilador no puede deducirlo.
- Los **traits** son interfaces con implementaciones estáticas: cero coste, sin herencia de clases.
- `Result` y `Option` convierten los fallos y la ausencia en **tipos que el compilador obliga a tratar**.
- `async` es una máquina de estados que el compilador genera; entenderla evita los bloqueos y los `Arc<Mutex<_>>` innecesarios.

El hilo conductor es **urlshort**: una herramienta de terminal que acorta URLs, las guarda, las resuelve y las sirve por HTTP. Crece con el curso y termina siendo un crate publicado.

> Stack fijado: **Rust estable (edición 2021)** con `rustup`, `cargo`, `clippy`, `rustfmt` y `rust-analyzer`; crates `clap` 4, `serde` 1, `serde_json` 1, `thiserror` 2, `tokio` 1 y `axum` 0.8. Proyecto final en `proyectos/urlshort/`.

---

# 2. Audiencia y requisitos previos

- **Saber programar** en cualquier lenguaje (variables, funciones, bucles, estructuras de datos).
- Manejo básico de **terminal** y de un editor con LSP (VS Code recomendado).
- No se asume experiencia con lenguajes compilados ni con gestión manual de memoria; se explica cuando aparece.
- No se asume experiencia previa con concurrencia ni con HTTP.
- Se agradece `Aprende Docker` para la parte de publicación y contenedores, pero **no es requisito**.

---

# 3. Hilo conductor: urlshort

Una herramienta que evoluciona con el curso:

1. **Un binario mínimo** que lee la URL de los argumentos y deriva un código corto.
2. **Un dominio real**: `Link { code, url, created_at, hits }` con `struct`, `enum` y traits.
3. **Errores propios** con `thiserror`, sin `unwrap` en el camino del usuario.
4. **Tests** unitarios, de integración y doctests.
5. **Persistencia** en JSON y, más adelante, SQLite.
6. **CLI profesional** con `clap` (subcomandos `short`, `resolve`, `list`, `delete`).
7. **Servidor HTTP** con `axum` que reutiliza el mismo dominio.
8. **Publicación** en crates.io con documentación, CI y release.

El proyecto ejecutable vive en `proyectos/urlshort/`.

---

# 4. Tecnologías

## Utilizar SIEMPRE

- **Rust estable** instalado con `rustup` (canal `stable`).
- **Cargo** como herramienta única: build, run, test, fmt, clippy, doc, publish.
- **`rustfmt`** y **`clippy`** en el flujo de trabajo diario y en CI.
- **`rust-analyzer`** como LSP.
- **Serde** para serialización (`derive`), **thiserror** para errores de dominio.
- **Tokio** para async y **Axum** para HTTP.
- **Tests** (`#[test]`, `tests/` de integración, doctests) desde el primer módulo.
- **Edición 2021** en `Cargo.toml`; la 2024 se explica como migración.

## No utilizar

- `unwrap()`/`expect()` en rutas que dependen de entrada del usuario.
- `clone()` para silenciar al borrow checker.
- `unsafe` sin justificación escrita y sin tests.
- `Arc<Mutex<T>>` como respuesta por defecto a cualquier problema de concurrencia.
- Frameworks web en el módulo de fundamentos: primero el lenguaje, luego la red.
- Dependencias sin justificar: cada crate se introduce con su motivo y su alternativa.

---

# 5. Forma de enseñar

Cada lección sigue EXACTAMENTE las 10 secciones del contrato Academy:

1. **Introducción** — qué, por qué existe, qué problema resuelve, cuándo se usa, errores típicos.
2. **Teoría** — concepto con detalle, diagramas ASCII, comparaciones con lenguajes que el alumno ya conoce.
3. **Ejemplo mínimo** — el fragmento más pequeño que compila, explicado línea a línea.
4. **Ejemplo real** — aplicado a urlshort o a un escenario profesional.
5. **Buenas prácticas** — tabla hacer / evitar / por qué.
6. **Errores habituales** — código incorrecto, el error real del compilador y la versión correcta.
7. **Ejercicio guiado** — paso a paso, resuelto, con salida esperada.
8. **Reto** — sin resolver en el HTML, solo pista.
9. **Resumen** — bullets + tabla de comandos clave.
10. **Preguntas frecuentes** — 4–5 `<details>`.

## Reglas de contenido

- Todo bloque de código Rust debe **compilar** con la edición indicada.
- Los errores del compilador se muestran **literales** (código E0382, E0502, E0308…) y se explican.
- `<` y `>` dentro de código van escapados (`&lt;`, `&gt;`) en el HTML.
- Los ejemplos usan `cargo` como vía canónica; `rustc` solo en el primer contacto.
- El reto nunca se resuelve en el HTML.
- Identificadores y comandos en inglés; prosa, comentarios y UI en español.
- Cuando una API pueda haber cambiado, se contrasta con la documentación oficial (Context7 / docs.rs).

---

# 6. Multiplataforma

- **Shell canónico:** bash (Linux, macOS y WSL2). Callout con PowerShell cuando cambie.
- **Windows:** se recomienda **WSL2** + VS Code Remote; también funciona nativo con el instalador de `rustup` y el linker MSVC (se explica en el módulo 0).
- Rutas: `~/.cargo/bin` en la instalación de usuario; en Windows, `%USERPROFILE%\.cargo\bin`.
- `rustup` gestiona toolchains, componentes (`clippy`, `rustfmt`, `rust-src`) y targets.
- Diferencias documentadas: separador de rutas, `./bin` vs `.\bin.exe`, fin de línea LF, permisos de ejecución.

---

# 7. Índice del curso

## Módulo 0 — Fundamentos
- **01-que-es-rust.html** — Qué es Rust, qué problema resuelve y por qué no tiene GC.
- **02-instalacion-y-toolchain.html** — rustup, cargo, rust-analyzer, WSL2 y primer proyecto.
- **03-cargo-build-run-test.html** — Cargo de punta a punta: build, run, test, fmt, clippy.
- **04-variables-y-tipos.html** — Variables, mutabilidad, shadowing, tipos y conversiones.

## Módulo 1 — Ownership y borrowing
- **01-ownership.html** — Stack, heap, move, copy y drop.
- **02-borrowing.html** — `&T`, `&mut T`, reglas del borrow checker.
- **03-lifetimes.html** — Vidas, elisión y por qué el compilador pide anotaciones.
- **04-slices-y-str.html** — Slices, `str` vs `String` y `&[T]`.

## Módulo 2 — Tipos y abstracciones
- **01-structs-y-impl.html** — Structs, `impl`, métodos y funciones asociadas.
- **02-enums-option-result.html** — Enums con datos, `Option`, `Result` y `match` exhaustivo.
- **03-traits-y-generics.html** — Traits, generics, bounds, `impl Trait` y objetos trait.
- **04-colecciones-e-iteradores.html** — `Vec`, `HashMap`, closures e iteradores perezosos.

## Módulo 3 — Errores y modularidad
- **01-result-y-operador.html** — `Result`, `?`, `panic!` y cuándo usar cada uno.
- **02-errores-propios.html** — Errores de dominio con `thiserror` y conversiones `From`.
- **03-modulos-y-crates.html** — `mod`, `pub`, crates, workspaces y visibilidad.
- **04-tests.html** — Tests unitarios, de integración, doctests y TDD.

## Módulo 4 — Ecosistema y herramientas
- **01-cargo-avanzado.html** — `Cargo.toml`, features, semver, lockfile y ediciones.
- **02-rustdoc.html** — Documentación con `///`, doctests y `cargo doc`.
- **03-serde.html** — Serde: derive, JSON, atributos y formatos alternativos.
- **04-calidad-y-ci.html** — Clippy, rustfmt, `cargo audit`, CI en GitHub Actions.

## Módulo 5 — Concurrencia y async
- **01-threads-y-sync.html** — Threads, `Send`, `Sync`, `Mutex`, `RwLock` y `Arc`.
- **02-canales.html** — `mpsc`, productores/consumidores y patrones de reparto.
- **03-async-await.html** — Futures, `async`/`await`, runtime y errores típicos.
- **04-tokio-en-practica.html** — Tareas, timeouts, `select!` y red TCP con Tokio.

## Módulo 6 — Red y CLI real
- **01-clap.html** — CLI con `clap` derive: subcomandos, validación y ayuda.
- **02-reqwest.html** — Cliente HTTP, timeouts, JSON y manejo de errores.
- **03-axum.html** — Servidor HTTP con Axum: rutas, estado, extractores y errores.
- **04-persistencia.html** — Ficheros, SQLite, migraciones y transacciones.

## Módulo 7 — Proyecto final
- **01-arquitectura-urlshort.html** — Dominio, casos de uso y capas.
- **02-comandos-cli.html** — `short`, `resolve`, `list`, `delete` sobre el dominio.
- **03-servidor-y-tests.html** — Servidor HTTP y tests de integración.
- **04-publicacion.html** — Documentación, crates.io, release y Docker.

**Total: 8 módulos, 32 lecciones + 3 recursos.**

---

# 8. Estructura

```text
Aprende Rust/
├── spec.md
├── AGENTS.md
├── package.json
├── .gitattributes
├── .gitignore
├── web/
│   ├── index.html
│   ├── assets/{css,js,fonts,favicon.svg}
│   ├── modulos/00-fundamentos/ … 07-proyecto-final/
│   └── recursos/{cheatsheet,glosario,faq}.html
├── scripts/serve-web.mjs
└── proyectos/urlshort/
    ├── Cargo.toml
    ├── README.md
    ├── src/{main.rs,lib.rs,cli.rs,store.rs,server.rs,error.rs}
    └── tests/
```

---

# 9. Formato de entrega

Web estática (HTML + CSS + JavaScript vanilla, sin build). El chrome (cabecera, sidebar, progreso, tema) lo inyecta `web/assets/js/site.js` a partir de `web/assets/js/course-data.js`, única fuente de verdad.

Anatomía de lección: idéntica a la familia Academy — `<html data-module data-lesson>`, script inline de tema con la clave `rust-theme`, `SITE_BASE = '../../'`, las 10 secciones con sus `id`, y los tres scripts al final.

Claves de `localStorage`: **`rust-theme`** y **`rust-progress`**.

---

# 10. Diseño y marca

- Tema oscuro por defecto (`#0d1117`), toggle a claro.
- Paleta: **óxido `#D1481F`** (acento principal) y **acero `#6EA8FE`** (acento secundario).
- Tipografía: sans del sistema para prosa; JetBrains Mono para código y chrome.
- Favicon y logo de la topbar: engranaje (la rueda del logo de Rust).
- Firma de portada: terminal `cargo` que crea el proyecto, compila, pasa tests y pasa clippy.

---

# 11. Convenciones

1. Español en contenido, comentarios y UI; inglés en identificadores y comandos.
2. `cargo` siempre; `rustc` solo en la primera lección.
3. Edición 2021 en el código del curso; 2024 documentada como migración.
4. Sin `unwrap` en código de ejemplo que simule producción.
5. Cada lección enlaza a la anterior o a la siguiente cuando depende de ella.
6. El reto nunca se resuelve en el HTML.
7. Tabla de comandos clave en el Resumen.
8. Los errores del compilador se reproducen literales, con su código.

---

# 12. Documentación actualizada

Consultar documentación oficial antes de afirmar APIs, comandos o versiones:

- **Rust Book** y **std docs** (`doc.rust-lang.org`).
- **docs.rs** para `clap`, `serde`, `thiserror`, `tokio`, `axum`, `reqwest`, `rusqlite`.
- **Cargo Book** para features, perfiles, publicación y workspaces.
- **Context7 MCP** disponible en la raíz de la Academy para consultar estas fuentes.

Versiones de referencia: Rust 1.85+, edición 2021 (2024 documentada), `clap` 4.6, `serde` 1.0, `thiserror` 2.0, `tokio` 1.54, `axum` 0.8.

---

# 13. Portada

- Hero con eyebrow `// curso-rust · ownership · async · urlshort`.
- Firma: consola `cargo` animada por `buildRepl` en `site.js`.
- Estadísticas calculadas desde `course-data.js` (módulos, lecciones, horas, proyecto).
- Cuatro callouts: estructura de lección, «el compilador es un revisor», para quién es, «sin GC no significa sin ayuda».
- Índice generado en `#module-list`.

---

# 14. Checklist de fases

- [x] Fase 0 — `spec.md` + `AGENTS.md`
- [x] Fase 1 — scaffold (`web/assets`, portada, recursos, scripts, favicon)
- [x] Fase 2 — 32 lecciones en 8 módulos
- [x] Fase 3 — recursos (cheatsheet, glosario, FAQ)
- [x] Fase 4 — `proyectos/urlshort/`
- [x] Fase 5 — registro en `Academy/index.html`, `specs.md` y totales
- [x] Verificación — `academy-verify`, `node --check`, HTTP 200

---

# 15. Reglas importantes

1. Nunca dar por supuesto un concepto: cada regla del lenguaje se explica con su motivo.
2. Todo código debe compilar con la edición declarada.
3. No silenciar errores con `unwrap` en ejemplos «de producción».
4. Mantener la progresión: ownership antes de traits, traits antes de async, async antes de Axum.
5. El proyecto `urlshort` crece con el curso y no se adelanta a las lecciones.
6. Verificar siempre antes de dar por terminado (`academy-verify`, `node --check`, HTTP 200, tarjeta en la portada).
