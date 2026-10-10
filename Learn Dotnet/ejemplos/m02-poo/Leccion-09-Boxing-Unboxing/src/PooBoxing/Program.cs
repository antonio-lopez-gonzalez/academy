// ──────────────────────────────────────────────────────────────────────────
// Módulo 2 — Lección 09: Boxing y Unboxing en C#
// ──────────────────────────────────────────────────────────────────────────
// Temas: Boxing (value type → object), Unboxing (object → value type),
//        impacto en heap, cómo detectarlo, cómo evitarlo con genéricos,
//        cuándo ocurre de forma invisible, rendimiento comparado.
//
// Comparación con Java (autoboxing int→Integer, Integer→int),
// JavaScript (no existe, todo es referencia salvo primitivos en stack de V8).
// ──────────────────────────────────────────────────────────────────────────

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

// ══════════════════════════════════════════════════════════════════════════════
// 1. QUÉ ES BOXING
// ══════════════════════════════════════════════════════════════════════════════

ImprimirSeccion("1. QUÉ ES BOXING");

Console.WriteLine("""
    Boxing = convertir un VALUE TYPE (struct, int, bool, etc.) a OBJECT
             (o a una interfaz que el value type implemente).

    ┌─────────── STACK ───────────┐      ┌─────────── HEAP ────────────┐
    │                             │      │                              │
    │  int x = 42;                │      │  ┌──────────────────────┐    │
    │  ┌─────┐                    │      │  │  object (boxed int)   │    │
    │  │ 42  │                    │ ──── │  │  SyncBlock | MT | 42 │    │
    │  └─────┘                    │ copy │  └──────────────────────┘    │
    │                             │      │                              │
    └─────────────────────────────┘      └──────────────────────────────┘

    El value type se COPIA al heap dentro de un "objeto caja".
    El GC ahora debe rastrear esta copia.
    """); 

// ──── Ejemplo mínimo de boxing ────
int numero = 42;
Console.WriteLine($"  int en stack: {numero}");

// Aquí ocurre boxing: el int 42 se copia al heap y la variable 'caja'
// almacena una referencia al objeto boxed en el heap.
object caja = numero;

Console.WriteLine($"  object en heap: {caja}");
Console.WriteLine($"  ¿Misma referencia? numero.Equals(caja) = {numero.Equals(caja)}");
Console.WriteLine($"  ¡OJO! Son objetos distintos. Modificar 'numero' NO afecta a 'caja'.");
numero = 99;
Console.WriteLine($"  numero ahora: {numero}, caja sigue siendo: {caja}");

// Cada boxing crea una NUEVA copia en el heap.
object caja2 = numero;  // Otro boxing, otra copia independiente
Console.WriteLine($"  caja2 (otra copia): {caja2}");

// ══════════════════════════════════════════════════════════════════════════════
// 2. QUÉ ES UNBOXING
// ══════════════════════════════════════════════════════════════════════════════

ImprimirSeccion("2. QUÉ ES UNBOXING");

Console.WriteLine("""
    Unboxing = extraer el value type original desde un object.
    Requiere CAST EXPLÍCITO al tipo correcto.
    Si el tipo no coincide → InvalidCastException en tiempo de ejecución.

    ┌─────────── HEAP ────────────┐      ┌─────────── STACK ───────────┐
    │  ┌──────────────────────┐   │      │                             │
    │  │  object (boxed int)  │   │ ──── │  int y = 42;               │
    │  │  SyncBlock | MT | 42 │   │ copy │  ┌─────┐                   │
    │  └──────────────────────┘   │      │  │ 42  │                   │
    └──────────────────────────────┘      │  └─────┘                   │
                                          └─────────────────────────────┘

    El objeto en heap NO se modifica. Se COPIA el valor al stack.
    """); 

// ──── Ejemplo de unboxing correcto ────
object valorBoxed = 100;          // Boxing: int → object
int recuperado = (int)valorBoxed; // Unboxing: object → int (cast explícito)
Console.WriteLine($"  Boxing: int 100 → object. Unboxing: object → int {recuperado}");

// ──── Unboxing con tipo incorrecto: InvalidCastException ────
Console.WriteLine();
Console.WriteLine("  Intentando unboxing al tipo incorrecto:");
object valorDouble = 3.14; // Boxing: double → object

