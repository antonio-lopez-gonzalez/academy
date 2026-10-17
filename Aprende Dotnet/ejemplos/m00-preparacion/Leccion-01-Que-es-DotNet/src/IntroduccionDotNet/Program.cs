// ──────────────────────────────────────────────────────────────────────────
// Lección 01 — ¿Qué es .NET? Historia y ecosistema
// ──────────────────────────────────────────────────────────────────────────
// Este programa demuestra los conceptos fundamentales de .NET: qué es,
// su historia, y cómo se compara con otros ecosistemas como Java y Node.js.
// Cada sección está explicada línea por línea para entender el "por qué".
// ──────────────────────────────────────────────────────────────────────────

// ────────────────────────────────── Sección 1 ─────────────────────────────
// Top-level statements: desde C# 10, no necesitas escribir `Main()`
// ni una clase `Program` explícita. El compilador genera una clase
// interna `Program` con un método `Main` automáticamente.
// Esto reduce el "boilerplate" y hace C# más amigable para principiantes.
// ──────────────────────────────────────────────────────────────────────────

// System.Runtime.InteropServices.RuntimeInformation nos dice en qué
// sistema operativo se está ejecutando nuestro código. Esto demuestra
// que .NET es verdaderamente multiplataforma: el mismo binario puede
// correr en Windows, Linux y macOS sin modificaciones.
using System.Runtime.InteropServices;

// ────────────────────────────────── Sección 2 ─────────────────────────────
// Mostramos la versión del runtime y el sistema operativo.
// `Environment.Version` devuelve la versión del CLR en ejecución.
// Esto es diferente de la versión del SDK: el SDK compila, el runtime ejecuta.
// ──────────────────────────────────────────────────────────────────────────
ImprimirSeparador("1. INFORMACIÓN DEL ENTORNO .NET");

string frameworkVersion = Environment.Version.ToString();
string osDescription = RuntimeInformation.OSDescription;
string architecture = RuntimeInformation.OSArchitecture.ToString();
string processArch = RuntimeInformation.ProcessArchitecture.ToString();
bool esWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
bool esLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
bool esMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

// $ — string interpolation: más limpio y eficiente que concatenar con +
Console.WriteLine($"CLR Runtime versión : {frameworkVersion}");
Console.WriteLine($"Sistema Operativo   : {osDescription}");
Console.WriteLine($"Arquitectura SO     : {architecture}");
Console.WriteLine($"Arquitectura proceso: {processArch}");
Console.WriteLine($"¿Windows?           : {esWindows}");
Console.WriteLine($"¿Linux?             : {esLinux}");
Console.WriteLine($"¿macOS?             : {esMac}");

// ────────────────────────────────── Sección 3 ─────────────────────────────
// La BCL (Base Class Library) contiene miles de clases listas para usar:
// manejo de strings, colecciones, fechas, archivos, red, etc.
// Esto es como la JDK en Java o la stdlib en Python, pero unificada.
// ──────────────────────────────────────────────────────────────────────────
ImprimirSeparador("2. DEMOSTRACIÓN DE LA BCL (Base Class Library)");

// `ReadOnlySpan<char>`: una vista de solo lectura sobre un string.
// A diferencia de `string.Substring()`, NO crea una copia en memoria.
// Es parte de las optimizaciones de alto rendimiento de .NET moderno.
string frase = "Aprendiendo .NET 8 con C#";
ReadOnlySpan<char> subFrase = frase.AsSpan(0, 11); // "Aprendiendo"
Console.WriteLine($"Frase original (stack) : {frase}");
Console.WriteLine($"Sub-frase sin copia    : {subFrase.ToString()}");

// DateTime con TimeProvider (nuevo en .NET 8): abstrae el concepto de "ahora".
// Antes usábamos DateTime.UtcNow o DateTime.Now, que no se pueden mockear en tests.
// TimeProvider permite controlar el tiempo en tests sin Thread.Sleep().
TimeProvider timeProvider = TimeProvider.System;
DateTimeOffset ahora = timeProvider.GetUtcNow();
Console.WriteLine($"Hora UTC (vía TimeProvider): {ahora:yyyy-MM-dd HH:mm:ss}");

// `Path.Combine` — une rutas de forma segura, sin preocuparse por
// las barras \ de Windows o / de Linux. La BCL abstrae el SO.
string ruta = Path.Combine("home", "usuario", "documentos", "net8.md");
Console.WriteLine($"Ruta multiplataforma: {ruta}");

// `Guid.NewGuid()` — genera un identificador único global.
// Garantiza unicidad sin depender de una secuencia de base de datos.
// Muy usado en sistemas distribuidos y como claves primarias.
Guid idUnico = Guid.NewGuid();
Console.WriteLine($"GUID único global: {idUnico}");

