# Módulo 2 — Lección 09: Boxing y Unboxing en C#

---

## 1. Introducción

Boxing y unboxing son las operaciones que convierten entre **value types** (stack) y **reference types** (heap). Aunque C# las hace transparentes para el programador, tienen un coste de rendimiento significativo: alocación en heap + copia de datos + presión en el GC. Entender cuándo ocurren es esencial para escribir código .NET de alto rendimiento.

El boxing existe desde C# 1.0 como puente necesario entre el mundo de los value types (rápidos, en stack) y el sistema de tipos unificado de .NET donde todo deriva de `System.Object`. Java tiene un mecanismo similar llamado *autoboxing* (`int` ↔ `Integer`), pero con clases contenedoras explícitas para cada primitivo. En .NET, el boxing convierte directamente cualquier value type a `object` sin clases intermedias.

---

## 2. Teoría

### 2.1 Stack vs Heap

| Zona | Qué almacena | Gestión | Velocidad |
|---|---|---|---|
| **Stack** | Value types, referencias a objetos, parámetros de método, variables locales | LIFO automático (por frame de método) | Muy rápida (puntero de pila) |
| **Heap** | Objetos (instancias de `class`, arrays, strings, objetos boxed) | Garbage Collector (GC) | Más lenta (compactación, recolección) |

Los value types viven en el stack (o *inline* dentro de objetos en el heap, como campos de una `class`). Los reference types viven en el heap gestionado por el GC y solo almacenan una referencia (puntero) en el stack.

### 2.2 Boxing

**Boxing** es el proceso de tomar un value type, crear un `object` en el heap que lo contiene, y obtener una referencia a ese `object`.

Pasos internos del CLR:

1. El CLR aloca memoria en el heap (~12-24 bytes adicionales más el tamaño del struct: SyncBlock index + Method Table pointer + datos).
2. Copia el valor del struct bit a bit desde el stack al espacio alocado en el heap.
3. Devuelve una referencia (`object`) al valor *boxed*.

```
┌─────────── STACK ───────────┐      ┌─────────── HEAP ────────────┐
│                             │      │                              │
│  int x = 42;                │      │  ┌──────────────────────┐    │
│  ┌─────┐                    │      │  │  object (boxed int)   │    │
│  │ 42  │                    │ ──── │  │  SyncBlock | MT | 42 │    │
│  └─────┘                    │ copy │  └──────────────────────┘    │
│                             │      │                              │
└─────────────────────────────┘      └──────────────────────────────┘
```

El valor original en el stack **no se modifica**. Existen ahora dos copias independientes: el `int` en el stack y el `object` en el heap.

Cada boxing crea una **nueva** copia en el heap. Si haces boxing del mismo valor dos veces, obtienes dos objetos distintos en el heap.

### 2.3 Unboxing

**Unboxing** es extraer el value type de vuelta desde el `object`.

Pasos internos:

1. Verificar que el `object` realmente contiene un valor del tipo esperado. Si no coincide → `InvalidCastException` en tiempo de ejecución (no en compilación).
2. Copiar el valor del heap al stack.

```
┌─────────── HEAP ────────────┐      ┌─────────── STACK ───────────┐
│  ┌──────────────────────┐   │      │                             │
│  │  object (boxed int)  │   │ ──── │  int y = 42;               │
│  │  SyncBlock | MT | 42 │   │ copy │  ┌─────┐                   │
│  └──────────────────────┘   │      │  │ 42  │                   │
└──────────────────────────────┘      │  └─────┘                   │
                                      └─────────────────────────────┘
```

El objeto en el heap **no se modifica**. Se copia el valor al stack.

**Importante:** el unboxing requiere un cast explícito al tipo **exacto**. No se puede hacer unboxing de un `double` boxed a `int` directamente; hay que hacerlo en dos pasos: unboxing a `double` y luego conversión explícita a `int`.

