// ============================================================================
// Tests de la lógica pura del hub: convención de keys y DTOs del backplane.
// No requieren Redis: validan contratos que el resto del código usa.
// ============================================================================

using RedisWsHub;
using RedisWsHub.Hub;
using Xunit;

namespace RedisWsHub.Tests;

public class KeysTests
{
    [Fact]
    public void Connection_SigueElNamespace()
        => Assert.Equal("hub:conn:8632", Keys.Connection("8632").ToString());

    [Fact]
    public void Node_SigueElNamespace()
        => Assert.Equal("hub:node:node-1", Keys.Node("node-1").ToString());

    [Fact]
    public void ChannelMembers_SigueElNamespace()
        => Assert.Equal("chan:gaming", Keys.ChannelMembers("gaming").ToString());

    [Fact]
    public void Stats_SigueElNamespace()
        => Assert.Equal("stats:node-1:msgs", Keys.Stats("node-1").ToString());

    [Fact]
    public void UserConnections_SigueElNamespace()
        => Assert.Equal("hub:user:user-42:conns", Keys.UserConnections("user-42").ToString());
}

public class PresencePolicyTests
{
    [Theory]
    [InlineData(1, false)]   // queda otra conexión → el usuario SIGUE online
    [InlineData(0, true)]    // última conexión → se limpia presencia
    [InlineData(-1, true)]   // DECR sobre contador inexistente → -1 → se limpia
    [InlineData(-5, true)]
    public void IsLastSocket_DevuelveCuandoYaNoQuedan(long remaining, bool expected)
        => Assert.Equal(expected, ConnectionRegistry.IsLastSocket(remaining));
}

public class JsonTests
{
    [Fact]
    public void SerializaHubEventEnCamelCase()
    {
        var evt = new HubEvent("broadcast", "hola", "node-1");

        string json = Json.Serialize(evt);

        Assert.Contains("\"type\":\"broadcast\"", json);
        Assert.Contains("\"from\":\"node-1\"", json);
        Assert.DoesNotContain("\"Type\"", json);
    }

    [Fact]
    public void DeserializaWsEnvelopeCaseInsensitive()
    {
        var env = Json.Deserialize<WsEnvelope>("""
            {"TYPE":"send","To":"abc","Payload":"hola"}
            """);

        Assert.NotNull(env);
        Assert.Equal("send", env!.Type);
        Assert.Equal("abc", env.To);
        Assert.Equal("hola", env.Payload);
    }
}