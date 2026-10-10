// ──────────────────────────────────────────────────────────────────────────
// MÓDULO 3 — SOLID en C#
// Lección 01: Principio de Responsabilidad Única (SRP)
//
// SRP: "Una clase debe tener una, y solo una, razón para cambiar."
//      — Robert C. Martin (Uncle Bob)
//
// Si una clase hace más de una cosa, cualquier cambio en cualquiera de esas
// cosas te obliga a tocar (y potencialmente romper) TODO el código de la
// clase. En este archivo mostramos una violación clara y su refactor.
// ──────────────────────────────────────────────────────────────────────────

using System.Text.RegularExpressions;

// ══════════════════════════════════════════════════════════════════════════
// PROGRAMA PRINCIPAL — DEMO
// ══════════════════════════════════════════════════════════════════════════

Separador("DEMOSTRACIÓN DEL PRINCIPIO DE RESPONSABILIDAD ÚNICA (SRP)");

// ── Modelo de datos ────────────────────────────────────────────────────
// Creamos un post de ejemplo que usaremos en ambas versiones.
Post post = new Post
{
    Title = "Aprendiendo SOLID en C#",
    Content = "El Principio de Responsabilidad Única (SRP) es el primero de los cinco principios SOLID.",
    AuthorEmail = "profesor@cursodotnet.com"
};

// ══════════════════════════════════════════════════════════════════════
// PARTE 1 — VIOLACIÓN DEL SRP (CÓDIGO INCORRECTO)
// ══════════════════════════════════════════════════════════════════════

Separador("1. VIOLACIÓN — PostService que hace DEMASIADO");

Console.WriteLine(
    "❌ La clase 'BadPostService' tiene 4 responsabilidades:\n" +
    "   1. Valida el post (reglas de negocio)\n" +
    "   2. Guarda en 'base de datos' (persistencia)\n" +
    "   3. Envía email de notificación (infraestructura)\n" +
    "   4. Escribe log en archivo (infraestructura)\n"
);

BadPostService badService = new BadPostService();

try
{
    badService.Publish(post);
    Console.WriteLine("  ✅ Post publicado con la versión INCORRECTA.");
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ Error: {ex.Message}");
}

// ══════════════════════════════════════════════════════════════════════
// PARTE 2 — REFACTOR SRP (CÓDIGO CORRECTO)
// ══════════════════════════════════════════════════════════════════════

Separador("2. REFACTOR — Una clase, una responsabilidad");

Console.WriteLine(
    "✅ Cada clase tiene UNA sola responsabilidad:\n" +
    "   · PostValidator   → solo valida\n" +
    "   · PostRepository  → solo persiste\n" +
    "   · EmailNotifier   → solo notifica\n" +
    "   · Logger          → solo registra logs\n" +
    "   · PostService     → ORQUESTA (coordina a las demás, no hace el trabajo)\n"
);

// Construimos las dependencias por separado.
// Cada una puede evolucionar, testearse y mantenerse de forma independiente.
// El validador acepta el mínimo de título como parámetro (por si las reglas cambian).
PostValidator validator = new PostValidator(minTitleLength: 5);
PostRepository repository = new PostRepository();
EmailNotifier notifier = new EmailNotifier();
Logger logger = new Logger();

// PostService ahora es un ORQUESTADOR.
// Recibe sus dependencias por constructor — esto es Dependency Injection manual.
PostService postService = new PostService(validator, repository, notifier, logger);

try
{
    postService.Publish(post);
    Console.WriteLine("  ✅ Post publicado con la versión CORRECTA.");
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ Error: {ex.Message}");
}

// ══════════════════════════════════════════════════════════════════════
// PARTE 3 — CÓMO DETECTAR VIOLACIONES DE SRP
// ══════════════════════════════════════════════════════════════════════

Separador("3. HEURÍSTICAS PARA DETECTAR VIOLACIONES DE SRP");

