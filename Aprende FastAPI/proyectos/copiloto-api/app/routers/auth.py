"""Router de autenticación (OAuth2 password → JWT)."""

from __future__ import annotations

from fastapi import APIRouter, Depends, HTTPException, status
from fastapi.security import OAuth2PasswordRequestForm

from ..security import create_access_token, hash_password, verify_password

router = APIRouter(prefix="/auth", tags=["auth"])

# Usuario demo en memoria (en el curso se usa SQLModel).
_USERS: dict[str, dict[str, str]] = {
    "admin": {"password": hash_password("admin"), "role": "admin"},
}


@router.post("/token")
def login(form: OAuth2PasswordRequestForm = Depends()) -> dict[str, str]:
    """Devuelve un `access_token` para las credenciales correctas."""
    user = _USERS.get(form.username)
    if user is None or not verify_password(form.password, user["password"]):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Credenciales inválidas",
            headers={"WWW-Authenticate": "Bearer"},
        )
    token = create_access_token(form.username, user["role"])
    return {"access_token": token, "token_type": "bearer"}
