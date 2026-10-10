using Asp.Versioning; // Espacio de nombres principal de API Versioning
using Microsoft.AspNetCore.Mvc;

// ─────────────────────────────────────────────────────────────
// Lección 06 — API Versioning
// Estrategias para versionar APIs REST sin romper clientes
// existentes: URL path, query string y headers.
// ─────────────────────────────────────────────────────────────

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════════════
// CONFIGURACIÓN DE API VERSIONING
// ═══════════════════════════════════════════════════════════════

builder.Services.AddApiVersioning(options =>
{
    // ── Versión por defecto ──────────────────────────────────
    // Si el cliente no especifica versión, se usa 1.0.
    options.DefaultApiVersion = new ApiVersion(1, 0);

    // ── Asumir versión por defecto ───────────────────────────
    // Si es true y no se especifica versión, se usa DefaultApiVersion.
    // Si es false, la petición sin versión recibe 400 Bad Request.
    options.AssumeDefaultVersionWhenUnspecified = true;

    // ── Reportar versiones disponibles ───────────────────────
    // Añade el header api-supported-versions y api-deprecated-versions en las respuestas.
    options.ReportApiVersions = true;

    // ── Lector de versiones compuesto ────────────────────────
    // Soporta múltiples estrategias a la vez. El orden importa:
    // 1. URL path: /api/v1/posts, /api/v2/posts
    // 2. Query string: /api/posts?api-version=1.0
    // 3. Header: api-version: 1.0
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),           // /api/v{version:apiVersion}/posts
        new QueryStringApiVersionReader("api-version"), // ?api-version=1.0
        new HeaderApiVersionReader("api-version")       // Header: api-version: 1.0
    );
})
.AddMvc() // Habilita versionado en controladores
.AddApiExplorer(options =>
{
    // ── Formato de grupo en Swagger ──────────────────────────
    // "v1", "v2", etc. — necesario si usas Swagger con versionado.
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true; // Reemplaza {version:apiVersion} en rutas
});

// ── Registrar controladores ──────────────────────────────────
builder.Services.AddControllers();

WebApplication app = builder.Build();
app.Urls.Add("http://localhost:5000");
app.MapControllers();

app.Run();

// ═══════════════════════════════════════════════════════════════
// CONTROLADORES CON VERSIONADO
// ═══════════════════════════════════════════════════════════════

// ─────────────────────────────────────────────────────────────
// VERSIÓN 1.0 (Endpoint con URL path versioning)
// ─────────────────────────────────────────────────────────────

/// <summary>
/// Controlador de posts versión 1.
/// Ruta: /api/v1/posts
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/posts")]
[ApiVersion("1.0")]           // Este controlador maneja la versión 1.0
[ApiVersion("1.1")]           // También maneja la 1.1 como fallback
public class PostsControllerV1 : ControllerBase
{
    /// <summary>GET /api/v1/posts — Lista de posts (v1, formato simple).</summary>
    [HttpGet]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("1.1")]
    public IActionResult GetAll()
    {
        // v1: datos simulados simples
        return Ok(new[]
        {
            new { Id = 1, Title = "Post v1: Introducción a .NET", Author = "Profesor" },
            new { Id = 2, Title = "Post v1: ASP.NET Core", Author = "Profesor" },
        });
    }

    /// <summary>GET /api/v1/posts/{id} — Obtener post individual (v1).</summary>
    [HttpGet("{id:int}")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("1.1")]
    public IActionResult GetById(int id)
    {
        return Ok(new { Id = id, Title = $"Post v1 #{id}", Content = "Contenido simple en v1" });
    }
}

// ─────────────────────────────────────────────────────────────
// VERSIÓN 2.0 (Mismo recurso, respuesta enriquecida)
// ─────────────────────────────────────────────────────────────

/// <summary>
/// Controlador de posts versión 2.
/// Ruta: /api/v2/posts
/// La v2 añade metadatos que la v1 no tiene (Author completo, tags, etc.).
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/posts")]
[ApiVersion("2.0")]           // Este controlador maneja la versión 2.0
public class PostsControllerV2 : ControllerBase
{
    /// <summary>GET /api/v2/posts — Lista de posts (v2, respuesta enriquecida).</summary>
    [HttpGet]
    [MapToApiVersion("2.0")]
    public IActionResult GetAll()
    {
        // v2: respuesta enriquecida con más campos
        return Ok(new[]
        {
            new
            {
                Id = 1,
                Title = "Post v2: Introducción a .NET 8",
                Author = new { Name = "Profesor .NET", Email = "profe@email.com" },
                Tags = new[] { "csharp", "dotnet", "tutorial" },
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                Status = "published"
            },
            new
            {
                Id = 2,
                Title = "Post v2: ASP.NET Core en profundidad",
                Author = new { Name = "Profesor .NET", Email = "profe@email.com" },
                Tags = new[] { "aspnetcore", "webapi", "tutorial" },
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                Status = "published"
            }
        });
    }

    /// <summary>GET /api/v2/posts/{id} — Obtener post individual (v2).</summary>
    [HttpGet("{id:int}")]
    [MapToApiVersion("2.0")]
    public IActionResult GetById(int id)
    {
        return Ok(new
        {
            Id = id,
            Title = $"Post v2 #{id}",
            Content = "Contenido enriquecido en v2 con soporte para Markdown y HTML.",
            Author = new { Name = "Profesor .NET", Email = "profe@email.com" },
            Tags = new[] { "api", "versioning" },
            CreatedAt = DateTime.UtcNow,
            Metadata = new { Views = 1234, Likes = 56 }
        });
    }

