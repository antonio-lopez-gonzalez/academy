"""Dependencias reutilizables: sesión de BD y autenticación."""

from __future__ import annotations

from collections.abc import Callable

import jwt
from fastapi import Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer

from .security import decode_token

oauth2_scheme = OAuth2PasswordBearer(tokenUrl="/auth/token")


def get_current_user(token: str = Depends(oauth2_scheme)) -> dict:
    """Devuelve el usuario del token o lanza 401."""
    try:
        payload = decode_token(token)
    except jwt.PyJWTError as exc:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Token inválido o caducado",
            headers={"WWW-Authenticate": "Bearer"},
        ) from exc
    return {"sub": payload["sub"], "role": payload.get("role", "user")}


def require_role(role: str) -> Callable[[dict], dict]:
    """Fábrica de dependencias: exige un rol concreto."""

    def check(user: dict = Depends(get_current_user)) -> dict:
        if user["role"] != role:
            raise HTTPException(status.HTTP_403_FORBIDDEN, "Permisos insuficientes")
        return user

    return check
