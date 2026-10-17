/*
 * ============================================================================
 * MÓDULO 3 — SOLID en C#                                        Lección 05
 * ============================================================================
 * PRINCIPIO DE INVERSIÓN DE DEPENDENCIAS (Dependency Inversion Principle)
 * ============================================================================
 *
 * DEFINICIÓN (Robert C. Martin):
 *
 *   1. "Los módulos de alto nivel NO deben depender de módulos de bajo nivel.
 *       AMBOS deben depender de abstracciones."
 *
 *   2. "Las abstracciones NO deben depender de los detalles.
 *       LOS DETALLES deben depender de las abstracciones."
 *
 * ──────────────────────────────────────────────────────────────────────────
 * ¿QUÉ SIGNIFICA ESTO?
 * ──────────────────────────────────────────────────────────────────────────
 *
 * Imagina una jerarquía clásica de capas:
 *
 *          ┌─────────────────┐
 *          │  Capa de alto    │  ← Lógica de negocio (NotificationService)
 *          │  nivel           │
 *          ├─────────────────┤
 *          │  Capa de bajo    │  ← Detalles (EmailSender, SqlServerDatabase)
 *          │  nivel           │
 *          └─────────────────┘
 *
 * SIN DIP (VIOLACIÓN):
 *   NotificationService ──► EmailSender (dependencia directa)
 *   NotificationService conoce detalles concretos → ACOPLAMIENTO FUERTE
 *
 * CON DIP (CORRECTO):
 *   NotificationService ──► INotificationSender (abstracción)
 *   EmailSender ──► INotificationSender (detalle depende de abstracción)
 *
 *   La flecha de dependencia en código fuente APUNTA HACIA ARRIBA,
 *   hacia las abstracciones. Los detalles "invierten" su dependencia.
 *
 * ──────────────────────────────────────────────────────────────────────────
 * ¿POR QUÉ ES LA PUERTA A LA INYECCIÓN DE DEPENDENCIAS?
 * ──────────────────────────────────────────────────────────────────────────
 *
 *   DIP  → Principio       (qué hacer: depende de abstracciones)
 *   IoC  → Mecanismo       (cómo se hace: el control se invierte)
 *   DI   → Patrón          (implementación concreta: constructor injection)
 *
 *   "No me llames, yo te llamo"  — Hollywood Principle
 *
 *   Sin DIP, la DI no tendría sentido. DIP es el "por qué", DI es el "cómo".
 *
 * ──────────────────────────────────────────────────────────────────────────
 * CONSECUENCIAS DE VIOLAR DIP
 * ──────────────────────────────────────────────────────────────────────────
 *
 *   ❌ Imposible cambiar implementaciones (ej: Email → SMS → Push).
 *   ❌ Imposible testear unitariamente (dependencia hardcodeada).
 *   ❌ Cada cambio en el detalle fuerza cambiar el módulo de alto nivel.
 *   ❌ Violación del OCP (no se puede extender sin modificar).
 *
 * ──────────────────────────────────────────────────────────────────────────
 * BENEFICIOS DE CUMPLIR DIP
 * ──────────────────────────────────────────────────────────────────────────
 *
 *   ✅ Puedes cambiar la base de datos sin tocar la lógica de negocio.
 *   ✅ Puedes testear con mocks sin infraestructura real.
 *   ✅ El código de alto nivel solo conoce contratos (interfaces).
 *   ✅ Nuevas implementaciones se añaden sin modificar código existente (OCP).
 *   ✅ La dirección de las dependencias sigue la regla de Clean Architecture:
 *      lógica de negocio ← abstracciones ← infraestructura.
 */

namespace SolidDIP;

// ╔═══════════════════════════════════════════════════════════════════════════╗
// ║                       SECCIÓN 1 — VIOLACIÓN DIP                          ║
// ║                Notificaciones acopladas a implementaciones concretas      ║
// ╚═══════════════════════════════════════════════════════════════════════════╝

