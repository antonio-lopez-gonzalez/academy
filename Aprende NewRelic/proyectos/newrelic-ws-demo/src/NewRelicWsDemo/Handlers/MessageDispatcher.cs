// ============================================================================
// Orquesta el procesado de CADA mensaje y lo convierte en una transacción.
//
// Patrón (M4):
//   1. [Transaction] crea una OtherTransaction por mensaje (si no hay una en curso).
//   2. Atributos del mensaje en la transacción (facetables con NRQL).
//   3. AcceptDistributedTraceHeaders enlaza el W3C que viaja en el JSON (M3-04).
//   4. Errores esperados vs. no esperados con NoticeError (M2-03).
//   5. Métricas de volumen y latencia de cola (M2-02).
// ============================================================================

using Microsoft.Extensions.Logging;
using NewRelic.Api.Agent;
using NewRelicWsDemo.Messages;

namespace NewRelicWsDemo.Handlers;

public sealed class MessageDispatcher
{
    private readonly TradeProcessor _trade;
    private readonly PingProcessor _ping;
    private readonly ILogger<MessageDispatcher> _logger;

    public MessageDispatcher(
        TradeProcessor trade,
        PingProcessor ping,
        ILogger<MessageDispatcher> logger)
    {
        _trade = trade;
        _ping = ping;
        _logger = logger;
    }

    /// <summary>Una transacción por mensaje (el "corazón del WebSocket").</summary>
    [Transaction]
    public void OnMessage(MessageEnvelope msg)
    {
        ITransaction txn = NewRelic.Api.Agent.NewRelic.GetAgent().CurrentTransaction;

        // Contexto del mensaje como atributos (M2-04)
        txn.AddCustomAttribute("messageType", msg.Type)
           .AddCustomAttribute("messageId", msg.MessageId);

        // Latencia de cola: cuánto tardó el mensaje en llegar desde su emisión
        var queueMs = (DateTimeOffset.UtcNow - msg.Timestamp).TotalMilliseconds;
        txn.AddCustomAttribute("queueLatencyMs", (int)queueMs);
        NewRelic.Api.Agent.NewRelic.RecordMetric("Custom/QueueLatencyMs", (float)queueMs);

        // Propaga el contexto W3C que viaja en el payload (M3-04). El getter
        // debe ser tolerante: si una clave no existe, devuelve lista vacía.
        // TransportType no tiene un valor "WebSocket": usamos Other.
        txn.AcceptDistributedTraceHeaders(
            msg.TraceContext,
            static (carrier, key) =>
                carrier.TryGetValue(key, out var value) ? [value] : [],
            TransportType.Other);

        try
        {
            // Despacho por tipo
            switch (msg.Type)
            {
                case "trade": _trade.ProcessTrade(msg); break;
                case "ping":  _ping.Respond(msg);       break;
                default:
                    _logger.LogWarning("Tipo de mensaje desconocido {Type}", msg.Type);
                    break;
            }
        }
        catch (Exception ex)
        {
            var attrs = new Dictionary<string, string>
            {
                ["messageType"] = msg.Type,
                ["messageId"] = msg.MessageId,
            };

            var expected = IsBusinessError(msg, ex);
            if (expected)
                NewRelic.Api.Agent.NewRelic.NoticeError(ex.Message, attrs, true);
            else
                NewRelic.Api.Agent.NewRelic.NoticeError(ex, attrs);

            _logger.LogError(ex, "Fallo procesando mensaje {Type} {MessageId}",
                msg.Type, msg.MessageId);
        }
        finally
        {
            // Contador por tipo (métrica, no evento: es alto volumen)
            NewRelic.Api.Agent.NewRelic.IncrementCounter($"Custom/Messages/{msg.Type}");
        }
    }

    /// <summary>Notifica mensajes que ni siquiera se pudieron parsear.</summary>
    public void OnMalformed(Exception ex)
    {
        NewRelic.Api.Agent.NewRelic.NoticeError(
            "Mensaje no parseable",
            new Dictionary<string, string>(),
            isExpected: true);
        _logger.LogError(ex, "JSON malformado recibido por el WebSocket");
    }

    private static bool IsBusinessError(MessageEnvelope msg, Exception ex)
        => ex is InvalidDataException or ArgumentException or KeyNotFoundException;
}