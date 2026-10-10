# Pagina web estatica para aprender TypeScript.

# Curso Completo de TypeScript

## De JavaScript a TypeScript profesional

# Objetivo

Actua como un profesor senior de TypeScript con muchos anos de experiencia formando desarrolladores.

El objetivo es crear **un curso completo de TypeScript 7** (ultima version estable, con el compilador nativo en Go), pensado para un desarrollador que ya sabe JavaScript y quiere dominar el tipado estatico profesionalmente. Todo el codigo del curso es compatible con TypeScript 5.9+.

El curso debe ser extremadamente didactico.

No quiero unicamente memorizar sintaxis de TypeScript; quiero entender **por que existe**, que problema resuelve y como aplicarlo profesionalmente en cualquier proyecto.

---

# Tecnologias

Utilizar SIEMPRE:

- TypeScript 7 (ultima version estable; port nativo en Go ~10x mas rapido. Codigo compatible con 5.9+)
- Vite (para los proyectos en `proyectos/`)
- pnpm (gestor de paquetes)
- Vitest (para testing en el modulo 9)
- Node.js
- VSCode
- React (solo en el modulo 8)

No utilizar:

- Tailwind CSS
- Bootstrap
- npm (usar pnpm en todos los comandos)
- Frameworks JS (salvo React en el modulo 8)
- Cualquier otro framework o libreria no listada arriba

---

# Forma de ensenar

Cada leccion debe seguir exactamente esta estructura.

# 1. Introduccion

Explicar que vamos a aprender.
Por que existe.
Que problema resuelve.
Cuando se utiliza.
Errores tipicos.

---

# 2. Teoria

Explicar el concepto con mucho detalle.
NO asumir conocimientos previos de TypeScript.
Utilizar comparaciones del mundo real.
Explicar el porque de cada decision.
No limitarse a decir "asi funciona TypeScript".
Quiero entender el diseno interno y la filosofia.

---

# 3. Ejemplo minimo

Mostrar el ejemplo mas pequeno posible.
Explicar linea por linea.

---

# 4. Ejemplo real

Crear un ejemplo mas cercano a una aplicacion profesional.
Explicar todas las decisiones.

---

# 5. Buenas practicas

Explicar:
- que hacer
- que evitar
- por que

---

# 6. Errores habituales

Mostrar ejemplos incorrectos.
Explicar por que estan mal.
Mostrar la version correcta.

---

# 7. Ejercicio guiado

Crear un ejercicio.
No demasiado dificil.
Resolverlo paso a paso.

---

# 8. Reto

Proponer un reto adicional.
NO resolverlo inmediatamente.
Esperar a que el alumno lo intente.

---

# 9. Resumen

Resumen de la leccion.
Conceptos importantes.

---

# 10. Preguntas frecuentes

Anadir preguntas frecuentes relacionadas con la leccion.

---

# Codigo

Todo el codigo debe estar completamente comentado.
Cada linea importante debe explicarse.

---

# Organizacion

Cada modulo debe estar dividido en lecciones.
Cada leccion debe ser independiente.

---

# Indice del curso

# Modulo 0 — Preparacion

- Que es TypeScript y por que existe (25 min)
- Instalacion: Vite, TypeScript y tsconfig.json (35 min)
- Flujo de trabajo (25 min)

---

# Modulo 1 — Tipos basicos

- Tipos primitivos, inferencia y anotaciones (30 min)
- Arrays, tuples y readonly (30 min)
- Enums y const assertions (30 min)
- any, unknown, never y modo strict (30 min)

---

# Modulo 2 — Objetos e interfaces

- Interfaces, opcionales y readonly (30 min)
- Type aliases vs interfaces (30 min)
- Index signatures e intersection (30 min)
- Discriminated unions y literales (30 min)

---

# Modulo 3 — Funciones

- Tipado de parametros y retorno (30 min)
- Callbacks y function types (30 min)
- Overloads y genericos en funciones (30 min)
- this en TypeScript y declare (30 min)

---

# Modulo 4 — Unions y narrowing

- Union types y type guards (30 min)
- Discriminated unions exhaustivos (30 min)
- Assertion y type predicates (30 min)
- satisfies, as const, template literals (30 min)

---

# Modulo 5 — Genericos

- Genericos basicos (30 min)
- Restricciones extends (30 min)
- keyof, indexed, mapped types (30 min)
- Infer y conditional types (30 min)

---

# Modulo 6 — Utility Types

- Partial, Required, Pick, Omit (30 min)
- Record, Exclude, Extract (30 min)
- ReturnType, Parameters, Awaited (30 min)
- Template literals y branded types (30 min)

---

# Modulo 7 — Modulos y configuracion

- Modulos ES y import type (30 min)
- tsconfig.json en profundidad (30 min)
- Declaration files .d.ts (30 min)
- Monorepos y project references (30 min)

---

# Modulo 8 — TypeScript con React

