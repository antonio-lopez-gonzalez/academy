using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Demo;

// ============================================================================
// Lección 05 — Async/Await en .NET 8 + C# 12
// ============================================================================
// Este programa demuestra el modelo de programación asíncrona de .NET:
//   - Task / Task<T> como unidad de trabajo futura
//   - async / await y la máquina de estados que genera el compilador
//   - Task.Delay, Task.Run, Task.FromResult, Task.CompletedTask
//   - Task.WhenAll vs Task.WhenAny
//   - ValueTask<T>: evitar alocaciones en el heap
//   - IAsyncEnumerable<T> con await foreach (streaming)
//   - ConfigureAwait(false) para librerías
//   - CancellationToken / CancellationTokenSource
//   - TaskCompletionSource<T> para envolver callbacks
//   - Parallel.ForEachAsync (.NET 6+)
//   - Manejo de excepciones: AggregateException, try-catch con await
//   - Comparación: C# async/await vs Promises (JS) vs asyncio (Python)
// ============================================================================

// ============================================================================
// Clase estática con métodos expuestos para testing y demostración
// ============================================================================
public static class AsyncDemo
{
    /// <summary>
    /// Simula una operación asíncrona que devuelve Task&lt;int&gt;.
    /// Task&lt;T&gt; es la promesa de un valor futuro de tipo T.
    /// </summary>
    public static async Task<int> FetchDataAsync(int value)
    {
        // Task.Delay no bloquea el hilo: devuelve el control al caller
        await Task.Delay(10);
        return value * 2;
    }

    /// <summary>
    /// Task.FromResult: cuando el resultado ya está disponible de forma síncrona.
    /// No asigna un hilo extra ni crea una máquina de estados innecesaria.
    /// </summary>
    public static Task<int> GetCachedValueAsync(int value) => Task.FromResult(value * 2);

