// ====================================================================
// LECCIÓN 04-04 — IAsyncEnumerable<T>
// C# Avanzado — Streaming asíncrono de datos
// ====================================================================
// ¿Qué es IAsyncEnumerable<T>?
//   La versión asíncrona de IEnumerable<T>. Mientras IEnumerable<T> bloquea
//   el hilo actual mientras itera, IAsyncEnumerable<T> permite esperar
//   cada elemento de forma asíncrona con await foreach.
//
// ¿Por qué existe?
//   IEnumerable<T> devuelve todos los elementos sincrónicamente. Si la fuente
//   es lenta (BD, API remota, archivo grande), tienes que:
//     a) Bloquear el hilo hasta tenerlo todo → degrada la experiencia
//     b) Devolver Task<List<T>> → esperas todo y solo entonces iteras
//   IAsyncEnumerable<T> resuelve esto: produces CADA elemento de forma
//   asíncrona y el consumidor lo procesa SIN esperar al resto.
//
// Comparación clave:
//   Task<List<T>>   → "Dame todo. Espero. Ahora proceso."
//   IAsyncEnumerable → "Dame uno. Lo proceso. Dame el siguiente."
//
// Similar a:
//   - Node.js: Readable Stream con async iteration (for await...of)
//   - Python:  async for sobre un AsyncIterator (PEP 525)
//   - Java:    no tiene equivalente nativo; librerías como Reactive Streams
// ====================================================================

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CSharpAsyncEnumerable;

public static class Program
{
    public static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║   IAsyncEnumerable<T> — Streaming asíncrono          ║");
        Console.WriteLine("║   Procesa datos a medida que llegan                  ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine();

        await DemostrarYieldReturnAsync();
        await DemostrarTaskListVsAsyncEnumerable();
        await DemostrarLecturaArchivoAsync();
        await DemostrarPaginatedDataSourceAsync();
        await DemostrarCancellationTokenAsync();
        await DemostrarConfigureAwaitAsync();
    }

    // ─── 1. yield return en métodos async ────────────────────────────
    // async + IAsyncEnumerable<T> permite usar await DENTRO del generador.
    // Cada yield return emite un elemento; entre yields, puedes hacer await.
    private static async Task DemostrarYieldReturnAsync()
    {
        Console.WriteLine("─── 1. async IAsyncEnumerable + yield return ───");
        Console.WriteLine("Cada número se genera tras un delay simulado\n");

        // Consumimos el stream: cada elemento aparece tras 200ms de delay
        await foreach (int numero in GenerarNumerosAsync(5))
        {
            Console.WriteLine($"  -> Recibido: {numero,2}  [{DateTime.Now:HH:mm:ss.fff}]");
        }

        Console.WriteLine("  Fin del stream.\n");
    }

