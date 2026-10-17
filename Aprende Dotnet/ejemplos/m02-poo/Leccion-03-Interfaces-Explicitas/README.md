# Lección 03 — Interfaces Explícitas

---

## 1. Introducción

En C#, cuando una clase implementa una interfaz, los miembros de esa interfaz suelen estar disponibles directamente en el tipo concreto. Esto es la **implementación implícita** y es lo que hacemos el 90% del tiempo:

```csharp
public class Logger : ILogger
{
    // Implementación implícita: accesible desde Logger e ILogger
    public void Log(string msg) => Console.WriteLine(msg);
}
```

Pero C# permite algo más: la **implementación explícita de interfaces**. Consiste en que los miembros de una interfaz solo sean accesibles cuando tratamos el objeto **a través de esa interfaz**, no directamente desde el tipo concreto.

¿Qué problemas resuelve esto?

| Problema | Solución con implementación explícita |
|---|---|
| **Conflicto de nombres**: dos interfaces distintas exigen el mismo método con misma firma pero semántica diferente | Implementas cada método por separado, cada uno asociado a su interfaz |
| **Ocultación de miembros del API pública**: una interfaz de framework (como `ICloneable`, `IComparable`) introduce métodos que no quieres exponer en tu tipo | Los ocultas detrás de la interfaz, limpiando el Intellisense para el consumidor habitual |
| **Implementación de interfaces "técnicas" sin contaminar tu API**: `IDisposable`, `IEnumerable.GetEnumerator`, etc. | El consumidor ve solo los métodos relevantes; la interfaz sigue funcionando cuando el framework la necesita |

En esta lección aprenderás cuándo y cómo usar interfaces explícitas, sus ventajas, sus trampas y cómo afectan a la visibilidad de los miembros en tiempo de compilación y en tiempo de ejecución.

---

## 2. Teoría

### 2.1. Sintaxis

La sintaxis de una implementación explícita tiene tres reglas inamovibles:

```csharp
class MiClase : IUnaInterfaz
{
    // SIN modificador de acceso (ni public, ni private, ni internal)
    // Prefijado con el nombre de la interfaz y un punto
    void IUnaInterfaz.Metodo()
    {
        Console.WriteLine("Implementación explícita");
    }
}
```

Fíjate en lo que **no** lleva:
- No lleva `public` ni ningún modificador de acceso. El compilador lo rechaza si lo intentas.
- Siempre va prefijado con `NombreDeLaInterfaz.` antes del nombre del método.

### 2.2. Visibilidad en tiempo de compilación

Cuando un miembro está implementado explícitamente:

- **No aparece en Intellisense** si la variable está tipada como la clase concreta.
- **Sí aparece** si la variable está tipada como la interfaz.
- El compilador **rechaza** cualquier intento de llamarlo desde el tipo concreto.

Ejemplo mental:

```text
Variable de tipo MiClase:
  .Metodo()          → ❌ Error de compilación: "MiClase no contiene una definición para Metodo"

Variable de tipo IUnaInterfaz:
  .Metodo()          → ✅ Compila y ejecuta correctamente

Para acceder desde el tipo concreto, necesitas un cast:
  ((IUnaInterfaz)miObjeto).Metodo()   → ✅
  (miObjeto as IUnaInterfaz)?.Metodo() → ✅
```

### 2.3. Qué ocurre en el IL (Intermediate Language)

El compilador de C# emite el método explícito como un método **`private`** en IL (el lenguaje intermedio que ejecuta el CLR). Sin embargo, también emite una entrada en la tabla de mapeo de interfaz de la clase, diciendo: "cuando me llamen a través de esta interfaz, redirige a este método private".

```
┌─────────────────────────────────┐
│            IL emitido           │
│                                 │
│  .class MiClase                 │
│    implements IUnaInterfaz      │
│  {                              │
│    .method private hidebysig    │  ← private en IL
│      newslot virtual final      │
│      instance void              │
│      IUnaInterfaz.Metodo() { }  │  ← pero mapeado en la tabla de interfaces
│  }                              │
└─────────────────────────────────┘
```

Esto significa que en tiempo de ejecución **no se puede acceder por reflexión como método público normal**, pero sí a través de `typeof(IUnaInterfaz).GetMethod("Metodo")`.

### 2.4. Cuándo usar implementación explícita

