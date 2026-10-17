// ============================================================================
// Tests de la parte "pura" del proyecto: el parsing del MessageEnvelope.
// (El resto de la instrumentación no se testea aquí: la API de New Relic
//  es no-op sin agente y su objetivo es observacional, no funcional.)
// ============================================================================

using System.Text.Json;
using NewRelicWsDemo.Messages;
using Xunit;

namespace NewRelicWsDemo.Tests;

public class MessageEnvelopeTests
{
    [Fact]
    public void Parse_DevuelveCamposYContextoW3C()
    {
        // Arrange: un trade con traceparent/tracestate en el JSON
        const string json = """
            {
              "type": "trade",
              "timestamp": "2026-08-31T10:15:00.123Z",
              "messageId": "887",
              "traceparent": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01",
              "tracestate": "newrelic=1-21-...-...",
              "payload": { "symbol": "EURUSDD", "amount": 1000 }
            }
            """;

        // Act
        var env = MessageEnvelope.Parse(json);

        // Assert
        Assert.Equal("trade", env.Type);
        Assert.Equal("887", env.MessageId);
        Assert.Equal(1000m, env.Payload.GetProperty("amount").GetDecimal());
        Assert.True(env.TraceContext.ContainsKey("traceparent"));
        Assert.True(env.TraceContext.ContainsKey("tracestate"));
        Assert.Equal("4bf92f3577b34da6a3ce929d0e0e4736",
            env.TraceContext["traceparent"]!.Split('-')[1]);
    }

    [Fact]
    public void Parse_SoloConTraceIdComoCarrier()
    {
        // Algunos clientes solo envían "traceId": lo guardamos igualmente
        const string json = """{ "type": "ping", "messageId": "1", "traceId": "abcdef0123456789abcdef0123456789", "timestamp": "2026-08-31T10:00:00Z" }""";

        var env = MessageEnvelope.Parse(json);

        Assert.Equal("ping", env.Type);
        Assert.Equal("abcdef0123456789abcdef0123456789", env.TraceContext["traceId"]);
    }

    [Fact]
    public void Parse_JsonMalformadoLanza()
    {
        // JsonDocument lanza JsonException (o su subclase JsonReaderException)
        Assert.ThrowsAny<JsonException>(() => MessageEnvelope.Parse("{ no es json"));
    }
}