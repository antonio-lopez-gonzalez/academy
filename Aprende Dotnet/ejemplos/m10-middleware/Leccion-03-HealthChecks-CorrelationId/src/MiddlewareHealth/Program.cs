// MODULO 10 - LECCION 03: Health Checks + Correlation ID + Request Logging
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

// ---- APP ----
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API corriendo"), tags: new[] { "api", "basic" })
    .AddCheck("database", () =>
    {
        if (Random.Shared.Next(0, 10) > 1)
            return HealthCheckResult.Healthy("PostgreSQL responde en 15ms");
        return HealthCheckResult.Unhealthy("PostgreSQL no responde",
            exception: new TimeoutException("Connection timeout"));
    }, tags: new[] { "database", "critical" })
    .AddCheck("redis_cache", () =>
    {
        int latency = Random.Shared.Next(50, 500);
        var data = new Dictionary<string, object> { ["latency_ms"] = latency };
        if (latency > 300)
            return HealthCheckResult.Degraded($"Redis lento ({latency}ms)", data: data);
        return HealthCheckResult.Healthy($"Redis OK ({latency}ms)", data: data);
    }, tags: new[] { "cache" })
    .AddCheck("disk_space", () =>
    {
        DriveInfo drive = new(Path.GetPathRoot(Environment.CurrentDirectory) ?? "C:\\");
        double freeGb = drive.AvailableFreeSpace / 1_073_741_824.0;
        var data = new Dictionary<string, object>
        {
            ["free_gb"] = Math.Round(freeGb, 2),
            ["total_gb"] = Math.Round(drive.TotalSize / 1_073_741_824.0, 2)
        };
        if (freeGb < 1.0) return HealthCheckResult.Unhealthy($"Critico: {freeGb:F1}GB", data: data);
        if (freeGb < 5.0) return HealthCheckResult.Degraded($"Bajo: {freeGb:F1}GB", data: data);
        return HealthCheckResult.Healthy($"OK: {freeGb:F1}GB libres", data: data);
    }, tags: new[] { "infrastructure" });

builder.Services.AddTransient<CorrelationIdMiddleware>();
builder.Services.AddTransient<RequestLoggingMiddleware>();

WebApplication app = builder.Build();

app.UseExceptionHandler("/error");
app.UseCorrelationId();
app.UseRequestLogging();

app.MapGet("/", () => Results.Json(new
{
    app = "MiddlewareHealth",
    health = "/health",
    ready = "/health/ready",
    live = "/health/live",
    info = "/info"
}));

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var json = new System.Text.Json.Nodes.JsonObject
        {
            ["status"] = report.Status.ToString(),
            ["duration_ms"] = report.TotalDuration.TotalMilliseconds.ToString("F2"),
            ["timestamp"] = DateTime.UtcNow.ToString("O"),
            ["correlation_id"] = context.TraceIdentifier,
            ["checks"] = new System.Text.Json.Nodes.JsonArray(
                report.Entries.Select(e =>
                {
                    var entry = new System.Text.Json.Nodes.JsonObject
                    {
                        ["name"] = e.Key,
                        ["status"] = e.Value.Status.ToString(),
                        ["description"] = e.Value.Description ?? "",
                        ["duration_ms"] = e.Value.Duration.TotalMilliseconds.ToString("F2")
                    };
                    if (e.Value.Data.Count > 0)
                    {
                        var data = new System.Text.Json.Nodes.JsonObject();
                        foreach ((string k, object? v) in e.Value.Data)
                            data[k] = v?.ToString();
                        entry["data"] = data;
                    }
                    if (e.Value.Exception is not null)
                        entry["exception"] = e.Value.Exception.Message;
                    return (System.Text.Json.Nodes.JsonNode?)entry;
                }).ToArray()
            )
        };
        await context.Response.WriteAsync(json.ToJsonString(
            new JsonSerializerOptions { WriteIndented = true }));
    }
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = c => c.Tags.Contains("critical") || c.Tags.Contains("database"),
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "text/plain";
        await ctx.Response.WriteAsync(report.Status == HealthStatus.Healthy ? "Ready\n" : "Not Ready\n");
    }
});

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = c => c.Tags.Contains("basic"),
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "text/plain";
        await ctx.Response.WriteAsync(report.Status == HealthStatus.Healthy ? "Alive\n" : "Dead\n");
    }
});

app.MapGet("/info", (HttpContext ctx) => Results.Json(new
{
    correlation_id = ctx.TraceIdentifier,
    timestamp = DateTime.UtcNow,
    remote_ip = ctx.Connection.RemoteIpAddress?.ToString()
}));

app.MapGet("/simulate-error", () => { throw new InvalidOperationException("Error simulado para probar logging de 500"); });

app.MapGet("/simulate-notfound", (HttpContext ctx) =>
{
    ctx.Response.StatusCode = StatusCodes.Status404NotFound;
    ctx.Response.ContentType = "application/json";
    return ctx.Response.WriteAsync("""{"error":"Recurso no encontrado"}""");
});

app.MapGet("/simulate-badrequest", (HttpContext ctx) =>
{
    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
    ctx.Response.ContentType = "application/json";
    return ctx.Response.WriteAsync("""{"error":"Request invalida"}""");
});

app.MapGet("/error", (HttpContext ctx) =>
{
    ctx.Response.ContentType = "application/json";
    return ctx.Response.WriteAsync($$"""{"error":"Error interno","correlationId":"{{ctx.TraceIdentifier}}"}""");
});

Console.WriteLine("MiddlewareHealth escuchando en http://localhost:5000");
app.Run("http://localhost:5000");

// ================================================================
// TIPOS
// ================================================================

public class CorrelationIdMiddleware : IMiddleware
{
    public const string Header = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Headers.TryGetValue(Header, out var existing)
            && !string.IsNullOrWhiteSpace(existing.ToString()))
        {
            context.TraceIdentifier = existing.ToString();
        }
        else
        {
            string id = Guid.NewGuid().ToString("N");
            context.TraceIdentifier = id;
            context.Request.Headers[Header] = id;
        }

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[Header] = context.TraceIdentifier;
            return Task.CompletedTask;
        });

        Activity.Current?.SetTag("correlation_id", context.TraceIdentifier);
        await next(context);
    }
}

public class RequestLoggingMiddleware : IMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string cid = context.TraceIdentifier;
        string method = context.Request.Method;
        string path = context.Request.Path;
        string qs = context.Request.QueryString.HasValue ? context.Request.QueryString.Value! : "";
        string ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        _logger.LogInformation("[REQ] {Method} {Path}{QS} | IP={IP} | CID={CID}", method, path, qs, ip, cid);

        Stopwatch sw = Stopwatch.StartNew();
        await next(context);
        sw.Stop();

        int status = context.Response.StatusCode;
        long ms = sw.ElapsedMilliseconds;

        if (status >= 500)
            _logger.LogError("[RES] {Method} {Path} -> {Status} en {Ms}ms | CID={CID}", method, path, status, ms, cid);
        else if (status >= 400)
            _logger.LogWarning("[RES] {Method} {Path} -> {Status} en {Ms}ms | CID={CID}", method, path, status, ms, cid);
        else
            _logger.LogInformation("[RES] {Method} {Path} -> {Status} en {Ms}ms | CID={CID}", method, path, status, ms, cid);
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        => builder.UseMiddleware<CorrelationIdMiddleware>();

    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        => builder.UseMiddleware<RequestLoggingMiddleware>();
}
