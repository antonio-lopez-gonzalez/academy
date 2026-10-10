// ====================================================================
// LECCIÓN 04-06 — TimeProvider, Keyed Services, HttpClientFactory
// C# Avanzado — Abstracciones para testing y DI en .NET 8
// ====================================================================
// ¿Qué cubre esta lección?
//   1. TimeProvider (.NET 8): Abstracción del tiempo para hacerlo testeable
//   2. Keyed Services  (.NET 8): Misma interfaz, múltiples implementaciones por clave
//   3. HttpClientFactory:   Gestión profesional del ciclo de vida de HttpClient
//
// NOTA: Esta lección es conceptual. Keyed Services e HttpClientFactory
//       requieren el contenedor de DI, que se estudia en el Módulo 5.
//       Aquí mostramos cómo se usarían, con ejemplos ejecutables donde sea
//       posible y comentarios de código para el resto.
//
// ¿Por qué existen?
//   - TimeProvider: DateTime.UtcNow/DateTimeOffset.Now son estáticos →
//     imposibles de mockear en tests. TimeProvider es inyectable y mockeable.
//   - Keyed Services: Antes, para elegir entre varias implementaciones de
//     una interfaz, necesitabas factories manuales o patrones engorrosos.
//   - HttpClientFactory: new HttpClient() en producción → agota sockets,
//     no maneja DNS changes, y complica la gestión de políticas.
// ====================================================================

using System.Diagnostics;

