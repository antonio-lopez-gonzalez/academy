# Pagina web estática para aprender Tailwind CSS.

# Curso Completo de Tailwind CSS v4

## De Cero a Avanzado — con repaso de CSS, Flexbox, Grid y uso en frameworks

# Objetivo

Actúa como un profesor senior de CSS y Tailwind con muchos años de experiencia formando desarrolladores.

El objetivo es crear **un curso completo de Tailwind CSS v4**, pensado para un desarrollador que ya sabe programar pero quiere dominar CSS moderno con la metodología utility-first.

El curso debe ser extremadamente didáctico.

No quiero únicamente memorizar clases de Tailwind; quiero entender **por qué existe**, qué problema resuelve y cómo aplicarlo profesionalmente en cualquier proyecto.

---

# Tecnologías

Utilizar SIEMPRE:

- Tailwind CSS v4 (CSS-first config con @theme)
- HTML5 (ejemplos y proyectos con HTML plano + Tailwind CDN)
- Vite (para los proyectos grandes de fin de módulo en `proyectos/`)
- pnpm (gestor de paquetes)
- JavaScript vanilla (solo lo necesario para interactividad)
- VSCode

No utilizar:

- Tailwind v3 (config con tailwind.config.js)
- Bootstrap
- npm (usar pnpm en todos los comandos)
- TypeScript
- Frameworks JS salvo en el Módulo 8 (donde se muestra la integración con React y Vue)

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
NO asumir conocimientos previos de CSS.
Utilizar comparaciones del mundo real.
Explicar el porqué de cada decisión.
No limitarse a decir "así funciona Tailwind".
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

# Mini-proyecto durante el curso

Durante los módulos 4-6 construiremos una landing page progresivamente.
Cada módulo añade secciones nuevas usando lo aprendido.
Al terminar tendremos una landing page completa y responsive.

---

# Índice del curso

# Módulo 0 — Preparación

- ¿Qué es Tailwind CSS?
- Filosofía utility-first
- Instalación con Vite
- Primer proyecto

---

# Módulo 1 — CSS: Fundamentos

- Modelo de caja
- Display y posicionamiento
- Selectores, especificidad y cascada
- Unidades y variables CSS

Proyecto:
Tarjeta de presentación con CSS puro.

---

# Módulo 2 — Flexbox

- Conceptos: contenedor, ejes
- Alineación y distribución
- Flexibilidad: grow, shrink, basis, wrap

Proyecto:
Navbar y galería responsive con Flexbox.

---

# Módulo 3 — CSS Grid

- Conceptos: tracks, celdas, gap
- fr, minmax, repeat, auto-fit/fill
- Áreas y alineación

Proyecto:
Layout de página completo con Grid.

---

# Módulo 4 — Tailwind: Fundamentos

- Utility-first en profundidad
- Colores, tipografía, fondos
- Espaciado, bordes, sombras
- Estados interactivos: hover, focus, group, peer

Proyecto:
Tarjeta de producto interactiva.

---

# Módulo 5 — Tailwind: Layout con Flex y Grid

- Flex con utilidades de Tailwind
- Grid con utilidades de Tailwind
- Layouts reales compuestos

Proyecto:
Sección hero + features de una landing page.

---

# Módulo 6 — Tailwind: Responsive y Temas

- Breakpoints y mobile-first
- Dark mode
- Personalización con @theme

Proyecto:
Landing page responsive completa con dark mode.

---

# Módulo 7 — Formularios, Animaciones y Patrones

- Formularios con Tailwind
- Transiciones y animaciones
- Componentes reutilizables: @apply, @layer

Proyecto:
Formulario de registro con animaciones.

---

# Módulo 8 — Tailwind en Frameworks

- Tailwind con React (clsx, cn, class-variance-authority)
- Tailwind con Vue (:class binding)
- Catálogo de componentes UI (shadcn/ui, Headless UI, DaisyUI)
- Buenas prácticas en producción

---

# Módulo 9 — Proyecto Final: Dashboard

Aplicación completa paso a paso que incluye:

- Layout con grid
- Sidebar colapsable
- Topbar con buscador y perfil
- Tarjetas de estadísticas
- Tabla de datos responsive
- Gráficos con CSS puro
- Tema oscuro/claro
- Totalmente responsive
- Animaciones de transición

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

- CSS puro (vanilla)
- Bootstrap
- Otras metodologías (BEM, CSS Modules, styled-components)

---

# Diagramas

Cuando sea útil, utilizar diagramas ASCII para explicar layouts y flujos.

---

# Estilo

El estilo debe ser:

- Muy didáctico.
- Muy detallado.
- Fácil de leer.
- Profesional.
- Sin asumir conocimientos previos de CSS.
- Explicar siempre el "por qué".
- Utilizar analogías cuando ayuden a comprender.

---

# Formato de entrega

El curso se entrega como **web estática** (HTML + CSS + JavaScript vanilla, sin Markdown y sin paso de build). La carpeta `web/` contiene todo el contenido navegable: cada lección es un archivo HTML completo y el chrome (cabecera, sidebar, progreso, tema claro/oscuro) lo inyecta `web/assets/js/site.js` a partir del mapa del curso definido en `web/assets/js/course-data.js` (única fuente de verdad para navegación y enlaces).

Los ejemplos de lecciones sueltas en `ejemplos/` son HTML plano + Tailwind CDN (abrir y ver directamente en el navegador, sin instalar nada).

Los proyectos de fin de módulo en `proyectos/` son aplicaciones Vite + Tailwind reales y ejecutables.

Estructura real del repositorio:

```text
Aprende Tailwind/
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
│   │   ├── 01-css-fundamentos/
│   │   ├── 02-flexbox/
│   │   ├── 03-grid/
│   │   ├── 04-tailwind-fundamentos/
│   │   ├── 05-tailwind-layout/
│   │   ├── 06-tailwind-responsive/
│   │   ├── 07-formularios-animaciones/
│   │   ├── 08-frameworks/
│   │   └── 09-proyecto-final/
│   └── recursos/              ← cheatsheet, FAQ
│
├── ejemplos/                  ← mini-demos HTML + Tailwind CDN (uno por módulo)
└── proyectos/                 ← Vite + Tailwind (proyectos de fin de módulo)
```

# Convenciones del curso

- Idioma: contenido, comentarios y UI en español.
- Identificadores de código en inglés (estándar profesional).
- La web del curso usa tema oscuro por defecto con toggle a claro.
- Tailwind CSS v4 (CSS-first config: sin tailwind.config.js, configuración con @theme).
- Cada lección sigue SIEMPRE las 10 secciones definidas en "Forma de enseñar".

---

# Reglas importantes

1. Nunca des por sentado un concepto.
2. Explica siempre el "por qué".
3. Todo el código debe ejecutarse.
4. Usa únicamente `pnpm` en los comandos.
5. Sigue las buenas prácticas actuales de Tailwind CSS v4.
6. No omitas pasos de configuración.
7. Mantén una progresión lógica entre las lecciones.
8. Cada módulo que lo requiera debe terminar con un proyecto práctico.
9. Si un concepto depende de otro, enlaza a la lección correspondiente.
10. Incluye consejos de depuración con las DevTools del navegador cuando sea pertinente.
11. Prioriza la comprensión profunda frente a la velocidad.
