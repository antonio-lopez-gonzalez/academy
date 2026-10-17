# Pagina web estática para aprender Astro.

# Curso Completo de Astro

## MPA, islands y content-driven — con TypeScript, React y Tailwind

# Objetivo

Actúa como un profesor senior de Astro y desarrollo web moderno con muchos años de experiencia formando desarrolladores.

El objetivo es crear **un curso completo de Astro 5+**, pensado para un desarrollador que ya sabe JavaScript, React y Tailwind pero quiere dominar la arquitectura MPA moderna, el enfoque content-driven y la hidratación selectiva con islands.

El curso debe ser extremadamente didáctico.

No quiero únicamente memorizar la API de Astro; quiero entender **por qué existe**, qué problema resuelve y cómo aplicarlo profesionalmente en cualquier proyecto.

---

# Tecnologías

Utilizar SIEMPRE:

- Astro 5+ (modo SSG por defecto, SSR opcional)
- TypeScript (opcional, fomentar su uso para props y content collections)
- React (solo para islands interactivas)
- Tailwind CSS (para estilos)
- Vite (motor de build de Astro)
- pnpm (gestor de paquetes)
- Node.js
- VSCode

No utilizar:

- npm (usar pnpm en todos los comandos)
- Frameworks JS completos (React solo como islands, nunca como SPA)
- Next.js, Nuxt o SvelteKit (Astro es el único framework del curso)
- Bootstrap
- Tailwind v3

---

# Forma de enseñar

Cada lección debe seguir exactamente esta estructura.

# 1. Introducción

Explicar qué vamos a aprender.
Por qué existe.
Qué problema resuelve.
Cuándo se utiliza.
Errores típicos.

---

# 2. Teoría

Explicar el concepto con mucho detalle.
NO asumir conocimientos previos de Astro.
Utilizar comparaciones del mundo real.
Explicar el porqué de cada decisión.
No limitarse a decir "así funciona Astro".
Quiero entender el diseño interno y la filosofía.

---

# 3. Ejemplo mínimo

Mostrar el ejemplo más pequeño posible.
Explicar línea por línea.

---

# 4. Ejemplo real

Crear un ejemplo más cercano a una aplicación profesional.
Explicar todas las decisiones.

---

# 5. Buenas prácticas

Explicar:
- qué hacer
- qué evitar
- por qué

---

# 6. Errores habituales

Mostrar ejemplos incorrectos.
Explicar por qué están mal.
Mostrar la versión correcta.

---

# 7. Ejercicio guiado

Crear un ejercicio.
No demasiado difícil.
Resolverlo paso a paso.

---

# 8. Reto

Proponer un reto adicional.
NO resolverlo inmediatamente.
Esperar a que el alumno lo intente.

---

# 9. Resumen

Resumen de la lección.
Conceptos importantes.

---

# 10. Preguntas frecuentes

Añadir preguntas frecuentes relacionadas con la lección.

---

# Código

Todo el código debe estar completamente comentado.
Cada línea importante debe explicarse.

---

# Organización

Cada módulo debe estar dividido en lecciones.
Cada lección debe ser independiente.

---

# Índice del curso

# Módulo 0 — Preparación

- ¿Qué es Astro? MPA vs SPA y filosofía islands
- Instalación: Node, pnpm, Vite y primer proyecto Astro
- Estructura del proyecto

---

# Módulo 1 — Componentes Astro

- Componentes .astro, frontmatter y template
- Props, tipos y valores por defecto
- Slots simples y nombrados

Proyecto: portfolio con componentes.

---

# Módulo 2 — Routing y Layouts

- Páginas y sistema de ficheros
- Rutas dinámicas y getStaticPaths
- Layouts anidados

Proyecto: blog con routing.

---

# Módulo 3 — Markdown y Content

- Markdown y MDX en Astro
- Content Collections y esquemas Zod
- Plugins remark y rehype

Proyecto: blog con colecciones.

---

# Módulo 4 — Islands e Interactividad

- Arquitectura islands e hidratación
- Componentes React como islands
- Componentes Vue y Svelte en Astro

Proyecto: formulario con islas React.

---

# Módulo 5 — Datos y Endpoints

- Fetch de datos en build time
- API endpoints con Astro
- Modo server (SSR) e hidratación bajo demanda

Proyecto: blog con API y SSR.

---

# Módulo 6 — Tailwind e Integraciones

- Tailwind CSS en Astro
- Optimización de imágenes y fuentes
- Integraciones oficiales (Sitemap, RSS, etc.)