// ────────────────────────────────── Sección 4 ─────────────────────────────
// LINQ (Language Integrated Query): consultas integradas en el lenguaje.
// No necesitas un ORM ni escribir SQL — LINQ funciona sobre cualquier
// colección en memoria, base de datos, XML, JSON, etc.
// Comparable a Streams de Java o list comprehensions de Python.
// ──────────────────────────────────────────────────────────────────────────
ImprimirSeparador("3. LINQ — Consultas integradas en el lenguaje");

string[] tecnologias = [
    "C#",
    "F#",
    "ASP.NET Core",
    "Entity Framework Core",
    "Blazor",
    "MAUI",
    "ML.NET",
    "SignalR"
];

// Query syntax: estilo SQL dentro de C# (azúcar sintáctico).
// El compilador lo traduce a llamadas de método (method syntax).
Console.WriteLine("--- Estilo SQL (query syntax) ---");
var tecnologiasConCore = from t in tecnologias
                         where t.Contains("Core")
                         orderby t
                         select t.ToUpper();

foreach (string tech in tecnologiasConCore)
{
    Console.WriteLine($"  → {tech}");
}

// Method syntax: más idiomático en código profesional.
// Encadena métodos como Where(), Select(), OrderBy().
// Cada método devuelve IEnumerable<T>, habilitando "fluent chaining".
Console.WriteLine("\n--- Estilo método (method syntax) ---");
var estadisticas = tecnologias
    .Where(t => t.Length > 3)           // Filtra nombres largos
    .OrderByDescending(t => t.Length)   // Ordena por longitud
    .Select(t => $"[{t.Length}] {t}");  // Proyecta con formato

foreach (string stat in estadisticas)
{
    Console.WriteLine($"  → {stat}");
}

// Expresiones lambda (`t => t.Length > 3`): funciones anónimas inline.
// Equivalente a arrow functions de JS o lambdas de Java y Python.
// En C# se compilan a delegados (`Func<>`, `Action<>`) o árboles
// de expresión (`Expression<>`), según el contexto.

// ────────────────────────────────── Sección 5 ─────────────────────────────
// Comparación con Java y JavaScript (comentado, no se ejecuta)
// ──────────────────────────────────────────────────────────────────────────
ImprimirSeparador("4. COMPARATIVA CONCEPTUAL CON OTROS ECOSISTEMAS");

// ┌─────────────────┬──────────────────┬──────────────────┐
// │ .NET / C#       │ Java             │ JavaScript/TS    │
// ├─────────────────┼──────────────────┼──────────────────┤
// │ CLR             │ JVM              │ V8 / Node.js     │
// │ IL (MSIL)       │ Bytecode         │ Directo JIT      │
// │ System.* (BCL)  │ JDK (java.*)     │ stdlib / npm     │
// │ NuGet           │ Maven / Gradle   │ npm / yarn       │
// │ C# 12 (.NET 8)  │ Java 21          │ ES2023 / TS 5    │
// │ Garbage Collector│ G1 / ZGC / Shen │ V8 GC (Orinoco)  │
// │ JIT + AOT (R2R) │ JIT + AOT (Graal)│ Solo JIT         │
// │ Value types     │ Solo referencia  │ Solo referencia  │
// │ (stack)         │ (Proyecto Valh.) │                  │
// └─────────────────┴──────────────────┴──────────────────┘

// Puntos clave de .NET frente a Java:
//   - Tipos de valor (struct) en el stack → menos presión en el GC
//   - Propiedades (get/set) integradas en la sintaxis del lenguaje
//   - LINQ: consultas como ciudadano de primera clase
//   - async/await nativo desde 2012 (Java lo añadió con Project Loom en 2023)
//   - Delegados y eventos (Java usa interfaces funcionales y listeners)

// Puntos clave de .NET frente a Node.js:
//   - Tipado estático y fuerte en todo el stack (no solo TypeScript)
//   - Un solo runtime unificado (no npm + webpack + babel + tsconfig...)
//   - Multi-hilo real con `Task` (Node.js es single-thread con event loop)
//   - GC generacional optimizado para servidores de larga duración
//   - Mejor rendimiento en CPU-bound: C# compilado vs JS interpretado/JIT

Console.WriteLine("Consulta la tabla comparativa en el README.md de esta lección.");
Console.WriteLine("Allí se explican en detalle las diferencias de diseño.");

// ────────────────────────────────── Sección 6 ─────────────────────────────
// Tipado estático y fuerte: C# no te deja mezclar tipos sin conversión
// explícita. Esto evita bugs en tiempo de compilación, no en producción.
// ──────────────────────────────────────────────────────────────────────────
ImprimirSeparador("5. DEMOSTRACIÓN DE TIPADO ESTÁTICO");

int entero = 42;
string texto = "Hola";

// Esto NO compilaría: int suma = entero + texto;
// Error CS0019: Operator '+' cannot be applied to operands of type 'int' and 'string'

// En JavaScript esto sí se ejecutaría: 42 + "Hola" → "42Hola"
// Y podría causar bugs difíciles de detectar.

// La conversión debe ser explícita:
string concatenadoCorrecto = entero.ToString() + " " + texto;
Console.WriteLine($"Concatenación explícita : {concatenadoCorrecto}");

