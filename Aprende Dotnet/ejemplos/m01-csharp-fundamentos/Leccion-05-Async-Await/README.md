# Lección 05 — Async/Await en .NET 8

## 1. Introducción

**¿Qué vamos a aprender?** El modelo de programación asíncrona de .NET basado en `Task`, `async` y `await`. Este modelo permite escribir código que no bloquea el hilo mientras espera operaciones de I/O (red, disco, base de datos), manteniendo la aplicación responsiva y escalable.

**¿Por qué existe?** En aplicaciones web, la mayoría del tiempo se pasa esperando respuestas externas. Si bloqueamos el hilo en cada espera, necesitaríamos cientos de hilos para atender cientos de peticiones. El modelo async permite que un mismo hilo atienda múltiples peticiones mientras las operaciones de I/O están pendientes.

**¿Cuándo se utiliza?** Siempre que haya operaciones de I/O: llamadas HTTP, consultas a base de datos, lectura/escritura de archivos. También para trabajo CPU-bound paralelizable mediante `Task.Run` o `Parallel.ForEachAsync`.

**Errores típicos:** usar `.Result` o `.Wait()` (deadlocks), no propagar `CancellationToken`, async void en lugar de async Task, no hacer `ConfigureAwait(false)` en librerías.

---

## 2. Teoría

### El modelo Task-based Asynchronous Pattern (TAP)

.NET 4.0 introdujo `Task` y `Task<T>` como unidad de trabajo asíncrono. C# 5.0 añadió `async`/`await` para escribir código asíncrono que se lee como síncrono.

```
Sin async:                              Con async:
───────────                             ───────────
var task = DoWorkAsync();               var result = await DoWorkAsync();
var result = task.Result; // BLOQUEA    // No bloquea, devuelve el control
```

### Máquina de estados del compilador

Cuando el compilador encuentra `async`, transforma el método en una **struct** que implementa `IAsyncStateMachine`. Cada `await` es un punto de suspensión (un estado numerado). El código después del `await` se convierte en una continuación que se ejecuta cuando la tarea esperada termina.

```
async Task<int> SumAsync()
{
    int a = await GetAAsync();  // Estado 0 → suspende, continúa en estado 1
    int b = await GetBAsync();  // Estado 1 → suspende, continúa en estado 2
    return a + b;               // Estado 2 → completo
}
```

### SynchronizationContext

Por defecto, `await` captura el `SynchronizationContext` actual y reanuda en él. En aplicaciones UI (WPF, WinForms) esto significa volver al hilo de UI. En ASP.NET Core no existe este contexto, por lo que `ConfigureAwait(false)` no tiene efecto (pero se recomienda en librerías por compatibilidad).

### ValueTask<T>

`ValueTask<T>` es un `struct` que puede contener el resultado directamente (sin alocar `Task` en el heap) o envolver un `Task<T>`. Se introdujo para escenarios de alta frecuencia donde la mayoría de las llamadas son síncronas (ej. cachés). **Reglas**: no hacer await dos veces, no almacenar en colecciones, no usar como tipo de retorno público por defecto.

### IAsyncEnumerable<T>

C# 8 + .NET Core 3.0 introdujeron streams asíncronos. En lugar de devolver todos los datos de golpe (`Task<IEnumerable<T>>`), el productor emite elementos bajo demanda con `yield return` y el consumidor los recibe con `await foreach`.

### Comparación con otros lenguajes

| Concepto | C# | JavaScript | Python |
|---|---|---|---|
| Unidad base | `Task` / `Task<T>` | `Promise` | Coroutine / Future |
| Palabra clave | `await` | `await` | `await` |
| Paralelo | `Task.WhenAll` | `Promise.all` | `asyncio.gather` |
| Carrera | `Task.WhenAny` | `Promise.race` | `asyncio.wait(FIRST)` |
| Cancelación | `CancellationToken` | `AbortController` | `Task.cancel` |
| Streaming | `IAsyncEnumerable<T>` | AsyncIterator | `async for` |
| Hilos reales | Sí (ThreadPool) | No (workers aparte) | No (GIL) |

---

## 3. Ejemplo mínimo

```csharp
// Método async que devuelve Task<int>
static async Task<int> FetchDataAsync()
{
    // Simula espera de I/O sin bloquear el hilo
    await Task.Delay(100);
    return 42;
}

// Punto de entrada async (C# 7.1+)
static async Task Main()
{
    Console.WriteLine("Iniciando...");
    int result = await FetchDataAsync(); // await desenvuelve Task<int> → int
    Console.WriteLine($"Resultado: {result}");
    Console.WriteLine("Completado.");
}
```

