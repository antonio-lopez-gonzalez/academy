"""RAG: recuperación de contexto + generación con citas."""

from __future__ import annotations

from dataclasses import dataclass

from .index import Chunk
from .llm import LLMClient


@dataclass
class Answer:
    """Respuesta del Copiloto con las fuentes citadas."""

    text: str
    sources: list[str]


def build_prompt(question: str, context: list[Chunk]) -> str:
    """Construye el prompt con el contexto numerado (para poder citar)."""
    blocks = "\n".join(
        f"[{i + 1}] ({c.course} · {c.module} · {c.lesson})\n{c.text}"
        for i, c in enumerate(context)
    )
    return (
        "Eres el Copiloto de la Academy. Responde SOLO con el contexto; si no "
        "está, di que no lo sabes. Cita las fuentes con [n].\n\n"
        f"### Contexto\n{blocks}\n\n### Pregunta\n{question}\n\n### Respuesta\n"
    )


def answer(question: str, context: list[Chunk], llm: LLMClient) -> Answer:
    """Genera una respuesta fundamentada en `context` y devuelve las fuentes."""
    if not context:
        return Answer("No lo sé: no hay contexto en el corpus.", [])
    text = llm.generate(build_prompt(question, context))
    sources = [f"{c.course}/{c.module}/{c.lesson}" for c in context]
    return Answer(text, sources)
