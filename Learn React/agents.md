# Pagina web estática para aprender React.

# Curso Completo de React 19 con JavaScript

## De Cero a Avanzado usando pnpm + Vite + JavaScript (Vanilla)

# Objetivo

Actúa como un profesor senior de React con muchos años de experiencia formando desarrolladores.

El objetivo es crear **un curso completo de React 19**, pensado para un desarrollador que ya sabe programar (por ejemplo .NET, JavaScript, Python, etc.) pero que **nunca ha aprendido React correctamente**.

El curso debe ser extremadamente didáctico.

No quiero únicamente aprender a escribir React; quiero entender **por qué funciona**, cómo está diseñado y cuáles son las mejores prácticas profesionales.

---

# Tecnologías

Utilizar SIEMPRE:

- React 19
- JavaScript (NO TypeScript)
- pnpm
- Vite
- CSS puro
- Fetch API
- React Router
- Context API
- Redux Toolkit (cuando llegue el momento)
- Zustand (comparación)
- Vitest
- React Testing Library
- VSCode

No utilizar:

- Create React App
- Yarn
- npm (usar pnpm en todos los ejemplos)
- Bootstrap
- Tailwind (solo opcional al final)

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

NO asumir conocimientos previos.

Utilizar comparaciones del mundo real.

Explicar el porqué de cada decisión.

No limitarse a decir "así funciona React".

Quiero entender el diseño interno.

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

# Proyecto durante el curso

Durante todo el curso construiremos una aplicación profesional.

Será una aplicación de gestión de tareas similar a Trello.

La iremos construyendo poco a poco.

Cada módulo añadirá funcionalidades nuevas.

Al terminar el curso tendremos una aplicación completa.

Backend:

- .NET 8
- API REST
- JWT
- PostgreSQL

Frontend:

- React

---

# Índice del curso

# Módulo 0 — Preparación

- ¿Qué es React?
- Historia
- SPA
- Virtual DOM
- Componentes
- React vs Vanilla JS
- React vs Angular
- React vs Vue
- React vs Solid
- Instalación de Node
- Instalación de pnpm
- Instalación de VSCode
- Crear el primer proyecto con Vite
- Estructura del proyecto

---

# Módulo 1 — Fundamentos

- JSX
- Babel
- Componentes
- Renderizado
- ReactDOM
- Expresiones
- Fragment
- Props
- Children
- Composición

Proyecto:

Crear una página sencilla.

---

# Módulo 2 — Renderizado

- map()
- keys
- renderizado condicional
- listas
- fragmentos

Proyecto:

Listado de productos.

---

# Módulo 3 — Eventos

- onClick
- onChange
- onSubmit
- paso de parámetros
- Synthetic Events

Proyecto:

Calculadora.

---

# Módulo 4 — Estado

- useState
- renderizado
- actualización
- arrays
- objetos
- inmutabilidad

Proyecto:

Lista de tareas.

---

# Módulo 5 — Formularios

- inputs
- textarea
- checkbox
- radio
- select
- validaciones

Proyecto:

Formulario de registro.

---

# Módulo 6 — Hooks

- useEffect
- useRef
- useMemo
- useCallback
- custom hooks

Proyecto:

Buscador de películas.

---

# Módulo 7 — Estilos

- CSS
- CSS Modules
- Responsive
- Variables CSS

Proyecto:

Dashboard responsive.

---

# Módulo 8 — Routing

- React Router
- Routes
- Link
- Navigate
- parámetros
- rutas protegidas

Proyecto:

Aplicación multipágina.

---

# Módulo 9 — APIs

- fetch
- async/await
- loading
- errores
- AbortController

Proyecto:

Consumir una API pública.

---

# Módulo 10 — Estado Global

- Context API
- useReducer
- Redux Toolkit
- Zustand
- comparación

Proyecto:

Carrito de compra.

---

# Módulo 11 — Optimización

- React.memo
- lazy
- Suspense
- Code Splitting
- Profiler

Proyecto:

Optimización completa.

---

# Módulo 12 — Testing

- Vitest
- React Testing Library
- mocks

Proyecto:

Tests del proyecto.

---

# Módulo 13 — Proyecto Final

Aplicación profesional de gestión de tareas (TaskFlow).

Debe incluir:

- Login
- Registro
- JWT
- Dashboard
- CRUD de proyectos
- CRUD de tareas
- Etiquetas
- Prioridades
- Comentarios
- Subtareas
- Filtros
- Búsquedas
- Paginación
- Tema oscuro
- Responsive
- Consumo de API
- WebSockets
- Despliegue

---

# Módulo 14 — Backend .NET 8 (añadido: curso full-stack)

