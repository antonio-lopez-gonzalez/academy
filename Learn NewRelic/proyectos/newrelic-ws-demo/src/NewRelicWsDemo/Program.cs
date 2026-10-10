// ============================================================================
// Punto de entrada de la app de ejemplo: servidor WebSocket + Minimal API.
//
// Instrumentación con New Relic (API NewRelic.Agent.Api):
//   - Endpoints de la API  → instrumentados por el agente automáticamente.
//   - Mensajes del WebSocket → instrumentación custom ([Transaction]/[Trace]).
//   - Métricas custom, custom events, logs en contexto y propagación W3C.
// ----------------------------------------------------------------------------
// EJECUTAR (con New Relic APM — perfil por defecto en launchSettings):
//   dotnet run --project src/NewRelicWsDemo
//
// Requisitos ya cableados en el proyecto:
//   - Paquete NewRelic.Agent (profiler en bin/.../newrelic/)
//   - CORECLR_* vía RunEnvironmentVariables del .csproj (dotnet run / VS)
//   - Licencia / app / host EU: launchSettings.json + newrelic.config
//   - Espejo en appsettings.json para /agent y lectura vía IConfiguration
// ============================================================================

using NewRelicWsDemo;
using NewRelicWsDemo.Handlers;
using NewRelicWsDemo.Messages;
using NewRelicWsDemo.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// El agente (profiler) se carga ANTES de Main y lee NEW_RELIC_* / newrelic.config.
// Si faltan en el entorno pero están en appsettings (p. ej. tests), las publicamos
// para que el resto de la app y herramientas las vean igual.
PublishNewRelicSettingsToEnvironment(builder.Configuration);

// Servicios de negocio vía inyección de dependencias estándar
builder.Services.AddSingleton<PriceSimulator>();
builder.Services.AddSingleton<TradeProcessor>();
builder.Services.AddSingleton<PingProcessor>();
builder.Services.AddSingleton<MessageDispatcher>();

WebApplication app = builder.Build();

// ----------------------------------------------------------------------------
// Minimal API: el agente (si está adjunto) captura estas como WebTransactions.
// ----------------------------------------------------------------------------
app.MapGet("/api/status", () =>
{
    // Contexto en la transacción: versión desplegada (M2-04)
    NewRelic.Api.Agent.NewRelic.GetAgent().CurrentTransaction
        .AddCustomAttribute("version",
            app.Configuration["APP_VERSION"] ?? "dev");

    return Results.Ok(new { status = "ok", utc = DateTimeOffset.UtcNow });
});

app.MapPost("/api/broadcast", (BroadcastRequest req) =>
{
    // Nombre estable de dominio + atributo de audiencia (M2-01)
    NewRelic.Api.Agent.NewRelic.SetTransactionName("WebAPI", "Broadcast");
    NewRelic.Api.Agent.NewRelic.GetAgent().CurrentTransaction
        .AddCustomAttribute("audience", req.Audience);

    var sw = System.Diagnostics.Stopwatch.StartNew();
    // En un caso real aquí enviarías el mensaje a los clientes del WS.
    System.Threading.Thread.Sleep(req.SimulateMs);
    sw.Stop();

    // Métrica de "broadcast" (M2-02)
    NewRelic.Api.Agent.NewRelic.RecordResponseTimeMetric("Custom/BroadcastMs", sw.ElapsedMilliseconds);

    return Results.Ok(new { sent = req.Audience, latencyMs = sw.ElapsedMilliseconds });
});

// Health check: lo ignores a propósito para no contaminar datasets (M2-01)
app.MapGet("/healthz", () =>
{
    NewRelic.Api.Agent.NewRelic.IgnoreTransaction();
    return Results.Ok("healthy");
});

// Diagnóstico de configuración: los requisitos para que el agente se adjunte.
// Nota: NewRelic.Api.Agent.GetAgent() NO es null sin profiler (devuelve el
// placeholder no-op), así que "attached" se deduce de las variables/paquete.
app.MapGet("/agent", () =>
{
    var profilingEnabled =
        Environment.GetEnvironmentVariable("CORECLR_ENABLE_PROFILING") == "1";
    var profilerHome =
        Environment.GetEnvironmentVariable("CORECLR_NEWRELIC_HOME");

    return Results.Ok(new
    {
        licenseKeyConfigured = !string.IsNullOrWhiteSpace(
            app.Configuration["NEW_RELIC_LICENSE_KEY"]),
        appNameConfigured = !string.IsNullOrWhiteSpace(
            app.Configuration["NEW_RELIC_APP_NAME"]),
        host = app.Configuration["NEW_RELIC_HOST"],
        profilingEnabled,
        profilerHome,
        hint = profilingEnabled
            ? "Profiler activo (CORECLR_ENABLE_PROFILING=1). Revisa la consola del agente " +
              "('Connected' / 'agentEnabled') y APM → NotificadorWebSocket en ~1-2 min."
            : "Profiler NO adjunto. Usa `dotnet run` (perfil NewRelicWsDemo) para cargar " +
              "CORECLR_* + NEW_RELIC_* desde el .csproj / launchSettings."
    });
});

// ----------------------------------------------------------------------------
// WebSocket: el upgrade lo captura el agente; los mensajes se instrumentan (M4).
// ----------------------------------------------------------------------------
app.UseWebSockets();

app.Map("/ws", async (HttpContext ctx, MessageDispatcher dispatcher) =>
{
    if (!ctx.WebSockets.IsWebSocketRequest) return Results.BadRequest();

    using var socket = await ctx.WebSockets.AcceptWebSocketAsync();
    await WsServer.HandleAsync(socket, dispatcher);
    return Results.Empty;
});

app.Run();

/// <summary>
/// Copia claves NEW_RELIC_* (y APP_VERSION) de IConfiguration al entorno del proceso
/// solo si aún no están definidas. No sustituye al profiler: CORECLR_* deben existir
/// antes de arrancar (launchSettings / RunEnvironmentVariables).
/// </summary>
static void PublishNewRelicSettingsToEnvironment(IConfiguration config)
{
    string[] keys =
    [
        "NEW_RELIC_LICENSE_KEY",
        "NEW_RELIC_APP_NAME",
        "NEW_RELIC_HOST",
        "APP_VERSION",
    ];

    foreach (var key in keys)
    {
        var value = config[key];
        if (string.IsNullOrWhiteSpace(value)) continue;
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key))) continue;
        Environment.SetEnvironmentVariable(key, value);
    }
}