Para unboxing seguro con posibles `null`, usar `as` con nullable value types:
```csharp
object? nulo = null;
int? resultadoSeguro = nulo as int?; // Devuelve null sin excepción
```

### 2.4 Cuándo ocurre boxing

| Situación | Ejemplo | ¿Boxing? |
|---|---|---|
| Asignar value type a `object` | `object o = 42;` | SÍ |
| Pasar value type a método con parámetro `object` | `Console.WriteLine(42);` | SÍ (en sobrecargas antiguas) |
| Asignar struct a interfaz (genérica o no) | `IComparable c = 42;` | SÍ |
| Colecciones no genéricas | `ArrayList.Add(42);` | SÍ |
| `GetType()` en value type | `42.GetType();` | SÍ (en la mayoría de casos) |
| Concatenación con `+` y value types | `"Número: " + 42;` | SÍ (usa `string.Concat(object, object)`) |
| Métodos de `System.Enum` | `DayOfWeek.Monday.ToString();` | SÍ (llamada interna a `Enum.ToString()`) |
| `string.Format` con value types | `string.Format("{0}", 42);` | SÍ |
| Conversión explícita `object` → value type | `int x = (int)obj;` | Unboxing |

### 2.5 Coste

- **Memoria:** ~12-24 bytes en el heap por cada boxing (SyncBlock index de 4-8 bytes + Method Table pointer de 8 bytes + tamaño del struct).
- **Tiempo:** ~5-20 nanosegundos por operación en hardware moderno.
- **Presión sobre el GC:** cada boxing genera un objeto que el GC debe rastrear y eventualmente recolectar. En bucles intensivos (1M+ iteraciones), el GC puede dispararse frecuentemente degradando el rendimiento.
- **Pérdida de localidad de caché:** los objetos boxed se dispersan por el heap, perdiendo la ventaja de acceso secuencial que tiene un array de structs contiguos.

---

## 3. Ejemplo mínimo

```csharp
int x = 42;           // stack: 4 bytes, acceso directo
object o = x;         // BOXING: se copia 42 al heap (~40 bytes), 'o' referencia al objeto boxed
int y = (int)o;       // UNBOXING: se extrae el valor del heap y se copia al stack

Console.WriteLine($"x={x}, o={o}, y={y}"); // 42, 42, 42

// Modificar x NO afecta a o (son independientes)
x = 99;
Console.WriteLine($"x={x}, o={o}"); // 99, 42 — ¡son objetos distintos!
```

### Línea por línea

- `int x = 42;` — El valor `42` se almacena directamente en el stack. Ocupa 4 bytes (tamaño de `int`).
- `object o = x;` — Aquí ocurre **boxing**: el CLR aloca ~40 bytes en el heap (sync block + method table + 4 bytes del int + alineación), copia `42` allí, y guarda la referencia en `o`. Ahora hay DOS copias de `42`: una en el stack (`x`) y otra en el heap (objeto boxed apuntado por `o`).
- `int y = (int)o;` — Aquí ocurre **unboxing**: el CLR verifica que el objeto en heap realmente sea un `int`, luego copia su valor al stack en la variable `y`. Si el tipo no coincidiera, lanzaría `InvalidCastException`.
- `x = 99;` — Modifica SOLO la copia del stack. El objeto boxed en el heap sigue valiendo `42`.

---

## 4. Ejemplo real

```bash
dotnet run --project ejemplos/m02-poo/Leccion-09-Boxing-Unboxing/src/PooBoxing
```

El programa (`src/PooBoxing/Program.cs`) cubre **9 casos concretos** donde ocurre boxing y unboxing:

