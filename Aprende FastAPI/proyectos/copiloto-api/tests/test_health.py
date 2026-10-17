"""Tests básicos de Copiloto API."""

from __future__ import annotations

from fastapi.testclient import TestClient

from app.main import app


def test_health() -> None:
    with TestClient(app) as client:
        response = client.get("/health")
    assert response.status_code == 200
    assert response.json()["status"] == "ok"


def test_modules() -> None:
    with TestClient(app) as client:
        response = client.get("/modules")
    assert response.status_code == 200
    assert isinstance(response.json(), list)


def test_ask_requires_auth() -> None:
    with TestClient(app) as client:
        response = client.post("/ask", json={"question": "¿qué es RAG?"})
    assert response.status_code == 401
