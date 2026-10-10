// ============================================================================
// NodeId.cs — identidad única de este nodo (M7/M8).
// Diferentes instancias DEBEN tener NODE_ID distinto para no pisarse
// el registro en Redis.
// ============================================================================

namespace RedisWsHub;

public static class NodeId
{
    public static string FromEnvironment()
    {
        string? id = Environment.GetEnvironmentVariable("NODE_ID");
        if (!string.IsNullOrWhiteSpace(id)) return id!;
        // Por desarrollo: guid corto por proceso.
        return $"node-{Guid.NewGuid():N}"[..14];
    }
}