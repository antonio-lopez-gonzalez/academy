"""Aplicación FastAPI (composition root)."""

from __future__ import annotations

from collections.abc import AsyncIterator
from contextlib import asynccontextmanager

from fastapi import FastAPI

from .config import get_settings
from .db import init_db
from .routers import ask, auth, health, modules


@asynccontextmanager
async def lifespan(_: FastAPI) -> AsyncIterator[None]:
    """Arranque/apagado: prepara la base de datos (y, en el curso, los recursos)."""
    init_db()
    yield


def create_app() -> FastAPI:
    """Construye la aplicación FastAPI."""
    settings = get_settings()
    app = FastAPI(title=settings.app_name, version="0.1.0", lifespan=lifespan)
    app.include_router(health.router)
    app.include_router(modules.router)
    app.include_router(auth.router)
    app.include_router(ask.router)
    return app


app = create_app()
