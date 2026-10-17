# Página web estática para aprender Python moderno.

# Curso Completo de Python (3.12+)

## De cero a profesional con proyectos reales

---

# 1. Objetivo

Actúa como un ingeniero senior de Python con muchos años de experiencia formando desarrolladores.

El objetivo es crear **un curso completo de Python moderno (3.12+)** pensado para un **desarrollador que ya sabe programar** (en cualquier lenguaje: JavaScript, C#, Java, Go…) y quiere dominar Python en profundidad, o para quien ya lo toca y quiere llenar los huecos.

El curso debe ser **extremadamente didáctico** y orientado al **por qué**, no a memorizar sintaxis.

No quiero únicamente memorizar `def`, `for` o `with`; quiero entender **por qué Python funciona como funciona**: por qué la indentación es significativa, por qué todo es un objeto, qué implica el tipado dinámico, cómo funcionan la mutabilidad y la identidad (`is` vs `==`), por qué existen los generadores, cómo se resuelve el ámbito (LEGB y closures), qué papel juegan el GIL y el modelo de ejecución en la asincronía, y cómo se escribe código **pythonico** que otros entiendan y que las herramientas (pytest, mypy, ruff) puedan verificar.

El alumno debe terminar el curso **sabiendo escribir Python idiomático, testeado, tipado y empaquetable**, no scripts sueltos.

---

# 2. Audiencia y requisitos previos

- Desarrolladores/as que **ya programan** (variables, funciones, bucles, colecciones). El curso va al grano y compara con otros lenguajes cuando aporta (JS, C#, Java).
- Se asume: terminal, Git básico, editar archivos y ejecutar comandos.
- **No** se asume conocimiento previo de Python ni de su ecosistema (`venv`, `pip`, `uv`, `pytest`, `pyproject.toml`).
- Toda la teoría se acompaña de **código ejecutable** y de un proyecto real que crece módulo a módulo.

---

# 3. Hilo conductor: Biblioteca

Todo el curso gira alrededor de **un proyecto real** que se construye de forma incremental:

> **Biblioteca** — un catálogo personal de **libros y películas** compuesto por:
>
> 1. `bib` — una **CLI en Python** que añade, lista, busca y presta/devolver ítems.
> 2. Un **almacén JSON** (`data/library.json`) validado por un esquema.
> 3. Un paquete Python real (`biblioteca/`) con tests, tipos y empaquetado.

El alumno verá el mismo sistema evolucionar:

- **Módulo 0–3:** se escriben los primeros scripts y se modela el catálogo (colecciones y funciones).
- **Módulo 4:** el script se convierte en un **paquete con CLI** (`argparse`), con entorno virtual (`venv + pip` y `uv`).
- **Módulo 5:** los ítems pasan a ser **clases / dataclasses** con comportamiento.
- **Módulo 6:** persistencia real en **JSON** con manejo de errores y `pathlib`.
- **Módulo 7:** el catálogo se **testea con pytest**, se **tipa** y se formatea (mypy, ruff, black).
- **Módulo 8:** se añaden **decoradores, pattern matching, logging** y una importación **asíncrona** de datos.
- **Módulo 9:** se **empaqueta** (`pyproject.toml`, entrada de consola) y se entrega.

Ver el mismo producto pasar de "script suelto" a "paquete instalable con tests" cierra el círculo y justifica cada herramienta.

---

# 4. Tecnologías

## Utilizar SIEMPRE

- **Python 3.12+** (sintaxis moderna: f-strings, match/case, type hints, `tomllib`).
- **Entornos virtuales**: `venv` + `pip` (vía estándar) y **`uv`** (vía moderna, rápida, con lockfile) — se enseñan **ambos**.
- **pytest** para testing (asserts, fixtures, `parametrize`, `monkeypatch`/mocks).
- **Type hints** + **mypy** para tipado estático opcional.
- **ruff** (lint + formato rápido; se menciona **black** como formateador clásico).
- **argparse** (biblioteca estándar) para la CLI; se menciona **Typer/Click** solo como comparativa.
- **pathlib** y `json` (biblioteca estándar) para ficheros y persistencia.
- **logging** y **datetime** (biblioteca estándar) para operación.
- **VSCode** con extensión **Python** (y **Pylance**), o el editor que use el alumno.
- **pnpm** solo para el servidor estático de la web del curso (no para el código de ejemplo).

## No utilizar

- **Python 2** (ni `print` como sentencia, ni `unicode`): todo es Python 3.12+.
- Frameworks web (Django/Flask/FastAPI) como requisito del curso: se mencionan como siguiente paso, no se enseñan.
- Librerías de terceros donde la biblioteca estándar basta (priorizar la stdlib).
- **conda/pipenv** como gestor principal (se mencionan como alternativas existentes).
- Notebooks (Jupyter) como vehículo de la enseñanza: los ejemplos son ficheros `.py` ejecutables. Jupyter se menciona como herramienta de exploración.
- `sudo pip install` / instalar paquetes globalmente sin entorno virtual.

---

# 5. Forma de enseñar

Cada lección debe seguir **exactamente** esta estructura (numeración visible en el HTML con la clase `section-number`).

## 1. Introducción
Qué vamos a aprender. Por qué existe. Qué problema resuelve. Cuándo se utiliza. Errores típicos de esa pieza.

## 2. Teoría
Explicar el concepto con mucho detalle, **sin asumir conocimientos previos de Python**. Comparaciones del mundo real. Explicar el **porqué** de cada decisión de diseño del lenguaje (indentación, objetos, mutabilidad, protocolo de iteración, GIL…). Diagramas ASCII cuando ayuden (modelo de objetos, resolución de nombres LEGB, flujo async).

## 3. Ejemplo mínimo
El fragmento más pequeño posible (3–10 líneas de Python). Explicar **línea por línea**, incluida la salida esperada.

## 4. Ejemplo real
El mismo concepto aplicado a **Biblioteca** o a un escenario profesional. Explicar todas las decisiones.

## 5. Buenas prácticas
Qué hacer / qué evitar / por qué. Tabla cuando aplique. Estilo **PEP 8** y código *pythonico*.

## 6. Errores habituales
Ejemplos incorrectos (mutables como default, `==` vs `is`, indentation errors, shadows de builtins, excepciones silenciadas). Por qué están mal. Versión correcta.

## 7. Ejercicio guiado
Ejercicio paso a paso, no demasiado difícil, resuelto al detalle, **con el resultado esperado** y cómo comprobarlo.

## 8. Reto
Reto adicional **sin resolverlo** en el HTML (pista incluida).

## 9. Resumen
Bullets + conceptos clave. Tabla "Concepto | Para qué sirve" o "Método | Qué hace" cuando aplique.

## 10. Preguntas frecuentes
3–5 `<details>` relacionados con la lección.

## Reglas de código
- Todo el código debe estar **completamente comentado** (comentarios en español) y **ejecutarse** con Python 3.12+.
- Mostrar la **salida esperada** de los ejemplos, o explicar cómo obtenerla.
- Identificadores en **inglés**; prosa y comentarios en **español**.
- Cuando haya varias formas de hacer algo, mostrar la **pythonica** y mencionar las alternativas (y por qué NO usarlas).

---

# 6. Multiplataforma: Linux / macOS / Windows

Python corre **nativo** en los tres sistemas; no hace falta WSL (a diferencia de Docker). Aun así, las diferencias se explican como contenido:

| Tema | Linux / macOS | Windows |
|---|---|---|
| Lanzar Python | `python3` (o `python`) | **`py`** (Python Launcher) o `python` |
| Entorno virtual | `python -m venv .venv` | `py -m venv .venv` |
| Activar venv | `source .venv/bin/activate` | `.venv\Scripts\Activate.ps1` (PowerShell) o `Activate.bat` |
| Gestor de paquetes | `pip` / `uv` | `pip` / `uv` (iguales) |
| Rutas | `/home/usuario/proyectos/biblioteca` | `C:\Users\...\proyectos\biblioteca` |
| Finales de línea / scripts | LF | cuidado con CRLF en scripts (`.gitattributes` con `eol=lf`) |
| Shell | bash/zsh | **PowerShell** (callout con el equivalente) |

- Shell canónico de los ejemplos: **bash**; callout con el equivalente **PowerShell** cuando la sintaxis cambie (`$env:VAR` vs `$VAR`, rutas, activación de venv).
- Se menciona **pyenv/asdf** (Linux/macOS) y el **py launcher de Windows** para gestionar versiones, y que **`uv`** también gestiona versiones de Python.
- Opcional: en Windows también se puede trabajar dentro de **WSL2** si el alumno ya lo usa, pero no es requisito.

---

# 7. Índice del curso

## Módulo 0 — Preparación
- **01-que-es-python.html** — Qué es Python: intérprete y bytecode, CPython, filosofía (`import this`), dónde se usa, por qué ahora.
- **02-entorno-python-vscode.html** — Instalación y entorno: Python 3.12+, `py`/`python`, REPL, VSCode + extensión, primer script.
- **03-como-se-ejecuta-python.html** — Ejecución: `python script.py`, `-c`, `-m`, el REPL, archivos `.py`/`.pyi`, el `__pycache__` y bytecode.
- **04-caso-biblioteca.html** — Caso de estudio Biblioteca: qué construiremos, arquitectura, repositorio y plan del curso.

## Módulo 1 — Fundamentos del lenguaje
- **01-variables-y-tipos.html** — Nombres, asignación, tipado dinámico, `type()`, `isinstance`, todo es un objeto.
- **02-numeros-y-strings.html** — `int`/`float`/`complex`, operadores, f-strings, métodos de `str`, inmutabilidad.
- **03-listas-y-tuplas.html** — Listas (mutables) y tuplas (inmutables), índices, slicing, métodos, desempaquetado.
- **04-dicts-y-sets.html** — Diccionarios y conjuntos, hashabilidad, acceso, métodos, `in`.
- **05-entrada-salida-y-conversion.html** — `print` (sep/end), `input`, conversiones (`int`, `str`, `list`…), `None` y truthiness.

## Módulo 2 — Control de flujo y funciones
- **01-condicionales-y-truthiness.html** — `if/elif/else`, operadores lógicos, valores truthy/falsy, ternario, `match` básico.
- **02-bucles.html** — `for`, `while`, `range`, `enumerate`, `zip`, `break/continue/else`, iterables.
- **03-funciones.html** — `def`, parámetros, `return`, argumentos por defecto, keyword vs positional, docstrings.
- **04-args-kwargs-y-desempaquetado.html** — `*args`, `**kwargs`, keyword-only, positional-only, desempaquetado de colecciones.
- **05-scope-legb-y-closures.html** — Ámbito LEGB, `global`/`nonlocal`, funciones de primera clase, closures.

## Módulo 3 — Colecciones avanzadas y estilo pythonico
- **01-comprehensiones.html** — List/set/dict comprehensions, filtros, anidados, cuándo NO usarlas.
- **02-iteradores-y-generadores.html** — Protocolo de iteración, `iter`/`next`, `yield`, pereza y memoria.
- **03-funciones-integradas.html** — `sorted` con `key`, `map`/`filter`, `any`/`all`, `sum`/`min`/`max`, `functools`.
- **04-slicing-y-copias.html** — Slicing avanzado, paso negativo, copias superficiales vs profundas (`copy`), aliasing.

## Módulo 4 — Módulos, paquetes y entornos
- **01-modulos-y-paquetes.html** — `import` (formas), módulos, paquetes, `__init__.py`, imports relativos, `sys.path`.
- **02-pyproject-y-proyectos.html** — `pyproject.toml`, metadatos, dependencias, estructura de un proyecto (`src/` layout).
- **03-venv-y-pip.html** — Entornos virtuales, `python -m venv`, activar/desactivar, `pip install`, `requirements.txt`.
- **04-uv.html** — `uv`: por qué y cuándo, `uv venv`, `uv add`, `uv run`, lockfile, gestión de versiones de Python.
- **05-cli-con-argparse.html** — Convertir el script en CLI `bib`: `argparse`, subcomandos, `__main__`, códigos de salida.

## Módulo 5 — Programación orientada a objetos
- **01-clases-y-objetos.html** — `class`, instancias, `self`, atributos de instancia vs de clase, `__init__`.
- **02-metodos-y-encapsulacion.html** — Métodos, `@property`, convenciones `_`/`__`, `@staticmethod`/`@classmethod`.
- **03-herencia-y-composicion.html** — Herencia, `super()`, MRO, cuándo componer en vez de heredar, ABC.
- **04-dataclasses.html** — `@dataclass`, campos, `field`, `default_factory`, `frozen`, `order`; modelar un `Item`.
- **05-dunders-y-protocolos.html** — `__repr__`/`__str__`, `__eq__`, `__len__`, `__iter__`, dunders y duck typing.

## Módulo 6 — Errores, ficheros y datos
- **01-excepciones.html** — `try/except/else/finally`, `raise`, jerarquía de excepciones, excepciones propias.
- **02-context-managers.html** — `with`, protocolo de contexto (`__enter__`/`__exit__`), `contextlib`.
- **03-ficheros-y-pathlib.html** — Leer/escribir texto y binario, codificaciones, `pathlib.Path`, `with open`.
- **04-json-y-persistencia.html** — `json`, serializar/deserializar, validar el almacén de Biblioteca, manejo de datos corruptos.

## Módulo 7 — Testing y calidad
- **01-pytest.html** — Por qué tests, `pytest`, `assert`, organización, ejecutar y leer fallos.
- **02-fixtures-y-parametrize.html** — Fixtures, `tmp_path`, `parametrize`, `monkeypatch`, aislar el almacén.
- **03-typing-y-mypy.html** — Anotaciones, `Optional`, `list[str]`, `Protocol`, mypy y errores típicos.
- **04-lint-y-formato.html** — PEP 8, `ruff` (lint + format), `black`, pre-commit, ordenar imports, docstrings.

## Módulo 8 — Python moderno y asincronía
- **01-decoradores.html** — Funciones que envuelven funciones, `functools.wraps`, decoradores con argumentos, casos reales.
- **02-generadores-avanzados.html** — `yield from`, generadores infinitos, `itertools`, pipeline perezoso.
- **03-pattern-matching.html** — `match/case`, patrones, guardas, desestructuración, cuándo usarlo (¡y cuándo no!).
- **04-datetime-y-logging.html** — `datetime`/`zoneinfo`, `logging` (niveles, handlers, formato), configurar logs de la CLI.
- **05-async-y-concurrencia.html** — GIL, `threading` vs `multiprocessing` vs `asyncio`, `async/await`, `asyncio.gather`, `httpx`/`urllib` (concepto).

## Módulo 9 — Proyecto final: Biblioteca empaquetada
- **01-arquitectura-y-paquete.html** — Estructura `src/biblioteca/`, módulos, `pyproject.toml`, separar núcleo de la CLI.
- **02-implementar-nucleo.html** — Modelo de datos, almacén, reglas de negocio, errores propios, logging.
- **03-tests-y-calidad.html** — Suite de pytest completa, tipado con mypy, ruff/black en verde.
- **04-empaquetar-y-entregar.html** — `[project.scripts]`, `uv build`, instalar el paquete, README, verificación end-to-end.

## Recursos
- **cheatsheet.html** — Referencia rápida de Python (tipos, colecciones, comprehensions, ficheros, pytest, CLI).
- **glosario.html** — Glosario de Python (objeto, mutabilidad, iterable, generador, dunder, GIL…).
- **faq.html** — FAQ general del curso.

**Total: 10 módulos (0–9), 45 lecciones + 3 recursos.**

---

# 8. Estructura de repositorio

```text
Aprende Python/
│
├── specs.md                     ← ★ ESTE DOCUMENTO
├── AGENTS.md                    ← puntero a specs.md + reglas para agentes
├── package.json                 ← script de servidor estático
├── pnpm-workspace.yaml
├── .gitignore
├── .gitattributes               ← LF (evita CRLF en scripts .py y .sh)
│
│   (skills centralizadas en `Academy/.agents/skills/`: documentation,
│    frontend-design, academy-course, python-course)
│
├── web/                         ← ★ EL CURSO (web estática, sin build)
│   ├── index.html               ← portada + índice
│   ├── assets/
│   │   ├── css/site.css         ← identidad Python
│   │   ├── js/course-data.js    ← mapa del curso (única fuente de verdad)
│   │   ├── js/highlight.js      ← resaltado de sintaxis (copiar)
│   │   ├── js/site.js           ← chrome: `py-theme` / `py-progress`
│   │   ├── fonts/               ← JetBrains Mono (copiar)
│   │   └── favicon.svg
│   ├── modulos/
│   │   ├── 00-preparacion/
│   │   ├── 01-fundamentos/
│   │   ├── 02-control-y-funciones/
│   │   ├── 03-colecciones-avanzadas/
│   │   ├── 04-modulos-paquetes-entornos/
│   │   ├── 05-poo/
│   │   ├── 06-errores-ficheros-datos/
│   │   ├── 07-testing-y-calidad/
│   │   ├── 08-python-moderno-async/
│   │   └── 09-proyecto-final/
│   └── recursos/                ← cheatsheet.html, glosario.html, faq.html
│
├── scripts/serve-web.mjs        ← servidor estático (node)
│
└── proyectos/
    └── biblioteca/              ← ★ PROYECTO FINAL (ejecutable de verdad)
        ├── README.md
        ├── pyproject.toml       ← paquete + entry point `bib`
        ├── src/biblioteca/
        │   ├── __init__.py
        │   ├── models.py        ← dataclasses Item/Book/Movie
        │   ├── store.py         ← persistencia JSON
        │   ├── service.py       ← reglas (add/list/search/borrow/return)
        │   └── cli.py           ← CLI con argparse
        ├── tests/
        │   ├── test_models.py
        │   ├── test_store.py
        │   └── test_cli.py
        ├── data/library.json    ← almacén de ejemplo
        └── schema/library.schema.json
```

---

# 9. Formato de entrega

Igual que el resto de Academy: **web estática** (HTML + CSS + JavaScript vanilla, **sin Markdown en las lecciones y sin build**).

- `web/` es lo navegable; cada lección es un **archivo HTML completo**.
- El chrome (cabecera, sidebar, progreso, tema claro/oscuro) lo inyecta `web/assets/js/site.js` a partir del mapa en `web/assets/js/course-data.js` (**única fuente de verdad** de navegación).
- El proyecto `proyectos/biblioteca/` es **ejecutable de verdad** (`pip`/`uv`, Python 3.12+).

## Anatomía de una lección

```html
<!DOCTYPE html>
<html lang="es" data-theme="dark" data-module="05" data-lesson="04-dataclasses">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Dataclasses — Módulo 5 · Curso Python</title>
  <script>document.documentElement.dataset.theme = localStorage.getItem('py-theme') || 'dark';</script>
  <link rel="stylesheet" href="../../assets/css/site.css">
  <link rel="icon" href="../../assets/favicon.svg" type="image/svg+xml">
</head>
<body>
  <main>
    <article class="lesson">
      <header class="lesson-header">
        <h1>Dataclasses</h1>
        <div class="lesson-header__meta">
          <span class="badge">Módulo 5 · Programación orientada a objetos</span>
          <span>⏱ 30 minutos</span>
        </div>
      </header>
      <section id="introduccion"><h2><span class="section-number">1 ·</span> Introducción</h2>…</section>
      <!-- 10 secciones con ids fijos: introduccion, teoria, ejemplo-minimo,
           ejemplo-real, buenas-practicas, errores, ejercicio, reto, resumen, faq -->
    </article>
  </main>
  <script>window.SITE_BASE = '../../';</script>
  <script src="../../assets/js/course-data.js"></script>
  <script src="../../assets/js/highlight.js"></script>
  <script src="../../assets/js/site.js"></script>
</body>
</html>
```

**SITE_BASE:** `'./'` en la portada · `'../'` en `web/recursos/` · `'../../'` en lecciones.

**data-lesson** = nombre de archivo **sin** `.html`.

Clases disponibles: `callout callout--why/--tip/--note/--warning/--analogy`, `challenge`, `faq details`, `table`, `ol.steps`, `pre > code.language-*`, `dl.glossary`.

Lenguajes de resaltado habituales: `language-python`, `language-bash`, `language-toml`, `language-json`, `language-text`.

---

# 10. Diseño y marca

- Tema oscuro por defecto, toggle a claro.
- **Identidad Python** (no copiar la paleta de otro curso):
  - Acento principal: **azul Python `#3776AB`**.
  - Acento secundario: **amarillo Python `#FFD43B`** (para destacados y el elemento firma).
  - Fondo oscuro: `#0e1116` (familia Academy).
- **Elemento firma de la portada**: una **sesión de REPL** (`>>>`) que se escribe sola: el curso se presenta con código Python y su salida.
  ```
  >>> import curso
  >>> curso.temario()
  ✓ 10 módulos · 45 lecciones · Python 3.12+ · con proyecto real
  ```
  (equivalente a la consola JS, la terminal Docker o el documento spec de SDD).
- Tipografía: sans del sistema + **JetBrains Mono** (código y chrome), como el resto de Academy.
- localStorage: **`py-theme`**, **`py-progress`**.
- UI y comentarios en **español**; identificadores y comandos en **inglés**.

---

# 11. Convenciones del curso

1. Idioma: contenido, comentarios y UI en **español**; identificadores y comandos en **inglés**.
2. Python **3.12+**, estilo **PEP 8**, código *pythonico*.
3. Proyecto de muestra: **Biblioteca**; CLI `bib`; almacén `data/library.json`.
4. Cada lección usa SIEMPRE las 10 secciones de "Forma de enseñar".
5. Los ejemplos muestran la **salida esperada**; el código va comentado en español.
6. Los retos quedan **sin resolver** en el HTML (solo pista).
7. En el Resumen, tabla "Concepto | Para qué sirve" o "Método | Qué hace".
8. Rutas canónicas en Linux con callouts macOS/Windows (launcher `py`, activación de venv).
9. No convertir el curso en un tutorial de un framework; la stdlib primero.

---

# 12. Documentación actualizada

Antes de afirmar versiones, flags o APIs, consultar la documentación oficial (docs.python.org, packaging.python.org, docs.astral.sh/uv, docs.pytest.org) vía **Context7** o web. Preferir la versión estable más reciente:
- Python **3.12 / 3.13**.
- pytest 8.x, mypy 1.x, ruff 0.6+.
- uv 0.4+.

---

# 13. Página principal (web/index.html)

Adaptar la plantilla de JavaScript/Docker:

- **Hero + REPL** (`>>>`) con código Python y su salida.
- Lead: curso en español de Python moderno para quien ya programa.
- "Cómo funciona este curso" (10 secciones, proyecto Biblioteca real, sin frameworks).
- Estadísticas calculadas por `site.js` (módulos, lecciones, horas).
- Índice de módulos (`#module-list`) y botón "Continuar donde lo dejaste".

---

# 14. Checklist de implementación (para el agente que construya el curso)

## Fase 0 — Spec y skills ✅
- [x] `specs.md` (este documento).
- [x] Skills: `documentation`, `frontend-design`, `academy-course`, `python-course` (centralizadas en `Academy/.agents/skills/`).
- [x] `AGENTS.md` (puntero). Context7 MCP: `opencode.json` en la raíz de la Academy.

## Fase 1 — Scaffold ✅
- [x] `package.json`, `pnpm-workspace.yaml`, `.gitignore` (con entradas Python), `.gitattributes` (LF).
- [x] `scripts/serve-web.mjs` (mensaje "Curso Python").
- [x] Copiar `web/assets/fonts/` y `highlight.js` desde Aprende Docker, y **añadir soporte de Python al highlighter**.
- [x] `web/assets/js/course-data.js` con el mapa de la sección 7.
- [x] `web/assets/js/site.js` con claves `py-*`, logo "PY", firma REPL, colores Python.
- [x] `web/assets/css/site.css` con la identidad de la sección 10.
- [x] `web/index.html` + `favicon.svg`.

## Fase 2 — 45 lecciones HTML ✅
- [x] Módulos 00–09 según el índice, 10 secciones cada una, Biblioteca como hilo.
- [x] Cada lección: código comentado y ejecutable, salida esperada, diagramas ASCII, callouts multi-OS.
- [x] Verificación estructural 45/45 + balance HTML 0 problemas + smoke HTTP 54/54.

## Fase 3 — Recursos ✅
- [x] `cheatsheet.html`, `glosario.html`, `faq.html`.

## Fase 4 — Proyecto Biblioteca (ejecutable) ✅
- [x] `pyproject.toml` con paquete `biblioteca` y entry point `bib`.
- [x] `src/biblioteca/` (models, store, service, cli) funcional y comentado.
- [x] `tests/` con pytest en verde (**18/18**), `mypy --strict` y `ruff` limpios.
- [x] `schema/library.schema.json` y `data/library.json` de ejemplo.
- [x] `README.md` con instalación (venv/pip y uv), uso y verificación.

## Fase 5 — Integración con la Academy ✅
- [x] Tarjeta `Aprende Python` en `Academy/index.html`.
- [x] Footer N cursos (10 → 11).

---

# 15. Reglas importantes

1. Nunca des por sentado un concepto de Python.
2. Explica siempre el "por qué" (indentación, objetos, mutabilidad, GIL…).
3. Todo el código debe ejecutarse con Python 3.12+; muestra la salida.
4. Enseña primero la biblioteca estándar; terceros solo cuando aportan.
5. Versiona ejemplos reales (Python 3.12+, pytest 8, mypy 1, ruff 0.6+, uv 0.4+).
6. No omitas pasos de configuración (crear y activar el venv, instalar deps, ejecutar tests).
7. Mantén una progresión lógica: fundamentos → funciones → colecciones → entornos → POO → datos → testing → moderno → proyecto.
8. Si un concepto depende de otro, enlaza a la lección correspondiente.
9. Prioriza la comprensión profunda frente a la velocidad.
10. Enseña `venv`+`pip` y `uv` **ambos**, con criterio de cuándo usar cada uno.