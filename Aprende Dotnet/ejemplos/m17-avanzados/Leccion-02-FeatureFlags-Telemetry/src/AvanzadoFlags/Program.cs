// ─── MÓDULO 17 ─── Lección 2: Feature Flags + OpenTelemetry ───
// Feature Flags: activar/desactivar funcionalidades sin redeploy
// OpenTelemetry: trazas distribuidas, métricas y logs estandarizados

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using System.Diagnostics; // ActivitySource, Activity

namespace AvanzadoFlags;

// ─── Servicios de ejemplo con Feature Flags ───
public interface IDashboardService
{
    Task<string> GetDashboardAsync();
}

public class DashboardService : IDashboardService
{
    private readonly IFeatureManager _featureManager;

    public DashboardService(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    public async Task<string> GetDashboardAsync()
    {
        // Verificar si el nuevo dashboard está habilitado
        bool newDashboardEnabled = await _featureManager.IsEnabledAsync("NewDashboard");
        bool darkModeEnabled = await _featureManager.IsEnabledAsync("DarkMode");
        bool experimentalWidgets = await _featureManager.IsEnabledAsync("ExperimentalWidgets");

        string dashboardType = newDashboardEnabled ? "NUEVO Dashboard v2" : "Dashboard clásico v1";
        string theme = darkModeEnabled ? "(Modo oscuro)" : "(Modo claro)";
        string extras = experimentalWidgets ? " [Widgets experimentales ACTIVOS]" : "";

        return $"{dashboardType} {theme}{extras}";
    }
}

// ─── OpenTelemetry: ActivitySource para trazas ───
public class TelemetryDemo
{
    // ActivitySource: punto de entrada para crear spans (trazas)
    private static readonly ActivitySource Source = new("Cms.Api", "1.0.0");

    public static async Task RunWithTracing()
    {
        Console.WriteLine("""

        ── OPEN TELEMETRY ──

        OpenTelemetry es un estándar para recopilar trazas, métricas y logs.
        Componentes clave:

        1. ActivitySource → Crear spans (trazas)
        2. ActivityListener → Escuchar y exportar spans
        3. Meter → Crear métricas (Counter, Histogram)
        4. Exporters → Enviar a: Console, Jaeger, Zipkin, OTLP, Azure Monitor...

        Configuración en ASP.NET Core:
        ┌─────────────────────────────────────────────────────────────┐
        │ builder.Services.AddOpenTelemetry()                         │
        │     .WithTracing(tracing => tracing                         │
        │         .AddAspNetCoreInstrumentation()  // Requests HTTP   │
        │         .AddHttpClientInstrumentation()   // Llamadas HTTP  │
        │         .AddEntityFrameworkCoreInstrumentation() // EF Core │
        │         .AddConsoleExporter())            // Export a consola│
        │     .WithMetrics(metrics => metrics                         │
        │         .AddAspNetCoreInstrumentation()                     │
        │         .AddConsoleExporter());                             │
        │ ─────────────────────────────────────────────────────────  │
        │                                                             │
        │ Paquetes NuGet necesarios:                                  │
        │ - OpenTelemetry.Extensions.Hosting                          │
        │ - OpenTelemetry.Instrumentation.AspNetCore                  │
        │ - OpenTelemetry.Instrumentation.Http                        │
        │ - OpenTelemetry.Exporter.Console                            │
        │ - OpenTelemetry.Exporter.OpenTelemetryProtocol (OTLP)       │
        └─────────────────────────────────────────────────────────────┘

        ── EJEMPLO DE TRAZAS MANUALES ──
        """);

        // Crear un span raíz
        using (Activity? rootActivity = Source.StartActivity("ProcesarSolicitud"))
        {
            rootActivity?.SetTag("usuario.id", "123");
            rootActivity?.SetTag("solicitud.tipo", "Dashboard");

            Console.WriteLine("  [TRACE] Span raíz iniciado: ProcesarSolicitud");

            // Span hijo 1: Consultar base de datos
            using (Activity? dbActivity = Source.StartActivity("ConsultarBaseDeDatos"))
            {
                dbActivity?.SetTag("db.operacion", "SELECT");
                dbActivity?.SetTag("db.tabla", "Posts");
                await Task.Delay(50); // Simular consulta
                Console.WriteLine("  [TRACE]   Span hijo: ConsultarBaseDeDatos");
            }

            // Span hijo 2: Llamada a API externa
            using (Activity? apiActivity = Source.StartActivity("LlamarApiExterna"))
            {
                apiActivity?.SetTag("http.url", "https://api.ejemplo.com/datos");
                apiActivity?.SetTag("http.metodo", "GET");
                await Task.Delay(80); // Simular llamada HTTP
                apiActivity?.SetTag("http.status_code", 200);
                Console.WriteLine("  [TRACE]   Span hijo: LlamarApiExterna");
            }

            rootActivity?.SetTag("estado", "completado");
        }

        Console.WriteLine("\n  [TRACE] Todos los spans completados");
    }