| # | Caso | Descripción |
|---|---|---|
| 1 | `object o = valueType` | Boxing directo al asignar a `object` |
| 2 | Parámetro `object` en método | `PrintObject(50)` — boxing implícito |
| 3 | Asignación a interfaz | `IComparable c = 42` — boxing incluso con interfaces genéricas |
| 4 | `ArrayList` | Cada `Add(1)` hace boxing, cada `(int)list[0]` hace unboxing |
| 5 | `GetType()` | `42.GetType()` — el compilador hace boxing para llamar al método |
| 6 | Concatenación de strings | `"Número: " + 42` — boxing vía `string.Concat(object, object)` |
| 7 | `Enum.ToString()` | `DayOfWeek.Monday.ToString()` — boxing interno en `System.Enum` |
| 8 | Struct propio a interfaz | `IEquatable<Point> eq = punto` — boxing del struct |
| 9 | `string.Format` | `string.Format("{0}", 10)` — cada parámetro value type hace boxing |

**Demos adicionales en el programa:**

- **Contador manual de boxing**: método genérico `DetectarBoxing<T>(T valor)` que incrementa un contador antes de hacer el boxing. Se demuestra que 100.000 boxing generan ~2.4 MB de basura en el heap.
- **Comparativa de rendimiento con `Stopwatch`**: 10 millones de iteraciones comparando `ArrayList` (boxing + unboxing en cada operación) vs `List<int>` (cero boxing) vs `int[]` (acceso directo a memoria continua). La diferencia de velocidad suele ser de 3x–10x.
- **`Point` struct**: demuestra cómo sobrescribir `ToString()`, `GetHashCode()`, `Equals()` e implementar `IEquatable<Point>` evita boxing en comparaciones.
- **`Money` struct**: ejemplo real de struct vs class con `IEquatable<Money>`, `IComparable<Money>` y la versión no genérica `IComparable` (que sí causa boxing). Se usa en `Dictionary<Money, string>` para demostrar búsquedas sin boxing.

---

## 5. Buenas prácticas

### Qué hacer

| Práctica | Por qué |
|---|---|
| Usar colecciones genéricas: `List<T>`, `Dictionary<K,V>` | Almacenan value types directamente en el array interno, sin boxing. `ArrayList`/`Hashtable` causan boxing en cada `Add`. |
| Evitar pasar structs a métodos que esperan `object` o interfaces no genéricas | Cada conversión implícita a `object` o interfaz causa boxing. |
| Sobrescribir `ToString()`, `GetHashCode()`, `Equals()` en structs | Evita el boxing implícito al llamar a estos métodos, porque el compilador resuelve la llamada al override en tiempo de compilación. |
| Implementar `IEquatable<T>` en structs | `Dictionary<K,V>` y `HashSet<T>` usan `EqualityComparer<T>.Default`, que prefiere `IEquatable<T>.Equals()` evitando boxing. |
| Usar métodos genéricos con restricción `where T : struct` | El JIT genera código especializado para cada `T`, evitando cualquier boxing. |
| Usar interpolación de strings (`$"{value}"`) en lugar de `string.Format` o concatenación con `+` | En .NET 6+, la interpolación usa `ISpanFormattable` y genéricos, evitando boxing para tipos estándar. |
| Para structs grandes (>16 bytes), considerar `class` en su lugar | Copiar structs grandes en cada asignación puede ser más caro que el GC de una clase. |

### Qué evitar

| Práctica | Por qué |
|---|---|
| `ArrayList`, `Hashtable`, `Queue`, `Stack` (no genéricos) | Son reliquias de .NET 1.0. Causan boxing en cada operación. |
| `string.Format` con muchos value types | Cada argumento se convierte a `object`, causando boxing. |
| Usar `IComparable` (no genérico) en lugar de `IComparable<T>` | Obliga a hacer boxing del struct al pasarlo como parámetro. |
| Asignar structs a variables de tipo interfaz sin necesidad | `IEquatable<Point> eq = punto` causa boxing; llama directamente a `punto.Equals(otro)`. |
| Llamar a `GetType()` en value types dentro de bucles | Causa boxing innecesario. Usa `typeof(T)` en su lugar si trabajas con genéricos. |

---

## 6. Errores habituales

### 6.1 Modificar struct a través de interfaz (NO modifica el original)

