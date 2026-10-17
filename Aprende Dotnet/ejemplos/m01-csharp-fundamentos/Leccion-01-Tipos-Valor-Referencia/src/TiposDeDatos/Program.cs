// ──────────────────────────────────────────────────────────────────────────
// Módulo 1 — Lección 01: Tipos de valor y tipos de referencia
// ──────────────────────────────────────────────────────────────────────────
// Temas: Stack vs Heap, struct vs class, record, boxing/unboxing,
//        ref struct, Span<T>, in/ref/out, default values.
//
// Comparación con JavaScript (todo es referencia salvo primitivos),
// Python (todo es objeto en heap) y Java (primitivos en stack, objetos en heap).
// ──────────────────────────────────────────────────────────────────────────

using System.Buffers;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

ImprimirSeccion("1. STACK vs HEAP — DÓNDE VIVEN LOS DATOS");

// ══════════════════════════════════════════════════════════════════════════
// En .NET hay DOS zonas de memoria principales:
//
// STACK (pila):
//   - Memoria LIFO (Last In, First Out) por hilo
//   - ~1 MB por hilo
//   - Rápida (acceso contiguo, caché-friendly)
//   - Se libera automáticamente al salir del método
//   - Aquí viven: variables locales, parámetros, structs, refs
//
// HEAP (montón):
//   - Memoria dinámica gestionada por el GC
//   - Sin límite fijo (hasta que el SO diga basta)
//   - Más lenta (acceso indirecto, fragmentación potencial)
//   - Se libera cuando el GC decide (no determinista)
//   - Aquí viven: class, arrays, strings, objetos boxed
// ══════════════════════════════════════════════════════════════════════════

Console.WriteLine("""
    ┌──────────────────┬─────────────────────┬─────────────────────┐
    │                  │       STACK         │        HEAP         │
    ├──────────────────┼─────────────────────┼─────────────────────┤
    │ Velocidad        │ Muy rápido          │ Más lento           │
    │ Asignación       │ Automática (LIFO)   │ GC (complejo)       │
    │ Liberación       │ Al salir del scope  │ GC (no determinista)│
    │ Tamaño           │ ~1 MB por hilo      │ GB (límite del SO)  │
    │ Qué vive aquí    │ int, struct, refs   │ class, arrays,      │
    │                  │ Span<T>, params     │ string, object      │
    │ Fragmentación    │ No                  │ Sí (LOH)            │
    └──────────────────┴─────────────────────┴─────────────────────┘
    """);

ImprimirSeccion("2. TIPOS DE VALOR (VALUE TYPES)");

// Tipos de valor: contienen el dato DIRECTAMENTE.
// Al asignar, se COPIA el valor completo.
// Viven en el stack (o inline dentro de otro objeto en el heap).

// ──── Tipos de valor integrados (built-in) ────
int entero = 42;
double real = 3.14159;
bool flag = true;
char caracter = 'A';
decimal precio = 19.99m; // 128-bit, precisión financiera
Console.WriteLine($"bool y char de ejemplo: {flag}, {caracter}");

// ──── Tipos de valor personalizados (struct y enum) ────
Punto2D punto1 = new() { X = 10, Y = 20 };
Punto2D punto2 = punto1; // ¡COPIA! punto2 es independiente
punto2.X = 99;

Console.WriteLine($"int (System.Int32)   : {entero} (ocupa {sizeof(int)} bytes en stack)");
Console.WriteLine($"double (System.Double): {real} (ocupa {sizeof(double)} bytes)");
Console.WriteLine($"decimal               : {precio} (ocupa {sizeof(decimal)} bytes)");
Console.WriteLine();
Console.WriteLine($"punto1 después de modificar punto2: ({punto1.X}, {punto1.Y})");
Console.WriteLine($"punto2 modificado                : ({punto2.X}, {punto2.Y})");
Console.WriteLine("¡punto1 NO cambió! struct se copia por valor, como int.");

ImprimirSeccion("3. TIPOS DE REFERENCIA (REFERENCE TYPES)");

// Tipos de referencia: contienen una REFERENCIA (puntero) al objeto en heap.
// Al asignar, se copia la referencia, NO el objeto.
// El objeto real vive en el heap.

// ──── Tipo de referencia: class ────
Cliente cliente1 = new("Ana García", "ana@email.com");
Cliente cliente2 = cliente1; // ¡NO COPIA! Ambas referencias apuntan al MISMO objeto
cliente2.Nombre = "Ana Modificada";

Console.WriteLine($"cliente1.Nombre después de modificar cliente2: {cliente1.Nombre}");
Console.WriteLine($"cliente2.Nombre                               : {cliente2.Nombre}");
Console.WriteLine("¡cliente1 TAMBIÉN cambió! class es tipo de referencia.");
Console.WriteLine($"¿Mismo objeto? {ReferenceEquals(cliente1, cliente2)}");

// ──── Tipo de referencia: string (inmutable) ────
string original = "Hola";
string copia = original;   // Ambas apuntan al mismo string en heap
copia = "Mundo";           // Crea un NUEVO string, no modifica el original
Console.WriteLine($"\nstring original: {original}"); // "Hola"
Console.WriteLine($"string copia   : {copia}");       // "Mundo"
Console.WriteLine("string es inmutable: modificar crea una nueva instancia.");