/// <summary>
/// Implementación concreta de bajo nivel: envía emails.
/// En un sistema real usaría SMTP, SendGrid, etc.
/// </summary>
public class EmailSender
{
    public void Send(string to, string message)
    {
        Console.WriteLine($"[EMAIL] Enviando a {to}: {message}");
    }
}

/// <summary>
/// Otra implementación concreta de bajo nivel: envía SMS.
/// </summary>
public class SmsSender
{
    public void Send(string phone, string message)
    {
        Console.WriteLine($"[SMS] Enviando a {phone}: {message}");
    }
}

/// <summary>
/// VIOLACIÓN: NotificationService depende DIRECTAMENTE de EmailSender.
/// La clase de alto nivel conoce una clase concreta de bajo nivel.
///
/// Problemas:
///   - Si quiero usar SMS, tengo que MODIFICAR esta clase.
///   - No puedo testear sin un servidor SMTP real.
///   - Cada nuevo canal de notificación obliga a reescribir esta clase.
/// </summary>
public class NotificationServiceViolation
{
    // Dependencia dura: la clase CREA su propia dependencia.
    private readonly EmailSender _emailSender = new EmailSender();

    public void Notify(string recipient, string message)
    {
        // No hay opción: siempre usa Email.
        // Si mañana hay que enviar SMS, hay que cambiar este código.
        _emailSender.Send(recipient, message);
    }
}

// ╔═══════════════════════════════════════════════════════════════════════════╗
// ║               SECCIÓN 2 — VIOLACIÓN DIP en acceso a datos                ║
// ║                 Capa de servicio acoplada a una BD concreta              ║
// ╚═══════════════════════════════════════════════════════════════════════════╝

/// <summary>
/// Implementación concreta: SQL Server.
/// El servicio de alto nivel no debería saber qué motor de BD se usa.
/// </summary>
public class SqlServerDatabase
{
    public void Save(string data)
    {
        Console.WriteLine($"[SQL Server] Guardando: {data}");
        // Aquí irían: SqlConnection, SqlCommand, parámetros, etc.
    }

    public string Get(int id)
    {
        Console.WriteLine($"[SQL Server] Leyendo registro {id}");
        return $"Registro-{id}";
    }
}

/// <summary>
/// VIOLACIÓN: UserService depende de SqlServerDatabase.
/// Si quiero migrar a PostgreSQL tengo que tocar UserService.
/// Para testear necesito un SQL Server real.
/// </summary>
public class UserServiceViolation
{
    // Hardcodeado: UserService "sabe" que existe SqlServerDatabase.
    private readonly SqlServerDatabase _database = new SqlServerDatabase();

    public void CreateUser(string name)
    {
        _database.Save($"Usuario: {name}");
    }

    public string GetUser(int id)
    {
        return _database.Get(id);
    }
}

// ╔═══════════════════════════════════════════════════════════════════════════╗
// ║                     SECCIÓN 3 — SOLUCIÓN DIP                             ║
// ║                 Crear abstracciones e invertir dependencias               ║
// ╚═══════════════════════════════════════════════════════════════════════════╝

// --- Abstracciones (interfaces) ---
// Estas interfaces VIVEN en la capa de alto nivel (lógica de negocio).
// Son CONTRATOS: definen QUÉ se puede hacer, no CÓMO se hace.
// Los detalles (EmailSender, SmsSender, SqlServerDatabase) las IMPLEMENTAN.

/// <summary>
/// Abstracción: cualquier mecanismo que pueda enviar una notificación.
/// Esta interfaz pertenece a la lógica de negocio, NO a infraestructura.
/// </summary>
public interface INotificationSender
{
    void Send(string to, string message);
}

/// <summary>
/// Abstracción: cualquier sistema de persistencia.
/// La lógica de negocio depende de ESTE contrato, no de SQL Server directamente.
/// </summary>
public interface IDatabase
{
    void Save(string data);
    string Get(int id);
}