```csharp
// ❌ MAL: la modificación se pierde porque se opera sobre la copia boxed
interface IIncrementable { void Increment(); }

struct Counter : IIncrementable
{
    public int Value;
    public void Increment() => Value++;
}

Counter c = new Counter();
IIncrementable inc = c; // BOXING: se crea una copia en el heap
inc.Increment();         // Modifica la copia boxed, NO el struct original
Console.WriteLine(c.Value); // 0 — ¡el original no cambió!

// ✅ BIEN: trabajar directamente con el struct
c.Increment();
Console.WriteLine(c.Value); // 1
```

### 6.2 Usar `ArrayList` por costumbre

```csharp
// ❌ MAL: boxing en cada Add, unboxing en cada lectura
ArrayList list = new ArrayList();
for (int i = 0; i < 1000; i++)
    list.Add(i); // 1000 boxing
foreach (int i in list) // 1000 unboxing
    Console.WriteLine(i);

// ✅ BIEN: cero boxing
List<int> list = new List<int>();
for (int i = 0; i < 1000; i++)
    list.Add(i); // Directo al array interno, sin boxing
```

### 6.3 No sobrescribir `ToString()` en structs de uso frecuente

```csharp
// ❌ MAL: cada llamada a ToString() causa boxing si no hay override
struct BadPoint { public int X, Y; }
BadPoint bp = new BadPoint { X = 1, Y = 2 };
Console.WriteLine(bp); // BOXING: llama a ValueType.ToString() vía object

// ✅ BIEN: override evita el boxing
struct GoodPoint
{
    public int X, Y;
    public override string ToString() => $"({X}, {Y})"; // Sin boxing
}
GoodPoint gp = new GoodPoint { X = 1, Y = 2 };
Console.WriteLine(gp); // Sin boxing, llama al override directamente
```

### 6.4 Usar `Enum.GetValues()` en bucles

```csharp
// ❌ MAL: boxing en cada iteración
foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
    Console.WriteLine(day); // Boxing en cada elemento

// ✅ BIEN: usar Enum.GetValues<T>() (disponible en .NET 5+)
foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
    Console.WriteLine(day); // Sin boxing
```

### 6.5 Comparar structs con `.Equals(object)` en lugar de `IEquatable<T>`

```csharp
// ❌ MAL: boxing del argumento
Point p1 = new Point(1, 2);
Point p2 = new Point(1, 2);
bool iguales = p1.Equals(p2); // p2 se boxea como object

// ✅ BIEN: IEquatable<T> evita boxing
bool iguales = ((IEquatable<Point>)p1).Equals(p2); // Aún boxing de p1
// Realmente bien: llamada directa con método Equals(Point) sobrecargado
bool iguales = p1.Equals(p2); // El compilador elige Equals(Point) — sin boxing
```

---

## 7. Ejercicio guiado

**Objetivo:** optimizar código legacy eliminando toda operación de boxing y unboxing, y medir la diferencia de rendimiento.

### Código original (con boxing)

```csharp
ArrayList numbers = new ArrayList();
for (int i = 0; i < 1000; i++)
    numbers.Add(i); // 1000 boxing

int sum = 0;
foreach (object item in numbers)
    sum += (int)item; // 1000 unboxing
```

### Solución optimizada