| Escenario | ¿Explícita? | Razón |
|---|---|---|
| Una sola interfaz, sin conflictos | ❌ Implícita | Más natural, más fácil de usar |
| Dos interfaces exigen el mismo método con firma idéntica | ✅ Explícita en al menos una | No hay otra forma de distinguirlas |
| Quieres ocultar un miembro "técnico" que no forma parte del API principal | ✅ Explícita | El consumidor no necesita ver `ICloneable.Clone()` cada vez que usa tu clase |
| Implementas `IEnumerable<T>` y también `IEnumerable` (no genérica) | ✅ Explícita en `IEnumerable.GetEnumerator()` | La versión no genérica es legado, ocúltala |
| `IDisposable` cuando tu clase ofrece un método `Close()` público | ✅ Explícita | Ejemplo clásico: `FileStream` tiene `Close()` público y `IDisposable.Dispose()` explícito que llama a `Close()` |

### 2.5. Diferencia clave: implícita vs explícita

```
┌─────────────────────────────────────────────────────────────────┐
│              IMPLÍCITA                   EXPLÍCITA               │
├─────────────────────────────────────────────────────────────────┤
│  public void Metodo() { }          void IInterfaz.Metodo() { }  │
│                                                                    │
│  Accesible desde la clase:  ✅     Accesible desde la clase:  ❌  │
│  Accesible desde interfaz: ✅     Accesible desde interfaz: ✅    │
│  Visible en Intellisense:  ✅     Visible en Intellisense:  ❌   │
│  (en tipo concreto)                (en tipo concreto)              │
└─────────────────────────────────────────────────────────────────┘
```

### 2.6. Comparación con otros lenguajes

| Lenguaje | Mecanismo equivalente |
|---|---|
| **Java** | No tiene equivalente directo. Lo más parecido es usar clases internas que implementan la interfaz y delegar. |
| **C++** | Herencia múltiple con resolución explícita de ambigüedad (`Base1::metodo`, `Base2::metodo`). Más cercano conceptualmente. |
| **TypeScript** | No existe. Se puede simular con símbolos (`Symbol`) como claves de método. |
| **Go** | Las interfaces son implícitas (estructural typing), así que no hay conflicto de nombres: simplemente el método existe y satisface todas las interfaces que lo requieran. |

C# es uno de los pocos lenguajes mainstream que ofrece este nivel de control sobre la visibilidad de los miembros de interfaz.

---

## 3. Ejemplo mínimo

Este es el ejemplo más pequeño que demuestra la funcionalidad. Cópialo en un `Program.cs` y ejecuta con `dotnet run`:

```csharp
// Dos interfaces con el mismo método
interface IA { void Do(); }
interface IB { void Do(); }

// Una clase que implementa ambas con implementación explícita
class C : IA, IB
{
    void IA.Do() => Console.WriteLine("IA.Do() ejecutado");
    void IB.Do() => Console.WriteLine("IB.Do() ejecutado");
}

// ──── Uso ────────────────────────────────────────────
C c = new();

// c.Do(); // ❌ ERROR CS1061: 'C' no contiene una definición para 'Do'

((IA)c).Do();          // ✅ Imprime: IA.Do() ejecutado
((IB)c).Do();          // ✅ Imprime: IB.Do() ejecutado

// También con pattern matching:
if (c is IA ia) ia.Do();
if (c is IB ib) ib.Do();
```

**Explicación línea por línea:**

1. `interface IA { void Do(); }` — Primera interfaz, exige un método `Do()`.
2. `interface IB { void Do(); }` — Segunda interfaz, también exige `Do()`. Misma firma, distinta semántica.
3. `class C : IA, IB` — La clase implementa ambas.
4. `void IA.Do() => ...` — Implementación explícita para `IA`. Solo accesible a través de `IA`.
5. `void IB.Do() => ...` — Implementación explícita para `IB`. Solo accesible a través de `IB`.
6. `c.Do()` — Esto **no compila**. El compilador no encuentra `Do` en `C`.
7. `((IA)c).Do()` — Cast explícito a `IA` para acceder al método.
8. `c is IA ia` — Pattern matching que declara una variable `ia` de tipo `IA`. Equivalente al cast.

---

## 4. Ejemplo real

