using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ============================================================================
// LECCIÓN 04 — Options Pattern
// ============================================================================

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║     MÓDULO 5 — Inyección de Dependencias               ║");
Console.WriteLine("║     Lección 04: Options Pattern                         ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
Console.WriteLine();

DemoIOptions();
DemoIOptionsMonitor();
DemoValidacion();
DemoNamedOptions();

Console.WriteLine("=== RESUMEN ===");
Console.WriteLine();
Console.WriteLine("1. Options Pattern: configuración tipada, validable, testeable.");
Console.WriteLine("2. IOptions<T>: snapshot, no detecta cambios.");
Console.WriteLine("3. IOptionsSnapshot<T>: por scope, se recalcula en cada ámbito.");
Console.WriteLine("4. IOptionsMonitor<T>: detecta cambios en tiempo real (OnChange).");
Console.WriteLine("5. ValidateOnStart/ValidateDataAnnotations: valida al arrancar, no en runtime.");
Console.WriteLine("6. PostConfigure: modifica opciones después de cargarlas.");
Console.WriteLine("7. Named Options: múltiples configuraciones del mismo tipo con nombres.");

return;

// ============================================================================
// MÉTODOS ESTÁTICOS
// ============================================================================

static void DemoIOptions()
{
    Console.WriteLine("=== DEMO 1: IOptions<T> (snapshot, no detecta cambios) ===");
    Console.WriteLine();

    var services = new ServiceCollection();

    services.Configure<EmailOptions>(options =>
    {
        options.SmtpServer = "smtp.gmail.com";
        options.Port = 587;
        options.Username = "mi-app@empresa.com";
        options.Password = "secreto-en-user-secrets";
        options.EnableSsl = true;
        options.FromAddress = "noreply@empresa.com";
        options.FromName = "Mi Aplicación CMS";
    });

    services.AddSingleton<EmailService>();

    var provider = services.BuildServiceProvider();

    var emailService = provider.GetRequiredService<EmailService>();
    emailService.SendEmail("cliente@email.com", "Bienvenido al CMS", "Gracias por registrarte.");

    Console.WriteLine();
    Console.WriteLine("NOTA: IOptions<T> es un snapshot. Si la configuración cambia después");
    Console.WriteLine("de que el servicio se ha resuelto, NO verá los cambios.");
    Console.WriteLine("Para detectar cambios, usa IOptionsMonitor<T>.");
    Console.WriteLine();
}

static void DemoIOptionsMonitor()
{
    Console.WriteLine("=== DEMO 2: IOptionsMonitor<T> (detecta cambios en tiempo real) ===");
    Console.WriteLine();

    var services = new ServiceCollection();

    services.Configure<RateLimitOptions>(options =>
    {
        options.RequestsPerMinute = 100;
        options.BurstLimit = 20;
        options.Enabled = true;
    });

    services.AddSingleton<RateLimitService>();

    var provider = services.BuildServiceProvider();

    var rateLimitService = provider.GetRequiredService<RateLimitService>();

    Console.WriteLine("Estado inicial:");
    rateLimitService.IsRequestAllowed();

    Console.WriteLine();
    Console.WriteLine("En producción: al modificar appsettings.json en caliente,");
    Console.WriteLine("IOptionsMonitor<T>.OnChange se dispara automáticamente.");
    Console.WriteLine("Esto permite cambiar la config sin reiniciar la app.");
    Console.WriteLine();

    Console.WriteLine("Comparación de las 3 interfaces:");
    Console.WriteLine($"  {"Interfaz",-25} {"Detección de cambios",-25} {"Uso típico"}");
    Console.WriteLine($"  {"IOptions<T>",-25} {"Snapshot (no detecta)",-25} {"Servicios Singleton"}");
    Console.WriteLine($"  {"IOptionsSnapshot<T>",-25} {"Por scope (HTTP request)",-25} {"Servicios Scoped"}");
    Console.WriteLine($"  {"IOptionsMonitor<T>",-25} {"Tiempo real (callback)",-25} {"Singleton + cambios en caliente"}");
    Console.WriteLine();
}

static void DemoValidacion()
{
    Console.WriteLine("=== DEMO 3: Validación de opciones ===");
    Console.WriteLine();

    Console.WriteLine("OPCIÓN 1: ValidateOnStart()");
    Console.WriteLine("  services.AddOptions<EmailOptions>()");
    Console.WriteLine("      .Bind(configuration.GetSection(\"Email\"))");
    Console.WriteLine("      .ValidateOnStart();  // Lanza excepción al iniciar si falta algo");
    Console.WriteLine();

    Console.WriteLine("OPCIÓN 2: ValidateDataAnnotations");
    Console.WriteLine("  [Required] public string SmtpServer { get; set; }");
    Console.WriteLine("  [Range(1, 65535)] public int Port { get; set; }");
    Console.WriteLine("  services.AddOptions<EmailOptions>()");
    Console.WriteLine("      .Bind(configuration.GetSection(\"Email\"))");
    Console.WriteLine("      .ValidateDataAnnotations()");
    Console.WriteLine("      .ValidateOnStart();");
    Console.WriteLine();

    Console.WriteLine("OPCIÓN 3: Validación personalizada");
    Console.WriteLine("  services.AddOptions<EmailOptions>()");
    Console.WriteLine("      .Bind(configuration.GetSection(\"Email\"))");
    Console.WriteLine("      .Validate(options =>");
    Console.WriteLine("      {");
    Console.WriteLine("          if (string.IsNullOrEmpty(options.SmtpServer))");
    Console.WriteLine("              return false;");
    Console.WriteLine("          return true;");
    Console.WriteLine("      });");
    Console.WriteLine();

    Console.WriteLine("OPCIÓN 4: PostConfigure (modificar después de cargar)");
    Console.WriteLine("  services.PostConfigure<EmailOptions>(options =>");
    Console.WriteLine("  {");
    Console.WriteLine("      if (options.Port == 0)");
    Console.WriteLine("          options.Port = 587;  // Valor por defecto");
    Console.WriteLine("  });");
    Console.WriteLine();
}

static void DemoNamedOptions()
{
    Console.WriteLine("=== DEMO 4: Named Options (opciones nombradas) ===");
    Console.WriteLine();

    var services = new ServiceCollection();

    services.Configure<EmailOptions>("Transactional", options =>
    {
        options.SmtpServer = "smtp.transaccional.com";
        options.Port = 587;
        options.FromAddress = "noreply@empresa.com";
        options.FromName = "CMS Notificaciones";
    });

    services.Configure<EmailOptions>("Marketing", options =>
    {
        options.SmtpServer = "smtp.marketing.com";
        options.Port = 2525;
        options.FromAddress = "newsletter@empresa.com";
        options.FromName = "CMS Newsletter";
    });

    services.AddTransient<MultiEmailService>();

    var provider = services.BuildServiceProvider();

    var multiEmail = provider.GetRequiredService<MultiEmailService>();
    multiEmail.SendTransactional("usuario@email.com", "Confirmación de pedido");
    Console.WriteLine();
    multiEmail.SendMarketing("usuario@email.com", "Nuevas ofertas esta semana");

    Console.WriteLine();
}

// ============================================================================
// TIPOS
// ============================================================================

public class EmailOptions
{
    public const string SectionName = "Email";
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}

public class RateLimitOptions
{
    public const string SectionName = "RateLimiting";
    public int RequestsPerMinute { get; set; } = 100;
    public int BurstLimit { get; set; } = 20;
    public bool Enabled { get; set; } = true;
}

public class EmailService
{
    private readonly EmailOptions _options;

    public EmailService(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public void SendEmail(string to, string subject, string body)
    {
        Console.WriteLine($"[EmailService] Conectando a {_options.SmtpServer}:{_options.Port}...");
        Console.WriteLine($"  SSL: {_options.EnableSsl}");
        Console.WriteLine($"  Usuario: {_options.Username}");
        Console.WriteLine($"  From: {_options.FromName} <{_options.FromAddress}>");
        Console.WriteLine($"  Para: {to}");
        Console.WriteLine($"  Asunto: {subject}");
        Console.WriteLine("  Email enviado (simulado).");
    }
}

public class RateLimitService
{
    private RateLimitOptions _current;
    private readonly IDisposable? _onChangeSubscription;

    public RateLimitService(IOptionsMonitor<RateLimitOptions> monitor)
    {
        _current = monitor.CurrentValue;

        _onChangeSubscription = monitor.OnChange((newOptions, _) =>
        {
            Console.WriteLine();
            Console.WriteLine($"[OptionsMonitor] ¡Configuración cambiada!");
            Console.WriteLine($"  RequestsPerMinute: {_current.RequestsPerMinute} → {newOptions.RequestsPerMinute}");
            Console.WriteLine($"  Enabled: {_current.Enabled} → {newOptions.Enabled}");
            _current = newOptions;
        });

        Console.WriteLine("[OptionsMonitor] Suscripción a cambios activada.");
    }

    public bool IsRequestAllowed()
    {
        if (!_current.Enabled)
        {
            Console.WriteLine("  Rate limiting: DESACTIVADO");
            return true;
        }

        Console.WriteLine($"  Rate limiting: ACTIVO (máx {_current.RequestsPerMinute} req/min, burst {_current.BurstLimit})");
        return true;
    }
}

public class MultiEmailService
{
    private readonly EmailOptions _transactional;
    private readonly EmailOptions _marketing;

    public MultiEmailService(IOptionsSnapshot<EmailOptions> optionsSnapshot)
    {
        _transactional = optionsSnapshot.Get("Transactional");
        _marketing = optionsSnapshot.Get("Marketing");
    }

    public void SendTransactional(string to, string subject)
    {
        Console.WriteLine($"  [Transactional] From: {_transactional.FromName} <{_transactional.FromAddress}>");
        Console.WriteLine($"  [Transactional] SMTP: {_transactional.SmtpServer}:{_transactional.Port}");
        Console.WriteLine($"  [Transactional] Enviando '{subject}' a {to}");
    }

    public void SendMarketing(string to, string subject)
    {
        Console.WriteLine($"  [Marketing] From: {_marketing.FromName} <{_marketing.FromAddress}>");
        Console.WriteLine($"  [Marketing] SMTP: {_marketing.SmtpServer}:{_marketing.Port}");
        Console.WriteLine($"  [Marketing] Enviando '{subject}' a {to}");
    }
}
