// ─── MÓDULO 18 ─── Seguridad OWASP Top 10 para APIs ───
// OWASP: Open Web Application Security Project
// Top 10: las 10 vulnerabilidades más críticas en APIs
// EF Core previene SQL Injection con parámetros automáticamente

using Microsoft.AspNetCore.Authorization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ─── Registrar servicios de autorización ───
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("EditorOrAbove", policy => policy.RequireRole("Admin", "Editor"));

WebApplication app = builder.Build();

// ──────────────────────────────────────────────────────────────────────────────
// A08 — SECURITY MISCONFIGURATION: Headers HTTP de seguridad
// ──────────────────────────────────────────────────────────────────────────────
app.Use(async (context, next) =>
{
    // Evitar que el navegador "adivine" el content-type (MIME sniffing)
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";

    // Evitar clickjacking: no permitir que la página se cargue en un iframe
    context.Response.Headers["X-Frame-Options"] = "DENY";

    // Content Security Policy: controlar qué recursos puede cargar el navegador
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:;";

    // HTTP Strict Transport Security: forzar HTTPS
    context.Response.Headers["Strict-Transport-Security"] =
        "max-age=31536000; includeSubDomains; preload";

    // Controlar qué información del referrer se envía al navegar
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

    await next();
});

// ──────────────────────────────────────────────────────────────────────────────
// A01 + A05 — AUTHORIZATION: Verificar propiedad del recurso + Roles
// ──────────────────────────────────────────────────────────────────────────────

// ─── BUEN EJEMPLO: Verificar que el usuario es dueño del recurso ───
// A01 - Broken Object Level Authorization (BOLA):
//   Un usuario autenticado no debe poder acceder a recursos de otros usuarios
//   solo cambiando el ID en la URL (ej: /posts/1 → /posts/2).
app.MapGet("/api/posts/{postId:int}", (int postId, HttpContext http) =>
{
    // En producción, el userId viene del token JWT
    string currentUserId = http.Request.Headers["X-User-Id"].FirstOrDefault() ?? "anon";

    Console.WriteLine($"[A01] Usuario {currentUserId} solicita post {postId}");

    // Simular verificación de propiedad
    // BAD:  return GetPost(postId); // ← Cualquiera puede ver cualquier post
    // GOOD: Verificar que el post pertenece al usuario actual
    if (!IsOwnerOfPost(postId, currentUserId))
    {
        return Results.Forbid(); // 403 - El usuario no es dueño de este recurso
    }

    return Results.Ok(new { Id = postId, Title = "Mi Post", OwnerId = currentUserId });
});

// ─── BUEN EJEMPLO: [Authorize] con rol ───
// A05 - Broken Function Level Authorization:
//   Solo los administradores deben poder acceder a endpoints administrativos.
app.MapGet("/api/admin/users", [Authorize(Roles = "Admin")] () =>
{
    // Este endpoint SOLO es accesible para usuarios con rol "Admin"
    return Results.Ok(new[] { new { Id = 1, Name = "Admin User" } });
});

// ─── MAL EJEMPLO (comentado): Endpoint sin protección ───
app.MapGet("/api/admin/users-dangerous", () =>
{
    // ⚠️ BAD: Este endpoint no tiene [Authorize]. Cualquiera puede acceder.
    // Debería tener [Authorize(Roles = "Admin")]
    return Results.Ok(new[] { new { Id = 1, Name = "Admin User", Password = "secret123" } });
});

// ──────────────────────────────────────────────────────────────────────────────
// A03 — EXPOSICIÓN DE DATOS: Devolver DTOs, nunca entidades de dominio
// ──────────────────────────────────────────────────────────────────────────────
app.MapGet("/api/users/{id:int}", (int id) =>
{
    // BAD:  return _context.Users.Find(id); // ← devuelve PasswordHash, Email interno, etc.
    // GOOD: Mapear a DTO que solo contenga datos seguros
    return Results.Ok(new UserDto(id, "usuario@email.com", "Usuario Público"));
});

