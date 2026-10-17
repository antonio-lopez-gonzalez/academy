// ═══════════════════════════════════════════════════════════════════════════════
// Lección 03 — Interfaces Explícitas en C#
// ═══════════════════════════════════════════════════════════════════════════════
//
// ¿QUÉ SON LAS INTERFACES EXPLÍCITAS?
// ──────────────────────────────────
// Una implementación explícita de interfaz (EII, por Explicit Interface
// Implementation) permite implementar un miembro de una interfaz de forma que
// SOLO sea accesible a través de una referencia del tipo de esa interfaz.
//
// Sintaxis:   TipoRetorno IInterfaz.Metodo() { ... }
//             int IInterfaz.Propiedad { get; set; }
//
// Observaciones clave:
//   - NO llevan modificador de acceso (ni public, ni private, ni internal).
//   - NO pueden ser virtual ni abstract.
//   - NO aparecen en la API pública de la clase (IntelliSense no las muestra).
//   - Solo se invocan si la referencia es del tipo de la interfaz.
//
// ¿CUÁNDO USARLAS?
// ────────────────
// 1. Colisión de nombres: dos interfaces definen un miembro con el mismo nombre
//    y firma pero requieren comportamientos diferentes.
// 2. Ocultación deliberada: quieres implementar una interfaz de framework
//    (ej. ICloneable, IDisposable simulado) pero no quieres exponer sus métodos
//    como parte de la API pública de tu clase.
// 3. Reducción de ruido: cuando una clase implementa muchas interfaces, reduces
//    la "superficie" pública visible para el consumidor de la clase.
//
// ═══════════════════════════════════════════════════════════════════════════════


// ═══════════════════════════════════════════════════════════════════════════════
// DEMO PRINCIPAL — Top-level statements
// ═══════════════════════════════════════════════════════════════════════════════

Console.WriteLine("═══════════════════════════════════════════");
Console.WriteLine("  LECCIÓN 03 — INTERFACES EXPLÍCITAS");
Console.WriteLine("═══════════════════════════════════════════\n");

// ──── EJEMPLO 1: Métodos duplicados ───────────────────────────────────────────

Console.WriteLine("─── EJEMPLO 1: DiagnosticService (ILogger + ITracer) ───\n");

DiagnosticService diagnostic = new DiagnosticService();

// ❌ Esto NO compila — Log no es parte del API pública de DiagnosticService:
//   diagnostic.Log("Hola");  // Error CS1061

// ✔️ Solo se accede a Log mediante una referencia de la interfaz correspondiente:

Console.WriteLine("Usando referencia ILogger:");
ILogger logger = diagnostic;
logger.Log("Aplicación iniciada.");
logger.Log("Usuario autenticado.");

Console.WriteLine();

Console.WriteLine("Usando referencia ITracer:");
ITracer tracer = diagnostic;
tracer.Log("Entering method CalculateTotal()");
tracer.Log("Stack walk depth: 3");

Console.WriteLine();

// ✔️ También se puede invocar con casting inline:
Console.WriteLine("Usando casting inline:");
((ILogger)diagnostic).Log("Conexión a BD establecida (vía cast).");

Console.WriteLine();

// ✔️ Pattern matching (más seguro que el casting directo):
Console.WriteLine("Usando pattern matching:");
if (diagnostic is ILogger diagnosticLogger)
{
    diagnosticLogger.Log("Servicio health-check OK (vía pattern match).");
}

Console.WriteLine();

// ✔️ El método público SÍ está accesible directamente:
diagnostic.ReportStatus();

Console.WriteLine("\n────────────────────────────────────────\n");

// ──── EJEMPLO 2: Ocultación deliberada ────────────────────────────────────────

Console.WriteLine("─── EJEMPLO 2: ResourceManager (Close público, Dispose oculto) ───\n");

ResourceManager resources = new ResourceManager();

// ❌ Esto NO compila — Dispose está oculto:
//   resources.Dispose();  // Error CS1061