namespace CSharpTimeProvider;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║   TimeProvider, Keyed Services, HttpClientFactory    ║");
        Console.WriteLine("║   Abstracciones profesionales en .NET 8              ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine();

        DemostrarProblemaConDateTimeUtcNow();
        DemostrarTimeProvider();
        DemostrarFakeTimeProvider();
        DemostrarKeyedServicesConceptual();
        DemostrarProblemaConNewHttpClient();
        DemostrarHttpClientFactoryConceptual();
    }

    // ─── 1. El problema con DateTime.UtcNow ───────────────────────────
    // DateTime.UtcNow (y DateTimeOffset.Now) son propiedades estáticas.
    // Esto significa que NO se pueden mockear en tests.
    // Si tu lógica de negocio depende de la hora actual, no puedes probar
    // escenarios como "qué pasa a las 3 AM" sin esperar a las 3 AM de verdad.
    private static void DemostrarProblemaConDateTimeUtcNow()
    {
        Console.WriteLine("─── 1. El problema: DateTime.UtcNow es estático ───\n");

        ServicioAlertaClasico alerta = new ServicioAlertaClasico();

        // En producción: depende de DateTime.UtcNow real
        string estado = alerta.ObtenerEstado();
        Console.WriteLine($"  Estado real:   {estado}");

        // En tests: ¿cómo pruebas el caso nocturno?
        //   - NO puedes mockear DateTime.UtcNow
        //   - NO puedes cambiar el reloj del sistema
        //   - SOLO puedes esperar a que sea la hora correcta (inviable)
        Console.WriteLine("  ❌ En tests, DateTime.UtcNow no se puede mockear.");
        Console.WriteLine("  ❌ Probar escenarios nocturnos es imposible sin abstracción.\n");
    }

    // ─── 2. TimeProvider: la solución (.NET 8) ────────────────────────
    // TimeProvider es una clase abstracta en System (incluida en .NET 8).
    // Proporciona GetUtcNow(), GetLocalNow(), CreateTimer(), etc.
    //
    // En producción usas TimeProvider.System (el reloj real).
    // En tests inyectas un FakeTimeProvider que devuelve la hora que quieras.
    private static void DemostrarTimeProvider()
    {
        Console.WriteLine("─── 2. TimeProvider: abstracción del tiempo ───\n");

        // ── En producción: se inyecta TimeProvider.System ──
        ServicioAlertaModerno servicioProd = new ServicioAlertaModerno(TimeProvider.System);
        Console.WriteLine($"  Con TimeProvider.System: {servicioProd.ObtenerEstado()}");

        // ── Métodos clave de TimeProvider ──
        // TimeProvider.System.GetUtcNow()    → DateTimeOffset (UTC)
        // TimeProvider.System.GetLocalNow()  → DateTimeOffset (local)
        // TimeProvider.System.GetTimestamp() → long (alta precisión, para medir)

        DateTimeOffset ahoraUtc = TimeProvider.System.GetUtcNow();
        DateTimeOffset ahoraLocal = TimeProvider.System.GetLocalNow();
        long timestamp = TimeProvider.System.GetTimestamp();

        Console.WriteLine($"  GetUtcNow():    {ahoraUtc:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"  GetLocalNow():  {ahoraLocal:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"  GetTimestamp(): {timestamp}");
        Console.WriteLine("  ℹ️  TimeProvider reemplaza DateTime.UtcNow, DateTimeOffset.Now, etc.\n");
    }

    // ─── 3. FakeTimeProvider para tests ──────────────────────────────
    // Creamos un proveedor de tiempo falso que permite controlar la hora.
    // En un proyecto real usarías Microsoft.Extensions.TimeProvider.Testing
    // (NuGet: Microsoft.Extensions.TimeProvider.Testing).
    private static void DemostrarFakeTimeProvider()
    {
        Console.WriteLine("─── 3. FakeTimeProvider: controlar el tiempo en tests ───\n");

        // ── Simulamos que es de día (10:00 AM) ──
        FakeTimeProvider tiempoDiurno = new FakeTimeProvider(
            new DateTimeOffset(2026, 7, 29, 10, 0, 0, TimeSpan.Zero));

        ServicioAlertaModerno servicioDia = new ServicioAlertaModerno(tiempoDiurno);
        Console.WriteLine($"  Hora simulada: 10:00 AM -> {servicioDia.ObtenerEstado()}");

        // ── Simulamos que es de noche (03:00 AM) ──
        FakeTimeProvider tiempoNocturno = new FakeTimeProvider(
            new DateTimeOffset(2026, 7, 29, 3, 0, 0, TimeSpan.Zero));

        ServicioAlertaModerno servicioNoche = new ServicioAlertaModerno(tiempoNocturno);
        Console.WriteLine($"  Hora simulada: 03:00 AM -> {servicioNoche.ObtenerEstado()}");

        // ── Avanzar el tiempo manualmente ──
        Console.WriteLine($"\n  Hora inicial: {tiempoDiurno.GetUtcNow():HH:mm}");
        tiempoDiurno.Adelantar(TimeSpan.FromHours(5));
        Console.WriteLine($"  Tras adelantar 5h: {tiempoDiurno.GetUtcNow():HH:mm}");
        Console.WriteLine($"  Nuevo estado: {servicioDia.ObtenerEstado()}");

        Console.WriteLine("  ✅ Con TimeProvider, cualquier escenario temporal es testeable.\n");
    }

    // ─── 4. Keyed Services: concepto ──────────────────────────────────
    // Keyed Services (.NET 8) permite registrar múltiples implementaciones
    // de la misma interfaz, diferenciadas por una clave (string o enum).
    //
    // Ejemplo: tienes INotificationSender con EmailSender, SmsSender,
    // PushSender. Según el contexto, necesitas una u otra.
    //
    // NOTA: Esto usa el contenedor de DI real (IServiceCollection),
    //       que se estudia en el Módulo 5. Aquí vemos el concepto
    //       con código comentado de cómo se haría en DI.
    private static void DemostrarKeyedServicesConceptual()
    {
        Console.WriteLine("─── 4. Keyed Services: misma interfaz, múltiples implementaciones ───\n");

        // ============================================================
        // SIN Keyed Services (antes de .NET 8)
        // ============================================================
        Console.WriteLine("  [ANTES] Sin Keyed Services: factory manual con switch\n");

        // Antes de .NET 8, necesitabas un factory manual:
        NotificationSenderFactory factory = new();
        INotificationSender senderEmail = factory.CreateSender("email");
        INotificationSender senderSms = factory.CreateSender("sms");

        senderEmail.Send("admin@empresa.com", "Bienvenido al sistema");
        senderSms.Send("+34600000000", "Tu código de verificación es 123456");

        Console.WriteLine();

        // ============================================================
        // CON Keyed Services (cómo se haría en DI)
        // ============================================================
        Console.WriteLine("  [AHORA] Con Keyed Services (.NET 8):\n");

        Console.WriteLine("  // Registro en Program.cs (Módulo 5 - DI):");
        Console.WriteLine("  // builder.Services.AddKeyedSingleton<INotificationSender, EmailSender>(\"email\");");
        Console.WriteLine("  // builder.Services.AddKeyedSingleton<INotificationSender, SmsSender>(\"sms\");");
        Console.WriteLine("  // builder.Services.AddKeyedSingleton<INotificationSender, PushSender>(\"push\");");
        Console.WriteLine();
        Console.WriteLine("  // Consumo en un controlador/servicio:");
        Console.WriteLine("  // class AlertController(");
        Console.WriteLine("  //   [FromKeyedServices(\"email\")] INotificationSender email,  <- inyecta EmailSender");
        Console.WriteLine("  //   [FromKeyedServices(\"sms\")]   INotificationSender sms     <- inyecta SmsSender");
        Console.WriteLine("  // )");
        Console.WriteLine();
        Console.WriteLine("  // O desde IServiceProvider manualmente:");
        Console.WriteLine("  // var sender = provider.GetKeyedService<INotificationSender>(\"email\");");

        Console.WriteLine("\n  ℹ️  [FromKeyedServices] es un atributo de .NET 8.");
        Console.WriteLine("  ℹ️  La clave puede ser string, int, enum... lo que necesites.");
        Console.WriteLine("  ℹ️  Elimina factories manuales y hace el código más limpio.\n");
    }

    // ─── 5. El problema con new HttpClient() ──────────────────────────
    // Crear HttpClient manualmente en cada petición es un error clásico
    // en .NET que puede tumbar tu aplicación en producción.
    private static void DemostrarProblemaConNewHttpClient()
    {
        Console.WriteLine("─── 5. El problema: new HttpClient() ───\n");

        Console.WriteLine("  ❌ PROBLEMA 1: Agotamiento de sockets (socket exhaustion)");
        Console.WriteLine("     Cada new HttpClient() crea un nuevo HttpClientHandler.");
        Console.WriteLine("     Cada handler abre un socket TCP nuevo.");
        Console.WriteLine("     Al hacer Dispose(), el socket entra en TIME_WAIT (~240s en Windows).");
        Console.WriteLine("     Resultado: con tráfico alto, el SO se queda sin sockets.");
        Console.WriteLine();

        Console.WriteLine("  ❌ PROBLEMA 2: DNS no se actualiza");
        Console.WriteLine("     Cada HttpClientHandler resuelve DNS al crearse.");
        Console.WriteLine("     Si usas un singleton HttpClient, el DNS se resuelve");
        Console.WriteLine("     una vez y nunca se actualiza.");
        Console.WriteLine("     Si el backend cambia de IP, tu app sigue apuntando a la antigua.");
        Console.WriteLine();

        Console.WriteLine("  // ❌ CÓDIGO PELIGROSO (nunca hagas esto en producción):");
        Console.WriteLine("  // using HttpClient client = new();");
        Console.WriteLine("  // var response = await client.GetAsync(\"https://api.example.com\");");
        Console.WriteLine();

        Console.WriteLine("  // ❌ TAMBIÉN PELIGROSO (singleton manual):");
        Console.WriteLine("  // static readonly HttpClient client = new();");
        Console.WriteLine("  // Soluciona sockets pero no actualiza DNS nunca.\n");
    }

    // ─── 6. HttpClientFactory: la solución ────────────────────────────
    // HttpClientFactory gestiona el ciclo de vida de HttpClientHandler
    // internamente, resolviendo ambos problemas.
    private static void DemostrarHttpClientFactoryConceptual()
    {
        Console.WriteLine("─── 6. HttpClientFactory: gestión profesional de HttpClient ───\n");

        Console.WriteLine("  ✅ SOLUCIÓN: IHttpClientFactory");
        Console.WriteLine("     - Pooling de HttpClientHandler: reutiliza sockets.");
        Console.WriteLine("     - Rotación periódica de handlers: DNS se actualiza.");
        Console.WriteLine("     - Políticas de resiliencia integrables (Polly).");
        Console.WriteLine("     - Centraliza la configuración de BaseAddress, headers, timeouts.");
        Console.WriteLine();

        // ============================================================
        // Named Clients: registras un cliente con nombre
        // ============================================================
        Console.WriteLine("  [Named Client] Registro en DI:");
        Console.WriteLine("  // builder.Services.AddHttpClient(\"GitHub\", client =>");
        Console.WriteLine("  // {");
        Console.WriteLine("  //     client.BaseAddress = new Uri(\"https://api.github.com/\");");
        Console.WriteLine("  //     client.DefaultRequestHeaders.Add(\"Accept\", \"application/json\");");
        Console.WriteLine("  //     client.Timeout = TimeSpan.FromSeconds(30);");
        Console.WriteLine("  // });");
        Console.WriteLine();

        Console.WriteLine("  // Consumo del named client:");
        Console.WriteLine("  // var factory = provider.GetRequiredService<IHttpClientFactory>();");
        Console.WriteLine("  // HttpClient ghClient = factory.CreateClient(\"GitHub\");");
        Console.WriteLine("  // var response = await ghClient.GetAsync(\"repos/dotnet/runtime\");");
        Console.WriteLine();

        // ============================================================
        // Typed Clients: tu propia clase recibe HttpClient
        // ============================================================
        Console.WriteLine("  [Typed Client] Tu clase recibe HttpClient por constructor:\n");

        Console.WriteLine("  // Registro en DI:");
        Console.WriteLine("  // builder.Services.AddHttpClient<GitHubService>(client =>");
        Console.WriteLine("  // {");
        Console.WriteLine("  //     client.BaseAddress = new Uri(\"https://api.github.com/\");");
        Console.WriteLine("  //     client.DefaultRequestHeaders.Add(\"User-Agent\", \"MiApp\");");
        Console.WriteLine("  // });");
        Console.WriteLine();

        Console.WriteLine("  // La clase recibe HttpClient automáticamente:");

        // Simulamos el concepto sin DI real:
        // En producción, el contenedor DI crea y configura el HttpClient.
        // Aquí lo hacemos manual para entender el concepto.
        using HttpClient httpClientManual = new();
        httpClientManual.BaseAddress = new Uri("https://api.github.com/");
        httpClientManual.DefaultRequestHeaders.Add("User-Agent", "CursoDotNet-Demo");
        httpClientManual.Timeout = TimeSpan.FromSeconds(10);

        ClienteTipado githubService = new ClienteTipado(httpClientManual);
        Console.WriteLine($"  Cliente tipado creado: BaseAddress = {githubService.ObtenerBaseAddress()}");
        Console.WriteLine("  ℹ️  El framework inyecta HttpClient en ClienteTipado automáticamente.");
        Console.WriteLine("  ℹ️  HttpClientHandler se gestiona por el factory (pooling).");
        Console.WriteLine("  ℹ️  Al llamar a Dispose(), el handler no se destruye → vuelve al pool.");
        Console.WriteLine();

        // ============================================================
        // Resumen comparativo
        // ============================================================
        Console.WriteLine("  ┌─────────────────────────┬──────────────────┬──────────────────────┐");
        Console.WriteLine("  │ Estrategia              │ Sockets          │ DNS                  │");
        Console.WriteLine("  ├─────────────────────────┼──────────────────┼──────────────────────┤");
        Console.WriteLine("  │ new HttpClient() c/u    │ ❌ Agotamiento   │ ✅ Actualizado       │");
        Console.WriteLine("  │ HttpClient singleton    │ ✅ Reutiliza     │ ❌ Stale             │");
        Console.WriteLine("  │ IHttpClientFactory      │ ✅ Pooling       │ ✅ Rotación periódica│");
        Console.WriteLine("  └─────────────────────────┴──────────────────┴──────────────────────┘");

        Console.WriteLine("\n  🏆 IHttpClientFactory: lo mejor de ambos mundos.\n");
    }
}