// --- Implementaciones concretas que dependen de las abstracciones ---
// Fíjate: las implementaciones concretas AHORA dependen de interfaces.
// Es decir, la flecha de dependencia en el código fuente se ha INVERTIDO.

/// <summary>
/// Implementación concreta de bajo nivel para email.
/// IMPLEMENTA la abstracción INotificationSender definida arriba.
/// </summary>
public class EmailSenderDIP : INotificationSender
{
    public void Send(string to, string message)
    {
        Console.WriteLine($"[EMAIL] Enviando a {to}: {message}");
    }
}

/// <summary>
/// Implementación concreta de bajo nivel para SMS.
/// </summary>
public class SmsSenderDIP : INotificationSender
{
    public void Send(string to, string message)
    {
        Console.WriteLine($"[SMS] Enviando a {to}: {message}");
    }
}

/// <summary>
/// Implementación concreta para SQL Server.
/// IMPLEMENTA la abstracción IDatabase.
/// </summary>
public class SqlServerDatabaseDIP : IDatabase
{
    public void Save(string data)
    {
        Console.WriteLine($"[SQL Server] Guardando: {data}");
    }

    public string Get(int id)
    {
        Console.WriteLine($"[SQL Server] Leyendo registro {id}");
        return $"Registro-{id}";
    }
}

/// <summary>
/// Segunda implementación concreta: PostgreSQL.
/// Misma abstracción IDatabase, distinta tecnología.
/// </summary>
public class PostgreSqlDatabaseDIP : IDatabase
{
    public void Save(string data)
    {
        Console.WriteLine($"[PostgreSQL] Guardando: {data}");
    }

    public string Get(int id)
    {
        Console.WriteLine($"[PostgreSQL] Leyendo registro {id}");
        return $"Registro-{id}";
    }
}

// --- Módulos de alto nivel que ahora dependen de abstracciones ---

/// <summary>
/// CORRECTO: NotificationService depende de INotificationSender (abstracción).
/// NO conoce EmailSender ni SmsSender. Solo conoce el contrato.
///
/// Recibe la dependencia por CONSTRUCTOR (Inyección por Constructor).
/// </summary>
public class NotificationService
{
    // Dependencia hacia la abstracción, no hacia la implementación.
    private readonly INotificationSender _sender;

    /// <summary>
    /// La dependencia se INYECTA desde fuera.
    /// NotificationService no crea nada, solo recibe lo que necesita.
    /// </summary>
    public NotificationService(INotificationSender sender)
    {
        _sender = sender;
    }

    public void Notify(string recipient, string message)
    {
        _sender.Send(recipient, message);
    }
}

/// <summary>
/// CORRECTO: UserService depende de IDatabase (abstracción).
/// Puede recibir SqlServerDatabase, PostgreSqlDatabase o un mock para tests.
/// </summary>
public class UserService
{
    private readonly IDatabase _database;

    public UserService(IDatabase database)
    {
        _database = database;
    }

    public void CreateUser(string name)
    {
        _database.Save($"Usuario: {name}");
    }

    public string GetUser(int id)
    {
        return _database.Get(id);
    }
}

// ╔═══════════════════════════════════════════════════════════════════════════╗
// ║               SECCIÓN 4 — MOCK para TESTING (gracias a DIP)              ║
// ║          Sin DIP sería imposible testear sin infraestructura real         ║
// ╚═══════════════════════════════════════════════════════════════════════════╝

/// <summary>
/// Mock de notificación: implementa INotificationSender sin enviar nada real.
/// Registra las llamadas para poder verificarlas en tests.
/// Gracias a DIP, podemos INYECTAR este mock en lugar del EmailSender real.
/// </summary>
public class MockNotificationSender : INotificationSender
{
    // Registro público para que los tests puedan verificar qué se envió.
    public List<(string To, string Message)> SentMessages { get; } = new();

    public void Send(string to, string message)
    {
        Console.WriteLine($"[MOCK] Simulando envío a {to}: {message}");
        SentMessages.Add((to, message));
    }
}

