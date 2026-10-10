// ============================================================================
// Programa principal — hub de conexiones WebSocket distribuido con Redis.
//
// Endpoints:
//   GET /node              → identidad de este nodo (debug)
//   GET /health/redis      → estado de Redis (mux / ping / hitRatio)
//   GET /api/online        → nº de usuarios online (presencia)
//   WS  /ws?userId=u       → conexión WebSocket del hub
//
// Mensajes por el socket (envelope WsEnvelope):
//   {"type":"broadcast","payload":"...", "to":null}
//   {"type":"send","to":"<connId>","payload":"..."}
//
// Ejecuta con:
//   docker compose up -d
//   dotnet run --project src/RedisWsHub --urls http://localhost:5210
//   (segunda instancia: NODE_ID=node-2 dotnet run ... --urls http://localhost:5211)
// ============================================================================

using System.Net;
using System.Net.WebSockets;
using System.Text;
using RedisWsHub;
using RedisWsHub.Hub;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string redisConn = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
string nodeId = NodeId.FromEnvironment();

// 1) Multiplexer: UNO por proceso, con resiliencia (M6-01 / M6-04).
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    ConfigurationOptions opts = ConfigurationOptions.Parse(redisConn);
    opts.AbortOnConnectFail = false;
    opts.ConnectTimeout = 5000;
    opts.SyncTimeout = 5000;
    opts.AsyncTimeout = 5000;
    opts.ReconnectRetryPolicy = new ExponentialRetry(5000);
    return ConnectionMultiplexer.Connect(opts);
});

// 2) Servicios del hub.
builder.Services.AddSingleton(sp => new ConnectionRegistry(
    sp.GetRequiredService<IConnectionMultiplexer>(), nodeId));
builder.Services.AddSingleton<ConnectionHub>();
builder.Services.AddHostedService<HubRecoveryHostedService>();

WebApplication app = builder.Build();

// ----------------------------------------------------------------------------
// Diagnóstico y observabilidad.
// ----------------------------------------------------------------------------
app.MapGet("/node", () => Results.Ok(new { nodeId }));

app.MapGet("/health/redis", async (IConnectionMultiplexer mux) =>
{
    IDatabase db = mux.GetDatabase();
    TimeSpan ping;
    try
    {
        ping = await db.PingAsync();
    }
    catch (RedisException ex)
    {
        return Results.Json(new { ok = false, error = ex.Message }, statusCode: 503);
    }

    // En StackExchange.Redis 3.x, Info() viene por secciones (stats, clients…).
    var info = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
    EndPoint[] endpoints = mux.GetEndPoints();
    if (endpoints.Length > 0)
    {
        try
        {
            IServer server = mux.GetServer(endpoints[0]);
            foreach (var section in server.Info())
            foreach (var pair in section)
            {
                if ((pair.Key is "keyspace_hits" or "keyspace_misses" or "connected_clients")
                    && long.TryParse(pair.Value, out long n))
                    info[pair.Key] = n;
            }
        }
        catch (RedisException)
        {
            // Redis conectado para PING pero Info falló: devolvemos lo esencial.
        }
    }

    long hits = info.GetValueOrDefault("keyspace_hits");
    long misses = info.GetValueOrDefault("keyspace_misses");
    return Results.Ok(new
    {
        ok = true,
        pingMs = Math.Round(ping.TotalMilliseconds, 2),
        hitRatio = hits + misses == 0
            ? (double?)null
            : Math.Round(hits / (double)(hits + misses), 3),
        clients = info.GetValueOrDefault("connected_clients"),
    });
});

// ----------------------------------------------------------------------------
// API: presencia.
// ----------------------------------------------------------------------------
app.MapGet("/api/online", async (ConnectionRegistry registry) =>
{
    try
    {
        RedisValue[] online = await registry.OnlineAsync();
        return Results.Ok(new
        {
            onlineCount = online.Length,
            online = online.Select(v => v.ToString()),
        });
    }
    catch (RedisException ex)
    {
        return Results.Json(new { ok = false, error = ex.Message }, statusCode: 503);
    }
});

// ----------------------------------------------------------------------------
// WebSocket del hub.
// ----------------------------------------------------------------------------
app.UseWebSockets();

app.Map("/ws", async (HttpContext ctx, ConnectionHub hub) =>
{
    if (!ctx.WebSockets.IsWebSocketRequest) return Results.BadRequest();

    using var socket = await ctx.WebSockets.AcceptWebSocketAsync();
    string connectionId = Guid.NewGuid().ToString("N");

    // Query["userId"] es StringValues: convertido a string da null/"" si falta.
    string? userId = ctx.Request.Query["userId"];
    if (string.IsNullOrWhiteSpace(userId))
        userId = $"user-{connectionId[..6]}";

    await hub.AddAsync(socket, connectionId, userId);

    // Heartbeat periódico (M7-04): mientras el socket siga abierto, refresca
    // lease + presencia. Sin esto, una conexión idle "sana" se marcaría offline
    // o perdería su lease en ~30 s. NOTA: el stack de WebSockets responde pongs
    // automáticamente; la detección de TCP muerto se apoya en cierre/abort.
    using var heartbeatCts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);
    Task heartbeat = PumpHeartbeatAsync(socket, hub, connectionId, userId, heartbeatCts.Token);

    try
    {
        var buffer = new byte[4 * 1024];
        while (socket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result;
            try
            {
                result = await socket.ReceiveAsync(buffer, CancellationToken.None);
            }
            catch (WebSocketException)
            {
                break;   // el cliente se fue sin cerrar: limpia en finally
            }
            if (result.MessageType == WebSocketMessageType.Close) break;

            string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
            WsEnvelope? env = Json.Deserialize<WsEnvelope>(json);

            switch (env?.Type)
            {
                case "broadcast":
                    await hub.BroadcastAsync(env.Payload ?? json);
                    break;
                case "send":
                    await hub.SendToAsync(env.To ?? "", env.Payload ?? json);
                    break;
            }

            // Heartbeat amortiguado: refresca lease + presencia.
            await hub.HeartbeatAsync(connectionId, userId);
        }
    }
    finally
    {
        heartbeatCts.Cancel();
        try { await heartbeat; } catch (OperationCanceledException) { }
        await hub.RemoveAsync(connectionId, userId);
    }

    return Results.Empty;
});

app.Run();

static async Task PumpHeartbeatAsync(
    WebSocket socket, ConnectionHub hub, string connectionId, string userId,
    CancellationToken ct)
{
    TimeSpan interval = TimeSpan.FromSeconds(10);   // ≪ lease (30 s) → margen de red
    while (!ct.IsCancellationRequested)
    {
        await Task.Delay(interval, ct);
        if (socket.State != WebSocketState.Open) break;
        try
        {
            await hub.HeartbeatAsync(connectionId, userId);
        }
        catch (RedisException ex)
        {
            // Redis caído: el recovery (HubRecoveryHostedService) re-registrará.
            // No cortamos el socket; seguiremos intentando en el siguiente tick.
            Console.WriteLine($"Heartbeat fallido (Redis caído?): {ex.Message}");
        }
    }
}