// ✔️ Close() sí está disponible:
resources.DoWork();
resources.Close();                         // Internamente llama a Dispose

// ✔️ Si usamos using (que espera IDisposable), funcionaría con la interfaz real.
//    Aquí lo simulamos llamando Dispose vía casteo:
Console.WriteLine("Llamando Dispose vía interfaz explícitamente:");
((IMyDisposable)resources).Dispose();      // Segunda llamada no debería hacer nada

Console.WriteLine("\n────────────────────────────────────────\n");

// ──── EJEMPLO 3: Múltiples Render() distintos ─────────────────────────────────

Console.WriteLine("─── EJEMPLO 3: Document (IDrawable + IPrintable + ISaveable) ───\n");

Document doc = new Document("Informe Anual 2026");

// ❌ No compila:
//   doc.Render();  // Error CS1061 — ¿cuál Render?

// ✔️ Cada interfaz activa su propia implementación:

IDrawable drawable = doc;
drawable.Render();

IPrintable printable = doc;
printable.Render();

ISaveable saveable = doc;
saveable.Render();

Console.WriteLine("\n────────────────────────────────────────\n");

// ──── EJEMPLO 4: Propiedades e indexadores explícitos ─────────────────────────

Console.WriteLine("─── EJEMPLO 4: LimitedCollection (Count e indexador explícitos) ───\n");

LimitedCollection collection = new LimitedCollection(
    new[] { "Alpha", "Beta", "Gamma", "Delta" }
);

// ❌ No compilan directamente:
//   int c = collection.Count;        // Error CS1061
//   string x = collection[0];        // Error CS0021

// ✔️ Solo a través de la interfaz:
ICounter counter = collection;
Console.WriteLine($"Número de elementos (vía ICounter.Count): {counter.Count}");
Console.WriteLine($"Elemento en índice 2 (vía ICounter[index]): {counter[2]}");

// ✔️ El método público de la clase sí funciona:
Console.WriteLine($"Elemento en índice 0 (vía GetItem): {collection.GetItem(0)}");

Console.WriteLine("\n═══════════════════════════════════════════");
Console.WriteLine("  FIN DE LA LECCIÓN");
Console.WriteLine("═══════════════════════════════════════════");


// ═══════════════════════════════════════════════════════════════════════════════
// TIPOS — Las declaraciones de tipo deben ir DESPUÉS de los top-level statements
// ═══════════════════════════════════════════════════════════════════════════════


// ───────────────────────────────────────────────────────────────────────────────
// INTERFACES PARA EL EJEMPLO 1 — Métodos con nombre duplicado
// ───────────────────────────────────────────────────────────────────────────────

// Simula un sistema de logging tradicional (escribe a archivo/consola)
interface ILogger
{
    void Log(string message);
}

// Simula un sistema de tracing (instrumentación para diagnóstico profundo)
interface ITracer
{
    void Log(string message);  // Misma firma que ILogger, pero distinto propósito
}


// ───────────────────────────────────────────────────────────────────────────────
// EJEMPLO 1 — DiagnosticService implementa ambas interfaces explícitamente
//            Así Log() hace una cosa u otra según la referencia usada.
// ───────────────────────────────────────────────────────────────────────────────

class DiagnosticService : ILogger, ITracer
{
    // Implementación explícita de ILogger.Log
    // Solo accesible si la referencia es de tipo ILogger
    void ILogger.Log(string message)
    {
        Console.WriteLine($"[LOG]    {DateTime.Now:HH:mm:ss} | {message}");
    }

    // Implementación explícita de ITracer.Log
    // Solo accesible si la referencia es de tipo ITracer
    void ITracer.Log(string message)
    {
        Console.WriteLine($"[TRACE]  {DateTime.Now:HH:mm:ss.fff} | {message}");
    }

    // Método público propio de la clase — NO colisiona con las interfaces
    public void ReportStatus()
    {
        Console.WriteLine("DiagnosticService está operativo.");
    }
}


