# Lección 02 — CLR, BCL y cómo funciona la compilación

---

## 1. Introducción

En esta lección vamos a meternos **dentro del motor** de .NET. No vamos a escribir código de negocio, vamos a entender **qué le pasa a nuestro código** desde que escribimos `Console.WriteLine("Hola")` hasta que el procesador ejecuta esa instrucción.

Vamos a responder:

- **¿Qué es el CLR?** El motor que hace funcionar .NET. Como la JVM pero con superpoderes.
- **¿Qué es la BCL?** Las 10 000+ clases que vienen "de fábrica" y que te ahorran escribir todo desde cero.
- **¿Cómo funciona el GC?** El recolector de basura que gestiona la memoria por ti. Sin `free()`, sin `delete`, sin memory leaks (en teoría).
- **¿Qué es IL?** El "lenguaje intermedio" al que se compila C# antes de llegar al procesador.
- **¿Qué es JIT?** El compilador que convierte IL en código máquina justo antes de ejecutarlo.
- **Errores típicos:** Pensar que .NET es interpretado (como Python). Llamar a `GC.Collect()` manualmente. Ignorar la diferencia entre stack y heap.

> **Objetivo:** Al terminar esta lección, entenderás exactamente qué pasa dentro del runtime cuando tu código se ejecuta, y por qué eso importa para el rendimiento.

---

## 2. Teoría

### 2.1 El pipeline de compilación: C# → IL → JIT → Máquina

Cuando escribes código C# y ejecutas `dotnet run`, ocurre esto:

```
┌──────────┐     ┌──────────┐     ┌──────────┐     ┌──────────┐
│  C#      │────▶│   IL     │────▶│   JIT    │────▶│  Código  │
│ (tu)     │     │ (Roslyn) │     │ (RyuJIT) │     │  Máquina │
└──────────┘     └──────────┘     └──────────┘     └──────────┘
    .cs              .dll            Runtime           CPU
```

#### Fase 1: Roslyn (compilador C# → IL)

- **Qué hace:** Convierte tu código C# a **IL (Intermediate Language)**, también llamado **MSIL** o **CIL**.
- **Cuándo:** En tiempo de compilación (`dotnet build`), NO en ejecución.
- **Resultado:** Un archivo `.dll` o `.exe` que contiene IL + metadata.
- **Analogía Java:** `javac` compila `.java` → `.class` (bytecode).
- **Analogía Python:** Python no tiene este paso (es interpretado, aunque `.pyc` es similar).

#### Fase 2: CLR (Common Language Runtime)

- **Qué hace:** El CLR carga el assembly, verifica la seguridad de tipos, y gestiona la ejecución.
- **Componentes del CLR:**

```
┌────────────────────────────────────────────────┐
│                  CLR                           │
├────────────────────────────────────────────────┤
│  Class Loader     │ Carga assemblies en memoria│
│  JIT Compiler     │ IL → código máquina        │
│  Garbage Collector│ Gestión automática de mem. │
│  Exception Manager│ Captura y propaga exceptions│
│  Thread Manager   │ Thread pool y scheduling   │
│  Security Manager │ Code Access Security (CAS) │
│  Debug Engine     │ Soporte para debugging     │
└────────────────────────────────────────────────┘
```

#### Fase 3: JIT (Just-In-Time Compilation)

- **Qué hace:** Convierte IL → código máquina nativo (x64, ARM64, etc.) **justo antes de ejecutar** cada método.
- **Por qué JIT y no AOT (compilar todo de una vez)?**
  - Optimiza para la CPU específica donde se ejecuta (AVX2, SSE4, etc.)
  - Solo compila lo que realmente se usa (métodos no llamados = no se compilan)
  - Puede recompilar con más optimizaciones si un método se llama mucho (Tiered Compilation)

#### Fase 4: Ejecución en CPU

- El código máquina se ejecuta directamente en el procesador.
- El GC se ejecuta en segundo plano, liberando memoria no utilizada.

### 2.2 IL (Intermediate Language) en detalle

IL es un **lenguaje de pila** (stack-based). Esto significa que las operaciones no usan registros directamente, sino una pila virtual.

**Ejemplo: `int resultado = 3 + 5;`**

```
C#:              int resultado = 3 + 5;

IL generado:
  ldc.i4.3       // Load Constant: push 3 al stack
  ldc.i4.5       // Load Constant: push 5 al stack
  add            // Pop 3 y 5, sumar, push resultado (8)
  stloc.0        // Store Local: guarda 8 en variable local 0
```

