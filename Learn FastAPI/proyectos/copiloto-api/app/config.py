"""Configuración (variables de entorno; sin secretos en el repo)."""

from __future__ import annotations

from functools import lru_cache

from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    """Configuración de la API, leída del entorno (prefijo `COPILOTO_API_`)."""

    model_config = SettingsConfigDict(
        env_prefix="COPILOTO_API_", env_file=".env", extra="ignore"
    )

    app_name: str = "Copiloto API"
    database_url: str = "sqlite:///./data/copiloto.db"
    jwt_secret: str = "dev-secret-change-me"
    jwt_algorithm: str = "HS256"
    access_token_minutes: int = 60


@lru_cache
def get_settings() -> Settings:
    """Devuelve la configuración (cacheada)."""
    return Settings()
