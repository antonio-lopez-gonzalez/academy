"""El bucle agentic: pensar → actuar → observar, con límite de pasos.

Implementa un ReAct ligero: se le muestra al modelo el catálogo de tools y debe
responder con un JSON `{"tool": ..., "arguments": {...}}` para actuar, o
`{"final": "..."}` cuando ya puede responder. Sin dependencias de proveedor.
"""

from __future__ import annotations

import json
import re
from dataclasses import dataclass, field

from .llm import LLMClient
from .tools import Tool, dispatch

MAX_STEPS = 6
_JSON = re.compile(r"\{[\s\S]*\}")
SYSTEM = (
    "Eres un agente que resuelve preguntas usando herramientas. Si necesitas una "
    'herramienta responde SOLO con JSON: {"tool": "<nombre>", "arguments": {...}}. '
    'Cuando ya puedas responder responde SOLO con JSON: {"final": "<respuesta>"}. '
    "No añadas texto fuera del JSON."
)


@dataclass
class Step:
    """Un paso: tool invocada y observación obtenida."""

    tool: str
    arguments: dict
    observation: str


@dataclass
class Run:
    """Resultado de una ejecución del agente."""

    answer: str
    steps: list[Step] = field(default_factory=list)


def _manifest(tools: dict[str, Tool]) -> str:
    lines = []
    for tool in tools.values():
        lines.append(f"- {tool.name}: {tool.description} args={json.dumps(tool.parameters)}")
    return "\n".join(lines)


def _parse(raw: str) -> dict | None:
    """Extrae el primer objeto JSON de la respuesta del modelo."""
    match = _JSON.search(raw)
    if not match:
        return None
    try:
        return json.loads(match.group(0))
    except json.JSONDecodeError:
        return None


def run(question: str, llm: LLMClient, tools: dict[str, Tool], max_steps: int = MAX_STEPS) -> Run:
    """Ejecuta el bucle del agente y devuelve la respuesta con sus pasos."""
    steps: list[Step] = []
    prompt = f"Pregunta: {question}\n\nHerramientas:\n{_manifest(tools)}\n\nResponde en JSON."
    for _ in range(max_steps):
        raw = llm.generate(prompt, system=SYSTEM)
        action = _parse(raw)
        if action is None or "final" in action:
            return Run(str(action.get("final", raw)) if action else raw, steps)
        name = str(action.get("tool", ""))
        arguments = action.get("arguments") or {}
        observation = dispatch(tools, name, arguments)
        steps.append(Step(name, arguments, observation))
        prompt += f"\nObservación de {name}: {observation}\nSiguiente paso (JSON):"
    return Run("(límite de pasos alcanzado sin respuesta final)", steps)
