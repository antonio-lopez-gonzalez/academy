// ──────────────────────────────────────────────────────────────────────────
// Lección 02 — CLR, BCL y cómo funciona la compilación
// ──────────────────────────────────────────────────────────────────────────
// Este programa explica el viaje que hace tu código C# desde el editor
// hasta que se ejecuta como instrucciones máquina en el procesador.
// Entender este pipeline es FUNDAMENTAL para depurar problemas de
// rendimiento, entender por qué el GC se comporta como lo hace, y
// sacar el máximo partido a .NET.
// ──────────────────────────────────────────────────────────────────────────

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

// ══════════════════════════════════════════════════════════════════════════
// PARTE 1: EL PIPELINE DE COMPILACIÓN
// ══════════════════════════════════════════════════════════════════════════
//
// El viaje de tu código C# al procesador:
//
//   C# ──→ IL ──→ JIT ──→ Código Máquina ──→ Ejecución
//   (tu)   (Roslyn)  (CLR)   (nativo)       (CPU)
//
// 1. Roslyn (compilador C#) convierte C# → IL (Intermediate Language)
//    El IL se guarda en el .dll/.exe como bytecode.
//
// 2. El CLR carga el assembly y el JIT (Just-In-Time compiler)
//    convierte IL → código máquina nativo.
//
// 3. La CPU ejecuta el código máquina.
//
// Este diseño de 2 pasos permite:
//   - Escribir una vez, ejecutar en cualquier SO (Windows/Linux/macOS)
//   - Optimizaciones específicas para la CPU en tiempo de ejecución
//   - Tiered Compilation: compilar rápido primero, optimizar después
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("1. EL PIPELINE C# → IL → JIT → MÁQUINA");

// Cada assembly .NET contiene metadata + IL.
// Podemos inspeccionar el assembly actual con Reflection.
Assembly esteAssembly = Assembly.GetExecutingAssembly();
Console.WriteLine($"Assembly en ejecución : {esteAssembly.GetName().Name}");
Console.WriteLine($"Versión               : {esteAssembly.GetName().Version}");

// Los assemblies .NET son archivos .dll o .exe que contienen:
// ┌──────────────────────────────────────┐
// │ PE Header (Portable Executable)      │ ← Igual que un .exe de Windows
// │ CLR Header                           │ ← Dice "esto es .NET, no nativo"
// │ Metadata (tablas de tipos, métodos)   │ ← Reflection lee esto
// │ IL Code (el bytecode)                │ ← Lo que el JIT compila
// │ Resources (strings, imágenes, etc.)   │
// └──────────────────────────────────────┘

// ══════════════════════════════════════════════════════════════════════════
// PARTE 2: INSPECCIONANDO IL (Intermediate Language)
// ══════════════════════════════════════════════════════════════════════════
//
// Para ver el IL generado, usa: dotnet tool install -g dotnet-ildasm
// Luego: dotnet ildasm CompilacionDotNet.dll
//
// O desde Visual Studio: Debug → Windows → Disassembly
//
// Ejemplo de IL (versión simplificada de lo que genera esta línea):
//   Console.WriteLine("Hola");
//
// IL generado (aproximado):
//   ldstr      "Hola"           // Carga el string "Hola" en el stack
//   call       void [System.Console]System.Console::WriteLine(string)
//   ret                          // Retorna
//
// IL es un lenguaje de pila (stack-based), como el bytecode de Java.
// Las operaciones empujan (push) y sacan (pop) valores del stack de evaluación.
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("2. DEMOSTRACIÓN DE IL (STACK-BASED)");

// Esta función aparentemente simple...
int resultado = Sumar(3, 5);
Console.WriteLine($"3 + 5 = {resultado}");

// Genera IL como:
//   ldc.i4.3     // Push 3 al stack
//   ldc.i4.5     // Push 5 al stack
//   call Sumar   // Pop 5 y 3, ejecuta Sumar, push resultado
//   // ... usar el resultado