    /// <summary>
    /// Task.CompletedTask: devuelve una tarea ya completada sin valor.
    /// Útil en interfaces async que no necesitan trabajo real.
    /// </summary>
    public static Task LogAsync(string message)
    {
        Console.WriteLine($"  [LOG] {message}");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Task.WhenAll: espera a que TODAS las tareas terminen.
    /// Devuelve un array con los resultados en el mismo orden de entrada.
    /// Si alguna falla, WhenAll lanza AggregateException (o la primera TaskCanceledException).
    /// </summary>
    public static async Task<int[]> FetchAllAsync(int[] values)
    {
        var tasks = values.Select(v => FetchDataAsync(v));
        return await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Task.WhenAny: espera a que la PRIMERA tarea termine.
    /// Las demás siguen ejecutándose en segundo plano.
    /// Pensar en: timeout, escoger el servidor más rápido, etc.
    /// </summary>
    public static async Task<int> FetchFirstAsync(int delayMs1, int delayMs2)
    {
        var task1 = Task.Delay(delayMs1).ContinueWith(_ => 1);
        var task2 = Task.Delay(delayMs2).ContinueWith(_ => 2);

        var completed = await Task.WhenAny(task1, task2);
        return await completed; // Desenvuelve el Task<int> al int
    }

    /// <summary>
    /// ValueTask&lt;T&gt;: alternativa a Task&lt;T&gt; que puede ser struct o class.
    /// Si el resultado es síncrono, ValueTask es un struct (0 alocaciones en heap).
    /// Si es asíncrono, envuelve un Task internamente.
    /// Regla: solo usar en métodos donde el camino caliente es síncrono.
    /// No hacer await dos veces al mismo ValueTask.
    /// </summary>
    public static ValueTask<int> FetchValueTaskAsync(int value)
    {
        // Camino síncrono: no asigna Task en el heap
        if (value < 0)
            return ValueTask.FromResult(0);

        // Camino asíncrono: delega a un Task real
        return new ValueTask<int>(FetchDataAsync(value));
    }

    /// <summary>
    /// IAsyncEnumerable&lt;T&gt;: streaming asíncrono con await foreach.
    /// Equivalente a IEnumerable&lt;Task&lt;T&gt;&gt; pero con control de contrapresión.
    /// El consumidor decide cuándo pedir el siguiente elemento (pull model).
    /// </summary>
    public static async IAsyncEnumerable<int> StreamDataAsync(
        int count,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 0; i < count; i++)
        {
            await Task.Delay(10, ct); // El CancellationToken respeta la cancelación
            yield return i;           // yield devuelve el control al await foreach
        }
    }

    /// <summary>
    /// Método async que lanza una excepción.
    /// La excepción se captura dentro del Task y NO se propaga
    /// hasta que se hace await (o se accede a .Result / .Exception).
    /// </summary>
    public static async Task<int> ThrowAsync()
    {
        await Task.Delay(10);
        throw new InvalidOperationException("Error simulado dentro de un método async");
    }

    /// <summary>
    /// TaskCompletionSource&lt;T&gt;: crea un Task manualmente,
    /// ideal para envolver APIs basadas en callbacks o eventos.
    /// Es el "puente" entre el modelo EAP/APM y TAP.
    /// </summary>
    public static Task<int> WrapCallbackAsync(int value)
    {
        var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

        // Simulamos un callback que completa la tarea desde otro hilo
        Task.Run(async () =>
        {
            await Task.Delay(10);
            tcs.SetResult(value * 2); // Completa el Task con el resultado
        });

        return tcs.Task;
    }

    /// <summary>
    /// Operación cancelable mediante CancellationToken.
    /// El token se propaga por toda la cadena de tareas.
    /// </summary>
    public static async Task<int> CancellableOperationAsync(int delayMs, CancellationToken ct)
    {
        // Task.Delay lanza TaskCanceledException si el token se cancela
        await Task.Delay(delayMs, ct);
        return 42;
    }

    /// <summary>
    /// Simula una operación que lanza múltiples excepciones en paralelo.
    /// Demuestra AggregateException y cómo aplanarla.
    /// </summary>
    public static async Task ThrowMultipleAsync()
    {
        var task1 = Task.Run(() => throw new ArgumentException("Error A"));
        var task2 = Task.Run(() => throw new InvalidOperationException("Error B"));

        // Sin await aquí, la excepción se capturará en el Task
        await Task.WhenAll(task1, task2); // Lanza una de las excepciones al await
    }
}

// ============================================================================
// Programa principal: ejecuta todas las demostraciones secuencialmente.
// ============================================================================
class Program
{
    static async Task Main(string[] args)
    {
        PrintHeader("LECCIÓN 05: ASYNC/AWAIT EN .NET 8 + C# 12");

        await Demo01_TaskYTaskT();
        await Demo02_FromResultYCompletedTask();
        await Demo03_TaskRunYTaskDelay();
        await Demo04_WhenAllVsWhenAny();
        await Demo05_ValueTask();
        await Demo06_IAsyncEnumerable();
        await Demo07_ConfigureAwait();
        await Demo08_CancellationToken();
        await Demo09_TaskCompletionSource();
        await Demo10_ParallelForEachAsync();
        await Demo11_ExceptionHandling();
        await Demo12_ComparacionJsPython();

        PrintHeader("DEMOSTRACIÓN COMPLETA");
    }

    // ========================================================================
    // Demo 01: Task y Task<T> — la base de la programación asíncrona
    // ========================================================================
    static async Task Demo01_TaskYTaskT()
    {
        PrintSection("1. Task y Task<T> — Fundamentos");

        // Task: representa una operación asíncrona que NO devuelve valor
        // Task<T>: representa una operación asíncrona que SÍ devuelve valor de tipo T
        Console.WriteLine("  [INFO] Task = operación sin resultado. Task<T> = operación con resultado T.\n");

        // await desenvuelve el Task<int> y obtiene el int directamente
        Console.WriteLine("  Llamando a FetchDataAsync(21)...");
        int result = await AsyncDemo.FetchDataAsync(21);
        Console.WriteLine($"  Resultado: {result} (esperado 42)\n");

        // Sin await, tenemos un Task<int> (la "promesa"), no el valor
        Task<int> pendingTask = AsyncDemo.FetchDataAsync(100);
        Console.WriteLine($"  Estado antes del await: {pendingTask.Status}");
        int resolved = await pendingTask;
        Console.WriteLine($"  Estado después del await: {pendingTask.Status}, valor={resolved}\n");

        // El compilador de C# transforma async/await en una máquina de estados.
        // Cada 'await' es un punto de "suspensión".
        // El código después del await se convierte en una continuación (callback).
        // Esto evita bloquear el hilo mientras se espera I/O.
        Console.WriteLine("  [TEORÍA] El compilador genera una struct <FetchDataAsync>d__0");
        Console.WriteLine("           que implementa IAsyncStateMachine. Cada await = un estado.\n");
    }

