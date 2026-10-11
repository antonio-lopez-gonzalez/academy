"""Motor y sesión de base de datos (SQLModel)."""

from __future__ import annotations

from collections.abc import Iterator

from sqlmodel import Session, SQLModel, create_engine

from .config import get_settings

_settings = get_settings()
engine = create_engine(
    _settings.database_url,
    connect_args={"check_same_thread": False},
)


def init_db() -> None:
    """Crea las tablas (en el curso se sustituye por migraciones Alembic)."""
    SQLModel.metadata.create_all(engine)


def get_session() -> Iterator[Session]:
    """Dependencia: una sesión por petición."""
    with Session(engine) as session:
        yield session
