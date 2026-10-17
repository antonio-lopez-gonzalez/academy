using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

// ─────────────────────────────────────────────────────────────
// Lección 04 — Exception Handling + Problem Details (RFC 9457)
// Manejo global de excepciones con IExceptionHandler (.NET 8)
// y Problem Details para respuestas de error estandarizadas.
// ─────────────────────────────────────────────────────────────

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ── Registrar Problem Details ────────────────────────────────
// ProblemDetails es el estándar RFC 9457 para respuestas de error.
// Añade type, title, status, detail, instance a cada error.
builder.Services.AddProblemDetails(options =>
{
    // Customiza el ProblemDetails según el tipo de error
    options.CustomizeProblemDetails = context =>
    {
        // Añade el trace ID para correlacionar logs con errores
        context.ProblemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

        // Añade timestamp ISO 8601
        context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow.ToString("O");

        // Añade el path de la request que falló
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
    };
});

// ── Registrar el manejador global de excepciones ─────────────
// IExceptionHandler es la forma moderna (.NET 8) de manejar excepciones.
// Reemplaza al middleware UseExceptionHandler() con una interfaz inyectable.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// ── Registrar controladores ──────────────────────────────────
builder.Services.AddControllers();

WebApplication app = builder.Build();
app.Urls.Add("http://localhost:5000");

// ── IMPORTANTE: orden de middlewares ─────────────────────────
// 1. ExceptionHandler debe ir PRIMERO para capturar todos los errores
app.UseExceptionHandler(); // Activa IExceptionHandler registrado
// 2. StatusCodePages convierte status codes (401, 403, 404...) en ProblemDetails
app.UseStatusCodePages();
// 3. Routing y endpoints al final
app.MapControllers();

app.Run();

// ═══════════════════════════════════════════════════════════════
// EXCEPCIONES PERSONALIZADAS
// ═══════════════════════════════════════════════════════════════

/// <summary>Excepción lanzada cuando un recurso no se encuentra.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entityName, object id)
        : base($"'{entityName}' con ID '{id}' no encontrado.") { }
}

/// <summary>Excepción lanzada cuando hay error de validación de negocio.</summary>
public class DomainValidationException : Exception
{
    public IEnumerable<string> Errors { get; }

    public DomainValidationException(string message) : base(message)
    {
        Errors = new[] { message };
    }

    public DomainValidationException(IEnumerable<string> errors)
        : base(string.Join("; ", errors))
    {
        Errors = errors;
    }
}

/// <summary>Excepción lanzada cuando hay un conflicto de estado (ej: email duplicado).</summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

// ═══════════════════════════════════════════════════════════════
// GLOBAL EXCEPTION HANDLER (IExceptionHandler)
// ═══════════════════════════════════════════════════════════════

/// <summary>
/// Manejador global de excepciones implementando IExceptionHandler (.NET 8).
/// Captura TODAS las excepciones no manejadas y las convierte en ProblemDetails.
/// 
/// Ventajas sobre UseExceptionHandler():
/// - Inyectable (puede usar DI)
/// - Testeable unitariamente
/// - Puede haber múltiples handlers encadenados
/// - Tipado fuerte (TryHandleAsync con tipo Exception)
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Intenta manejar la excepción. Si devuelve true, la excepción queda manejada.
    /// Si devuelve false, se pasa al siguiente handler o al comportamiento por defecto.
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Determinamos el ProblemDetails según el tipo de excepción
        ProblemDetails problemDetails = exception switch
        {
            NotFoundException notFound => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Recurso no encontrado",
                Detail = notFound.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            },

            DomainValidationException validation => new ValidationProblemDetails(
                validation.Errors.Select((e, i) => new { Key = $"Error{i}", Value = new[] { e } })
                    .ToDictionary(x => x.Key, x => x.Value))
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Error de validación",
                Detail = validation.Message,
                Type = "https://tools.ietf.org/html/rfc4918#section-11.2"
            },

            ConflictException conflict => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflicto",
                Detail = conflict.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            },

            ArgumentException argEx => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Argumento inválido",
                Detail = argEx.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            },

            // Excepción genérica (500 Internal Server Error)
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Error interno del servidor",
                Detail = "Ha ocurrido un error inesperado. Contacta al administrador.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            }
        };

        // Loguear la excepción con nivel adecuado
        LogLevel logLevel = exception switch
        {
            NotFoundException => LogLevel.Warning,      // 404 no es culpa del servidor
            DomainValidationException => LogLevel.Warning,
            ConflictException => LogLevel.Warning,
            _ => LogLevel.Error                         // 500 sí es grave
        };

        _logger.Log(logLevel, exception, "Error manejado: {ExceptionType} - {Message}",
            exception.GetType().Name, exception.Message);

        // Escribir la respuesta con el status code y ProblemDetails como JSON
        httpContext.Response.StatusCode = problemDetails.Status ?? 500;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails, problemDetails.GetType(), cancellationToken: cancellationToken);

        return true; // Excepción manejada
    }
}