**¿Por qué stack-based?**
- Es más fácil de verificar (cada operación sabe cuántos elementos espera en el stack)
- Es portable (no depende de registros específicos de CPU)
- El JIT lo convierte a código basado en registros para la CPU real

### 2.3 Tiered Compilation

Desde .NET Core 3.0, el JIT compila en dos niveles:

| Característica | Tier 0 | Tier 1 |
|---|---|---|
| **Velocidad de compilación** | Rápida (~pocos ms) | Lenta (~10-100ms) |
| **Optimizaciones** | Mínimas | Todas (inlining, loop unrolling...) |
| **Cuándo se usa** | Primera llamada al método | Después de ~30 llamadas |
| **Objetivo** | Arranque rápido | Máximo rendimiento |
| **Contrapartida** | Código menos optimizado | Código muy optimizado |

El CLR cuenta cuántas veces se llama a cada método y automáticamente lo recompila en Tier 1 cuando vale la pena. Esto es transparente para el desarrollador.

### 2.4 El Garbage Collector (GC)

El GC es probablemente el componente más importante y peor entendido del CLR.

#### ¿Por qué existe?

En C/C++ gestionas la memoria manualmente: `malloc`/`free`, `new`/`delete`. Esto causa:
- **Memory leaks:** olvidas liberar → la memoria crece sin control
- **Dangling pointers:** liberas y luego usas → crash o corrupción de datos
- **Double free:** liberas dos veces → comportamiento indefinido

El GC elimina estas categorías de bugs **automáticamente**. Tú creas objetos, el GC los destruye cuando ya no se usan.

#### Generaciones

El GC divide el heap en 3 generaciones basándose en la **hipótesis generacional**:
"Los objetos que mueren jóvenes mueren muy jóvenes. Los que sobreviven, tienden a vivir mucho."

```
┌─────────────────────────────────────────────────────┐
│                      HEAP                           │
├──────────┬──────────┬──────────┬────────────────────┤
│  Gen 0   │  Gen 1   │  Gen 2   │  LOH (Large Obj)  │
│  ~256KB  │  ~2MB    │  ~10MB   │  >= 85000 bytes   │
├──────────┼──────────┼──────────┼────────────────────┤
│  Nuevos  │  Sobrev. │  Longevos│  Arrays grandes,   │
│  objetos │  de Gen0 │  (cachés)│  strings largos    │
└──────────┴──────────┴──────────┴────────────────────┘
```

- **Gen 0:** Todos los objetos nuevos empiezan aquí. Se recolecta muy frecuentemente (~cada pocos ms bajo carga). Muy rápido porque es pequeño.
- **Gen 1:** Buffer entre Gen 0 y Gen 2. Objetos que sobrevivieron a una recolección de Gen 0.
- **Gen 2:** Objetos longevos. Se recolecta raramente. Una recolección completa de Gen 2 ("full GC") puede pausar la aplicación ~10-100ms.
- **LOH:** Objetos mayores de 85 000 bytes (~83 KB). No se compacta por defecto (mover objetos enormes es caro). Puede fragmentarse.

#### Fases de una recolección

1. **Mark:** El GC recorre todas las "raíces" (variables locales en el stack, campos static, registros de CPU) y marca como "vivos" todos los objetos alcanzables.
2. **Sweep:** Libera la memoria de los objetos no marcados.
3. **Compact (opcional):** Mueve los objetos vivos para juntarlos y eliminar huecos (fragmentación). Solo en Gen 0/1/2, no en LOH por defecto.

### 2.5 La BCL (Base Class Library)

La BCL es la biblioteca estándar de .NET. Contiene **más de 10 000 clases** organizadas en namespaces.

**Namespaces principales:**

