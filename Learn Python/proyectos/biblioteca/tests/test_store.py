"""Tests de la persistencia (`store.py`) usando `tmp_path`."""

import pytest

from biblioteca import store
from biblioteca.models import BibliotecaError, Book


def test_load_inexistente_devuelve_vacio(tmp_path):
    assert store.load(tmp_path / "no-existe.json") == []


def test_save_y_load_roundtrip(tmp_path):
    path = tmp_path / "library.json"
    items = [Book(id="it_1", title="1984", kind="book", year=1949, author="Orwell")]

    store.save(items, path)

    assert path.exists()
    assert store.load(path) == items


def test_almacen_corrupto_lanza_error(tmp_path):
    path = tmp_path / "roto.json"
    path.write_text("{esto no es json", encoding="utf-8")

    with pytest.raises(BibliotecaError):
        store.load(path)


def test_almacen_que_no_es_lista_lanza_error(tmp_path):
    path = tmp_path / "objeto.json"
    path.write_text('{"no": "es una lista"}', encoding="utf-8")

    with pytest.raises(BibliotecaError):
        store.load(path)