// ====================================================================
// CLASES DE EJEMPLO: TimeProvider
// ====================================================================

// ── Servicio SIN abstracción (usa DateTime.UtcNow) ─────────────────
// Problema: imposible de testear para escenarios temporales.
public class ServicioAlertaClasico
{
    public string ObtenerEstado()
    {
        int hora = DateTime.UtcNow.Hour;

        if (hora >= 6 && hora < 22)
            return "Modo diurno: alertas activas";

        return "Modo nocturno: solo alertas críticas";
    }
}

// ── Servicio CON TimeProvider (testeable) ───────────────────────────
// Recibe TimeProvider por constructor (inyección de dependencias).
// En producción: TimeProvider.System.
// En tests: FakeTimeProvider.
public class ServicioAlertaModerno
{
    private readonly TimeProvider _timeProvider;

    // El contenedor DI inyecta TimeProvider.System automáticamente
    // si registras: builder.Services.AddSingleton(TimeProvider.System);
    public ServicioAlertaModerno(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public string ObtenerEstado()
    {
        int hora = _timeProvider.GetUtcNow().Hour;

        if (hora >= 6 && hora < 22)
            return "Modo diurno: alertas activas";

        return "Modo nocturno: solo alertas críticas";
    }

    // También podemos usar TimeProvider para medir intervalos
    public TimeSpan MedirTiempoTranscurrido(long timestampInicio)
    {
        long ahora = _timeProvider.GetTimestamp();
        return _timeProvider.GetElapsedTime(timestampInicio, ahora);
    }
}

// ── FakeTimeProvider para tests ─────────────────────────────────────
// En un proyecto real usarías Microsoft.Extensions.TimeProvider.Testing
// de NuGet. Esta versión simplificada muestra el concepto.
public class FakeTimeProvider : TimeProvider
{
    private DateTimeOffset _tiempoActual;

