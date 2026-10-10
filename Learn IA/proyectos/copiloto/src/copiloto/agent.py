"""El bucle agentic: pensar → actuar → observar, con límite de iteraciones."""

from __future__ import annotations

from dataclasses import dataclass, field

from .llm import LLMClient
from .tools import TOOLS, dispatch

MAX_STEPS = 6


@dataclass
class Step:
    """Un paso del agente: tool invocada y observación obtenida."""

    tool: str
    arguments: dict
    observation: str


@dataclass
class Run:
    """Resultado de una ejecución del agente."""

    answer: str
    steps: list[Step] = field(default_factory=list)


def run(question: str, llm: LLMClient) -> Run:
    """Bucle mínimo del agente.

    En el curso, el modelo decide la tool en cada paso; aquí dejamos una versión
    didáctica que llama a `search_lessons` una vez para ilustrar la mecánica.
    """
    steps: list[Step] = []
    if "search_lessons" in TOOLS:
        obs = dispatch("search_lessons", {"query": question})
        steps.append(Step("search_lessons", {"query": question}, obs))
    answer = llm.generate(f"Pregunta: {question}\nObservaciones: {steps}")
    return Run(answer=answer, steps=steps)