// ═══════════════════════════════════════════════════════════════
// CONTROLADOR DE DEMOSTRACIÓN
// ═══════════════════════════════════════════════════════════════

[ApiController]
[Route("api")]
public class PostsController : ControllerBase
{
    private static readonly List<Post> _posts = new()
    {
        new Post(1, "Primer post", "Contenido inicial", "activo"),
        new Post(2, "Segundo post", "Más contenido", "borrador"),
    };

    /// <summary>
    /// GET /api/posts — Lista todos los posts (éxito normal).
    /// </summary>
    [HttpGet("posts")]
    public IActionResult GetAll() => Ok(_posts);

    /// <summary>
    /// GET /api/posts/{id} — Obtener por ID.
    /// Lanza NotFoundException si el ID no existe → capturada por GlobalExceptionHandler.
    /// </summary>
    [HttpGet("posts/{id:int}")]
    public IActionResult GetById(int id)
    {
        Post? post = _posts.Find(p => p.Id == id);
        if (post is null)
            throw new NotFoundException("Post", id);

        return Ok(post);
    }

    /// <summary>
    /// POST /api/posts — Crear post.
    /// Lanza DomainValidationException si los datos no son válidos.
    /// Lanza ConflictException si el título ya existe.
    /// </summary>
    [HttpPost("posts")]
    public IActionResult Create([FromBody] CreatePostRequest request)
    {
        // Validación manual → lanza excepción de dominio
        List<string> errors = new();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("El título es obligatorio.");

        if (request.Title is { Length: > 100 })
            errors.Add("El título no puede superar los 100 caracteres.");

        if (string.IsNullOrWhiteSpace(request.Content))
            errors.Add("El contenido es obligatorio.");

        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        // Verificar unicidad
        if (_posts.Any(p => p.Title.Equals(request.Title, StringComparison.OrdinalIgnoreCase)))
            throw new ConflictException($"Ya existe un post con el título '{request.Title}'.");

        Post post = new(_posts.Max(p => p.Id) + 1, request.Title, request.Content, "borrador");
        _posts.Add(post);
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    /// <summary>
    /// DELETE /api/posts/{id} — Eliminar post.
    /// </summary>
    [HttpDelete("posts/{id:int}")]
    public IActionResult Delete(int id)
    {
        Post? post = _posts.Find(p => p.Id == id);
        if (post is null)
            throw new NotFoundException("Post", id);

        _posts.Remove(post);
        return NoContent();
    }

    /// <summary>
    /// GET /api/error — Simula un error 500 para probar el handler.
    /// </summary>
    [HttpGet("error")]
    public IActionResult SimulateError()
    {
        throw new InvalidOperationException("Este es un error simulado para probar el ExceptionHandler");
    }

    /// <summary>
    /// GET /api/server-error — 500 devuelto explícitamente con Problem().
    /// </summary>
    [HttpGet("server-error")]
    public IActionResult ServerError()
    {
        // Problem() es un helper de ControllerBase que devuelve ProblemDetails directamente
        return Problem(
            detail: "Error interno simulado desde el controlador",
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Error del servidor");
    }

    /// <summary>
    /// GET /api/bad-request — 400 con Problem().
    /// </summary>
    [HttpGet("bad-request")]
    public IActionResult BadRequestDemo()
    {
        return Problem(
            detail: "Esta petición no es válida porque faltan parámetros.",
            statusCode: StatusCodes.Status400BadRequest,
            title: "Petición incorrecta");
    }
}

// ─────────────────────────────────────────────────────────────
// MODELOS
// ─────────────────────────────────────────────────────────────

public record Post(int Id, string Title, string Content, string Status);
public record CreatePostRequest(
    [Required] string Title,
    [Required] string Content
);

// ─────────────────────────────────────────────────────────────
// RESUMEN:
//
// UseExceptionHandler vs IExceptionHandler:
// ┌─────────────────────────┬──────────────────────────────────┐
// │ UseExceptionHandler     │ Middleware clásico, lambda inline│
// │ IExceptionHandler       │ Interfaz inyectable (.NET 8+)    │
// │                         │ - Puede usar DI                  │
// │                         │ - Varios handlers encadenados    │
// │                         │ - Testeable                      │
// │                         │ - Más limpio y modular           │
// └─────────────────────────┴──────────────────────────────────┘
//
// ProblemDetails (RFC 9457):
// - type: URI que identifica el tipo de error
// - title: Resumen legible del error
// - status: Código HTTP
// - detail: Explicación detallada
// - instance: URI que identifica la ocurrencia específica
// - extensions: Campos adicionales (traceId, timestamp, etc.)
// ─────────────────────────────────────────────────────────────