    public FakeTimeProvider(DateTimeOffset tiempoInicial)
    {
        _tiempoActual = tiempoInicial;
    }

    // Sobrescribimos GetUtcNow para devolver el tiempo que queramos
    public override DateTimeOffset GetUtcNow()
    {
        return _tiempoActual;
    }

    // Sobrescribimos LocalTimeZone para devolver UTC
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;

    // Método helper para tests: avanzar el tiempo manualmente
    public void Adelantar(TimeSpan intervalo)
    {
        _tiempoActual = _tiempoActual.Add(intervalo);
    }

    // Método helper: establecer una hora concreta
    public void EstablecerHora(DateTimeOffset nuevaHora)
    {
        _tiempoActual = nuevaHora;
    }
}

// ====================================================================
// CLASES DE EJEMPLO: Keyed Services (conceptual, sin DI real)
// ====================================================================

// ── Interfaz común para envío de notificaciones ────────────────────
public interface INotificationSender
{
    void Send(string destination, string message);
}

// ── Implementaciones concretas ─────────────────────────────────────
public class EmailSender : INotificationSender
{
    public void Send(string destination, string message)
    {
        Console.WriteLine($"     [EMAIL]  Para: {destination} | Asunto: {message}");
    }
}

public class SmsSender : INotificationSender
{
    public void Send(string destination, string message)
    {
        Console.WriteLine($"     [SMS]    Para: {destination} | Texto: {message}");
    }
}

public class PushSender : INotificationSender
{
    public void Send(string destination, string message)
    {
        Console.WriteLine($"     [PUSH]   Token: {destination} | Mensaje: {message}");
    }
}

// ── Factory manual (lo que Keyed Services evita) ───────────────────
// Antes de .NET 8, esto era necesario para elegir la implementación.
// Keyed Services elimina esta clase por completo.
public class NotificationSenderFactory
{
    public INotificationSender CreateSender(string type)
    {
        return type switch
        {
            "email" => new EmailSender(),
            "sms"   => new SmsSender(),
            "push"  => new PushSender(),
            _       => throw new ArgumentException($"Tipo no soportado: {type}")
        };
    }
}

// ====================================================================
// CLASES DE EJEMPLO: HttpClientFactory (conceptual, sin DI real)
// ====================================================================

// ── Typed Client: clase que recibe HttpClient en constructor ────────
// En producción, el contenedor DI inyecta un HttpClient configurado.
// La clase no sabe (ni necesita saber) de dónde viene el HttpClient.
public class ClienteTipado
{
    private readonly HttpClient _httpClient;

