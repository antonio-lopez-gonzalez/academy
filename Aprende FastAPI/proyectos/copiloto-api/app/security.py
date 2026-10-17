"""Hash de contraseñas y tokens JWT."""

from __future__ import annotations

from datetime import datetime, timedelta, timezone

import jwt
from passlib.context import CryptContext

from .config import get_settings

_pwd = CryptContext(schemes=["bcrypt"], deprecated="auto")


def hash_password(raw: str) -> str:
    """Devuelve el hash de una contraseña."""
    return _pwd.hash(raw)


def verify_password(raw: str, hashed: str) -> bool:
    """Comprueba una contraseña contra su hash."""
    return _pwd.verify(raw, hashed)


def create_access_token(subject: str, role: str = "user") -> str:
    """Emite un JWT con `sub` y `exp`."""
    settings = get_settings()
    now = datetime.now(timezone.utc)
    payload = {
        "sub": subject,
        "role": role,
        "iat": now,
        "exp": now + timedelta(minutes=settings.access_token_minutes),
    }
    return jwt.encode(payload, settings.jwt_secret, algorithm=settings.jwt_algorithm)


def decode_token(token: str) -> dict:
    """Verifica y decodifica un JWT (lanza `jwt.PyJWTError` si no es válido)."""
    settings = get_settings()
    return jwt.decode(token, settings.jwt_secret, algorithms=[settings.jwt_algorithm])
