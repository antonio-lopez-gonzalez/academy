// ============================================================================
// ConnectionHub.cs — el "corazón" del hub: sockets locales + backplane Redis.
//
//  - Mantiene los sockets que le tocan a ESTE nodo (diccionario local).
//  - Se suscribe a hub:events (broadcast) y hub:direct (dirigido) y publica.
//  - Filtro anti-loop: ignora mensajes cuyo From es este mismo nodo.
//  - Al conectarse/desconectarse, registra/desregistra en Redis (M7).
//
// Regla de oro: Redis guarda "el directorio"; los sockets viven en el nodo.
// ============================================================================

using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using StackExchange.Redis;

namespace RedisWsHub.Hub;

public sealed class ConnectionHub
{
    private readonly ConcurrentDictionary<string, WebSocket> _local = new();
    private readonly ConnectionRegistry _registry;
    private readonly ISubscriber _sub;
    private readonly ILogger<ConnectionHub> _logger;

    private static readonly RedisChannel ChannelEvents =
        new(Keys.ChannelEvents, RedisChannel.PatternMode.Literal);
    private static readonly RedisChannel ChannelDirect =
        new(Keys.ChannelDirect, RedisChannel.PatternMode.Literal);

    public ConnectionHub(
        IConnectionMultiplexer mux,
        ConnectionRegistry registry,
        ILogger<ConnectionHub> logger)
    {
        _registry = registry;
        _logger = logger;
        _sub = mux.GetSubscriber();
        SubscribeOnce();
    }

    /// <summary>Suscripción inicial (o repetida tras reconexión, M6-04).</summary>
    public async Task ReSubscribeAsync()
    {
        await _sub.UnsubscribeAllAsync();           // evita duplicar callbacks
        SubscribeOnce();
        await Task.CompletedTask;
    }

    private void SubscribeOnce()
    {
        _sub.SubscribeAsync(ChannelEvents, OnEvent);
        _sub.SubscribeAsync(ChannelDirect, OnDirect);
    }

    private async void OnEvent(RedisChannel channel, RedisValue value)
    {
        try
        {
            HubEvent? evt = Json.Deserialize<HubEvent>(value!);
            if (evt is null || evt.From == _registry.NodeId) return;   // origen → no loop

            if (evt.Type == "broadcast")
                await SendToAllAsync(evt.Payload ?? "{}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fallo en hub:events");
        }
    }

    private async void OnDirect(RedisChannel channel, RedisValue value)
    {
        try
        {
            DirectRequest? req = Json.Deserialize<DirectRequest>(value!);
            if (req is null) return;

            if (_local.TryGetValue(req.To, out WebSocket? ws))
                await SendAsync(ws, Json.Serialize(req.Payload));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fallo en hub:direct");
        }
    }

    /// <summary>Nueva conexión: guarda local + registra en Redis + anuncia "connect".</summary>
    public async Task AddAsync(WebSocket socket, string connectionId, string userId)
    {
        _local[connectionId] = socket;
        await _registry.RegisterAsync(connectionId, userId);
        await _sub.PublishAsync(ChannelEvents,
            Json.Serialize(new HubEvent("connect", connectionId, _registry.NodeId)));
    }

    /// <summary>Desconexión: quita local + desregistra + anuncia "disconnect".</summary>
    public async Task RemoveAsync(string connectionId, string userId)
    {
        _local.TryRemove(connectionId, out _);
        await _registry.UnregisterAsync(connectionId, userId);
        await _sub.PublishAsync(ChannelEvents,
            Json.Serialize(new HubEvent("disconnect", connectionId, _registry.NodeId)));
    }

    /// <summary>Broadcast global: reenvío local + publica (cada nodo reenvía lo suyo).</summary>
    public async Task BroadcastAsync(string payload)
    {
        await SendToAllAsync(payload);   // a los sockets de ESTE nodo
        await _sub.PublishAsync(ChannelEvents,
            Json.Serialize(new HubEvent("broadcast", payload, _registry.NodeId)));
    }

    /// <summary>Envío dirigido: si lo tengo local envío; si no, publico hub:direct.</summary>
    public async Task SendToAsync(string connectionId, string payload)
    {
        if (_local.TryGetValue(connectionId, out WebSocket? ws))
            await SendAsync(ws, payload);
        else
            await _sub.PublishAsync(ChannelDirect,
                Json.Serialize(new DirectRequest(connectionId, payload, _registry.NodeId)));
    }

    /// <summary>Heartbeat del socket (llamado desde el loop del conectado).</summary>
    public Task HeartbeatAsync(string connectionId, string userId)
        => _registry.HeartbeatAsync(connectionId, userId);

    private async Task SendToAllAsync(string payload)
    {
        foreach (WebSocket ws in _local.Values)
            await SendAsync(ws, payload);
    }

    private static async Task SendAsync(WebSocket ws, string payload)
    {
        if (ws.State != WebSocketState.Open) return;
        var bytes = Encoding.UTF8.GetBytes(payload);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}