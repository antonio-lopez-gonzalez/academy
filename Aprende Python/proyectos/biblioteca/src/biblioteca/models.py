"""Modelo de datos de Biblioteca.

Un ítem del catálogo es siempre un `Item`; los tipos concretos son `Book`
(libro) y `Movie` (película). Se usan `dataclasses` para no escribir a mano
`__init__`, `__repr__` ni `__eq__`.
"""

from __future__ import annotations

from dataclasses import asdict, dataclass
from typing import Any

# Tipos de ítem admitidos. Un `kind` fuera de esta lista es un error de negocio.
KINDS = ("book", "movie")


class BibliotecaError(Exception):
    """Error de negocio de Biblioteca (entrada inválida, ítem inexistente…)."""


@dataclass
class Item:
    """Ítem de catálogo (base). No usar directamente: crear Book o Movie."""

    id: str
    title: str
    kind: str
    year: int | None = None
    borrowed: bool = False

    def __post_init__(self) -> None:
        # Validación en el constructor: un objeto nunca nace en estado inválido.
        if self.kind not in KINDS:
            raise BibliotecaError(f"kind inválido: {self.kind!r} (usa {KINDS})")
        if not self.title.strip():
            raise BibliotecaError("el título no puede estar vacío")

    @property
    def creator(self) -> str | None:
        """Autor o director; None si no se conoce."""
        return None

    def to_dict(self) -> dict[str, Any]:
        """Convierte el ítem a un dict serializable a JSON."""
        return asdict(self)


@dataclass
class Book(Item):
    """Libro: además tiene `author`."""

    author: str | None = None

    @property
    def creator(self) -> str | None:
        return self.author


@dataclass
class Movie(Item):
    """Película: además tiene `director`."""

    director: str | None = None

    @property
    def creator(self) -> str | None:
        return self.director


def item_from_dict(data: dict[str, Any]) -> Item:
    """Reconstruye el ítem concreto (Book/Movie) a partir de un dict JSON."""
    kind = data.get("kind")
    if kind == "movie":
        return Movie(**data)
    if kind == "book":
        return Book(**data)
    raise BibliotecaError(f"kind inválido en el almacén: {kind!r}")
