/**
 * course-data.js — MAPA COMPLETO DEL CURSO PYTHON MODERNO
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
  title: "Python",
  subtitle: "De script a paquete · Python 3.12+ con proyecto real (Biblioteca)",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de Python" },
    { file: "recursos/glosario.html", title: "Glosario de Python" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Preparación",
      description: "Qué es Python, cómo montar el entorno (Python 3.12+, VSCode) y cómo se ejecuta; presentación del caso de estudio Biblioteca.",
      lessons: [
        { file: "01-que-es-python.html", title: "Qué es Python", minutes: 30 },
        { file: "02-entorno-python-vscode.html", title: "Entorno: Python, VSCode y el REPL", minutes: 35 },
        { file: "03-como-se-ejecuta-python.html", title: "Cómo se ejecuta Python: script, -c, -m y REPL", minutes: 30 },
        { file: "04-caso-biblioteca.html", title: "Caso de estudio: Biblioteca", minutes: 25 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-fundamentos",
      title: "Fundamentos del lenguaje",
      description: "Nombres y tipos, números y strings, listas y tuplas, diccionarios y conjuntos, entrada/salida y conversiones.",
      lessons: [
        { file: "01-variables-y-tipos.html", title: "Variables y tipos: todo es un objeto", minutes: 30 },
        { file: "02-numeros-y-strings.html", title: "Números, strings y f-strings", minutes: 35 },
        { file: "03-listas-y-tuplas.html", title: "Listas y tuplas", minutes: 35 },
        { file: "04-dicts-y-sets.html", title: "Diccionarios y conjuntos", minutes: 35 },
        { file: "05-entrada-salida-y-conversion.html", title: "print, input, conversiones y None", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-control-y-funciones",
      title: "Control de flujo y funciones",
      description: "Condicionales y truthiness, bucles, funciones, *args/**kwargs y ámbito LEGB con closures.",
      lessons: [
        { file: "01-condicionales-y-truthiness.html", title: "Condicionales y truthiness", minutes: 30 },
        { file: "02-bucles.html", title: "Bucles: for, while, range y enumerate", minutes: 30 },
        { file: "03-funciones.html", title: "Funciones y parámetros", minutes: 35 },
        { file: "04-args-kwargs-y-desempaquetado.html", title: "*args, **kwargs y desempaquetado", minutes: 30 },
        { file: "05-scope-legb-y-closures.html", title: "Ámbito LEGB y closures", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-colecciones-avanzadas",
      title: "Colecciones avanzadas y estilo pythonico",
      description: "Comprehensions, iteradores y generadores, funciones integradas y slicing/copias.",
      lessons: [
        { file: "01-comprehensiones.html", title: "List, set y dict comprehensions", minutes: 35 },
        { file: "02-iteradores-y-generadores.html", title: "Iteradores y generadores", minutes: 35 },
        { file: "03-funciones-integradas.html", title: "Funciones integradas: sorted, zip, any/all", minutes: 30 },
        { file: "04-slicing-y-copias.html", title: "Slicing, copias y aliasing", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-modulos-paquetes-entornos",
      title: "Módulos, paquetes y entornos",
      description: "Imports y paquetes, pyproject.toml, entornos virtuales con venv/pip y uv, y la CLI con argparse.",
      lessons: [
        { file: "01-modulos-y-paquetes.html", title: "Módulos y paquetes", minutes: 30 },
        { file: "02-pyproject-y-proyectos.html", title: "pyproject.toml y estructura de proyecto", minutes: 35 },
        { file: "03-venv-y-pip.html", title: "Entornos virtuales: venv y pip", minutes: 35 },
        { file: "04-uv.html", title: "uv: el gestor moderno", minutes: 30 },
        { file: "05-cli-con-argparse.html", title: "La CLI de Biblioteca con argparse", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-poo",
      title: "Programación orientada a objetos",
      description: "Clases y objetos, métodos y encapsulación, herencia y composición, dataclasses y dunders/protocolos.",
      lessons: [
        { file: "01-clases-y-objetos.html", title: "Clases y objetos", minutes: 35 },
        { file: "02-metodos-y-encapsulacion.html", title: "Métodos, propiedades y encapsulación", minutes: 30 },
        { file: "03-herencia-y-composicion.html", title: "Herencia y composición", minutes: 35 },
        { file: "04-dataclasses.html", title: "Dataclasses", minutes: 30 },
        { file: "05-dunders-y-protocolos.html", title: "Dunders y protocolos (duck typing)", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-errores-ficheros-datos",
      title: "Errores, ficheros y datos",
      description: "Excepciones, context managers, ficheros con pathlib y persistencia JSON del catálogo.",
      lessons: [
        { file: "01-excepciones.html", title: "Excepciones y errores propios", minutes: 35 },
        { file: "02-context-managers.html", title: "Context managers y with", minutes: 30 },
        { file: "03-ficheros-y-pathlib.html", title: "Ficheros y pathlib", minutes: 30 },
        { file: "04-json-y-persistencia.html", title: "JSON y persistencia del catálogo", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-testing-y-calidad",
      title: "Testing y calidad",
      description: "pytest, fixtures y parametrize, tipado con mypy, y lint/formato con ruff y black.",
      lessons: [
        { file: "01-pytest.html", title: "Introducción a pytest", minutes: 35 },
        { file: "02-fixtures-y-parametrize.html", title: "Fixtures, tmp_path y parametrize", minutes: 35 },
        { file: "03-typing-y-mypy.html", title: "Type hints y mypy", minutes: 35 },
        { file: "04-lint-y-formato.html", title: "PEP 8, ruff y black", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 8 */
    {
      id: "08",
      dir: "modulos/08-python-moderno-async",
      title: "Python moderno y asincronía",
      description: "Decoradores, generadores avanzados, pattern matching, datetime/logging y async/concurrencia.",
      lessons: [
        { file: "01-decoradores.html", title: "Decoradores", minutes: 35 },
        { file: "02-generadores-avanzados.html", title: "Generadores avanzados e itertools", minutes: 30 },
        { file: "03-pattern-matching.html", title: "Pattern matching (match/case)", minutes: 30 },
        { file: "04-datetime-y-logging.html", title: "datetime y logging", minutes: 30 },
        { file: "05-async-y-concurrencia.html", title: "Concurrencia: GIL, threading y asyncio", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 9 */
    {
      id: "09",
      dir: "modulos/09-proyecto-final",
      title: "Proyecto final: Biblioteca empaquetada",
      description: "Arquitectura del paquete, núcleo, tests y calidad, y empaquetado con entry point e instalación.",
      lessons: [
        { file: "01-arquitectura-y-paquete.html", title: "Arquitectura del paquete", minutes: 30 },
        { file: "02-implementar-nucleo.html", title: "Implementar el núcleo", minutes: 35 },
        { file: "03-tests-y-calidad.html", title: "Tests y calidad", minutes: 35 },
        { file: "04-empaquetar-y-entregar.html", title: "Empaquetar y entregar", minutes: 35 },
      ],
    },
  ],
};