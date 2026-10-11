"""Evaluación del Copiloto: dataset + comprobaciones deterministas."""

from __future__ import annotations

import json
from collections.abc import Callable
from dataclasses import dataclass
from pathlib import Path

from .rag import Answer


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


def load_dataset(path: str | Path) -> list[Case]:
    """Carga el dataset JSONL de evaluación."""
    cases: list[Case] = []
    for line in Path(path).read_text(encoding="utf-8").splitlines():
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


def check_abstention(text: str) -> bool:
    """¿La respuesta se abstiene en lugar de inventar?"""
    low = text.lower()
    return any(p in low for p in ("no lo sé", "no lo se", "no encuentro", "no tengo"))


def check_citation(text: str) -> bool:
    """¿La respuesta cita alguna fuente (`[n]`)?"""
    return "[" in text and "]" in text


def run_evals(cases: list[Case], answer_fn: Callable[[str], Answer]) -> list[Result]:
    """Ejecuta el dataset contra el Copiloto y evalúa cada caso."""
    results: list[Result] = []
    for case in cases:
        text = answer_fn(case.question).text
        if case.must_abstain:
            passed = check_abstention(text)
            detail = "debe abstenerse"
        elif case.expect_citation:
            passed = check_citation(text) and not check_abstention(text)
            detail = "debe citar"
        else:
            passed = True
            detail = ""
        results.append(Result(case, passed, detail))
    return results