```csharp
using System.Diagnostics;

const int iterations = 1_000_000;

// ──── Versión con boxing (ArrayList) ────
var sw1 = Stopwatch.StartNew();
ArrayList numbersArrayList = new ArrayList(iterations);
for (int i = 0; i < iterations; i++)
    numbersArrayList.Add(i); // BOXING: cada int se copia al heap

long sum1 = 0;
for (int i = 0; i < iterations; i++)
    sum1 += (int)numbersArrayList[i]!; // UNBOXING: cada object se extrae del heap
sw1.Stop();

// ──── Versión sin boxing (List<int>) ────
var sw2 = Stopwatch.StartNew();
List<int> numbersList = new List<int>(iterations);
for (int i = 0; i < iterations; i++)
    numbersList.Add(i); // SIN boxing: el int se almacena directamente en el array interno

long sum2 = 0;
for (int i = 0; i < iterations; i++)
    sum2 += numbersList[i]; // SIN unboxing: acceso directo al array tipado
sw2.Stop();

Console.WriteLine($"ArrayList (boxing): {sw1.ElapsedMilliseconds} ms — suma={sum1}");
Console.WriteLine($"List<int> (sin boxing): {sw2.ElapsedMilliseconds} ms — suma={sum2}");
Console.WriteLine($"Diferencia: ArrayList es {sw1.ElapsedMilliseconds / (double)sw2.ElapsedMilliseconds:F1}x más lento");
```

**Resultado típico:** `ArrayList` es entre 3x y 8x más lento que `List<int>`, y genera cientos de MB de basura que fuerzan colecciones del GC.

---

## 8. Reto

Crea un `struct Vector3` con campos `X`, `Y`, `Z` (tipo `float`) y las siguientes características:

1. **Operaciones aritméticas**: suma, resta y multiplicación por escalar mediante operadores (`+`, `-`, `*`).
2. **Interfaces genéricas**: implementa `IEquatable<Vector3>` y `IComparable<Vector3>` (comparar por magnitud: `MathF.Sqrt(X*X + Y*Y + Z*Z)`).
3. **Sobrescribe** `ToString()`, `GetHashCode()` y `Equals(object)` para evitar boxing.
4. **Procesa 1 millón de vectores** en dos escenarios:
   - **Con boxing**: almacenando los vectores en `ArrayList`, comparando con `IComparable` (no genérico) y llamando a `Equals(object)`.
   - **Sin boxing**: almacenando en `List<Vector3>`, comparando con `IComparable<Vector3>` y llamando a `Equals(Vector3)`.
5. **Mide la diferencia** con `Stopwatch` (tiempo y memoria con `GC.GetTotalMemory`).
6. **(Opcional avanzado)** Analiza las alocaciones con `dotnet counters`:
   ```bash
   dotnet counters monitor --process-id <PID> --counters System.Runtime
   ```

**Pista:** para la comparación por magnitud ten en cuenta que `MathF.Sqrt` es una operación de punto flotante; puedes comparar los cuadrados de las magnitudes para evitar la raíz cuadrada en `IComparable<Vector3>`.

---

## 9. Resumen

### Tabla de qué hacer y qué evitar

| Situación | ❌ Mal (con boxing) | ✅ Bien (sin boxing) |
|---|---|---|
| Colección de `int` | `ArrayList` | `List<int>` |
| Diccionario con clave struct | `Hashtable` | `Dictionary<TKey, TValue>` |
| Método que recibe cualquier tipo | `void Process(object item)` | `void Process<T>(T item) where T : struct` |
| Comparar structs | `IComparable` (no genérico) | `IComparable<T>` (genérico) |
| Igualdad de structs | `Equals(object obj)` | `IEquatable<T>.Equals(T other)` |
| Mostrar struct en consola | Sin override de `ToString()` | Con `override string ToString()` |
| Formatear strings | `string.Format("{0}", val)` | `$"El valor es {val}"` |
| Iterar valores de enum | `Enum.GetValues(typeof(T))` | `Enum.GetValues<T>()` |

### Conceptos clave

- **Boxing**: value type → object. Alocación en heap, copia del valor, presión sobre el GC.
- **Unboxing**: object → value type. Cast explícito requerido, `InvalidCastException` si el tipo no coincide.
- **Los genéricos** (`List<T>`, `Dictionary<K,V>`, métodos genéricos con `where T : struct`) son la herramienta principal para evitar boxing.
- **El JIT** genera código especializado para cada `T` en genéricos, eliminando la necesidad de boxing.
- **Colecciones no genéricas** (`ArrayList`, `Hashtable`, `Queue`, `Stack`) son vestigios de .NET 1.0 y deben evitarse en código nuevo.
- **La interpolación de strings** moderna (`$""`) evita boxing para tipos que implementan `ISpanFormattable` (todos los tipos numéricos estándar en .NET 8).

