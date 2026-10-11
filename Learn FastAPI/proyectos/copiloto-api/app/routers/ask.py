"""Router de consultas al Copiloto (`/ask`)."""

from __future__ import annotations

from fastapi import APIRouter, Depends

from ..deps import get_current_user
from ..schemas import AskRequest, AskResponse, Source

router = APIRouter(tags=["copilot"])


@router.post("/ask", response_model=AskResponse)
def ask(payload: AskRequest, user: dict = Depends(get_current_user)) -> AskResponse:
    """Responde a una pregunta (stub; el módulo 7 lo conecta al RAG de Learn IA)."""
    answer = f"(stub) Respuesta a: {payload.question}"
    sources = [Source(course="Learn IA", module="04-rag", lesson="01-pipeline-rag")]
    return AskResponse(answer=answer, sources=sources)