    // Método generador asíncrono: produce números con delay entre cada uno.
    // El consumidor recibe CADA elemento sin esperar a que se generen todos.
    private static async IAsyncEnumerable<int> GenerarNumerosAsync(
        int count,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 1; i <= count; i++)
        {
            // Simula trabajo asíncrono: consulta a BD, llamada HTTP, etc.
            await Task.Delay(200, ct);
            yield return i; // Emite el elemento y PAUSA hasta que el consumidor pida el siguiente
        }
    }

    // ─── 2. Task<List<T>> vs IAsyncEnumerable<T> ──────────────────────
    // Demostración visual de la diferencia fundamental entre ambos enfoques.
    private static async Task DemostrarTaskListVsAsyncEnumerable()
    {
        Console.WriteLine("─── 2. Task<List<T>> vs IAsyncEnumerable<T> ───");
        Console.WriteLine("Comparación visual: todo de golpe vs streaming\n");

        // ── Enfoque A: Task<List<int>> ──
        // Esperas a que TODOS los datos estén listos y luego los procesas.
        Console.WriteLine("  [Enfoque A] Task<List<int>>: esperar todo, procesar después...");
        Stopwatch swList = Stopwatch.StartNew();
        List<int> todos = await GenerarListaAsync(5); // Bloquea hasta tener los 5
        swList.Stop();
        Console.WriteLine($"  -> Recibida lista de {todos.Count} elementos. Tiempo de espera: {swList.ElapsedMilliseconds}ms");
        Console.WriteLine($"  -> Ahora puedo iterar: [{string.Join(", ", todos)}]");
        Console.WriteLine();

        // ── Enfoque B: IAsyncEnumerable<int> ──
        // Procesas cada elemento en cuanto está disponible.
        Console.WriteLine("  [Enfoque B] IAsyncEnumerable<int>: procesar cada elemento al llegar...");
        Stopwatch swStream = Stopwatch.StartNew();
        await foreach (int numero in GenerarNumerosAsync(5))
        {
            Console.WriteLine($"  -> Recibido #{numero} a los {swStream.ElapsedMilliseconds}ms (sin esperar al resto)");
        }
        swStream.Stop();
        Console.WriteLine($"  -> Tiempo total del stream: {swStream.ElapsedMilliseconds}ms");
        Console.WriteLine("  -> No hubo que esperar al último para procesar el primero.\n");
    }

    // Método que devuelve Task<List<T>>: genera todo y lo devuelve junto.
    private static async Task<List<int>> GenerarListaAsync(int count)
    {
        List<int> result = new(count);
        for (int i = 1; i <= count; i++)
        {
            await Task.Delay(200);
            result.Add(i);
        }
        return result;
    }

    // ─── 3. Ejemplo real: lectura de archivo línea a línea ────────────
    // Simula leer un log remoto donde cada línea tarda en llegar.
    private static async Task DemostrarLecturaArchivoAsync()
    {
        Console.WriteLine("─── 3. Lectura simulada de archivo remoto ───");

        // Simulamos 10 líneas de log, cada una tarda 150ms en "transmitirse"
        Console.WriteLine("  Leyendo líneas de log (150ms por línea)...\n");
        await foreach (string linea in LeerLineasLogSimuladoAsync(10))
        {
            Console.WriteLine($"  📄 {linea}");
        }

        Console.WriteLine("  Fin de la lectura.\n");
    }

    /// <summary>
    /// Simula leer un archivo de log línea a línea desde una fuente lenta.
    /// En un caso real, usarías StreamReader.ReadLineAsync().
    /// </summary>
    private static async IAsyncEnumerable<string> LeerLineasLogSimuladoAsync(
        int lineCount,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 1; i <= lineCount; i++)
        {
            await Task.Delay(150, ct); // Simula latencia de red/disco
            yield return $"[2026-07-29 10:{i:D2}:00] INFO  RequestHandler - Procesada solicitud #{i:D4}";
        }
    }

    // ─── 4. Ejemplo real: fuente de datos paginada ────────────────────
    // Simula una API que devuelve páginas de 10 elementos.
    // El consumidor no sabe (ni necesita saber) que hay paginación interna.
    private static async Task DemostrarPaginatedDataSourceAsync()
    {
        Console.WriteLine("─── 4. Fuente de datos paginada (simula API/BD) ───");
        Console.WriteLine("  El consumidor ve un stream; internamente se pagina de 10 en 10\n");

        PaginatedDataSource dataSource = new(totalItems: 25, pageSize: 10);

        int contador = 0;
        await foreach (string item in dataSource.GetItemsAsync())
        {
            contador++;
            Console.WriteLine($"  [{contador,2}] {item}");
        }

        Console.WriteLine($"  Total procesados: {contador} elementos, desde {dataSource.PageCount} páginas.\n");
    }

    // ─── 5. CancellationToken con IAsyncEnumerable ────────────────────
    // Muestra cómo cancelar un stream asíncrono desde fuera.
    private static async Task DemostrarCancellationTokenAsync()
    {
        Console.WriteLine("─── 5. CancellationToken con IAsyncEnumerable ───");
        Console.WriteLine("  Cancelamos el stream a los 500ms...\n");

        using CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(500));

        try
        {
            // .WithCancellation(cts.Token) enlaza el token al await foreach
            // [EnumeratorCancellation] en el método generador permite que el
            // compilador pase el token automáticamente al IAsyncEnumerator.
            await foreach (int numero in GenerarNumerosAsync(10).WithCancellation(cts.Token))
            {
                Console.WriteLine($"  -> {numero}");
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("  ⚠️  Stream cancelado tras 500ms. No se procesaron todos los elementos.\n");
        }
    }

    // ─── 6. ConfigureAwait(false) en librerías ────────────────────────
    // En librerías (no en UI/app principal), se usa ConfigureAwait(false)
    // para evitar volver al SynchronizationContext original.
    private static async Task DemostrarConfigureAwaitAsync()
    {
        Console.WriteLine("─── 6. ConfigureAwait con IAsyncEnumerable ───");
        Console.WriteLine("  En librerías, usar ConfigureAwait(false)\n");

        // ConfigureAwait(false) clásico en Task:
        //   await SomeTask().ConfigureAwait(false);

        // Con IAsyncEnumerable (.NET 8), se usa el método de extensión ConfigureAwait:
        //   await foreach (var x in source.ConfigureAwait(false))
        // Devuelve ConfiguredCancelableAsyncEnumerable<T> con continueOnCapturedContext = false.
        // (.NET 9 añade WithConfigureAwaitOptions con más granularidad.)

        // Demostración: el ConfigureAwait no cambia el comportamiento visible,
        // pero internamente evita capturar el SynchronizationContext.
        await foreach (int numero in GenerarNumerosAsync(3)
            .ConfigureAwait(false))
        {
            Console.WriteLine($"  -> Elemento: {numero} (ConfigureAwait(false))");
        }

        Console.WriteLine("  ℹ️  En app de consola no hay SynchronizationContext, da igual.");
        Console.WriteLine("  ℹ️  Pero en WinForms/WPF/ASP.NET sí importa para evitar deadlocks.\n");
    }
}

