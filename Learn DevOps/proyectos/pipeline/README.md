# Publicar el Copiloto — pipeline CI/CD

Pipeline de **CI/CD** de la `Copiloto API` (curso `Learn FastAPI`). Es el **proyecto conductor** del
curso [`Learn DevOps`](../../spec.md): un flujo `commit → CI → imagen inmutable → CD` de punta a punta.

## Flujos

| Workflow | Se dispara con | Hace |
|---|---|---|
| `.github/workflows/ci.yml` | push / PR | lint + tests + cobertura |
| `.github/workflows/publish.yml` | tag `v*` | build de imagen (multi-stage) + push a GHCR por digest, con SBOM |
| `.github/workflows/deploy.yml` | tras publicar | staging automático + producción con aprobación |

## Reglas

- **Un solo artefacto**: la imagen que se prueba es la que se promueve (por **digest**), nunca se reconstruye.
- **Secretos** fuera del repositorio (GitHub Secrets / OIDC); permisos mínimos del `GITHUB_TOKEN`.
- **Mismo commit**, entornos distintos: la configuración entra por variables de entorno.

## Estructura

```text
pipeline/
├── .github/workflows/{ci,publish,deploy}.yml
├── Dockerfile
├── .dockerignore
├── scripts/smoke.sh
├── infra/                 # Terraform (OpenTofu)
│   ├── versions.tf
│   ├── main.tf
│   ├── variables.tf
│   └── outputs.tf
└── README.md
```

> Sí, esto es código: se versiona, se revisa y se ejecuta igual que la aplicación.
