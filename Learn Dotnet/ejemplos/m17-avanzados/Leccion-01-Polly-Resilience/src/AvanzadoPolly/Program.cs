// ─── MÓDULO 17 ─── Lección 1: Polly (Resiliencia) ───
// Polly: librería de resiliencia para .NET
// Políticas: Retry, Circuit Breaker, Timeout, Rate Limiter, Fallback
// ResiliencePipelineBuilder: no genérico, compatible con todas las estrategias

using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using Polly.Timeout;
using Polly.RateLimiting;
using System.Threading.RateLimiting;

namespace AvanzadoPolly;

public class ExternalService
{
    private int _callCount = 0;

    public async Task<string> GetDataAsync(int requestId)
    {
        int callNumber = Interlocked.Increment(ref _callCount);
        Console.WriteLine($"  [HTTP #{requestId}] Intento {callNumber}...");
        await Task.Delay(100);

        if (Random.Shared.NextDouble() < 0.6)
        {
            Console.WriteLine($"  [HTTP #{requestId}] ❌ FALLO simulado");
            throw new HttpRequestException($"Error de red en llamada #{requestId}");
        }

        Console.WriteLine($"  [HTTP #{requestId}] ✅ ÉXITO");
        return $"Datos de la request {requestId}";
    }

    public void ResetCallCount() => _callCount = 0;
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("=== MÓDULO 17 — Polly (Resiliencia) ===\n");

        Console.WriteLine("── 1. RETRY: Reintentar hasta 3 veces ──");
        await DemoRetryAsync();

        Console.WriteLine("\n── 2. RETRY CON BACKOFF EXPONENCIAL ──");
        await DemoRetryWithBackoffAsync();

        Console.WriteLine("\n── 3. CIRCUIT BREAKER ──");
        await DemoCircuitBreakerAsync();

        Console.WriteLine("\n── 4. TIMEOUT ──");
        await DemoTimeoutAsync();

        Console.WriteLine("\n── 5. RATE LIMITER (antes Bulkhead) ──");
        await DemoRateLimiterAsync();

        Console.WriteLine("\n── 6. FALLBACK (manual con try/catch) ──");
        await DemoFallbackAsync();

        Console.WriteLine("\n── 7. POLÍTICAS COMBINADAS (Retry + Timeout + Fallback) ──");
        await DemoCombinedAsync();

        Console.WriteLine("\n── Fin de la demo ──");

