"""Router del catálogo de módulos."""

from __future__ import annotations

from fastapi import APIRouter

from ..schemas import ModulePublic

router = APIRouter(prefix="/modules", tags=["catalog"])

_MODULES: list[dict[str, str]] = [
    {"id": "00", "title": "Preparación"},
    {"id": "04", "title": "RAG"},
    {"id": "05", "title": "Agentes y tools"},
]


@router.get("", response_model=list[ModulePublic])
def list_modules() -> list[ModulePublic]:
    """Lista los módulos del curso."""
    return [ModulePublic(**item) for item in _MODULES]
