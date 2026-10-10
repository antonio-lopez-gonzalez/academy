// ============================================================================
// Simulador de precio: hace de "servicio externo" para el ejemplo.
// En un caso real sería un HttpClient a tu proveedor de precios. Lo mantenemos
// SÍNCRONO para que la instrumentación con [Trace] sea sencilla y predecible.
// ============================================================================

namespace NewRelicWsDemo.Services;

public sealed record Price(decimal Value, string Source);

public sealed class PriceSimulator
{
    private readonly decimal _base;

    public PriceSimulator() => _base = 1m + (decimal)Random.Shared.NextDouble() * 2m;

    /// <summary>Simula una llamada a un API de precios con algo de coste (10-90 ms).</summary>
    public Price GetPrice(string symbol)
    {
        // Simula la latencia de red/proveedor
        Thread.Sleep(Random.Shared.Next(10, 90));

        var tick = (decimal)(Random.Shared.NextDouble() - 0.5) * 0.02m;
        return new Price(_base + tick, "simulator");
    }
}