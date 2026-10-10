# Proyecto CMS — Setup Inicial (Módulo 0)

---

## ¿Qué es?

El CMS (Sistema de Gestión de Contenidos) es el proyecto que construiremos **a lo largo de todo el curso**. Es una API headless similar a WordPress, pero solo backend (sin frontend visual).

**En este módulo 0**, solo creamos el esqueleto: un proyecto ASP.NET Core Web API vacío con estructura preparada para crecer.

---

## Estructura actual

```
m00-cms-setup/
├── README.md
└── src/
    └── Cms.Api/
        ├── Cms.Api.csproj              ← Proyecto ASP.NET Core
        ├── Program.cs                  ← Punto de entrada + Minimal APIs
        ├── appsettings.json            ← Configuración
        ├── appsettings.Development.json
        ├── Cms.Api.http                ← Pruebas de endpoints (REST Client)
        └── Properties/
            └── launchSettings.json     ← Perfiles de ejecución
```

---

## Endpoints disponibles

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/` | Redirige a Swagger UI |
| `GET` | `/swagger` | Documentación interactiva de la API |
| `GET` | `/api/health` | Health check (monitoreo) |
| `GET` | `/api/info` | Información de la API (nombre, versión, estado) |

---

## Cómo ejecutar

```bash
# Desde la raíz del repositorio
dotnet run --project proyectos/m00-cms-setup/src/Cms.Api

# O con hot reload (se reinicia al guardar cambios)
dotnet watch run --project proyectos/m00-cms-setup/src/Cms.Api
```

Abre <https://localhost:5001/swagger> en el navegador.

---

## Lo que vendrá después

| Módulo | Qué añadiremos |
|---|---|
| M01 | Modelos de dominio: `Post`, `Category`, `Tag`, `User` |
| M02 | Servicios con interfaces |
| M03 | Principios SOLID aplicados |
| M04 | Source Generators para DTOs |
| M05 | Inyección de dependencias profesional |
| M06 | Clean Architecture + CQRS + MediatR |
| M07 | Entity Framework Core + PostgreSQL |
| M08 | Endpoints CRUD REST completos |
| M09 | Validación con FluentValidation |
| M10 | Middleware: logging, correlation ID, rate limiting |
| M11 | JWT + roles (Admin, Editor, Author) |
| M12 | Suite completa de tests |
| M13 | Caching con Redis |
| M14 | Notificaciones en tiempo real con SignalR |
| M15 | Background jobs con Hangfire |
| M16 | Docker Compose + CI/CD |
| M17 | gRPC + OpenTelemetry + Polly |
| M18 | Auditoría de seguridad OWASP Top 10 |

---

**Estado actual:** Esqueleto funcional con health check y documentación Swagger.