| Namespace | ¿Qué contiene? | Ejemplos |
|---|---|---|
| `System` | Tipos fundamentales | `Object`, `Int32`, `String`, `DateTime`, `Console`, `Math`, `GC` |
| `System.Collections.Generic` | Colecciones genéricas | `List<T>`, `Dictionary<K,V>`, `HashSet<T>`, `Queue<T>` |
| `System.Linq` | Consultas integradas | `Where`, `Select`, `GroupBy`, `Join`, `Aggregate` |
| `System.IO` | Entrada/Salida | `File`, `Directory`, `Stream`, `Path`, `FileInfo` |
| `System.Net.Http` | HTTP | `HttpClient`, `HttpRequestMessage`, `HttpResponseMessage` |
| `System.Text.Json` | JSON moderno | `JsonSerializer`, `JsonDocument`, `JsonNode` |
| `System.Threading.Tasks` | Programación asíncrona | `Task`, `Task<T>`, `Parallel`, `CancellationToken` |
| `System.Diagnostics` | Diagnóstico | `Stopwatch`, `Process`, `ActivitySource`, `Debug` |
| `System.Reflection` | Introspección | `Type`, `Assembly`, `MethodInfo`, `PropertyInfo` |
| `System.Security.Cryptography` | Cifrado | `SHA256`, `Aes`, `RSA`, `RandomNumberGenerator` |

### 2.6 NativeAOT

Tradicionalmente, .NET usa compilación JIT. Pero desde .NET 7, existe **NativeAOT** (Ahead-Of-Time), maduro en .NET 8:

```
JIT:      C# → IL → [en ejecución] JIT → código máquina
NativeAOT: C# → IL → [en build] AOT → código máquina nativo
```

| | JIT | NativeAOT |
|---|---|---|
| **Arranque** | Lento (~300ms) | Instantáneo (~50ms) |
| **Memoria** | Más (JIT + IL en memoria) | Menos (solo código nativo) |
| **Reflection** | Completa | Limitada (solo anotaciones) |
| **Tamaño binario** | ~70 MB (runtime + app) | ~10-20 MB (trimmed) |
| **Optimizaciones** | Específicas de la CPU | Genéricas |
| **Uso ideal** | Servidores, APIs | CLI tools, lambdas, contenedores |

---

## 3. Ejemplo mínimo

### Inspeccionando IL

```bash
# Instalar la herramienta para ver IL (solo una vez)
dotnet tool install -g dotnet-ildasm

# Compilar y ver el IL generado
dotnet build
dotnet ildasm bin/Debug/net8.0/CompilacionDotNet.dll
```

### Ver la información del GC

```csharp
// Información de la última recolección del GC
GCMemoryInfo info = GC.GetGCMemoryInfo();
Console.WriteLine($"Generación máxima: {GC.MaxGeneration}"); // 2
Console.WriteLine($"Memoria del heap: {info.HeapSizeBytes} bytes");
Console.WriteLine($"Memoria total: {info.TotalAvailableMemoryBytes} bytes");
```

---

## 4. Ejemplo real

El proyecto `CompilacionDotNet` demuestra:

1. **Pipeline de compilación:** Inspección del assembly con Reflection, estructura de un .dll.
2. **IL stack-based:** Ejemplo de suma que muestra cómo IL usa la pila virtual.
3. **JIT y Tiered Compilation:** Benchmark de 1M de iteraciones para ver cuándo optimiza.
4. **Garbage Collector:** Monitoreo de memoria antes/después del GC, generaciones, LOH.
5. **BCL namespaces:** Tabla con los namespaces más importantes.
6. **Comparativa de runtimes:** CLR vs JVM vs V8.
7. **NativeAOT:** Explicación y comando para probarlo.

```bash
dotnet run --project ejemplos/m00-preparacion/Leccion-02-CLR-BCL-Compilacion/src/CompilacionDotNet
```

---

## 5. Buenas prácticas

### ✅ Hacer

| Práctica | Por qué |
|---|---|
| Entender las generaciones del GC | Para escribir código que minimice la presión en Gen 2 |
| Usar `struct` para objetos pequeños y efímeros | Viven en el stack, no presionan el GC |
| Usar `ArrayPool<T>` para buffers temporales | Reutiliza arrays, evita asignaciones en LOH |
| Medir con `GC.GetTotalMemory()` en tests de rendimiento | Para verificar que no hay memory leaks |
| Compilar en Release para benchmarks | El JIT aplica optimizaciones completas |
| Preferir `stackalloc` y `Span<T>` para datos temporales | Evita asignaciones en el heap |

### ❌ Evitar

| Práctica | Por qué |
|---|---|
| `GC.Collect()` | Interrumpe el algoritmo generacional, degrada el rendimiento |
| Concatenar strings en bucles con `+` | Cada concatenación crea un nuevo string → presión en Gen 0. Usa `StringBuilder` |
| `new HttpClient()` por request | Agota los sockets (que son objetos de SO, no de .NET) |
| Objetos grandes frecuentes (>85KB) | Van al LOH, que no se compacta → fragmentación |
| Capturar variables locales en closures de larga duración | Extiende la vida de objetos que deberían morir en Gen 0 |
| Ignorar el tiered compilation en benchmarks | Las primeras ejecuciones son más lentas (Tier 0) |

