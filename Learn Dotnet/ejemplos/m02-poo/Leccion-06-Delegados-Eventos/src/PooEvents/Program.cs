// ============================================================
// MÓDULO 2 — POO EN C# EN PROFUNDIDAD
// Lección 06 — DELEGADOS Y EVENTOS
// ============================================================
// Un delegado es un TIPO que representa referencias a métodos
// con una firma específica. Es un "puntero a función" tipado y seguro.
// Los EVENTOS son delegados con protección: solo la clase dueña
// puede invocarlos. Desde fuera solo se puede suscribir (+=) y
// desuscribir (-=).
// ============================================================

// ──── 1. QUÉ ES UN DELEGADO ──────────────────────────────────
// Declaración de delegado: define la firma que los métodos deben cumplir.
// Va al final del archivo: delegate int Operation(int a, int b);
// Significa: "Operation es un tipo que referencia métodos que
//            reciben dos int y devuelven un int".

// Métodos que cumplen con la firma del delegado Operation
static int Add(int a, int b)
{
    Console.WriteLine($"  Add({a}, {b}) = {a + b}");
    return a + b;
}

static int Multiply(int a, int b)
{
    Console.WriteLine($"  Multiply({a}, {b}) = {a * b}");
    return a * b;
}

static int Subtract(int a, int b)
{
    Console.WriteLine($"  Subtract({a}, {b}) = {a - b}");
    return a - b;
}

Console.WriteLine("===== 1. DELEGADO BÁSICO =====");
Console.WriteLine("Un delegado es un tipo que almacena referencias a métodos.");
Console.WriteLine();

// Instanciar un delegado — tres formas equivalentes:
Operation sumOp = Add;                         // Forma moderna (C# 2.0+)
Operation sumOpOld = new Operation(Add);        // Forma clásica (C# 1.0)

// Invocar el delegado como si fuera el método original:
int result = sumOp(5, 3);
Console.WriteLine($"  Resultado de sumOp(5, 3): {result}");
Console.WriteLine();

// ──── 2. DELEGADOS MULTICAST ─────────────────────────────────
// Un delegado puede contener una CADENA de métodos (invocation list).
// Al invocarlo, se ejecutan TODOS en orden.
// IMPORTANTE: solo se devuelve el valor del ÚLTIMO método ejecutado.

Console.WriteLine("===== 2. DELEGADOS MULTICAST =====");
Console.WriteLine("Un delegado puede apuntar a múltiples métodos.");
Console.WriteLine("Al invocar se ejecutan TODOS, pero solo retorna el ÚLTIMO valor.");
Console.WriteLine();

Operation pipeline = Add;       // pipeline apunta a Add
pipeline += Multiply;           // Ahora también a Multiply
pipeline += Subtract;           // Ahora también a Subtract

Console.WriteLine("Invocando pipeline (Add + Multiply + Subtract):");
int finalResult = pipeline(10, 5);
Console.WriteLine($"  Valor retornado (último método): {finalResult}");
Console.WriteLine();

// Quitar un método de la cadena:
Console.WriteLine("Quitando Multiply del pipeline con -=");
// Al quitar un método de una cadena de 3, el resultado no será null
#pragma warning disable CS8601 // El compilador advierte que -= puede devolver null
pipeline -= Multiply;
#pragma warning restore CS8601
Console.WriteLine("Invocando pipeline (solo Add + Subtract):");
pipeline?.Invoke(10, 5);
Console.WriteLine();

// ──── 3. MÉTODOS ANÓNIMOS (C# 2.0) ──────────────────────────
// Antes de que existieran las lambdas, se usaba la palabra clave
// "delegate" para definir métodos sin nombre.
Console.WriteLine("===== 3. MÉTODOS ANÓNIMOS (C# 2.0) =====");
Console.WriteLine("Estilo antiguo: delegate(tipo param) { cuerpo }");
Console.WriteLine();

Operation anonymousMethod = delegate(int a, int b)
{
    int powerResult = (int)Math.Pow(a, b); // a elevado a b
    Console.WriteLine($"  Potencia({a}, {b}) = {powerResult}");
    return powerResult;
};

