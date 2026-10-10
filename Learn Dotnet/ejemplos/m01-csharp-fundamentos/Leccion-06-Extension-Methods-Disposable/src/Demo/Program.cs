// ====================================================================
// Lección 06: Extension Methods, IDisposable y IAsyncDisposable
// Demostración completa de métodos de extensión y el patrón Dispose.
// ====================================================================

// ════════════════════════════════════════════════════════════════════
// PARTE 1: EXTENSION METHODS (Métodos de extensión)
// ════════════════════════════════════════════════════════════════════

Console.WriteLine("=== 1. EXTENSION METHODS ===");
Console.WriteLine();

// --------------------------------------------------------------------
// 1.1 Ejemplo básico: WordCount y ToTitleCase para string
// --------------------------------------------------------------------
// Un método de extensión se usa como si fuera un método de instancia,
// pero en realidad es un método estático definido en una clase aparte.
Console.WriteLine("--- 1.1 Extensiones sobre string ---");

string frase = "  hola   mundo   desde .NET 8  ";

// Se invoca como método de instancia (azúcar sintáctico)
int palabras = frase.WordCount();
string titulo = frase.ToTitleCase();

Console.WriteLine($"Frase original:  '{frase}'");
Console.WriteLine($"WordCount():     {palabras} palabras");
Console.WriteLine($"ToTitleCase():   '{titulo}'");
Console.WriteLine();

// --------------------------------------------------------------------
// 1.2 Cómo funciona internamente: el compilador lo traduce
// --------------------------------------------------------------------
// El compilador de C# traduce la llamada de extensión a una llamada
// estática normal. No hay magia en tiempo de ejecución.
Console.WriteLine("--- 1.2 Cómo funciona internamente ---");

// Estas dos líneas generan EXACTAMENTE el mismo IL:
// frase.WordCount()  ← azúcar sintáctico
// StringExtensions.WordCount(frase)  ← llamada real
int viaExtension = frase.WordCount();
int viaEstatica = StringExtensions.WordCount(frase);

Console.WriteLine($"frase.WordCount()                 → {viaExtension}");
Console.WriteLine($"StringExtensions.WordCount(frase) → {viaEstatica}");
Console.WriteLine($"¿Mismo resultado? {viaExtension == viaEstatica}");
Console.WriteLine();

// --------------------------------------------------------------------
// 1.3 Ejemplo con IEnumerable<T>: método Median
// --------------------------------------------------------------------
// Las extensiones también funcionan sobre interfaces genéricas,
// lo que permite extender TODAS las colecciones de una sola vez.
Console.WriteLine("--- 1.3 Extensión sobre IEnumerable<T> (Median) ---");

int[] numeros = { 1, 3, 5, 7, 9 };
double medianaInt = numeros.Median();
Console.WriteLine($"Mediana de [1,3,5,7,9]: {medianaInt}");

double[] precios = { 10.5, 20.0, 30.0, 40.0, 50.0 };
double medianaDouble = precios.Median();
Console.WriteLine($"Mediana de [10.5,20,30,40,50]: {medianaDouble}");

// Con cantidad par: la mediana es el promedio de los dos centrales
int[] pares = { 1, 2, 3, 4 };
double medianaPares = pares.Median();
Console.WriteLine($"Mediana de [1,2,3,4] (par): {medianaPares}");
Console.WriteLine();

// --------------------------------------------------------------------
// 1.4 Encadenamiento de extension methods
// --------------------------------------------------------------------
// Como cada extensión devuelve un valor, se pueden encadenar.
// Esto es la base de la fluidez de LINQ: .Where().Select().OrderBy()...
Console.WriteLine("--- 1.4 Encadenamiento de extension methods ---");

string textoCrudo = "  lorem IPSUM dolor SIT amet  ";

// Trim() es método nativo; ToTitleCase() y WordCount() son extensiones nuestras
string transformado = textoCrudo.Trim().ToTitleCase();
int conteo = transformado.WordCount();

// Encadenamiento en una sola expresión
int resultadoEncadenado = textoCrudo
    .Trim()
    .ToTitleCase()
    .WordCount();

Console.WriteLine($"Original:            '{textoCrudo}'");
Console.WriteLine($"Trim().ToTitleCase(): '{transformado}'");
Console.WriteLine($"WordCount():          {conteo}");
Console.WriteLine($"Encadenado directo:   {resultadoEncadenado}");
Console.WriteLine();