- Setup y arquitectura limpia de la API
- EF Core + PostgreSQL (code-first, migraciones)
- Autenticación JWT (registro/login, hash de contraseñas)
- CRUD completo de la API
- SignalR (WebSockets tiempo real)
- Validación y manejo de errores
- Docker (PostgreSQL y stack completo)
- Despliegue del backend

---

# Buenas prácticas

Explicar siempre:

- cuándo utilizar cada solución
- ventajas
- inconvenientes
- alternativas
- rendimiento
- mantenibilidad
- escalabilidad

---

# Comparaciones

Cuando aparezca una característica nueva, compararla con:

- JavaScript Vanilla
- .NET
- Angular (cuando tenga sentido)
- Vue (cuando tenga sentido)

---

# Diagramas

Cuando sea útil, utilizar diagramas ASCII para explicar el flujo interno.

Ejemplo:

```text
Usuario
    │
    ▼
Componente
    │
    ▼
useState
    │
    ▼
Render
    │
    ▼
DOM
```

---

# Estilo

El estilo debe ser:

- Muy didáctico.
- Muy detallado.
- Fácil de leer.
- Profesional.
- Sin asumir conocimientos previos de React.
- Explicar siempre el "por qué".
- Utilizar analogías cuando ayuden a comprender.

---

# Formato de entrega

**DECISIÓN ACTUALIZADA (confirmada con el usuario):** el curso se entrega como **web estática** (HTML + CSS + JavaScript vanilla, sin Markdown y sin paso de build). La carpeta `web/` contiene todo el contenido navegable: cada lección es un archivo HTML completo y el chrome (cabecera, sidebar, progreso, tema claro/oscuro) lo inyecta `web/assets/js/site.js` a partir del mapa del curso definido en `web/assets/js/course-data.js` (única fuente de verdad para navegación y enlaces).

Los ejemplos y proyectos son **proyectos Vite reales y ejecutables** (React 19 + pnpm), gestionados como pnpm workspace desde la raíz (`pnpm install` único + `pnpm -r build` para verificar que todo compila).

El curso es **full-stack**: incluye el Módulo 14 con el backend completo (.NET 8 + EF Core + PostgreSQL + JWT + SignalR + Docker + despliegue).

Estructura real del repositorio:

```text
Aprender React/
│
├── AGENTS.md
├── package.json               ← workspace raíz (scripts)
├── pnpm-workspace.yaml        ← ejemplos/*, proyectos/*, proyecto-final/frontend
├── scripts/                   ← servidor estático y scaffolding de apps Vite
│
├── web/                       ← ★ EL CURSO (web estática, sin build)
│   ├── index.html             ← portada + índice completo
│   ├── assets/css/            ← estilos del sitio (tema claro/oscuro)
│   ├── assets/js/             ← course-data.js, highlight.js, site.js
│   ├── modulos/
│   │   ├── 00-preparacion/ … 12-testing/
│   │   ├── 13-proyecto-final/
│   │   └── 14-backend-dotnet/
│   └── recursos/              ← glosario, cheatsheet, FAQ
│
├── ejemplos/                  ← mini-demos Vite ejecutables (uno por módulo)
├── proyectos/                 ← proyecto práctico de cada módulo (Vite + React 19)
└── proyecto-final/
    ├── frontend/              ← TaskFlow, app tipo Trello (React 19)
    ├── backend/               ← TaskFlow API (.NET 8 + EF Core + JWT + SignalR)
    └── docker-compose.yml     ← PostgreSQL
```

# Convenciones del curso

- Idioma: contenido, comentarios y UI en español.
- Identificadores de código en inglés (estándar profesional; se enseña explícitamente en el Módulo 0).
- La web del curso usa tema oscuro por defecto con toggle a claro.
- Versiones fijadas por caret: React ^19, Vite ^7, pnpm 11 (corepack).
- Cada lección sigue SIEMPRE las 10 secciones definidas en "Forma de enseñar".

---

# Reglas importantes

1. Nunca des por sentado un concepto.
2. Explica siempre el "por qué".
3. Todo el código debe ejecutarse.
4. Usa únicamente `pnpm` en los comandos.
5. Sigue las buenas prácticas actuales de React 19.
6. No omitas pasos de configuración.
7. Mantén una progresión lógica entre las lecciones.
8. Cada módulo debe terminar con un proyecto práctico.
9. Reutiliza el proyecto principal a lo largo del curso para reforzar los conceptos.
10. Si un concepto depende de otro, enlaza a la lección correspondiente y recuérdalo brevemente.
11. Incluye consejos de depuración con React DevTools cuando sea pertinente.
12. Prioriza la comprensión profunda frente a la velocidad; el objetivo es que el alumno pueda desarrollar aplicaciones React profesionales al finalizar el curso.
