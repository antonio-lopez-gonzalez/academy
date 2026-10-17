# Lección 06 — Extension Methods, IDisposable y IAsyncDisposable

## 1. Introducción

Dos conceptos fundamentales que aparecen en TODO el código .NET profesional:

- **Extension Methods**: añadir métodos a tipos existentes sin modificar su código fuente. LINQ entero está construido con ellos.
- **IDisposable / IAsyncDisposable**: liberar recursos externos (archivos, sockets, conexiones a BD) de forma **determinista**, sin depender del Garbage Collector.

Ambos son "azúcar sintáctico" del compilador: no requieren cambios en el CLR ni en el runtime.

---

## 2. Teoría

### 2.1 Extension Methods

Un método de extensión es un **método estático** en una **clase estática** cuyo primer parámetro lleva `this`:

```csharp
public static class StringExtensions
{
    public static int WordCount(this string source) { ... }
}
```

El compilador traduce `frase.WordCount()` → `StringExtensions.WordCount(frase)`. No hay magia en runtime: es una llamada estática normal. El `this` es solo una marca para el compilador.

**Lo que NO pueden hacer**: acceder a miembros privados/protegidos del tipo extendido (no hay "friend" como en C++). Solo ven lo público.

**Comparación con otros lenguajes:**

| Lenguaje | Mecanismo | ¿Modifica el tipo? |
|---|---|---|
| C# | static class + `this` | No |
| Kotlin | `fun Tipo.metodo()` | No |
| JavaScript | `Prototype.fn = ...` | **Sí** (mutable) |
| Swift | `extension Tipo { }` | No |

### 2.2 IDisposable

El GC libera **memoria**. No libera handles de archivo, sockets, conexiones a BD, mutex del SO, etc.

`IDisposable` proporciona un contrato con un método `Dispose()` que tú llamas (o el `using`) para liberar esos recursos **ya**, no cuando el GC se despierte.

**Flujo sin Dispose:**
```
new FileStream() → abrir handle → usar → olvidar
                                        ↓
                              GC eventualmente pasa
                              el finalizador (~destructor)
                              cierra el handle (si lo implementa)
                              → lag impredecible
```

**Flujo con Dispose:**
```
using var fs = new FileStream(...) → abrir handle → usar
                                    ↓
                              Dispose() cierra handle ya
                              → determinista
```

### 2.3 IAsyncDisposable

Cuando `Dispose()` necesita hacer I/O (flush de buffers, enviar comandos de cierre por red), no puedes bloquear el hilo. `IAsyncDisposable` devuelve `ValueTask` para permitir `await`:

```csharp
await using var db = new MyDbContext();
// ...
// DisposeAsync() se llama aquí de forma asíncrona
```

**ValueTask vs Task**: `ValueTask` es un struct que evita heap allocation cuando el resultado es inmediato (la mayoría de los DisposeAsync lo son).

---

## 3. Ejemplo mínimo

### Extension method

```csharp
public static class StringExtensions
{
    public static int WordCount(this string source)
        => source.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
}

// Uso:
int n = "hola mundo".WordCount(); // 2
```

### IDisposable

```csharp
public class MiRecurso : IDisposable
{
    public void Dispose() { /* liberar */ }
}

// Uso con using statement:
using (var r = new MiRecurso()) { /* usar */ } // Dispose aquí

// Uso con using declaration (C# 8+):
using var r2 = new MiRecurso(); // Dispose al final del scope
```

### IAsyncDisposable

```csharp
public class MiRecursoAsync : IAsyncDisposable
{
    public async ValueTask DisposeAsync() { await ...; }
}

await using var r = new MiRecursoAsync();
```

---

## 4. Ejemplo real

El proyecto `src/Demo/Program.cs` contiene ejemplos ejecutables completos:

- **WordCount / ToTitleCase**: extensiones para string.
- **Median**: extensión sobre `IEnumerable<int>` y `IEnumerable<double>`.
- **FileSimulator**: clase `IDisposable` que simula un archivo.
- **DisposeTracker**: implementación del patrón `Dispose(bool)` completo con contador.
- **AsyncDatabaseConnection**: `IAsyncDisposable` que simula una conexión a BD asíncrona.

Ejecutar con:

```bash
dotnet run --project src/Demo
```

---

## 5. Buenas prácticas

### Extension Methods

- **Haz**: extender interfaces (`IEnumerable<T>`, `ILogger<T>`) para que la extensión funcione en todas las implementaciones.
- **Haz**: nombres descriptivos que indiquen qué hace el método.
- **Haz**: usar extensiones para adaptar APIs de terceros a tu modelo (wrappers fluidos).
- **Evita**: extender `object` o `string` sin un motivo muy específico (contaminas IntelliSense).
- **Evita**: extensiones con estado (no hay dónde guardar estado, son métodos estáticos).
- **Evita**: ocultar métodos existentes con una extensión del mismo nombre (el compilador prefiere el método de instancia).

### IDisposable

