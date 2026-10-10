using Microsoft.Extensions.DependencyInjection;

// ============================================================================
// LECCIÓN 05 — Decorators con Scrutor + Open Generics en DI
// ============================================================================

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║     MÓDULO 5 — Inyección de Dependencias               ║");
Console.WriteLine("║     Lección 05: Decorators + Open Generics + Scrutor    ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
Console.WriteLine();

int numeroLeccion = 5;

DemoDecorator();
DemoDecoratorAsync().GetAwaiter().GetResult();
DemoOpenGenerics();
DemoAssemblyScanning();

Console.WriteLine("=== RESUMEN ===");
Console.WriteLine();
Console.WriteLine("1. Decorator Pattern: envolver servicios para añadir comportamiento (logging, cache, etc.).");
Console.WriteLine("2. Scrutor.Decorate<TService, TDecorator>(): registra decoradores con orden controlado.");
Console.WriteLine("3. El orden importa: el último decorador envuelve al anterior.");
Console.WriteLine("4. Open Generics: typeof(IRepository<>), typeof(Repository<>) — un registro para todos los tipos.");
Console.WriteLine("5. Assembly Scanning: Scrutor.Scan() registra clases automáticamente por convención.");
Console.WriteLine("6. Estos patrones reducen drásticamente el código de registro manual.");
Console.WriteLine($"Se registran los servicios en el contenedor de DI: {numeroLeccion}");


return;

// ============================================================================
// MÉTODOS ESTÁTICOS
// ============================================================================

static void DemoDecorator()
{
    Console.WriteLine("=== DEMO 1: Decorator Pattern con Scrutor ===");
    Console.WriteLine();


    var services = new ServiceCollection();

    services.AddSingleton<IDataService, DataService>();
    services.Decorate<IDataService, LoggingDataServiceDecorator>();
    services.Decorate<IDataService, CachingDataServiceDecorator>();

    var provider = services.BuildServiceProvider();
    var dataService = provider.GetRequiredService<IDataService>();

    Console.WriteLine("Primera llamada (cache MISS, logging, data real):");
    var result1 = dataService.GetData(1);
    Console.WriteLine($"  Resultado final: '{result1}'");
    Console.WriteLine();

    Console.WriteLine("Segunda llamada con mismo ID (cache HIT, no llega a DataService):");
    var result2 = dataService.GetData(1);
    Console.WriteLine($"  Resultado final: '{result2}'");
    Console.WriteLine();

    Console.WriteLine("Tercera llamada con ID distinto (cache MISS):");
    var result3 = dataService.GetData(2);
    Console.WriteLine($"  Resultado final: '{result3}'");
    Console.WriteLine();
}

static async Task DemoDecoratorAsync()
{
    Console.WriteLine("=== DEMO 2: Decorator Pattern con Async ===");
    Console.WriteLine();

    var services = new ServiceCollection();
    services.AddSingleton<IDataService, DataService>();
    services.Decorate<IDataService, LoggingDataServiceDecorator>();
    services.Decorate<IDataService, CachingDataServiceDecorator>();

    var provider = services.BuildServiceProvider();
    var dataService = provider.GetRequiredService<IDataService>();

    Console.WriteLine("Primera llamada async (cache MISS):");
    var result1 = await dataService.GetDataAsync(3);
    Console.WriteLine($"  Resultado: '{result1}'");
    Console.WriteLine();

    Console.WriteLine("Segunda llamada async con mismo ID (cache HIT):");
    var result2 = await dataService.GetDataAsync(3);
    Console.WriteLine($"  Resultado: '{result2}'");
    Console.WriteLine();
}

static void DemoOpenGenerics()
{
    Console.WriteLine("=== DEMO 3: Open Generics en DI ===");
    Console.WriteLine();

    var services = new ServiceCollection();

    services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));

    var provider = services.BuildServiceProvider();

    var postRepo = provider.GetRequiredService<IRepository<Post>>();
    var categoryRepo = provider.GetRequiredService<IRepository<Category>>();
    var userRepo = provider.GetRequiredService<IRepository<User>>();

    var post = new Post { Title = "Mi primer post", Content = "Contenido del post..." };
    postRepo.Add(post);

    var category = new Category { Name = "Tecnología", Slug = "tecnologia" };
    categoryRepo.Add(category);

    var user = new User { Email = "admin@cms.com", Username = "admin" };
    userRepo.Add(user);

    Console.WriteLine();

    var foundPost = postRepo.GetById(post.Id);
    var foundCategory = categoryRepo.GetById(category.Id);
    var foundUser = userRepo.GetById(user.Id);

    Console.WriteLine($"  Post recuperado: {foundPost}");
    Console.WriteLine($"  Category recuperada: {foundCategory}");
    Console.WriteLine($"  User recuperado: {foundUser}");

    Console.WriteLine();
    Console.WriteLine("Ventajas de Open Generics:");
    Console.WriteLine("  - 1 registro en lugar de N (Post, Category, User, Tag, Comment...)");
    Console.WriteLine("  - Si añades una nueva entidad, no necesitas tocar el registro.");
    Console.WriteLine("  - El contenedor construye automáticamente Repository<T> para cualquier T.");
    Console.WriteLine();
}

