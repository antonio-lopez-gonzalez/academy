using Microsoft.Extensions.DependencyInjection;

// ============================================================================
// LECCIÓN 03 — Singleton, Scoped, Transient (lifetimes)
// ============================================================================

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║     MÓDULO 5 — Inyección de Dependencias               ║");
Console.WriteLine("║     Lección 03: Singleton, Scoped, Transient            ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
Console.WriteLine();

DemoLifetimesSeparados();
DemoCaptiveDependency();
DemoCaptiveDependencySolucion();
ReglaPractica();

Console.WriteLine("=== RESUMEN ===");
Console.WriteLine();
Console.WriteLine("1. Singleton: 1 instancia para toda la app. Cuidado con estado compartido.");
Console.WriteLine("2. Scoped: 1 instancia por scope (petición HTTP). Ideal para DbContext.");
Console.WriteLine("3. Transient: nueva instancia cada vez. Para servicios ligeros sin estado.");
Console.WriteLine("4. Captive Dependency: Nunca inyectar Scoped en Singleton.");
Console.WriteLine("5. ValidateScopes = true en desarrollo para detectar problemas.");
Console.WriteLine("6. Regla: un servicio no debe depender de otro con lifetime más corto.");

return;

// ============================================================================
// MÉTODOS ESTÁTICOS
// ============================================================================

static void DemoLifetimesSeparados()
{
    Console.WriteLine("=== DEMO: Singleton, Scoped y Transient por separado ===");
    Console.WriteLine();

    // --- SINGLETON ---
    var singletonServices = new ServiceCollection();
    singletonServices.AddSingleton<GuidService>();
    var singletonProvider = singletonServices.BuildServiceProvider();

    Console.WriteLine("SINGLETON (misma instancia siempre):");
    var sing1 = singletonProvider.GetRequiredService<GuidService>();
    var sing2 = singletonProvider.GetRequiredService<GuidService>();
    Console.WriteLine($"  Resolución 1: Id={sing1.Id}");
    Console.WriteLine($"  Resolución 2: Id={sing2.Id}");
    Console.WriteLine($"  ¿Mismo ID? {(sing1.Id == sing2.Id ? "SÍ" : "NO")}");
    Console.WriteLine($"  CallCount (estado compartido): llamada 1={sing1.CallCount}, llamada 2={sing1.CallCount}");
    Console.WriteLine($"  CallCount desde sing2 (misma instancia): {sing2.CallCount}");
    Console.WriteLine();

    // --- SCOPED ---
    var scopedServices = new ServiceCollection();
    scopedServices.AddScoped<GuidService>();
    var scopedProvider = scopedServices.BuildServiceProvider();

    Console.WriteLine("SCOPED (una instancia por scope):");
    Console.WriteLine("  --- Scope 1 ---");
    using (var scope1 = scopedProvider.CreateScope())
    {
        var sc1a = scope1.ServiceProvider.GetRequiredService<GuidService>();
        var sc1b = scope1.ServiceProvider.GetRequiredService<GuidService>();
        Console.WriteLine($"    Resolución 1a: Id={sc1a.Id}");
        Console.WriteLine($"    Resolución 1b: Id={sc1b.Id}");
        Console.WriteLine($"    ¿Mismo ID dentro del scope? {(sc1a.Id == sc1b.Id ? "SÍ" : "NO")}");
    }

    Console.WriteLine("  --- Scope 2 ---");
    using (var scope2 = scopedProvider.CreateScope())
    {
        var sc2 = scope2.ServiceProvider.GetRequiredService<GuidService>();
        Console.WriteLine($"    Resolución 2:  Id={sc2.Id}");
        Console.WriteLine("    ¿Diferente del Scope 1? SÍ (cada scope tiene su propia instancia)");
    }
    Console.WriteLine();

    // --- TRANSIENT ---
    var transientServices = new ServiceCollection();
    transientServices.AddTransient<GuidService>();
    var transientProvider = transientServices.BuildServiceProvider();

    Console.WriteLine("TRANSIENT (nueva instancia siempre):");
    var trans1 = transientProvider.GetRequiredService<GuidService>();
    var trans2 = transientProvider.GetRequiredService<GuidService>();
    var trans3 = transientProvider.GetRequiredService<GuidService>();
    Console.WriteLine($"  Resolución 1: Id={trans1.Id}");
    Console.WriteLine($"  Resolución 2: Id={trans2.Id}");
    Console.WriteLine($"  Resolución 3: Id={trans3.Id}");
    Console.WriteLine($"  ¿Todos diferentes? {(trans1.Id != trans2.Id && trans2.Id != trans3.Id && trans1.Id != trans3.Id ? "SÍ" : "NO")}");
    Console.WriteLine();
}

static void DemoCaptiveDependency()
{
    Console.WriteLine("=== DEMO: CAPTIVE DEPENDENCY (dependencia cautiva) ===");
    Console.WriteLine();

    var services = new ServiceCollection();
    services.AddScoped<ScopedService>();
    services.AddSingleton<SingletonService>();

    ServiceProviderOptions options = new() { ValidateScopes = false };
    var provider = services.BuildServiceProvider(options);

    var singleton = provider.GetRequiredService<SingletonService>();
    singleton.DoWork();

    Console.WriteLine();
    Console.WriteLine("  PROBLEMA: ScopedService está atrapado dentro de SingletonService.");
    Console.WriteLine("  Aunque crees nuevos scopes, el SingletonService siempre usará la");
    Console.WriteLine("  misma instancia de ScopedService (la que se creó con él).");
    Console.WriteLine();

    Console.WriteLine("  Creando nuevo scope...");
    using (var scope = provider.CreateScope())
    {
        var scopedFromScope = scope.ServiceProvider.GetRequiredService<ScopedService>();
        Console.WriteLine($"  ScopedService desde scope nuevo: {scopedFromScope.Name}");
        singleton.DoWork();
        Console.WriteLine("  (SingletonService sigue usando el ScopedService capturado)");
    }

    Console.WriteLine();
}

static void DemoCaptiveDependencySolucion()
{
    Console.WriteLine("=== DEMO: Solución a Captive Dependency ===");
    Console.WriteLine();

    Console.WriteLine("SOLUCIÓN 1 — ValidateScopes = true (desarrollo):");
    Console.WriteLine("  En Program.cs de ASP.NET, en desarrollo:");
    Console.WriteLine("  builder.Host.UseDefaultServiceProvider(options =>");
    Console.WriteLine("  {");
    Console.WriteLine("      options.ValidateScopes = true;");
    Console.WriteLine("      options.ValidateOnBuild = true;");
    Console.WriteLine("  });");
    Console.WriteLine();

    Console.WriteLine("SOLUCIÓN 2 — Rediseñar:");
    Console.WriteLine("  - Si un Singleton necesita datos de un Scoped, pásaselos por parámetro.");
    Console.WriteLine("  - Usa IServiceScopeFactory para crear scopes manualmente dentro del Singleton.");
    Console.WriteLine("  - Cambia el Singleton a Scoped si realmente necesita dependencias Scoped.");
    Console.WriteLine();

    var services = new ServiceCollection();
    services.AddScoped<ScopedService>();
    services.AddSingleton<SingletonServiceConFactory>();

    var provider = services.BuildServiceProvider();
    var singletonConFactory = provider.GetRequiredService<SingletonServiceConFactory>();

    Console.WriteLine("SOLUCIÓN 3 — IServiceScopeFactory (crear scopes manualmente):");
    singletonConFactory.DoWork();
    singletonConFactory.DoWork();
    Console.WriteLine("  (Cada llamada crea un scope nuevo, ScopedService se libera correctamente)");
    Console.WriteLine();
}

static void ReglaPractica()
{
    Console.WriteLine("=== REGLA PRÁCTICA DE LIFETIMES ===");
    Console.WriteLine();

    Console.WriteLine("REGLAS DE ORO:");
    Console.WriteLine();
    Console.WriteLine("  1. SINGLETON → Scoped  ❌ NUNCA");
    Console.WriteLine("     (Scoped quedaría capturado para siempre)");
    Console.WriteLine();
    Console.WriteLine("  2. SCOPED → Singleton  ✅ OK");
    Console.WriteLine();
    Console.WriteLine("  3. SCOPED → Transient  ✅ OK");
    Console.WriteLine();
    Console.WriteLine("  4. TRANSIENT → Scoped   ✅ OK (pero cuidado con la semántica)");
    Console.WriteLine();
    Console.WriteLine("  Regla nemotécnica:");
    Console.WriteLine("  Un servicio NO debe depender de otro con tiempo de vida MÁS CORTO.");
    Console.WriteLine("  Singleton(vive siempre) → NO depende de → Scoped(vive por petición)");
    Console.WriteLine();
}

// ============================================================================
// TIPOS
// ============================================================================

public class GuidService
{
    public Guid Id { get; } = Guid.NewGuid();
    private int _callCount;
    public int CallCount => ++_callCount;
}

public class ScopedService
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name => $"ScopedService ({Id})";
}

public class SingletonService
{
    private readonly ScopedService _scoped;

    public SingletonService(ScopedService scoped)
    {
        _scoped = scoped;
    }

    public void DoWork()
    {
        Console.WriteLine($"  SingletonService usando {_scoped.Name}");
    }
}

public class SingletonServiceConFactory
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SingletonServiceConFactory(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void DoWork()
    {
        using var scope = _scopeFactory.CreateScope();
        var scoped = scope.ServiceProvider.GetRequiredService<ScopedService>();
        Console.WriteLine($"  Usando {scoped.Name} desde scope temporal");
    }
}
