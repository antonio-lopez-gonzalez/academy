"""Tools del agente.

Cada tool declara un esquema (nombre, descripción, argumentos) y un handler que
**no lanza excepciones**: los errores se devuelven como observación para el modelo.
"""

from __future__ import annotations

import json
from dataclasses import dataclass
from pathlib import Path
from typing import Any, Callable


@dataclass
class Tool:
    """Definición de una tool (esquema + handler)."""

    name: str
    description: str
    parameters: dict[str, Any]
    handler: Callable[..., str]


def build_tools(retriever: Callable[[str], list], academy_root: Path) -> dict[str, Tool]:
    """Construye las tools del Copiloto a partir de un `retriever` y la raíz de la Academy."""

    def search_lessons(query: str) -> str:
        """Busca fragmentos relevantes en las lecciones indexadas."""
        results = retriever(query)
        if not results:
            return "Sin resultados para esa búsqueda."
        lines = [f"[{i + 1}] {chunk.source}\n    {chunk.text[:220]}" for i, (_, chunk) in enumerate(results)]
        return "\n".join(lines)

    def open_module(module: str) -> str:
        """Lista las lecciones (ficheros) de un módulo, p. ej. '04-rag'."""
        hits = sorted(academy_root.glob(f"Learn */web/modulos/{module}/*.html"))
        if not hits:
            return f"No encuentro el módulo {module!r}."
        return "\n".join(f"- {p.parent.name}/{p.name}" for p in hits)

    def count_progress() -> str:
        """Cuenta las lecciones marcadas como completadas (si hay un registro local)."""
        record = Path("data/progress.json")
        if not record.exists():
            return "No hay progreso registrado."
        data = json.loads(record.read_text(encoding="utf-8"))
        return f"{len(data)} lecciones completadas."

    return {
        "search_lessons": Tool(
            "search_lessons",
            "Busca fragmentos de las lecciones del curso para responder una pregunta.",
            {"query": {"type": "string", "description": "Pregunta o términos a buscar"}},
            search_lessons,
        ),
        "open_module": Tool(
            "open_module",
            "Lista las lecciones de un módulo concreto del curso.",
            {"module": {"type": "string", "description": "Carpeta del módulo, p. ej. '04-rag'"}},
            open_module,
        ),
        "count_progress": Tool(
            "count_progress",
            "Cuenta las lecciones completadas por el alumno.",
            {},
            count_progress,
        ),
    }


def dispatch(tools: dict[str, Tool], name: str, arguments: dict[str, Any]) -> str:
    """Ejecuta una tool validando el nombre; nunca propaga excepciones."""
    tool = tools.get(name)
    if tool is None:
        return f"Error: tool desconocida {name!r}. Disponibles: {', '.join(tools)}."
    try:
        return tool.handler(**arguments)
    except TypeError as exc:
        return f"Error en los argumentos de {name!r}: {exc}"
    except Exception as exc:  # noqa: BLE001 — se convierte en observación
        return f"Error ejecutando {name!r}: {exc}"
