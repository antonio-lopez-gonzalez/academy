"""Configuración del Copiloto (variables de entorno)."""

from __future__ import annotations

import os
from dataclasses import dataclass


@dataclass(frozen=True)
class Settings:
    """Configuración leída del entorno. Sin claves en el repositorio."""

    model: str = os.environ.get("COPILOTO_MODEL", "llama3.2")
    ollama_host: str = os.environ.get("OLLAMA_HOST", "http://localhost:11434")
    data_dir: str = os.environ.get("COPILOTO_DATA", "data")


def load_settings() -> Settings:
    """Devuelve la configuración efectiva del Copiloto."""
    return Settings()
