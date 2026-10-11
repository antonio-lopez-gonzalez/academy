"""Tablas de la base de datos (SQLModel)."""

from __future__ import annotations

from datetime import datetime, timezone

from sqlmodel import Field, SQLModel


def _now() -> datetime:
    return datetime.now(timezone.utc)


class User(SQLModel, table=True):
    """Usuario de la API."""

    id: int | None = Field(default=None, primary_key=True)
    email: str = Field(index=True, unique=True)
    hashed_password: str
    role: str = "user"


class QueryLog(SQLModel, table=True):
    """Histórico de consultas al Copiloto."""

    id: int | None = Field(default=None, primary_key=True)
    question: str
    answer: str
    owner_id: int | None = Field(default=None, foreign_key="user.id", index=True)
    created_at: datetime = Field(default_factory=_now)