Console.WriteLine(
    "🔍 Señales de alarma en tu código:\n" +
    "\n" +
    "   · Regla de la conjunción 'y':\n" +
    "       'Esta clase valida Y guarda Y envía emails Y escribe logs'\n" +
    "       → Cada 'y' es una razón de cambio distinta. SRP violado.\n" +
    "\n" +
    "   · Métodos que cambian por motivos distintos:\n" +
    "       Si cambias la validación, tocas el mismo archivo que\n" +
    "       cuando cambias la conexión a BD. Eso es una bandera roja.\n" +
    "\n" +
    "   · Clases con más de ~200 líneas:\n" +
    "       No es una regla estricta, pero es un síntoma frecuente.\n" +
    "       Una clase que hace una sola cosa rara vez necesita tanto código.\n" +
    "\n" +
    "   · Constructor con demasiadas dependencias:\n" +
    "       Si tu constructor recibe 7+ parámetros, probablemente\n" +
    "       la clase está haciendo demasiadas cosas.\n" +
    "\n" +
    "   · Dificultad para nombrar la clase:\n" +
    "       Si solo se te ocurre 'Manager', 'Helper', 'Processor' o 'Utils',\n" +
    "       suele ser señal de que la clase no tiene una responsabilidad clara.\n"
);

// ══════════════════════════════════════════════════════════════════════
// PARTE 4 — ESCENARIO DE CAMBIO (la gran ventaja de SRP)
// ══════════════════════════════════════════════════════════════════════

Separador("4. SIMULACIÓN: ¿qué pasa si cambia la lógica de validación?");

Console.WriteLine(
    "📋 Escenario: ahora el título del post debe tener al menos 10 caracteres\n" +
    "   (antes el mínimo era 5).\n"
);

Console.WriteLine(
    "   🔴 Versión INCORRECTA (BadPostService):\n" +
    "       Tocas el archivo de BadPostService.\n" +
    "       Riesgo: puedes romper la lógica de guardado, email O log\n" +
    "       sin darte cuenta, porque TODO convive en la misma clase.\n"
);

Console.WriteLine(
    "   🟢 Versión CORRECTA (con SRP):\n" +
    "       Tocas SOLO PostValidator.\n" +
    "       PostRepository, EmailNotifier, Logger y PostService\n" +
    "       NO se modifican. Riesgo mínimo. Cero efectos colaterales.\n"
);

// Demostramos el cambio: simulamos que el negocio pide subir el mínimo a 10.
// Con SRP, basta con instanciar un NUEVO validador con la regla actualizada
// — el resto de clases (Repository, Notifier, Logger, PostService)
// no se tocan en absoluto.
PostValidator validatorNuevo = new PostValidator(minTitleLength: 10);
PostService postServiceActualizado = new PostService(
    validatorNuevo, repository, notifier, logger);

Post postCortoCorto = new Post
{
    Title = "SOLID",            // Solo 5 caracteres — la nueva regla exige 10
    Content = "Este post tiene un título demasiado corto para la nueva regla.",
    AuthorEmail = "alumno@cursodotnet.com"
};

Console.WriteLine("  Probando con título 'SOLID' (5 chars, nueva regla = 10 mínimo)...");

try
{
    postServiceActualizado.Publish(postCortoCorto);
    Console.WriteLine("  ⚠️  El post fue aceptado (no debería).");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  ✅ Correctamente rechazado: {ex.Message}");
    Console.WriteLine("     Solo PostValidator cambió. El resto de clases no se tocaron.");
}

Separador("FIN DE LA DEMOSTRACIÓN");

// ──────────────────────────────────────────────────────────────────────────
// HELPERS DE PRESENTACIÓN
// ──────────────────────────────────────────────────────────────────────────

static void Separador(string titulo)
{
    Console.WriteLine();
    Console.WriteLine(new string('═', 70));
    Console.WriteLine($"  {titulo}");
    Console.WriteLine(new string('═', 70));
    Console.WriteLine();
}

