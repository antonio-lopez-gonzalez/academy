// ============================================================================
// HubEvents.cs — DTOs que viajan por el backplane de Redis (pub/sub).
// ============================================================================

namespace RedisWsHub.Hub;

/// <summary>Evento emitido por un nodo y recibido por todos (hub:events).</summary>
public sealed record HubEvent(string Type, string? Payload, string From);

/// <summary>Petición de envío dirigido a una conexión concreta (hub:direct).</summary>
public sealed record DirectRequest(string To, string? Payload, string From);

/// <summary>Mensaje recibido por el WebSocket (type, to, payload).</summary>
public sealed record WsEnvelope(string? Type, string? To, string? Payload);