// ───────────────────────────────────────────────────────────────────────────────
// INTERFAZ PARA EL EJEMPLO 2 — Ocultación deliberada
// ───────────────────────────────────────────────────────────────────────────────

// Simula IDisposable. En un proyecto real usarías la IDisposable de .NET.
// Aquí la definimos nosotros para ilustrar el concepto sin confundir.
interface IMyDisposable
{
    void Dispose();
}


// ───────────────────────────────────────────────────────────────────────────────
// EJEMPLO 2 — ResourceManager implementa Dispose explícitamente.
//            Expone un método público Close() en su lugar, que internamente
//            llama a Dispose. El consumidor nunca ve Dispose().
// ───────────────────────────────────────────────────────────────────────────────

class ResourceManager : IMyDisposable
{
    private bool _disposed = false;

    // Dispose está OCULTO del API pública de ResourceManager.
    // Solo se puede llamar haciendo cast a IMyDisposable.
    void IMyDisposable.Dispose()
    {
        if (!_disposed)
        {
            Console.WriteLine("  -> Liberando recursos internos...");
            _disposed = true;
        }
    }

    // Close() es la cara pública. Internamente delega en Dispose.
    public void Close()
    {
        Console.WriteLine("Close() llamado.");
        // Convertimos this a IMyDisposable para invocar Dispose explícitamente
        ((IMyDisposable)this).Dispose();
    }

    public void DoWork()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ResourceManager));

        Console.WriteLine("  -> ResourceManager trabajando...");
    }
}


// ───────────────────────────────────────────────────────────────────────────────
// INTERFACES PARA EL EJEMPLO 3 — Múltiples interfaces con Render()
// ───────────────────────────────────────────────────────────────────────────────

interface IDrawable
{
    void Render();  // Dibujar en pantalla
}

interface IPrintable
{
    void Render();  // Renderizar para impresora
}

interface ISaveable
{
    void Render();  // Renderizar para exportar a archivo
}


// ───────────────────────────────────────────────────────────────────────────────
// EJEMPLO 3 — Document implementa 3 interfaces, cada una con Render().
//            Implementación explícita para que cada una haga algo distinto.
// ───────────────────────────────────────────────────────────────────────────────

class Document : IDrawable, IPrintable, ISaveable
{
    public string Title { get; }

    public Document(string title)
    {
        Title = title;
    }

    // Render para dibujar en pantalla (WYSIWYG)
    void IDrawable.Render()
    {
        Console.WriteLine($"  [DIBUJO] Renderizando '{Title}' a 96 DPI para pantalla.");
    }

    // Render para enviar a impresora (alta resolución, CMYK)
    void IPrintable.Render()
    {
        Console.WriteLine($"  [IMPRESIÓN] Renderizando '{Title}' a 300 DPI en CMYK.");
    }

    // Render para exportar (PDF, PNG, etc.)
    void ISaveable.Render()
    {
        Console.WriteLine($"  [EXPORTACIÓN] Serializando '{Title}' a formato portable.");
    }
}


// ───────────────────────────────────────────────────────────────────────────────
// EJEMPLO 4 — Propiedades e indexadores explícitos
// ───────────────────────────────────────────────────────────────────────────────

interface ICounter
{
    int Count { get; }               // Propiedad de solo lectura
    string this[int index] { get; }  // Indexador de solo lectura
}


class LimitedCollection : ICounter
{
    private readonly string[] _items;

    public LimitedCollection(string[] items)
    {
        _items = items;
    }

    // La propiedad Count de la interfaz NO se expone como API pública.
    // Si la clase tuviese su propio Count público, aquí habría ambigüedad y
    // necesitaríamos implementación explícita para diferenciarlos.
    int ICounter.Count => _items.Length;

    // Indexador explícito: solo accesible vía referencia ICounter
    string ICounter.this[int index] => _items[index];

    // Método público de la clase — el consumidor usa este, no el indexador
    public string GetItem(int index) => _items[index];
}