// ══════════════════════════════════════════════════════════════════════════
// PARTE 3: JIT COMPILATION (Just-In-Time)
// ══════════════════════════════════════════════════════════════════════════
//
// El JIT (RyuJIT en .NET Core+) convierte IL a código máquina.
// Características clave:
//
// 1. COMPILACIÓN BAJO DEMANDA:
//    Solo compila los métodos que realmente se llaman.
//    Métodos no usados = nunca se compilan = ahorran memoria.
//
// 2. TIERED COMPILATION (.NET Core 3+):
//    Tier 0: compilación rápida, poca optimización (para arranque veloz)
//    Tier 1: compilación optimizada (cuando el método se llama muchas veces)
//    El CLR decide automáticamente cuándo recompilar en Tier 1.
//
// 3. INLINING:
//    El JIT inserta el cuerpo de métodos pequeños directamente en el caller,
//    eliminando el coste de la llamada.
//
// 4. DEVIRTUALIZATION:
//    Si el JIT sabe el tipo exacto de un objeto, elimina la llamada virtual.
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("3. JIT COMPILATION Y OPTIMIZACIONES");

// Vamos a demostrar los niveles de optimización con un benchmark simple.
// Ejecutamos una operación muchas veces para que el JIT la optimice.
const int Iteraciones = 1_000_000;

Stopwatch sw = Stopwatch.StartNew();
double acumulador = 0;

// Primera ejecución: Tier 0 (rápido, poca optimización)
for (int i = 0; i < Iteraciones; i++)
{
    acumulador += InteresCompuesto(100, 0.05, 10);
}

sw.Stop();
Console.WriteLine($"Resultado            : {acumulador}");
Console.WriteLine($"Tiempo (1M iter)     : {sw.ElapsedMilliseconds} ms");
Console.WriteLine($"Modo de compilación  : {(Debugger.IsAttached ? "DEBUG (sin optimizar)" : "RELEASE (optimizado)")}");

// En Release, el JIT hace:
//   - Inlining de métodos cortos
//   - Eliminación de bounds checking en arrays (cuando puede probar que es seguro)
//   - Loop unrolling
//   - Constant folding (evalúa expresiones constantes en compilación)

// ══════════════════════════════════════════════════════════════════════════
// PARTE 4: EL GARBAGE COLLECTOR (GC)
// ══════════════════════════════════════════════════════════════════════════
//
// El GC gestiona automáticamente la memoria del heap.
// NO tienes que llamar a free()/delete como en C/C++.
//
// Funcionamiento:
//
// 1. GENERACIONES (para optimizar):
//
//    ┌──────────┐
//    │  Gen 0   │ ← Objetos nuevos (< 1 KB). Se recolecta MUY seguido.
//    │  (corta) │   Colección rápida (~1ms). La mayoría de objetos mueren aquí.
//    ├──────────┤
//    │  Gen 1   │ ← Objetos que sobrevivieron a Gen 0.
//    │  (media) │   Buffer entre Gen 0 y Gen 2.
//    ├──────────┤
//    │  Gen 2   │ ← Objetos longevos (singletons, cachés, estáticos).
//    │  (larga) │   Se recolecta MUY poco. Colección lenta (~10-100ms).
//    └──────────┘
//
//    ┌────────────────┐
//    │  LOH (Large    │ ← Objetos >= 85 000 bytes (arrays grandes, strings).
//    │  Object Heap)  │   No se compacta por defecto (costoso mover GBs).
//    └────────────────┘
//
// 2. FASES DE UNA COLECCIÓN:
//    Mark  → Marca los objetos vivos (recorriendo las raíces: stack, statics)
//    Sweep → Libera la memoria de objetos no marcados
//    Compact → (Opcional) Compacta el heap, moviendo objetos para evitar fragmentación
//
// 3. CUÁNDO SE DISPARA:
//    - Gen 0 se llena (lo más común)
//    - Gen 2 (se llama manualmente, casi nunca necesario)
//    - Sistema con poca memoria (low memory notification)
//    - GC.Collect() (NO usar en producción)
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("4. GARBAGE COLLECTOR — GENERACIONES Y MEMORIA");

// GC.GetGCMemoryInfo() nos da información sobre la última recolección.
GCMemoryInfo gcInfo = GC.GetGCMemoryInfo();

