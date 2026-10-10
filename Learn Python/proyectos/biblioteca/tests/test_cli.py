"""Tests de la CLI (`cli.py`): la interfaz, no la lógica."""

from biblioteca import cli


def test_add_e_list(tmp_path, capsys):
    store = str(tmp_path / "library.json")

    code = cli.main(
        ["--store", store, "add", "1984", "--author", "Orwell", "--year", "1949"]
    )
    assert code == 0
    assert "añadido" in capsys.readouterr().out

    code = cli.main(["--store", store, "list"])
    out = capsys.readouterr().out
    assert code == 0
    assert "1984" in out


def test_add_sin_titulo_falla(tmp_path, capsys):
    code = cli.main(["--store", str(tmp_path / "l.json"), "add", "   "])

    assert code == 1
    assert "error" in capsys.readouterr().err


def test_borrow_de_id_inexistente_falla(tmp_path, capsys):
    code = cli.main(["--store", str(tmp_path / "l.json"), "borrow", "it_nope"])

    assert code == 1
    assert "error" in capsys.readouterr().err