try
{
    // Esto lanza InvalidCastException porque el valor boxed es double, no int
    int forzado = (int)valorDouble;
    Console.WriteLine($"  Nunca llegamos aquí: {forzado}");
}
catch (InvalidCastException ex)
{
    Console.WriteLine($"  [InvalidCastException] {ex.Message}");
    Console.WriteLine("  El tipo en tiempo de ejecución era double, no int.");
}

// Para hacer unboxing correcto de double a int hay que hacer doble paso:
double primeroDouble = (double)valorDouble; // Unboxing al tipo correcto
int luegoInt = (int)primeroDouble;          // Conversión explícita
Console.WriteLine($"  Forma correcta: (double)valorDouble → {(double)valorDouble}, luego (int) → {luegoInt}");

// ──── Unboxing de null: NullReferenceException ────
object? nulo = null;
try
{
    int imposible = (int)nulo!; // NRE: no se puede hacer unboxing de null
    Console.WriteLine($"  Nunca llegamos: {imposible}");
}
catch (NullReferenceException ex)
{
    Console.WriteLine($"  [NullReferenceException] {ex.Message}");
    Console.WriteLine("  No se puede hacer unboxing de null a un value type.");
}

// Solución: usar nullable value types
object? nulo2 = null;
int? resultadoSeguro = nulo2 as int?; // as int? devuelve null sin excepción
Console.WriteLine($"  Unboxing seguro con 'as': nulo2 as int? = {(resultadoSeguro.HasValue ? resultadoSeguro.Value : (object)"null")}");

Console.WriteLine();

// ══════════════════════════════════════════════════════════════════════════════
// 3. CUÁNDO OCURRE BOXING
// ══════════════════════════════════════════════════════════════════════════════

ImprimirSeccion("3. CUÁNDO OCURRE BOXING (CASOS CONCRETOS)");

// ──── 3a. Asignar value type a object ────
Console.WriteLine("  3a. Asignar value type a object:");
int a = 10;
object o1 = a;                            // BOXING
Console.WriteLine($"  object o1 = a; // int {a} → object {o1}");

// ──── 3b. Pasar value type a método que recibe object ────
Console.WriteLine("\n  3b. Pasar value type a método con parámetro object:");
PrintObject(50);                          // BOXING: int 50 → object
PrintObject(DateTime.Now);                // BOXING: DateTime → object
PrintObject(Guid.NewGuid());              // BOXING: Guid → object

// ──── 3c. Asignar value type a interfaz ────
Console.WriteLine("\n  3c. Asignar value type a interfaz:");
IComparable comparable = 42;              // BOXING: int → IComparable
Console.WriteLine($"  IComparable c = 42; // boxing. Tipo real: {comparable.GetType()}");

IEquatable<int> equatable = 99;           // BOXING: int → IEquatable<int>
Console.WriteLine($"  IEquatable<int> e = 99; // boxing. Tipo real: {equatable.GetType()}");
Console.WriteLine("  ¡Incluso las interfaces genéricas causan boxing si T es value type!");
Console.WriteLine("  (a menos que se use restricción 'where T : struct' en el método que recibe)");

// ──── 3d. Colecciones no genéricas (ArrayList, Hashtable, Queue, Stack) ────
Console.WriteLine("\n  3d. Colecciones no genéricas:");
ArrayList listaAntigua = new();
listaAntigua.Add(1);                       // BOXING: int → object
listaAntigua.Add(2);                       // BOXING
listaAntigua.Add(3);                       // BOXING
Console.WriteLine($"  ArrayList con 3 ints: {listaAntigua.Count} elementos");
Console.WriteLine("  Cada Add() causó boxing.");

// Al recuperar, también hay unboxing:
int primerElemento = (int)listaAntigua[0]!; // UNBOXING: object → int
Console.WriteLine($"  Recuperar [0]: (int)listaAntigua[0] = {primerElemento} (unboxing)");

// ──── 3e. Llamar a GetType() en value type ────
Console.WriteLine("\n  3e. GetType() en value type:");
Type tipoInt = 42.GetType();              // BOXING (el compilador podría optimizar en algunos casos)
Console.WriteLine($"  42.GetType() = {tipoInt} (causa boxing implícitamente)");

// ──── 3f. String concatenation con value types ────
Console.WriteLine("\n  3f. Concatenación de strings con value types:");
string resultado = "El número es " + 42;   // BOXING en versiones antiguas de C#
                                           // En .NET moderno se usa string.Concat(object, object)
                                           // que recibe object → boxing