    // HttpClient se inyecta por constructor.
    // En producción lo hace el contenedor DI automáticamente.
    // Nosotros lo simulamos manual para entender el concepto.
    public ClienteTipado(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Método de ejemplo que haría una petición real.
    // En este demo, solo devolvemos información de configuración.
    public string ObtenerBaseAddress()
    {
        return _httpClient.BaseAddress?.ToString() ?? "(no configurado)";
    }

    // Ejemplo de cómo se consumiría en producción:
    // public async Task<string> ObtenerInfoAsync()
    // {
    //     HttpResponseMessage response = await _httpClient.GetAsync("/repos/dotnet/runtime");
    //     response.EnsureSuccessStatusCode();
    //     return await response.Content.ReadAsStringAsync();
    // }
}

// ====================================================================
// RESUMEN DE LA LECCIÓN
// ====================================================================
// 1. TimeProvider (.NET 8): Abstracción del tiempo, inyectable y mockeable
//    - TimeProvider.System = reloj real (producción)
//    - FakeTimeProvider = reloj controlable (tests)
//    - Reemplaza: DateTime.UtcNow, DateTimeOffset.Now, Stopwatch
//    - Permite testear escenarios temporales sin esperar en tiempo real
//
// 2. Keyed Services (.NET 8): Misma interfaz, múltiples implementaciones
//    - [FromKeyedServices("clave")] en el constructor
//    - Elimina factories manuales y patrones de resolución complejos
//    - Clave puede ser string, int, enum, etc.
//    - Se registra con AddKeyedSingleton/Scoped/Transient
//
// 3. HttpClientFactory: Gestión profesional del ciclo de vida de HttpClient
//    - Pooling de HttpClientHandler (evita socket exhaustion)
//    - Rotación periódica de handlers (DNS se mantiene actualizado)
//    - Named Clients: configuración centralizada por nombre
//    - Typed Clients: inyección directa de HttpClient en tus clases
//    - Integrable con Polly para resiliencia (retries, circuit breaker)
//
// Cuándo usar cada uno:
//   - TimeProvider: Siempre que tu código dependa de la hora actual
//   - Keyed Services: Cuando tengas varias implementaciones de una interfaz
//     y necesites elegir cuál inyectar según el contexto
//   - HttpClientFactory: SIEMPRE que hagas llamadas HTTP en producción.
//     NUNCA uses new HttpClient() en aplicaciones reales.
// ====================================================================