// --------------------------------------------------------------------
// 1.5 Cuándo NO usar extension methods
// --------------------------------------------------------------------
Console.WriteLine("--- 1.5 Cuándo NO usar extension methods ---");
Console.WriteLine("""
    EVITAR métodos de extensión cuando:
    
    ❌ Extender 'object': contamina IntelliSense de TODOS los tipos.
    ❌ Ocultar métodos existentes del tipo original.
    ❌ Lógica de negocio con dependencias: usar inyección de dependencias.
    ❌ El método necesita acceder a miembros privados (imposible).
    ❌ Extender tipos que no controlas y pueden añadir ese método en el futuro.
    
    Ejemplo INCORRECTO:
      public static void Print(this object o) { ... }
      // Ahora 42.Print(), "hola".Print(), TODO tiene .Print()
    """);
Console.WriteLine();

// --------------------------------------------------------------------
// 1.6 Comparación con JavaScript y Kotlin
// --------------------------------------------------------------------
Console.WriteLine("--- 1.6 Comparación con otros lenguajes ---");
Console.WriteLine("""
    ┌─────────────────┬──────────────────────┬──────────────────────────┐
    │ Lenguaje        │ Mecanismo            │ Ejemplo                  │
    ├─────────────────┼──────────────────────┼──────────────────────────┤
    │ C#              │ static class + this  │ this string source       │
    │ JavaScript      │ Prototype            │ String.prototype.fn = .. │
    │ Kotlin          │ Extension functions  │ fun String.metodo()      │
    │ Swift           │ Extensions           │ extension String { }     │
    └─────────────────┴──────────────────────┴──────────────────────────┘
    
    JavaScript modifica el prototipo (mutable, riesgo en producción).
    Kotlin es muy similar a C#: funciones estáticas con azúcar sintáctico.
    C# NO modifica el tipo original; es pura sintaxis en tiempo de compilación.
    """);

Console.WriteLine();
Console.WriteLine("=== FIN PARTE 1 ===\n");

// ════════════════════════════════════════════════════════════════════
// PARTE 2: IDISPOSABLE E IAsyncDisposable
// ════════════════════════════════════════════════════════════════════

Console.WriteLine("=== 2. IDISPOSABLE E IAsyncDisposable ===");
Console.WriteLine();

// --------------------------------------------------------------------
// 2.1 ¿Por qué existe IDisposable?
// --------------------------------------------------------------------
Console.WriteLine("--- 2.1 ¿Por qué existe IDisposable? ---");
Console.WriteLine("""
    El Garbage Collector (GC) libera MEMORIA automáticamente.
    Pero no libera recursos externos: archivos, sockets, conexiones a BD,
    handles del sistema operativo, mutex, etc.
    
    IDisposable permite liberar esos recursos DE FORMA DETERMINISTA:
    tú decides cuándo, no el GC.
    
    Sin Dispose: el recurso se libera cuando el GC ejectua el finalizador
                 (~destructor). Puede ser segundos, minutos o NUNCA.
    Con Dispose: liberas el recurso inmediatamente al terminar de usarlo.
    """);
Console.WriteLine();

// --------------------------------------------------------------------
// 2.2 using statement (tradicional, con llaves)
// --------------------------------------------------------------------
Console.WriteLine("--- 2.2 using statement (tradicional, con llaves) ---");

// using con bloque: Dispose() se llama al salir de las llaves
// Aunque ocurra una excepción, el compilador genera un try-finally
using (var recurso = new FileSimulator("archivo.log"))
{
    recurso.Write("Línea 1: Hola");
    recurso.Write("Línea 2: Mundo");
} // ← Dispose() se llama AQUÍ automáticamente

Console.WriteLine("(Fuera del bloque using — recurso ya liberado)");
Console.WriteLine();

// --------------------------------------------------------------------
// 2.3 using declaration (C# 8+, sin llaves)
// --------------------------------------------------------------------
Console.WriteLine("--- 2.3 using declaration (C# 8+) ---");

// using declaration: el recurso se libera al final del SCOPE actual
// (método, función, bloque más externo). Más limpio, menos indentación.
using var recurso2 = new FileSimulator("datos.txt");
recurso2.Write("Dato 1");
recurso2.Write("Dato 2");
// Sin llaves. Dispose() se invoca al salir del scope que contiene esta variable.
Console.WriteLine("(El recurso se liberará al final del scope actual)\n");

// Para demostrar el scope, creamos un bloque anidado
{
    using var scoped = new FileSimulator("temporal.tmp");
    scoped.Write("Dato temporal");
    Console.WriteLine("  (Saliendo del bloque anidado — scoped se libera aquí)");
} // ← Dispose de 'scoped' se llama aquí
Console.WriteLine("(De vuelta al scope principal)\n");