        Console.WriteLine("""

        ── POLLY + IHttpClientFactory ──
        ┌─────────────────────────────────────────────────────────────┐
        │ builder.Services.AddHttpClient("GitHub")                    │
        │     .AddResilienceHandler("my-pipeline", builder =>         │
        │     {                                                       │
        │         builder.AddRetry(new RetryStrategyOptions           │
        │         {                                                   │
        │             MaxRetryAttempts = 3,                           │
        │             Delay = TimeSpan.FromMilliseconds(200)          │
        │         });                                                 │
        │         builder.AddCircuitBreaker(new CircuitBreaker...);   │
        │         builder.AddTimeout(TimeSpan.FromSeconds(10));       │
        │     });                                                     │
        └─────────────────────────────────────────────────────────────┘
        """);
    }

    private static async Task DemoRetryAsync()
    {
        ExternalService service = new();

        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(200),
                OnRetry = args =>
                {
                    Console.WriteLine($"    [RETRY] Intento {args.AttemptNumber}. " +
                        $"Esperando {args.RetryDelay.TotalMilliseconds}ms...");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();

        try
        {
            string result = await pipeline.ExecuteAsync(
                async token => await service.GetDataAsync(1));
            Console.WriteLine($"  Resultado final: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ Fallo definitivo: {ex.Message}");
        }
    }

    private static async Task DemoRetryWithBackoffAsync()
    {
        ExternalService service = new();
        service.ResetCallCount();

        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                DelayGenerator = args =>
                {
                    int attempt = args.AttemptNumber;
                    TimeSpan delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    Console.WriteLine($"    [BACKOFF] Intento {attempt}, esperando {delay.TotalSeconds}s...");
                    return new ValueTask<TimeSpan?>(delay);
                },
                OnRetry = args =>
                {
                    Console.WriteLine($"    [RETRY] Error: {args.Outcome.Exception?.Message}");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();

        try
        {
            string result = await pipeline.ExecuteAsync(
                async token => await service.GetDataAsync(2));
            Console.WriteLine($"  Resultado: {result}");
        }
        catch
        {
            Console.WriteLine("  ❌ Agotados todos los reintentos");
        }
    }

    private static async Task DemoCircuitBreakerAsync()
    {
        ExternalService service = new();
        service.ResetCallCount();

        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = 2,
                BreakDuration = TimeSpan.FromSeconds(5),
                OnOpened = args =>
                {
                    Console.WriteLine($"\n  ⚡ CIRCUITO ABIERTO por {args.BreakDuration.TotalSeconds}s");
                    return ValueTask.CompletedTask;
                },
                OnClosed = args =>
                {
                    Console.WriteLine($"\n  🔄 CIRCUITO CERRADO");
                    return ValueTask.CompletedTask;
                },
                OnHalfOpened = args =>
                {
                    Console.WriteLine($"\n  🔶 SEMI-ABIERTO: probando...");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();

        for (int i = 1; i <= 10; i++)
        {
            try
            {
                string result = await pipeline.ExecuteAsync(
                    async token => await service.GetDataAsync(i));
                Console.WriteLine($"  ✅ OK: {result}\n");
            }
            catch (BrokenCircuitException)
            {
                Console.WriteLine($"  ⚡ CIRCUITO ABIERTO - Llamada {i} rechazada\n");
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Error: {ex.Message}\n");
            }
        }
    }

    private static async Task DemoTimeoutAsync()
    {
        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(2),
                OnTimeout = args =>
                {
                    Console.WriteLine("  ⏰ TIMEOUT: operación excedió tiempo límite");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();

        Console.WriteLine("  Operación rápida (500ms)...");
        try
        {
            string r1 = await pipeline.ExecuteAsync(async _ =>
            {
                await Task.Delay(500);
                return "OK - Rápida";
            });
            Console.WriteLine($"  ✅ {r1}");
        }
        catch (TimeoutRejectedException)
        {
            Console.WriteLine("  ⏰ Timeout (no esperado)");
        }

        Console.WriteLine("  Operación lenta (3s)...");
        try
        {
            string r2 = await pipeline.ExecuteAsync(async ct =>
            {
                await Task.Delay(3000, ct); // Pasamos el token para cancelar a tiempo
                return "OK - Lenta";
            });
            Console.WriteLine($"  ✅ {r2}");
        }
        catch (TimeoutRejectedException)
        {
            Console.WriteLine("  ⏰ TIMEOUT: cancelada correctamente");
        }
    }

    private static async Task DemoRateLimiterAsync()
    {
        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddRateLimiter(new ConcurrencyLimiter(
                new ConcurrencyLimiterOptions
                {
                    PermitLimit = 3,
                    QueueLimit = 10,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                }))
            .Build();

        Console.WriteLine("  Lanzando 10 peticiones (máx 3 simultáneas)...");

        List<Task> tasks = new();
        for (int i = 1; i <= 10; i++)
        {
            int id = i;
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    string result = await pipeline.ExecuteAsync(async _ =>
                    {
                        Console.WriteLine($"    [INICIO] Petición {id}");
                        await Task.Delay(1000);
                        Console.WriteLine($"    [FIN] Petición {id}");
                        return $"Resultado {id}";
                    });
                }
                catch (RateLimiterRejectedException)
                {
                    Console.WriteLine($"    [RECHAZADA] Petición {id} - Rate Limiter lleno");
                }
            }));
        }

        await Task.WhenAll(tasks);
        Console.WriteLine("  Completado (solo 3 a la vez)");
    }

    private static async Task DemoFallbackAsync()
    {
        // Fallback manual: simplemente capturamos la excepción
        // En Polly 8.x, para fallback tipado se usa ResiliencePipelineBuilder<T>,
        // pero no es compatible con AddRetry. Alternativa: try/catch manual.
        Console.WriteLine("  Ejecutando con fallback manual...");
        try
        {
            string result = await EjecutarConFallback(
                async () =>
                {
                    await Task.Delay(100);
                    throw new InvalidOperationException("Servicio no disponible");
                },
                "DATOS EN CACHÉ (fallback)");
            Console.WriteLine($"  Resultado: {result}");
        }
        catch
        {
            Console.WriteLine("  ❌ No debería llegar aquí (el fallback lo captura)");
        }
    }

    // Helper: ejecuta una operación con fallback (valor por defecto si falla)
    private static async Task<T> EjecutarConFallback<T>(
        Func<Task<T>> operation, T fallbackValue)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    [FALLBACK] Error: {ex.Message}. Usando valor por defecto.");
            return fallbackValue;
        }
    }

    private static async Task DemoCombinedAsync()
    {
        // Pipeline con Retry + Timeout
        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 2,
                Delay = TimeSpan.FromMilliseconds(300),
                OnRetry = args =>
                {
                    Console.WriteLine($"    [RETRY] Intento {args.AttemptNumber}");
                    return ValueTask.CompletedTask;
                }
            })
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(3)
            })
            .Build();

        Console.WriteLine("  Pipeline: Retry(2) + Timeout(3s) + Fallback manual...");

        string result = await EjecutarConFallback(
            async () =>
            {
                return await pipeline.ExecuteAsync<string>(async _ =>
                {
                    await Task.Delay(100);
                    throw new HttpRequestException("Servicio caído");
                });
            },
            "VALOR POR DEFECTO (combinado)");

        Console.WriteLine($"  Resultado: {result}");

        Console.WriteLine("""

        ── RESUMEN DE POLÍTICAS ──
        | Política         | ¿Qué hace?                                   | Cuándo usarla              |
        |----------------- |----------------------------------------------|----------------------------|
        | Retry            | Reintenta N veces si falla                   | Errores transitorios       |
        | Circuit Breaker  | Corta llamadas tras N fallos, luego prueba   | Proteger servicio externo  |
        | Timeout          | Cancela si excede el tiempo límite           | Evitar llamadas colgadas   |
        | Rate Limiter     | Limita llamadas concurrentes                 | Evitar saturación          |
        | Fallback         | Valor por defecto si todo falla              | Degradación graceful       |

        ── NOTA SOBRE CONVERSIÓN DESDE POLLY V7 ──
        Polly v7: Policy.Handle<T>().RetryAsync(3) → cadenas fluidas
        Polly v8: ResiliencePipelineBuilder().AddRetry(...) → builder pattern
        La API nueva es más modular y permite añadir/eliminar políticas dinámicamente.
        """);
    }
}
