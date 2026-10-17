# Copiloto API

API HTTP del **Copiloto** (curso `Aprende IA`) construida con **FastAPI**. Es el **proyecto conductor**
del curso [`Aprende FastAPI`](../../spec.md): crece módulo a módulo hasta ser una API con RAG,
persistencia, autenticación, tests y despliegue.

## Requisitos

- **Python 3.12+** y [`uv`](https://docs.astral.sh/uv/).

## Uso

```bash
cd "Aprende FastAPI/proyectos/copiloto-api"
uv sync                       # instala dependencias
uv run uvicorn app.main:app --reload
# Abre http://localhost:8000/docs
```

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/health` | Estado del servicio |
| GET | `/modules` | Catálogo de módulos del curso |
| POST | `/auth/token` | Login (OAuth2 password) → JWT |
| POST | `/ask` | Pregunta al Copiloto (requiere token) |

## Estructura

```text
copiloto-api/
├── pyproject.toml
├── README.md
├── data/                 # base de datos generada (no versionada)
├── tests/
│   └── test_health.py
└── app/
    ├── main.py           # create_app() + lifespan
    ├── config.py         # Settings (pydantic-settings)
    ├── db.py             # engine + get_session
    ├── models.py         # tablas SQLModel (User, QueryLog)
    ├── schemas.py        # modelos Pydantic (contratos)
    ├── security.py       # hash + JWT
    ├── deps.py           # dependencias (auth, DB)
    └── routers/          # health, modules, ask, auth
```

> Esqueleto inicial: cada módulo del curso implementa una capa. Los secretos (`COPILOTO_API_JWT_SECRET`)
> van por variables de entorno / `.env`, nunca en el repositorio.
