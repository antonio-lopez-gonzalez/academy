"""Ingesta: convierte las lecciones HTML de la Academy en chunks con metadatos."""

from __future__ import annotations

from dataclasses import dataclass
from html.parser import HTMLParser
from pathlib import Path


@dataclass
class Chunk:
    """Fragmento citable: texto + de qué curso/módulo/lección sale."""

    chunk_id: str
    course: str
    module: str
    lesson: str
    section: str
    text: str


class _TextExtractor(HTMLParser):
    """Extrae texto plano de un HTML (implementación completa en el módulo 3)."""

    def __init__(self) -> None:
        super().__init__()
        self.parts: list[str] = []

    def handle_data(self, data: str) -> None:
        self.parts.append(data)

    def text(self) -> str:
        return " ".join(" ".join(self.parts).split())


def lessons_root(academy_root: Path) -> Path:
    """Raíz de los cursos dentro de la Academy."""
    return academy_root / "Learn IA"


def iter_lesson_files(academy_root: Path) -> list[Path]:
    """Lista los HTML de lecciones de todos los cursos de la Academy."""
    return sorted(academy_root.glob("Learn */web/modulos/**/*.html"))


def chunk_lesson(path: Path, *, size: int = 900, overlap: int = 150) -> list[Chunk]:
    """Trocea una lección en chunks. Version simple (el módulo 3 la mejora)."""
    parser = _TextExtractor()
    parser.feed(path.read_text(encoding="utf-8"))
    text = parser.text()
    chunks: list[Chunk] = []
    step = max(1, size - overlap)
    for i in range(0, len(text), step):
        piece = text[i : i + size].strip()
        if not piece:
            continue
        chunks.append(
            Chunk(
                chunk_id=f"{path.stem}-{i // step:03d}",
                course=path.parts[-5],
                module=path.parts[-3],
                lesson=path.stem,
                section="",
                text=piece,
            )
        )
    return chunks
