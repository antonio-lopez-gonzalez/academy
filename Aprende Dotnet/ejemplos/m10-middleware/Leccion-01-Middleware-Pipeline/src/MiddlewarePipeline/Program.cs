// MODULO 10 - LECCION 01: Middleware Pipeline
using System.Diagnostics;

// ---- APP ----
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<RequestTimingMiddleware>();
WebApplication app = builder.Build();

app.UseExceptionHandler("/error");

app.Map("/health", healthApp =>
{
    healthApp.Run(async ctx =>
    {
        ctx.Response.ContentType = "text/plain";
        await ctx.Response.WriteAsync("OK - Healthy");
    });
});

app.Map("/info", infoApp =>
{
    infoApp.Use(async (ctx, next) =>
    {
        Console.WriteLine("[INFO] dentro de rama /info");
        await next();
    });
    infoApp.Run(async ctx =>
    {
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsync("""{"app":"MiddlewarePipeline","version":"1.0.0"}""");
    });
});

app.UseWhen(
    ctx => ctx.Request.Path.StartsWithSegments("/api"),
    branch =>
    {
        branch.Use(async (ctx, next) =>
        {
            Console.WriteLine($"[API] -> {ctx.Request.Method} {ctx.Request.Path}");
            await next();
        });
    });

app.Use(async (ctx, next) =>
{
    Console.WriteLine($"[ANTES] -> {ctx.Request.Method} {ctx.Request.Path}");
    await next();
    Console.WriteLine($"[DESPUES] <- {ctx.Response.StatusCode}");
});

app.UseRequestTiming();

app.UseWhen(
    ctx => ctx.Request.Path.StartsWithSegments("/blocked"),
    branch =>
    {
        branch.Run(async ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync("""{"error":"Endpoint bloqueado por middleware"}""");
        });
    });

app.MapGet("/", () => "Demo de Middleware Pipeline. Prueba /api/test, /health, /info, /blocked, /error-test");
app.MapGet("/api/test", () => Results.Json(new { message = "Endpoint de prueba", timestamp = DateTime.UtcNow }));
app.MapGet("/api/users/{id}", (int id) => Results.Json(new { id, name = $"Usuario {id}", email = $"user{id}@ejemplo.com" }));
app.MapGet("/error-test", () => { throw new InvalidOperationException("Error de prueba"); });
app.MapGet("/error", () => Results.Json(new { type = "https://tools.ietf.org/html/rfc9110", title = "Error interno", status = 500 }));

Console.WriteLine("MiddlewarePipeline escuchando en http://localhost:5000");
app.Run("http://localhost:5000");

// ================================================================
// TIPOS
// ================================================================

public class RequestTimingMiddleware : IMiddleware
{
    private readonly ILogger<RequestTimingMiddleware> _logger;
    public RequestTimingMiddleware(ILogger<RequestTimingMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Stopwatch sw = Stopwatch.StartNew();
        string method = context.Request.Method;
        string path = context.Request.Path;
        await next(context);
        sw.Stop();
        _logger.LogInformation("{Method} {Path} -> {StatusCode} en {ElapsedMs}ms",
            method, path, context.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}

public static class RequestTimingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder builder)
        => builder.UseMiddleware<RequestTimingMiddleware>();
}
