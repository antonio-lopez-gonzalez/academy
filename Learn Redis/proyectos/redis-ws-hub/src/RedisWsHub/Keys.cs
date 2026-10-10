// ============================================================================
// Keys.cs — convención de namespaces (M1-01).
// Toda key del hub se construye aquí, en un único sitio.
// Patrón: dominio:entidad:identificador
// ============================================================================

namespace RedisWsHub;

using StackExchange.Redis;

public static class Keys
{
    // --- canales de pub/sub ---
    public const string ChannelEvents = "hub:events";    // broadcasts de conexión
    public const string ChannelDirect = "hub:direct";    // envíos dirigidos
    public const string ChannelPresence = "hub:pulse";   // pulsos de presencia (opcional)

    // --- keys de estado ---
    public const string Presence = "hub:presence";        // zset: userId → timestamp
    public const string Nodes = "hub:nodes";              // set: ids de nodos vivos

    /// <summary>Hash del detalle de una conexión (node, userId, since).</summary>
    public static RedisKey Connection(string connectionId) => $"hub:conn:{connectionId}";

    /// <summary>String con TTL que proclama "este nodo sigue vivo".</summary>
    public static RedisKey Node(string nodeId) => $"hub:node:{nodeId}";

    /// <summary>Set de miembros de un canal de chat.</summary>
    public static RedisKey ChannelMembers(string channel) => $"chan:{channel}";

    /// <summary>Contador de mensajes procesados por nodo (M1-04).</summary>
    public static RedisKey Stats(string nodeId) => $"stats:{nodeId}:msgs";

    /// <summary>
    /// Contador de sockets abiertos por usuario (hub:user:&lt;uid&gt;:conns).
    /// Permite presencia correcta cuando un usuario tiene varias conexiones
    /// (pestañas/dispositivos): solo se quita de la presencia al cerrar la última
    /// (INCR/DECR atómicos, M1-04 / M7-02).
    /// </summary>
    public static RedisKey UserConnections(string userId) => $"hub:user:{userId}:conns";
}