El proyecto de esta lección se encuentra en `src/PooExplicitInterfaces/Program.cs`. A continuación se describe el código y su propósito.

### 4.1. Escenario 1: `DiagnosticService` — Mismo método, distinto significado

```csharp
interface ILogger  { void Log(string message); }
interface ITracer  { void Log(string message); }

class DiagnosticService : ILogger, ITracer
{
    void ILogger.Log(string message)
    {
        Console.WriteLine($"[LOG]   {DateTime.Now:HH:mm:ss} {message}");
    }

    void ITracer.Log(string message)
    {
        Console.WriteLine($"[TRACE] {DateTime.Now:HH:mm:ss} {message}");
    }
}
```

Aquí tanto `ILogger` como `ITracer` definen un método `Log(string)`. Pero semánticamente son distintos: uno escribe un log normal y el otro escribe una traza de diagnóstico. La implementación explícita permite que cada interfaz tenga su propio comportamiento sin que `DiagnosticService` tenga que exponer dos métodos con nombres artificiales como `LogAsLogger` y `LogAsTracer`.

### 4.2. Escenario 2: `ResourceManager` — Ocultar `IDisposable` tras un `Close()` público

```csharp
interface IMyDisposable { void Close(); }

class ResourceManager : IMyDisposable, IDisposable
{
    // API público natural
    public void Close()
    {
        Console.WriteLine("Liberando recursos del sistema...");
    }

    // IDisposable oculto, simplemente llama a Close()
    void IDisposable.Dispose()
    {
        Close();
    }
}
```

Patrón clásico de .NET: la clase expone un método `Close()` semánticamente claro y `IDisposable.Dispose()` se implementa de forma explícita, llamando a `Close()`. Esto sigue el mismo patrón que clases del framework como `FileStream`, `StreamReader` o `SqlConnection`:

```csharp
var rm = new ResourceManager();
rm.Close();                          // ✅ Natural, sin pensar en Dispose
// rm.Dispose();                     // ❌ No compila
((IDisposable)rm).Dispose();         // ✅ Cuando el using statement lo necesita
using (rm) { /* ... */ }             // ✅ using llama a IDisposable.Dispose()
```

### 4.3. Escenario 3: `Document` — Tres formas distintas de "Render"

```csharp
interface IDrawable  { void Render(); }
interface IPrintable { void Render(); }
interface ISaveable  { void Render(); }

class Document : IDrawable, IPrintable, ISaveable
{
    void IDrawable.Render()
    {
        Console.WriteLine("Renderizando en pantalla (gráficos)...");
    }

    void IPrintable.Render()
    {
        Console.WriteLine("Preparando documento para impresión...");
    }

    void ISaveable.Render()
    {
        Console.WriteLine("Serializando documento a disco...");
    }
}
```

Tres interfaces distintas piden un método `Render()`, pero cada una espera un comportamiento radicalmente distinto. La implementación explícita permite que `Document` sea a la vez dibujable, imprimible y guardable, con una implementación separada para cada caso.

### 4.4. Escenario 4: `LimitedCollection` — Propiedad `Count` e indexador explícitos

```csharp
interface ILimitedList
{
    int Count { get; }
    string this[int index] { get; }
}

class LimitedCollection : ILimitedList
{
    private readonly List<string> _items = new();

    // API público: método Add (no viene de la interfaz)
    public void Add(string item) => _items.Add(item);

    // Count oculto: solo visible si tratas la colección como ILimitedList
    int ILimitedList.Count => _items.Count;

    // Indexador oculto: solo accesible desde ILimitedList
    string ILimitedList.this[int index] => _items[index];

    // Método público para saber el tamaño sin necesitar la interfaz
    public int GetCount() => _items.Count;
}
```

Las propiedades y los indexadores también pueden implementarse explícitamente. La sintaxis es idéntica: `Tipo IInterfaz.Propiedad { get; set; }`. En este ejemplo, `LimitedCollection` oculta `Count` para que el usuario use `GetCount()` y el indexador solo está disponible cuando se trata como `ILimitedList`.

### 4.5. Demo en `Program.cs`

El programa principal demuestra todos los escenarios anteriores con llamadas comentadas que explican qué compila y qué no:

