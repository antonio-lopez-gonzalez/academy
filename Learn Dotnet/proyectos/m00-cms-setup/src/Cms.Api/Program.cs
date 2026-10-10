// ──────────────────────────────────────────────────────────────────────────
// Cms.Api — Setup inicial del CMS
// Módulo 0: Preparación
// ──────────────────────────────────────────────────────────────────────────
// Esta es la estructura base del CMS que construiremos durante todo el curso.
// Por ahora es un esqueleto mínimo con:
//   - Health Check (para monitoreo)
//   - Swagger (para documentación de API)
//   - Estructura preparada para DI, EF Core, JWT, etc.
// En los módulos siguientes añadiremos modelos, servicios, repositorios, etc.
// ──────────────────────────────────────────────────────────────────────────

// ─────────────────────────── 1. Builder ───────────────────────────────────
// WebApplication.CreateBuilder es la fábrica principal en ASP.NET Core.
// Configura el servidor web (Kestrel), el contenedor de DI,
// el sistema de configuración (appsettings.json + env vars + CLI args)
// y el sistema de logging.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ─────────────────────────── 2. Servicios ─────────────────────────────────
// IServiceCollection: aquí registramos todos los servicios que usará la app.
// Singleton (1 instancia para toda la app)
// Scoped (1 instancia por petición HTTP)
// Transient (nueva instancia cada vez que se pide)
// Por ahora solo registramos lo mínimo necesario.

// Swagger / OpenAPI: documentación interactiva de la API.
// En producción se puede desactivar si no se necesita.
// Scalar es una alternativa más ligera y rápida que Swagger UI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "CMS API",
        Version = "v1",
        Description = "Sistema de Gestión de Contenidos (CMS) Headless — API REST"
    });
});

// Health Checks: endpoint para monitoreo (Kubernetes, balanceadores, etc.)
// Más adelante añadiremos checks para PostgreSQL, Redis, etc.
builder.Services.AddHealthChecks();

// ─────────────────────────── 3. App ───────────────────────────────────────
// Build() compila el contenedor de DI y crea la aplicación.
WebApplication app = builder.Build();

// ─────────────────────── 4. Pipeline HTTP ─────────────────────────────────
// El orden de los middlewares IMPORTA. Se ejecutan en el orden en que se añaden.
// Cada middleware puede:
//   1. Procesar la request antes del siguiente
//   2. Llamar al siguiente middleware (next())
//   3. Procesar la response después del siguiente

if (app.Environment.IsDevelopment())
{
    // Swagger UI solo en desarrollo (no exponer en producción)
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CMS API v1");
        options.RoutePrefix = string.Empty; // Swagger en la raíz: http://localhost:5000/
    });
}

// HTTPS en desarrollo. En producción lo maneja el reverse proxy (nginx, Caddy).
app.UseHttpsRedirection();

// ─────────────────────────── 5. Endpoints ─────────────────────────────────
// Minimal APIs: definimos endpoints directamente en Program.cs.
// Más adelante migraremos a ControllerBase o usaremos MapGroup para organizar.

// GET / → Redirige a Swagger en desarrollo
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription(); // No aparece en Swagger

// GET /api/health → Health Check endpoint
// Los balanceadores de carga y Kubernetes usan este endpoint
// para saber si la aplicación está viva y recibiendo tráfico.
app.MapHealthChecks("/api/health");

// GET /api/info → Información básica del CMS
// Endpoint que muestra la versión de la API y estado.
// Más adelante añadiremos versión de BD, número de posts, etc.
app.MapGet("/api/info", () => new
{
    Name = "CMS API",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Status = "Running",
    Timestamp = DateTime.UtcNow
})
.WithName("GetApiInfo")
.WithOpenApi();

// ─────────────────────────── 6. Arranque ──────────────────────────────────
// Run() inicia el servidor web Kestrel y bloquea el hilo principal.
// La aplicación escucha en http://localhost:5000 y https://localhost:5001.
// Los puertos se pueden configurar en launchSettings.json o appsettings.json.
app.Run();