Console.WriteLine($"  \"El número es \" + 42 = \"{resultado}\" (boxing interno)");

// La interpolación evita boxing (el compilador genera llamadas a AppendFormat tipadas):
string interpolado = $"El número es {42}"; // SIN boxing (usa ISpanFormattable o generic)
Console.WriteLine($"  $\"El número es {{42}}\" = \"{interpolado}\" (sin boxing, usa genéricos)");

// Pero ojo: si el value type no implementa ISpanFormattable, sí hay boxing.
// int, double, decimal, DateTime, etc. SÍ implementan ISpanFormattable en .NET 8.

// ──── 3g. Métodos de System.Enum en value types ────
Console.WriteLine("\n  3g. Métodos de System.Enum:");
DayOfWeek dia = DayOfWeek.Monday;
string nombreDia = dia.ToString();          // Boxing interno porque Enum.ToString() usa object internamente
Console.WriteLine($"  DayOfWeek.Monday.ToString() = \"{nombreDia}\" (boxing por llamada a Enum.ToString)");

// ──── 3h. Pasar struct a método que recibe interfaz ────
Console.WriteLine("\n  3h. Pasar struct a método que espera interfaz:");
static void UsarComparable(IComparable c) => Console.WriteLine($"    CompareTo(10) = {c.CompareTo(10)}");
UsarComparable(25);  // BOXING: int → IComparable

// ──── 3i. struct de usuario asignado a interfaz ────
Console.WriteLine("\n  3i. Struct propio asignado a interfaz:");
Point punto = new(5, 10);
IEquatable<Point> puntoEquatable = punto;  // BOXING: Point → IEquatable<Point>
Console.WriteLine($"  IEquatable<Point> eq = punto; // boxing. ¿Equals? {puntoEquatable.Equals(new Point(5, 10))}");
Console.WriteLine("  Incluso si el struct implementa la interfaz, al asignar a IEquatable<T> hay boxing.");

// ══════════════════════════════════════════════════════════════════════════════
// 4. DETECCIÓN DE BOXING (contador manual)
// ══════════════════════════════════════════════════════════════════════════════

ImprimirSeccion("4. DETECCIÓN DE BOXING CON CONTADOR MANUAL");

Console.WriteLine("""
    .NET no expone un contador global de boxing, pero podemos simularlo.
    Cada boxing asigna ~12-24 bytes en el heap (sync block index + method table
    pointer + datos del value type). En bucles intensivos esto dispara el GC.
    """); 

int contadorBoxing = 0;

object DetectarBoxing<T>(T valor) where T : struct
{
    contadorBoxing++;
    return valor!; // El boxing ocurre aquí al convertir T a object
}

// Simulamos varios boxing y llevamos la cuenta
object box1 = DetectarBoxing(100);
object box2 = DetectarBoxing(3.14);
object box3 = DetectarBoxing(DateTime.Now);
object box4 = DetectarBoxing(Guid.NewGuid());
object box5 = DetectarBoxing(false);

Console.WriteLine($"  Boxing detectados: {contadorBoxing}");
Console.WriteLine("  Cada uno generó un objeto en el heap (~12-24 bytes + overhead GC).");
Console.WriteLine("  En un bucle de 1M iteraciones, serían ~12-24 MB extra de basura.");

// ──── Memoria estimada ────
long memoriaAntes = GC.GetTotalMemory(forceFullCollection: true);
int contadorAntesBox = contadorBoxing;

for (int i = 0; i < 100_000; i++)
{
    DetectarBoxing(i); // 100.000 boxing
}

long memoriaDespues = GC.GetTotalMemory(forceFullCollection: false);
long bytesAsignados = memoriaDespues - memoriaAntes;

Console.WriteLine($"  Después de 100.000 boxing:");
Console.WriteLine($"    Boxing extra detectados: {contadorBoxing - contadorAntesBox}");
Console.WriteLine($"    Bytes asignados en heap: {bytesAsignados:N0} (~{bytesAsignados / 100_000.0:F1} bytes por boxing)");

// ══════════════════════════════════════════════════════════════════════════════
// 5. CÓMO EVITAR BOXING
// ══════════════════════════════════════════════════════════════════════════════