```csharp
// ─── DiagnosticService ───
var diag = new DiagnosticService();
// diag.Log("test");              // ❌ No compila
((ILogger)diag).Log("Inicio");    // ✅ [LOG] ...
((ITracer)diag).Log("Inicio");    // ✅ [TRACE] ...

// ─── ResourceManager ───
var rm = new ResourceManager();
rm.Close();                        // ✅ Liberando recursos...
using (rm) { }                     // ✅ using llama a IDisposable.Dispose()

// ─── Document ───
var doc = new Document();
((IDrawable)doc).Render();         // ✅ Renderizando en pantalla...
((IPrintable)doc).Render();        // ✅ Preparando documento para impresión...
((ISaveable)doc).Render();         // ✅ Serializando documento a disco...

// ─── LimitedCollection ───
var col = new LimitedCollection();
col.Add("uno");
// int count = col.Count;          // ❌ No compila
int count = ((ILimitedList)col).Count; // ✅ Compila
```

---

## 5. Buenas prácticas

### ✅ Hacer

1. **Usa implementación implícita por defecto.** Es más natural, más legible y más fácil de descubrir para quien consuma tu clase. Solo recurre a la explícita cuando realmente lo necesites.

2. **Usa implementación explícita cuando haya conflicto de nombres entre interfaces.** Es su razón de ser. No intentes resolver el conflicto renombrando métodos o usando `new`; la explícita es la solución idiomática.

   ```csharp
   // ✅ Correcto
   class C : IA, IB
   {
       void IA.Do() { }
       void IB.Do() { }
   }
   ```

3. **Usa implementación explícita para ocultar miembros que no son parte del API principal.** Si implementas `ICloneable`, `IComparable`, `IEquatable<T>`, valora si tu consumidor realmente necesita ver esos métodos en cada uso de la clase. Si no, ocúltalos.

   ```csharp
   class Persona : IEquatable<Persona>
   {
       // API público claro
       public string Nombre { get; init; }
       public string Email { get; init; }

       // IEquatable oculto: el consumidor usará LINQ o colecciones, que lo invocan automáticamente
       bool IEquatable<Persona>.Equals(Persona? other) =>
           other is not null && Email == other.Email;
   }
   ```

4. **Documenta que ciertos miembros son explícitos.** Si quien usa tu clase necesita acceder a un método de interfaz, debe saber que tiene que hacer un cast. Un comentario XML en la clase ayuda:

   ```csharp
   /// <summary>
   /// Para acceder al log de diagnóstico, usar ((ITracer)instancia).Log(mensaje).
   /// </summary>
   class DiagnosticService : ITracer { ... }
   ```

5. **Prefiere el cast con `as` + null-conditional cuando no estés seguro de que la interfaz está implementada:**

   ```csharp
   (obj as IA)?.Do();  // ✅ Seguro: si no implementa IA, no hace nada
   ((IA)obj).Do();     // ⚠️ Lanza InvalidCastException si obj no implementa IA
   ```

### ❌ Evitar

1. **No uses `this.Metodo()` dentro de una implementación explícita para llamar a otro método de la misma interfaz explícita.** Eso no compila, porque `this` es del tipo concreto. Si necesitas reutilizar lógica, extrae un método privado:

   ```csharp
   class C : IA
   {
       void IA.Do()
       {
           DoInternal(); // ✅ Delegas a un método privado
       }

       private void DoInternal()
       {
           Console.WriteLine("Lógica compartida");
       }
   }
   ```

2. **No marques una implementación explícita como `virtual`.** No compila. Si necesitas que una subclase pueda sobrescribir el comportamiento, extrae la lógica a un método `protected virtual` y llama a ese desde la implementación explícita.

3. **No abuses de interfaces explícitas para "esconder" métodos que deberían ser públicos.** Si el método tiene sentido en el API de tu clase, hazlo implícito. La ocultación debe tener un motivo (conflicto, API técnico, legado).

---

## 6. Errores habituales

### Error 1: Intentar llamar a un método explícito desde el tipo concreto

```csharp
var diag = new DiagnosticService();
diag.Log("error"); // ❌ CS1061: 'DiagnosticService' no contiene una definición para 'Log'
```

**Por qué falla:** El compilador no ve el método en la clase, solo en la interfaz.

**Corrección:**
```csharp
((ILogger)diag).Log("error"); // ✅
```

### Error 2: Olvidarse de que el método existe

