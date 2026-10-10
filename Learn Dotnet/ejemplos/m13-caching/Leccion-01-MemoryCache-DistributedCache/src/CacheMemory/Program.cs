// ─── MÓDULO 13 ─── Lección 1: Memory Cache + Distributed Cache ───
// IMemoryCache (en memoria del proceso) vs IDistributedCache (Redis, SQL Server)
// Patrón Cache-Aside: app → cache → miss → DB → store in cache → return

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace CacheMemory;

// ─── Modelo de datos de ejemplo ───
public record Product(int Id, string Name, decimal Price);

// ─── Servicio que simula acceso a base de datos (lento) ───
public interface IDatabase
{
    Task<Product?> GetProductAsync(int id);
}

public class FakeDatabase : IDatabase
{
    // Simula una consulta costosa (200ms)
    public async Task<Product?> GetProductAsync(int id)
    {
        Console.WriteLine($"  [DB] Consultando producto {id}...");
        await Task.Delay(200); // Simula latencia de red/BD

        // Solo devolvemos "datos" para IDs 1-5
        if (id is >= 1 and <= 5)
            return new Product(id, $"Producto {id}", id * 9.99m);

        return null;
    }
}

// ─── Servicio con IMemoryCache (caché en memoria del proceso) ───
public class MemoryCacheService
{
    private readonly IMemoryCache _cache;
    private readonly IDatabase _db;

    public MemoryCacheService(IMemoryCache cache, IDatabase db)
    {
        _cache = cache;
        _db = db;
    }

    // ─── ENFOQUE 1: Manual ───
    // Intentamos obtener del caché. Si no existe, vamos a BD y guardamos.
    public async Task<Product?> GetProductManualAsync(int id)
    {
        string key = $"product:{id}";

        // TryGetValue: devuelve true si la clave existe, y el valor por out
        if (_cache.TryGetValue(key, out Product? cached))
        {
            Console.WriteLine($"  [CACHE HIT] Producto {id} desde caché");
            return cached;
        }

        Console.WriteLine($"  [CACHE MISS] Producto {id} no está en caché");

        // Consultamos la "base de datos"
        Product? product = await _db.GetProductAsync(id);

        if (product is not null)
        {
            // Guardamos en caché con expiración absoluta de 5 minutos
            MemoryCacheEntryOptions options = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                // SlidingExpiration: renueva la expiración cada vez que se accede
                SlidingExpiration = TimeSpan.FromMinutes(2),
                // Priority: en caso de presión de memoria, qué eliminar primero
                Priority = CacheItemPriority.Normal
            };

            _cache.Set(key, product, options);
            Console.WriteLine($"  [CACHE SET] Producto {id} guardado en caché");
        }

        return product;
    }

    // ─── ENFOQUE 2: GetOrCreate ───
    // Evita race condition: si dos hilos piden lo mismo, solo uno ejecuta la factoría.
    public async Task<Product?> GetProductAtomicAsync(int id)
    {
        string key = $"product:{id}";

        // GetOrCreateAsync es atómico: garantiza que la factoría se ejecuta UNA SOLA VEZ
        // incluso si múltiples hilos piden la misma clave simultáneamente.
        Product? product = await _cache.GetOrCreateAsync(key, async entry =>
        {
            // Configuramos la entrada del caché
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.SlidingExpiration = TimeSpan.FromMinutes(2);

            Console.WriteLine($"  [FACTORY] Ejecutando factoría para producto {id}");
            return await _db.GetProductAsync(id);
        });

        return product;
    }

    // Invalidación: eliminar una entrada cuando los datos cambian
    public void Invalidate(int id)
    {
        string key = $"product:{id}";
        _cache.Remove(key);
        Console.WriteLine($"  [INVALIDATE] Producto {id} eliminado del caché");
    }
}

// ─── Servicio con IDistributedCache (simulado con DistributedMemoryCache) ───
public class DistributedCacheService
{
    private readonly IDistributedCache _cache;
    private readonly IDatabase _db;

