/**
 * course-data.js — MAPA COMPLETO DEL CURSO TAILWIND CSS
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
  title: "Curso Tailwind CSS v4",
  subtitle: "De cero a avanzado · CSS, Flexbox, Grid, Responsive y frameworks",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de Tailwind CSS v4" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Preparación",
      description: "Qué es Tailwind, la filosofía utility-first y cómo montar tu entorno.",
      lessons: [
        { file: "01-que-es-tailwind.html", title: "Qué es Tailwind CSS", minutes: 25 },
        { file: "02-instalacion-y-vite.html", title: "Instalación y primer proyecto con Vite", minutes: 35 },
        { file: "03-flujo-de-trabajo.html", title: "Flujo de trabajo con Tailwind", minutes: 25 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-css-fundamentos",
      title: "CSS: Fundamentos",
      description: "Modelo de caja, display, posicionamiento, selectores, especificidad y unidades.",
      lessons: [
        { file: "01-modelo-de-caja.html", title: "El modelo de caja", minutes: 30 },
        { file: "02-display-y-posicionamiento.html", title: "Display y posicionamiento", minutes: 35 },
        { file: "03-selectores-y-especificidad.html", title: "Selectores, especificidad y cascada", minutes: 30 },
        { file: "04-unidades-y-variables.html", title: "Unidades CSS y custom properties", minutes: 30 },
        { file: "05-proyecto-tarjeta-presentacion.html", title: "🛠 Proyecto: tarjeta de presentación", minutes: 45, project: true },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-flexbox",
      title: "Flexbox",
      description: "Contenedor flex, ejes, alineación, distribución y flexibilidad.",
      lessons: [
        { file: "01-conceptos-flexbox.html", title: "Conceptos de Flexbox: contenedor y ejes", minutes: 30 },
        { file: "02-alineacion-y-distribucion.html", title: "Alineación y distribución", minutes: 30 },
        { file: "03-flexibilidad.html", title: "Flexibilidad: grow, shrink, basis y wrap", minutes: 30 },
        { file: "04-proyecto-navbar-galeria.html", title: "🛠 Proyecto: navbar y galería con Flexbox", minutes: 50, project: true },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-grid",
      title: "CSS Grid",
      description: "Grid container, tracks, fr, minmax, áreas y layouts completos.",
      lessons: [
        { file: "01-conceptos-grid.html", title: "Conceptos de Grid: contenedor, tracks y celdas", minutes: 30 },
        { file: "02-definicion-avanzada.html", title: "Definición avanzada: fr, minmax, repeat, auto-fit", minutes: 35 },
        { file: "03-areas-y-alineacion.html", title: "Áreas de grid y alineación", minutes: 30 },
        { file: "04-proyecto-layout-pagina.html", title: "🛠 Proyecto: layout de página completa con Grid", minutes: 50, project: true },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-tailwind-fundamentos",
      title: "Tailwind: Fundamentos",
      description: "Utility-first en profundidad, colores, tipografía, espaciado y estados.",
      lessons: [
        { file: "01-utility-first.html", title: "Utility-first en profundidad", minutes: 30 },
        { file: "02-colores-tipografia-fondos.html", title: "Colores, tipografía y fondos", minutes: 35 },
        { file: "03-espaciado-bordes-sombras.html", title: "Espaciado, bordes y sombras", minutes: 30 },
        { file: "04-estados-interactivos.html", title: "Estados interactivos: hover, focus, group, peer", minutes: 35 },
        { file: "05-proyecto-tarjeta-producto.html", title: "🛠 Proyecto: tarjeta de producto interactiva", minutes: 50, project: true },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-tailwind-layout",
      title: "Tailwind: Layout",
      description: "Flex y Grid con utilidades de Tailwind, layouts compuestos reales.",
      lessons: [
        { file: "01-flex-en-tailwind.html", title: "Flex con utilidades de Tailwind", minutes: 30 },
        { file: "02-grid-en-tailwind.html", title: "Grid con utilidades de Tailwind", minutes: 35 },
        { file: "03-layouts-reales.html", title: "Layouts reales compuestos", minutes: 35 },
        { file: "04-proyecto-hero-features.html", title: "🛠 Proyecto: hero + features de landing page", minutes: 50, project: true },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-tailwind-responsive",
      title: "Tailwind: Responsive y Temas",
      description: "Breakpoints, mobile-first, dark mode y personalización con @theme.",
      lessons: [
        { file: "01-breakpoints-mobile-first.html", title: "Breakpoints y mobile-first", minutes: 30 },
        { file: "02-dark-mode.html", title: "Dark mode", minutes: 30 },
        { file: "03-personalizacion-theme.html", title: "Personalización con @theme", minutes: 35 },
        { file: "04-proyecto-landing-responsive.html", title: "🛠 Proyecto: landing page responsive completa", minutes: 55, project: true },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-formularios-animaciones",
      title: "Formularios, Animaciones y Patrones",
      description: "Forms, transiciones, animaciones, @apply, @layer y componentes reutilizables.",
      lessons: [
        { file: "01-formularios.html", title: "Formularios con Tailwind", minutes: 35 },
        { file: "02-transiciones-y-animaciones.html", title: "Transiciones y animaciones", minutes: 35 },
        { file: "03-componentes-reutilizables.html", title: "Componentes reutilizables: @apply y @layer", minutes: 30 },
        { file: "04-proyecto-formulario-animado.html", title: "🛠 Proyecto: formulario de registro animado", minutes: 50, project: true },
        { file: "05-novedades-v4-1-v4-3.html", title: "Novedades de Tailwind v4.1 a v4.3", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 8 */
    {
      id: "08",
      dir: "modulos/08-frameworks",
      title: "Tailwind en Frameworks",
      description: "Integración con React y Vue, catálogo de componentes y buenas prácticas en producción.",
      lessons: [
        { file: "01-tailwind-con-react.html", title: "Tailwind con React", minutes: 40 },
        { file: "02-tailwind-con-vue.html", title: "Tailwind con Vue", minutes: 30 },
        { file: "03-catalogo-componentes.html", title: "Catálogo de componentes UI", minutes: 35 },
        { file: "04-buenas-practicas-produccion.html", title: "Buenas prácticas en producción", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 9 */
    {
      id: "09",
      dir: "modulos/09-proyecto-final",
      title: "Proyecto Final: Dashboard",
      description: "Dashboard profesional paso a paso con Tailwind CSS: layout, sidebar, stats, tablas, responsive y dark mode.",
      lessons: [
        { file: "01-estructura-base.html", title: "Estructura base del dashboard", minutes: 45, project: true },
        { file: "02-sidebar-y-navegacion.html", title: "Sidebar y navegación", minutes: 40, project: true },
        { file: "03-tarjetas-y-metricas.html", title: "Tarjetas de estadísticas y métricas", minutes: 35, project: true },
        { file: "04-tabla-y-widgets.html", title: "Tabla de datos y widgets", minutes: 40, project: true },
        { file: "05-responsive-y-pulido.html", title: "Responsive, dark mode y pulido final", minutes: 45, project: true },
      ],
    },
  ],
};
