/**
 * course-data.js — MAPA COMPLETO DEL CURSO JAVASCRIPT MODERNO
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
  title: "Curso JavaScript Moderno",
  subtitle: "De cero a profesional · ES2025+ con proyectos reales",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de JavaScript" },
    { file: "recursos/glosario.html", title: "Glosario de JavaScript" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Preparación",
      description: "Qué es JavaScript, cómo montar el entorno de desarrollo y cómo depurar código.",
      lessons: [
        { file: "01-que-es-javascript.html", title: "Qué es JavaScript", minutes: 30 },
        { file: "02-entorno-node-vite-vscode.html", title: "Entorno: Node.js, pnpm, Vite y VSCode", minutes: 35 },
        { file: "03-ejecucion-navegador-node-vite.html", title: "Ejecución en navegador, Node y Vite", minutes: 30 },
        { file: "04-flujo-de-trabajo-debugging.html", title: "Flujo de trabajo, debugging y DevTools", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-fundamentos",
      title: "Fundamentos",
      description: "Variables, tipos primitivos, operadores, strings, arrays, objetos literales y métodos básicos.",
      lessons: [
        { file: "01-variables-y-tipos.html", title: "Variables (let, const, var) y tipos primitivos", minutes: 30 },
        { file: "02-operadores-y-coercion.html", title: "Operadores, coerción de tipos y comparaciones", minutes: 30 },
        { file: "03-strings-y-template-literals.html", title: "Strings, template literals y métodos", minutes: 30 },
        { file: "04-arrays.html", title: "Arrays: creación, acceso, métodos básicos", minutes: 30 },
        { file: "05-objetos-literales.html", title: "Objetos literales, propiedades y métodos", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-control-y-funciones",
      title: "Control de Flujo y Funciones",
      description: "Condicionales, bucles, funciones, parámetros, retorno y métodos avanzados de arrays.",
      lessons: [
        { file: "01-condicionales.html", title: "Condicionales (if, switch, ternario) y valores truthy/falsy", minutes: 30 },
        { file: "02-bucles.html", title: "Bucles (for, while, do-while, for...of, for...in)", minutes: 30 },
        { file: "03-funciones.html", title: "Funciones declaradas, expresadas y arrow functions", minutes: 30 },
        { file: "04-parametros-y-retorno.html", title: "Parámetros (default, rest, spread) y retorno", minutes: 30 },
        { file: "05-metodos-arrays-avanzados.html", title: "Métodos de arrays avanzados (map, filter, reduce)", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-scope-y-closures",
      title: "Scope, Closures y Prog. Funcional",
      description: "Scope, hoisting, TDZ, closures, funciones de orden superior y programación funcional.",
      lessons: [
        { file: "01-scope.html", title: "Scope global, de función y de bloque", minutes: 30 },
        { file: "02-hoisting-y-tdz.html", title: "Hoisting y TDZ (Temporal Dead Zone)", minutes: 30 },
        { file: "03-closures-y-hof.html", title: "Closures y funciones de orden superior", minutes: 30 },
        { file: "04-inmutabilidad-y-composicion.html", title: "Inmutabilidad, pureza y composición funcional", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-objetos-y-clases",
      title: "Objetos y Clases",
      description: "Prototipos, clases ES6, herencia, propiedades privadas y el comportamiento de this.",
      lessons: [
        { file: "01-prototipos.html", title: "Prototipos y cadena de prototipos", minutes: 30 },
        { file: "02-clases-es6.html", title: "Clases (ES6): constructor, métodos, getters/setters", minutes: 30 },
        { file: "03-herencia-y-propiedades-privadas.html", title: "Herencia (extends, super) y propiedades privadas (#)", minutes: 30 },
        { file: "04-this-en-profundidad.html", title: "this en profundidad: bind, call, apply, arrow functions", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-dom-y-eventos",
      title: "DOM y Eventos",
      description: "Estructura del DOM, manipulación de nodos, sistema de eventos, formularios y almacenamiento.",
      lessons: [
        { file: "01-dom-estructura-y-seleccion.html", title: "El DOM: estructura, nodos, selección", minutes: 30 },
        { file: "02-manipulacion-dom.html", title: "Manipulación: crear, modificar, mover y eliminar nodos", minutes: 30 },
        { file: "03-eventos.html", title: "Eventos: addEventListener, propagación, delegación", minutes: 30 },
        { file: "04-formularios.html", title: "Formularios: inputs, validación, submit, FormData", minutes: 30 },
        { file: "05-almacenamiento.html", title: "Almacenamiento: localStorage, sessionStorage, cookies", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-asincronia-y-apis",
      title: "Asincronía y APIs",
      description: "Callbacks, Promises, async/await, Fetch API y cancelación de peticiones.",
      lessons: [
        { file: "01-callbacks.html", title: "Callbacks y el problema del callback hell", minutes: 30 },
        { file: "02-promises.html", title: "Promises: then, catch, finally, Promise.all, Promise.race", minutes: 30 },
        { file: "03-async-await.html", title: "async/await, try/catch y manejo de errores asíncrono", minutes: 30 },
        { file: "04-fetch-api.html", title: "Fetch API: GET, POST, headers, response", minutes: 30 },
        { file: "05-abortcontroller.html", title: "AbortController, timeouts y cancelación de peticiones", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-modulos-y-testing",
      title: "Módulos, Tooling y Testing",
      description: "Módulos ES, pnpm, package.json, Vitest y debugging avanzado.",
      lessons: [
        { file: "01-modulos-es.html", title: "Módulos ES (import/export), CommonJS vs ESM", minutes: 30 },
        { file: "02-pnpm-package-json.html", title: "npm/pnpm, package.json, dependencias, scripts", minutes: 30 },
        { file: "03-vitest.html", title: "Introducción a Vitest: test, describe, expect, mocks", minutes: 30 },
        { file: "04-debugging-avanzado.html", title: "Debugging avanzado: breakpoints, watch, call stack, performance", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 8 */
    {
      id: "08",
      dir: "modulos/08-web-apis-y-produccion",
      title: "Web APIs y Producción",
      description: "Web APIs nativas, seguridad, rendimiento y accesibilidad en aplicaciones reales.",
      lessons: [
        { file: "01-web-apis.html", title: "Web APIs: Geolocation, Notifications, Clipboard, History", minutes: 30 },
        { file: "02-seguridad.html", title: "Seguridad: XSS, CORS, CSP, sanitización", minutes: 30 },
        { file: "03-rendimiento.html", title: "Rendimiento: lazy loading, debounce/throttle, Web Workers", minutes: 30 },
        { file: "04-accesibilidad.html", title: "Accesibilidad (a11y): ARIA, focus, contraste, screen readers", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 9 */
    {
      id: "09",
      dir: "modulos/09-proyecto-final",
      title: "Proyecto Final: TaskFlow Vanilla",
      description: "SPA completa sin frameworks, CRUD de tareas con Web Storage, integración con API REST, testing y despliegue.",
      lessons: [
        { file: "01-arquitectura-spa.html", title: "Arquitectura SPA sin frameworks: sistema de rutas y estado", minutes: 45, project: true },
        { file: "02-crud-tareas.html", title: "CRUD de tareas con Web Storage + DOM", minutes: 45, project: true },
        { file: "03-integracion-api.html", title: "Integración con API REST (Fetch, loading, errores)", minutes: 45, project: true },
        { file: "04-testing-y-despliegue.html", title: "Testeo, build de producción y despliegue", minutes: 50, project: true },
      ],
    },
  ],
};