ImprimirSeccion("5. CÓMO EVITAR BOXING");

// ──── 5a. Usar genéricos en lugar de object ────
Console.WriteLine("  5a. Genéricos (List<T>, Dictionary<K,V>) en lugar de colecciones no genéricas:");
List<int> listaModerna = new() { 1, 2, 3 }; // SIN boxing: List<int> almacena ints directamente
Console.WriteLine($"  List<int> con {listaModerna.Count} elementos. CERO boxing.");
Console.WriteLine($"  listaModerna[0] = {listaModerna[0]} (sin unboxing, acceso directo)");

// ──── 5b. Métodos genéricos en lugar de object ────
Console.WriteLine("\n  5b. Métodos genéricos en lugar de object:");
PrintGeneric(42);             // SIN boxing
PrintGeneric(new Point(7, 8)); // SIN boxing (el compilador especializa para Point)
PrintGeneric(DateTime.Now);    // SIN boxing

// ──── 5c. Sobrescribir ToString(), GetHashCode(), Equals() en structs ────
Console.WriteLine("\n  5c. Sobrescribir métodos virtuales en structs:");
Point p1 = new(3, 4);
Point p2 = new(3, 4);

// Si Point NO tuviera ToString() override, esta llamada causaría boxing.
// Como SÍ lo tiene, el compilador llama a la versión tipada directamente.
Console.WriteLine($"  p1.ToString() = {p1} (sin boxing, override resuelve en tiempo de compilación)");
Console.WriteLine($"  p1.Equals(p2) = {p1.Equals(p2)} (sin boxing, IEquatable<Point> tipado)");

// PERO si forzamos la llamada a través de object, SÍ hay boxing:
object objPunto = p1; // BOXING
Console.WriteLine($"  ((object)p1).ToString() = {objPunto} (SÍ hay boxing al asignar a object)");

// ──── 5d. Usar interfaces genéricas (IComparable<T> en lugar de IComparable) ────
Console.WriteLine("\n  5d. Interfaces genéricas vs no genéricas:");
Money m1 = new(100m, "EUR");
Money m2 = new(50m, "EUR");

// Bien: IComparable<Money> — sin boxing
int comparacionTipada = m1.CompareTo(m2);
Console.WriteLine($"  m1.CompareTo(m2) tipado = {comparacionTipada} (sin boxing)");

// Mal: si usáramos IComparable (no genérico), habría boxing
IComparable m1Boxed = m1; // BOXING al asignar struct a interfaz
Console.WriteLine($"  Al asignar Money a IComparable SÍ hay boxing. Tipo: {m1Boxed.GetType().Name}");

// ──── 5e. Interpolación de strings (ya usa genéricos internamente) ────
Console.WriteLine("\n  5e. Interpolación de strings:");
Console.WriteLine($"  int: {42}, double: {3.14}, DateTime: {DateTime.Now:d}");
Console.WriteLine("  La interpolación moderna usa ISpanFormattable → cero boxing para tipos integrados.");

// ══════════════════════════════════════════════════════════════════════════════
// 6. DEMO PRINCIPAL: COMPARATIVA DE RENDIMIENTO
// ══════════════════════════════════════════════════════════════════════════════

ImprimirSeccion("6. COMPARATIVA DE RENDIMIENTO: BOXING vs SIN BOXING");

const int iteraciones = 10_000_000; // 10 millones

Console.WriteLine($"  Iteraciones: {iteraciones:N0}");

// ──── Escenario 1: ArrayList (boxing en cada Add y unboxing en cada lectura) ────
Medir("ArrayList (boxing + unboxing)", () =>
{
    ArrayList arrayList = new(iteraciones);
    for (int i = 0; i < iteraciones; i++)
    {
        arrayList.Add(i);                   // BOXING: int → object
    }

    long suma = 0;
    for (int i = 0; i < iteraciones; i++)
    {
        suma += (int)arrayList[i]!;         // UNBOXING: object → int
    }

    // Usamos la suma para evitar que el compilador optimice el bucle
    if (suma < 0) Console.WriteLine("imposible");
});

// Forzar GC entre pruebas para condiciones justas
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