// ──────────────────────────────────────────────────────────────────────────────
// A04 — UNRESTRICTED RESOURCE CONSUMPTION: Paginación + Rate Limiting
// ──────────────────────────────────────────────────────────────────────────────
app.MapGet("/api/posts", (int page = 1, int pageSize = 10) =>
{
    // Limitar el tamaño máximo de página para evitar abuso
    // BAD:  pageSize sin límite → el cliente puede pedir pageSize=999999
    const int maxPageSize = 100;
    pageSize = Math.Min(pageSize, maxPageSize);

    // Siempre devolver metadatos de paginación
    return Results.Ok(new
    {
        Page = page,
        PageSize = pageSize,
        TotalCount = 150,
        Items = Enumerable.Range(1, pageSize).Select(i => $"Post {i}")
    });
});

// ──────────────────────────────────────────────────────────────────────────────
// SQL INJECTION: Cómo EF Core previene y cómo se puede romper
// ──────────────────────────────────────────────────────────────────────────────
app.MapGet("/api/search/bad", (string query) =>
{
    // ⚠️ BAD: Nunca concatenar strings en SQL. Esto permite SQL Injection.
    // Si query = "'; DROP TABLE Posts; --" → eliminación de la tabla Posts
    //
    // string sql = $"SELECT * FROM Posts WHERE Title LIKE '%{query}%'";
    // var posts = context.Posts.FromSqlRaw(sql).ToList(); // ← ¡PELIGRO!

    Console.WriteLine($"[BAD] Peligro de SQL Injection con query: '{query}'");
    return Results.Ok(new { Warning = "Esta query concatena strings → vulnerable a SQL Injection" });
});

app.MapGet("/api/search/good", (string query) =>
{
    // ✅ GOOD: Usar parámetros. EF Core genera SQL parametrizado automáticamente.
    //
    // var posts = context.Posts
    //     .Where(p => EF.Functions.Like(p.Title, $"%{query}%"))
    //     .ToList();

    // Si necesitas SQL crudo, usa FromSqlInterpolated (C# string interpolation):
    // var posts = context.Posts
    //     .FromSqlInterpolated($"SELECT * FROM Posts WHERE Title LIKE {'%' + query + '%'}")
    //     .ToList();
    // FromSqlInterpolated convierte la interpolación a parámetros SQL seguros.

    Console.WriteLine($"[GOOD] SQL parametrizado con query: '{query}'");
    return Results.Ok(new { Message = "Búsqueda segura con SQL parametrizado", Query = query });
});

// ──────────────────────────────────────────────────────────────────────────────
// XSS en APIs: No es relevante porque las APIs devuelven JSON
// ──────────────────────────────────────────────────────────────────────────────
app.MapGet("/api/xss-info", () =>
{
    return Results.Ok(new
    {
        Message = "XSS no afecta a las APIs REST porque el Content-Type es application/json",
        Detail = "Los navegadores NO ejecutan scripts dentro de respuestas JSON.",
        Warning = "Pero SIEMPRE valida y sanitiza el input del usuario en el servidor.",
        BestPractice = "Usa ASP.NET Core Model Validation ([Required], [MaxLength], [RegularExpression])"
    });
});

// ──────────────────────────────────────────────────────────────────────────────
// CSRF en APIs JWT: No es relevante porque JWT es stateless
// ──────────────────────────────────────────────────────────────────────────────
app.MapGet("/api/csrf-info", () =>
{
    return Results.Ok(new
    {
        Message = "CSRF no afecta a APIs JWT (stateless)",
        Reason = "No hay cookies de sesión que el navegador envíe automáticamente.",
        Detail = "El token JWT se envía manualmente en el header Authorization: Bearer xxx",
        Note = "Si usas cookies de autenticación, entonces SÍ necesitas anti-CSRF (AddAntiforgery)"
    });
});

// ──────────────────────────────────────────────────────────────────────────────
// ENDPOINTS DE DEMOSTRACIÓN
// ──────────────────────────────────────────────────────────────────────────────

// Endpoint que simula verificación de propiedad
app.MapGet("/api/my-posts", (HttpContext http) =>
{
    string currentUserId = http.Request.Headers["X-User-Id"].FirstOrDefault() ?? "anon";

    // ✅ GOOD: Filtrar siempre por el usuario actual, nunca devolver todos
    return Results.Ok(new[]
    {
        new { Id = 1, Title = "Post del usuario", OwnerId = currentUserId }
    });
});