- Setup React + TypeScript + Vite (35 min)
- Props y eventos tipados (30 min)
- Hooks con genericos (30 min)
- Proyecto: app tipada con API (50 min, project:true)

---

# Modulo 9 — Proyecto final

Aplicacion completa paso a paso que incluye:

- Arquitectura del proyecto final (45 min)
- CRUD completo tipado (40 min)
- Testing con Vitest tipado (40 min)
- Build de produccion y despliegue (40 min)

---

# Modulo 10 — Tipos avanzados (ampliacion)

- Recursion de tipos con template literals (35 min)
- infer en funciones: ReturnType y Parameters a mano (30 min)
- Branded types con validacion en runtime (35 min)
- Strict mode y exhaustividad en profundidad (35 min)

---

# Modulo 11 — Genericos en profundidad (ampliacion)

- Restricciones, defaults e inferencia contextual (30 min)
- Variadic tuple types (30 min)
- Mapped types y remapeo de claves (35 min)
- Builder tipado: fluent APIs con genericos (35 min)

---

# Modulo 12 — Monorepos y produccion (ampliacion)

- Monorepos con project references (40 min)
- Contratos end-to-end tipados (40 min)
- Publicar librerias tipadas (40 min)
- TypeScript en produccion y CI (35 min)

---

# Buenas practicas

Explicar siempre:

- cuando utilizar cada solucion
- ventajas
- inconvenientes
- alternativas
- rendimiento
- mantenibilidad

---

# Comparaciones

Cuando aparezca una caracteristica nueva, compararla con:

- JavaScript puro (vanilla)
- Otros lenguajes tipados
- Alternativas de tipado (Flow, JSDoc)

---

# Diagramas

Cuando sea util, utilizar diagramas ASCII para explicar flujos de tipos y relaciones entre entidades.

---

# Estilo

El estilo debe ser:

- Muy didactico.
- Muy detallado.
- Facil de leer.
- Profesional.
- Sin asumir conocimientos previos de TypeScript (pero si de JavaScript).
- Explicar siempre el "por que".
- Utilizar analogias cuando ayuden a comprender.

---

# Formato de entrega

El curso se entrega como **web estatica** (HTML + CSS + JavaScript vanilla, sin Markdown y sin paso de build). La carpeta `web/` contiene todo el contenido navegable: cada leccion es un archivo HTML completo y el chrome (cabecera, sidebar, progreso, tema claro/oscuro) lo inyecta `web/assets/js/site.js` a partir del mapa del curso definido en `web/assets/js/course-data.js` (unica fuente de verdad para navegacion y enlaces).

Los proyectos de fin de modulo en `proyectos/` son aplicaciones Vite + TypeScript reales y ejecutables.

Estructura real del repositorio:

```text
Learn TypeScript/
│
├── AGENTS.md
├── package.json               ← scripts de servidor estatico
├── pnpm-workspace.yaml        ← proyectos/*
│
├── web/                       ← ★ EL CURSO (web estatica, sin build)
│   ├── index.html             ← portada + indice completo
│   ├── assets/css/            ← site.css (tema claro/oscuro)
│   ├── assets/js/             ← course-data.js, highlight.js, site.js
│   ├── modulos/
│   │   ├── 00-preparacion/
│   │   ├── 01-tipos-basicos/
│   │   ├── 02-objetos-interfaces/
│   │   ├── 03-funciones/
│   │   ├── 04-unions-narrowing/
│   │   ├── 05-genericos/
│   │   ├── 06-utility-types/
│   │   ├── 07-modulos-config/
│   │   ├── 08-typescript-react/
│   │   └── 09-proyecto-final/
│   └── recursos/              ← cheatsheet, glosario, FAQ
│
├── ejemplos/                  ← mini-demos TypeScript (uno por modulo)
└── proyectos/                 ← Vite + TypeScript (proyectos de fin de modulo)
```

# Convenciones del curso

- Idioma: contenido, comentarios y UI en espanol.
- Identificadores de codigo en ingles (estandar profesional).
- La web del curso usa tema oscuro por defecto con toggle a claro.
- TypeScript 7 en modo strict siempre.
- Cada leccion sigue SIEMPRE las 10 secciones definidas en "Forma de ensenar".

---

# Reglas importantes

1. Nunca des por sentado un concepto.
2. Explica siempre el "por que".
3. Todo el codigo debe ejecutarse y compilar sin errores.
4. Usa unicamente `pnpm` en los comandos.
5. Sigue las buenas practicas actuales de TypeScript 7 (compatibles con 5.9+).
6. No omitas pasos de configuracion.
7. Manten una progresion logica entre las lecciones.
8. Cada modulo que lo requiera debe terminar con un proyecto practico.
9. Si un concepto depende de otro, enlaza a la leccion correspondiente.
10. Incluye consejos de depuracion con el compilador de TypeScript y VSCode cuando sea pertinente.
11. Prioriza la comprension profunda frente a la velocidad.
12. El alumno ya sabe JavaScript. No expliques conceptos basicos de JS, centrate en el tipado.
