"""Router de estado del servicio."""

from __future__ import annotations

from fastapi import APIRouter

from .. import __version__
from ..schemas import HealthResponse

router = APIRouter(tags=["system"])


@router.get("/health", response_model=HealthResponse)
def health() -> HealthResponse:
    """Comprueba que el servicio está vivo."""
    return HealthResponse(status="ok", version=__version__)
