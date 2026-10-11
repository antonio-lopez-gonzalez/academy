"""Contratos de entrada/salida (Pydantic v2)."""

from __future__ import annotations

from pydantic import BaseModel, Field


class AskRequest(BaseModel):
    """Pregunta al Copiloto."""

    question: str = Field(min_length=1, max_length=500)


class Source(BaseModel):
    """Fuente citada por la respuesta."""

    course: str
    module: str
    lesson: str


class AskResponse(BaseModel):
    """Respuesta del Copiloto con sus fuentes."""

    answer: str
    sources: list[Source] = []


class HealthResponse(BaseModel):
    """Estado del servicio."""

    status: str
    version: str


class ModulePublic(BaseModel):
    """Módulo del curso (vista pública)."""

    id: str
    title: str