    // ========================================================================
    // Demo 02: Task.FromResult y Task.CompletedTask
    // ========================================================================
    static async Task Demo02_FromResultYCompletedTask()
    {
        PrintSection("2. Task.FromResult y Task.CompletedTask");

        // FromResult: devuelve un Task<T> ya resuelto con un valor.
        // No lanza hilo nuevo, no aloca máquina de estados.
        Console.WriteLine("  [Task.FromResult] Para cuando ya tenemos el valor síncrono:");
        Task<int> cached = AsyncDemo.GetCachedValueAsync(50);
        Console.WriteLine($"  Estado: {cached.Status}, Resultado: {await cached}");

        // CompletedTask: singleton global, siempre el mismo objeto.
        // Evita alocar un nuevo Task cada vez.
        Console.WriteLine("\n  [Task.CompletedTask] Para métodos async sin valor de retorno:");
        Task logTask = AsyncDemo.LogAsync("Operación completada");
        Console.WriteLine($"  Estado: {logTask.Status}, Es completed: {logTask.IsCompleted}");
        Console.WriteLine($"  ¿Es el singleton? {ReferenceEquals(logTask, Task.CompletedTask)}\n");
    }

    // ========================================================================
    // Demo 03: Task.Run y Task.Delay
    // ========================================================================
    static async Task Demo03_TaskRunYTaskDelay()
    {
        PrintSection("3. Task.Run y Task.Delay");

        // Task.Run: encola trabajo en el ThreadPool.
        // Útil para trabajo CPU-bound (cálculos pesados).
        // NO usar para I/O (HttpClient, DB, archivos): ya son async nativos.
        Console.WriteLine("  [Task.Run] Ejecuta código CPU-bound en el ThreadPool:");
        Task<int> cpuTask = Task.Run(() =>
        {
            // Esto corre en un hilo del pool, NO en el hilo principal
            int sum = 0;
            for (int i = 0; i < 1000; i++) sum += i;
            return sum;
        });
        int sumResult = await cpuTask;
        Console.WriteLine($"  Suma calculada en ThreadPool: {sumResult}");

        // Task.Delay: espera asíncrona SIN bloquear el hilo.
        // Equivalente a setTimeout() en JS, asyncio.sleep() en Python.
        // NO usar Thread.Sleep() en código async: bloquea el hilo.
        Console.WriteLine("\n  [Task.Delay] Espera asíncrona sin bloquear:");
        var sw = Stopwatch.StartNew();
        await Task.Delay(150); // 150ms sin ocupar un hilo
        sw.Stop();
        Console.WriteLine($"  Delay completado en {sw.ElapsedMilliseconds}ms\n");
    }

