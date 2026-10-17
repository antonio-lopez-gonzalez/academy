// ============================================================================
// Procesamiento de un mensaje "ping": devuelve una métrica de latencia y un log.
// ============================================================================

using Microsoft.Extensions.Logging;
using NewRelic.Api.Agent;
using NewRelicWsDemo.Messages;

namespace NewRelicWsDemo.Handlers;

public sealed class PingProcessor
{
    private readonly ILogger<PingProcessor> _logger;

    public PingProcessor(ILogger<PingProcessor> logger) => _logger = logger;

    /// <summary>Segmento del ping dentro de la transacción del mensaje.</summary>
    [Trace]
    public void Respond(MessageEnvelope msg)
    {
        // Coste mínimo del ping
        Thread.Sleep(Random.Shared.Next(1, 8));

        // Métrica de latencia del ping (M2-02)
        NewRelic.Api.Agent.NewRelic.RecordMetric(
            "Custom/PingLatencyMs", Random.Shared.Next(1, 8));

        _logger.LogDebug("ping {MessageId} ok", msg.MessageId);
    }
}