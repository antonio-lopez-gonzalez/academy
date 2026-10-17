/**
 * course-data.js — MAPA COMPLETO DEL CURSO
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
  title: "Curso React 19.2",
  subtitle: "De cero a avanzado · Full-stack con .NET 8",

  resources: [
    { file: "recursos/glosario.html", title: "Glosario de términos" },
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de React 19" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Preparación",
      description: "Qué es React, cómo piensa y cómo preparar tu entorno profesional.",
      lessons: [
        { file: "01-que-es-react.html", title: "Qué es React e historia", minutes: 25 },
        { file: "02-spa-y-virtual-dom.html", title: "SPA y Virtual DOM", minutes: 30 },
        { file: "03-modelo-de-componentes.html", title: "El modelo mental de componentes", minutes: 25 },
        { file: "04-react-vs-otros.html", title: "React vs Vanilla, Angular, Vue y Solid", minutes: 30 },
        { file: "05-instalacion.html", title: "Instalación: Node, pnpm y VSCode", minutes: 35 },
        { file: "06-primer-proyecto-vite.html", title: "Tu primer proyecto con Vite", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-fundamentos",
      title: "Fundamentos",
      description: "JSX, componentes, props y composición: los ladrillos de React.",
      lessons: [
        { file: "01-jsx-y-babel.html", title: "JSX y el compilador", minutes: 35 },
        { file: "02-componentes-y-reactdom.html", title: "Componentes y ReactDOM", minutes: 35 },
        { file: "03-expresiones-en-jsx.html", title: "Expresiones en JSX", minutes: 25 },
        { file: "04-fragments.html", title: "Fragments", minutes: 20 },
        { file: "05-props.html", title: "Props", minutes: 35 },
        { file: "06-children.html", title: "Children", minutes: 25 },
        { file: "07-composicion.html", title: "Composición", minutes: 30 },
        { file: "08-portals.html", title: "Portals (createPortal)", minutes: 35 },
        { file: "09-proyecto-pagina-perfil.html", title: "🛠 Proyecto: página de perfil", minutes: 45, project: true },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-renderizado",
      title: "Renderizado de listas y condicional",
      description: "map(), keys y renderizado condicional: mostrar datos reales.",
      lessons: [
        { file: "01-listas-con-map.html", title: "Listas con map()", minutes: 30 },
        { file: "02-keys-y-reconciliacion.html", title: "Keys y reconciliación", minutes: 35 },
        { file: "03-renderizado-condicional.html", title: "Renderizado condicional", minutes: 30 },
        { file: "04-proyecto-catalogo-productos.html", title: "🛠 Proyecto: catálogo de productos", minutes: 50, project: true },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-eventos",
      title: "Eventos",
      description: "onClick, onChange, onSubmit y Synthetic Events.",
      lessons: [
        { file: "01-onclick-y-synthetic-events.html", title: "onClick y Synthetic Events", minutes: 35 },
        { file: "02-onchange-y-onsubmit.html", title: "onChange y onSubmit", minutes: 30 },
        { file: "03-paso-de-parametros.html", title: "Paso de parámetros a manejadores", minutes: 25 },
        { file: "04-proyecto-calculadora.html", title: "🛠 Proyecto: calculadora", minutes: 55, project: true },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-estado",
      title: "Estado",
      description: "useState, el ciclo de renderizado y la inmutabilidad.",
      lessons: [
        { file: "01-usestate.html", title: "useState: la memoria del componente", minutes: 40 },
        { file: "02-ciclo-de-renderizado.html", title: "El ciclo de renderizado y el batching", minutes: 35 },
        { file: "03-estado-con-arrays-y-objetos.html", title: "Estado con arrays y objetos", minutes: 35 },
        { file: "04-inmutabilidad.html", title: "Inmutabilidad: por qué es sagrada", minutes: 30 },
        { file: "05-proyecto-lista-de-tareas.html", title: "🛠 Proyecto: lista de tareas", minutes: 60, project: true },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-formularios",
      title: "Formularios",
      description: "Inputs controlados, validaciones y las Form Actions de React 19.",
      lessons: [
        { file: "01-inputs-controlados.html", title: "Inputs controlados", minutes: 35 },
        { file: "02-checkbox-radio-select.html", title: "Checkbox, radio y select", minutes: 30 },
        { file: "03-validaciones.html", title: "Validaciones", minutes: 40 },
        { file: "04-form-actions-react-19.html", title: "Form Actions de React 19", minutes: 35 },
        { file: "05-useformstatus.html", title: "useFormStatus (React 19)", minutes: 30 },
        { file: "06-useactionstate.html", title: "useActionState (React 19)", minutes: 40 },
        { file: "07-proyecto-formulario-registro.html", title: "🛠 Proyecto: formulario de registro", minutes: 60, project: true },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-hooks",
      title: "Hooks en profundidad",
      description: "useEffect, useRef, useMemo, useCallback y custom hooks.",
      lessons: [
        { file: "01-useeffect.html", title: "useEffect: sincronizar con el exterior", minutes: 45 },
        { file: "02-useref.html", title: "useRef", minutes: 30 },
        { file: "03-usememo.html", title: "useMemo", minutes: 30 },
        { file: "04-usecallback.html", title: "useCallback", minutes: 30 },
        { file: "05-custom-hooks.html", title: "Custom hooks", minutes: 35 },
        { file: "06-usetransition.html", title: "useTransition", minutes: 45 },
        { file: "07-useoptimistic.html", title: "useOptimistic (React 19)", minutes: 35 },
        { file: "08-use-hook.html", title: "use() — Promesas y Context", minutes: 40 },
        { file: "09-forwardref-useimperativehandle.html", title: "forwardRef y useImperativeHandle", minutes: 40 },
        { file: "10-use-deferredvalue-y-useid.html", title: "useDeferredValue y useId", minutes: 35 },
        { file: "11-proyecto-buscador-peliculas.html", title: "🛠 Proyecto: buscador de películas", minutes: 65, project: true },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-estilos",
      title: "Estilos",
      description: "Arquitectura CSS, CSS Modules, variables y responsive.",
      lessons: [
        { file: "01-arquitectura-css.html", title: "Arquitectura CSS en React", minutes: 30 },
        { file: "02-css-modules.html", title: "CSS Modules", minutes: 30 },
        { file: "03-variables-css-y-responsive.html", title: "Variables CSS y responsive", minutes: 35 },
        { file: "04-proyecto-dashboard-responsive.html", title: "🛠 Proyecto: dashboard responsive", minutes: 60, project: true },
      ],
    },
    /* ============================================================ MÓDULO 8 */
    {
      id: "08",
      dir: "modulos/08-routing",
      title: "Routing",
      description: "React Router: rutas, parámetros, navegación y rutas protegidas.",
      lessons: [
        { file: "01-react-router-basico.html", title: "React Router: primeras rutas", minutes: 40 },
        { file: "02-parametros-y-rutas-anidadas.html", title: "Parámetros y rutas anidadas", minutes: 35 },
        { file: "03-navigate-y-404.html", title: "Navigate, useNavigate y 404", minutes: 30 },
        { file: "04-rutas-protegidas.html", title: "Rutas protegidas", minutes: 35 },
        { file: "05-proyecto-aplicacion-multipagina.html", title: "🛠 Proyecto: aplicación multipágina", minutes: 65, project: true },
      ],
    },
    /* ============================================================ MÓDULO 9 */
    {
      id: "09",
      dir: "modulos/09-apis",
      title: "Consumo de APIs",
      description: "fetch, loading, errores, AbortController y capa de servicios.",
      lessons: [
        { file: "01-fetch-y-async-await.html", title: "fetch y async/await en React", minutes: 40 },
        { file: "02-loading-y-errores.html", title: "Estados de carga y error", minutes: 35 },
        { file: "03-abortcontroller.html", title: "AbortController y cleanup", minutes: 30 },
        { file: "04-capa-de-servicios.html", title: "La capa de servicios", minutes: 30 },
        { file: "05-suspense-data-fetching.html", title: "Suspense para data fetching", minutes: 40 },
        { file: "06-proyecto-consumir-api-publica.html", title: "🛠 Proyecto: consumir una API pública", minutes: 60, project: true },
      ],
    },
    /* ========================================================== MÓDULO 10 */
    {
      id: "10",
      dir: "modulos/10-estado-global",
      title: "Estado global",
      description: "Context API, useReducer, Redux Toolkit y Zustand comparados.",
      lessons: [
        { file: "01-context-api.html", title: "Context API", minutes: 40 },
        { file: "02-usereducer.html", title: "useReducer", minutes: 40 },
        { file: "03-redux-toolkit.html", title: "Redux Toolkit", minutes: 50 },
        { file: "04-zustand.html", title: "Zustand", minutes: 30 },
        { file: "05-comparativa-estado-global.html", title: "Comparativa: cuándo usar cada uno", minutes: 30 },
        { file: "06-proyecto-carrito-de-compra.html", title: "🛠 Proyecto: carrito de compra", minutes: 70, project: true },
      ],
    },
    /* ========================================================== MÓDULO 11 */
    {
      id: "11",
      dir: "modulos/11-optimizacion",
      title: "Optimización",
      description: "React.memo, lazy, Suspense, code splitting y Profiler.",
      lessons: [
        { file: "01-renders-y-diagnostico.html", title: "Re-renders y cómo diagnosticarlos", minutes: 35 },
        { file: "02-react-memo.html", title: "React.memo", minutes: 30 },
        { file: "03-lazy-suspense-code-splitting.html", title: "lazy, Suspense y code splitting", minutes: 35 },
        { file: "04-profiler-y-devtools.html", title: "Profiler y React DevTools", minutes: 30 },
        { file: "05-react-compiler.html", title: "React Compiler", minutes: 25 },
        { file: "06-error-boundaries.html", title: "Error Boundaries", minutes: 40 },
        { file: "07-document-metadata-y-preloading.html", title: "Document Metadata y preloading", minutes: 35 },
        { file: "08-proyecto-optimizacion-completa.html", title: "🛠 Proyecto: optimización completa", minutes: 60, project: true },
      ],
    },
    /* ========================================================== MÓDULO 12 */
    {
      id: "12",
      dir: "modulos/12-testing",
      title: "Testing",
      description: "Vitest, React Testing Library y mocks.",
      lessons: [
        { file: "01-vitest.html", title: "Vitest: fundamentos", minutes: 35 },
        { file: "02-react-testing-library.html", title: "React Testing Library", minutes: 40 },
        { file: "03-mocks.html", title: "Mocks y spies", minutes: 35 },
        { file: "04-proyecto-tests-del-proyecto.html", title: "🛠 Proyecto: suite de tests", minutes: 60, project: true },
      ],
    },
    /* ========================================================== MÓDULO 13 */
    {
      id: "13",
      dir: "modulos/13-proyecto-final",
      title: "Proyecto final: TaskFlow",
      description: "Aplicación profesional tipo Trello, paso a paso.",
      lessons: [
        { file: "01-arquitectura-y-setup.html", title: "Arquitectura y setup del proyecto", minutes: 45, project: true },
        { file: "02-autenticacion-jwt-cliente.html", title: "Autenticación JWT en el cliente", minutes: 55, project: true },
        { file: "03-dashboard-y-layout.html", title: "Dashboard y layout principal", minutes: 45, project: true },
        { file: "04-crud-proyectos.html", title: "CRUD de proyectos", minutes: 50, project: true },
        { file: "05-crud-tareas.html", title: "CRUD de tareas, etiquetas y prioridades", minutes: 60, project: true },
        { file: "06-comentarios-y-subtareas.html", title: "Comentarios y subtareas", minutes: 45, project: true },
        { file: "07-filtros-busqueda-paginacion.html", title: "Filtros, búsqueda y paginación", minutes: 45, project: true },
        { file: "08-tema-oscuro-y-responsive.html", title: "Tema oscuro y responsive", minutes: 35, project: true },
        { file: "09-websockets-tiempo-real.html", title: "WebSockets: tiempo real", minutes: 50, project: true },
        { file: "10-despliegue-frontend.html", title: "Despliegue del frontend", minutes: 35, project: true },
        { file: "11-server-components-contexto.html", title: "Server Components: contexto y futuro", minutes: 40 },
      ],
    },
    /* ========================================================== MÓDULO 14 */
    {
      id: "14",
      dir: "modulos/14-backend-dotnet",
      title: "Backend con .NET 8",
      description: "API REST completa: EF Core, PostgreSQL, JWT, SignalR y Docker.",
      backend: true,
      lessons: [
        { file: "01-setup-y-arquitectura.html", title: "Setup y arquitectura limpia", minutes: 45, backend: true },
        { file: "02-ef-core-y-postgresql.html", title: "EF Core y PostgreSQL", minutes: 55, backend: true },
        { file: "03-jwt-y-autenticacion.html", title: "JWT y autenticación", minutes: 55, backend: true },
        { file: "04-crud-api.html", title: "CRUD completo de la API", minutes: 60, backend: true },
        { file: "05-signalr-tiempo-real.html", title: "SignalR: WebSockets en .NET", minutes: 45, backend: true },
        { file: "06-validacion-y-errores.html", title: "Validación y manejo de errores", minutes: 40, backend: true },
        { file: "07-docker.html", title: "Docker: PostgreSQL y stack completo", minutes: 45, backend: true },
        { file: "08-despliegue.html", title: "Despliegue del backend", minutes: 40, backend: true },
      ],
    },
    /* ========================================================== MÓDULO 15 */
    {
      id: "15",
      dir: "modulos/15-novedades-react-192",
      title: "Novedades de React 19.2",
      description: "Activity, useEffectEvent, PPR y el resto de novedades de React 19.2.",
      lessons: [
        { file: "01-activity.html", title: "Activity: ocultar sin perder estado", minutes: 40 },
        { file: "02-useeffectevent.html", title: "useEffectEvent", minutes: 40 },
        { file: "03-ppr-prerender-y-cachesignal.html", title: "PPR, prerender y cacheSignal", minutes: 40 },
        { file: "04-mas-novedades-19-2.html", title: "El resto: seguridad, Compiler y DevTools", minutes: 30 },
      ],
    },
  ],
};