    // ========================================================================
    // Demo 04: Task.WhenAll vs Task.WhenAny
    // ========================================================================
    static async Task Demo04_WhenAllVsWhenAny()
    {
        PrintSection("4. Task.WhenAll vs Task.WhenAny");

        // WhenAll: espera TODAS las tareas. Los resultados mantienen orden.
        Console.WriteLine("  [WhenAll] Espera a que todas terminen:");
        var sw = Stopwatch.StartNew();
        int[] allResults = await AsyncDemo.FetchAllAsync(new[] { 10, 20, 30 });
        sw.Stop();
        Console.WriteLine($"  Resultados: [{string.Join(", ", allResults)}] en {sw.ElapsedMilliseconds}ms");
        Console.WriteLine($"  Las 3 tareas corrieron en paralelo (~10ms total, no 30ms)");

        // WhenAny: espera la PRIMERA en completar.
        // Las demás tareas siguen ejecutándose — hay que cancelarlas si no interesan.
        Console.WriteLine("\n  [WhenAny] Espera a la primera en completar:");
        int fastest = await AsyncDemo.FetchFirstAsync(500, 50);
        Console.WriteLine($"  Ganó la tarea que devolvió: {fastest} (esperado 2, la más rápida)");

        // WhenAny + timeout: patrón común
        Console.WriteLine("\n  [Patrón] WhenAny + timeout:");
        var workTask = Task.Delay(2000);           // Operación larga
        var timeoutTask = Task.Delay(300);         // Timeout corto
        var completedFirst = await Task.WhenAny(workTask, timeoutTask);
        if (completedFirst == timeoutTask)
            Console.WriteLine("  TIMEOUT: la operación tardó demasiado");
        else
            Console.WriteLine("  La operación completó a tiempo");
        await Task.Delay(10); // Pequeña pausa entre demos
    }

    // ========================================================================
    // Demo 05: ValueTask<T> — evitar alocaciones
    // ========================================================================
    static async Task Demo05_ValueTask()
    {
        PrintSection("5. ValueTask<T> — Evitar alocaciones en el heap");

        // ValueTask<T> es un struct (discriminated union):
        //   - Caso 0: resultado síncrono (sin alocar Task en heap)
        //   - Caso 1: envuelve un Task<T> (cuando es realmente asíncrono)
        //   - Caso 2: envuelve IValueTaskSource (escenarios avanzados)
        Console.WriteLine("  [ValueTask] Camino rápido síncrono:");

        // Iteramos muchas veces para ver la diferencia de alocación
        for (int i = 0; i < 3; i++)
        {
            ValueTask<int> vt = AsyncDemo.FetchValueTaskAsync(-1); // Ramas síncrona
            Console.WriteLine($"    Iteración {i}: resultado={await vt}");
        }

        Console.WriteLine("  El camino síncrono NO aloca un Task en el heap.");
        Console.WriteLine("  El camino asíncrono (value > 0) SÍ aloca internamente.");

        Console.WriteLine("\n  ⚠️ Reglas importantes de ValueTask:");
        Console.WriteLine("    - No hacer await del mismo ValueTask más de una vez.");
        Console.WriteLine("    - No almacenar ValueTask en colecciones.");
        Console.WriteLine("    - Usar solo cuando el camino caliente sea síncrono.");
        Console.WriteLine("    - Preferir Task<T> como tipo de retorno público por defecto.\n");
    }

    // ========================================================================
    // Demo 06: IAsyncEnumerable<T> — streaming asíncrono
    // ========================================================================
    static async Task Demo06_IAsyncEnumerable()
    {
        PrintSection("6. IAsyncEnumerable<T> — Streaming asíncrono con await foreach");

        // IAsyncEnumerable<T> permite consumir una secuencia asíncrona
        // elemento por elemento, sin esperar a que todos estén listos.
        // Ideal para: paginación de BD, lectura de archivos grandes, SSE, etc.
        Console.WriteLine("  Recibiendo datos en streaming...");

        await foreach (int item in AsyncDemo.StreamDataAsync(5))
        {
            Console.WriteLine($"    Recibido: {item}");
        }

        Console.WriteLine("\n  Ventajas sobre Task<IEnumerable<T>>:");
        Console.WriteLine("    - El consumidor controla el ritmo (pull model).");
        Console.WriteLine("    - No necesitas cargar todo en memoria.");
        Console.WriteLine("    - Cada yield es un punto de await, liberando el hilo.\n");
    }

