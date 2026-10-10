"""Tests del modelo de datos (`models.py`)."""

import pytest

from biblioteca.models import BibliotecaError, Book, Movie, item_from_dict


def test_kind_invalido_lanza_error():
    with pytest.raises(BibliotecaError):
        Book(id="1", title="X", kind="revista")


def test_titulo_vacio_lanza_error():
    with pytest.raises(BibliotecaError):
        Book(id="1", title="   ", kind="book")


def test_creator_devuelve_autor_o_director():
    book = Book(id="1", title="1984", kind="book", author="George Orwell")
    movie = Movie(id="2", title="Dune", kind="movie", director="Denis Villeneuve")

    assert book.creator == "George Orwell"
    assert movie.creator == "Denis Villeneuve"


def test_to_dict_y_roundtrip():
    book = Book(id="1", title="1984", kind="book", year=1949, author="George Orwell")

    data = book.to_dict()
    assert data == {
        "id": "1",
        "title": "1984",
        "kind": "book",
        "year": 1949,
        "borrowed": False,
        "author": "George Orwell",
    }
    # Reconstruir desde el dict da un objeto igual (dataclass implementa __eq__).
    assert item_from_dict(data) == book


def test_item_from_dict_rechaza_kind_desconocido():
    with pytest.raises(BibliotecaError):
        item_from_dict({"id": "1", "title": "X", "kind": "cd", "borrowed": False})
