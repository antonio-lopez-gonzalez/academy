# Copiloto en la nube — infraestructura como código

Infraestructura en **Azure** (con notas de equivalencia **AWS**) para desplegar la `Copiloto API`
(curso `Aprende FastAPI`). Es el **proyecto conductor** del curso
[`Aprende Cloud`](../../spec.md), todo definido con **Terraform**.

## Qué crea

- **Resource Group** y **Azure Container Registry** (ACR).
- **Container Apps Environment** + **Container App** (la API, con ingress HTTPS y autoscaling).
- **Azure Database for PostgreSQL** (flexible server) para el histórico.
- **Key Vault** (secretos) y **Log Analytics / Application Insights** (observabilidad).
- Etiquetas de coste en todos los recursos.

## Uso

```bash
cd "Aprende Cloud/proyectos/infra-cloud"
az login
terraform init
terraform plan  -var="image=ghcr.io/org/copiloto-api@sha256:…"
terraform apply
terraform output copiloto_url
```

## Reglas

- **Sin secretos** en el repo ni en `tfvars`: se pasan por `TF_VAR_*` o Key Vault.
- El artefacto se referencia por **digest** (inmutable), no por `latest`.
- Todo recurso lleva **etiquetas** (`project`, `environment`, `owner`, `managed-by`).
- El estado remoto (backend) se configura en `versions.tf`; en local, el estado **no se versiona**.

## Estructura

```text
infra-cloud/
├── versions.tf
├── variables.tf
├── main.tf
├── outputs.tf
├── terraform.tfvars.example
├── .github/workflows/deploy.yml
└── README.md
```
