using Serilog; // Espacio de nombres principal de Serilog

// ─────────────────────────────────────────────────────────────
// Lección 05 — Logging con ILogger<T> + Serilog
// Logging estructurado nativo de .NET + Serilog para sinks
// avanzados (consola, archivo, enrichment).
// ─────────────────────────────────────────────────────────────

// ═══════════════════════════════════════════════════════════════
// 1. CONFIGURAR SERILOG (antes de construir el builder)
// ═══════════════════════════════════════════════════════════════
// Serilog se configura ANTES de crear el builder para capturar
// también los logs de arranque de la aplicación.

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Nivel mínimo: Debug (en prod sería Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning) // Reducir ruido de framework
    .Enrich.FromLogContext()      // Permite añadir propiedades con LogContext.PushProperty()
    .Enrich.WithProperty("Application", "CMS-API") // Añade propiedad fija a todos los logs
    .WriteTo.Console(             // Sink: consola con formato legible
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
    )
    .WriteTo.File(                // Sink: archivo de logs
        path: "logs/cms-api-.log",       // Rotación diaria automática
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,       // Mantener solo 7 días
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
    )
    .CreateLogger();

try
{
    // ═══════════════════════════════════════════════════════════
    // 2. INTEGRAR SERILOG CON ASP.NET CORE
    // ═══════════════════════════════════════════════════════════

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Reemplaza el sistema de logging nativo por Serilog.
    // A partir de aquí, todos los ILogger<T> usan Serilog por debajo.
    builder.Host.UseSerilog();

    WebApplication app = builder.Build();
    app.Urls.Add("http://localhost:5000");

    // ── Middleware que loguea todas las requests automáticamente ──
    // Añade: método HTTP, path, status code, duración
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondió {StatusCode} en {Elapsed:0.0000} ms";
    });

    // ═══════════════════════════════════════════════════════════
    // 3. ENDPOINTS DE DEMOSTRACIÓN
    // ═══════════════════════════════════════════════════════════

    // Datos en memoria
    List<Post> posts = new()
    {
        new Post(1, "Primer post", "Contenido inicial", "publicado"),
        new Post(2, "Segundo post", "Más contenido", "borrador"),
    };

    // ── GET /api/posts — Demuestra logging estructurado ──────
    app.MapGet("/api/posts", (ILogger<Program> logger) =>
    {
        // Logging estructurado: usa plantillas, NO interpolación de strings.
        // La plantilla conserva los nombres de los parámetros → búsqueda por campo en Serilog.
        logger.LogInformation("Obteniendo {Count} posts", posts.Count);

        // Ejemplo de diferentes niveles de log:
        logger.LogTrace("Esto solo se ve en Trace (muy detallado)");   // Normalmente no se ve
        logger.LogDebug("Procesando lista de posts");                   // Debug
        logger.LogInformation("Posts listados correctamente");          // Info (operación normal)
        // logger.LogWarning("...");  // Algo inesperado pero no crítico
        // logger.LogError("...");    // Error que requiere atención
        // logger.LogCritical("..."); // Fallo catastrófico

        return Results.Ok(posts);
    });

    // ── GET /api/posts/{id} — Demuestra log por ID ───────────
    app.MapGet("/api/posts/{id:int}", (int id, ILogger<Program> logger) =>
    {
        Post? post = posts.Find(p => p.Id == id);

        if (post is null)
        {
            // Warning porque no es culpa del servidor, pero es anómalo
            logger.LogWarning("Post con ID {PostId} no encontrado", id);
            return Results.NotFound(new { Message = $"Post {id} no encontrado" });
        }

        logger.LogInformation("Post {PostId} encontrado: '{Title}'", post.Id, post.Title);
        return Results.Ok(post);
    });

    // ── POST /api/posts — Demuestra logging con scope ────────
    app.MapPost("/api/posts", (Post newPost, ILogger<Program> logger) =>
    {
        // ── Logging con scope ─────────────────────────────────
        // BeginScope añade propiedades a TODOS los logs dentro del bloque.
        // Muy útil para correlacionar logs de una misma operación.
        using (logger.BeginScope(new
        {
            Operation = "CreatePost",
            PostTitle = newPost.Title,
            Timestamp = DateTime.UtcNow
        }))
        {
            logger.LogInformation("Iniciando creación de post '{Title}'", newPost.Title);

            int newId = posts.Max(p => p.Id) + 1;
            Post post = newPost with { Id = newId };
            posts.Add(post);

            logger.LogInformation(
                "Post creado exitosamente: ID={PostId}, Título='{Title}', Status={Status}",
                post.Id, post.Title, post.Status);

            return Results.Created($"/api/posts/{post.Id}", post);
        }
        // Al salir del using, el scope se cierra automáticamente.
    });

    // ── DELETE /api/posts/{id} — Demuestra log de error ─────
    app.MapDelete("/api/posts/{id:int}", (int id, ILogger<Program> logger) =>
    {
        Post? post = posts.Find(p => p.Id == id);

        if (post is null)
        {
            logger.LogError(
                "Intento de eliminar post inexistente: ID={PostId}. " +
                "Esto podría indicar un bug en el frontend o un ataque.", id);
            return Results.NotFound(new { Message = $"Post {id} no encontrado" });
        }

        posts.Remove(post);
        logger.LogInformation("Post {PostId} '{Title}' eliminado por el usuario", id, post.Title);
        return Results.NoContent();
    });

    // ── GET /api/log-levels — Demuestra todos los niveles ────
    app.MapGet("/api/log-levels", (ILogger<Program> logger) =>
    {
        logger.LogTrace("TRACE: Información muy detallada para debugging");
        logger.LogDebug("DEBUG: Información útil durante desarrollo");
        logger.LogInformation("INFORMATION: Operación normal del sistema");
        logger.LogWarning("WARNING: Algo inesperado, pero la app sigue funcionando");
        logger.LogError("ERROR: Fallo en una operación, requiere investigación");
        logger.LogCritical("CRITICAL: Fallo catastrófico, la app puede caerse");

        return Results.Ok(new
        {
            Message = "Revisa la consola y el archivo logs/cms-api-*.log para ver todos los niveles",
            Levels = new[] { "Trace", "Debug", "Information", "Warning", "Error", "Critical" }
        });
    });

    // ── GET /api/error — Demuestra log de excepción ─────────
    app.MapGet("/api/error", (ILogger<Program> logger) =>
    {
        try
        {
            throw new InvalidOperationException("Error simulado para demostrar logging de excepciones");
        }
        catch (Exception ex)
        {
            // El método LogError con excepción captura el stack trace completo.
            // NUNCA usar string interpolation para el mensaje base — usar plantillas.
            logger.LogError(ex, "Error al procesar la solicitud en {Endpoint}", "/api/error");
            return Results.Problem(
                detail: "Error interno simulado. Revisa los logs para ver el stack trace.",
                statusCode: 500);
        }
    });

    app.Run();
}
catch (Exception ex)
{
    // Si la app falla al arrancar, Serilog captura el error
    Log.Fatal(ex, "La aplicación terminó inesperadamente");
}
finally
{
    // IMPORTANTE: Vaciar los buffers de Serilog antes de salir.
    // Si no se llama, se pueden perder logs pendientes de escritura.
    Log.CloseAndFlush();
}

// ─────────────────────────────────────────────────────────────
// MODELO
// ─────────────────────────────────────────────────────────────

/// <summary>Post del blog usado en los endpoints de demostración.</summary>
public record Post(int Id, string Title, string Content, string Status);

// ─────────────────────────────────────────────────────────────
// RESUMEN — Niveles de log:
//
// Trace    (0) → Todo, extremadamente detallado. Solo en desarrollo.
// Debug    (1) → Información útil para depurar.
// Information (2) → Flujo normal de la aplicación (recomendado en prod).
// Warning  (3) → Algo inesperado pero manejable.
// Error    (4) → Fallo en una operación.
// Critical (5) → Fallo catastrófico, pérdida de datos.
//
// BUENAS PRÁCTICAS:
//   ✅ _logger.LogInformation("Post {PostId} creado por {User}", id, user)
//   ❌ _logger.LogInformation($"Post {id} creado por {user}") // No usar string interpolation
//   ✅ Usar plantillas con nombres de parámetros → búsqueda por campo en Serilog/Elastic
//   ✅ Usar scopes para correlacionar logs de una misma operación
//   ❌ No loguear contraseñas, tokens JWT, ni datos sensibles
// ─────────────────────────────────────────────────────────────