**Línea por línea:**
- `async Task<int>`: el método es asíncrono y devuelve una promesa de `int`.
- `await Task.Delay(100)`: cede el control al caller. El hilo queda libre durante 100ms.
- `await FetchDataAsync()`: espera el resultado sin bloquear.
- `Task Main()`: desde C# 7.1, `Main` puede ser async.

Ejecutar: `dotnet run --project src/Demo`

---

## 4. Ejemplo real

El programa `Program.cs` de esta lección ejecuta 12 demos reales:

1. **Task / Task<T>** — Creación, estados, await como desempaquetado.
2. **FromResult / CompletedTask** — Valores síncronos en métodos async sin overhead.
3. **Task.Run / Task.Delay** — CPU-bound en ThreadPool, espera asíncrona sin bloqueo.
4. **WhenAll / WhenAny** — Paralelismo multiple, patrón de timeout.
5. **ValueTask<T>** — Evitar alocaciones en heap para caminos calientes síncronos.
6. **IAsyncEnumerable<T>** — Streaming asíncrono con `await foreach`.
7. **ConfigureAwait(false)** — Cuándo y por qué en librerías.
8. **CancellationToken** — Cancelación cooperativa con timeout, token vinculado.
9. **TaskCompletionSource<T>** — Puente entre APIs de callback y TAP.
10. **Parallel.ForEachAsync** — Paralelismo controlado con async (.NET 6+).
11. **Excepciones** — AggregateException, try-catch con await, Task.Exception.
12. **Comparación JS/Python** — Tabla comparativa de modelos asíncronos.

Ejecutar: `dotnet run --project src/Demo`

---

## 5. Buenas prácticas

### Hacer
- **Sufijo `Async`** en métodos que devuelven `Task`/`Task<T>`/`ValueTask<T>` (`GetUserAsync`).
- **Propagar `CancellationToken`** hasta el nivel más bajo (BD, HTTP, archivos).
- **`ConfigureAwait(false)`** en todo código de librería.
- **`Task.Run`** solo para CPU-bound, nunca para I/O.
- **`IAsyncEnumerable<T>`** para paginación y streams de datos grandes.
- **`ValueTask<T>`** solo cuando el camino caliente es síncrono y el rendimiento importa.
- **`using var cts = new CancellationTokenSource()`** para liberar recursos del token.

### Evitar
- **`.Result` / `.Wait()`** — Pueden causar deadlocks al bloquear el SynchronizationContext.
- **`async void`** — Solo para event handlers. En cualquier otro caso, usar `async Task`.
- **`new HttpClient()`** — Usar `IHttpClientFactory` (gestiona sockets y pooling).
- **`Thread.Sleep()`** en código async — Bloquea el hilo; usar `Task.Delay()`.
- **await múltiple al mismo `ValueTask`** — Comportamiento indefinido.

---

## 6. Errores habituales

### Error 1: .Result en contexto UI → deadlock

```csharp
// ❌ MAL: deadlock en WPF/WinForms
var result = GetDataAsync().Result;

// ✅ BIEN: hacer await hasta el final
var result = await GetDataAsync();
```

### Error 2: async void

```csharp
// ❌ MAL: las excepciones no se capturan, no se puede esperar
async void HandleClick() { await SaveAsync(); }

// ✅ BIEN: async Task permite capturar excepciones y esperar
async Task HandleClickAsync() { await SaveAsync(); }
```

### Error 3: No pasar CancellationToken

```csharp
// ❌ MAL: la operación no se puede cancelar
public async Task<User> GetUserAsync(int id) { ... }

// ✅ BIEN: el token se propaga a la BD/HTTP
public async Task<User> GetUserAsync(int id, CancellationToken ct = default) { ... }
```

### Error 4: await foreach sin CancellationToken en streaming

```csharp
// ❌ MAL: iteración sin forma de cancelar
await foreach (var item in GetStreamAsync()) { ... }

// ✅ BIEN: pasar token para cancelar si el cliente se desconecta
await foreach (var item in GetStreamAsync(ct)) { ... }
```

### Error 5: Task.Run para I/O

```csharp
// ❌ MAL: ocupa un hilo del pool solo para esperar
var data = await Task.Run(() => httpClient.GetStringAsync(url));

// ✅ BIEN: el método ya es async, no necesita Task.Run
var data = await httpClient.GetStringAsync(url);
```

---

## 7. Ejercicio guiado

**Objetivo:** Crear un método que descargue el contenido de 3 URLs en paralelo, con timeout y cancelación.

