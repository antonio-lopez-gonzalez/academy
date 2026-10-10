/**
 * course-data.js — MAPA COMPLETO DEL CURSO TYPESCRIPT
 *
 * Unica fuente de verdad para:
 *  - el sidebar (arbol de modulos y lecciones)
 *  - la navegacion anterior / siguiente
 *  - el indice de la portada
 *  - el calculo de progreso
 *
 * Cada leccion apunta a un archivo HTML real dentro de web/modulos/.
 * Si anades o renombras una leccion, edita SOLO este archivo.
 */
window.COURSE = {
  title: "Curso TypeScript",
  subtitle: "De JavaScript a TypeScript profesional",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de TypeScript" },
    { file: "recursos/glosario.html", title: "Glosario de terminos" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MODULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Preparacion",
      description: "Que es TypeScript, por que existe y como montar tu entorno de desarrollo con Vite.",
      lessons: [
        { file: "01-que-es-typescript.html", title: "Que es TypeScript y por que existe", minutes: 25 },
        { file: "02-instalacion-y-tsconfig.html", title: "Instalacion: Vite, TypeScript y tsconfig.json", minutes: 35 },
        { file: "03-flujo-de-trabajo.html", title: "Flujo de trabajo", minutes: 25 },
      ],
    },
    /* ============================================================ MODULO 1 */
    {
      id: "01",
      dir: "modulos/01-tipos-basicos",
      title: "Tipos basicos",
      description: "Tipos primitivos, inferencia, anotaciones explicitas, arrays, tuples, enums y los tipos especiales any, unknown y never.",
      lessons: [
        { file: "01-tipos-primitivos.html", title: "Tipos primitivos, inferencia y anotaciones", minutes: 30 },
        { file: "02-arrays-y-tuples.html", title: "Arrays, tuples y readonly", minutes: 30 },
        { file: "03-enums-y-assertions.html", title: "Enums y const assertions", minutes: 30 },
        { file: "04-any-unknown-never.html", title: "any, unknown, never y modo strict", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 2 */
    {
      id: "02",
      dir: "modulos/02-objetos-interfaces",
      title: "Objetos e interfaces",
      description: "Interfaces, propiedades opcionales y readonly, type aliases, index signatures, intersection types y discriminated unions.",
      lessons: [
        { file: "01-interfaces-basicas.html", title: "Interfaces, opcionales y readonly", minutes: 30 },
        { file: "02-type-aliases.html", title: "Type aliases vs interfaces", minutes: 30 },
        { file: "03-index-intersection.html", title: "Index signatures e intersection", minutes: 30 },
        { file: "04-discriminated-unions.html", title: "Discriminated unions y literales", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 3 */
    {
      id: "03",
      dir: "modulos/03-funciones",
      title: "Funciones",
      description: "Tipado de parametros y retorno, callbacks, function types, overloads, genericos en funciones y el manejo de this.",
      lessons: [
        { file: "01-tipado-parametros.html", title: "Tipado de parametros y retorno", minutes: 30 },
        { file: "02-callbacks-types.html", title: "Callbacks y function types", minutes: 30 },
        { file: "03-overloads.html", title: "Overloads y genericos en funciones", minutes: 30 },
        { file: "04-this-declare.html", title: "this en TypeScript y declare", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 4 */
    {
      id: "04",
      dir: "modulos/04-unions-narrowing",
      title: "Unions y narrowing",
      description: "Union types, type guards, discriminated unions exhaustivos, assertion functions, type predicates, satisfies y template literals.",
      lessons: [
        { file: "01-union-types-guards.html", title: "Union types y type guards", minutes: 30 },
        { file: "02-discriminated-exhaustive.html", title: "Discriminated unions exhaustivos", minutes: 30 },
        { file: "03-assertion-predicates.html", title: "Assertion y type predicates", minutes: 30 },
        { file: "04-satisfies-template.html", title: "satisfies, as const, template literals", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 5 */
    {
      id: "05",
      dir: "modulos/05-genericos",
      title: "Genericos",
      description: "Genericos basicos, restricciones con extends, keyof, indexed access types, mapped types, infer y conditional types.",
      lessons: [
        { file: "01-genericos-basicos.html", title: "Genericos basicos", minutes: 30 },
        { file: "02-restricciones.html", title: "Restricciones extends", minutes: 30 },
        { file: "03-keyof-mapped.html", title: "keyof, indexed, mapped types", minutes: 30 },
        { file: "04-infer-conditionals.html", title: "Infer y conditional types", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 6 */
    {
      id: "06",
      dir: "modulos/06-utility-types",
      title: "Utility Types",
      description: "Partial, Required, Pick, Omit, Record, Exclude, Extract, ReturnType, Parameters, Awaited, template literals y branded types.",
      lessons: [
        { file: "01-partial-pick-omit.html", title: "Partial, Required, Pick, Omit", minutes: 30 },
        { file: "02-record-exclude.html", title: "Record, Exclude, Extract", minutes: 30 },
        { file: "03-returntype-parameters.html", title: "ReturnType, Parameters, Awaited", minutes: 30 },
        { file: "04-template-branded.html", title: "Template literals y branded types", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 7 */
    {
      id: "07",
      dir: "modulos/07-modulos-config",
      title: "Modulos y configuracion",
      description: "Modulos ES, import type, tsconfig.json en profundidad, declaration files .d.ts, monorepos y project references.",
      lessons: [
        { file: "01-modulos-typescript.html", title: "Modulos ES y import type", minutes: 30 },
        { file: "02-tsconfig-profundidad.html", title: "tsconfig.json en profundidad", minutes: 30 },
        { file: "03-declaration-files.html", title: "Declaration files .d.ts", minutes: 30 },
        { file: "04-monorepos.html", title: "Monorepos y project references", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 8 */
    {
      id: "08",
      dir: "modulos/08-typescript-react",
      title: "TypeScript con React",
      description: "Setup de React + TypeScript + Vite, props y eventos tipados, hooks con genericos y proyecto de app tipada con API REST.",
      lessons: [
        { file: "01-react-con-typescript.html", title: "Setup React + TypeScript + Vite", minutes: 35 },
        { file: "02-props-events.html", title: "Props y eventos tipados", minutes: 30 },
        { file: "03-hooks-genericos.html", title: "Hooks con genericos", minutes: 30 },
        { file: "04-proyecto-app-tipada.html", title: "Proyecto: app tipada con API", minutes: 50, project: true },
      ],
    },
    /* ============================================================ MODULO 9 */
    {
      id: "09",
      dir: "modulos/09-proyecto-final",
      title: "Proyecto final",
      description: "Aplicacion full-stack tipada paso a paso: arquitectura, CRUD, testing con Vitest y build de produccion con despliegue.",
      lessons: [
        { file: "01-arquitectura.html", title: "Arquitectura del proyecto final", minutes: 45, project: true },
        { file: "02-crud-tipado.html", title: "CRUD completo tipado", minutes: 40, project: true },
        { file: "03-testing.html", title: "Testing con Vitest tipado", minutes: 40, project: true },
        { file: "04-build-despliegue.html", title: "Build de produccion y despliegue", minutes: 40, project: true },
      ],
    },
  ],
};