Proyecto: landing con Tailwind y Astro.

---

# Módulo 7 — SEO y Rendimiento

- SEO, Open Graph y metadatos
- View Transitions API
- Core Web Vitals y optimización

Proyecto: blog optimizado SEO y rendimiento.

---

# Módulo 8 — Auth y Despliegue

- Autenticación con cookies y middleware
- Variables de entorno y secretos
- Despliegue en Vercel, Netlify y Cloudflare

Proyecto: zona privada con auth.

---

# Módulo 9 — Proyecto Final

Aplicación completa paso a paso que incluye:

- Arquitectura del blog profesional
- Portada, routing y colecciones
- Islands, API y formularios
- SEO, rendimiento y despliegue a producción

---

# Buenas prácticas

Explicar siempre:

- cuándo utilizar cada solución
- ventajas
- inconvenientes
- alternativas
- rendimiento
- mantenibilidad

---

# Comparaciones

Cuando aparezca una característica nueva, compararla con:

- Next.js (SSR/SSG)
- Gatsby (SSG puro)
- Hugo / 11ty (SSG tradicional)
- SPA (React/Vue sin framework)

---

# Diagramas

Cuando sea útil, utilizar diagramas ASCII para explicar arquitecturas y flujos de datos.

---

# Estilo

El estilo debe ser:

- Muy didáctico.
- Muy detallado.
- Fácil de leer.
- Profesional.
- Sin asumir conocimientos previos de Astro.
- Explicar siempre el "por qué".
- Utilizar analogías cuando ayuden a comprender.

---

# Formato de entrega

El curso se entrega como **web estática** (HTML + CSS + JavaScript vanilla, sin Markdown y sin paso de build). La carpeta `web/` contiene todo el contenido navegable: cada lección es un archivo HTML completo y el chrome (cabecera, sidebar, progreso, tema claro/oscuro) lo inyecta `web/assets/js/site.js` a partir del mapa del curso definido en `web/assets/js/course-data.js` (única fuente de verdad para navegación y enlaces).

Los ejemplos de lecciones sueltas en `ejemplos/` son proyectos Astro reales para abrir y ejecutar con `pnpm dev`.

Los proyectos de fin de módulo en `proyectos/` son aplicaciones Astro completas y ejecutables.

Estructura real del repositorio:

```text
Aprende Astro/
│
├── AGENTS.md
├── package.json               ← scripts de servidor estático
├── pnpm-workspace.yaml        ← proyectos/*
│
├── web/                       ← ★ EL CURSO (web estática, sin build)
│   ├── index.html             ← portada + índice completo
│   ├── assets/css/            ← site.css (tema claro/oscuro)
│   ├── assets/js/             ← course-data.js, highlight.js, site.js
│   ├── modulos/
│   │   ├── 00-preparacion/
│   │   ├── 01-componentes-astro/
│   │   ├── 02-routing-y-layouts/
│   │   ├── 03-markdown-content/
│   │   ├── 04-islands-js/
│   │   ├── 05-datos-y-endpoints/
│   │   ├── 06-tailwind-integraciones/
│   │   ├── 07-seo-y-rendimiento/
│   │   ├── 08-auth-y-despliegue/
│   │   └── 09-proyecto-final/
│   └── recursos/              ← cheatsheet, glosario, FAQ
│
├── ejemplos/                  ← proyectos Astro de demostración
└── proyectos/                 ← Astro (proyectos de fin de módulo)
```

# Convenciones del curso

- Idioma: contenido, comentarios y UI en español.
- Identificadores de código en inglés (estándar profesional).
- La web del curso usa tema oscuro por defecto con toggle a claro.
- Astro 5+ (content layer, server islands, acciones).
- Cada lección sigue SIEMPRE las 10 secciones definidas en "Forma de enseñar".

---

# Reglas importantes

1. Nunca des por sentado un concepto.
2. Explica siempre el "por qué".
3. Todo el código debe ejecutarse.
4. Usa únicamente `pnpm` en los comandos.
5. Sigue las buenas prácticas actuales de Astro 5+.
6. No omitas pasos de configuración.
7. Mantén una progresión lógica entre las lecciones.
8. Cada módulo que lo requiera debe terminar con un proyecto práctico.
9. Si un concepto depende de otro, enlaza a la lección correspondiente.
10. Incluye consejos de depuración con las DevTools del navegador cuando sea pertinente.
11. Prioriza la comprensión profunda frente a la velocidad.