    /// <summary>
    /// POST /api/v2/posts — Crear post (solo en v2).
    /// Este endpoint NO existe en v1.
    /// </summary>
    [HttpPost]
    [MapToApiVersion("2.0")]
    public IActionResult Create([FromBody] CreatePostRequestV2 request)
    {
        return Created($"/api/v2/posts/99", new
        {
            Id = 99,
            request.Title,
            request.Content,
            request.Tags,
            Status = "draft",
            CreatedAt = DateTime.UtcNow
        });
    }
}

// ─────────────────────────────────────────────────────────────
// ENDPOINT SIN VERSIÓN (ApiVersionNeutral)
// ─────────────────────────────────────────────────────────────

/// <summary>
/// Controlador para endpoints que NO están versionados.
/// ApiVersionNeutral indica que este endpoint funciona para cualquier versión.
/// Útil para: health checks, métricas, configuración.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiVersionNeutral]
public class HealthController : ControllerBase
{
    /// <summary>GET /api/health — Health check (sin versión).</summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            AvailableVersions = new[] { "1.0", "1.1", "2.0" }
        });
    }
}

// ─────────────────────────────────────────────────────────────
// ENDPOINT VERSIONADO POR QUERY STRING (sin URL path)
// ─────────────────────────────────────────────────────────────

/// <summary>
/// Demostración de versionado SOLO por query string.
/// Ruta base sin versión: /api/users
/// Se accede con: /api/users?api-version=1.0  o  /api/users?api-version=2.0
/// </summary>
[ApiController]
[Route("api/users")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class UsersController : ControllerBase
{
    /// <summary>GET /api/users?api-version=1.0 — Lista simple.</summary>
    [HttpGet]
    [MapToApiVersion("1.0")]
    public IActionResult GetAllV1()
    {
        return Ok(new
        {
            Version = "1.0",
            Strategy = "Query string versioning: /api/users?api-version=1.0",
            Users = new[] { new { Name = "Ana" }, new { Name = "Carlos" } }
        });
    }

    /// <summary>GET /api/users?api-version=2.0 — Lista enriquecida.</summary>
    [HttpGet]
    [MapToApiVersion("2.0")]
    public IActionResult GetAllV2()
    {
        return Ok(new
        {
            Version = "2.0",
            Strategy = "Query string versioning: /api/users?api-version=2.0",
            Users = new[]
            {
                new { FullName = "Ana García", Email = "ana@email.com", Role = "Admin" },
                new { FullName = "Carlos López", Email = "carlos@email.com", Role = "Editor" }
            }
        });
    }
}

// ─────────────────────────────────────────────────────────────
// ENDPOINT DEPRECADO (v1.0 será eliminada próximamente)
// ─────────────────────────────────────────────────────────────

/// <summary>
/// Controlador con versión deprecada.
/// Al llamar a un endpoint deprecado, el servidor devuelve el header:
/// api-deprecated-versions: 1.0
/// El cliente puede usar esto para mostrar advertencias.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/legacy")]
[ApiVersion("1.0", Deprecated = true)] // ¡Esta versión está deprecada!
[ApiVersion("2.0")]
public class LegacyController : ControllerBase
{
    /// <summary>GET /api/v1/legacy — Endpoint de la v1 (deprecado).</summary>
    [HttpGet]
    [MapToApiVersion("1.0")]
    public IActionResult GetV1()
    {
        return Ok(new
        {
            Version = "1.0",
            Status = "DEPRECADO — Usa /api/v2/legacy en su lugar",
            SunsetDate = "2025-06-01",
            Message = "Esta versión será eliminada. Migra a v2 lo antes posible."
        });
    }

    /// <summary>GET /api/v2/legacy — Reemplazo moderno.</summary>
    [HttpGet]
    [MapToApiVersion("2.0")]
    public IActionResult GetV2()
    {
        return Ok(new
        {
            Version = "2.0",
            Status = "ACTIVO — Esta es la versión recomendada",
            Improvements = new[]
            {
                "Mejor rendimiento",
                "Nuevos campos: metadata, analytics",
                "Soporte para filtros avanzados"
            }
        });
    }
}

// ─────────────────────────────────────────────────────────────
// MODELO PARA POST v2
// ─────────────────────────────────────────────────────────────

/// <summary>Request para crear un post en v2.</summary>
public record CreatePostRequestV2(string Title, string Content, string[] Tags);

// ─────────────────────────────────────────────────────────────
// RESUMEN — Estrategias de versionado:
//
// 1. URL Path  → /api/v1/posts, /api/v2/posts
//    ✅ Más visible, fácil de cachear, proxies-friendly
//    ❌ "Contamina" la URL
//
// 2. Query String → /api/posts?api-version=1.0
//    ✅ URL limpia, fácil de usar en navegador
//    ❌ Menos visible, difícil de cachear
//
// 3. Header → api-version: 1.0
//    ✅ URL completamente limpia
//    ❌ Difícil de probar en navegador, algunos proxies lo complican
//
// Atributos clave:
//   [ApiVersion("1.0")]           → Define versión a nivel controller/action
//   [MapToApiVersion("1.0")]      → Mapea acción a versión específica
//   [ApiVersionNeutral]           → Endpoint sin versión
//   Deprecated = true             → Marca versión como obsoleta
// ─────────────────────────────────────────────────────────────
