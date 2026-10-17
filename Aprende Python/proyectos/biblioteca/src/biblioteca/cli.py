"""Interfaz de línea de comandos de Biblioteca (`bib`).

Es una capa fina: interpreta argumentos, llama al servicio y decide el código
de salida. Toda la lógica vive en `service.py`.
"""

from __future__ import annotations

import argparse
import sys

from . import __version__, service, store
from .models import BibliotecaError, Item

DEFAULT_STORE = str(store.DEFAULT_STORE)


def build_parser() -> argparse.ArgumentParser:
    """Construye el parser con los subcomandos de `bib`."""
    parser = argparse.ArgumentParser(
        prog="bib",
        description="Biblioteca — catálogo personal de libros y películas.",
    )
    parser.add_argument("--version", action="version", version=f"bib {__version__}")
    parser.add_argument(
        "--store",
        default=DEFAULT_STORE,
        help="ruta del almacén JSON (por defecto: data/library.json)",
    )

    sub = parser.add_subparsers(dest="command", required=True)

    p_add = sub.add_parser("add", help="añadir un libro o película")
    p_add.add_argument("title", help="título del ítem")
    p_add.add_argument(
        "--kind", choices=("book", "movie"), default="book", help="tipo de ítem"
    )
    p_add.add_argument(
        "--year", type=int, default=None, help="año de publicación/estreno"
    )
    p_add.add_argument("--author", default=None, help="autor (libro)")
    p_add.add_argument("--director", default=None, help="director (película)")

    p_list = sub.add_parser("list", help="listar el catálogo")
    p_list.add_argument(
        "--kind", choices=("book", "movie"), default=None, help="filtrar por tipo"
    )

    p_search = sub.add_parser("search", help="buscar por título o creador")
    p_search.add_argument("term", help="texto a buscar")

    p_borrow = sub.add_parser("borrow", help="marcar como prestado")
    p_borrow.add_argument("id", help="id del ítem")

    p_return = sub.add_parser("return", help="marcar como devuelto")
    p_return.add_argument("id", help="id del ítem")

    return parser


def _format(item: Item) -> str:
    """Una línea legible por ítem, con estado de préstamo."""
    mark = "[prestado]" if item.borrowed else "[disponible]"
    year = f" ({item.year})" if item.year else ""
    creator = f" — {item.creator}" if item.creator else ""
    return f"{item.id}  {item.title}{year}{creator}  {mark}"


def main(argv: list[str] | None = None) -> int:
    """Punto de entrada. Devuelve el código de salida (0 = éxito)."""
    parser = build_parser()
    args = parser.parse_args(argv)

    try:
        if args.command == "add":
            creator = args.author if args.kind == "book" else args.director
            item = service.add(
                args.title,
                kind=args.kind,
                year=args.year,
                creator=creator,
                path=args.store,
            )
            print(f"añadido: {_format(item)}")

        elif args.command == "list":
            items = service.list_items(kind=args.kind, path=args.store)
            if not items:
                print("(catálogo vacío)")
            for item in items:
                print(_format(item))

        elif args.command == "search":
            for item in service.search(args.term, path=args.store):
                print(_format(item))

        elif args.command == "borrow":
            item = service.borrow(args.id, path=args.store)
            print(f"prestado: {item.id}  {item.title}")

        elif args.command == "return":
            item = service.return_item(args.id, path=args.store)
            print(f"devuelto: {item.id}  {item.title}")

    except BibliotecaError as err:
        print(f"error: {err}", file=sys.stderr)
        return 1

    return 0


if __name__ == "__main__":  # permite `python -m biblioteca.cli`
    raise SystemExit(main())