---

## 6. Errores habituales

### Error 1: Pensar que `struct` siempre es más rápido

```csharp
// ❌ INCORRECTO: struct enorme (>16 bytes) pasado por valor
struct DatosGrandes
{
    public decimal Precio;
    public DateTime Fecha;
    public string Nombre; // Referencia dentro del struct
    public string Descripcion;
}

void Procesar(DatosGrandes d) { /* ... */ }
// Cada llamada a Procesar COPIA ~64 bytes al stack
```

```csharp
// ✅ CORRECTO: class para objetos grandes, o pasar struct por referencia
class DatosGrandes { /* ... */ }

// O si usas struct grande, pásalo con `in` o `ref readonly`
void Procesar(in DatosGrandes d) { /* ... */ }
```

### Error 2: `GC.Collect()` para "ayudar" al GC

```csharp
// ❌ INCORRECTO: forzar recolección manual
GC.Collect();  // Pausa TODOS los hilos, recolecta Gen 0, 1 y 2
GC.WaitForPendingFinalizers(); // Peor aún: ejecuta destructores
// Esto puede causar pausas de 100ms+ destruyendo la latencia de tu API
```

```csharp
// ✅ CORRECTO: dejar que el GC haga su trabajo
// El GC está optimizado por ingenieros con décadas de experiencia.
// Si tienes problemas de memoria, usa:
// - dotnet-counters para monitorear (dotnet-counters monitor)
// - dotnet-gcdump para analizar el heap (dotnet-gcdump collect)
// - BenchmarkDotNet para medir asignaciones
```

### Error 3: No entender el LOH

```csharp
// ❌ INCORRECTO: arrays grandes en bucles
for (int i = 0; i < 1000; i++)
{
    byte[] buffer = new byte[100_000]; // 100KB → LOH
    Procesar(buffer);
    // Cada iteración asigna 100KB en el LOH. Puede fragmentarlo.
}

// ✅ CORRECTO: ArrayPool<T> reutiliza buffers
byte[] buffer = ArrayPool<byte>.Shared.Rent(100_000);
try
{
    for (int i = 0; i < 1000; i++)
    {
        Procesar(buffer);
    }
}
finally
{
    ArrayPool<byte>.Shared.Return(buffer);
}
```

---

## 7. Ejercicio guiado

### Ejercicio: Explorador del GC

Crea un programa que:

1. Muestre la memoria inicial del proceso.
2. Cree 1 millón de objetos temporales (simula una operación intensiva).
3. Mida y muestre cuánta memoria se usó.
4. Fuerce una recolección (solo para demostración) y muestre la memoria liberada.
5. Compare el tiempo de ejecución de `class` vs `struct` para una operación simple.

**Solución:**

```csharp
// 1. Memoria inicial
long memInicial = GC.GetTotalMemory(forceFullCollection: true);
Console.WriteLine($"Memoria inicial: {memInicial / 1024} KB");

// 2. Crear objetos temporales
for (int i = 0; i < 1_000_000; i++)
{
    string temp = $"objeto-{i}";
}
long memDespuesCrear = GC.GetTotalMemory(forceFullCollection: false);
Console.WriteLine($"Después de crear objetos: {memDespuesCrear / 1024} KB");

// 3. GC
GC.Collect();
GC.WaitForPendingFinalizers();
long memDespuesGC = GC.GetTotalMemory(forceFullCollection: true);
Console.WriteLine($"Después del GC: {memDespuesGC / 1024} KB");

// 4. Comparativa class vs struct
var sw = Stopwatch.StartNew();
for (int i = 0; i < 10_000_000; i++)
{
    var p = new PuntoStruct { X = i, Y = i }; // En el stack
}
sw.Stop();
Console.WriteLine($"struct (stack): {sw.ElapsedMilliseconds} ms");

sw.Restart();
for (int i = 0; i < 10_000_000; i++)
{
    var p = new PuntoClass { X = i, Y = i }; // En el heap
}
sw.Stop();
Console.WriteLine($"class (heap): {sw.ElapsedMilliseconds} ms");

// Tipos usados
struct PuntoStruct { public int X; public int Y; }
class PuntoClass { public int X; public int Y; }
```

