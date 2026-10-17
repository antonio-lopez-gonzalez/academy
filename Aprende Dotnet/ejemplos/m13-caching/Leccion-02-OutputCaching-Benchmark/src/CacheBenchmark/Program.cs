// ─── MÓDULO 13 ─── Lección 2: BenchmarkDotNet + Conceptos de Output Caching ───
// BenchmarkDotNet: librería para medir rendimiento de micro-benchmarks
// Output Caching: cachear respuestas HTTP completas (ASP.NET Core 8)
// ETags: cabecera If-None-Match para validación condicional de caché HTTP

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Text;

namespace CacheBenchmark;

// ─── Benchmark: Comparar 3 formas de invertir un string ───
[MemoryDiagnoser]        // Mide consumo de memoria
[RankColumn]             // Muestra ranking 1, 2, 3...
public class StringReverseBenchmarks
{
    private const string Input = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    // ─── Método 1: StringBuilder (clásico) ───
    [Benchmark(Baseline = true)] // Este es el baseline para comparar
    public string ReverseWithStringBuilder()
    {
        StringBuilder sb = new(Input.Length);
        for (int i = Input.Length - 1; i >= 0; i--)
            sb.Append(Input[i]);
        return sb.ToString();
    }

    // ─── Método 2: Array + string constructor ───
    [Benchmark]
    public string ReverseWithArray()
    {
        char[] chars = Input.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    // ─── Método 3: LINQ ───
    [Benchmark]
    public string ReverseWithLinq()
    {
        return new string(Input.Reverse().ToArray());
    }
}

// ─── Benchmark: Dictionary vs Lookup vs Array access ───
[MemoryDiagnoser]
[RankColumn]
public class LookupBenchmarks
{
    private Dictionary<int, string> _dictionary = null!;
    private string[] _array = null!;
    private ILookup<int, string> _lookup = null!;

    [GlobalSetup]
    public void Setup()
    {
        _dictionary = Enumerable.Range(1, 1000)
            .ToDictionary(i => i, i => $"Value {i}");

        _array = new string[1001];
        for (int i = 1; i <= 1000; i++)
            _array[i] = $"Value {i}";

        _lookup = Enumerable.Range(1, 1000)
            .SelectMany(i => new[] { (key: i, value: $"Value {i}") })
            .ToLookup(x => x.key, x => x.value);
    }

    [Benchmark(Baseline = true)]
    public string DictionaryAccess()
    {
        return _dictionary[500]; // Acceso por clave
    }

    [Benchmark]
    public string ArrayAccess()
    {
        return _array[500]; // Acceso por índice directo
    }

    [Benchmark]
    public string LookupAccess()
    {
        return _lookup[500].First(); // Lookup requiere First()
    }
}

// ─── Programa principal ───
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== MÓDULO 13 — BenchmarkDotNet ===\n");

        // ─── Explicación de Output Caching (.NET 8, ASP.NET) ───
        Console.WriteLine("""
        ── OUTPUT CACHING (.NET 8) ──

        Output Caching cachea la respuesta HTTP completa. Es diferente de:
        - IMemoryCache: cachea objetos en la aplicación
        - IDistributedCache: cachea datos serializados en infraestructura externa

        Configuración típica en Program.cs de ASP.NET:
        ┌─────────────────────────────────────────────────────────────┐
        │ builder.Services.AddOutputCache(options =>                  │
        │ {                                                           │
        │     options.AddBasePolicy(builder =>                        │
        │         builder.Expire(TimeSpan.FromMinutes(5)));           │
        │ });                                                         │
        │                                                             │
        │ app.UseOutputCache();                                       │
        │                                                             │
        │ app.MapGet("/posts", ...).CacheOutput(policy =>             │
        │     policy.Expire(TimeSpan.FromMinutes(10))                 │
        │            .SetVaryByQuery("page", "size"));                │
        │ ─────────────────────────────────────────────────────────  │
        │                                                             │
        │ - Expire(): tiempo de vida del caché                        │
        │ - SetVaryByQuery(): cachear versiones por parámetros query  │
        │ - SetVaryByHeader(): versiones por cabeceras HTTP           │
        │ - Tag(): permite invalidar grupos de cachés                 │
        └─────────────────────────────────────────────────────────────┘

        ── ETags e If-None-Match ──

        Los ETags son un mecanismo HTTP para validación condicional:
        1. El servidor envía Response Header: ETag: "abc123"
        2. El cliente guarda el ETag
        3. Siguiente petición: Request Header: If-None-Match: "abc123"
        4. Si el recurso NO cambió → HTTP 304 Not Modified (sin body)
        5. Si el recurso CAMBIÓ → HTTP 200 con el nuevo ETag

        En ASP.NET Core:
        ┌─────────────────────────────────────────────────────────────┐
        │ app.MapGet("/posts/{id}", (int id) => {                     │
        │     var post = GetPost(id);                                 │
        │     var etag = GenerateEtag(post);                          │
        │                                                             │
        │     // Si el cliente ya tiene esta versión, 304             │
        │     if (Request.Headers.IfNoneMatch == etag)                │
        │         return Results.StatusCode(304);                     │
        │                                                             │
        │     return Results.Ok(post)                                 │
        │         .WithHeader("ETag", etag);                          │
        │ });                                                         │
        │ ─────────────────────────────────────────────────────────  │
        │                                                             │
        │ Función helper para generar ETag:                           │
        │ string GenerateEtag(object obj) =>                          │
        │     $"\"{Convert.ToBase64String(SHA256.HashData(            │
        │         JsonSerializer.SerializeToUtf8Bytes(obj)))}\"";     │
        └─────────────────────────────────────────────────────────────┘

        ── Response Compression ──

        Comprime las respuestas HTTP (gzip, brotli) para ahorrar ancho de banda.
        Configuración en ASP.NET Core:
        ┌─────────────────────────────────────────────────────────────┐
        │ builder.Services.AddResponseCompression(options =>          │
        │ {                                                           │
        │     options.EnableForHttps = true;                          │
        │     options.Providers.Add<BrotliCompressionProvider>();     │
        │     options.Providers.Add<GzipCompressionProvider>();       │
        │ });                                                         │
        │                                                             │
        │ app.UseResponseCompression();                               │
        │ ─────────────────────────────────────────────────────────  │
        └─────────────────────────────────────────────────────────────┘

        """);

        // ─── Ejecutar benchmarks ───
        Console.WriteLine("Ejecutando benchmarks... (esto tarda unos segundos)\n");

        // Benchmark 1: String reverse
        Console.WriteLine("── Benchmark 1: String Reverse ──");
        BenchmarkRunner.Run<StringReverseBenchmarks>();

        Console.WriteLine("\n── Benchmark 2: Dictionary vs Array vs Lookup ──");
        BenchmarkRunner.Run<LookupBenchmarks>();

        Console.WriteLine("\n── Fin de los benchmarks ──");
    }
}
