// ============================================================================
// Procesamiento de un mensaje "trade": el ejemplo estrella de trace.
//
// Fases instrumentadas como spans hijos de la transacción (M4-03):
//   [Trace] ProcessTrade → span raíz del procesado del trade
//   [Trace] Validate     → validación de negocio
//   [Trace] Apply        → aplicar la operación + broadcast simulado
//
// Además: atributos de span, un custom event de negocio (OrderPlaced) y
// logs en contexto con el ILogger, que el agente enriquece con trace.id.
// ============================================================================

using System.Text.Json;
using Microsoft.Extensions.Logging;
using NewRelic.Api.Agent;
using NewRelicWsDemo.Messages;
using NewRelicWsDemo.Services;

namespace NewRelicWsDemo.Handlers;

public sealed class TradeProcessor
{
    private readonly PriceSimulator _prices;
    private readonly ILogger<TradeProcessor> _logger;

    public TradeProcessor(PriceSimulator prices, ILogger<TradeProcessor> logger)
    {
        _prices = prices;
        _logger = logger;
    }

    /// <summary>Segmento principal del trade dentro de la transacción del mensaje.</summary>
    [Trace]
    public void ProcessTrade(MessageEnvelope msg)
    {
        string symbol = GetSymbol(msg);

        // Estado del span: atributos de la fase (M4-03)
        ISpan span = NewRelic.Api.Agent.NewRelic.GetAgent().CurrentSpan;
        span.AddCustomAttribute("symbol", symbol);

        Validate(msg, symbol);   // [Trace]
        Apply(msg, symbol);      // [Trace]
    }

    [Trace]
    private void Validate(MessageEnvelope msg, string symbol)
    {
        ISpan span = NewRelic.Api.Agent.NewRelic.GetAgent().CurrentSpan;
        span.AddCustomAttribute("symbol", symbol);

        // Validación simple de negocio: cantidad > 0 y símbolo no vacío
        var amount = GetAmount(msg);
        var ok = amount > 0 && !string.IsNullOrWhiteSpace(symbol);
        span.AddCustomAttribute("isValid", ok);

        if (!ok)
            throw new InvalidDataException($"trade {msg.MessageId} inválido (amount={amount})");
    }

    [Trace]
    private void Apply(MessageEnvelope msg, string symbol)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        // "Costo" del trabajo: simula reglas + persistencia + broadcast
        var price = _prices.GetPrice(symbol);
        Thread.Sleep(Random.Shared.Next(5, 40));

        sw.Stop();

        IAgent agent = NewRelic.Api.Agent.NewRelic.GetAgent();
        agent.CurrentSpan
            .AddCustomAttribute("priceValue", price.Value)
            .AddCustomAttribute("applyMs", sw.ElapsedMilliseconds);

        // Métrica de tiempo de procesado de trades (M2-02)
        NewRelic.Api.Agent.NewRelic.RecordResponseTimeMetric(
            "Custom/TradeProcessingMs", sw.ElapsedMilliseconds);

        // Custom event de NEGOCIO (bajo volumen, con atributos facetables)
        NewRelic.Api.Agent.NewRelic.RecordCustomEvent("OrderPlaced",
            new Dictionary<string, object>
            {
                ["messageId"] = msg.MessageId,
                ["symbol"] = symbol,
                ["price"] = (double)price.Value,
                ["currency"] = "EUR",
                ["amount"] = (double)GetAmount(msg),
            });

        _logger.LogInformation(
            "trade={MessageId} symbol={Symbol} price={Price:N4} applyMs={ApplyMs}",
            msg.MessageId, symbol, price.Value, sw.ElapsedMilliseconds);
    }

    private static string GetSymbol(MessageEnvelope msg) =>
        msg.Payload.ValueKind == JsonValueKind.Object &&
        msg.Payload.TryGetProperty("symbol", out var s)
            ? s.GetString() ?? "UNKNOWN"
            : "UNKNOWN";

    private static decimal GetAmount(MessageEnvelope msg) =>
        msg.Payload.ValueKind == JsonValueKind.Object &&
        msg.Payload.TryGetProperty("amount", out var a) && a.TryGetDecimal(out var d)
            ? d
            : 0m;
}