// ──────────────────────────────────────────────────────────────────────────────
// RESUMEN OWASP TOP 10 EN COMENTARIO FINAL
// ──────────────────────────────────────────────────────────────────────────────
app.MapGet("/", () => Results.Content(
    """
    <!DOCTYPE html>
    <html lang="es">
    <head>
        <meta charset="UTF-8">
        <title>OWASP Top 10 - Módulo 18</title>
        <style>
            body{font-family:monospace;max-width:900px;margin:2rem auto;background:#111;color:#0f0}
            table{border-collapse:collapse;width:100%;margin:1rem 0}
            th,td{border:1px solid #0f0;padding:.5rem;text-align:left}
            th{background:#222}
            .bad{color:#f44}.good{color:#0f0}
            a{color:#0ff}
        </style>
    </head>
    <body>
        <h1>OWASP Top 10 para APIs (.NET 8)</h1>
        <p>Endpoints disponibles para probar:</p>
        <ul>
            <li><a href="/api/posts/1" target="_blank">GET /api/posts/{id}</a> — A01: Verificación de propiedad</li>
            <li><a href="/api/posts?page=1&pageSize=10" target="_blank">GET /api/posts?page=1</a> — A04: Paginación</li>
            <li><a href="/api/search/bad?query=test" target="_blank">GET /api/search/bad</a> — SQL Injection MAL</li>
            <li><a href="/api/search/good?query=test" target="_blank">GET /api/search/good</a> — SQL Injection BIEN</li>
            <li><a href="/api/xss-info" target="_blank">GET /api/xss-info</a> — XSS en APIs</li>
            <li><a href="/api/csrf-info" target="_blank">GET /api/csrf-info</a> — CSRF en APIs JWT</li>
            <li><a href="/api/users/1" target="_blank">GET /api/users/{id}</a> — A03: DTOs (no entidades)</li>
        </ul>
        <table>
            <tr><th>#</th><th>Vulnerabilidad</th><th>Solución en .NET</th></tr>
            <tr><td>A01</td><td>Broken Object Level Auth</td><td class="good">Verificar propiedad del recurso (userId del JWT)</td></tr>
            <tr><td>A02</td><td>Broken Authentication</td><td class="good">JWT con expiración, refresh tokens, BCrypt</td></tr>
            <tr><td>A03</td><td>Exposición de datos</td><td class="good">DTOs, nunca devolver entidades de dominio</td></tr>
            <tr><td>A04</td><td>Consumo sin restricción</td><td class="good">Rate limiting, paginación con max page size</td></tr>
            <tr><td>A05</td><td>Broken Function Level Auth</td><td class="good">[Authorize(Roles="Admin")] en endpoints</td></tr>
            <tr><td>A06</td><td>Flujos de negocio sensibles</td><td class="good">Rate limiting, CAPTCHA, anti-automation</td></tr>
            <tr><td>A07</td><td>SSRF</td><td class="good">No permitir URLs arbitrarias del cliente</td></tr>
            <tr><td>A08</td><td>Security Misconfiguration</td><td class="good">Headers HTTP, CORS restrictivo, HTTPS</td></tr>
            <tr><td>A09</td><td>Inventory Management</td><td class="good">Versionado de API, deprecación documentada</td></tr>
            <tr><td>A10</td><td>Consumo inseguro de APIs</td><td class="good">Validar todo input, nunca confiar en el cliente</td></tr>
        </table>
        <h3>SQL Injection: EF Core previene automáticamente</h3>
        <p class="bad">BAD: FromSqlRaw con string concatenado → vulnerable</p>
        <p class="good">GOOD: FromSqlInterpolated o LINQ → parametrizado y seguro</p>
        <h3>XSS en APIs</h3>
        <p>No afecta porque JSON no ejecuta JavaScript. Pero valida input siempre.</p>
        <h3>CSRF en APIs JWT</h3>
        <p>No afecta porque JWT es stateless (no usa cookies de sesión).</p>
    </body>
    </html>
    """, "text/html"));

app.Run();

// ─── Helpers ───

// Simula verificación de propiedad en BD
static bool IsOwnerOfPost(int postId, string userId)
{
    // En producción: SELECT COUNT(1) FROM Posts WHERE Id = @postId AND OwnerId = @userId
    return userId != "anon"; // Simulación: solo usuarios autenticados
}

// DTO que no expone campos sensibles
public record UserDto(int Id, string Email, string DisplayName);
// BAD: si devolviéramos la entidad User, expondríamos PasswordHash, ResetToken, etc.