    // ========================================================================
    // Demo 07: ConfigureAwait(false) — para librerías
    // ========================================================================
    static async Task Demo07_ConfigureAwait()
    {
        PrintSection("7. ConfigureAwait(false) — Por qué importa en librerías");

        // Por defecto, await captura el SynchronizationContext actual
        // y reanuda la ejecución en el mismo contexto.
        // En UI (WPF, WinForms), esto vuelve al hilo de UI.
        // En ASP.NET Core NO hay SynchronizationContext, así que no importa.
        //
        // ConfigureAwait(false) le dice al runtime:
        // "No necesito volver al contexto original, continúa en cualquier hilo."

        Console.WriteLine("  await sin ConfigureAwait:");
        Console.WriteLine("    Captura SynchronizationContext y vuelve al hilo original.");
        Console.WriteLine("    Necesario en UI, innecesario en ASP.NET Core.\n");

        // Simulación: usamos Task.Delay que internamente usa ConfigureAwait(false) o no
        // dependiendo de la implementación de la BCL.
        Console.WriteLine("  await con ConfigureAwait(false):");
        await Task.Delay(50).ConfigureAwait(false);
        Console.WriteLine("    No captura el contexto. Mejor rendimiento en librerías.\n");

        Console.WriteLine("  Regla práctica:");
        Console.WriteLine("    - Código de librería: SIEMPRE ConfigureAwait(false).");
        Console.WriteLine("    - Código de aplicación (UI): NUNCA ConfigureAwait(false).");
        Console.WriteLine("    - ASP.NET Core: no hace falta (no hay SynchronizationContext).\n");
    }

    // ========================================================================
    // Demo 08: CancellationToken — cancelación cooperativa
    // ========================================================================
    static async Task Demo08_CancellationToken()
    {
        PrintSection("8. CancellationToken — Cancelación cooperativa");

        // CancellationTokenSource crea un token cancelable.
        // El token se pasa por toda la cadena de métodos async.
        // La cancelación es COOPERATIVA: cada método decide cuándo parar.
        // Métodos bien diseñados lanzan OperationCanceledException al cancelar.

        // Caso 1: Cancelación manual después de un tiempo
        Console.WriteLine("  [Caso 1] Cancelación manual con CancelAfter:");
        using var cts1 = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        try
        {
            await AsyncDemo.CancellableOperationAsync(2000, cts1.Token);
            Console.WriteLine("  Completado (no debería llegar aquí)");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("  Operación cancelada correctamente (TaskCanceledException)");
        }

        // Caso 2: Timeout mediante WhenAny + token
        Console.WriteLine("\n  [Caso 2] Timeout con WhenAny:");
        using var cts2 = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
        var operation = AsyncDemo.CancellableOperationAsync(2000, cts2.Token);
        var delay = Task.Delay(100);

        var completed = await Task.WhenAny(operation, delay);
        if (completed == delay)
        {
            cts2.Cancel(); // Cancelamos manualmente
            Console.WriteLine("  Timeout detectado, operación cancelada.");
        }

        // Caso 3: CancellationToken vinculado (linked token)
        Console.WriteLine("\n  [Caso 3] Token combinado (LinkedTokenSource):");
        using var userCts = new CancellationTokenSource();
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(5000));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(userCts.Token, timeoutCts.Token);

        // Si cualquiera de los dos cancela, el token vinculado también cancela
        Console.WriteLine("  Token vinculado activo. Cancela si el usuario o el timeout disparan.\n");
    }

    // ========================================================================
    // Demo 09: TaskCompletionSource<T> — puente entre modelos
    // ========================================================================
    static async Task Demo09_TaskCompletionSource()
    {
        PrintSection("9. TaskCompletionSource<T> — Envolver callbacks en Tasks");

        // TaskCompletionSource permite crear un Task manualmente y
        // completarlo desde fuera. Es el "puente" entre:
        //   - Event-based Asynchronous Pattern (EAP)
        //   - Asynchronous Programming Model (APM, Begin/End)
        //   - Task-based Asynchronous Pattern (TAP)

        Console.WriteLine("  Envolviendo un callback de estilo antiguo en un Task:");
        Task<int> wrappedTask = AsyncDemo.WrapCallbackAsync(25);
        Console.WriteLine($"  Estado inmediato: {wrappedTask.Status}");

        int wrappedResult = await wrappedTask;
        Console.WriteLine($"  Resultado recibido: {wrappedResult} (esperado 50)");

        // Caso de uso típico: convertir WebClient.DownloadStringCompleted en Task
        Console.WriteLine("\n  Usos típicos de TaskCompletionSource:");
        Console.WriteLine("    - Wrapping de eventos (EAP).");
        Console.WriteLine("    - Convertir Begin/End (APM) en Task.");
        Console.WriteLine("    - Sincronización entre hilos.");
        Console.WriteLine("    - Test doubles de operaciones async.\n");
    }

