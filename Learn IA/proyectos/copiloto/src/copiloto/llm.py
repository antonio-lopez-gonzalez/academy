"""Cliente del modelo (LLM).

Local-first: el proveedor por defecto es **Ollama**. Los adaptadores de nube
(OpenAI/Anthropic) se añaden en el módulo 2 del curso, detrás de la misma
interfaz `LLMClient`.
"""

from __future__ import annotations

from typing import Protocol


class LLMClient(Protocol):
    """Contrato común a todos los proveedores de modelo."""

    def generate(self, prompt: str) -> str:
        """Devuelve la respuesta del modelo para `prompt`."""
        ...


class EchoLLM:
    """Proveedor de juguete: repite el prompt. Sirve para probar el pipeline sin red."""

    def generate(self, prompt: str) -> str:
        return f"[eco] {prompt.strip()[:280]}"


def default_client() -> LLMClient:
    """Devuelve el cliente por defecto (Ollama en el curso real)."""
    return EchoLLM()
