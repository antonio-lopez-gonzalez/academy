/**
 * course-data.js — MAPA COMPLETO DEL CURSO ASTRO
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
  title: "Curso Astro",
  subtitle: "El framework web moderno · MPA, islands y content-driven",

  resources: [
    { file: "recursos/glosario.html", title: "Glosario de Astro" },
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de Astro" },
    { file: "recursos/faq.html", title: "FAQ del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Preparación",
      description: "Qué es Astro, MPA vs SPA, la filosofía islands y cómo montar tu entorno de desarrollo.",
      lessons: [
        { file: "01-que-es-astro.html", title: "Qué es Astro", minutes: 25 },
        { file: "02-instalacion.html", title: "Instalación: Node, pnpm, Vite y primer proyecto", minutes: 35 },
        { file: "03-estructura-proyecto.html", title: "Estructura del proyecto", minutes: 25 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-componentes-astro",
      title: "Componentes Astro",
      description: "Componentes .astro, frontmatter, template, props tipadas, slots simples y nombrados.",
      lessons: [
        { file: "01-componentes-basicos.html", title: "Componentes .astro, frontmatter y template", minutes: 30 },
        { file: "02-props.html", title: "Props, tipos y valores por defecto", minutes: 30 },
        { file: "03-slots.html", title: "Slots simples y nombrados", minutes: 30 },
        { file: "04-proyecto-portfolio.html", title: "Proyecto: portfolio con componentes", minutes: 45, project: true },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-routing-y-layouts",
      title: "Routing y Layouts",
      description: "Páginas basadas en ficheros, rutas dinámicas con getStaticPaths y layouts anidados.",
      lessons: [
        { file: "01-paginas.html", title: "Páginas y sistema de ficheros", minutes: 30 },
        { file: "02-rutas-dinamicas.html", title: "Rutas dinámicas y getStaticPaths", minutes: 35 },
        { file: "03-layouts.html", title: "Layouts anidados", minutes: 30 },
        { file: "04-proyecto-blog-routing.html", title: "Proyecto: blog con routing", minutes: 45, project: true },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-markdown-content",
      title: "Markdown y Content",
      description: "Markdown, MDX, Content Collections con esquemas Zod, plugins remark y rehype.",
      lessons: [
        { file: "01-markdown.html", title: "Markdown y MDX en Astro", minutes: 30 },
        { file: "02-content-collections.html", title: "Content Collections y esquemas Zod", minutes: 35 },
        { file: "03-remark-rehype.html", title: "Plugins remark y rehype", minutes: 30 },
        { file: "04-proyecto-blog-markdown.html", title: "Proyecto: blog con colecciones", minutes: 45, project: true },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-islands-js",
      title: "Islands e Interactividad",
      description: "Arquitectura islands, hidratación selectiva, React, Vue y Svelte como islas interactivas.",
      lessons: [
        { file: "01-islands-concepto.html", title: "Arquitectura islands e hidratación", minutes: 30 },
        { file: "02-react-en-astro.html", title: "Componentes React como islands", minutes: 35 },
        { file: "03-vue-svelte.html", title: "Componentes Vue y Svelte en Astro", minutes: 30 },
        { file: "04-proyecto-formulario-island.html", title: "Proyecto: formulario con islas React", minutes: 45, project: true },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-datos-y-endpoints",
      title: "Datos y Endpoints",
      description: "Fetch de datos en build time, API endpoints con Astro, modo server (SSR) e hidratación bajo demanda.",
      lessons: [
        { file: "01-fetch-astro.html", title: "Fetch de datos en build time", minutes: 30 },
        { file: "02-endpoints-api.html", title: "API endpoints con Astro", minutes: 35 },
        { file: "03-server-mode.html", title: "Modo server (SSR) e hidratación bajo demanda", minutes: 35 },
        { file: "04-proyecto-api-blog.html", title: "Proyecto: blog con API y SSR", minutes: 45, project: true },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-tailwind-integraciones",
      title: "Tailwind e Integraciones",
      description: "Tailwind CSS en Astro, optimización de imágenes y fuentes, sitemap, RSS y otras integraciones oficiales.",
      lessons: [
        { file: "01-tailwind-astro.html", title: "Tailwind CSS en Astro", minutes: 30 },
        { file: "02-imagenes-y-fonts.html", title: "Optimización de imágenes y fuentes", minutes: 30 },
        { file: "03-integraciones.html", title: "Integraciones oficiales (Sitemap, RSS, etc.)", minutes: 30 },
        { file: "04-proyecto-landing-tailwind.html", title: "Proyecto: landing con Tailwind y Astro", minutes: 45, project: true },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-seo-y-rendimiento",
      title: "SEO y Rendimiento",
      description: "SEO, Open Graph, metadatos, View Transitions API, Core Web Vitals y optimización en Astro.",
      lessons: [
        { file: "01-seo.html", title: "SEO, Open Graph y metadatos", minutes: 30 },
        { file: "02-view-transitions.html", title: "View Transitions API", minutes: 35 },
        { file: "03-rendimiento.html", title: "Core Web Vitals y optimización", minutes: 30 },
        { file: "04-proyecto-blog-seo.html", title: "Proyecto: blog optimizado SEO y rendimiento", minutes: 45, project: true },
      ],
    },
    /* ============================================================ MÓDULO 8 */
    {
      id: "08",
      dir: "modulos/08-auth-y-despliegue",
      title: "Auth y Despliegue",
      description: "Autenticación con cookies y middleware, variables de entorno, secretos y despliegue en producción.",
      lessons: [
        { file: "01-autenticacion.html", title: "Autenticación con cookies y middleware", minutes: 35 },
        { file: "02-variables-entorno.html", title: "Variables de entorno y secretos", minutes: 30 },
        { file: "03-despliegue.html", title: "Despliegue en Vercel, Netlify y Cloudflare", minutes: 35 },
        { file: "04-proyecto-auth-despliegue.html", title: "Proyecto: zona privada con auth", minutes: 45, project: true },
      ],
    },
    /* ============================================================ MÓDULO 9 */
    {
      id: "09",
      dir: "modulos/09-proyecto-final",
      title: "Proyecto Final: Blog Profesional",
      description: "Arquitectura completa del blog profesional: portada, routing, colecciones, islands, API, SEO y despliegue a producción.",
      lessons: [
        { file: "01-arquitectura.html", title: "Arquitectura del blog profesional", minutes: 45, project: true },
        { file: "02-portada-y-routing.html", title: "Portada, routing y colecciones", minutes: 40, project: true },
        { file: "03-interactividad-y-api.html", title: "Islands, API y formularios", minutes: 40, project: true },
        { file: "04-seo-despliegue.html", title: "SEO, rendimiento y despliegue a producción", minutes: 40, project: true },
      ],
    },
  ],
};
