using Microsoft.AspNetCore.Mvc; // ControllerBase, [ApiController], etc.

// ─────────────────────────────────────────────────────────────
// Lección 02 — Routing, Model Binding y Status Codes HTTP
// Cómo llegan las peticiones a tu código y cómo devolver
// respuestas HTTP correctas y semánticas.
// ─────────────────────────────────────────────────────────────

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ── Registrar controladores + soporte para XML ───────────────
// AddXmlSerializerFormatters permite que la API acepte y devuelva XML.
// Content Negotiation: el cliente elige JSON o XML con el header Accept.
builder.Services.AddControllers()
    .AddXmlSerializerFormatters();

WebApplication app = builder.Build();
app.Urls.Add("http://localhost:5000");
app.MapControllers();

app.Run();

// ═══════════════════════════════════════════════════════════════
// Toda la lógica está en el controlador de abajo.
// Ejecuta con: dotnet run --project ...
// Prueba con curl o el archivo .http incluido en el proyecto.
// ═══════════════════════════════════════════════════════════════

// ─────────────────────────────────────────────────────────────
// MODELOS
// ─────────────────────────────────────────────────────────────

/// <summary>Modelo para el body de creación de usuario</summary>
public record CreateUserRequest(string Name, string Email, int Age);

/// <summary>Modelo de respuesta</summary>
public record UserResponse(int Id, string Name, string Email, int Age, DateTime CreatedAt);

/// <summary>Request de búsqueda con paginación</summary>
public record SearchRequest(string Q, int Page = 1, int Size = 10);

/// <summary>Respuesta paginada genérica</summary>
public record PagedResponse<T>(List<T> Items, int Page, int Size, int Total);

/// <summary>Usuario en memoria (simula BD)</summary>
public class User
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime CreatedAt { get; init; }
}

// ─────────────────────────────────────────────────────────────
// CONTROLADOR: Demostración completa de Routing + Binding + Status Codes
// ─────────────────────────────────────────────────────────────

[ApiController]
[Route("api")]
public class DemoController : ControllerBase
{
    private static readonly List<User> _users = new()
    {
        new User { Id = 1, Name = "Ana García", Email = "ana@email.com", Age = 28, CreatedAt = DateTime.UtcNow.AddDays(-30) },
        new User { Id = 2, Name = "Carlos López", Email = "carlos@email.com", Age = 35, CreatedAt = DateTime.UtcNow.AddDays(-15) },
        new User { Id = 3, Name = "María Torres", Email = "maria@email.com", Age = 22, CreatedAt = DateTime.UtcNow.AddDays(-7) },
        new User { Id = 4, Name = "Pedro Ruiz", Email = "pedro@email.com", Age = 41, CreatedAt = DateTime.UtcNow },
    };

    private static readonly object _lock = new(); // Para thread-safety en memoria

    // ───────────────────────────────────────────────────────
    // ROUTING: Diferentes patrones de ruta
    // ───────────────────────────────────────────────────────

