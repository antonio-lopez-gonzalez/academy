"""Almacén vectorial en SQLite (solo biblioteca estándar).

Guarda los chunks con su embedding como BLOB (floats empaquetados) y permite
buscar por similitud coseno. Los vectores ya se guardan normalizados, así que
el coseno es un producto punto.
"""

from __future__ import annotations

import sqlite3
import struct
from dataclasses import dataclass
from pathlib import Path


@dataclass
class Chunk:
    """Fragmento citable de una lección."""

    chunk_id: str
    course: str
    module: str
    lesson: str
    section: str
    text: str
    embedding: list[float] | None = None

    @property
    def source(self) -> str:
        """Referencia legible para citar: `Curso · módulo · lección`. """
        return f"{self.course} · {self.module} · {self.lesson}"


def _pack(vec: list[float]) -> bytes:
    return struct.pack(f"<{len(vec)}f", *vec)


def _unpack(blob: bytes) -> list[float]:
    n = len(blob) // 4
    return list(struct.unpack(f"<{n}f", blob))


def _dot(a: list[float], b: list[float]) -> float:
    return sum(x * y for x, y in zip(a, b, strict=False))


class Store:
    """Índice de chunks respaldado por SQLite."""

    def __init__(self, db_path: str | Path) -> None:
        path = Path(db_path)
        path.parent.mkdir(parents=True, exist_ok=True)
        self.conn = sqlite3.connect(str(path))
        self.conn.execute(
            """
            CREATE TABLE IF NOT EXISTS chunks (
                chunk_id TEXT PRIMARY KEY,
                course   TEXT NOT NULL,
                module   TEXT NOT NULL,
                lesson   TEXT NOT NULL,
                section  TEXT NOT NULL,
                text     TEXT NOT NULL,
                embedding BLOB NOT NULL
            )
            """
        )
        self.conn.commit()

    def clear(self) -> None:
        """Vacía el índice."""
        self.conn.execute("DELETE FROM chunks")
        self.conn.commit()

    def add(self, chunks: list[Chunk]) -> None:
        """Inserta o reemplaza chunks (idempotente por `chunk_id`)."""
        rows = [
            (c.chunk_id, c.course, c.module, c.lesson, c.section, c.text, _pack(c.embedding or []))
            for c in chunks
        ]
        self.conn.executemany(
            "INSERT OR REPLACE INTO chunks VALUES (?, ?, ?, ?, ?, ?, ?)", rows
        )
        self.conn.commit()

    def count(self) -> int:
        """Número de chunks indexados."""
        return int(self.conn.execute("SELECT COUNT(*) FROM chunks").fetchone()[0])

    def search(
        self, query_vec: list[float], k: int = 5, course: str | None = None
    ) -> list[tuple[float, Chunk]]:
        """Devuelve los `k` chunks más similares (coseno), opcionalmente de un curso."""
        sql = "SELECT chunk_id, course, module, lesson, section, text, embedding FROM chunks"
        params: tuple = ()
        if course:
            sql += " WHERE course = ?"
            params = (course,)
        scored: list[tuple[float, Chunk]] = []
        for cid, co, mod, les, sec, text, blob in self.conn.execute(sql, params):
            chunk = Chunk(cid, co, mod, les, sec, text)
            scored.append((_dot(query_vec, _unpack(blob)), chunk))
        scored.sort(key=lambda pair: pair[0], reverse=True)
        return scored[:k]

    def close(self) -> None:
        """Cierra la conexión."""
        self.conn.close()