```csharp
// Paso 1: Declarar el método con CancellationToken
static async Task<string[]> DownloadAllAsync(string[] urls, int timeoutMs, CancellationToken ct)
{
    // Paso 2: Crear un CancellationTokenSource para el timeout
    using var timeoutCts = new CancellationTokenSource(timeoutMs);
    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

    // Paso 3: Crear las tareas de descarga (usando HttpClient inyectado)
    var tasks = urls.Select(url => DownloadAsync(url, linkedCts.Token));

    // Paso 4: Esperar todas con WhenAll
    return await Task.WhenAll(tasks);
}

// Simulación de descarga (en el ejercicio real se usa HttpClient)
static async Task<string> DownloadAsync(string url, CancellationToken ct)
{
    await Task.Delay(new Random().Next(50, 200), ct);
    return $"Contenido de {url}";
}
```

Ejecuta el código con:
```csharp
var urls = new[] { "url1", "url2", "url3" };
var results = await DownloadAllAsync(urls, 300, CancellationToken.None);
foreach (var r in results) Console.WriteLine(r);
```

---

## 8. Reto

Crea un **agregador de APIs meteorológicas** que consulte 3 servicios distintos en paralelo y devuelva la primera respuesta válida (ignorando las que fallen), con un timeout total de 2 segundos.

Requisitos:
- Cada servicio es un `Task<string>` simulado (unas fallan, otras tardan).
- Si todos fallan, devolver un mensaje de error.
- Si alguno responde antes del timeout, devolver ese resultado y cancelar las demás.
- Usar `CancellationToken`, `Task.WhenAny`, `TaskCompletionSource`, y `try/catch`.

No se resuelve aquí. Inténtalo primero.

---

## 9. Resumen

| Concepto | Descripción |
|---|---|
| `Task` / `Task<T>` | Unidad de trabajo asíncrono. `Task` no devuelve valor, `Task<T>` sí. |
| `async` / `await` | Transforma el método en máquina de estados. `await` cede el control sin bloquear. |
| `Task.Delay` | Espera asíncrona equivalente a `setTimeout`/`asyncio.sleep`. |
| `Task.Run` | Encola trabajo CPU-bound en el ThreadPool. |
| `Task.WhenAll` | Espera todas las tareas. Resultados en orden. |
| `Task.WhenAny` | Espera la primera tarea. Las demás siguen ejecutándose. |
| `ValueTask<T>` | Alternativa a `Task<T>` que evita alocaciones en el camino síncrono. |
| `IAsyncEnumerable<T>` | Streaming asíncrono con `await foreach`. |
| `ConfigureAwait(false)` | Evita capturar el SynchronizationContext. Obligatorio en librerías. |
| `CancellationToken` | Mecanismo de cancelación cooperativa. |
| `TaskCompletionSource<T>` | Crea un Task manualmente para envolver callbacks. |
| `Parallel.ForEachAsync` | Procesa elementos en paralelo con control de concurrencia. |
| Excepciones | `await` desenvuelve AggregateException. `.Result`/`.Wait()` lanzan AggregateException. |

---

## 10. Preguntas frecuentes

**P: ¿Cuándo usar `Task.Run`?**
R: Solo para trabajo CPU-bound intensivo. Para I/O (HTTP, BD, archivos), los métodos nativos ya son async y no necesitan `Task.Run`. Usarlo para I/O desperdicia un hilo del pool.

**P: ¿Por qué no debo usar `.Result`?**
R: `.Result` bloquea el hilo actual hasta que el Task termine. Si el SynchronizationContext está capturado (UI, ASP.NET clásico), puede causar deadlock porque el Task necesita el mismo hilo para completarse y ese hilo está bloqueado esperando `.Result`.

**P: ¿`ValueTask<T>` o `Task<T>` como tipo de retorno?**
R: Por defecto, `Task<T>`. Solo usar `ValueTask<T>` cuando el perfil de uso muestre que la mayoría de llamadas son síncronas y necesitas evitar la alocación del `Task`. Un ejemplo típico: un repositorio con caché en memoria.

**P: ¿Qué diferencia hay entre `OperationCanceledException` y `TaskCanceledException`?**
R: `TaskCanceledException` hereda de `OperationCanceledException`. `Task.Delay` y las APIs de la BCL lanzan `TaskCanceledException`. Tu código debe atrapar `OperationCanceledException` (la clase base) para cubrir ambos casos.

**P: ¿Cómo ejecuto varias tareas con un límite de concurrencia?**
R: Usa `Parallel.ForEachAsync` con `MaxDegreeOfParallelism` (.NET 6+), o implementa un `SemaphoreSlim` manual con `WaitAsync()`. Evita lanzar 1000 tareas a la vez con `Task.WhenAll` sin control.
