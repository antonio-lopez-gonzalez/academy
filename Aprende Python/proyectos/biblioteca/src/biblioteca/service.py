"""Reglas de negocio de Biblioteca.

Estas funciones cargan el almacén, aplican la operación y lo guardan. La CLI
es una capa fina por encima; así el núcleo se puede probar sin terminal.
"""

from __future__ import annotations

import secrets
from collections.abc import Iterable
from pathlib import Path

from . import store
from .models import BibliotecaError, Book, Item, Movie


def _new_id(existing: set[str]) -> str:
    """Genera un id corto y único que no colisione con los existentes."""
    while True:
        candidate = "it_" + secrets.token_hex(4)  # p. ej. it_9f3a1c7b
        if candidate not in existing:
            return candidate


def add(
    title: str,
    kind: str = "book",
    year: int | None = None,
    creator: str | None = None,
    path: str | Path = store.DEFAULT_STORE,
) -> Item:
    """Añade un ítem nuevo al catálogo y devuelve el ítem creado."""
    if not title or not title.strip():
        raise BibliotecaError("el título es obligatorio")
    items = store.load(path)
    item_id = _new_id({item.id for item in items})

    if kind == "book":
        item: Item = Book(
            id=item_id, title=title.strip(), kind="book", year=year, author=creator
        )
    elif kind == "movie":
        item = Movie(
            id=item_id, title=title.strip(), kind="movie", year=year, director=creator
        )
    else:
        raise BibliotecaError(f"kind inválido: {kind!r} (usa 'book' o 'movie')")

    items.append(item)
    store.save(items, path)
    return item


def find(items: Iterable[Item], item_id: str) -> Item:
    """Devuelve el ítem con ese id o lanza BibliotecaError si no existe."""
    for item in items:
        if item.id == item_id:
            return item
    raise BibliotecaError(f"no existe ningún ítem con id {item_id!r}")


def list_items(
    kind: str | None = None, path: str | Path = store.DEFAULT_STORE
) -> list[Item]:
    """Lista el catálogo, opcionalmente filtrando por tipo."""
    items = store.load(path)
    if kind is None:
        return items
    if kind not in ("book", "movie"):
        raise BibliotecaError(f"kind inválido: {kind!r}")
    return [item for item in items if item.kind == kind]


def search(term: str, path: str | Path = store.DEFAULT_STORE) -> list[Item]:
    """Busca por título o creador (autor/director), sin distinguir mayúsculas."""
    needle = term.strip().lower()
    if not needle:
        raise BibliotecaError("el término de búsqueda no puede estar vacío")
    results = []
    for item in store.load(path):
        haystack = f"{item.title} {item.creator or ''}".lower()
        if needle in haystack:
            results.append(item)
    return results


def borrow(item_id: str, path: str | Path = store.DEFAULT_STORE) -> Item:
    """Marca un ítem como prestado."""
    items = store.load(path)
    item = find(items, item_id)
    if item.borrowed:
        raise BibliotecaError(f"el ítem {item_id!r} ya está prestado")
    item.borrowed = True
    store.save(items, path)
    return item


def return_item(item_id: str, path: str | Path = store.DEFAULT_STORE) -> Item:
    """Marca un ítem como devuelto."""
    items = store.load(path)
    item = find(items, item_id)
    if not item.borrowed:
        raise BibliotecaError(f"el ítem {item_id!r} no estaba prestado")
    item.borrowed = False
    store.save(items, path)
    return item