Trabajas en un proyecto, ves la clase `DiagnosticService`, abres Intellisense pulsando `.` y no ves `Log`. Piensas: "esta clase no tiene método Log". Pero sí lo tiene, solo que está oculto tras la interfaz.

**Cómo evitarlo:** Usa la ventana "Go to Implementation" (Ctrl+F12 en VS Code/Rider) sobre el nombre de la interfaz. O revisa la documentación de la clase.

### Error 3: Olvidar prefijar con el nombre de la interfaz

```csharp
class C : IA
{
    void Do() { } // ❌ CS0536: 'C' no implementa el miembro de interfaz 'IA.Do()'
                  //     ¿Olvidaste prefijar con 'IA.'?
}
```

Esto es una implementación implícita fallida porque no lleva `public`. El compilador piensa que intentaste hacerla explícita pero olvidaste el prefijo `IA.`.

**Corrección:**
```csharp
class C : IA
{
    public void Do() { }  // ✅ Implícita (lleva public)
    // o bien:
    void IA.Do() { }      // ✅ Explícita (lleva prefijo)
}
```

### Error 4: Confundir implementación explícita con `new` en métodos de clase

```csharp
class Base
{
    public void Do() => Console.WriteLine("Base");
}

class Derived : Base, IA  // donde IA también tiene Do()
{
    public new void Do() => Console.WriteLine("Derived"); // Esto oculta el de Base
    // Pero NO implementa IA.Do() a menos que sea implícito y coincida
}
```

La implementación explícita **no tiene nada que ver** con `new`. `new` oculta miembros heredados de una clase base. La implementación explícita gestiona miembros de interfaz. No los confundas.

### Error 5: Creer que la implementación explícita es `private` en C#

Aunque en IL se emita como `private`, en C# **no puedes poner `private` delante**. Esto no compila:

```csharp
private void IA.Do() { }  // ❌ CS0106: El modificador 'private' no es válido para este elemento
```

Simplemente omite el modificador. El compilador ya sabe que es explícito.

---

## 7. Ejercicio guiado

Vamos a construir un sistema de logs con múltiples destinos usando implementación explícita.

### Paso 1: Define las interfaces

Crea un proyecto de consola:

```bash
dotnet new console -n MultiLogger -o .
```

Sustituye el contenido de `Program.cs`:

```csharp
// Tres interfaces, mismo método Log, distintos destinos
interface IFileLogger      { void Log(string msg); }
interface IConsoleLogger   { void Log(string msg); }
interface IDatabaseLogger  { void Log(string msg); }
```

### Paso 2: Implementa la clase con implementación explícita

```csharp
class MultiLogger : IFileLogger, IConsoleLogger, IDatabaseLogger
{
    private readonly string _filePath;

    public MultiLogger(string filePath)
    {
        _filePath = filePath;
    }

    // Implementación explícita para archivo
    void IFileLogger.Log(string msg)
    {
        var entry = $"[FILE] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {msg}";
        File.AppendAllText(_filePath, entry + Environment.NewLine);
        Console.WriteLine($"  → Escrito en archivo: {_filePath}");
    }

    // Implementación explícita para consola
    void IConsoleLogger.Log(string msg)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[CONSOLE] {DateTime.Now:T} - {msg}");
        Console.ForegroundColor = originalColor;
    }

    // Implementación explícita para base de datos (simulado)
    void IDatabaseLogger.Log(string msg)
    {
        // Simulamos inserción en BD
        Console.WriteLine($"  → INSERT INTO Logs VALUES ('{msg}') [SIMULADO]");
    }

    // Método público que llama a las tres implementaciones
    public void LogToAll(string msg)
    {
        Console.WriteLine($"\n═══ Registrando '{msg}' en todos los destinos ═══");

        // Para llamar a las implementaciones explícitas desde dentro,
        // hacemos cast de this a cada interfaz
        ((IFileLogger)this).Log(msg);
        ((IConsoleLogger)this).Log(msg);
        ((IDatabaseLogger)this).Log(msg);
    }
}
```

### Paso 3: Programa principal

```csharp
var logger = new MultiLogger("app.log");

// ❌ Esto no compilaría:
// logger.Log("mensaje");

// ✅ Llamamos a través de las interfaces:
((IFileLogger)logger).Log("Archivo de sistema creado");
((IConsoleLogger)logger).Log("Aplicación iniciada");
((IDatabaseLogger)logger).Log("Usuario 'admin' hizo login");

// ✅ O usamos el método público:
logger.LogToAll("Sistema arrancado correctamente");
```

