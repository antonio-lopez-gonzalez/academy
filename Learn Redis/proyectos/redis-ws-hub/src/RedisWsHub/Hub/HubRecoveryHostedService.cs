// ============================================================================
// HubRecoveryHostedService.cs — resiliencia ante caídas de Redis (M6-04/M7-04).
//
// Cuando Redis se restablece:
//   - re-registra el nodo (Key de vida + set de nodos)
//   - re-suscribe los canales (hub:events / hub:direct)
//   - limpia presencia antigua (prune perezoso)
// ============================================================================

using StackExchange.Redis;

namespace RedisWsHub.Hub;

public sealed class HubRecoveryHostedService : BackgroundService
{
    private readonly IConnectionMultiplexer _mux;
    private readonly ConnectionRegistry _registry;
    private readonly ConnectionHub _hub;
    private readonly ILogger<HubRecoveryHostedService> _logger;

    public HubRecoveryHostedService(
        IConnectionMultiplexer mux,
        ConnectionRegistry registry,
        ConnectionHub hub,
        ILogger<HubRecoveryHostedService> logger)
    {
        _mux = mux;
        _registry = registry;
        _hub = hub;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _mux.ConnectionFailed += (_, e) =>
            _logger.LogWarning("Redis caído: {Msg}", e.Exception?.Message);

        _mux.ConnectionRestored += async (_, _) =>
        {
            _logger.LogInformation("Redis restaurado: re-registro y re-suscripción");
            await _registry.RegisterNodeAsync();
            await _registry.PrunePresenceAsync();
            await _hub.ReSubscribeAsync();
        };

        return Task.CompletedTask;
    }
}