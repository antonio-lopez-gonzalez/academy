# Módulo 16 — Docker y Despliegue

## Lección 2 — CI/CD con GitHub Actions

### Pipeline CI/CD

El archivo `ci.yml` define un pipeline que se ejecuta automáticamente en cada **push** y **pull request** a las ramas `main` y `develop`.

### Etapas del pipeline

1. **Checkout**: Clona el repositorio
2. **Setup .NET 8**: Instala el SDK de .NET 8
3. **Restore**: Descarga los paquetes NuGet
4. **Build**: Compila en modo Release
5. **Unit Tests**: Ejecuta tests unitarios (sin dependencias externas)
6. **Integration Tests**: Ejecuta tests de integración (con PostgreSQL y Redis como services)
7. **Publish**: Publica la aplicación y sube el artefacto

### Services en GitHub Actions

Los servicios (`postgres`, `redis`) son contenedores Docker que se levantan durante el workflow. Son ideales para integration tests sin necesidad de mocks.

### Extensiones del pipeline (para producción)

```yaml
# Build y push de imagen Docker
- name: Build Docker image
  run: docker build -t ghcr.io/usuario/cms-api:${{ github.sha }} .

- name: Push to GitHub Container Registry
  run: |
    echo "${{ secrets.GITHUB_TOKEN }}" | docker login ghcr.io -u ${{ github.actor }} --password-stdin
    docker push ghcr.io/usuario/cms-api:${{ github.sha }}

# Deploy a Azure Container Apps
- name: Deploy to Azure
  uses: azure/container-apps-deploy-action@v1
  with:
    imageToDeploy: ghcr.io/usuario/cms-api:${{ github.sha }}
    containerAppName: cms-api
    resourceGroup: cms-rg
```
