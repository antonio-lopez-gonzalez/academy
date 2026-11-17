/**
 * course-data.js — MAPA COMPLETO DEL CURSO RUST
 *
 * Única fuente de verdad para:
 *  - el sidebar (árbol de módulos y lecciones)
 *  - la navegación anterior / siguiente
 *  - el índice de la portada
 *  - el cálculo de progreso
 *
 * Cada lección apunta a un archivo HTML real dentro de web/modulos/.
 * Si añades o renombras una lección, edita SOLO este archivo.
 */
window.COURSE = {
  title: "Aprende Rust",
  subtitle: "Del préstamo de datos a un CLI y un servidor en producción",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de Rust" },
    { file: "recursos/glosario.html", title: "Glosario de términos" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-fundamentos",
      title: "Fundamentos",
      description: "Qué es Rust, el toolchain, Cargo y las variables: sin prisa y sin magia.",
      lessons: [
        { file: "01-que-es-rust.html", title: "Qué es Rust y por qué existe", minutes: 30 },
        { file: "02-instalacion-y-toolchain.html", title: "Instalación: rustup, cargo y rust-analyzer", minutes: 35 },
        { file: "03-cargo-build-run-test.html", title: "Cargo: build, run, test, fmt y clippy", minutes: 35 },
        { file: "04-variables-y-tipos.html", title: "Variables, tipos y mutabilidad", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-ownership",
      title: "Ownership y borrowing",
      description: "La propiedad, los préstamos, las vidas y los slices: el corazón del lenguaje.",
      lessons: [
        { file: "01-ownership.html", title: "Ownership: stack, heap, move y drop", minutes: 45 },
        { file: "02-borrowing.html", title: "Borrowing: &T, &mut T y el borrow checker", minutes: 45 },
        { file: "03-lifetimes.html", title: "Lifetimes: por qué el compilador pide anotaciones", minutes: 40 },
        { file: "04-slices-y-str.html", title: "Slices, str y String", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-tipos",
      title: "Tipos y abstracciones",
      description: "Structs, enums, traits, generics, colecciones e iteradores.",
      lessons: [
        { file: "01-structs-y-impl.html", title: "Structs, métodos y funciones asociadas", minutes: 40 },
        { file: "02-enums-option-result.html", title: "Enums, Option, Result y pattern matching", minutes: 45 },
        { file: "03-traits-y-generics.html", title: "Traits, generics y trait bounds", minutes: 45 },
        { file: "04-colecciones-e-iteradores.html", title: "Vec, HashMap, closures e iteradores", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-errores",
      title: "Errores y modularidad",
      description: "Result y el operador ?, errores propios, módulos, crates y tests.",
      lessons: [
        { file: "01-result-y-operador.html", title: "Result, el operador ? y panic!", minutes: 40 },
        { file: "02-errores-propios.html", title: "Errores propios con thiserror", minutes: 40 },
        { file: "03-modulos-y-crates.html", title: "Módulos, crates y workspaces", minutes: 40 },
        { file: "04-tests.html", title: "Tests: unitarios, de integración y doc-tests", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-ecosistema",
      title: "Ecosistema y herramientas",
      description: "Cargo avanzado, rustdoc, Serde, calidad y CI.",
      lessons: [
        { file: "01-cargo-avanzado.html", title: "Cargo.toml, features y semver", minutes: 35 },
        { file: "02-rustdoc.html", title: "Documentación con rustdoc y doctests", minutes: 35 },
        { file: "03-serde.html", title: "Serde: JSON, derive y formatos", minutes: 40 },
        { file: "04-calidad-y-ci.html", title: "Clippy, rustfmt, auditoría y CI", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-concurrencia",
      title: "Concurrencia y async",
      description: "Threads, Send/Sync, canales, async/await y el runtime de Tokio.",
      lessons: [
        { file: "01-threads-y-sync.html", title: "Threads, Send, Sync, Mutex y Arc", minutes: 45 },
        { file: "02-canales.html", title: "Canales y trabajo en paralelo", minutes: 40 },
        { file: "03-async-await.html", title: "async/await, futures y el runtime", minutes: 45 },
        { file: "04-tokio-en-practica.html", title: "Tokio: tareas, timeouts y red TCP", minutes: 45 },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-red-y-cli",
      title: "Red y CLI real",
      description: "Clap, reqwest, Axum y persistencia: el CLI se encuentra con la red.",
      lessons: [
        { file: "01-clap.html", title: "CLI con clap: subcomandos y validación", minutes: 40 },
        { file: "02-reqwest.html", title: "HTTP con reqwest", minutes: 40 },
        { file: "03-axum.html", title: "Servidor HTTP con Axum", minutes: 45 },
        { file: "04-persistencia.html", title: "Persistencia: ficheros, SQLite y migraciones", minutes: 45 },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-proyecto-final",
      title: "Proyecto final",
      description: "Construir urlshort: dominio, CLI, servidor HTTP, tests y publicación.",
      lessons: [
        { file: "01-arquitectura-urlshort.html", title: "Arquitectura de urlshort", minutes: 40, project: true },
        { file: "02-comandos-cli.html", title: "Comandos: short, resolve, list y delete", minutes: 45, project: true },
        { file: "03-servidor-y-tests.html", title: "Servidor HTTP y tests", minutes: 45, project: true },
        { file: "04-publicacion.html", title: "Publicación en crates.io y release", minutes: 40, project: true },
      ],
    },
  ],
};