ImprimirSeccion("4. struct vs class — CUÁNDO USAR CADA UNO");

Console.WriteLine("""
    ┌──────────────────┬─────────────────────┬─────────────────────┐
    │                  │       struct        │        class        │
    ├──────────────────┼─────────────────────┼─────────────────────┤
    │ Dónde vive       │ Stack (o inline)    │ Heap                │
    │ Copia            │ Por VALOR           │ Por REFERENCIA      │
    │ Herencia         │ NO (solo interfaces)│ SÍ                  │
    │ Constructor      │ No sin parámetros   │ Sí                  │
    │   sin params     │ (C# 10+ sí permite) │                     │
    │ Nullable         │ int? = Nullable<int>│ string? (NRT)       │
    │ Default          │ Todo a 0/false/null │ null                │
    │ Rendimiento      │ Mejor (sin GC)      │ Peor (presión GC)   │
    │ Cuándo usar      │ < 16 bytes, inmutable Peq. datos, lógica │
    │                  │ dato pequeño,        │ compleja, herencia  │
    │                  │ efímero              │                     │
    └──────────────────┴─────────────────────┴─────────────────────┘

    REGLA PRÁCTICA de Microsoft:
    "Usa struct si el tamaño es < 16 bytes, es inmutable,
     y no necesitas herencia. En caso contrario, usa class."
    """);

// ──── Ejemplo: struct para coordenadas (pequeño, inmutable lógico) ────
// ──── class para entidad de negocio (herencia, identidad) ────

ImprimirSeccion("5. RECORD — INMUTABILIDAD POR DEFECTO");

// `record` (C# 9) y `record struct` (C# 10) son tipos especiales
// diseñados para datos inmutables y comparación por valor.

// ──── record class (referencia, pero con value equality) ────
// Sintaxis posicional: constructor, propiedades y Deconstruct en una línea
Persona ana = new("Ana", "López", new DateOnly(1990, 5, 15));
Persona anaCopia = ana with { Apellido = "García" }; // with: crea copia modificando campos

Console.WriteLine($"Persona original: {ana}");
Console.WriteLine($"Copia modificada: {anaCopia}");
Console.WriteLine($"¿Igualdad estructural? {ana == anaCopia}"); // false: apellido distinto

// ──── record struct (valor, inmutable, alto rendimiento) ────
// Ideal para DTOs, mensajes, value objects en DDD
Punto3D origen = new(0, 0, 0);
Punto3D otroOrigen = new(0, 0, 0);
Punto3D destino = origen with { Z = 10 };

Console.WriteLine($"\nPunto3D origen : {origen}");
Console.WriteLine($"Punto3D destino: {destino}");
Console.WriteLine($"¿origen == otroOrigen? {origen == otroOrigen}"); // true: value equality
Console.WriteLine($"¿origen == destino? {origen == destino}");     // false: Z distinto

ImprimirSeccion("6. BOXING Y UNBOXING");

// Boxing: convertir value type → object/reference type (va al heap)
// Unboxing: extraer value type del object (del heap al stack)
//
// El boxing es CARO porque:
//   1. Asigna memoria en el heap
//   2. Copia el valor del stack al heap
//   3. Genera basura que el GC debe recolectar
//   4. El unboxing requiere un cast y verificación de tipos en runtime

int numero = 42;

// ──── BOXING: int → object (asignación en heap + copia) ────
object caja = numero; // El int 42 se copia al heap dentro de un "objeto boxed"

// ──── UNBOXING: object → int (extracción + cast) ────
int recuperado = (int)caja; // Cast explícito obligatorio

Console.WriteLine($"Original: {numero}");
Console.WriteLine($"Boxed   : {caja}");
Console.WriteLine($"Unboxed : {recuperado}");

// ──── Dónde ocurre boxing sin que te des cuenta ────
// 1. Pasar value type a método que espera object
// 2. Concatenar con string usando + (el compilador lo evita con interpolación)
// 3. Usar value types en colecciones no genéricas (ArrayList, Hashtable)
// 4. Asignar struct a interfaz (boxing!)

ArrayList listaAntigua = []; // No genérica → boxing
listaAntigua.Add(1);    // Boxing: int → object
listaAntigua.Add(2);    // Boxing
listaAntigua.Add(3);    // Boxing

// ✅ List<T> genérica NO hace boxing
List<int> listaModerna = [1, 2, 3]; // Sin boxing

Console.WriteLine($"\nArrayList (con boxing): {listaAntigua.Count} elementos");
Console.WriteLine($"List<int> (sin boxing): {listaModerna.Count} elementos");

ImprimirSeccion("7. REF STRUCT Y SPAN<T> (C# 7.2+)");

// `ref struct`: struct que SOLO puede vivir en el stack.
// No puede ser campo de una class, no puede estar en el heap,
// no puede ser capturado por lambdas, no puede ser async.
//
// ¿Por qué? Porque el GC no sabe moverlos (no están en el heap gestionado).
// Esto permite optimizaciones extremas de rendimiento.