int anonResult = anonymousMethod(2, 5);
Console.WriteLine($"  Resultado: {anonResult}");
Console.WriteLine();

// ──── 4. LAMBDAS COMO DELEGADOS (C# 3.0+) ───────────────────
// Las lambdas son la forma moderna y concisa de crear delegados.
Console.WriteLine("===== 4. LAMBDAS COMO DELEGADOS (C# 3.0+) =====");
Console.WriteLine("Las lambdas son la forma concisa de crear delegados.");
Console.WriteLine();

// Lambda asignada a nuestro delegado personalizado:
Operation lambdaSum = (a, b) =>
{
    int sum = a + b;
    Console.WriteLine($"  Lambda Sum({a}, {b}) = {sum}");
    return sum;
};

// Lambda de expresión (una sola línea, sin llaves):
Operation lambdaMult = (a, b) => a * b;

Console.WriteLine("Lambda de bloque (multilínea):");
lambdaSum(7, 3);

Console.WriteLine("Lambda de expresión (una línea):");
Console.WriteLine($"  lambdaMult(7, 3) = {lambdaMult(7, 3)}");
Console.WriteLine();

// Delegados genéricos predefinidos en .NET:
// - Func<T1, T2, TResult>: acepta T1, T2 y devuelve TResult
// - Action<T1, T2>: acepta T1, T2 y no devuelve nada (void)
// - Predicate<T>: acepta T y devuelve bool
Console.WriteLine("Usando delegados genéricos de .NET:");
Func<int, int, int> funcSum = (a, b) => a + b;
Action<string, int> logAction = (msg, n) => Console.WriteLine($"  LOG: {msg} - {n}");
Predicate<int> isEven = n => n % 2 == 0;

Console.WriteLine($"  Func<int,int,int> (a,b)=>a+b : {funcSum(4, 6)}");
logAction("Procesado", 42);
Console.WriteLine($"  Predicate<int> IsEven(7): {isEven(7)}");
Console.WriteLine($"  Predicate<int> IsEven(8): {isEven(8)}");
Console.WriteLine();

// ──── 5. QUÉ SON LOS EVENTOS ────────────────────────────────
Console.WriteLine("===== 5. EVENTOS =====");
Console.WriteLine("Un EVENTO es un delegado con dos restricciones:");
Console.WriteLine("  1. Solo la clase propietaria puede INVOCARLO (dispararlo).");
Console.WriteLine("  2. Desde fuera solo se puede SUSCRIBIR (+=) y DESUSCRIBIR (-=).");
Console.WriteLine("Esto evita que código externo dispare el evento o lo sobreescriba.");
Console.WriteLine();
Console.WriteLine("  Patrón estándar:");
Console.WriteLine("  - event EventHandler<TEventArgs> NombreEvento;");
Console.WriteLine("  - protected virtual void OnNombreEvento(TEventArgs e)");
Console.WriteLine("  - NombreEvento?.Invoke(this, e);");
Console.WriteLine();

// ──── 6. EJEMPLO PRÁCTICO: BOTÓN SIMULADO ───────────────────
Console.WriteLine("===== 6. BOTÓN SIMULADO CON EVENTO CLICKED =====");
Console.WriteLine();

// Creamos el botón y los suscriptores (Display y Logger)
Button button = new Button("Guardar");
Display display = new Display();
Logger logger = new Logger("app.log");

// Suscribimos ( += ) los manejadores al evento Clicked
button.Clicked += display.OnButtonClicked;
button.Clicked += logger.OnButtonClicked;

Console.WriteLine("El usuario hace clic en el botón...");
button.Click(); // Dispara el evento → se ejecutan Display y Logger
Console.WriteLine();

Console.WriteLine("El usuario hace clic otra vez...");
button.Click();
Console.WriteLine();

// ──── 7. EJEMPLO PRÁCTICO: PROGRESO DE DESCARGA ─────────────
Console.WriteLine("===== 7. PROGRESO DE DESCARGA =====");
Console.WriteLine();

