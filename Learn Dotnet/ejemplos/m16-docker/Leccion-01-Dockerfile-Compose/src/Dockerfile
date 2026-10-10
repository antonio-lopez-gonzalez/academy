# ─── MÓDULO 16 ─── Docker: Dockerfile multi-stage para .NET 8 ───
# Un Dockerfile multi-stage optimiza el tamaño final de la imagen:
# - Stage 1 (build): SDK completo para compilar (GRANDE, ~800 MB)
# - Stage 2 (runtime): solo el runtime para ejecutar (PEQUEÑO, ~200 MB)

# ─────────────────────────────────────────────────────────────────────────────
# STAGE 1: BUILD — Compilamos la aplicación
# ─────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar SOLO los archivos .csproj primero (aprovechar caché de capas Docker)
# Si los .csproj no cambian, Docker reutiliza la capa de restore
COPY *.csproj .
RUN dotnet restore

# Copiar el resto del código fuente y publicar
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

# ─────────────────────────────────────────────────────────────────────────────
# STAGE 2: RUNTIME — Solo lo necesario para ejecutar
# ─────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Crear usuario no-root por seguridad
RUN adduser --disabled-password --gecos "" appuser
USER appuser

# Copiar los binarios publicados desde el stage de build
COPY --from=build /app .

# Exponer el puerto 8080 (puerto por defecto en .NET 8)
EXPOSE 8080

# Configurar URLs y entorno
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check: .NET 8 expone /healthz por defecto si configuras health checks
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/healthz || exit 1

# Punto de entrada
ENTRYPOINT ["dotnet", "Cms.Api.dll"]
