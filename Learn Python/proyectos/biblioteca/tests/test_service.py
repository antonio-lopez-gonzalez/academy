"""Tests de las reglas de negocio (`service.py`)."""

import pytest

from biblioteca import service
from biblioteca.models import BibliotecaError


def test_add_y_list(tmp_path):
    path = tmp_path / "library.json"

    item = service.add(
        "1984", kind="book", year=1949, creator="George Orwell", path=path
    )

    assert item.title == "1984"
    assert item.creator == "George Orwell"
    assert service.list_items(path=path) == [item]


def test_add_titulo_vacio_lanza_error(tmp_path):
    with pytest.raises(BibliotecaError):
        service.add("   ", path=tmp_path / "library.json")


def test_add_genera_ids_unicos(tmp_path):
    path = tmp_path / "library.json"

    a = service.add("Dune", path=path)
    b = service.add("Dune", path=path)

    assert a.id != b.id


def test_search_por_titulo_y_creador(tmp_path):
    path = tmp_path / "library.json"
    service.add("Dune", kind="book", creator="Frank Herbert", path=path)
    service.add("Blade Runner", kind="movie", creator="Ridley Scott", path=path)

    assert [i.title for i in service.search("herbert", path=path)] == ["Dune"]
    assert [i.title for i in service.search("blade", path=path)] == ["Blade Runner"]


def test_borrow_y_return(tmp_path):
    path = tmp_path / "library.json"
    item = service.add("Dune", path=path)

    service.borrow(item.id, path=path)
    assert service.list_items(path=path)[0].borrowed is True

    # No se puede prestar dos veces sin devolver.
    with pytest.raises(BibliotecaError):
        service.borrow(item.id, path=path)

    service.return_item(item.id, path=path)
    assert service.list_items(path=path)[0].borrowed is False


def test_operar_sobre_id_inexistente_lanza_error(tmp_path):
    with pytest.raises(BibliotecaError):
        service.borrow("it_no_existe", path=tmp_path / "library.json")
