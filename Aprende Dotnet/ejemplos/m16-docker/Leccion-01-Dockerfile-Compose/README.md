# Módulo 16 — Docker y Despliegue

## Lección 1 — Dockerfile + Docker Compose

### Dockerfile Multi-Stage

El Dockerfile usa **multi-stage builds** para optimizar el tamaño de la imagen final:

1. **Stage 1 (`build`)**: Usa la imagen `dotnet/sdk:8.0` (~800 MB) que incluye el compilador y todas las herramientas. Aquí se ejecuta `dotnet restore` y `dotnet publish`.
2. **Stage 2 (`runtime`)**: Usa la imagen `dotnet/aspnet:8.0` (~200 MB) que solo contiene el runtime. Copia los binarios publicados desde el stage 1.

**¿Por qué multi-stage?** La imagen final es ~4 veces más pequeña. Solo incluye lo necesario para ejecutar.

**Optimización de capas:**
- Primero se copian los `.csproj` y se ejecuta `dotnet restore`
- Luego se copia el resto del código
- Si los `.csproj` no cambian, Docker reutiliza la capa de restore (mucho más rápido)

### Docker Compose

El archivo `docker-compose.yml` orquesta 3 servicios:
- `api`: Nuestra API .NET 8
- `postgres`: Base de datos PostgreSQL 16
- `redis`: Caché Redis 7

**Comandos útiles:**
```bash
docker compose build          # Construir las imágenes
docker compose up -d          # Levantar todo en segundo plano
docker compose logs -f api    # Ver logs de la API
docker compose down           # Detener y eliminar contenedores
docker compose down -v        # Detener, eliminar contenedores Y volúmenes
```

### .dockerignore

Reduce el contexto de build excluyendo archivos innecesarios:
- `bin/`, `obj/` — binarios de compilación local
- `.git/` — historial de git
- `node_modules/` — si usas frontend
- `*.md`, `docs/` — documentación
- Secrets (`.env`, `*.pfx`, `secrets.json`)

**IMPORTANTE:** Nunca incluyas secretos en la imagen Docker. Usa variables de entorno o gestores de secretos.
