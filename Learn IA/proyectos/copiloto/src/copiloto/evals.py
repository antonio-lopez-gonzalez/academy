"""Evaluación del Copiloto: dataset + métricas deterministas (+ juez opcional)."""

from __future__ import annotations

import json
from dataclasses import dataclass
from pathlib import Path


@dataclass
class Case:
    """Un caso de evaluación: pregunta y expectativa."""

    question: str
    expect_citation: bool = True
    must_abstain: bool = False


@dataclass
class Result:
    """Resultado de evaluar un caso."""

    case: Case
    passed: bool
    detail: str = ""


def load_dataset(path: Path) -> list[Case]:
    """Carga el dataset JSONL de evaluación."""
    cases: list[Case] = []
    for line in path.read_text(encoding="utf-8").splitlines():
        line = line.strip()
        if not line:
            continue
        row = json.loads(line)
        cases.append(
            Case(
                question=row["question"],
                expect_citation=row.get("expect_citation", True),
                must_abstain=row.get("must_abstain", False),
            )
        )
    return cases


def check_abstention(answer_text: str) -> bool:
    """Comprueba que una respuesta sin contexto se abstiene."""
    return any(phrase in answer_text.lower() for phrase in ("no lo sé", "no lo se", "no tengo"))