Console.WriteLine($"Memoria total heap         : {gcInfo.TotalAvailableMemoryBytes / 1024 / 1024} MB");
Console.WriteLine($"Memoria usada antes del GC : {gcInfo.HeapSizeBytes / 1024} KB");
Console.WriteLine($"Generación más alta        : {GC.MaxGeneration} (0=Gen0, 1=Gen1, 2=Gen2)");

// Podemos ver cuánta memoria estamos usando actualmente
long memoriaAntes = GC.GetTotalMemory(forceFullCollection: false);
Console.WriteLine($"\nMemoria actual del proceso : {memoriaAntes / 1024} KB");

// Demostración: crear objetos temporales → mueren en Gen0
CrearObjetosTemporales();

long memoriaDespues = GC.GetTotalMemory(forceFullCollection: true);
Console.WriteLine($"Memoria después del GC     : {memoriaDespues / 1024} KB");

// El GC en .NET 8 incluye:
// - Non-concurrent GC en algunos escenarios (más rápido para cargas bajas)
// - DATAS (Dynamic Adaptation To Application Sizes): ajusta el heap dinámicamente
// - Regions: nueva organización del heap para servidores multi-core (en preview)

// ══════════════════════════════════════════════════════════════════════════
// PARTE 5: LA BCL (Base Class Library) — NAMESPACES PRINCIPALES
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("5. BCL — NAMESPACES ESENCIALES");

Console.WriteLine("""
    ┌───────────────────────┬──────────────────────────────────────┐
    │ Namespace             │ Propósito                            │
    ├───────────────────────┼──────────────────────────────────────┤
    │ System                │ Tipos fundamentales (Object, Int32,  │
    │                       │ String, DateTime, Console, Math...)  │
    │ System.Collections    │ List<T>, Dictionary<K,V>, HashSet<T> │
    │ System.Linq           │ Where, Select, GroupBy, Join...      │
    │ System.IO             │ File, Directory, Stream, Path...     │
    │ System.Net.Http       │ HttpClient, HttpRequestMessage...    │
    │ System.Text.Json      │ JsonSerializer, JsonDocument...      │
    │ System.Threading      │ Thread, Mutex, Semaphore...          │
    │ System.Threading.Tasks│ Task, Task<T>, Parallel...           │
    │ System.Diagnostics    │ Stopwatch, Process, ActivitySource...│
    │ System.Reflection     │ Type, Assembly, MethodInfo...        │
    │ System.Globalization  │ CultureInfo, DateTimeFormat...       │
    │ System.Security       │ Cryptography, Authentication...      │
    └───────────────────────┴──────────────────────────────────────┘
    """);

// ══════════════════════════════════════════════════════════════════════════
// PARTE 6: COMPARACIÓN CLR vs JVM vs V8
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("6. COMPARATIVA DE RUNTIMES");

Console.WriteLine("""
    ┌──────────────┬─────────────┬──────────────┬──────────────┐
    │              │ CLR (.NET)  │ JVM (Java)   │ V8 (Node.js) │
    ├──────────────┼─────────────┼──────────────┼──────────────┤
    │ Bytecode     │ CIL (MSIL)  │ Java Bytecode│ JS source    │
    │ Compilación  │ JIT + AOT   │ JIT + AOT    │ Solo JIT     │
    │ GC           │ Generacional│ G1/ZGC/Shen  │ Orinoco      │
    │ Value types  │ Sí (struct) │ No (en dev)  │ No           │
    │ Genéricos    │ Reificados  │ Borrado      │ Sin tipos    │
    │ Punteros     │ unsafe (opt)│ No           │ No           │
    │ Interop      │ P/Invoke    │ JNI          │ N-API        │
    │ Hot Reload   │ Sí (.NET 6+)| Limitado     │ Sí (nativo)  │
    └──────────────┴─────────────┴──────────────┴──────────────┘

    DIFERENCIA CLAVE — Value Types (tipos de valor):
    En .NET, `struct` vive en el stack (o inline en el objeto padre).
    En Java, TODO es referencia al heap (salvo primitivos).
    Esto significa que .NET puede crear arrays de structs contiguos
    en memoria, lo cual es MUCHO más eficiente para la caché de CPU.
    """);