// --------------------------------------------------------------------
// 2.4 El patrón Dispose completo: Dispose(bool)
// --------------------------------------------------------------------
Console.WriteLine("--- 2.4 Patrón Dispose(bool) ---");
Console.WriteLine("""
    El patrón canónico de Dispose en .NET separa dos caminos:
    
    ┌──────────────────────────────────────────────────────────┐
    │ public void Dispose()                                    │
    │ {                                                        │
    │     Dispose(true);        ← disposing = true (manual)    │
    │     GC.SuppressFinalize(this); ← evita doble limpieza    │
    │ }                                                        │
    │                                                          │
    │ protected virtual void Dispose(bool disposing)           │
    │ {                                                        │
    │     if (_disposed) return;                               │
    │                                                          │
    │     if (disposing)                                       │
    │     {                                                    │
    │         // Liberar recursos GESTIONADOS (otros IDisp.)   │
    │         _stream?.Dispose();                              │
    │     }                                                    │
    │                                                          │
    │     // Liberar recursos NO GESTIONADOS (handles nativos) │
    │     CloseHandle(_nativeHandle);                          │
    │                                                          │
    │     _disposed = true;                                    │
    │ }                                                        │
    │                                                          │
    │ ~MiClase()  ← finalizador (SÓLO si hay handles nativos)  │
    │ {                                                        │
    │     Dispose(false); ← disposing = false (GC)             │
    │ }                                                        │
    └──────────────────────────────────────────────────────────┘
    
    disposing=true : llamado por el usuario (usando using o .Dispose())
    disposing=false: llamado por el GC desde el finalizador
    """);

// Demostración práctica con DisposeTracker
var tracker = new DisposeTracker();
Console.WriteLine($"¿Disposed antes del using? {tracker.IsDisposed}");

using (tracker)
{
    Console.WriteLine($"  ¿Disposed dentro del using? {tracker.IsDisposed}");
}
Console.WriteLine($"¿Disposed después del using? {tracker.IsDisposed}");

// La propiedad IsDisposed se actualiza por el patrón Dispose(bool)
Console.WriteLine();

// --------------------------------------------------------------------
// 2.5 IAsyncDisposable y await using (C# 8+)
// --------------------------------------------------------------------
Console.WriteLine("--- 2.5 IAsyncDisposable y await using ---");

// await using: necesario cuando la liberación del recurso es asíncrona
// (por ejemplo, flush de buffers a BD o cierre de conexión de red)
await using (var asyncDb = new AsyncDatabaseConnection())
{
    await asyncDb.WriteAsync("INSERT INTO users VALUES (1, 'Ana')");
} // ← DisposeAsync() se llama AQUÍ, de forma asíncrona

Console.WriteLine("(Después del await using)\n");

// También existe la declaración await using (sin llaves):
await using var asyncDb2 = new AsyncDatabaseConnection();
await asyncDb2.WriteAsync("SELECT * FROM users");
Console.WriteLine("(Se liberará al final del scope actual)\n");

// --------------------------------------------------------------------
// 2.6 Tipos comunes que implementan IDisposable
// --------------------------------------------------------------------
Console.WriteLine("--- 2.6 Tipos IDisposable / IAsyncDisposable comunes ---");
Console.WriteLine("""
    ┌─────────────────────────────┬────────────────────────────────────┐
    │ Tipo                        │ Recurso que gestiona               │
    ├─────────────────────────────┼────────────────────────────────────┤
    │ FileStream                  │ Handle de archivo del SO           │
    │ StreamReader / StreamWriter │ Buffer + stream subyacente         │
    │ SqlConnection / DbConnection│ Conexión a base de datos           │
    │ HttpClient                  │ Conexiones HTTP (pool de sockets)  │
    │ Timer / PeriodicTimer       │ Timer del sistema operativo        │
    │ CancellationTokenSource     │ Registro de tokens de cancelación  │
    │ IAsyncDisposable            │ Recursos que requieren Dispose     │
    │                             │   asíncrono (Ej: EF Core DbContext)│
    └─────────────────────────────┴────────────────────────────────────┘
    """);

