"""Biblioteca — catálogo personal de libros y películas.

Proyecto del curso de Python: paquete instalable con CLI `bib`.

Uso rápido (tras instalar)::

    bib add "1984" --kind book --year 1949 --author "George Orwell"
    bib list
    bib search orwell
    bib borrow <id>
"""

from importlib.metadata import PackageNotFoundError, version

from .models import BibliotecaError, Book, Item, Movie

try:
    __version__ = version("biblioteca")
except PackageNotFoundError:  # paquete no instalado (se ejecuta desde el repo)
    __version__ = "1.0.0"

__all__ = ["BibliotecaError", "Book", "Item", "Movie", "__version__"]