// ══════════════════════════════════════════════════════════════════════════
// PARTE 7: NATIVEAOT — EL FUTURO DE LA COMPILACIÓN
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("7. NATIVEAOT (AHEAD-OF-TIME COMPILATION)");

// Tradicionalmente .NET usa JIT:
//   IL → JIT en tiempo de ejecución → código máquina
//
// Con NativeAOT (maduro en .NET 8):
//   IL → compilación AOT en build → ejecutable nativo (sin JIT ni IL)
//
// Ventajas de NativeAOT:
//   - Arranque instantáneo (sin esperar al JIT)
//   - Menor uso de memoria (sin JIT compiler ni metadata de IL)
//   - Ejecutable autocontenido (sin instalar .NET runtime)
//
// Desventajas:
//   - Sin reflection dinámica (Assembly.Load, etc.)
//   - Compilación más lenta
//   - Tamaño del binario mayor
//
// Para probar NativeAOT:
//   dotnet publish -c Release -r win-x64 /p:PublishAot=true

Console.WriteLine("NativeAOT está disponible desde .NET 7 y maduro en .NET 8.");
Console.WriteLine("Lo exploraremos en detalle en el Módulo 17 (Temas avanzados).");

// ══════════════════════════════════════════════════════════════════════════
// FIN DEL PROGRAMA
// ══════════════════════════════════════════════════════════════════════════

Console.WriteLine();
Console.WriteLine("✅ Fin de la Lección 02.");
Console.WriteLine("   Para ver el IL generado, ejecuta:");
Console.WriteLine("   dotnet tool install -g dotnet-ildasm");
Console.WriteLine("   dotnet ildasm bin/Debug/net8.0/CompilacionDotNet.dll");

// ──────────────────────────────────────────────────────────────────────────
// Métodos auxiliares y de demostración
// ──────────────────────────────────────────────────────────────────────────

/// <summary>
/// Calcula el interés compuesto: capital * (1 + tasa)^periodos.
/// Método intencionadamente sencillo para demostrar inlining del JIT.
/// </summary>
/// <remarks>
/// En Release, el JIT probablemente hará inline de este método,
/// eliminando la sobrecarga de la llamada (call → cuerpo directo).
/// Además, como los argumentos son constantes en el bucle,
/// puede aplicar constant folding y evaluar todo en compilación.
/// </remarks>
[MethodImpl(MethodImplOptions.AggressiveInlining)] // Sugerencia al JIT (no garantía)
static double InteresCompuesto(double capital, double tasa, int periodos)
{
    // Math.Pow usa algoritmos optimizados de la BCL
    return capital * Math.Pow(1 + tasa, periodos);
}

/// <summary>
/// Método tradicional (no top-level) para demostrar el IL generado.
/// </summary>
static int Sumar(int a, int b)
{
    return a + b;
}

/// <summary>
/// Crea objetos temporales que serán recolectados por el GC en Gen0.
/// Demostración de presión de memoria y recolección generacional.
/// </summary>
static void CrearObjetosTemporales()
{
    // Creamos muchos objetos que morirán al salir de este método.
    // Estos objetos NUNCA llegarán a Gen1 o Gen2 porque:
    // 1. Son locales a este método (no hay referencias externas)
    // 2. El GC de Gen0 es muy frecuente y los recoge rápido
    for (int i = 0; i < 100_000; i++)
    {
        // Cada iteración crea un string → presión en Gen0
        // El GC puede dispararse durante este bucle si Gen0 se llena
        string temp = $"Objeto temporal {i}";
        _ = temp.Length; // Usamos el string para que no lo elimine el compilador
    }
}

/// <summary>
/// Imprime una sección con formato visual consistente.
/// </summary>
static void ImprimirSeccion(string titulo)
{
    Console.WriteLine();
    Console.WriteLine(new string('═', 65));
    Console.WriteLine($"  {titulo}");
    Console.WriteLine(new string('═', 65));
}