// ──── Escenario 2: List<int> (cero boxing) ────
Medir("List<int> (sin boxing)", () =>
{
    List<int> lista = new(iteraciones);
    for (int i = 0; i < iteraciones; i++)
    {
        lista.Add(i);                       // SIN boxing: almacena int directamente
    }

    long suma = 0;
    for (int i = 0; i < iteraciones; i++)
    {
        suma += lista[i];                   // SIN unboxing: acceso directo al array interno
    }

    if (suma < 0) Console.WriteLine("imposible");
});

GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

// ──── Escenario 3: Array de int (más rápido aún) ────
Medir("int[] (array nativo)", () =>
{
    int[] arrayNativo = new int[iteraciones];
    for (int i = 0; i < iteraciones; i++)
    {
        arrayNativo[i] = i;                 // SIN boxing, acceso directo a memoria continua
    }

    long suma = 0;
    for (int i = 0; i < iteraciones; i++)
    {
        suma += arrayNativo[i];             // SIN boxing
    }

    if (suma < 0) Console.WriteLine("imposible");
});

Console.WriteLine();
Console.WriteLine("""
    ┌─────────────────────┬───────────────────────────────────────────────┐
    │ Colección           │ ¿Boxing?                                     │
    ├─────────────────────┼───────────────────────────────────────────────┤
    │ ArrayList           │ SÍ: boxing en Add, unboxing en lectura        │
    │ List<T>             │ NO: almacena T[] internamente sin conversión  │
    │ int[]               │ NO: acceso directo al array en heap           │
    │ Hashtable           │ SÍ: boxing en clave y valor                   │
    │ Dictionary<K,V>     │ NO (si K y V son value types, están inline)  │
    └─────────────────────┴───────────────────────────────────────────────┘
    """);

// ══════════════════════════════════════════════════════════════════════════════
// 7. BUENAS PRÁCTICAS — QUÉ HACER Y QUÉ EVITAR
// ══════════════════════════════════════════════════════════════════════════════

ImprimirSeccion("7. BUENAS PRÁCTICAS vs ERRORES COMUNES");

// ──── MAL: event handler con boxing ────
Console.WriteLine("  MAL: Pasar value type como parámetro object en eventos:");
static void EventHandlerMalo(object sender, object valor) // valor recibe int → boxing
{
    int numeroInterno = (int)valor; // unboxing
    Console.WriteLine($"    Recibido: {numeroInterno}");
}
EventHandlerMalo(null!, 123); // BOXING al pasar 123 como object

// ──── BIEN: event handler genérico ────
Console.WriteLine("\n  BIEN: EventHandler<T> genérico:");
static void EventHandlerBueno<T>(object sender, T valor) // T especializado por el compilador
{
    Console.WriteLine($"    Recibido: {valor}"); // sin boxing ni unboxing
}
EventHandlerBueno(null!, 456); // SIN boxing

// ──── MAL: usar IComparable en lugar de IComparable<T> ────
Console.WriteLine("\n  MAL: IComparable (no genérico) con structs:");
static int CompararElementosMal(IComparable a, IComparable b)
{
    return a.CompareTo(b); // Ambos parámetros causan boxing al pasar structs
}
// Llamada con ints: boxing en ambos parámetros
int resultadoMal = CompararElementosMal(10, 20);
Console.WriteLine($"  CompararElementosMal(10, 20) = {resultadoMal} (boxing en ambos parámetros)");

// ──── BIEN: IComparable<T> ────
Console.WriteLine("\n  BIEN: IComparable<T> con restricción de tipo:");
static int CompararElementosBien<T>(T a, T b) where T : IComparable<T>
{
    return a.CompareTo(b); // SIN boxing gracias a la restricción
}
int resultadoBien = CompararElementosBien(10, 20);
Console.WriteLine($"  CompararElementosBien(10, 20) = {resultadoBien} (sin boxing)");

// ──── MAL: string.Format con muchos value types ────
Console.WriteLine("\n  MAL: string.Format causa boxing con value types:");
string msgMalo = string.Format("Valores: {0}, {1}, {2}", 10, 20, 30); // 3 boxing
Console.WriteLine($"    \"{msgMalo}\" (cada parámetro va como object)");

// ──── BIEN: interpolación ────
Console.WriteLine("  BIEN: interpolación de strings:");
string msgBueno = $"Valores: {10}, {20}, {30}"; // cero boxing
Console.WriteLine($"    \"{msgBueno}\"");

// ══════════════════════════════════════════════════════════════════════════════
// 8. RESUMEN Y EJEMPLO FINAL CON MONEY STRUCT
// ══════════════════════════════════════════════════════════════════════════════

