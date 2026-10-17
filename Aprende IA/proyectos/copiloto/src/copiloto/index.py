"""Ingesta: HTML de las lecciones → secciones → chunks → embeddings → SQLite."""

from __future__ import annotations

from html.parser import HTMLParser
from pathlib import Path

from .config import Settings
from .embeddings import Embedder, get_embedder
from .store import Chunk, Store

_SKIP_TAGS = {"script", "style"}


class _SectionParser(HTMLParser):
    """Extrae el texto de cada `<section id="...">` de una lección."""

    def __init__(self) -> None:
        super().__init__()
        self.sections: list[tuple[str, str]] = []
        self._current_id = ""
        self._buf: list[str] = []
        self._skip = 0

    def handle_starttag(self, tag: str, attrs: list) -> None:
        if tag in _SKIP_TAGS:
            self._skip += 1
        if tag == "section":
            self._flush()
            self._current_id = dict(attrs).get("id", "")

    def handle_endtag(self, tag: str) -> None:
        if tag in _SKIP_TAGS and self._skip:
            self._skip -= 1

    def handle_data(self, data: str) -> None:
        if not self._skip:
            self._buf.append(data)

    def _flush(self) -> None:
        text = " ".join(" ".join(self._buf).split())
        if self._current_id and text:
            self.sections.append((self._current_id, text))
        self._current_id = ""
        self._buf = []

    def close(self) -> None:  # type: ignore[override]
        super().close()
        self._flush()


def extract_sections(html: str) -> list[tuple[str, str]]:
    """Devuelve `[(section_id, texto)]` de una lección."""
    parser = _SectionParser()
    parser.feed(html)
    parser.close()
    return parser.sections


def chunk_text(text: str, size: int = 900, overlap: int = 150) -> list[str]:
    """Trocea un texto en ventanas solapadas (por caracteres)."""
    if not text:
        return []
    step = max(1, size - overlap)
    out: list[str] = []
    for i in range(0, len(text), step):
        piece = text[i : i + size].strip()
        if piece:
            out.append(piece)
    return out


def iter_lesson_files(academy_root: Path, course: str | None = None) -> list[Path]:
    """Lista los HTML de lecciones de la Academy (o de un curso concreto)."""
    pattern = f"{course}/web/modulos/**/*.html" if course else "Aprende */web/modulos/**/*.html"
    return sorted(academy_root.glob(pattern))


def _meta(path: Path, academy_root: Path) -> tuple[str, str, str]:
    """Devuelve `(curso, módulo, lección)` desde la ruta del fichero."""
    rel = path.relative_to(academy_root).parts
    course = rel[0]
    module = rel[3] if len(rel) > 3 else ""
    return course, module, path.stem


def build_index(
    academy_root: Path,
    settings: Settings,
    embedder: Embedder | None = None,
    course: str | None = None,
    store: Store | None = None,
) -> Store:
    """Indexa las lecciones y devuelve el almacén listo para buscar."""
    embedder = embedder or get_embedder(settings)
    store = store or Store(settings.db_path)
    if course is None:
        store.clear()

    meta: list[tuple[str, str, str, str, str, str]] = []
    for path in iter_lesson_files(academy_root, course):
        html = path.read_text(encoding="utf-8")
        co, mod, les = _meta(path, academy_root)
        for section_id, text in extract_sections(html):
            for i, piece in enumerate(chunk_text(text)):
                chunk_id = f"{co}/{mod}/{les}#{section_id}-{i}"
                meta.append((chunk_id, co, mod, les, section_id, piece))

    if meta:
        vectors = embedder.embed([m[5] for m in meta])
        store.add([Chunk(*m, embedding=v) for m, v in zip(meta, vectors, strict=True)])
    return store