---

## 10. Preguntas frecuentes

**P: ¿Boxing siempre es malo?**

R: No. En aplicaciones normales (web API, servicios, aplicaciones de escritorio con uso moderado de datos), el impacto del boxing es insignificante. Preocupa en *hot paths* (bucles con millones de iteraciones), aplicaciones de alta concurrencia, procesamiento de datos en tiempo real o juegos. La regla: no optimices prematuramente, pero conoce el mecanismo para no abusar de él sin saberlo.

**P: ¿`var` evita boxing?**

R: No. `var` es solo inferencia de tipos **en tiempo de compilación**. El IL generado es idéntico al que se generaría con el tipo explícito. `var x = 42;` es exactamente igual que `int x = 42;`. Si luego haces `object o = x;`, hay boxing independientemente de que uses `var`.

**P: ¿Los genéricos evitan boxing en structs?**

R: Sí, completamente. Cuando escribes `List<int>`, el JIT genera una versión especializada de `List<T>` donde `T = int`. El array interno es `int[]` y todas las operaciones trabajan directamente con `int`, sin conversiones a `object`. Esto es una de las razones principales por las que los genéricos se añadieron en C# 2.0.

**P: ¿Cómo detectar boxing en mi código?**

R: Varias herramientas:
- **IL generado**: compila y busca la instrucción `box` en el IL con `ildasm` o `ILSpy`.
- **JetBrains dotMemory**: muestra alocaciones de objetos boxed en tiempo real.
- **PerfView**: analiza eventos ETW del GC, incluyendo alocaciones de boxing.
- **Visual Studio Diagnostic Tools**: la pestaña *Memory Usage* muestra alocaciones, incluidos objetos boxed.
- **BenchmarkDotNet**: con `[MemoryDiagnoser]` muestra bytes alocados y colecciones del GC, que se disparan con boxing excesivo.

**P: ¿Los `record struct` hacen boxing?**

R: No, salvo que los uses como `object` o los asignes a una interfaz no genérica. `record struct` es sintácticamente un struct con `Equals`, `GetHashCode` y `ToString` generados por el compilador, que ya incluyen las optimizaciones para evitar boxing (implementan `IEquatable<T>` automáticamente).

**P: ¿Cuánto cuesta realmente un boxing en términos de tiempo?**

R: Un solo boxing cuesta ~5-20 nanosegundos en hardware moderno. Individualmente es despreciable. El problema real es la **presión sobre el GC**: 1 millón de boxing por segundo genera ~12-24 MB de basura por segundo. El GC de .NET está muy optimizado, pero en escenarios de alta frecuencia (procesamiento de streams, bucles de renderizado, servidores con miles de peticiones/segundo) puede convertirse en un cuello de botella.

**P: ¿Existe boxing en Java?**

R: Sí. Java tiene *autoboxing*: conversión automática entre `int` ↔ `Integer`, `double` ↔ `Double`, etc. A diferencia de C# donde todo se boxea a `object`, Java tiene una clase contenedora por cada primitivo (`Integer`, `Long`, `Boolean`, etc.). El mecanismo es similar: alocación en heap y presión sobre el GC. La diferencia principal es que en C# puedes boxear cualquier struct, no solo los tipos primitivos.

**P: ¿Existe boxing en JavaScript?**

R: No exactamente. En JavaScript los primitivos se "wrappean" temporalmente al llamar métodos (ej. `(42).toString()`), pero este wrapper es efímero y el motor V8 lo optimiza agresivamente. No hay un heap gestionado con boxing persistente como en .NET o Java.

---

**Siguiente:** Módulo 3 — SOLID en C#
