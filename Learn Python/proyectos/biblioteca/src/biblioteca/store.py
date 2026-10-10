"""Persistencia del catálogo en un fichero JSON.

El almacén es una lista de ítems. Si el fichero no existe se trata como una
biblioteca vacía; si está corrupto, se lanza `BibliotecaError` en vez de
sobrescribirlo silenciosamente (perder datos es peor que fallar).
"""

from __future__ import annotations

import json
from collections.abc import Sequence
from pathlib import Path

from .models import BibliotecaError, Item, item_from_dict

# Ruta por defecto, relativa al directorio de trabajo.
DEFAULT_STORE = Path("data/library.json")


def load(path: str | Path = DEFAULT_STORE) -> list[Item]:
    """Lee el almacén y devuelve la lista de ítems (vacía si no existe)."""
    file = Path(path)
    if not file.exists():
        return []
    try:
        raw = json.loads(file.read_text(encoding="utf-8"))
    except json.JSONDecodeError as err:
        raise BibliotecaError(f"el almacén {file} no es JSON válido: {err}") from err
    if not isinstance(raw, list):
        raise BibliotecaError(f"el almacén {file} debe ser una lista JSON")
    return [item_from_dict(entry) for entry in raw]


def save(items: Sequence[Item], path: str | Path = DEFAULT_STORE) -> None:
    """Escribe la lista de ítems, creando el directorio si hace falta."""
    file = Path(path)
    file.parent.mkdir(parents=True, exist_ok=True)
    data = [item.to_dict() for item in items]
    text = json.dumps(data, ensure_ascii=False, indent=2) + "\n"
    file.write_text(text, encoding="utf-8")