### Paso 4: Ejecuta

```bash
dotnet run
```

**Salida esperada:**

```
  → Escrito en archivo: app.log
[CONSOLE] 12:30:45 - Aplicación iniciada
  → INSERT INTO Logs VALUES ('Usuario 'admin' hizo login') [SIMULADO]

═══ Registrando 'Sistema arrancado correctamente' en todos los destinos ═══
  → Escrito en archivo: app.log
[CONSOLE] 12:30:45 - Sistema arrancado correctamente
  → INSERT INTO Logs VALUES ('Sistema arrancado correctamente') [SIMULADO]
```

### Reflexión

Observa que `LogToAll()` tiene que hacer `((IFileLogger)this).Log(msg)` para invocar las implementaciones explícitas. Si intentara `this.Log(msg)`, el compilador diría que `Log` no existe en `MultiLogger`. El cast a la interfaz es obligatorio incluso desde dentro de la propia clase.

---

## 8. Reto

Crea un sistema de caché con dos niveles usando implementación explícita de interfaces.

### Especificaciones

Define dos interfaces con los mismos métodos pero distinta semántica:

```csharp
interface IMemoryCache
{
    string? Get(string key);
    void Set(string key, string value);
}

interface IDistributedCache
{
    string? Get(string key);
    void Set(string key, string value);
}
```

Implementa `class CacheService : IMemoryCache, IDistributedCache` con estas reglas:

- **`IMemoryCache`**: almacena los datos en un `Dictionary<string, string>` en memoria.
- **`IDistributedCache`**: simula Redis esperando 200 ms (con `Thread.Sleep(200)`) antes de cada operación y mostrando un mensaje `[REDIS] GET key` / `[REDIS] SET key=value`.
- **Ambos `Get`** deben devolver `null` si la clave no existe.
- Añade un método público `ClearAll()` que borre ambas cachés.

### Pistas

- Desde dentro de `ClearAll()`, necesitarás acceder al `Dictionary` directamente (es un campo privado) o hacer cast a `IMemoryCache` para llamar a una operación de limpieza que diseñes.
- Para simular Redis, puedes usar un segundo `Dictionary` como almacén del cache distribuido.
- Recuerda: si `Set` se implementa explícitamente, no puedes llamarlo con `this.Set(...)` desde `ClearAll()`. Necesitarás el cast `((IMemoryCache)this).Set(...)`.

Intenta resolverlo antes de mirar la solución. En la carpeta `soluciones/` de esta lección encontrarás una implementación de referencia.

---

## 9. Resumen

| Característica | Implementación implícita | Implementación explícita |
|---|---|---|
| **Acceso desde el tipo concreto** | ✅ Sí (`obj.Metodo()`) | ❌ No (error de compilación) |
| **Acceso desde la interfaz** | ✅ Sí | ✅ Sí |
| **Visibilidad en Intellisense** (tipo concreto) | ✅ Visible | ❌ Oculto |
| **Visibilidad en Intellisense** (variable de interfaz) | ✅ Visible | ✅ Visible |
| **Sintaxis** | `public void Metodo() { }` | `void IInterfaz.Metodo() { }` |
| **Modificador de acceso** | `public` obligatorio | Ninguno (el compilador lo prohíbe) |
| **Útil para conflictos de nombres** | ❌ No (no compilaría) | ✅ Sí (su propósito principal) |
| **Útil para ocultar API técnico** | ❌ No se puede ocultar | ✅ Sí (`IDisposable`, `ICloneable`) |
| **Se puede heredar en subclase** | ✅ Sí, con `virtual`/`override` | Sí, pero requiere reimplementación de interfaz |
| **Emisión en IL** | Método público virtual | Método `private` + mapeo de interfaz |
| **Uso recomendado** | Caso general (90% del tiempo) | Conflictos y ocultación de API técnico |

### Conceptos importantes

