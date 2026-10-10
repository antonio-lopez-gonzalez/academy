
// ================================================================
// MÓDULO 10 — Middleware y Pipelines
// Lección 02 — Rate Limiting + CORS
// ================================================================

using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------------
// 1. CONFIGURACIÓN DE RATE LIMITING (.NET 8 nativo)
// ------------------------------------------------------------------

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Política 1: Ventana Fija — 3 requests cada 60 segundos
    options.AddFixedWindowLimiter(policyName: "fixed3perMinute", limiterOptions =>
    {
        limiterOptions.PermitLimit = 3;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // Política 2: Ventana Deslizante — 5 requests/minuto, más preciso
    options.AddSlidingWindowLimiter(policyName: "sliding5perMinute", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.SegmentsPerWindow = 6;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 2;
    });

    // Política 3: Concurrencia — solo 2 requests simultáneos
    options.AddConcurrencyLimiter(policyName: "concurrency2", limiterOptions =>
    {
        limiterOptions.PermitLimit = 2;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 3;
    });

    // Respuesta personalizada cuando se excede el límite
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        string retryAfter = "60";
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfterSpan))
        {
            retryAfter = retryAfterSpan.TotalSeconds.ToString("F0");
        }
        context.HttpContext.Response.Headers.RetryAfter = retryAfter;
        string reason = context.Lease.TryGetMetadata(MetadataName.ReasonPhrase, out string? phrase)
            ? phrase : "Rate limit exceeded";
        await context.HttpContext.Response.WriteAsync(
            $$"""{"error":"{{reason}}","retryAfterSeconds":{{retryAfter}}}""", cancellationToken);
    };
});

// ------------------------------------------------------------------
// 2. CONFIGURACIÓN DE CORS
// ------------------------------------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });

    options.AddPolicy("Production", policy =>
    {
        policy
            .WithOrigins("https://miapp.com", "https://admin.miapp.com")
            .WithMethods("GET", "POST", "PUT", "DELETE")
            .WithHeaders("Content-Type", "Authorization")
            .WithExposedHeaders("X-Correlation-ID")
            .AllowCredentials();
    });
});

WebApplication app = builder.Build();

// Orden correcto: Routing → CORS → RateLimiter → Endpoints
app.UseRouting();
app.UseCors("Development");
app.UseRateLimiter();

// ---- ENDPOINTS ----
app.MapGet("/fixed", () => Results.Json(new
{
    message = "Endpoint con FixedWindowLimiter (3 req/min)",
    timestamp = DateTime.UtcNow,
    limit = "3 requests por minuto, sin cola"
})).RequireRateLimiting("fixed3perMinute");

app.MapGet("/sliding", () => Results.Json(new
{
    message = "Endpoint con SlidingWindowLimiter (5 req/min)",
    timestamp = DateTime.UtcNow,
    limit = "5 requests por minuto, hasta 2 en cola"
})).RequireRateLimiting("sliding5perMinute");

app.MapGet("/heavy", async () =>
{
    await Task.Delay(3000); // Simula proceso pesado
    return Results.Json(new
    {
        message = "Operación pesada completada",
        timestamp = DateTime.UtcNow,
        limit = "Solo 2 requests simultáneos, 3 en cola"
    });
}).RequireRateLimiting("concurrency2");

app.MapGet("/public", () => Results.Json(new
{
    message = "Endpoint público sin rate limiting",
    timestamp = DateTime.UtcNow
}));

app.MapGet("/cors-test", (HttpContext context) => Results.Json(new
{
    message = "Si ves esto desde otro dominio, CORS funciona",
    origin = context.Request.Headers.Origin.ToString()
}));

app.MapGet("/", () => Results.Json(new
{
    app = "MiddlewareRateLimit",
    endpoints = new[] { "/fixed", "/sliding", "/heavy", "/public", "/cors-test" }
}));

Console.WriteLine("MiddlewareRateLimit escuchando en http://localhost:5000");
app.Run("http://localhost:5000");
