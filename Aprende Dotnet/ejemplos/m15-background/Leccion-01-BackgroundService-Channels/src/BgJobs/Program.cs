// ─── MÓDULO 15 ─── Background Jobs ───
// IHostedService / BackgroundService: servicios que corren en segundo plano
// Channel<T>: cola productor-consumidor async, thread-safe, bounded/unbounded
// Hangfire / Quartz.NET: librerías para jobs programados (cron)

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;

namespace BgJobs;

// ─── Modelo de trabajo: email a enviar ───
public record EmailJob(Guid Id, string To, string Subject, string Body, DateTime CreatedAt);

// ─── Cola de emails usando Channel<T> ───
// Channel<T> es una estructura de datos productor-consumidor integrada en .NET.
// - Unbounded: sin límite (crece indefinidamente)
// - Bounded: límite fijo (bloquea al productor si está lleno)
public interface IEmailQueue
{
    ValueTask EnqueueAsync(EmailJob job);
    IAsyncEnumerable<EmailJob> ReadAllAsync(CancellationToken ct);
}

public class EmailQueue : IEmailQueue
{
    // BoundedChannel con capacidad 100: backpressure si se llena
    private readonly Channel<EmailJob> _channel = Channel.CreateBounded<EmailJob>(
        new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait // Esperar si está lleno
        });

    // Productor: añade un trabajo a la cola
    public async ValueTask EnqueueAsync(EmailJob job)
    {
        await _channel.Writer.WriteAsync(job);
        Console.WriteLine($"  [COLA] Email encolado: {job.Id} → {job.To}");
    }

    // Consumidor: lee trabajos de la cola de forma asíncrona
    public IAsyncEnumerable<EmailJob> ReadAllAsync(CancellationToken ct)
    {
        return _channel.Reader.ReadAllAsync(ct);
    }
}

// ─── BackgroundService: procesa la cola de emails ───
// BackgroundService implementa IHostedService y maneja el ciclo de vida.
// ExecuteAsync se ejecuta al iniciar el host y se cancela al detener.
public class EmailBackgroundService : BackgroundService
{
    private readonly IEmailQueue _queue;

    public EmailBackgroundService(IEmailQueue queue)
    {
        _queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("[EMAIL SERVICE] Iniciado. Esperando emails...\n");

        // IAsyncEnumerable: consumimos trabajos de la cola conforme llegan
        await foreach (EmailJob job in _queue.ReadAllAsync(stoppingToken))
        {
            await ProcessEmailAsync(job, stoppingToken);
        }
    }

    private static async Task ProcessEmailAsync(EmailJob job, CancellationToken ct)
    {
        Console.WriteLine($"""
          ┌─────────────────────────────────────────┐
          │ [PROCESANDO] Email #{job.Id.ToString()[..8]}
          │ Para: {job.To}
          │ Asunto: {job.Subject}
          │ Cuerpo: {job.Body}
          │ Creado: {job.CreatedAt:HH:mm:ss}
          └─────────────────────────────────────────┘
          """);

        // Simular envío de email (1-2 segundos)
        await Task.Delay(Random.Shared.Next(1000, 2001), ct);

        Console.WriteLine($"  [ENVIADO] Email {job.Id.ToString()[..8]} ✅\n");
    }
}

// ─── Servicio que encola emails desde la "UI" (simulada) ───
public class EmailProducer
{
    private readonly IEmailQueue _queue;

    public EmailProducer(IEmailQueue queue)
    {
        _queue = queue;
    }

    public async Task SendWelcomeEmailAsync(string to)
    {
        EmailJob job = new(
            Id: Guid.NewGuid(),
            To: to,
            Subject: "¡Bienvenido al CMS!",
            Body: "Gracias por registrarte. Esperamos que disfrutes de la plataforma.",
            CreatedAt: DateTime.UtcNow
        );

        await _queue.EnqueueAsync(job);
    }

    public async Task SendNotificationAsync(string to, string message)
    {
        EmailJob job = new(
            Id: Guid.NewGuid(),
            To: to,
            Subject: "Nueva notificación del CMS",
            Body: message,
            CreatedAt: DateTime.UtcNow
        );

        await _queue.EnqueueAsync(job);
    }
}