// ══════════════════════════════════════════════════════════════════════════
// MODELO DE DOMINIO
// ══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Representa un artículo del CMS.
/// Es un modelo simple con las propiedades mínimas para la demo.
/// </summary>
public class Post
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
}

// ══════════════════════════════════════════════════════════════════════════
// PARTE 1 — CÓDIGO INCORRECTO: VIOLACIÓN DEL SRP
// ══════════════════════════════════════════════════════════════════════════

/// <summary>
/// ❌ VIOLACIÓN CLARA DE SRP.
/// Esta clase tiene 4 razones distintas para cambiar:
///   1. Cambian las reglas de validación → hay que tocar esta clase.
///   2. Cambia el motor de base de datos → hay que tocar esta clase.
///   3. Cambia el proveedor de email → hay que tocar esta clase.
///   4. Cambia el formato de log → hay que tocar esta clase.
///
/// Cualquier cambio, por pequeño que sea, obliga a modificar esta clase
/// y arriesga romper funcionalidad NO relacionada.
///
/// El problema NO es que la clase sea grande.
/// El problema es que ACOPLA responsabilidades que evolucionan
/// por motivos distintos.
/// </summary>
public class BadPostService
{
    // Base de datos simulada (estática para que persista entre llamadas).
    // En un proyecto real esto sería una base de datos externa.
    private static readonly List<Post> Database = new List<Post>();

    // Ruta del archivo de log.
    private readonly string _logFilePath =
        Path.Combine(Path.GetTempPath(), "cms-srp-demo-violation.log");