// Creamos el descargador y los suscriptores
Downloader downloader = new Downloader("foto.jpg", 1_000_000); // 1 MB
ProgressBar progressBar = new ProgressBar();
NotificationService notifier = new NotificationService();

// Suscribimos a los eventos del descargador
downloader.ProgressChanged += progressBar.OnProgressChanged;
downloader.DownloadCompleted += notifier.OnDownloadCompleted;

Console.WriteLine("Iniciando descarga simulada...");
Console.WriteLine();

// Ejecutamos la descarga (simulada con delays)
await downloader.DownloadAsync();

Console.WriteLine();

// ──── 8. DEMOSTRACIÓN DE DESUSCRIPCIÓN (-=) ─────────────────
Console.WriteLine("===== 8. DESUSCRIPCIÓN DE EVENTOS (-=) =====");
Console.WriteLine("Podemos quitar suscriptores en cualquier momento con -=");
Console.WriteLine();

Button button2 = new Button("Cancelar");
button2.Clicked += display.OnButtonClicked;
button2.Clicked += logger.OnButtonClicked;

Console.WriteLine("Primer clic (con ambos suscriptores):");
button2.Click();

// Desuscribimos al logger
button2.Clicked -= logger.OnButtonClicked;

Console.WriteLine();
Console.WriteLine("Segundo clic (sin el logger):");
button2.Click();
Console.WriteLine();

// ──── 9. RESUMEN ─────────────────────────────────────────────
Console.WriteLine("===== 9. RESUMEN DE LA LECCIÓN =====");
Console.WriteLine();
Console.WriteLine("  DELEGADO: Tipo que referencia métodos con una firma específica.");
Console.WriteLine("    - Sintaxis: delegate TResult Nombre(params);");
Console.WriteLine("    - Multicast: += para añadir, -= para quitar.");
Console.WriteLine("    - Invocación: delegado(args) o delegado?.Invoke(args).");
Console.WriteLine();
Console.WriteLine("  DELEGADOS PREDEFINIDOS:");
Console.WriteLine("    - Func<T1,T2,...,TResult>  → métodos que devuelven valor.");
Console.WriteLine("    - Action<T1,T2,...>        → métodos que devuelven void.");
Console.WriteLine("    - Predicate<T>             → métodos que devuelven bool.");
Console.WriteLine("    - EventHandler / EventHandler<T>  → eventos.");
Console.WriteLine();
Console.WriteLine("  EVENTO: Delegado protegido.");
Console.WriteLine("    - Solo la clase propietaria puede invocarlo.");
Console.WriteLine("    - Desde fuera: += (suscribir) y -= (desuscribir).");
Console.WriteLine("    - Siempre verificar null: MiEvento?.Invoke(this, args).");
Console.WriteLine();
Console.WriteLine("  PATRÓN ESTÁNDAR .NET:");
Console.WriteLine("    - EventArgs como clase base de argumentos.");
Console.WriteLine("    - EventHandler<TEventArgs> como tipo del evento.");
Console.WriteLine("    - protected virtual void OnXxx(...) como método disparador.");
Console.WriteLine();
Console.WriteLine("  CUÁNDO USAR EVENTOS:");
Console.WriteLine("    - Notificar a otros componentes que algo ocurrió.");
Console.WriteLine("    - Desacoplar el emisor de los receptores.");
Console.WriteLine("    - Implementar el patrón Observer (pub/sub).");
Console.WriteLine();
Console.WriteLine("  CUÁNDO USAR DELEGADOS DIRECTOS:");
Console.WriteLine("    - Pasar lógica como parámetro (callbacks, estrategias).");
Console.WriteLine("    - LINQ (Where, Select, OrderBy reciben delegados Func<>).");
Console.WriteLine("    - Tasks (Task.Run recibe Action / Func<>).");
Console.WriteLine();

Console.WriteLine("===== FIN DE LA LECCIÓN 06 =====");