    // ========================================================================
    // Demo 10: Parallel.ForEachAsync (.NET 6+)
    // ========================================================================
    static async Task Demo10_ParallelForEachAsync()
    {
        PrintSection("10. Parallel.ForEachAsync — Paralelismo con async (.NET 6+)");

        // Parallel.ForEachAsync ejecuta un delegado async para cada elemento,
        // controlando el grado de paralelismo con MaxDegreeOfParallelism.
        // A diferencia de Task.WhenAll, no dispara todas las tareas a la vez.
        var items = Enumerable.Range(1, 6);
        var sw = Stopwatch.StartNew();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 3 // Máximo 3 tareas concurrentes
        };

        Console.WriteLine($"  Procesando {items.Count()} elementos con máximo 3 concurrentes:");
        await Parallel.ForEachAsync(items, options, async (item, ct) =>
        {
            Console.WriteLine($"    [{sw.ElapsedMilliseconds,4}ms] Iniciando item {item} (hilo {Environment.CurrentManagedThreadId})");
            await Task.Delay(100, ct); // Simula I/O
            Console.WriteLine($"    [{sw.ElapsedMilliseconds,4}ms] Completado item {item}");
        });

        sw.Stop();
        Console.WriteLine($"  Total: {sw.ElapsedMilliseconds}ms (~200ms porque 6/3=2 tandas)\n");