/// <summary>
/// Mock de base de datos: implementa IDatabase sin BD real.
/// Almacena datos en memoria para tests rápidos y aislados.
/// </summary>
public class MockDatabase : IDatabase
{
    private readonly Dictionary<int, string> _storage = new();
    private int _nextId = 1;

    public void Save(string data)
    {
        Console.WriteLine($"[MOCK] Guardando: {data}");
        _storage[_nextId] = data;
        _nextId++;
    }

    public string Get(int id)
    {
        bool exists = _storage.TryGetValue(id, out string? value);
        Console.WriteLine($"[MOCK] Leyendo registro {id}: {(exists ? value : "NO ENCONTRADO")}");
        return exists ? value! : string.Empty;
    }
}

// ╔═══════════════════════════════════════════════════════════════════════════╗
// ║                         SECCIÓN 5 — DEMOSTRACIÓN                         ║
// ║           Comparación lado a lado: código MALO vs código BUENO           ║
// ╚═══════════════════════════════════════════════════════════════════════════╝

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.WriteLine("  DIP — Principio de Inversión de Dependencias");
        Console.WriteLine("═══════════════════════════════════════════════════════\n");

        DemoViolacionDIP();
        DemoSolucionDIP_Notificaciones();
        DemoSolucionDIP_BaseDeDatos();
        DemoPoorMansDI();
        DemoTestingConMocks();
        DemoIoC_HollywoodPrinciple();
    }

    /// <summary>
    /// DEMO 1: Muestra por qué el código SIN DIP es problemático.
    /// </summary>
    private static void DemoViolacionDIP()
    {
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("  DEMO 1 — Código con VIOLACIÓN de DIP");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        Console.WriteLine("\n--- Notificaciones (violación) ---");
        // NotificationServiceViolation está casado con EmailSender para siempre.
        NotificationServiceViolation notifViolation = new();
        notifViolation.Notify("cliente@email.com", "Hola desde el CMS");

        Console.WriteLine("\n--- Base de datos (violación) ---");
        // UserServiceViolation está casado con SqlServerDatabase para siempre.
        UserServiceViolation userViolation = new();
        userViolation.CreateUser("María García");
        string result = userViolation.GetUser(1);
        Console.WriteLine($"Resultado: {result}");

        Console.WriteLine("\n⚠️  Problemas del código anterior:");
        Console.WriteLine("   1. No se puede cambiar Email por SMS sin modificar código.");
        Console.WriteLine("   2. No se puede cambiar SQL Server por PostgreSQL sin modificar código.");
        Console.WriteLine("   3. Es IMPOSIBLE testear sin infraestructura real.\n");
    }

    /// <summary>
    /// DEMO 2: NotificationService con DIP — inyecta EmailSender, luego SmsSender.
    /// El servicio de alto nivel NO cambia, solo cambia lo que se inyecta.
    /// </summary>
    private static void DemoSolucionDIP_Notificaciones()
    {
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("  DEMO 2 — Notificaciones CON DIP (inyección manual)");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        // El mismo servicio, distinta dependencia inyectada.
        // NotificationService NUNCA cambió. Solo cambió lo que recibe.

        Console.WriteLine("\n--- Con EmailSender ---");
        INotificationSender emailSender = new EmailSenderDIP();
        NotificationService emailService = new(emailSender);
        emailService.Notify("cliente@empresa.com", "Pedido confirmado");

        Console.WriteLine("\n--- Con SmsSender ---");
        INotificationSender smsSender = new SmsSenderDIP();
        NotificationService smsService = new(smsSender);
        smsService.Notify("+34 600 000 000", "Pedido confirmado");

        Console.WriteLine("\n✅ Ventaja: NotificationService NO se modificó al cambiar de canal.");
        Console.WriteLine("   Se cumple OCP: abierto a extensión (nuevos INotificationSender),");
        Console.WriteLine("   cerrado a modificación (NotificationService no cambia).\n");
    }

    /// <summary>
    /// DEMO 3: UserService con DIP — inyecta SQL Server y luego PostgreSQL.
    /// Misma lógica de negocio, distinto motor de base de datos.
    /// </summary>
    private static void DemoSolucionDIP_BaseDeDatos()
    {
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("  DEMO 3 — Base de datos CON DIP (inyección manual)");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        Console.WriteLine("\n--- Con SQL Server ---");
        IDatabase sqlDb = new SqlServerDatabaseDIP();
        UserService sqlUserService = new(sqlDb);
        sqlUserService.CreateUser("Carlos López");
        string sqlResult = sqlUserService.GetUser(1);
        Console.WriteLine($"Resultado: {sqlResult}");

        Console.WriteLine("\n--- Con PostgreSQL ---");
        IDatabase pgDb = new PostgreSqlDatabaseDIP();
        UserService pgUserService = new(pgDb);
        pgUserService.CreateUser("Ana Martínez");
        string pgResult = pgUserService.GetUser(1);
        Console.WriteLine($"Resultado: {pgResult}");

        Console.WriteLine("\n✅ Ventaja: UserService NO se modificó al cambiar de motor de BD.");
        Console.WriteLine("   Si mañana llega MongoDB, solo creas MongoDatabase : IDatabase");
        Console.WriteLine("   y la inyectas. CERO cambios en UserService.\n");
    }

    /// <summary>
    /// DEMO 4: Poor Man's DI — Inyección de Dependencias SIN contenedor.
    /// Es el "punto de composición" (composition root) manual.
    /// Así funciona internamente cualquier contenedor DI.
    /// </summary>
    private static void DemoPoorMansDI()
    {
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("  DEMO 4 — Poor Man's DI (DI manual sin contenedor)");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        Console.WriteLine("\n--- Composición manual (composition root) ---");

        // Este es el "punto de entrada" donde se decide QUÉ implementaciones usar.
        // En una app real, esto lo haría el contenedor DI de .NET.
        // Poor Man's DI es exactamente esto: new manual en el entry point.

        INotificationSender sender = new EmailSenderDIP();       // Decisión: Email
        IDatabase database = new PostgreSqlDatabaseDIP();        // Decisión: PostgreSQL

        // Inyección manual: cada servicio recibe sus dependencias.
        NotificationService notificationService = new(sender);
        UserService userService = new(database);

        // Los servicios NO saben qué implementación concreta están usando.
        notificationService.Notify("usuario@sistema.com", "Bienvenido al CMS");
        userService.CreateUser("Pedro Sánchez");

        Console.WriteLine("\n--- Cambiando configuración sin tocar servicios ---");

        // Si quiero cambiar a SMS y SQL Server, solo cambio las líneas de arriba.
        // Los servicios NotificationService y UserService NO se tocan.

        INotificationSender newSender = new SmsSenderDIP();
        IDatabase newDatabase = new SqlServerDatabaseDIP();

        NotificationService newNotifService = new(newSender);
        UserService newUserService = new(newDatabase);

        newNotifService.Notify("+34 600 000 001", "Aviso importante");
        newUserService.CreateUser("Lucía Fernández");

        Console.WriteLine("\n✅ Esto es EXACTAMENTE lo que hace el contenedor DI de .NET,");
        Console.WriteLine("   pero automatizado: resuelve dependencias y las inyecta por ti.\n");
    }

    /// <summary>
    /// DEMO 5: Testing con Mocks — solo posible gracias a DIP.
    /// Sin abstracciones no podríamos simular dependencias.
    /// </summary>
    private static void DemoTestingConMocks()
    {
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("  DEMO 5 — Testing con Mocks (gracias a DIP)");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        Console.WriteLine("\n--- Simulando un test unitario de NotificationService ---");

        // Creamos el mock en lugar de la dependencia real.
        MockNotificationSender mockSender = new();
        NotificationService serviceUnderTest = new(mockSender);

        // Act: ejecutamos el método que queremos probar.
        serviceUnderTest.Notify("test@test.com", "Mensaje de prueba");

        // Assert: verificamos que la dependencia se llamó correctamente.
        // Esto NO sería posible si NotificationService usara EmailSender directamente.
        int totalSent = mockSender.SentMessages.Count;
        (string to, string message) = mockSender.SentMessages[0];

        Console.WriteLine($"\n   Se enviaron {totalSent} mensaje(s)");
        Console.WriteLine($"   Destinatario: {to}");
        Console.WriteLine($"   Mensaje: {message}");

        // Verificación programática (como haría un test real con FluentAssertions):
        bool destinatarioCorrecto = to == "test@test.com";
        bool mensajeCorrecto = message == "Mensaje de prueba";
        bool testPasado = destinatarioCorrecto && mensajeCorrecto;

        Console.WriteLine($"\n   ¿Test pasado? {(testPasado ? "✅ SÍ" : "❌ NO")}");

        // --- Test con MockDatabase ---
        Console.WriteLine("\n--- Simulando un test unitario de UserService ---");

        MockDatabase mockDb = new();
        UserService userServiceUnderTest = new(mockDb);

        userServiceUnderTest.CreateUser("Usuario de prueba");
        string retrieved = userServiceUnderTest.GetUser(1);

        Console.WriteLine($"   Usuario recuperado: {retrieved}");
        Console.WriteLine($"   ¿Test pasado? {(retrieved == "Usuario: Usuario de prueba" ? "✅ SÍ" : "❌ NO")}");

        Console.WriteLine("\n✅ Sin DIP, estos tests necesitarían un servidor SMTP");
        Console.WriteLine("   y una instancia de SQL Server real. Con DIP, los tests");
        Console.WriteLine("   son rápidos, aislados y repetibles.\n");
    }

    /// <summary>
    /// DEMO 6: IoC y el Hollywood Principle.
    /// Explica la diferencia entre DIP, IoC y DI.
    /// </summary>
    private static void DemoIoC_HollywoodPrinciple()
    {
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("  DEMO 6 — IoC, Hollywood Principle y resumen");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        Console.WriteLine(@"
 ┌────────────────────────────────────────────────────────────────┐
 │  Hollywood Principle: ""No me llames, yo te llamo""              │
 │                                                                │
 │  Sin IoC:                                                      │
 │    NotificationService ──crea──► new EmailSender()             │
 │    NotificationService llama a EmailSender cuando quiere       │
 │    → Flujo de control MANEJADO por NotificationService         │
 │                                                                │
 │  Con IoC (gracias a DIP):                                      │
 │    Framework/Main ──crea──► EmailSender                        │
 │    Framework/Main ──inyecta──► NotificationService(sender)     │
 │    NotificationService recibe la dependencia, NO la crea       │
 │    → Flujo de control INVERTIDO: el framework llama al código  │
 │                                                                │
 │  Jerarquía de conceptos:                                       │
 │    DIP (Principio)    → ""Depende de abstracciones""            │
 │    IoC (Mecanismo)    → ""Invierte quién controla el flujo""    │
 │    DI  (Patrón)       → ""Inyecta dependencias por constructor""│
 │    Contenedor DI      → ""Automatiza la creación e inyección"" │
 └────────────────────────────────────────────────────────────────┘
");

        Console.WriteLine("📋 RESUMEN DE LA LECCIÓN:");
        Console.WriteLine("   DIP dice: dependencias hacia abstracciones, no hacia concreciones.");
        Console.WriteLine("   El módulo de alto nivel DEFINE el contrato (interfaz).");
        Console.WriteLine("   Los módulos de bajo nivel IMPLEMENTAN el contrato.");
        Console.WriteLine("   Sin DIP no hay DI posible. Sin DI no hay tests aislados.");
        Console.WriteLine("   Sin tests no hay refactor seguro. Sin refactor no hay evolución.");
        Console.WriteLine("   DIP es la base sobre la que se construye todo lo demás.\n");
    }
}
