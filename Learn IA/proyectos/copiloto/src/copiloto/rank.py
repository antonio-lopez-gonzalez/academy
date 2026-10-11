"""Ranking: tokenización, stopwords y cobertura de términos ponderada por IDF.

La búsqueda híbrida combina dos señales absolutas:
- **cobertura de términos** (palabras clave): qué fracción del "peso" de la
  consulta aparece en el documento. Vale ~0 si no comparte ningún término útil.
- **similitud coseno** (embeddings): acierta en sinónimos y paráfrasis.

Al ser absolutas (no normalizadas por el máximo), permiten **abstenerse** cuando
nada es realmente relevante.
"""

from __future__ import annotations

import math
import re
from collections import Counter

_WORD = re.compile(r"[a-záéíóúüñ0-9]{2,}")

# Stopwords (es/en) que no aportan relevancia semántica.
STOPWORDS = {
    "de", "la", "el", "los", "las", "un", "una", "unos", "unas", "y", "o", "u",
    "que", "en", "con", "por", "para", "es", "son", "al", "del", "se", "su",
    "lo", "como", "más", "mas", "pero", "si", "no", "ya", "the", "of", "to",
    "and", "or", "is", "are", "a", "an", "in", "on", "for",
}


def tokenize(text: str) -> list[str]:
    """Trocea a términos en minúsculas, sin stopwords."""
    return [t for t in _WORD.findall(text.lower()) if t not in STOPWORDS]


def keyword_scores(query: str, docs: list[str]) -> list[float]:
    """Cobertura IDF de la consulta en cada documento, en [0, 1]."""
    terms = set(tokenize(query))
    if not terms or not docs:
        return [0.0] * len(docs)

    doc_terms = [set(tokenize(doc)) for doc in docs]
    n_docs = len(docs)
    df = Counter()
    for tokens in doc_terms:
        for term in terms & tokens:
            df[term] += 1

    idf = {
        term: math.log(1 + (n_docs - df.get(term, 0) + 0.5) / (df.get(term, 0) + 0.5))
        for term in terms
    }
    total = sum(idf.values()) or 1.0

    return [
        sum(idf[term] for term in terms if term in tokens) / total for tokens in doc_terms
    ]