// --------------------------------------------------------------------
// 2.7 ¿Qué pasa si NO haces Dispose?
// --------------------------------------------------------------------
Console.WriteLine("--- 2.7 Consecuencias de no llamar a Dispose ---");
Console.WriteLine("""
    FUGA DE RECURSOS (lo que ocurre en producción):
    
    1. FileStream sin Dispose → handle de archivo bloqueado
       Error: "The process cannot access the file because it is
               being used by another process."
    
    2. SqlConnection sin Dispose → pool de conexiones agotado
       Error: "Timeout expired. The timeout period elapsed prior to
               obtaining a connection from the pool."
    
    3. HttpClient sin Dispose → socket exhaustion
       Error: "Only one usage of each socket address is normally
               permitted" o "Unable to connect to the remote server"
    
    4. Timer sin Dispose → callback sobre objeto recolectado
       Error: NullReferenceException aleatoria e intermitente
    
    En desarrollo puede no notarse porque el GC limpia rápido con poca
    carga. En producción, con miles de requests, aparece en horas/días.
    Es uno de los bugs más difíciles de diagnosticar porque no es
    reproducible en local.
    """);

Console.WriteLine();
Console.WriteLine("=== FIN PARTE 2 ===\n");

// ════════════════════════════════════════════════════════════════════
// RESUMEN
// ════════════════════════════════════════════════════════════════════

Console.WriteLine("=== RESUMEN DE LA LECCIÓN ===");
Console.WriteLine("""
    EXTENSION METHODS:
    ┌─────────────────────────────────────────────────────────────────┐
    │ • Métodos estáticos que se invocan como si fueran de instancia  │
    │ • Sintaxis: static void Metodo(this TipoExtendido param, ...)   │
    │ • Compilador traduce: obj.Metodo() → ClaseEstatica.Metodo(obj)  │
    │ • No acceden a miembros privados del tipo extendido             │
    │ • Base de LINQ: .Where(), .Select(), .OrderBy() son extensiones │
    │ • Alternativa a herencia cuando no controlas el tipo original   │
    └─────────────────────────────────────────────────────────────────┘
    
    IDisposable / IAsyncDisposable:
    ┌─────────────────────────────────────────────────────────────────┐
    │ • IDisposable: liberación DETERMINISTA de recursos externos     │
    │ • using statement (tradicional): Dispose al salir de las llaves │
    │ • using declaration (C# 8+): Dispose al final del scope        │
    │ • await using (C# 8+): Dispose asíncrono (IAsyncDisposable)    │
    │ • Dispose(bool) pattern: separa limpieza manual vs GC           │
    │ • SIEMPRE usa using/await using con: Stream, DbConnection,      │
    │   HttpClient, Timer, CancellationTokenSource, etc.              │
    └─────────────────────────────────────────────────────────────────┘
    """);

// ════════════════════════════════════════════════════════════════════
// TIPOS PÚBLICOS
// Definidos en el mismo archivo para que los tests puedan referenciarlos.
// ════════════════════════════════════════════════════════════════════

// ────────────────────────────────────────────────────────────────────
// Métodos de extensión para string
// ────────────────────────────────────────────────────────────────────
// Requisitos para un método de extensión:
//   1. Clase estática (static class)
//   2. Método estático (static)
//   3. Primer parámetro con 'this' (this Tipo nombre)
public static class StringExtensions
{
    /// <summary>
    /// Cuenta el número de palabras en un string, ignorando espacios múltiples.
    /// </summary>
    public static int WordCount(this string source)
    {
        // StringSplitOptions.RemoveEmptyEntries ignora entradas vacías
        // producidas por espacios consecutivos
        return source.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }

    /// <summary>
    /// Convierte un string a formato título: primera letra de cada palabra
    /// en mayúscula, el resto en minúscula.
    /// </summary>
    public static string ToTitleCase(this string source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return source;

        var palabras = source.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < palabras.Length; i++)
        {
            if (palabras[i].Length > 0)
            {
                // char.ToUpper + slice [1..] para capitalizar cada palabra
                palabras[i] = char.ToUpper(palabras[i][0]) + palabras[i][1..].ToLower();
            }
        }
        return string.Join(' ', palabras);
    }
}

// ────────────────────────────────────────────────────────────────────
// Métodos de extensión para IEnumerable<T>
// ────────────────────────────────────────────────────────────────────
public static class EnumerableExtensions
{
    /// <summary>
    /// Calcula la mediana de una secuencia de enteros.
    /// Cantidad impar → elemento central. Cantidad par → promedio de los dos centrales.
    /// </summary>
    public static double Median(this IEnumerable<int> source)
    {
        var sorted = source.OrderBy(x => x).ToArray();
        int count = sorted.Length;

        if (count == 0)
            return 0;

        if (count % 2 == 1)
            return sorted[count / 2];
        else
            return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
    }

