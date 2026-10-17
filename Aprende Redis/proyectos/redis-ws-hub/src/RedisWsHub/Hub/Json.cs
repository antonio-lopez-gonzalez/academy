// ============================================================================
// Json.cs — helpers de System.Text.Json con políticas web (camelCase):
// - Escribir y leer DTOs del backplane de forma consistente.
// ============================================================================

using System.Text.Json;

namespace RedisWsHub.Hub;

public static class Json
{
    // Policy web: camelCase al escribir, lectura case-insensitive.
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

    public static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
}