    /// <summary>
    /// GET /api/health — Ruta simple, sin parámetros.
    /// </summary>
    [HttpGet("health")]
    public IActionResult HealthCheck() => Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });

    /// <summary>
    /// GET /api/users/42/posts/99 — Rutas con múltiples segmentos variables.
    /// Las restricciones (:int) aseguran que el valor sea numérico → 404 si no.
    /// </summary>
    [HttpGet("users/{userId:int}/posts/{postId:int}")]
    public IActionResult GetUserPost(int userId, int postId)
    {
        // Al llamar a este endpoint, MVC extrae userId y postId de la URL automáticamente.
        // Si la URL no coincide (ej: /users/abc/posts/1), devuelve 404.
        return Ok(new
        {
            UserId = userId,
            PostId = postId,
            Message = $"Post {postId} del usuario {userId}"
        });
    }

    // ───────────────────────────────────────────────────────
    // MODEL BINDING: De dónde vienen los datos
    // ───────────────────────────────────────────────────────

    /// <summary>
    /// GET /api/search?q=net&page=2&size=5
    /// [FromQuery]: Los parámetros vienen de la query string.
    /// </summary>
    [HttpGet("search")]
    public IActionResult Search([FromQuery] SearchRequest request)
    {
        // MVC hace binding automático: ?q=net&page=2&size=5 → SearchRequest { Q="net", Page=2, Size=5 }
        return Ok(new
        {
            Query = request.Q,
            request.Page,
            request.Size,
            Message = $"Buscando '{request.Q}' - página {request.Page}, {request.Size} resultados/página"
        });
    }

    /// <summary>
    /// GET /api/users/5 — El ID viene de la ruta.
    /// [FromRoute] es opcional aquí porque el nombre coincide con {id} en la ruta.
    /// Lo ponemos explícito para demostrar.
    /// </summary>
    [HttpGet("users/{id:int}")]
    public IActionResult GetUser([FromRoute] int id)
    {
        User? user = _users.Find(u => u.Id == id);
        return user is not null
            ? Ok(new UserResponse(user.Id, user.Name, user.Email, user.Age, user.CreatedAt))
            : NotFound(new { Message = $"Usuario con ID {id} no encontrado" });
    }

    /// <summary>
    /// GET /api/users — Obtener todos los usuarios.
    /// </summary>
    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        List<UserResponse> response = _users
            .Select(u => new UserResponse(u.Id, u.Name, u.Email, u.Age, u.CreatedAt))
            .ToList();
        // 200 OK — éxito con datos
        return Ok(response);
    }

    /// <summary>
    /// POST /api/users — Crear un usuario.
    /// [FromBody]: El modelo se deserializa del cuerpo JSON de la request.
    /// Content-Type debe ser application/json.
    /// </summary>
    [HttpPost("users")]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { Message = "El nombre es obligatorio" }); // 400

        if (_users.Any(u => u.Email == request.Email))
            return Conflict(new { Message = $"El email '{request.Email}' ya existe" }); // 409

        lock (_lock)
        {
            User user = new()
            {
                Id = _users.Max(u => u.Id) + 1,
                Name = request.Name,
                Email = request.Email,
                Age = request.Age,
                CreatedAt = DateTime.UtcNow
            };
            _users.Add(user);

            UserResponse response = new(user.Id, user.Name, user.Email, user.Age, user.CreatedAt);
            // 201 Created: recurso creado. Devolvemos la URL del nuevo recurso.
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
        }
    }

    /// <summary>
    /// DELETE /api/users/5 — Eliminar usuario.
    /// </summary>
    [HttpDelete("users/{id:int}")]
    public IActionResult DeleteUser(int id)
    {
        User? user = _users.Find(u => u.Id == id);
        if (user is null)
            return NotFound(new { Message = $"Usuario con ID {id} no encontrado" }); // 404

        _users.Remove(user);
        return NoContent(); // 204: éxito, sin cuerpo en la respuesta
    }

    /// <summary>
    /// GET /api/protected — Demuestra [FromHeader] para leer headers custom.
    /// Header: X-Api-Key: secret123
    /// Header: api-version: 1.0
    /// </summary>
    [HttpGet("protected")]
    public IActionResult ProtectedEndpoint(
        [FromHeader(Name = "X-Api-Key")] string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
            return Unauthorized(new { Message = "X-Api-Key header requerido" }); // 401

        if (apiKey != "secret123")
            return StatusCode(403, new { Message = "API Key inválida" }); // 403 Forbidden

        return Ok(new { Message = "Acceso concedido", Key = apiKey });
    }

    /// <summary>
    /// GET /api/demo-status — Demuestra todos los status codes típicos.
    /// Usa el header X-Demo-Status para elegir cuál devolver.
    /// </summary>
    [HttpGet("demo-status")]
    public IActionResult DemoStatusCodes(
        [FromHeader(Name = "X-Demo-Status")] string statusCode)
    {
        // Devuelve el status code que pidas para que veas la diferencia
        return statusCode switch
        {
            "200" => Ok(new { Code = 200, Meaning = "OK — La petición fue exitosa" }),
            "201" => StatusCode(201, new { Code = 201, Meaning = "Created — Recurso creado" }),
            "204" => NoContent(), // Sin body
            "400" => BadRequest(new { Code = 400, Meaning = "Bad Request — Datos inválidos del cliente" }),
            "401" => Unauthorized(new { Code = 401, Meaning = "Unauthorized — No autenticado" }),
            "403" => StatusCode(403, new { Code = 403, Meaning = "Forbidden — Autenticado pero sin permisos" }),
            "404" => NotFound(new { Code = 404, Meaning = "Not Found — Recurso no existe" }),
            "409" => Conflict(new { Code = 409, Meaning = "Conflict — Conflicto con el estado actual" }),
            "422" => UnprocessableEntity(new { Code = 422, Meaning = "Unprocessable Entity — Datos válidos pero semánticamente incorrectos" }),
            "500" => StatusCode(500, new { Code = 500, Meaning = "Internal Server Error — Error inesperado del servidor" }),
            _ => Ok(new { Message = "Usa el header X-Demo-Status con uno de: 200,201,204,400,401,403,404,409,422,500" })
        };
    }

    /// <summary>
    /// GET /api/users-xml — Demuestra content negotiation con XML.
    /// Si el cliente envía Accept: application/xml, recibe XML en vez de JSON.
    /// </summary>
    [HttpGet("users-xml")]
    public IActionResult GetUsersAsXml()
    {
        // Con .AddXmlSerializerFormatters(), el framework maneja XML automáticamente.
        // Pruébalo con: curl -H "Accept: application/xml" http://localhost:5000/api/users-xml
        return Ok(_users.Select(u => new UserResponse(u.Id, u.Name, u.Email, u.Age, u.CreatedAt)));
    }

    /// <summary>
    /// GET /api/error — Simula un error 500 para probar el manejo de excepciones.
    /// </summary>
    [HttpGet("error")]
    public IActionResult SimulateError()
    {
        // En producción, esto sería capturado por el middleware de excepciones (Lección 04)
        throw new InvalidOperationException("Error simulado para testing");
    }
}

// ─────────────────────────────────────────────────────────────
// RESUMEN DE STATUS CODES MÁS USADOS:
//
// 2xx — Éxito
//   200 OK              → GET, PUT exitoso
//   201 Created         → POST exitoso (recurso creado)
//   204 No Content      → DELETE exitoso, o PUT sin datos de vuelta
//
// 4xx — Error del cliente
//   400 Bad Request     → JSON malformado, dato inválido
//   401 Unauthorized    → Token JWT ausente o inválido
//   403 Forbidden       → Token válido pero sin permisos
//   404 Not Found       → ID no existe
//   409 Conflict        → Violación de unicidad
//   422 Unprocessable   → Validación de negocio fallida
//
// 5xx — Error del servidor
//   500 Internal Server → Error no controlado
// ─────────────────────────────────────────────────────────────
