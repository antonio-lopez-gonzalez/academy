"""RAG: recuperación de contexto + generación con citas."""

from __future__ import annotations

from dataclasses import dataclass

from .embeddings import Embedder
from .llm import LLMClient
from .rank import keyword_scores
from .store import Chunk, Store, dot

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
    """Recupera los `k` fragmentos más relevantes (híbrido: keywords + embeddings)."""
    rows = list(store.iter_rows(course))
    if not rows:
        return []
    query_vec = embedder.embed([question])[0]
    keyword = keyword_scores(question, [c.text for c in rows])
    cosine = [dot(query_vec, c.embedding or []) for c in rows]
    # 0.8 keywords + 0.2 embeddings: señal absoluta → permite abstenerse.
    # (Con el fallback offline el coseno es poco discriminante; con embeddings
    # reales de Ollama se puede subir el peso del vector.)
    combined = [0.8 * kw + 0.2 * cs for kw, cs in zip(keyword, cosine, strict=True)]
    scored = sorted(zip(combined, rows, strict=True), key=lambda p: p[0], reverse=True)
    return scored[:k]


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