- La **implementación explícita** es una herramienta de control de visibilidad, no una característica de diseño diario.
- El **conflicto de nombres entre interfaces** es su caso de uso principal.
- **Ocultar `IDisposable.Dispose()`** tras un `Close()` público es un patrón consagrado en el propio framework de .NET.
- Los miembros explícitos **no llevan modificador de acceso** y siempre van **prefijados** con el nombre de la interfaz.
- Para invocarlos, necesitas un **cast a la interfaz** — incluso desde dentro de la propia clase.

---

## 10. Preguntas frecuentes

### ¿Puedo implementar parte de una interfaz de forma implícita y otra parte de forma explícita?

**No.** Una vez que decides implementar una interfaz de forma explícita, **todos** los miembros de esa interfaz deben ser explícitos. No puedes tener en la misma clase `public void Metodo1()` (implícito) y `void IMiInterfaz.Metodo2()` (explícito) para la misma interfaz.

Sin embargo, sí puedes implementar una interfaz de forma implícita y **otra distinta** de forma explícita en la misma clase. Eso es perfectamente válido y muy común.

```csharp
class Ejemplo : IA, IB
{
    public void MetodoDeIA() { }  // ✅ IA implícita
    void IB.MetodoDeIB() { }      // ✅ IB explícita (distinta interfaz, OK)
}
```

### ¿Puedo heredar una implementación explícita en una subclase?

**Sí**, pero con matices. Si una clase base implementa una interfaz explícitamente, la subclase **hereda** esa implementación. Si la subclase **vuelve a declarar** que implementa la interfaz (`class Sub : Base, IMiInterfaz`), puede **reimplementar** el miembro explícito:

```csharp
class Base : IA
{
    void IA.Do() => Console.WriteLine("Base");
}

class Sub : Base, IA  // ← re-declara IA
{
    void IA.Do() => Console.WriteLine("Sub"); // Reimplementa
}
```

Esto es un caso avanzado que raramente se necesita. Lo habitual es que si necesitas modificar el comportamiento, uses un método `protected virtual` en la clase base y hagas que la implementación explícita lo invoque.

### ¿Las propiedades pueden ser explícitas?

**Sí.** La sintaxis es la misma que con los métodos:

```csharp
interface IContador
{
    int Valor { get; }
}

class Contador : IContador
{
    private int _valor;
    int IContador.Valor => _valor;
}
```

### ¿Los indexadores pueden ser explícitos?

**Sí.** La sintaxis es análoga:

```csharp
interface IColeccion
{
    string this[int index] { get; }
}

class Coleccion : IColeccion
{
    private readonly List<string> _items = new();
    string IColeccion.this[int index] => _items[index];
}
```

### ¿Los eventos pueden ser explícitos?

**Sí.** Aunque es poco común:

```csharp
interface INotificador
{
    event EventHandler AlgoCambio;
}

class Notificador : INotificador
{
    private EventHandler? _algoCambio;
    event EventHandler INotificador.AlgoCambio
    {
        add    { _algoCambio += value; }
        remove { _algoCambio -= value; }
    }
}
```

### ¿Puedo usar `var` con una implementación explícita?

La variable declarada con `var` toma el tipo del lado derecho. Si el lado derecho es el tipo concreto, `var` infiere el tipo concreto, y no podrás acceder al método explícito:

```csharp
var diag = new DiagnosticService();
// diag.Log(...); // ❌ 'var' infiere 'DiagnosticService', no 'ILogger'
```

Si necesitas acceder al método, declara la variable con el tipo de la interfaz:

```csharp
ILogger diag = new DiagnosticService();
diag.Log("test"); // ✅
```

### ¿Las implementaciones explícitas pueden ser async?

**Sí.** Simplemente devuelven `Task` o `Task<T>` como cualquier método async:

```csharp
interface IRepo
{
    Task<string> GetAsync(int id);
}

class Repo : IRepo
{
    async Task<string> IRepo.GetAsync(int id)
    {
        await Task.Delay(100);
        return $"item-{id}";
    }
}
```

### ¿Cómo detecto si una clase tiene una implementación explícita de una interfaz?

En tiempo de compilación: revisa la documentación o el código fuente. En tiempo de ejecución: usa reflexión con `BindingFlags.NonPublic | BindingFlags.Instance` o el método `GetInterfaceMap()`:

```csharp
var map = typeof(MiClase).GetInterfaceMap(typeof(IMiInterfaz));
foreach (var m in map.TargetMethods)
    Console.WriteLine($"Implementado por: {m.Name}");
```