    /// <summary>
    /// Publica un post: valida, guarda, notifica y registra.
    /// TODO en un solo método de una sola clase.
    /// </summary>
    public void Publish(Post post)
    {
        // ── RESPONSABILIDAD 1: Validación ──────────────────────────
        // Si mañana cambian las reglas de validación (ej: título mínimo
        // de 10 caracteres en vez de 5), tengo que modificar ESTA clase.
        if (string.IsNullOrWhiteSpace(post.Title) || post.Title.Length < 5)
        {
            // El mensaje de error usa conocimiento específico de validación.
            throw new InvalidOperationException(
                "El título debe tener al menos 5 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(post.Content))
        {
            throw new InvalidOperationException("El contenido no puede estar vacío.");
        }

        // Validación de email con regex — también responsabilidad de validación.
        if (!Regex.IsMatch(post.AuthorEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new InvalidOperationException("El email del autor no es válido.");
        }

        // ── RESPONSABILIDAD 2: Persistencia ──────────────────────────
        // Si mañana cambiamos de lista en memoria a SQL Server,
        // PostgreSQL o MongoDB, tengo que modificar ESTA clase.
        Database.Add(post);
        Console.WriteLine($"  [BD] Post '{post.Title}' guardado en la base de datos.");

        // ── RESPONSABILIDAD 3: Notificación por email ────────────────
        // Si mañana cambiamos de SMTP a SendGrid, Mailgun o Amazon SES,
        // tengo que modificar ESTA clase.
        string emailBody =
            $"Para: {post.AuthorEmail}\n" +
            $"Asunto: Tu post '{post.Title}' ha sido publicado.\n" +
            $"Mensaje: Gracias por contribuir al CMS. Tu artículo ya está disponible.";

        Console.WriteLine($"  [EMAIL] Enviando notificación:\n{emailBody}");
        Console.WriteLine($"  [EMAIL] Email enviado a {post.AuthorEmail}.");

        // ── RESPONSABILIDAD 4: Logging ──────────────────────────────
        // Si mañana cambiamos el formato de log, el destino (archivo →
        // ElasticSearch → Seq → Application Insights), tengo que
        // modificar ESTA clase.
        string logEntry =
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
            $"INFO — Post publicado: '{post.Title}' por {post.AuthorEmail}";

        try
        {
            // AppendAllText abre y cierra el archivo en cada escritura —
            // ineficiente pero suficiente para la demo.
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            Console.WriteLine($"  [LOG] Entrada escrita en {_logFilePath}");
            Console.WriteLine($"  [LOG] Contenido: {logEntry}");
        }
        catch (Exception ex)
        {
            // Si el log falla, el post YA se guardó.
            // Esto es un problema de consistencia que surge al mezclar
            // responsabilidades sin transacciones adecuadas.
            Console.WriteLine($"  [LOG] ⚠️ Error al escribir el log: {ex.Message}");
        }
    }
}

// ══════════════════════════════════════════════════════════════════════════
// PARTE 2 — CÓDIGO CORRECTO: REFACTOR CON SRP
// ══════════════════════════════════════════════════════════════════════════

/// <summary>
/// ✅ RESPONSABILIDAD ÚNICA: Validar posts.
///
/// Esta clase sabe TODO sobre reglas de validación de posts.
/// Si las reglas cambian, SOLO esta clase se modifica.
/// No sabe nada de bases de datos, emails ni logs.
/// </summary>
public class PostValidator
{
    // El mínimo de título se recibe por constructor.
    // Esto permite que la regla cambie sin modificar la clase:
    // simplemente se inyecta un valor distinto desde configuración.
    private readonly int _minTitleLength;
    private const int MaxTitleLength = 200;

    public PostValidator(int minTitleLength = 5)
    {
        _minTitleLength = minTitleLength;
    }

    // El patrón de email se compila una sola vez (RegexOptions.Compiled)
    // para evitar recompilar en cada validación.
    // Usamos un TimeSpan de timeout para evitar ataques ReDoS.
    private static readonly Regex EmailPattern = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(500));

    /// <summary>
    /// Valida un post. Lanza excepción si no cumple las reglas.
    /// Podría devolver un Result<T> en vez de lanzar excepción;
    /// aquí usamos excepción por simplicidad didáctica.
    /// </summary>
    public void Validate(Post post)
    {
        // Cada validación es independiente y específica.
        // Si mañana el mínimo cambia de 5 a 10, modifico solo esta clase.

        if (string.IsNullOrWhiteSpace(post.Title))
        {
            throw new InvalidOperationException("El título no puede estar vacío.");
        }

        if (post.Title.Length < _minTitleLength)
        {
            throw new InvalidOperationException(
                $"El título debe tener al menos {_minTitleLength} caracteres.");
        }

        if (post.Title.Length > MaxTitleLength)
        {
            throw new InvalidOperationException(
                $"El título no puede superar los {MaxTitleLength} caracteres.");
        }

        if (string.IsNullOrWhiteSpace(post.Content))
        {
            throw new InvalidOperationException("El contenido no puede estar vacío.");
        }

        if (string.IsNullOrWhiteSpace(post.AuthorEmail))
        {
            throw new InvalidOperationException("El email del autor es obligatorio.");
        }

        if (!EmailPattern.IsMatch(post.AuthorEmail))
        {
            throw new InvalidOperationException(
                $"El email '{post.AuthorEmail}' no tiene un formato válido.");
        }
    }
}

/// <summary>
/// ✅ RESPONSABILIDAD ÚNICA: Persistencia de posts.
///
/// Esta clase sabe TODO sobre cómo guardar y recuperar posts.
/// Si el motor de base de datos cambia, SOLO esta clase se modifica.
/// No sabe nada de validación, emails ni logs.
/// </summary>
public class PostRepository
{
    // En una aplicación real, esto sería un DbContext de EF Core.
    // Aquí usamos una lista en memoria para la demo.
    private readonly List<Post> _posts = new List<Post>();

    /// <summary>
    /// Guarda un post en el almacén de datos.
    /// El caller (PostService) ya validó el post antes de llegar aquí.
    /// </summary>
    public void Save(Post post)
    {
        _posts.Add(post);
        Console.WriteLine($"  [BD] Post '{post.Title}' guardado (total: {_posts.Count} posts).");
    }
}

/// <summary>
/// ✅ RESPONSABILIDAD ÚNICA: Enviar notificaciones por email.
///
/// Esta clase sabe TODO sobre el envío de correos electrónicos.
/// Si el proveedor de email cambia, SOLO esta clase se modifica.
/// No sabe nada de validación, persistencia ni logs.
/// </summary>
public class EmailNotifier
{
    // En producción inyectarías IConfiguration o IOptions<EmailSettings>.
    private readonly string _fromAddress = "noreply@cmsdemo.com";

    /// <summary>
    /// Envía una notificación por email al autor del post.
    /// En la demo, simulamos el envío escribiendo en consola.
    /// </summary>
    public void NotifyPostPublished(Post post)
    {
        string subject = $"Tu post '{post.Title}' ha sido publicado ✅";
        string body =
            $"Hola,\n\n" +
            $"Tu artículo '{post.Title}' ha sido publicado correctamente " +
            $"en el CMS.\n\n" +
            $"Saludos,\n" +
            $"El equipo del CMS";

        // En producción usarías SmtpClient, MailKit, SendGrid SDK, etc.
        SimulateSendEmail(post.AuthorEmail, subject, body);
    }

    /// <summary>
    /// Simula el envío de un email.
    /// En producción esto conectaría con un servidor SMTP o API externa.
    /// </summary>
    private void SimulateSendEmail(string to, string subject, string body)
    {
        Console.WriteLine($"  [EMAIL] ───────────────────────────────────────");
        Console.WriteLine($"  [EMAIL] De:     {_fromAddress}");
        Console.WriteLine($"  [EMAIL] Para:   {to}");
        Console.WriteLine($"  [EMAIL] Asunto: {subject}");
        Console.WriteLine($"  [EMAIL] ───────────────────────────────────────");
        Console.WriteLine($"  [EMAIL] {body}");
        Console.WriteLine($"  [EMAIL] ───────────────────────────────────────");
        Console.WriteLine($"  [EMAIL] ✅ Email enviado (simulado).");
    }
}

/// <summary>
/// ✅ RESPONSABILIDAD ÚNICA: Registrar logs.
///
/// Esta clase sabe TODO sobre cómo y dónde se escriben los logs.
/// Si el destino de logs cambia, SOLO esta clase se modifica.
/// No sabe nada de validación, persistencia ni emails.
/// </summary>
public class Logger
{
    private readonly string _logFilePath;

    /// <summary>
    /// Constructor. La ruta del archivo se podría inyectar
    /// desde configuración con IOptions<string>.
    /// </summary>
    public Logger()
    {
        _logFilePath = Path.Combine(Path.GetTempPath(), "cms-srp-demo-refactor.log");
    }

    /// <summary>
    /// Registra un mensaje informativo en el archivo de log.
    /// En producción usarías Serilog, NLog o ILogger<T> de Microsoft.
    /// </summary>
    public void LogInformation(string message)
    {
        string logEntry =
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO — {message}";

        try
        {
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            Console.WriteLine($"  [LOG] 📝 {_logFilePath}");
            Console.WriteLine($"  [LOG]    {logEntry}");
        }
        catch (Exception ex)
        {
            // Si el log falla, al menos lo mostramos en consola.
            // En producción, esto iría a un fallback logger.
            Console.WriteLine($"  [LOG] ⚠️ Error al escribir log: {ex.Message}");
        }
    }
}

/// <summary>
/// ✅ RESPONSABILIDAD ÚNICA: Orquestar la publicación de un post.
///
/// El PostService refactorizado NO valida, NO guarda, NO envía emails
/// y NO escribe logs. Su única responsabilidad es COORDINAR a las clases
/// que sí hacen esas tareas, en el orden correcto.
///
/// Esto se llama "Orquestación" o "Coordinación".
/// El servicio define EL FLUJO de trabajo, no LA IMPLEMENTACIÓN
/// de cada paso.
///
/// Si el flujo cambia (ej: validar DESPUÉS de guardar), solo cambia aquí.
/// Si la implementación de un paso cambia, esta clase no se toca.
/// </summary>
public class PostService
{
    // Cada dependencia se recibe por constructor.
    // Esto permite inyectar implementaciones reales en producción
    // y mocks/fakes en tests sin cambiar una sola línea de esta clase.
    private readonly PostValidator _validator;
    private readonly PostRepository _repository;
    private readonly EmailNotifier _notifier;
    private readonly Logger _logger;

    public PostService(
        PostValidator validator,
        PostRepository repository,
        EmailNotifier notifier,
        Logger logger)
    {
        _validator = validator;
        _repository = repository;
        _notifier = notifier;
        _logger = logger;
    }

    /// <summary>
    /// Publica un post siguiendo el flujo de negocio:
    ///   1. Validar
    ///   2. Guardar
    ///   3. Notificar
    ///   4. Registrar
    ///
    /// Fíjate: este método NO contiene lógica de validación,
    /// NO contiene SQL, NO contiene SMTP y NO contiene escritura
    /// de archivos. Solo ORQUESTA.
    /// </summary>
    public void Publish(Post post)
    {
        // Paso 1: Validar.
        // Delega COMPLETAMENTE en el validador. No sabe CÓMO se valida.
        _validator.Validate(post);

        // Paso 2: Guardar.
        // Delega COMPLETAMENTE en el repositorio. No sabe CÓMO se guarda.
        _repository.Save(post);

        // Paso 3: Notificar.
        // Delega COMPLETAMENTE en el notificador. No sabe CÓMO se envía el email.
        _notifier.NotifyPostPublished(post);

        // Paso 4: Registrar.
        // Delega COMPLETAMENTE en el logger. No sabe CÓMO se escribe el log.
        _logger.LogInformation(
            $"Post publicado: '{post.Title}' por {post.AuthorEmail}");
    }
}

// ══════════════════════════════════════════════════════════════════════════
// RESUMEN VISUAL: DIFERENCIA ENTRE VIOLACIÓN Y REFACTOR
// ══════════════════════════════════════════════════════════════════════════
//
//   VIOLACIÓN (BadPostService):
//   ┌──────────────────────────────────────────┐
//   │         BadPostService                   │
//   │                                          │
//   │  · Validar título, contenido, email      │  ← Reglas de negocio
//   │  · Insertar en base de datos             │  ← Persistencia
//   │  · Conectar con SMTP y enviar email      │  ← Infraestructura
//   │  · Abrir archivo y escribir log          │  ← Infraestructura
//   │                                          │
//   │  4 razones para cambiar ⇒ SRP violado    │
//   └──────────────────────────────────────────┘
//
//   REFACTOR (con SRP):
//   ┌──────────────┐  ┌────────────────┐  ┌──────────────┐  ┌──────────┐
//   │ PostValidator│  │ PostRepository │  │ EmailNotifier│  │  Logger  │
//   │              │  │                │  │              │  │          │
//   │  Validar()   │  │   Save()       │  │  Notify()    │  │  Log()   │
//   └──────┬───────┘  └───────┬────────┘  └──────┬───────┘  └────┬─────┘
//          │                  │                  │               │
//          └──────────────────┼──────────────────┼───────────────┘
//                             │                  │
//                     ┌───────┴──────────────────┴───────┐
//                     │         PostService              │
//                     │                                 │
//                     │  Publish(post):                 │
//                     │    1. _validator.Validate(post) │
//                     │    2. _repository.Save(post)    │
//                     │    3. _notifier.Notify(post)    │
//                     │    4. _logger.Log(...)          │
//                     │                                 │
//                     │  1 sola razón para cambiar:     │
//                     │  el FLUJO de publicación.       │
//                     └─────────────────────────────────┘
