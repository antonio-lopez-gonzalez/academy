"""Configuración del Copiloto (variables de entorno, sin claves en el repo)."""

from __future__ import annotations

import os
from dataclasses import dataclass
from pathlib import Path


@dataclass(frozen=True)
class Settings:
    """Configuración efectiva del Copiloto."""

    model: str = os.environ.get("COPILOTO_MODEL", "llama3.2")
    embed_model: str = os.environ.get("COPILOTO_EMBED", "nomic-embed-text")
    ollama_host: str = os.environ.get("OLLAMA_HOST", "http://localhost:11434")
    data_dir: str = os.environ.get("COPILOTO_DATA", "data")
    top_k: int = int(os.environ.get("COPILOTO_TOP_K", "5"))
    min_score: float = float(os.environ.get("COPILOTO_MIN_SCORE", "0.25"))

    @property
    def db_path(self) -> Path:
        """Ruta del índice vectorial (SQLite)."""
        return Path(self.data_dir) / "index.db"


def load_settings() -> Settings:
    """Devuelve la configuración efectiva."""
    return Settings()