static void DemoAssemblyScanning()
{
    Console.WriteLine("=== DEMO 4: Assembly Scanning con Scrutor ===");
    Console.WriteLine();

    var services = new ServiceCollection();

    services.Scan(scan => scan
        .FromAssemblyOf<Program>()
        .AddClasses(classes => classes.AssignableTo<IScannableService>())
        .AsSelf()
        .WithSingletonLifetime());

    services.Scan(scan => scan
        .FromAssemblyOf<IHealthCheck>()
        .AddClasses(classes => classes.AssignableTo<IHealthCheck>())
        .AsImplementedInterfaces()
        .WithSingletonLifetime());

    var provider = services.BuildServiceProvider();

    var notificationService = provider.GetRequiredService<NotificationService>();
    notificationService.Notify("¡Nuevo post publicado!");

    var auditService = provider.GetRequiredService<AuditService>();
    auditService.Audit("Usuario admin editó el post #42");

    Console.WriteLine();

    var healthChecks = provider.GetServices<IHealthCheck>();
    Console.WriteLine($"Health checks registrados automáticamente: {healthChecks.Count()}");
    foreach (var hc in healthChecks)
    {
        Console.WriteLine($"  {hc.GetType().Name}: {(hc.IsHealthy() ? "healthy" : "unhealthy")}");
    }

    Console.WriteLine();
    Console.WriteLine("Ventajas de Assembly Scanning:");
    Console.WriteLine("  - Añadir un servicio nuevo = crear la clase (sin tocar registros)");
    Console.WriteLine("  - Evita cientos de líneas de services.AddSingleton<...>()");
    Console.WriteLine("  - Convención sobre configuración");
    Console.WriteLine();
}

// ============================================================================
// TIPOS
// ============================================================================

public interface IDataService
{
    string GetData(int id);
    Task<string> GetDataAsync(int id);
}

public class DataService : IDataService
{
    private readonly Dictionary<int, string> _data = new()
    {
        [1] = "Post: Introducción a .NET 8",
        [2] = "Post: Guía de Entity Framework Core",
        [3] = "Post: Minimal APIs en ASP.NET Core",
    };

    public string GetData(int id)
    {
        Console.WriteLine($"  [DataService] Recuperando dato con ID={id}...");
        Thread.Sleep(50);
        return _data.TryGetValue(id, out var value) ? value : "No encontrado";
    }

    public async Task<string> GetDataAsync(int id)
    {
        Console.WriteLine($"  [DataService] Recuperando dato async con ID={id}...");
        await Task.Delay(50);
        return _data.TryGetValue(id, out var value) ? value : "No encontrado";
    }
}

