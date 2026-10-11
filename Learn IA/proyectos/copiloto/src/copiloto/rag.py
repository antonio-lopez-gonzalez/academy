"""RAG: recuperación de contexto + generación con citas."""

from __future__ import annotations

from dataclasses import dataclass

from .embeddings import Embedder
from .llm import LLMClient
from .store import Chunk, Store

SYSTEM = (
    "Eres el Copiloto de la Academy. Responde SOLO con la información del "
    "contexto; si la respuesta no está ahí, di que no lo sabes. Cita las "
    "fuentes usadas con su número entre corchetes, por ejemplo [1]."
)


@dataclass
class Answer:
    """Respuesta del Copiloto con las fuentes citadas."""

    text: str
    sources: list[str]


def retrieve(
    question: str,
    store: Store,
    embedder: Embedder,
    k: int = 5,
    course: str | None = None,
) -> list[tuple[float, Chunk]]:
    """Recupera los `k` fragmentos más similares a la pregunta."""
    vec = embedder.embed([question])[0]
    return store.search(vec, k=k, course=course)


def build_prompt(question: str, context: list[Chunk]) -> str:
    """Prompt con el contexto numerado (para poder citar)."""
    blocks = "\n\n".join(f"[{i + 1}] ({c.source})\n{c.text}" for i, c in enumerate(context))
    return f"### Contexto\n{blocks}\n\n### Pregunta\n{question}\n\n### Respuesta"


def answer(
    question: str,
    context: list[Chunk],
    llm: LLMClient,
    *,
    min_score: float = 0.0,
) -> Answer:
    """Genera una respuesta fundamentada en `context` y devuelve las fuentes.

    Si no hay contexto (o ninguno supera `min_score`), se abstiene en lugar de inventar.
    """
    good = [c for c in context] if min_score <= 0.0 else []
    if not good:
        return Answer("No lo sé: no encuentro nada relevante en el corpus.", [])
    text = llm.generate(build_prompt(question, good), system=SYSTEM)
    return Answer(text, [c.source for c in good])
