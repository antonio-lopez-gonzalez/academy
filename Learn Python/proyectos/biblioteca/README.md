# Biblioteca

Proyecto final del **Curso de Python**: un catálogo personal de libros y películas, con una CLI (`bib`), persistencia en JSON y tests.

La especificación del proyecto vive con el curso: `Learn Python/specs.md` (sección 3) y la skill `python-course` (centralizada en `Academy/.agents/skills/python-course/SKILL.md`).

## Estructura

```text
biblioteca/
├── pyproject.toml            # paquete + entry point `bib`
├── README.md
├── schema/library.schema.json
├── data/library.json         # almacén de ejemplo
├── src/biblioteca/
│   ├── __init__.py
│   ├── models.py             # dataclasses Item / Book / Movie
│   ├── store.py              # persistencia JSON
│   ├── service.py            # reglas de negocio
│   └── cli.py                # interfaz de línea de comandos
└── tests/
    ├── test_models.py
    ├── test_store.py
    ├── test_service.py
    └── test_cli.py
```

## Requisitos

- **Python 3.12+**.
- Opcional para desarrollo: `pytest`, `mypy`, `ruff`.

## Instalación

Con **venv + pip** (estándar):

```bash
python -m venv .venv
source .venv/bin/activate          # Windows (PowerShell): .venv\Scripts\Activate.ps1
pip install -e ".[dev]"            # instala el paquete en modo editable + herramientas
pip install -e .                   # solo el paquete, si no quieres las de desarrollo
```

Con **uv** (moderno y rápido):

```bash
uv venv
uv pip install -e ".[dev]"
# o directamente, sin instalar el entry point:
uv run python -m biblioteca.cli list
```

> Si no instalas el paquete, puedes ejecutar la CLI igualmente con
> `python -m biblioteca.cli ...` desde la raíz, o `PYTHONPATH=src python -m biblioteca.cli ...`.

## Uso

```bash
bib add "1984" --author "George Orwell" --year 1949
bib add "Blade Runner" --kind movie --director "Ridley Scott" --year 1982
bib list
bib list --kind movie
bib search orwell
bib borrow <id>
bib return <id>

bib --help          # ayuda general
bib add --help      # ayuda de un subcomando

# Almacén alternativo (por defecto: data/library.json)
bib --store data/demo.json list
```

## Tests y calidad

```bash
pytest                 # suite completa
pytest -v              # detallado
mypy src               # tipado estático
ruff check .           # lint
ruff format .          # formato
```

## Verificación end-to-end (definition of done)

```bash
# 1) crear un almacén limpio y añadir un ítem
bib --store data/demo.json add "Dune" --author "Frank Herbert"
bib --store data/demo.json list          # → aparece "Dune"

# 2) prestar y comprobar el estado
bib --store data/demo.json borrow <id>
bib --store data/demo.json list          # → aparece [prestado]

# 3) devolver
bib --store data/demo.json return <id>

# 4) error controlado + código de salida 1
bib --store data/demo.json borrow it_no_existe; echo $?   # → error: ... / 1
```

## Notas de diseño

- **Tú eres la fuente de verdad**: si el código y esta documentación discrepan, gana la especificación del curso; corrige y versiona.
- Los `id` se generan con `secrets.token_hex`, no con índices, para que sobrevivan a reordenaciones y no se reutilicen.
- El almacén JSON es deliberadamente simple; migrar a SQLite no cambiaría el contrato de `Item`.
- `mypy --strict` está configurado: los tipos son parte de la calidad, no un adorno.