        Console.WriteLine("  Diferencias con Task.WhenAll:");
        Console.WriteLine("    - WhenAll: dispara TODO de golpe (puede saturar).");
        Console.WriteLine("    - ForEachAsync: controla concurrencia con semáforo interno.");
        Console.WriteLine("    - ForEachAsync acepta CancellationToken.");
        Console.WriteLine("    - ForEachAsync para automáticamente en el primer error.\n");
    }

    // ========================================================================
    // Demo 11: Manejo de excepciones en código async
    // ========================================================================
    static async Task Demo11_ExceptionHandling()
    {
        PrintSection("11. Manejo de excepciones en código async");

        // Caso 1: try-catch con await funciona exactamente como esperas
        Console.WriteLine("  [Caso 1] try-catch con await:");
        try
        {
            await AsyncDemo.ThrowAsync();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"    Capturada: {ex.Message}");
        }

        // Caso 2: Sin await, la excepción queda almacenada en el Task
        Console.WriteLine("\n  [Caso 2] Excepción almacenada en el Task (sin await):");
        Task<int> faultedTask = AsyncDemo.ThrowAsync();
        // La excepción NO se lanza aquí. Queda dentro del Task.
        Console.WriteLine($"    Estado del Task: {faultedTask.Status}");
        Console.WriteLine($"    IsFaulted: {faultedTask.IsFaulted}");

        // Para acceder a la excepción sin await:
        if (faultedTask.IsFaulted)
        {
            // .Exception es un AggregateException que envuelve la excepción real
            AggregateException? aggEx = faultedTask.Exception;
            Console.WriteLine($"    Excepción interna: {aggEx?.InnerException?.GetType().Name}");
        }

        // Caso 3: AggregateException con WhenAll
        Console.WriteLine("\n  [Caso 3] AggregateException al esperar múltiples tareas:");
        try
        {
            await AsyncDemo.ThrowMultipleAsync();
        }
        catch (Exception ex)
        {
            // WhenAll solo lanza UNA de las excepciones (la primera).
            // Para ver TODAS, hay que inspeccionar Task.Exception.InnerExceptions.
            Console.WriteLine($"    Primera excepción: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine("    Usa Task.Exception.InnerExceptions para ver todas.");
        }

        // Caso 4: await propaga la excepción "desenvuelta", no AggregateException
        Console.WriteLine("\n  [Caso 4] await desenvuelve AggregateException:");
        try
        {
            await faultedTask; // Re-lanza la excepción original, no AggregateException
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"    await desenvuelve a: {ex.GetType().Name}");
        }

        Console.WriteLine("\n  Reglas de excepciones en async:");
        Console.WriteLine("    - await desenvuelve AggregateException → lanza la primera excepción.");
        Console.WriteLine("    - .Result / .Wait() → lanza AggregateException (envuelta).");
        Console.WriteLine("    - Task.Exception → AggregateException con todas las InnerExceptions.");
        Console.WriteLine("    - OperationCanceledException es especial: hereda de TaskCanceledException.\n");
    }

    // ========================================================================
    // Demo 12: Comparación C# vs JavaScript vs Python
    // ========================================================================
    static Task Demo12_ComparacionJsPython()
    {
        PrintSection("12. Comparación: C# async/await vs JavaScript Promises vs Python asyncio");

        Console.WriteLine("  ┌─────────────────────┬──────────────────────┬──────────────────────┬──────────────────────┐");
        Console.WriteLine("  │ Concepto            │ C# (.NET)            │ JavaScript           │ Python               │");
        Console.WriteLine("  ├─────────────────────┼──────────────────────┼──────────────────────┼──────────────────────┤");
        Console.WriteLine("  │ Unidad base         │ Task / Task<T>       │ Promise              │ Coroutine / Future   │");
        Console.WriteLine("  │ Valor resuelto      │ await task           │ await promise        │ await coroutine      │");
        Console.WriteLine("  │ Paralelo            │ Task.WhenAll         │ Promise.all          │ asyncio.gather       │");
        Console.WriteLine("  │ Primero en terminar │ Task.WhenAny         │ Promise.race         │ asyncio.wait(FIRST)  │");
        Console.WriteLine("  │ Delay               │ Task.Delay           │ setTimeout(promise)  │ asyncio.sleep        │");
        Console.WriteLine("  │ CPU-bound (hilo)    │ Task.Run             │ Worker threads       │ loop.run_in_executor │");
        Console.WriteLine("  │ Cancelación         │ CancellationToken    │ AbortController      │ Task.cancel / signal │");
        Console.WriteLine("  │ Streaming async     │ IAsyncEnumerable<T>  │ AsyncIterator        │ async for (AsyncIter)│");
        Console.WriteLine("  │ FromResult          │ Task.FromResult      │ Promise.resolve      │ asyncio.Future.set   │");
        Console.WriteLine("  │ Event loop          │ SyncContext / ThreadPool│ Event Loop (1 hilo)│ Event Loop (1 hilo)  │");
        Console.WriteLine("  └─────────────────────┴──────────────────────┴──────────────────────┴──────────────────────┘");

        Console.WriteLine("\n  Diferencias clave:");
        Console.WriteLine("    C#:  Hilos reales + ThreadPool. Tasks pueden correr en paralelo real.");
        Console.WriteLine("    JS:  Single-threaded. Promises son microtasks. Sin paralelismo real (sin workers).");
        Console.WriteLine("    Py:  Single-threaded con GIL. asyncio usa corrutinas cooperativas.");
        Console.WriteLine("    C#:  await captura SynchronizationContext. JS/Py no tienen este concepto.");
        Console.WriteLine("    C#:  Task<T> es tipado en compilación. Promise<any> puede ser dinámico.\n");

        return Task.CompletedTask;
    }

    // ========================================================================
    // Helpers de impresión
    // ========================================================================
    static void PrintHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n{'═',80}");
        Console.WriteLine($"  {title}");
        Console.WriteLine($"  .NET 8 + C# 12  |  Curso Completo de .NET");
        Console.WriteLine($"{'═',80}\n");
        Console.ResetColor();
    }

    static void PrintSection(string title)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"── {title}");
        Console.ResetColor();
    }
}