// ============================================================
// DECLARACIÓN DEL DELEGADO PERSONALIZADO
// ============================================================
// Define un tipo que referencia métodos con firma: int Método(int, int)
internal delegate int Operation(int a, int b);


// ============================================================
// CLASES PARA EL EJEMPLO DEL BOTÓN (SECCIÓN 6)
// ============================================================

/// <summary>
/// Simula un botón de UI. Expone un evento Clicked que se dispara
/// cuando el usuario hace clic.
/// </summary>
internal class Button
{
    private readonly string _name;

    /// <summary>
    /// Evento que se dispara cuando el botón recibe un clic.
    /// - EventHandler: delegado predefinido (object sender, EventArgs e).
    /// - La keyword 'event' impide que código externo lo invoque o reasigne.
    /// </summary>
    public event EventHandler? Clicked;

    public Button(string name)
    {
        _name = name;
    }

    /// <summary>
    /// Simula el clic del usuario. Solo el propio Button puede llamar a Click().
    /// Internamente dispara el evento Clicked.
    /// </summary>
    public void Click()
    {
        Console.WriteLine($"  [Button '{_name}': ¡Clic recibido!]");

        // Patrón estándar: invocar a través del método OnXxx
        OnClicked(EventArgs.Empty);
    }

    /// <summary>
    /// Método disparador del evento. Es protected virtual para que
    /// subclases puedan interceptar o modificar el comportamiento.
    /// Siempre se verifica null con el operador ?. antes de invocar.
    /// </summary>
    protected virtual void OnClicked(EventArgs e)
    {
        // ?.Invoke: si no hay suscriptores (null), no hace nada.
        // Si hay suscriptores, los ejecuta todos en orden.
        Clicked?.Invoke(this, e);
    }
}

/// <summary>
/// Simula una pantalla que reacciona al clic del botón mostrando un mensaje.
/// </summary>
internal class Display
{
    /// <summary>
    /// Manejador de evento. La firma debe coincidir con EventHandler:
    /// void Método(object? sender, EventArgs e)
    /// </summary>
    public void OnButtonClicked(object? sender, EventArgs e)
    {
        Button? btn = sender as Button;
        string buttonName = btn is not null
            ? $"'{btn.GetType().Name}'"
            : "desconocido";

        Console.WriteLine($"    [Display] Se hizo clic en el botón {buttonName}. ¡Mostrando feedback visual!");
    }
}

/// <summary>
/// Simula un sistema de logging que registra cada clic en un archivo.
/// </summary>
internal class Logger
{
    private readonly string _logFile;

    public Logger(string logFile)
    {
        _logFile = logFile;
    }

    /// <summary>
    /// Manejador de evento: registra el clic con timestamp.
    /// </summary>
    public void OnButtonClicked(object? sender, EventArgs e)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        Console.WriteLine($"    [Logger → {_logFile}] [{timestamp}] Botón presionado.");
    }
}


// ============================================================
// CLASES PARA EL EJEMPLO DE DESCARGA (SECCIÓN 7)
// ============================================================

/// <summary>
/// Argumentos personalizados para el evento de progreso de descarga.
/// Hereda de EventArgs (convención del patrón estándar .NET).
/// Contiene información sobre el estado actual de la descarga.
/// </summary>
internal class DownloadEventArgs : EventArgs
{
    /// <summary>Porcentaje completado (0 a 100).</summary>
    public int Progress { get; init; }

    /// <summary>Bytes descargados hasta el momento.</summary>
    public long BytesDownloaded { get; init; }

    /// <summary>Total de bytes del archivo.</summary>
    public long TotalBytes { get; init; }

    /// <summary>Nombre del archivo que se está descargando.</summary>
    public string FileName { get; init; }

    public DownloadEventArgs(int progress, long bytesDownloaded, long totalBytes, string fileName)
    {
        Progress = progress;
        BytesDownloaded = bytesDownloaded;
        TotalBytes = totalBytes;
        FileName = fileName;
    }
}