- **Haz**: SIEMPRE `using` (o `await using`) con `Stream`, `DbConnection`, `HttpClient`, `Timer`, `CancellationTokenSource`.
- **Haz**: `Dispose()` idempotente (llamarlo 2 veces no debe lanzar excepción).
- **Haz**: `GC.SuppressFinalize(this)` en `Dispose()` si implementas el patrón completo.
- **Evita**: `Dispose()` que lanza excepciones (el GC se traga las excepciones en finalizadores).
- **Evita**: dependencia del finalizador (`~MiClase`) si no hay recursos nativos.

---

## 6. Errores habituales

### Error 1: No hacer Dispose de HttpClient

```csharp
// ❌ MAL: cada instancia = nuevo socket que tarda en liberarse
for (int i = 0; i < 1000; i++)
{
    var client = new HttpClient();
    var result = await client.GetStringAsync(url);
}

// ✅ BIEN: usar IHttpClientFactory (se ve en módulos posteriores)
```

### Error 2: Extender object

```csharp
// ❌ MAL: TODO en tu código tendrá .Dump()
public static void Dump(this object o) => Console.WriteLine(o);
42.Dump(); "hola".Dump(); new Exception().Dump(); // ruido total
```

### Error 3: Dispose sincrono que bloquea

```csharp
// ❌ MAL: Dispose no debe bloquear el hilo
public void Dispose()
{
    _stream.Flush();        // bien
    _dbConnection.Close();  // puede bloquear
    Task.Delay(1000).Wait(); // ¡NUNCA! deadlock potencial
}

// ✅ BIEN: implementar IAsyncDisposable
public async ValueTask DisposeAsync()
{
    await _dbConnection.CloseAsync();
    await _stream.FlushAsync();
}
```

### Error 4: Usar objeto después de Dispose

```csharp
var fs = new FileSimulator("test.log");
fs.Dispose();
fs.Write("dato"); // ObjectDisposedException
```

---

## 7. Ejercicio guiado

Crea una extensión `IsPalindrome(this string source)` que devuelva `true` si la cadena se lee igual de izquierda a derecha ignorando mayúsculas y espacios.

```csharp
public static class StringChecker
{
    public static bool IsPalindrome(this string source)
    {
        var limpio = new string(source
            .Where(c => !char.IsWhiteSpace(c))
            .Select(char.ToLower)
            .ToArray());
        return limpio.SequenceEqual(limpio.Reverse());
    }
}
```

Pruébalo:

```csharp
"Anita lava la tina".IsPalindrome() // true
"A man a plan a canal Panama".IsPalindrome() // true
"hola mundo".IsPalindrome() // false
```

---

## 8. Reto

Implementa una extensión `ChunkBy<T>(this IEnumerable<T> source, int chunkSize)` que divida una secuencia en grupos de tamaño fijo **sin usar LINQ** (ni `Chunk()` de .NET 6+).

Después, implementa una clase `TimedFileSimulator : IDisposable` que:
1. Registre la hora de apertura y cierre.
2. Lance `ObjectDisposedException` si se escribe después de cerrar.
3. Exponga `TimeSpan Elapsed` con el tiempo que estuvo abierto.

---

## 9. Resumen

| Concepto | Propósito | Sintaxis clave |
|---|---|---|
| Extension methods | Añadir métodos a tipos existentes | `this Tipo p` en static class |
| `IDisposable` | Liberar recursos determinísticamente | `using` / `using var` |
| `IAsyncDisposable` | Liberar recursos de forma asíncrona | `await using` |
| Patrón `Dispose(bool)` | Separar limpieza manual vs GC | `protected virtual void Dispose(bool)` |

**Regla de oro**: si un tipo implementa `IDisposable` o `IAsyncDisposable`, SIEMPRE usa `using` o `await using`. No hay excepciones.

---

## 10. Preguntas frecuentes

**¿Cuándo implemento IDisposable vs IAsyncDisposable?**
Si tu `Dispose()` solo libera objetos gestionados (llamar a `.Dispose()` de otros), basta con `IDisposable`. Si necesitas hacer `await` (flush a BD, cerrar conexión de red), implementas `IAsyncDisposable`.

**¿Puedo tener Dispose síncrono y asíncrono en la misma clase?**
Sí. Es común tener ambos. El `using` llama a `Dispose()` y el `await using` llama a `DisposeAsync()`. Normalmente `Dispose()` delega en `DisposeAsync().GetAwaiter().GetResult()` o viceversa.

**¿Los extension methods afectan al rendimiento?**
No. El compilador los convierte en llamadas estáticas normales. El IL generado es idéntico a llamar a `ClaseEstatica.Metodo(obj)`.

**¿Por qué `ValueTask` en `IAsyncDisposable` y no `Task`?**
`ValueTask` es un struct. En la mayoría de casos, `DisposeAsync()` no necesita hacer nada asíncrono realmente (solo libera memoria), y `ValueTask` evita la heap allocation que `Task` siempre requiere. Es una micro-optimización importante en APIs de alto rendimiento.

**¿Qué pasa si olvido `using` en un `SqlConnection`?**
El pool de conexiones tiene un límite (por defecto 100). Si abres conexiones sin cerrarlas, llegarás al límite y las siguientes peticiones lanzarán `InvalidOperationException` con "timeout waiting for connection from pool". En producción es catastrófico.