public class LoggingDataServiceDecorator : IDataService
{
    private readonly IDataService _inner;

    public LoggingDataServiceDecorator(IDataService inner)
    {
        _inner = inner;
    }

    public string GetData(int id)
    {
        Console.WriteLine($"[LOG] Iniciando GetData({id})...");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var result = _inner.GetData(id);

        stopwatch.Stop();
        Console.WriteLine($"[LOG] GetData({id}) completado en {stopwatch.ElapsedMilliseconds}ms. Resultado: '{result}'");
        return result;
    }

    public async Task<string> GetDataAsync(int id)
    {
        Console.WriteLine($"[LOG] Iniciando GetDataAsync({id})...");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var result = await _inner.GetDataAsync(id);

        stopwatch.Stop();
        Console.WriteLine($"[LOG] GetDataAsync({id}) completado en {stopwatch.ElapsedMilliseconds}ms. Resultado: '{result}'");
        return result;
    }
}

public class CachingDataServiceDecorator : IDataService
{
    private readonly IDataService _inner;
    private readonly Dictionary<int, string> _cache = new();

    public CachingDataServiceDecorator(IDataService inner)
    {
        _inner = inner;
    }

    public string GetData(int id)
    {
        if (_cache.TryGetValue(id, out var cached))
        {
            Console.WriteLine($"[CACHE] HIT para ID={id}: '{cached}'");
            return cached;
        }

        Console.WriteLine($"[CACHE] MISS para ID={id}");
        var result = _inner.GetData(id);
        _cache[id] = result;
        return result;
    }

    public async Task<string> GetDataAsync(int id)
    {
        if (_cache.TryGetValue(id, out var cached))
        {
            Console.WriteLine($"[CACHE] HIT para ID={id}: '{cached}'");
            return cached;
        }

        Console.WriteLine($"[CACHE] MISS para ID={id}");
        var result = await _inner.GetDataAsync(id);
        _cache[id] = result;
        return result;
    }
}

public interface IRepository<T> where T : class
{
    T? GetById(Guid id);
    void Add(T entity);
}

public class Repository<T> : IRepository<T> where T : class
{
    private readonly Dictionary<Guid, T> _storage = new();
    private readonly string _typeName = typeof(T).Name;

    public T? GetById(Guid id)
    {
        _storage.TryGetValue(id, out var entity);
        Console.WriteLine($"  [Repository<{_typeName}>] GetById({id}): {(entity is not null ? "encontrado" : "NO encontrado")}");
        return entity;
    }

    public void Add(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty is not null)
        {
            var id = (Guid)idProperty.GetValue(entity)!;
            _storage[id] = entity;
            Console.WriteLine($"  [Repository<{_typeName}>] Agregado con Id={id}");
        }
    }
}

public class Post
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public override string ToString() => $"Post: '{Title}'";
}

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public override string ToString() => $"Category: '{Name}' ({Slug})";
}

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public override string ToString() => $"User: {Username} <{Email}>";
}

public interface IScannableService { }

public class NotificationService : IScannableService
{
    public void Notify(string message)
    {
        Console.WriteLine($"[NotificationService] Enviando notificación: {message}");
    }
}

public class AuditService : IScannableService
{
    public void Audit(string action)
    {
        Console.WriteLine($"[AuditService] Auditoría: {action} a las {DateTime.Now:HH:mm:ss}");
    }
}

public interface IHealthCheck
{
    bool IsHealthy();
}

public class DatabaseHealthCheck : IHealthCheck
{
    public bool IsHealthy()
    {
        Console.WriteLine("[DatabaseHealthCheck] Verificando conexión... OK");
        return true;
    }
}

public class CacheHealthCheck : IHealthCheck
{
    public bool IsHealthy()
    {
        Console.WriteLine("[CacheHealthCheck] Verificando Redis... OK");
        return true;
    }
}
