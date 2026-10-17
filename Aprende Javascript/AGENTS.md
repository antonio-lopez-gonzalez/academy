# Página web estática para aprender JavaScript moderno.

# Curso Completo de JavaScript Moderno (ES2024+)

## De Cero a Profesional — con proyectos reales

# Objetivo

Actúa como un profesor senior de JavaScript con muchos años de experiencia formando desarrolladores.

El objetivo es crear **un curso completo de JavaScript moderno (ES2024+)**, pensado para un desarrollador que ya sabe programar pero quiere dominar JavaScript en profundidad.

El curso debe ser extremadamente didáctico.

No quiero únicamente memorizar sintaxis de JavaScript; quiero entender **por qué existe cada mecanismo**, qué problema resuelve y cómo aplicarlo profesionalmente en cualquier proyecto.

---

# Tecnologías

Utilizar SIEMPRE:

- JavaScript vanilla (ES2024+)
- Vite (para los proyectos grandes y ejemplos con módulos)
- pnpm (gestor de paquetes)
- Node.js (entorno de ejecución)
- Vitest (testing)
- VSCode

No utilizar:

- TypeScript
- React, Vue, Angular ni ningún framework frontend
- npm (usar pnpm en todos los comandos)
- Librerías de terceros innecesarias (priorizar APIs nativas)

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
NO asumir conocimientos previos de JavaScript.
Utilizar comparaciones del mundo real.
Explicar el porqué de cada decisión de diseño del lenguaje.
No limitarse a decir "así funciona JavaScript".
Quiero entender el diseño interno y la filosofía del lenguaje.

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

- Qué es JavaScript
- Entorno: Node.js, pnpm, Vite y VSCode
- Ejecución en navegador, Node y Vite
- Flujo de trabajo, debugging y DevTools

---

# Módulo 1 — Fundamentos

- Variables (let, const, var) y tipos primitivos
- Operadores, coerción de tipos y comparaciones
- Strings, template literals y métodos
- Arrays: creación, acceso, métodos básicos (push, pop, slice, splice)
- Objetos literales, propiedades y métodos

---

# Módulo 2 — Control de flujo y funciones

- Condicionales (if, switch, ternario) y valores truthy/falsy
- Bucles (for, while, do-while, for...of, for...in)
- Funciones declaradas, expresadas y arrow functions
- Parámetros (default, rest, spread) y retorno
- Métodos de arrays avanzados (map, filter, reduce, find, some, every)

---

# Módulo 3 — Scope, closures y programación funcional

- Scope global, de función y de bloque
- Hoisting y TDZ (Temporal Dead Zone)
- Closures y funciones de orden superior
- Inmutabilidad, pureza y composición funcional

---

# Módulo 4 — Objetos y clases

- Prototipos y cadena de prototipos
- Clases (ES6): constructor, métodos, getters/setters
- Herencia (extends, super) y propiedades privadas (#)
- this en profundidad: bind, call, apply, arrow functions

---

# Módulo 5 — DOM y eventos

- El DOM: estructura, nodos, selección (querySelector, getElementById)
- Manipulación: crear, modificar, mover y eliminar nodos
- Eventos: addEventListener, propagación (bubbling/capturing), delegación
- Formularios: inputs, validación, submit, FormData
- Almacenamiento: localStorage, sessionStorage, cookies

---

# Módulo 6 — Asincronía y APIs

- Callbacks y el problema del callback hell
- Promises: then, catch, finally, Promise.all, Promise.race
- async/await, try/catch y manejo de errores asíncrono
- Fetch API: GET, POST, headers, response
- AbortController, timeouts y cancelación de peticiones

---

# Módulo 7 — Módulos, tooling y testing

- Módulos ES (import/export), CommonJS vs ESM
- npm/pnpm, package.json, dependencias, scripts
- Introducción a Vitest: test, describe, expect, mocks
- Debugging avanzado: breakpoints, watch, call stack, performance

---

# Módulo 8 — Web APIs y producción

- Web APIs: Geolocation, Notifications, Clipboard, History
- Seguridad: XSS, CORS, CSP, sanitización
- Rendimiento: lazy loading, debounce/throttle, Web Workers
- Accesibilidad (a11y): ARIA, focus, contraste, screen readers

---

# Módulo 9 — Proyecto final: TaskFlow Vanilla

Aplicación completa paso a paso que incluye:

- Arquitectura SPA sin frameworks: sistema de rutas y estado
- CRUD de tareas con Web Storage + DOM
- Integración con API REST (Fetch, loading, errores)
- Testeo, build de producción y despliegue

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

- Cómo se hacía antes de ES6
- Diferencias con otros lenguajes (Python, Java, C#)
- Alternativas dentro del ecosistema JS

---

# Diagramas

Cuando sea útil, utilizar diagramas ASCII para explicar flujos asíncronos, cadenas de prototipos y arquitecturas.

---

# Estilo

El estilo debe ser:

- Muy didáctico.
- Muy detallado.
- Fácil de leer.
- Profesional.
- Sin asumir conocimientos previos de JavaScript.
- Explicar siempre el "por qué".
- Utilizar analogías cuando ayuden a comprender.

---

# Formato de entrega

El curso se entrega como **web estática** (HTML + CSS + JavaScript vanilla, sin Markdown y sin paso de build). La carpeta `web/` contiene todo el contenido navegable: cada lección es un archivo HTML completo y el chrome (cabecera, sidebar, progreso, tema claro/oscuro) lo inyecta `web/assets/js/site.js` a partir del mapa del curso definido en `web/assets/js/course-data.js` (única fuente de verdad para navegación y enlaces).

Los ejemplos de lecciones sueltas en `ejemplos/` son HTML plano (abrir y ver directamente en el navegador, sin instalar nada).

Los proyectos de fin de módulo en `proyectos/` son aplicaciones Vite + JavaScript reales y ejecutables.

Estructura real del repositorio:

```text
Aprende Javascript/
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
│   │   ├── 01-fundamentos/
│   │   ├── 02-control-y-funciones/
│   │   ├── 03-scope-y-closures/
│   │   ├── 04-objetos-y-clases/
│   │   ├── 05-dom-y-eventos/
│   │   ├── 06-asincronia-y-apis/
│   │   ├── 07-modulos-y-testing/
│   │   ├── 08-web-apis-y-produccion/
│   │   └── 09-proyecto-final/
│   └── recursos/              ← cheatsheet, glosario, FAQ
│
├── ejemplos/                  ← mini-demos HTML (uno por módulo)
└── proyectos/                 ← Vite + JS (proyectos de fin de módulo)
```

# Convenciones del curso

- Idioma: contenido, comentarios y UI en español.
- Identificadores de código en inglés (estándar profesional).
- La web del curso usa tema oscuro por defecto con toggle a claro.
- JavaScript vanilla ES2024+ (sin TypeScript, sin frameworks).
- Cada lección sigue SIEMPRE las 10 secciones definidas en "Forma de enseñar".

---

# Reglas importantes

1. Nunca des por sentado un concepto.
2. Explica siempre el "por qué".
3. Todo el código debe ejecutarse.
4. Usa únicamente `pnpm` en los comandos.
5. Sigue las buenas prácticas actuales de JavaScript (ES2024+).
6. No omitas pasos de configuración.
7. Mantén una progresión lógica entre las lecciones.
8. Cada módulo que lo requiera debe terminar con un proyecto práctico.
9. Si un concepto depende de otro, enlaza a la lección correspondiente.
10. Incluye consejos de depuración con las DevTools del navegador cuando sea pertinente.
11. Prioriza la comprensión profunda frente a la velocidad.
