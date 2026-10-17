"""Cliente del modelo. Local-first: Ollama; fallback `EchoLLM` para offline."""

from __future__ import annotations

import json
import urllib.request
from typing import Protocol

from .embeddings import ollama_available


class LLMClient(Protocol):
    """Contrato común a todos los proveedores de modelo."""

    name: str

    def generate(self, prompt: str, system: str | None = None) -> str:
        """Devuelve la respuesta del modelo."""
        ...


class OllamaLLM:
    """Modelo local vía Ollama (`/api/chat`)."""

    def __init__(self, host: str, model: str) -> None:
        self.host = host
        self.model = model
        self.name = f"ollama:{model}"

    def generate(self, prompt: str, system: str | None = None) -> str:
        messages: list[dict[str, str]] = []
        if system:
            messages.append({"role": "system", "content": system})
        messages.append({"role": "user", "content": prompt})
        payload = json.dumps(
            {"model": self.model, "messages": messages, "stream": False}
        ).encode("utf-8")
        req = urllib.request.Request(  # noqa: S310
            f"{self.host}/api/chat",
            data=payload,
            headers={"Content-Type": "application/json"},
        )
        with urllib.request.urlopen(req, timeout=120) as resp:  # noqa: S310
            data = json.loads(resp.read())
        return str(data.get("message", {}).get("content", ""))


class EchoLLM:
    """Proveedor de juguete: repite el prompt. Permite probar el pipeline sin red."""

    name = "echo"

    def generate(self, prompt: str, system: str | None = None) -> str:
        prefix = "" if system is None else "(con system) "
        return f"[eco] {prefix}{prompt.strip()[:400]}"


def get_llm(settings) -> LLMClient:
    """Ollama si está vivo; si no, el eco (offline)."""
    if ollama_available(settings.ollama_host):
        return OllamaLLM(settings.ollama_host, settings.model)
    return EchoLLM()