// ====================================================================
// CLASE AUXILIAR: Fuente de datos paginada simulada
// ====================================================================
// Esta clase oculta la paginación al consumidor.
// El consumidor ve un IAsyncEnumerable<string> limpio.
// Internamente, se pide página a página de 10 elementos.
// ====================================================================
public class PaginatedDataSource
{
    private readonly int _totalItems;
    private readonly int _pageSize;

    public PaginatedDataSource(int totalItems, int pageSize)
    {
        _totalItems = totalItems;
        _pageSize = pageSize;
    }

    // Número total de páginas (solo para mostrar en el demo)
    public int PageCount => (int)Math.Ceiling((double)_totalItems / _pageSize);

    /// <summary>
    /// Devuelve un stream asíncrono de todos los items.
    /// El consumidor NO sabe que internamente se pagina.
    /// </summary>
    public async IAsyncEnumerable<string> GetItemsAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        int itemsRestantes = _totalItems;
        int paginaActual = 0;

        while (itemsRestantes > 0)
        {
            paginaActual++;
            int itemsEnPagina = Math.Min(_pageSize, itemsRestantes);

            // Simula una llamada a API externa o BD para obtener la página
            // Pasamos el índice del primer item de esta página
            int primerItem = _totalItems - itemsRestantes + 1;
            List<string> pagina = await ObtenerPaginaAsync(paginaActual, itemsEnPagina, primerItem, ct);

            foreach (string item in pagina)
            {
                yield return item;
            }

            itemsRestantes -= itemsEnPagina;
        }
    }

    /// <summary>
    /// Simula obtener una página desde una fuente remota (BD, API, etc).
    /// En producción, esto sería un DbContext.ToListAsync() o un HttpClient.GetAsync().
    /// </summary>
    private static async Task<List<string>> ObtenerPaginaAsync(
        int pagina,
        int cantidad,
        int primerItem,
        CancellationToken ct)
    {
        // Simula latencia de red
        await Task.Delay(100, ct);

        List<string> items = new(cantidad);
        for (int i = 0; i < cantidad; i++)
        {
            // Simulamos items con nombres
            items.Add($"Item-{primerItem + i:D4}");
        }

        Console.WriteLine($"     [Interno] Obtenida página {pagina} con {cantidad} items");
        return items;
    }
}

// ====================================================================
// CLASE AUXILIAR: IAsyncDisposable con IAsyncEnumerable
// ====================================================================
// Cuando un IAsyncEnumerable usa recursos (conexión BD, archivo, socket),
// implementa IAsyncDisposable para liberarlos al terminar.
// El consumidor usa `await using` para garantizar la liberación.
// ====================================================================
public class DatabaseStream : IAsyncEnumerable<string>, IAsyncDisposable
{
    private bool _disposed;

    /// <summary>
    /// Simula una conexión a BD que debe cerrarse al terminar.
    /// </summary>
    public async IAsyncEnumerator<string> GetAsyncEnumerator(
        CancellationToken ct = default)
    {
        // Simula abrir conexión
        Console.WriteLine("  [BD] Conexión abierta.");

        for (int i = 1; i <= 3; i++)
        {
            if (ct.IsCancellationRequested)
                yield break;

            await Task.Delay(100, ct); // Simula query a BD
            yield return $"Registro {i} desde BD";
        }

        // La liberación real se hace en DisposeAsync(), no aquí.
        // El consumidor debe usar await using.
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        // Simula cerrar conexión
        await Task.Delay(50); // Simula latencia de cierre
        Console.WriteLine("  [BD] Conexión cerrada (DisposeAsync).");
        GC.SuppressFinalize(this);
    }
}

// ====================================================================
// RESUMEN DE LA LECCIÓN
// ====================================================================
// 1. IAsyncEnumerable<T> = IEnumerable<T> + async para streaming
// 2. await foreach itera elemento a elemento SIN bloquear
// 3. yield return + await dentro del generador async
// 4. Task<List<T>> → esperas todo, luego procesas (bloqueante mentalmente)
// 5. IAsyncEnumerable → procesas cada elemento al llegar (reactivo)
// 6. CancellationToken con .WithCancellation() y [EnumeratorCancellation]
// 7. ConfigureAwaitOptions.None para librerías
// 8. IAsyncDisposable combinado con IAsyncEnumerable para liberar recursos
//
// Cuándo usar IAsyncEnumerable<T>:
//   - Lectura de archivos grandes línea a línea
//   - Consultas a BD con streaming (EF Core: AsAsyncEnumerable)
//   - APIs paginadas donde no quieres cargar todo en memoria
//   - Eventos en tiempo real (WebSockets, colas de mensajes)
//   - Procesamiento de datos que no caben en RAM
//
// Cuándo NO usar:
//   - Pocos elementos (Task<List<T>> es más simple)
//   - Necesitas todos los elementos para ordenar/agrupar (no puedes sin cargarlos)
//   - El consumidor solo hace await de cada elemento sin procesarlo (sin beneficio)
// ====================================================================