    public DistributedCacheService(IDistributedCache cache, IDatabase db)
    {
        _cache = cache;
        _db = db;
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        string key = $"product:{id}";

        // IDistributedCache trabaja con byte[], no con objetos
        byte[]? cachedBytes = await _cache.GetAsync(key);

        if (cachedBytes is not null)
        {
            Console.WriteLine($"  [DIST CACHE HIT] Producto {id} desde caché distribuido");
            return JsonSerializer.Deserialize<Product>(cachedBytes);
        }

        Console.WriteLine($"  [DIST CACHE MISS] Producto {id} no en caché distribuido");

        Product? product = await _db.GetProductAsync(id);

        if (product is not null)
        {
            // Serializamos a bytes y guardamos con expiración
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(product);

            DistributedCacheEntryOptions options = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            await _cache.SetAsync(key, bytes, options);
            Console.WriteLine($"  [DIST CACHE SET] Producto {id} guardado");
        }

        return product;
    }

    // GetStringAsync / SetStringAsync son métodos de extensión para simplificar
    // (usan UTF8 internamente, igual que nuestro código manual)
    public async Task<Product?> GetProductWithExtensionsAsync(int id)
    {
        string key = $"product-ext:{id}";

        string? json = await _cache.GetStringAsync(key);
        if (json is not null)
        {
            Console.WriteLine($"  [DIST CACHE HIT] Producto {id} (extensions)");
            return JsonSerializer.Deserialize<Product>(json);
        }

        Console.WriteLine($"  [DIST CACHE MISS] Producto {id} (extensions)");

        Product? product = await _db.GetProductAsync(id);
        if (product is not null)
        {
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
        }

        return product;
    }
}

// ─── Programa principal ───
public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("=== MÓDULO 13 — Caching ===\n");

        // Configuramos el contenedor DI mínimo
        ServiceCollection services = new();

        services.AddMemoryCache();                      // IMemoryCache
        services.AddDistributedMemoryCache();           // IDistributedCache simulado en memoria
        services.AddSingleton<IDatabase, FakeDatabase>();
        services.AddSingleton<MemoryCacheService>();
        services.AddSingleton<DistributedCacheService>();

        ServiceProvider provider = services.BuildServiceProvider();

        // ─── DEMO 1: IMemoryCache ───
        Console.WriteLine("── 1. IMemoryCache (Manual) ──");
        MemoryCacheService memService = provider.GetRequiredService<MemoryCacheService>();

        // Primera llamada: cache miss → BD
        Product? p1 = await memService.GetProductManualAsync(1);
        Console.WriteLine($"  Resultado: {p1}\n");

        // Segunda llamada: cache hit (no va a BD)
        Product? p2 = await memService.GetProductManualAsync(1);
        Console.WriteLine($"  Resultado: {p2}\n");

        // Invalidamos y volvemos a pedir
        memService.Invalidate(1);
        Product? p3 = await memService.GetProductManualAsync(1);
        Console.WriteLine($"  Resultado: {p3}\n");

        // ─── DEMO 2: IMemoryCache con GetOrCreate (atómico) ───
        Console.WriteLine("── 2. IMemoryCache (GetOrCreate - atómico) ──");
        Product? pa = await memService.GetProductAtomicAsync(2);
        Console.WriteLine($"  Resultado: {pa}\n");

        Product? pb = await memService.GetProductAtomicAsync(2); // Cache hit
        Console.WriteLine($"  Resultado: {pb}\n");

        // ─── DEMO 3: IDistributedCache ───
        Console.WriteLine("── 3. IDistributedCache ──");
        DistributedCacheService distService = provider.GetRequiredService<DistributedCacheService>();

        Product? da = await distService.GetProductAsync(3);
        Console.WriteLine($"  Resultado: {da}\n");

        Product? db = await distService.GetProductAsync(3); // Cache hit
        Console.WriteLine($"  Resultado: {db}\n");

        // ─── DEMO 4: IDistributedCache con extensiones ───
        Console.WriteLine("── 4. IDistributedCache con extensiones ──");
        Product? dx = await distService.GetProductWithExtensionsAsync(4);
        Console.WriteLine($"  Resultado: {dx}\n");

        Product? dy = await distService.GetProductWithExtensionsAsync(4);
        Console.WriteLine($"  Resultado: {dy}\n");

        Console.WriteLine("── Fin de la demo ──");
    }
}