// `var`: inferencia de tipos en tiempo de compilación.
// El tipo se determina en compilación, NO en ejecución.
// No es como `var` de JavaScript (que es dinámico) ni `var` de Java 10 (que sí es similar).
var numeroInferido = 100;       // El compilador sabe que es `int`
var textoInferido = ".NET 8";   // El compilador sabe que es `string`
// numeroInferido = "texto";     // Error de compilación: no puede cambiar de tipo
Console.WriteLine($"Tipo inferido de '100'       : {numeroInferido.GetType()}");
Console.WriteLine($"Tipo inferido de '\".NET 8\"' : {textoInferido.GetType()}");

// ────────────────────────────────── Sección 7 ─────────────────────────────
// Nullable Reference Types (NRT): activados por defecto en .NET 8.
// El compilador te AVISA si puedes tener un null no esperado.
// Esto reduce drásticamente la NullReferenceException, el error más
// común en .NET Framework clásico y en Java.
// ──────────────────────────────────────────────────────────────────────────
ImprimirSeparador("6. NULLABLE REFERENCE TYPES (SEGURIDAD ANTE NULL)");

// `string` (sin ?) → el compilador asume que NO debe ser null
string noNulleable = "Siempre tengo valor";
Console.WriteLine($"string no nulleable    : {noNulleable}");
// noNulleable = null;  // ⚠️ Warning CS8600: Converting null literal to non-nullable type

// `string?` → declaras explícitamente que PUEDE ser null
string? nulleable = null; // Correcto, el compilador sabe que es intencional

// Operador null-conditional (?.) y null-coalescing (??)
// `?.`  : accede al miembro SOLO si el objeto no es null
// `??`  : proporciona un valor por defecto si la expresión es null
int? longitud = nulleable?.Length;       // null si nulleable es null
int longitudSegura = nulleable?.Length ?? 0; // 0 si nulleable es null

Console.WriteLine($"string? nulleable = null");
Console.WriteLine($"  nulleable?.Length   → {longitud?.ToString() ?? "null"}");
Console.WriteLine($"  nulleable?.Length ?? 0 → {longitudSegura}");

// ────────────────────────────────── Sección 8 ─────────────────────────────
// Resumen visual de la historia de .NET
// ──────────────────────────────────────────────────────────────────────────
ImprimirSeparador("7. HISTORIA DE .NET EN UNA LÍNEA DE TIEMPO");

// Línea de tiempo (ASCII):
// 2002 ──── .NET Framework 1.0 ──── Solo Windows, monolítico
//   │
// 2016 ──── .NET Core 1.0 ────────── Open source, multiplataforma, modular
//   │
// 2020 ──── .NET 5 ───────────────── Unificación (.NET Core + Mono + Xamarin)
//   │                                  Se saltan el "4" para evitar confusión
//   │                                  con .NET Framework 4.x
// 2021 ──── .NET 6 (LTS) ─────────── Rendimiento, Minimal APIs, C# 10
//   │
// 2022 ──── .NET 7 ───────────────── C# 11, mejoras en GC, NativeAOT
//   │
// 2023 ──── .NET 8 (LTS) ─────────── C# 12, NativeAOT maduro, Blazor United
//   │                                  Aspire para cloud-native
// 2025 ──── .NET 10 (LTS) ────────── C# 14, mejoras de rendimiento
//   ▼
//  Futuro

Console.WriteLine("""
    2002 ─── .NET Framework 1.0 ─── Solo Windows
    2016 ─── .NET Core 1.0      ─── Open source + multiplataforma
    2020 ─── .NET 5             ─── Unificación
    2021 ─── .NET 6 (LTS)       ─── Minimal APIs + C# 10
    2022 ─── .NET 7             ─── NativeAOT + C# 11
    2023 ─── .NET 8 (LTS)       ─── C# 12 + NativeAOT maduro
    2024 ─── .NET 9             ─── C# 13
    2025 ─── .NET 10 (LTS)      ─── Próximo LTS
    """);

// ══════════════════════════════════════════════════════════════════════════
// FIN DEL PROGRAMA DE EJEMPLO
// ══════════════════════════════════════════════════════════════════════════
Console.WriteLine();
Console.WriteLine("✅ Fin de la demostración de la Lección 01.");
Console.WriteLine("   Revisa el README.md para la teoría completa.");
Console.WriteLine("   Ejecuta 'dotnet test' para validar los ejercicios.");

// ──────────────────────────────────────────────────────────────────────────
// Métodos auxiliares
// ──────────────────────────────────────────────────────────────────────────

/// <summary>
/// Imprime un separador visual para organizar la salida en consola.
/// Uso de `static` en métodos locales: evita capturar variables del scope.
/// </summary>
static void ImprimirSeparador(string titulo)
{
    Console.WriteLine();
    Console.WriteLine(new string('═', 60));
    Console.WriteLine($"  {titulo}");
    Console.WriteLine(new string('═', 60));
}
