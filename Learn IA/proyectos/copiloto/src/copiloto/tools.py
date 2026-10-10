"""Tools que el agente puede ejecutar.

Cada tool se describe con un esquema (nombre, descripción, argumentos) y una
función `execute(...)` que **no lanza excepciones**: devuelve errores como
observaciones para el modelo.
"""

from __future__ import annotations

from dataclasses import dataclass
from typing import Any, Callable


@dataclass
class Tool:
    """Definición de una tool (esquema + handler)."""

    name: str
    description: str
    parameters: dict[str, Any]
    handler: Callable[..., str]


def search_lessons(query: str) -> str:
    """Busca lecciones relevantes. (Implementación real en el módulo 4.)"""
    return f"[search_lessons] pendiente de indexar: {query!r}"


def open_module(module: str) -> str:
    """Devuelve el índice de un módulo del curso. (Stub.)"""
    return f"[open_module] módulo {module}: pendiente"


def count_progress() -> str:
    """Cuenta el progreso guardado. (Stub.)"""
    return "[count_progress] sin progreso registrado"


TOOLS: dict[str, Tool] = {
    "search_lessons": Tool(
        "search_lessons",
        "Busca fragmentos de las lecciones del curso para responder una pregunta.",
        {"query": {"type": "string", "description": "Pregunta o términos a buscar"}},
        search_lessons,
    ),
    "open_module": Tool(
        "open_module",
        "Devuelve el índice (lecciones) de un módulo concreto del curso.",
        {"module": {"type": "string", "description": "Id del módulo, p. ej. '04'"}},
        open_module,
    ),
    "count_progress": Tool(
        "count_progress",
        "Cuenta las lecciones completadas por el alumno.",
        {},
        count_progress,
    ),
}


def dispatch(name: str, arguments: dict[str, Any]) -> str:
    """Ejecuta una tool validando el nombre; nunca propaga excepciones."""
    tool = TOOLS.get(name)
    if tool is None:
        return f"Error: tool desconocida {name!r}"
    try:
        return tool.handler(**arguments)
    except Exception as exc:  # noqa: BLE001 — se convierte en observación
        return f"Error ejecutando {name!r}: {exc}"