/// <summary>
/// Simula la descarga de un archivo, reportando progreso mediante eventos.
/// Usa el patrón estándar: EventHandler<TEventArgs> + métodos OnXxx.
/// </summary>
internal class Downloader
{
    private readonly string _fileName;
    private readonly long _totalBytes;

    /// <summary>
    /// Evento que se dispara cada vez que avanza la descarga.
    /// Incluye información de progreso en DownloadEventArgs.
    /// </summary>
    public event EventHandler<DownloadEventArgs>? ProgressChanged;

    /// <summary>
    /// Evento que se dispara cuando la descarga termina.
    /// No necesita args personalizados: usa EventArgs.Empty.
    /// </summary>
    public event EventHandler? DownloadCompleted;

    public Downloader(string fileName, long totalBytes)
    {
        _fileName = fileName;
        _totalBytes = totalBytes;
    }

    /// <summary>
    /// Ejecuta la descarga simulada.
    /// Avanza en bloques de 10%, disparando ProgressChanged en cada paso.
    /// Al finalizar, dispara DownloadCompleted.
    /// </summary>
    public async Task DownloadAsync()
    {
        Console.WriteLine($"  Iniciando descarga de '{_fileName}' ({_totalBytes:N0} bytes)...");

        // Simulamos 10 pasos de descarga (10% cada uno)
        for (int step = 1; step <= 10; step++)
        {
            // Simular latencia de red variable (entre 100ms y 400ms)
            int delayMs = Random.Shared.Next(100, 400);
            await Task.Delay(delayMs);

            int progress = step * 10;                         // 10%, 20%, ..., 100%
            long bytesDownloaded = _totalBytes * step / 10;   // Bytes descargados proporcionalmente

            // Disparar evento de progreso con los datos actuales
            OnProgressChanged(new DownloadEventArgs(
                progress: progress,
                bytesDownloaded: bytesDownloaded,
                totalBytes: _totalBytes,
                fileName: _fileName
            ));
        }

        // Descarga completada: disparar evento de finalización
        OnDownloadCompleted(EventArgs.Empty);
    }

    /// <summary>
    /// Método disparador para ProgressChanged.
    /// protected virtual → las subclases pueden sobrescribirlo para
    /// añadir lógica antes/después de notificar (ej: logging interno).
    /// </summary>
    protected virtual void OnProgressChanged(DownloadEventArgs e)
    {
        ProgressChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Método disparador para DownloadCompleted.
    /// </summary>
    protected virtual void OnDownloadCompleted(EventArgs e)
    {
        DownloadCompleted?.Invoke(this, e);
    }
}

/// <summary>
/// Simula una barra de progreso visual que se actualiza
/// al recibir eventos de progreso de descarga.
/// </summary>
internal class ProgressBar
{
    /// <summary>
    /// Manejador del evento ProgressChanged.
    /// Dibuja una barra de progreso ASCII en consola.
    /// </summary>
    public void OnProgressChanged(object? sender, DownloadEventArgs e)
    {
        // Construir barra de progreso visual: [████████░░░░░░░░]
        int filledBlocks = e.Progress / 5;         // 20 bloques totales (100/5)
        int emptyBlocks = 20 - filledBlocks;

        string bar = new string('#', filledBlocks) + new string('-', emptyBlocks);

        // Mostrar porcentaje, barra y bytes descargados
        Console.WriteLine(
            $"  [{bar}] {e.Progress,3}%  " +
            $"{e.BytesDownloaded / 1024,4} KB / {e.TotalBytes / 1024} KB"
        );
    }
}

/// <summary>
/// Simula un servicio de notificaciones (push, email, etc.) que
/// avisa al usuario cuando una descarga se completa.
/// </summary>
internal class NotificationService
{
    /// <summary>
    /// Manejador del evento DownloadCompleted.
    /// Se ejecuta cuando la descarga termina.
    /// </summary>
    public void OnDownloadCompleted(object? sender, EventArgs e)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        Console.WriteLine();
        Console.WriteLine($"  [NotificationService] [{timestamp}] ¡Descarga completada!");
        Console.WriteLine($"  [NotificationService] Enviando notificación push al usuario...");
    }
}