// ──── Span<T>: vista de memoria contigua sin copia ────
int[] numeros = [10, 20, 30, 40, 50, 60, 70, 80];
Span<int> slice = numeros.AsSpan(2, 4); // Vista de [30, 40, 50, 60]
slice[0] = 999; // Modifica el array original (Span es vista mutable)

Console.WriteLine("Span<T> — vista mutable sin copia:");
Console.WriteLine($"Array original: [{string.Join(", ", numeros)}]");
Console.WriteLine($"Slice span   : [{string.Join(", ", slice.ToArray())}]");

// ──── ReadOnlySpan<T>: vista inmutable ────
string frase = "Aprendiendo C# 12 con .NET 8";
ReadOnlySpan<char> sub = frase.AsSpan(12, 4); // "C# 1"
Console.WriteLine($"\nReadOnlySpan<char> sobre string: \"{sub}\"");

// ──── stackalloc: asignar array EN EL STACK ────
// Útil para buffers temporales pequeños, evita asignaciones en heap.
Span<int> buffer = stackalloc int[4] { 1, 2, 3, 4 };
Console.WriteLine($"\nstackalloc int[4] en el stack: [{string.Join(", ", buffer.ToArray())}]");

ImprimirSeccion("8. PARÁMETROS: in, ref, out");

// C# permite pasar parámetros por valor, por referencia y como solo lectura.
// Esto es crucial para rendimiento con structs grandes.
// Java no tiene equivalente directo (solo pasa referencias de objetos por valor).

int valor = 10;

// ──── Por valor (default): se copia ────
void PorValor(int x) { x = 99; }
PorValor(valor);
Console.WriteLine($"Por valor (default)    : {valor}"); // 10, no cambió

// ──── ref: pasa por referencia (lectura/escritura) ────
void PorReferencia(ref int x) { x = 99; }
PorReferencia(ref valor);
Console.WriteLine($"Por referencia (ref)   : {valor}"); // 99

// ──── in: pasa por referencia SOLO LECTURA ────
// El compilador garantiza que no se modifica. Ideal para structs grandes.
void SoloLectura(in int x)
{
    // x = 50; // Error CS8331: Cannot assign to variable 'in int'
    Console.WriteLine($"  Valor recibido (in): {x}");
}
SoloLectura(valor);

// ──── out: pasa por referencia, DEBE asignarse dentro del método ────
// Útil para métodos que devuelven múltiples valores.
void Inicializar(out int x) { x = 100; } // Obligatorio asignar
Inicializar(out int nuevoValor);
Console.WriteLine($"Salida (out)           : {nuevoValor}"); // 100

ImprimirSeccion("9. COMPARACIÓN CON OTROS LENGUAJES");

Console.WriteLine("""
    ┌───────────────────┬───────────────┬───────────────┬───────────────┐
    │ Concepto          │ C# (.NET 8)   │ Java          │ JavaScript    │
    ├───────────────────┼───────────────┼───────────────┼───────────────┤
    │ Value types       │ struct, enum,  │ Solo primitivos│ Solo primitivos│
    │                   │ record struct  │ (int, long...)│ (number, ...) │
    │ Reference types   │ class, record, │ Todo lo demás │ Todo lo demás │
    │                   │ string, array, │ (objetos)     │ (objetos)     │
    │                   │ delegate       │               │               │
    │ Stack allocation  │ Sí (struct,    │ No (Escape    │ No            │
    │                   │ stackalloc,    │ Analysis      │               │
    │                   │ Span<T>)       │ limitado)     │               │
    │ Immutable types   │ record, record │ record (Java  │ Object.freeze │
    │                   │ struct, init    │ 14+), freeze  │ (superficial) │
    │ pass-by-reference │ ref, in, out   │ No (solo valor│ No (objetos   │
    │                   │                │ de referencia)│ por sharing)  │
    │ Default values    │ 0, false, null │ 0, false, null│ undefined     │
    │ Null safety       │ NRT (compiler) │ @Nullable     │ undefined/null│
    │                   │                │ (opcional)    │               │
    └───────────────────┴───────────────┴───────────────┴───────────────┘
    """);

Console.WriteLine();
Console.WriteLine("✅ Fin de la Lección 01 del Módulo 1.");
Console.WriteLine("   Hemos cubierto: value types, reference types, struct vs class,");
Console.WriteLine("   record, boxing/unboxing, ref struct, Span<T>, in/ref/out.");

// ══════════════════════════════════════════════════════════════════════════
// TIPOS USADOS EN LA DEMOSTRACIÓN
// ══════════════════════════════════════════════════════════════════════════

// Los tipos Punto2D, Cliente, Persona y Punto3D están definidos en Types.cs

// ──────────────────────────────────────────────────────────────────────────
// Método auxiliar
// ──────────────────────────────────────────────────────────────────────────
static void ImprimirSeccion(string titulo)
{
    Console.WriteLine();
    Console.WriteLine(new string('═', 65));
    Console.WriteLine($"  {titulo}");
    Console.WriteLine(new string('═', 65));
}