---

## 8. Reto

**Reto: Memory Leak Detector**

Investiga y escribe un programa que demuestre un **memory leak** en .NET (sí, aunque tiene GC, se pueden producir leaks).

Pistas:
- Event handlers no desuscritos (el publicador mantiene viva la referencia al suscriptor)
- `Timer` que no se disposea
- Colecciones estáticas que crecen indefinidamente
- `WeakReference<T>` es la solución para referencias que no impidan la recolección

```csharp
// Ejemplo de "leak": event handler no desuscrito
public class Publicador
{
    public event EventHandler? OnData;
}

public class Suscriptor
{
    private readonly byte[] _buffer = new byte[1_000_000]; // 1MB
    
    public void Suscribir(Publicador pub)
    {
        pub.OnData += (_, _) => Console.WriteLine("Data!");
        // Si Suscriptor se va del scope pero Publicador sigue vivo,
        // el Suscriptor NUNCA será recolectado (porque el delegate lo referencia)
    }
}
```

> **Pista:** Usa `dotnet-counters` para monitorear el GC Heap Size mientras ejecutas el programa.

---

## 9. Resumen

- **CLR (Common Language Runtime):** El motor de ejecución de .NET. Carga assemblies, compila IL a código máquina (JIT), gestiona la memoria (GC), maneja excepciones e hilos.
- **BCL (Base Class Library):** Más de 10 000 clases listas para usar. Es la razón por la que no necesitas 500 paquetes npm para hacer una API.
- **Pipeline C# → IL → JIT → Máquina:**
  1. Roslyn compila C# → IL (en build)
  2. CLR carga el assembly
  3. JIT compila IL → código nativo (en ejecución, bajo demanda)
  4. CPU ejecuta el código nativo
- **IL es stack-based**, como el bytecode de Java. Cada operación empuja/saca del stack virtual.
- **Tiered Compilation:** Tier 0 (rápido) → Tier 1 (optimizado) automáticamente según frecuencia de uso.
- **GC generacional:** Gen 0 (objetos nuevos, recolección frecuente), Gen 1 (buffer), Gen 2 (longevos, recolección rara), LOH (objetos >85KB, no compactado).
- **NativeAOT:** Compila C# directamente a código máquina en build. Arranque instantáneo, pero sin reflection dinámica.

**Conceptos clave:** IL (MSIL/CIL), JIT (RyuJIT), GC (generacional), Tiered Compilation, NativeAOT, stack-based VM, LOH.

---

## 10. Preguntas frecuentes

### ¿.NET es interpretado como Python?

**No.** .NET usa compilación JIT (Just-In-Time): el IL se compila a código máquina nativo antes de ejecutarse. Una vez compilado, se ejecuta a velocidad nativa. Python es interpretado (el intérprete lee y ejecuta línea por línea), lo cual es mucho más lento para CPU-bound.

### ¿Por qué .NET no compila todo AOT (ahead-of-time) como Go o Rust?

Porque el JIT puede optimizar para la CPU específica donde se ejecuta (instrucciones AVX2, SSE4, etc.) y porque permite reflection dinámica (inspeccionar y modificar tipos en runtime). Dicho esto, .NET 8 ya tiene NativeAOT maduro para cuando necesitas AOT.

### ¿El GC pausa mi aplicación?

Sí, pero muy poco. Las recolecciones de Gen 0 duran ~0.1-1ms. Las de Gen 2 pueden durar ~10-100ms. En servidores web de alta concurrencia, el GC de .NET está entre los mejores del mundo (comparable al ZGC de Java). Además, .NET 8 introdujo mejoras para reducir las pausas.

### ¿Puedo ver el IL que genera mi código?

Sí. Instala `dotnet-ildasm` y ejecútalo sobre tu .dll. También puedes usar sharplab.io para ver IL online sin instalar nada.

### ¿Qué diferencia hay entre `stack` y `heap`?

- **Stack:** Memoria de acceso rápido (LIFO). Cada hilo tiene su stack. Almacena variables locales, parámetros y `struct`. Se libera automáticamente al salir del método. Tamaño limitado (~1MB por hilo).
- **Heap:** Memoria dinámica gestionada por el GC. Almacena objetos `class`, arrays, strings. Sin límite fijo (hasta que el SO diga basta). Más lento de acceder y de liberar.

---

**Siguiente lección:** dotnet CLI, estructura de proyecto y VS Code