// ─── Programa principal ───
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== MÓDULO 15 — Background Jobs ===\n");

        // ─── Construir el Host ───
        // Host.CreateDefaultBuilder configura logging, configuración, DI
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                // Registrar la cola como Singleton (única instancia)
                services.AddSingleton<IEmailQueue, EmailQueue>();

                // Registrar el BackgroundService (IHostedService)
                services.AddHostedService<EmailBackgroundService>();

                // Registrar el productor de emails (lo usamos manualmente)
                services.AddSingleton<EmailProducer>();
            })
            .Build();

        // ─── Iniciar el Host (arranca el BackgroundService) ───
        await host.StartAsync();

        // ─── Simular la aplicación: encolar emails ───
        EmailProducer producer = host.Services.GetRequiredService<EmailProducer>();

        Console.WriteLine("── Encolando emails ──\n");

        // Encolar varios emails (el BackgroundService los procesa en paralelo)
        await producer.SendWelcomeEmailAsync("alumno@curso.net");
        await producer.SendWelcomeEmailAsync("profe@curso.net");
        await producer.SendNotificationAsync("admin@cms.com", "Se ha publicado un nuevo post");
        await producer.SendNotificationAsync("editor@cms.com", "Tienes un comentario pendiente de moderación");
        await producer.SendWelcomeEmailAsync("visitante@web.com");

        Console.WriteLine("── Esperando a que se procesen los emails... ──\n");

        // Dar tiempo al BackgroundService para procesar la cola
        await Task.Delay(10_000);

        // ─── Parar el Host ───
        await host.StopAsync();

        Console.WriteLine("\n── Fin del programa ──");

        // ─── TEORÍA: Hangfire y Quartz.NET ───
        Console.WriteLine("""

        ── HANGFIRE ──
        Librería para ejecutar trabajos en background, programados con expresión Cron.

        Configuración en Program.cs:
        ┌─────────────────────────────────────────────────────────────┐
        │ builder.Services.AddHangfire(config => config               │
        │     .UseSqlServerStorage(connectionString));                 │
        │ builder.Services.AddHangfireServer();                       │
        │                                                             │
        │ app.UseHangfireDashboard(); // Dashboard web en /hangfire   │
        │                                                             │
        │ // Programar un trabajo recurrente (cada día a las 3 AM)    │
        │ RecurringJob.AddOrUpdate("limpiar-archivos",                │
        │     () => CleanTempFiles(), Cron.Daily(3, 0));              │
        │                                                             │
        │ // Encolar un trabajo para ejecutar inmediatamente          │
        │ BackgroundJob.Enqueue(() => SendEmail("user@test.com"));    │
        │ ─────────────────────────────────────────────────────────  │
        └─────────────────────────────────────────────────────────────┘

        ── QUARTZ.NET ──
        Alternativa más ligera y flexible a Hangfire.

        Configuración:
        ┌─────────────────────────────────────────────────────────────┐
        │ builder.Services.AddQuartz(q =>                             │
        │ {                                                           │
        │     JobKey jobKey = new("cleanup-job");                     │
        │     q.AddJob<CleanupJob>(opts => opts.WithIdentity(jobKey)); │
        │                                                             │
        │     q.AddTrigger(opts => opts                               │
        │         .ForJob(jobKey)                                     │
        │         .WithCronSchedule("0 0 3 * * ?")); // Cada día 3AM │
        │ });                                                         │
        │ builder.Services.AddQuartzHostedService();                  │
        │ ─────────────────────────────────────────────────────────  │
        │                                                             │
        │ public class CleanupJob : IJob                              │
        │ {                                                           │
        │     public Task Execute(IJobExecutionContext context)       │
        │     {                                                       │
        │         // Lógica de limpieza...                            │
        │         return Task.CompletedTask;                          │
        │     }                                                       │
        │ }                                                           │
        └─────────────────────────────────────────────────────────────┘

        ── Channel<T> vs Colas externas ──
        Channel<T> es ideal para procesamiento EN MEMORIA (un solo proceso).
        Si necesitas múltiples procesos/servidores, usa:
        - RabbitMQ / Azure Service Bus / Amazon SQS
        - Hangfire con SQL Server / Redis
        - Quartz con almacenamiento persistente
        """);
    }
}
