// ============================================================================
// Mensajes del WebSocket y tipos de la API.
//
// El envoltorio común ("envelope") lleva: type, timestamp, messageId,
// el contexto de traza (carrier W3C) y el payload de negocio.
// ============================================================================

using System.Text.Json;

namespace NewRelicWsDemo.Messages;

/// <summary>
/// Envoltorio común para TODOS los mensajes del WebSocket.
/// - Type:       tipo de mensaje ("ping", "trade", ...) → se usa para FACET.
/// - Timestamp:  fecha/hora en que lo emitió el cliente (ISO-8601).
/// - MessageId:  identidad del mensaje para correlación.
/// - TraceContext: carrier del W3C Trace Context (traceparent/tracestate).
/// - Payload:    datos de negocio como JSON sin tipar.
/// </summary>
public sealed record MessageEnvelope(
    string Type,
    DateTimeOffset Timestamp,
    string MessageId,
    Dictionary<string, string> TraceContext,
    JsonElement Payload)
{
    /// <summary>
    /// Convierte el JSON recibido por el socket en un MessageEnvelope.
    /// Es tolerante: si faltan campos, los rellena con valores seguros
    /// para que la instrumentación siga funcionando.
    /// </summary>
    public static MessageEnvelope Parse(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var context = new Dictionary<string, string>(StringComparer.Ordinal);
        if (root.TryGetProperty("traceparent", out var tp) && tp.ValueKind == JsonValueKind.String)
            context["traceparent"] = tp.GetString()!;
        if (root.TryGetProperty("tracestate", out var ts) && ts.ValueKind == JsonValueKind.String)
            context["tracestate"] = ts.GetString()!;
        // Tolerancia a clientes que solo mandan "traceId"
        if (root.TryGetProperty("traceId", out var tid) && tid.ValueKind == JsonValueKind.String)
            context["traceId"] = tid.GetString()!;

        return new MessageEnvelope(
            Type: root.TryGetProperty("type", out var t) ? t.GetString() ?? "unknown" : "unknown",
            Timestamp: root.TryGetProperty("timestamp", out var ts2)
                ? ts2.TryGetDateTimeOffset(out var dto) ? dto : DateTimeOffset.UtcNow
                : DateTimeOffset.UtcNow,
            MessageId: root.TryGetProperty("messageId", out var m) ? m.GetString() ?? "?" : "?",
            TraceContext: context,
            Payload: root.TryGetProperty("payload", out var p) ? p.Clone() : default);
    }
}

/// <summary>Petición del endpoint POST /api/broadcast.</summary>
public sealed record BroadcastRequest(string Audience, int SimulateMs = 5);