    // ─── Demo de métricas ───
    public static void DemoMetrics()
    {
        Console.WriteLine("""

        ── MÉTRICAS CON METER ──

        Las métricas permiten medir valores numéricos a lo largo del tiempo:
        - Counter<T>: solo incrementa (número de peticiones, errores)
        - Histogram<T>: mide distribución (latencia, tamaño de respuesta)
        - ObservableGauge<T>: valor puntual (uso de memoria, conexiones activas)

        Ejemplo de código:
        ┌─────────────────────────────────────────────────────────────┐
        │ Meter meter = new("Cms.Api", "1.0.0");                     │
        │                                                             │
        │ // Contador de peticiones                                   │
        │ Counter<int> requestCounter = meter.CreateCounter<int>(    │
        │     "api.requests.total",                                   │
        │     description: "Total de peticiones HTTP");               │
        │                                                             │
        │ // Histograma de latencia                                   │
        │ Histogram<double> latencyHistogram = meter                  │
        │     .CreateHistogram<double>(                               │
        │     "api.latency.ms",                                       │
        │     unit: "ms",                                             │
        │     description: "Latencia de las peticiones");             │
        │                                                             │
        │ // Uso en endpoints:                                        │
        │ requestCounter.Add(1, tag: ("endpoint", "/posts"));         │
        │ latencyHistogram.Record(45.3, tag: ("endpoint", "/posts")); │
        └─────────────────────────────────────────────────────────────┘
        """);
    }
}

// ─── Programa principal ───
public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("=== MÓDULO 17 — Feature Flags + OpenTelemetry ===\n");

        // ─── Configurar Feature Flags ───
        // En una app real, esto se configura en appsettings.json
        Dictionary<string, string?> configData = new()
        {
            ["FeatureManagement:NewDashboard"] = "true",
            ["FeatureManagement:DarkMode"] = "false",
            ["FeatureManagement:ExperimentalWidgets"] = "true"
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        ServiceCollection services = new();
        services.AddSingleton(configuration);
        services.AddFeatureManagement(configuration);
        services.AddSingleton<IDashboardService, DashboardService>();
        ServiceProvider provider = services.BuildServiceProvider();

        // ─── DEMO 1: Feature Flags ───
        Console.WriteLine("── 1. FEATURE FLAGS ──\n");

        IFeatureManager featureManager = provider.GetRequiredService<IFeatureManager>();

        // Verificar flags individuales
        bool newDashboard = await featureManager.IsEnabledAsync("NewDashboard");
        bool darkMode = await featureManager.IsEnabledAsync("DarkMode");
        bool experimental = await featureManager.IsEnabledAsync("ExperimentalWidgets");

        Console.WriteLine($"  Feature 'NewDashboard': {(newDashboard ? "ACTIVADO ✅" : "DESACTIVADO ❌")}");
        Console.WriteLine($"  Feature 'DarkMode': {(darkMode ? "ACTIVADO ✅" : "DESACTIVADO ❌")}");
        Console.WriteLine($"  Feature 'ExperimentalWidgets': {(experimental ? "ACTIVADO ✅" : "DESACTIVADO ❌")}");

        // Usar flags en un servicio
        IDashboardService dashboard = provider.GetRequiredService<IDashboardService>();
        string dashboardContent = await dashboard.GetDashboardAsync();
        Console.WriteLine($"\n  Dashboard: {dashboardContent}");

        // ─── Configuración típica en appsettings.json ───
        Console.WriteLine("""

        ── CONFIGURACIÓN EN APPSETTINGS.JSON ──
        ┌─────────────────────────────────────────────────────────────┐
        │ {                                                           │
        │   "FeatureManagement": {                                     │
        │     "NewDashboard": true,         // Activar nuevo dashboard │
        │     "DarkMode": false,            // Desactivar modo oscuro  │
        │     "ExperimentalWidgets": true   // Widgets experimentales  │
        │   }                                                         │
        │ }                                                           │
        │ ─────────────────────────────────────────────────────────  │
        │                                                             │
        │ En Program.cs:                                              │
        │ builder.Services.AddFeatureManagement();                    │
        │                                                             │
        │ La configuración se lee automáticamente de la sección       │
        │ "FeatureManagement" en appsettings.json, variables de       │
        │ entorno o Azure App Configuration.                          │
        └─────────────────────────────────────────────────────────────┘
        """);

        // ─── DEMO 2: OpenTelemetry ───
        Console.WriteLine("── 2. OPEN TELEMETRY (Trazas) ──");
        await TelemetryDemo.RunWithTracing();

        // ─── DEMO 3: Métricas ───
        TelemetryDemo.DemoMetrics();

        Console.WriteLine("\n── Fin de la demo ──");

        Console.WriteLine("""

        ── RESUMEN: TRAZAS vs MÉTRICAS vs LOGS ──
        | Pilar        | ¿Qué responde?          | Herramientas              |
        |------------- |--------------------------|---------------------------|
        | Trazas       | ¿Qué pasó en esta request?| Activity, Span, Jaeger     |
        | Métricas     | ¿Cuántas requests? ¿Latencia?| Meter, Counter, Prometheus|
        | Logs         | ¿Qué mensaje se registró?  | ILogger, Serilog, Elastic  |
        """);
    }
}
