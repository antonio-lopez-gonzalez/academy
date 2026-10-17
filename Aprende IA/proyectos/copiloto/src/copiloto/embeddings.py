"""Embeddings: usa Ollama si está disponible; un fallback determinista si no.

El fallback (`HashEmbedder`) no necesita red: vectoriza por tokens con hashing y
normaliza a longitud 1, de modo que el pipeline RAG funciona en cualquier máquina
aunque no haya Ollama instalado. El curso enseña a sustituirlo por embeddings
reales cuando aporta.
"""

from __future__ import annotations

import hashlib
import json
import math
import re
import urllib.request
from typing import Protocol

_TOKEN = re.compile(r"[a-záéíóúüñ0-9]+")


class Embedder(Protocol):
    """Contrato común a los generadores de embeddings."""

    name: str
    dim: int

    def embed(self, texts: list[str]) -> list[list[float]]:
        """Devuelve un vector normalizado por cada texto."""
        ...


def _l2(vec: list[float]) -> list[float]:
    """Normaliza un vector a longitud 1 (para que el coseno sea un producto punto)."""
    norm = math.sqrt(sum(x * x for x in vec))
    if norm == 0.0:
        return vec
    return [x / norm for x in vec]


def ollama_available(host: str) -> bool:
    """Comprueba si hay un servidor Ollama escuchando."""
    try:
        with urllib.request.urlopen(f"{host}/api/tags", timeout=2):  # noqa: S310
            return True
    except Exception:  # noqa: BLE001 — cualquier fallo = no disponible
        return False


class OllamaEmbedder:
    """Embeddings reales vía Ollama (`/api/embeddings`)."""

    def __init__(self, host: str, model: str) -> None:
        self.host = host
        self.model = model
        self.name = f"ollama:{model}"
        self.dim = 0  # se fija con el primer vector

    def _one(self, text: str) -> list[float]:
        payload = json.dumps({"model": self.model, "prompt": text}).encode("utf-8")
        req = urllib.request.Request(  # noqa: S310
            f"{self.host}/api/embeddings",
            data=payload,
            headers={"Content-Type": "application/json"},
        )
        with urllib.request.urlopen(req, timeout=60) as resp:  # noqa: S310
            vec = list(json.loads(resp.read())["embedding"])
        self.dim = len(vec)
        return _l2([float(x) for x in vec])

    def embed(self, texts: list[str]) -> list[list[float]]:
        return [self._one(t) for t in texts]


class HashEmbedder:
    """Fallback sin dependencias: bolsa de palabras por hashing, L2-normalizada."""

    def __init__(self, dim: int = 256) -> None:
        self.dim = dim
        self.name = "hash-fallback"

    def embed(self, texts: list[str]) -> list[list[float]]:
        out: list[list[float]] = []
        for text in texts:
            vec = [0.0] * self.dim
            for token in _TOKEN.findall(text.lower()):
                idx = int(hashlib.md5(token.encode("utf-8")).hexdigest(), 16) % self.dim
                vec[idx] += 1.0
            out.append(_l2(vec))
        return out


def get_embedder(settings) -> Embedder:
    """Ollama si está vivo; si no, el fallback offline."""
    if ollama_available(settings.ollama_host):
        return OllamaEmbedder(settings.ollama_host, settings.embed_model)
    return HashEmbedder()