    /// <summary>
    /// Calcula la mediana de una secuencia de doubles (sobrecarga).
    /// </summary>
    public static double Median(this IEnumerable<double> source)
    {
        var sorted = source.OrderBy(x => x).ToArray();
        int count = sorted.Length;

        if (count == 0)
            return 0;

        if (count % 2 == 1)
            return sorted[count / 2];
        else
            return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
    }
}

// ────────────────────────────────────────────────────────────────────
// Simulador de archivo que implementa IDisposable
// ────────────────────────────────────────────────────────────────────
// Esta clase envuelve un recurso simulado y lo libera en Dispose().
// En un caso real, aquí habría un FileStream, SqlConnection, etc.
public class FileSimulator : IDisposable
{
    private bool _disposed;
    public string Name { get; }
    public bool IsDisposed => _disposed;

    public FileSimulator(string name)
    {
        Name = name;
        Console.WriteLine($"  [FileSimulator] Abriendo '{name}'...");
    }

    public void Write(string content)
    {
        // ObjectDisposedException es la excepción estándar cuando se usa
        // un objeto que ya ha sido liberado con Dispose
        ObjectDisposedException.ThrowIf(_disposed, this);
        Console.WriteLine($"  [FileSimulator] Escribiendo: {content}");
    }

    /// <summary>
    /// Libera el recurso. Debe ser seguro llamarlo múltiples veces (idempotente).
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            Console.WriteLine($"  [FileSimulator] Cerrando '{Name}' (Dispose llamado)");
        }
    }
}

// ────────────────────────────────────────────────────────────────────
// Rastreador de Dispose con el patrón Dispose(bool) completo
// ────────────────────────────────────────────────────────────────────
// Esta clase implementa el patrón canónico de Dispose que se usa en
// toda la BCL de .NET. Es útil para tests: podemos verificar si se llamó.
public class DisposeTracker : IDisposable
{
    private bool _disposed;
    public bool IsDisposed => _disposed;
    public int DisposeCount { get; private set; }

    /// <summary>
    /// Implementación pública de IDisposable.
    /// Llama a Dispose(true) y suprime el finalizador.
    /// </summary>
    public void Dispose()
    {
        // Dispose(true): disposing = true → llamado manualmente (no desde GC)
        Dispose(true);
        // GC.SuppressFinalize evita que el GC llame al finalizador,
        // mejorando el rendimiento (el objeto no pasa a la cola de finalización)
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Método protegido virtual que sigue el patrón Dispose(bool).
    /// </summary>
    /// <param name="disposing">
    /// true  → llamado por el usuario (using, .Dispose())
    /// false → llamado por el GC desde el finalizador (~destructor)
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return; // Idempotente: si ya está liberado, no hace nada

        if (disposing)
        {
            // Aquí se liberan los recursos GESTIONADOS (otros IDisposable)
            // Ejemplo: _stream?.Dispose(); _connection?.Dispose();
        }

        // Aquí se liberan los recursos NO GESTIONADOS (handles nativos)
        // Ejemplo: CloseHandle(_nativeHandle);

        _disposed = true;
        DisposeCount++;
    }
}

// ────────────────────────────────────────────────────────────────────
// Simulador de conexión a BD asíncrona (IAsyncDisposable)
// ────────────────────────────────────────────────────────────────────
// IAsyncDisposable se usa cuando el Dispose necesita operaciones
// asíncronas: flush de buffers, cierre de conexiones de red, etc.
public class AsyncDatabaseConnection : IAsyncDisposable
{
    private bool _disposed;
    public bool IsDisposed => _disposed;
    public int DisposeAsyncCount { get; private set; }

    public AsyncDatabaseConnection()
    {
        Console.WriteLine("  [AsyncDB] Abriendo conexión asíncrona...");
    }

    public async Task WriteAsync(string query)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Console.WriteLine($"  [AsyncDB] Ejecutando query: {query}");
        // Simula latencia de red / base de datos
        await Task.Delay(100);
    }

    /// <summary>
    /// Libera el recurso de forma asíncrona.
    /// ValueTask (struct) evita heap allocation cuando DisposeAsync
    /// es síncrono o muy rápido.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            _disposed = true;
            Console.WriteLine("  [AsyncDB] Cerrando conexión asíncrona (flush de buffers)...");
            await Task.Delay(50); // Simula flush asíncrono de buffers
            DisposeAsyncCount++;
        }
    }
}
