// ============================================================================
// ConnectionRegistry.cs — "¿dónde está?" y "¿quién está online?" en Redis.
//
//  - Registro: hash por conexión (node, userId, since) + TTL (lease).
//  - Presencia: zset global (userId → timestamp); la ventana de "online"
//    filtra por score y la limpieza es perezosa.
//  - Conteo por usuario: un usuario puede tener varias conexiones a la vez
//    (pestañas, dispositivos). Llevamos un contador atómico INCR/DECR por
//    usuario; SOLO cuando baja a 0 se quita de la presencia (M7-02).
//  - Nodo: key "hub:node:<id>" con TTL renovable (proclama vida).
//
// Todos los métodos son async y atómicos en Redis (M1/M2/M6/M7).
// ============================================================================

using StackExchange.Redis;

namespace RedisWsHub.Hub;

public sealed class ConnectionRegistry
{
    /// <summary>Ventana para considerar a alguien "online" (score reciente).</summary>
    public const long OnlineWindowSecs = 60;

    /// <summary>Lease de presencia/registro (TTL renovable). Debe ser > intervalo de heartbeat.</summary>
    public static readonly TimeSpan Lease = TimeSpan.FromSeconds(30);

    private readonly IDatabase _db;

    public ConnectionRegistry(IConnectionMultiplexer mux, string nodeId)
    {
        _db = mux.GetDatabase();
        NodeId = nodeId;
    }

    /// <summary>Identidad única de este nodo.</summary>
    public string NodeId { get; }

    /// <summary>Proclama el nodo vivo en Redis (se renueva al reconectar).</summary>
    public async Task RegisterNodeAsync()
    {
        await _db.StringSetAsync(Keys.Node(NodeId), "alive", Lease);
        await _db.SetAddAsync(Keys.Nodes, NodeId);
    }

    /// <summary>Registrar una conexión nueva: hash + TTL + presencia + memberships básicos.</summary>
    public async Task RegisterAsync(string connectionId, string userId)
    {
        RedisKey key = Keys.Connection(connectionId);
        await _db.HashSetAsync(key,
        [
            new HashEntry("node", NodeId),
            new HashEntry("userId", userId),
            new HashEntry("since", DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
        ]);
        await _db.KeyExpireAsync(key, Lease);

        // Contador de sockets abiertos de ESTE usuario. Al abrir una conexión:
        // INCR y renovamos el TTL del contador (limpieza si el nodo muere).
        RedisKey counter = Keys.UserConnections(userId);
        await _db.StringIncrementAsync(counter);
        await _db.KeyExpireAsync(counter, Lease);

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await _db.SortedSetAddAsync(Keys.Presence, userId, now);
    }

    /// <summary>Heartbeat: refresca el lease del registro y el score de presencia.</summary>
    public async Task HeartbeatAsync(string connectionId, string userId)
    {
        await _db.KeyExpireAsync(Keys.Connection(connectionId), Lease);
        await _db.SortedSetAddAsync(Keys.Presence, userId, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    /// <summary>Desconexión explícita: baja el contador; la presencia solo se quita con el último socket.</summary>
    public async Task UnregisterAsync(string connectionId, string userId)
    {
        await _db.KeyDeleteAsync(Keys.Connection(connectionId));

        RedisKey counter = Keys.UserConnections(userId);
        long remaining = await _db.StringDecrementAsync(counter);
        if (IsLastSocket(remaining))
        {
            // Última conexión del usuario: limpia contador y presencia.
            await _db.KeyDeleteAsync(counter);
            await _db.SortedSetRemoveAsync(Keys.Presence, userId);
        }
    }

    /// <summary>
    /// Política pura (testeable sin Redis): ¿es este el último socket del usuario?
    /// true cuando el contador llega a 0 o menos (DECR sobre clave inexistente da -1).
    /// </summary>
    public static bool IsLastSocket(long remainingConnections) => remainingConnections <= 0;

    /// <summary>¿En qué nodo vive la conexión? (null si no existe).</summary>
    public async Task<string?> FindNodeAsync(string connectionId)
    {
        RedisValue node = await _db.HashGetAsync(Keys.Connection(connectionId), "node");
        return node.IsNull ? null : node.ToString();
    }

    /// <summary>Ventana "online" actual (p. ej. para /api/online).</summary>
    public async Task<RedisValue[]> OnlineAsync()
    {
        long cutoff = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - OnlineWindowSecs;
        return await _db.SortedSetRangeByScoreAsync(Keys.Presence, cutoff, double.PositiveInfinity);
    }

    /// <summary>Conteo "online" en la ventana.</summary>
    public async Task<long> OnlineCountAsync()
    {
        long cutoff = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - OnlineWindowSecs;
        return await _db.SortedSetLengthAsync(Keys.Presence, cutoff, double.PositiveInfinity);
    }

    /// <summary>Limpieza perezosa: borra de la presencia los scores antiguos.</summary>
    public async Task PrunePresenceAsync()
    {
        long cutoff = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - OnlineWindowSecs;
        await _db.SortedSetRemoveRangeByScoreAsync(Keys.Presence, double.NegativeInfinity, cutoff);
    }
}