ImprimirSeccion("8. EJEMPLO FINAL: STRUCT MONEY");

// Creamos varios valores Money y los usamos en colecciones genéricas
List<Money> precios = new()
{
    new Money(19.99m, "EUR"),
    new Money(9.50m, "EUR"),
    new Money(25.00m, "USD")
};

Console.WriteLine("  Precios almacenados en List<Money> (sin boxing):");
foreach (Money precio in precios)
{
    Console.WriteLine($"    {precio} — hash: {precio.GetHashCode()}");
}

// Dictionary con struct como clave: el Dictionary genérico usa EqualityComparer<T>.Default,
// que llama a IEquatable<T>.Equals sin boxing si está implementado.
Dictionary<Money, string> productos = new()
{
    [new Money(19.99m, "EUR")] = "Libro",
    [new Money(9.50m, "EUR")]  = "Café",
    [new Money(25.00m, "USD")] = "Suscripción"
};

Money busqueda = new(19.99m, "EUR");
if (productos.TryGetValue(busqueda, out string? nombreProducto))
{
    Console.WriteLine($"\n  Búsqueda en Dictionary<Money, string>: {busqueda} → {nombreProducto}");
    Console.WriteLine("  La búsqueda NO causó boxing gracias a IEquatable<Money>.");
}

// ══════════════════════════════════════════════════════════════════════════════
// 9. PREGUNTAS FRECUENTES
// ══════════════════════════════════════════════════════════════════════════════

ImprimirSeccion("9. PREGUNTAS FRECUENTES");

Console.WriteLine("""
    Q: ¿Boxing existe en Java?
    A: Sí. Java hace autoboxing (int ↔ Integer) automático.
       En C# es más explícito: no hay clase contenedora para cada primitivo;
       boxing convierte directamente a object.

    Q: ¿Boxing existe en JavaScript/TypeScript?
    A: No exactamente igual. En JS los primitivos se "wrappean" temporalmente
       al llamar métodos (p.ej. (42).toString()), pero no hay boxing al heap
       gestionado como en .NET/Java.

    Q: ¿Cuánto cuesta un boxing?
    A: ~12-24 bytes en heap + el tiempo de asignación (~5-20 ns).
       Lo grave es la presión sobre el GC: 1M boxing/s generan basura constante.

    Q: ¿Puedo ver boxing en el IL generado?
    A: Sí. Instrucción IL: box [System.Runtime]System.Int32
       Unboxing: unbox.any [System.Runtime]System.Int32
       Usa: dotnet build && ildasm bin/Debug/net8.0/TuApp.dll

    Q: ¿Las tuplas (ValueTuple) causan boxing?
    A: NO. ValueTuple es un struct. Se almacenan en stack o inline.
       (int, string) tiene el string como referencia, pero la tupla es struct.

    Q: ¿Los records causan boxing?
    A: 'record' (class) → NO boxing, son referencia.
       'record struct' → como cualquier struct, boxing si se asigna a object/interfaz.

    Q: ¿string interpolation siempre evita boxing?
    A: En .NET 6+ con tipos que implementan ISpanFormattable, sí.
       Para tipos propios, implementar IFormattable o ISpanFormattable.

    ─────────────────────────────────────────────────────────────────────
    REGLA DE ORO:
    Si tu código hace operaciones intensivas con value types en bucles,
    EVITA cualquier asignación a object, interfaces no genéricas o
    colecciones de .NET 1.0 (ArrayList, Hashtable, Queue, Stack no genéricos).
    ─────────────────────────────────────────────────────────────────────
    """);

// ══════════════════════════════════════════════════════════════════════════════
// DIAGRAMA FINAL: CUÁNDO HAY BOXING (RESUMEN VISUAL)
// ══════════════════════════════════════════════════════════════════════════════

