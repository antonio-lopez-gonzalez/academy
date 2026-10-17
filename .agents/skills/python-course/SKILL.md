---
name: python-course
description: Use when writing Aprende Python lesson content or the Biblioteca sample project. Defines the Python teaching stance (why indentation, everything is an object, dynamic typing, mutability, LEGB, generators, GIL/async), the per-module focus, the Biblioteca contract, and the anti-patterns to teach. Companion to Aprende Python/specs.md.
---

# Python course (domain skill)

You are writing a course that teaches **idiomatic Python 3.12+** to developers who already program in another language. The method is in `Aprende Python/specs.md`; this skill is the domain companion. If they disagree, the spec wins — flag the conflict, don't guess.

## 1. Teaching stance (non-negotiable)

- **Explain the why, always.** Indentation isn't arbitrary (it defines blocks, no braces); `is` isn't `==` (identity vs equality); functions are objects (so decorators and closures exist); names are bindings to objects (so rebinding is not mutation).
- **Compare to other languages** when it clarifies: Python's dynamic typing vs JS `undefined`, lists vs JS arrays/C# `List<T>`, dicts vs JS objects/Map, `for/else`, comprehensions, the GIL vs single-threaded JS.
- **Start with the standard library**, introduce third-party only when it earns its place (pytest, ruff, mypy, uv).
- **Show the expected output** of every example.
- Comments and prose in Spanish; identifiers/commands in English.
- "Pythonic" means: use comprehensions when they clarify, iterate directly over items (not `range(len(x))`), use `with` for resources, use `enumerate`/`zip`, unpack, use f-strings, prefer EAFP (`try/except`) over LBYL when appropriate.

## 2. Python 3.12+ only

- f-strings (and nested), `match/case` (3.10+), `tomllib` (3.11+), type parameter syntax `list[str]`, `X | None`.
- No Python 2 idioms (`print` as statement, `unicode`, `dict.has_key`).
- Modern packaging with `pyproject.toml`; `setup.py` only as historical note.
- Mention 3.12/3.13 features accurately; check docs via the `documentation` skill before asserting specifics.

## 3. Per-module focus (write to this)

- **M0 Preparación:** interpreter vs compiler, CPython/bytecode, REPL vs script vs `-m`, `__pycache__`, `import this`. Get the environment right the first time.
- **M1 Fundamentos:** names as bindings, dynamic types, mutability, f-strings, list vs tuple, dict/set and hashability. The `is` vs `==` lesson lives here conceptually.
- **M2 Control y funciones:** truthiness, `for/else`, functions as first-class objects, default-argument evaluation (**never a mutable default**), LEGB, closures.
- **M3 Colecciones avanzadas:** comprehensions (and their limits), iterator protocol (`__iter__`/`__next__`), generators and laziness, `sorted(key=…)`, shallow vs deep copy and aliasing.
- **M4 Módulos y entornos:** packages, `__init__.py`, `pyproject.toml`, `src/` layout, `venv`+`pip` **and** `uv`, `argparse` and `__main__`.
- **M5 POO:** classes, `self`, `__init__`, instance vs class attributes, properties, inheritance vs composition, MRO, dunder methods, `@dataclass`.
- **M6 Errores y datos:** exception hierarchy, custom exceptions, EAFP, `with` and context managers, `pathlib` over `os.path`, `json` and encoding.
- **M7 Testing y calidad:** pytest idioms, fixtures, `tmp_path`, `parametrize`, `monkeypatch`, typing and mypy, ruff/black, pre-commit.
- **M8 Moderno y async:** decorators (with `functools.wraps`), `yield from`/`itertools`, structural pattern matching, `datetime`/`zoneinfo`, `logging`, then the GIL and `threading`/`multiprocessing`/`asyncio`.
- **M9 Proyecto final:** architecture, package, tests, build with `uv build`.

## 4. Biblioteca (the running project) — stable contract

Keep these names/behaviour identical across lessons:

- Package `biblioteca`, CLI command **`bib`** via `[project.scripts]`.
- Module layout: `src/biblioteca/{__init__,models,store,service,cli}.py`.
- Item fields (grow deliberately): `id: str`, `title: str`, `kind: str` (`"book"` | `"movie"`), `year: int | None`, `borrowed: bool`, plus `author`/`director` as the model matures. Do **not** leak later fields into early lessons.
- Commands: `bib add "1984" --kind book --year 1949`, `bib list`, `bib list --kind movie`, `bib search "orwell"`, `bib borrow <id>`, `bib return <id>`.
- Store: `data/library.json`, an array of items, validated against `schema/library.schema.json`.
- Errors: a `BibliotecaError` (or `StoreError`) hierarchy, not bare `Exception`.

Use the same canonical examples across the course (`1984`, `Dune`, `Blade Runner`) so the student recognizes the system.

## 5. Anti-patterns to teach explicitly

- **Mutable default argument** (`def f(x=[])`) → use `None` + create inside. Classic interview bug; show it and fix it.
- **`==` vs `is`**: `is` for `None`/sentinel identity; `==` for value. `is` on ints/strings is an implementation detail.
- **Shadows de builtins** (`list = [...]`, `id = 3`).
- **Bare `except:`** swallowing everything; catch specific exceptions.
- **`range(len(x))`** instead of iterating/`enumerate`.
- **String concatenation in a loop** instead of `"".join(...)`.
- **Installing globally / no venv**; `pip install` outside a virtualenv.
- **Modifying a list while iterating it**.
- **Using `dict.get` wrong / assuming key exists**; use `get`/`defaultdict`/`setdefault`.
- **Shadowing: type hints that lie** — hints are not runtime enforcement; that's what mypy is for.
- **Calling `asyncio.run` inside a running loop** and other async pitfalls; the GIL misconception ("threads don't help Python at all").

## 6. Verification of examples

- Every code sample must run on **Python 3.12+** with only the standard library, unless the lesson explicitly introduces a dependency (pytest/uv/mypy/ruff).
- Show how to run it (`python file.py`, `python -m pytest`, `uv run bib list`).
- When using a virtualenv, show the cross-platform activation (bash vs PowerShell).
- Prefer real, small examples over pseudocode; the Biblioteca project ties them together.