Console.WriteLine();
Console.WriteLine("""
    ┌──────────────────────────────────────────────────────────────────────┐
    │                    RESUMEN: ¿CUÁNDO HAY BOXING?                      │
    ├──────────────────────────────────────────────────────────────────────┤
    │ ✅ value type → object                          (boxing)             │
    │ ✅ value type → interfaz                        (boxing)             │
    │ ✅ value type → método con parámetro object     (boxing)             │
    │ ✅ value type en ArrayList/Hashtable            (boxing)             │
    │ ✅ value type → object, luego GetType()         (boxing)             │
    │ ✅ object → value type (cast explícito)         (unboxing)           │
    │                                                                      │
    │ ❌ value type → mismo tipo                       (sin boxing)        │
    │ ❌ List<T> con T = value type                   (sin boxing)        │
    │ ❌ Dictionary<K,V> con K,V = value types        (sin boxing)        │
    │ ❌ Métodos genéricos con where T : struct       (sin boxing)        │
    │ ❌ string interpolation ($"{valueType}")        (sin boxing en .NET8)│
    │ ❌ IEquatable<T>, IComparable<T> en structs     (sin boxing)        │
    └──────────────────────────────────────────────────────────────────────┘
    """);

Console.WriteLine("Programa finalizado. ¡Revisa la salida para entender cada caso de boxing!");

// ══════════════════════════════════════════════════════════════════════════════
// MÉTODOS AUXILIARES (miembros de la clase Program implícita)
// ══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Método que recibe object: cada vez que se pasa un value type aquí, ocurre boxing.
/// Esto es exactamente lo que NO debemos hacer en código de alto rendimiento.
/// </summary>
static void PrintObject(object obj) =>
    Console.WriteLine($"  PrintObject recibió: {obj} (tipo: {obj.GetType().Name})");

/// <summary>
/// Versión genérica del mismo método: CERO boxing.
/// El compilador genera una versión especializada para cada T, evitando el heap.
/// </summary>
static void PrintGeneric<T>(T value) =>
    Console.WriteLine($"  PrintGeneric recibió: {value} (tipo: {typeof(T).Name})");

/// <summary>
/// Mide y muestra el tiempo de ejecución de una acción, con etiqueta descriptiva.
/// </summary>
static void Medir(string descripcion, Action accion)
{
    Stopwatch sw = Stopwatch.StartNew();
    accion();
    sw.Stop();
    Console.WriteLine($"  {descripcion}: {sw.ElapsedMilliseconds} ms ({sw.ElapsedTicks} ticks)");
}

/// <summary>
/// Imprime una sección con formato destacado.
/// </summary>
static void ImprimirSeccion(string titulo)
{
    Console.WriteLine();
    Console.WriteLine(new string('─', 70));
    Console.WriteLine($"  {titulo}");
    Console.WriteLine(new string('─', 70));
}

// ══════════════════════════════════════════════════════════════════════════════
// TIPOS DE DATOS (deben ir al final en archivos con top-level statements)
// ══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Representa un punto 2D como struct (value type).
/// Implementa IEquatable&lt;Point&gt; para evitar boxing en comparaciones.
/// Sobrescribe ToString(), GetHashCode() y Equals() para evitar boxing.
/// </summary>
internal readonly struct Point(int x, int y) : IEquatable<Point>
{
    public int X { get; } = x;
    public int Y { get; } = y;

    public override bool Equals(object? obj) => obj is Point other && Equals(other);
    public bool Equals(Point other) => X == other.X && Y == other.Y;
    public override int GetHashCode() => HashCode.Combine(X, Y);
    public override string ToString() => $"({X}, {Y})";
}

/// <summary>
/// Representa una cantidad de dinero. Demuestra cuándo usar struct vs class.
/// Un struct es adecuado para valores pequeños, inmutables y con semántica de valor.
/// </summary>
internal readonly struct Money(decimal amount, string currency) : IEquatable<Money>, IComparable<Money>, IComparable
{
    public decimal Amount { get; } = amount;
    public string Currency { get; } = currency;

    public override bool Equals(object? obj) => obj is Money other && Equals(other);
    public bool Equals(Money other) => Amount == other.Amount && Currency == other.Currency;
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
    public override string ToString() => $"{Amount:F2} {Currency}";

    public int CompareTo(Money other)
    {
        int currencyCompare = string.Compare(Currency, other.Currency, StringComparison.Ordinal);
        return currencyCompare != 0 ? currencyCompare : Amount.CompareTo(other.Amount);
    }

    // IComparable (no genérico) — cada llamada causa boxing del argumento.
    // Lo incluimos explícitamente para demostrar el boxing.
    int IComparable.CompareTo(object? obj)
    {
        if (obj is null) return 1;
        return CompareTo((Money)obj); // unboxing del argumento
    }
}
