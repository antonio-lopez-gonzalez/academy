// ====================================================================
// LECCIÓN 04-05 — Required members, init accessors, FrozenSet/Dictionary
// C# Avanzado — Inmutabilidad y colecciones de alto rendimiento
// ====================================================================
// ¿Qué cubre esta lección?
//   1. required (C# 11): Propiedades obligatorias en inicializadores de objeto
//   2. init accessor:   Asignable solo en inicializador o constructor
//   3. required + init: Objeto inmutable tras creación, campos obligatorios
//   4. FrozenSet<T>     (.NET 8): HashSet inmutable optimizado para búsqueda
//   5. FrozenDictionary  (.NET 8): Dictionary inmutable optimizado para búsqueda
//
// ¿Por qué existen?
//   - required evita objetos a medio inicializar (error común con new() y olvido de campos)
//   - init permite crear objetos inmutables sin constructores kilométricos
//   - FrozenSet/Dictionary hacen búsquedas más rápidas que HashSet/Dictionary
//     porque optimizan la estructura interna en tiempo de construcción
// ====================================================================

using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;

namespace CSharpRequired;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║   Required members, init, FrozenSet/Dictionary       ║");
        Console.WriteLine("║   Inmutabilidad + colecciones de alto rendimiento    ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine();

        DemostrarRequiredKeyword();
        DemostrarSetsRequiredMembers();
        DemostrarInitAccessor();
        DemostrarRequiredConInit();
        DemostrarFrozenSet();
        DemostrarFrozenDictionary();
        DemostrarBenchmarkFrozenVsNormal();
    }

    // ─── 1. La keyword `required` ─────────────────────────────────────
    // `required` fuerza a que la propiedad se inicialice en el momento
    // de creación del objeto. Si no lo haces, no compila.
    private static void DemostrarRequiredKeyword()
    {
        Console.WriteLine("─── 1. required keyword (C#11) ───");
        Console.WriteLine("Propiedades obligatorias en el inicializador de objeto\n");

        // ✅ OK: Todas las propiedades required están presentes
        Persona p1 = new Persona { Nombre = "Ana", Edad = 30 };
        Console.WriteLine($"  ✅ Persona creada: {p1.Nombre}, {p1.Edad} años");

        // ❌ Esto NO compilaría (descomenta para ver el error):
        // Persona p2 = new Persona(); // Error CS9035: Required member 'Persona.Nombre' must be set
        // Persona p3 = new Persona { Nombre = "Luis" }; // Error: falta 'Edad'

        Console.WriteLine("  ❌ new Persona() sin Nombre ni Edad → ERROR de compilación");
        Console.WriteLine("  ℹ️  required evita objetos a medio inicializar en tiempo de compilación\n");
    }

    // ─── 2. [SetsRequiredMembers] en constructores ────────────────────
    // Cuando tienes un constructor que inicializa TODOS los campos required,
    // puedes decorarlo con [SetsRequiredMembers] para que el compilador lo sepa.
    private static void DemostrarSetsRequiredMembers()
    {
        Console.WriteLine("─── 2. [SetsRequiredMembers] ───");
        Console.WriteLine("Delegar la inicialización al constructor\n");

        // ✅ El constructor establece todos los required, así que compila
        Producto p1 = new Producto("Teclado", 49.99m);
        Console.WriteLine($"  ✅ Producto: {p1.Nombre} - {p1.Precio:C}");

        // ✅ También puedes usar el inicializador si quieres (pero no es obligatorio)
        Producto p2 = new Producto("Ratón", 29.99m) { Sku = "MOU-001" };
        Console.WriteLine($"  ✅ Producto con SKU opcional: {p2.Nombre} - {p2.Precio:C} - SKU: {p2.Sku}");

        Console.WriteLine("  ℹ️  [SetsRequiredMembers] dice al compilador: 'este ctor lo inicializa todo'\n");
    }

    // ─── 3. init accessor ─────────────────────────────────────────────
    // `init` permite asignar la propiedad solo durante la inicialización
    // del objeto (constructor o inicializador). Después es de solo lectura.
    private static void DemostrarInitAccessor()
    {
        Console.WriteLine("─── 3. init accessor — asignación solo en creación ───\n");

        // ✅ Asignación en inicializador de objeto
        Configuracion cfg = new Configuracion { Tema = "oscuro", Idioma = "es" };
        Console.WriteLine($"  ✅ Configuración: tema={cfg.Tema}, idioma={cfg.Idioma}");

        // ❌ Esto NO compilaría (descomenta para ver el error):
        // cfg.Tema = "claro"; // Error CS8852: Init-only property can only be assigned in an initializer

        Console.WriteLine("  ❌ cfg.Tema = \"claro\" después de creado → ERROR de compilación");
        Console.WriteLine("  ℹ️  init = inmutable tras creación. Más seguro que set.\n");
    }

    // ─── 4. required + init = objeto inmutable obligatorio ────────────
    // La combinación perfecta: todos los campos obligatorios, y una vez
    // creado el objeto, no se puede modificar.
    private static void DemostrarRequiredConInit()
    {
        Console.WriteLine("─── 4. required + init = objeto inmutable con campos obligatorios ───\n");

        // ✅ Obligatorio rellenar todo en la creación. Después, inmutable.
        Direccion dir = new Direccion
        {
            Calle = "Gran Vía, 1",
            Ciudad = "Madrid",
            CodigoPostal = "28013"
        };
        Console.WriteLine($"  ✅ Dirección: {dir.Calle}, {dir.Ciudad}, {dir.CodigoPostal}");
        Console.WriteLine("  ℹ️  Una vez creada, no se puede modificar ninguna propiedad.");
        Console.WriteLine("  ℹ️  Patrón ideal para DTOs, Value Objects, mensajes, configuraciones.\n");
    }

    // ─── 5. FrozenSet<T> (.NET 8) ─────────────────────────────────────
    // FrozenSet es una colección inmutable de solo lectura que, en tiempo
    // de construcción, reorganiza sus datos internos para maximizar la
    // velocidad de búsqueda (Contains).
    // 
    // Internamente usa: hash perfecto minimal o árbol de decisión,
    // según el tamaño y características de los datos.
    private static void DemostrarFrozenSet()
    {
        Console.WriteLine("─── 5. FrozenSet<T> — HashSet inmutable ultra-rápido ───\n");

        // ── Creación ──
        // FrozenSet se crea desde un IEnumerable existente.
        // La creación es O(n log n) pero las búsquedas son O(1) optimizadas.
        HashSet<string> palabrasComunes = new(StringComparer.OrdinalIgnoreCase)
        {
            "the", "be", "to", "of", "and", "a", "in", "that", "have", "I",
            "it", "for", "not", "on", "with", "he", "as", "you", "do", "at"
        };

        FrozenSet<string> stopWords = palabrasComunes.ToFrozenSet();

        // ── Búsqueda ──
        string[] frasesEjemplo = { "the", "hello", "AND", "world", "To" };
        foreach (string palabra in frasesEjemplo)
        {
            bool esStopWord = stopWords.Contains(palabra);
            Console.WriteLine($"  ¿\"{palabra}\" es stop word? {esStopWord}");
        }

        Console.WriteLine($"\n  Conjunto creado con {stopWords.Count} elementos.");

        // ❌ No se puede modificar (es inmutable):
        // stopWords.Add("nuevo"); // No compila: FrozenSet no tiene Add

        Console.WriteLine("  ℹ️  FrozenSet optimiza la estructura interna al construirse.");
        Console.WriteLine("  ℹ️  Ideal para datos de referencia que se cargan una vez y se consultan mucho.\n");
    }

    // ─── 6. FrozenDictionary<TKey, TValue> (.NET 8) ───────────────────
    // Mismo concepto que FrozenSet pero para clave-valor.
    private static void DemostrarFrozenDictionary()
    {
        Console.WriteLine("─── 6. FrozenDictionary<TKey, TValue> ───\n");

        // ── Catálogo de productos: cargado al inicio, consultado miles de veces ──
        Dictionary<string, decimal> preciosIniciales = new()
        {
            ["Laptop"] = 999.99m,
            ["Monitor"] = 299.99m,
            ["Teclado"] = 79.99m,
            ["Ratón"] = 49.99m,
            ["Auriculares"] = 89.99m,
            ["Webcam"] = 59.99m,
            ["Hub USB"] = 29.99m,
            ["Cable HDMI"] = 14.99m,
        };

        // Construcción: lenta (una vez al inicio)
        FrozenDictionary<string, decimal> precios = preciosIniciales.ToFrozenDictionary();

        // Búsqueda: ultra-rápida (millones de veces durante la ejecución)
        string[] productosABuscar = { "Teclado", "Tablet", "Ratón", "Smartphone" };
        foreach (string producto in productosABuscar)
        {
            if (precios.TryGetValue(producto, out decimal precio))
                Console.WriteLine($"  ✅ {producto}: {precio:C}");
            else
                Console.WriteLine($"  ❌ {producto}: no encontrado en el catálogo");
        }

        Console.WriteLine("\n  ℹ️  Mismas ventajas que FrozenSet: inmutable + búsqueda optimizada.");
        Console.WriteLine("  ℹ️  El catálogo se congela al inicio y se consulta sin locks.\n");
    }

    // ─── 7. Benchmark: HashSet vs FrozenSet ───────────────────────────
    // Comparamos la velocidad de búsqueda entre HashSet y FrozenSet
    // con 100.000 elementos y 1.000.000 de consultas.
    private static void DemostrarBenchmarkFrozenVsNormal()
    {
        Console.WriteLine("─── 7. Benchmark: HashSet<T> vs FrozenSet<T> ───");
        Console.WriteLine("100.000 elementos, 1.000.000 búsquedas\n");

        // ── Preparar datos ──
        const int totalElementos = 100_000;
        const int totalConsultas = 1_000_000;

        HashSet<string> hashSet = new(totalElementos);
        for (int i = 0; i < totalElementos; i++)
        {
            hashSet.Add($"item-{i:D6}");
        }

        // Congelar: esta operación es costosa pero se hace UNA vez
        Stopwatch swCreacion = Stopwatch.StartNew();
        FrozenSet<string> frozenSet = hashSet.ToFrozenSet();
        swCreacion.Stop();

        Console.WriteLine($"  Creación del FrozenSet: {swCreacion.ElapsedMilliseconds}ms");
        Console.WriteLine($"  (se paga una vez al inicio, se amortiza con las consultas)\n");

        // ── Preparar consultas (mezcla de hits y misses) ──
        string[] consultas = new string[totalConsultas];
        Random rng = new(42); // Semilla fija para resultados reproducibles
        for (int i = 0; i < totalConsultas; i++)
        {
            // 70% hits, 30% misses (simula uso real)
            if (rng.NextDouble() < 0.7)
                consultas[i] = $"item-{rng.Next(totalElementos):D6}";
            else
                consultas[i] = $"missing-{rng.Next(totalElementos):D6}";
        }

        // ── Benchmark: HashSet ──
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Stopwatch swHashSet = Stopwatch.StartNew();
        int hitsHashSet = 0;
        foreach (string consulta in consultas)
        {
            if (hashSet.Contains(consulta))
                hitsHashSet++;
        }
        swHashSet.Stop();

        // ── Benchmark: FrozenSet ──
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Stopwatch swFrozenSet = Stopwatch.StartNew();
        int hitsFrozen = 0;
        foreach (string consulta in consultas)
        {
            if (frozenSet.Contains(consulta))
                hitsFrozen++;
        }
        swFrozenSet.Stop();

        // ── Resultados ──
        Console.WriteLine("  ┌────────────────────┬──────────────┬──────────┐");
        Console.WriteLine("  │ Colección          │ Tiempo (ms)  │ Hits     │");
        Console.WriteLine("  ├────────────────────┼──────────────┼──────────┤");
        Console.WriteLine($"  │ HashSet<T>         │ {swHashSet.ElapsedMilliseconds,10}   │ {hitsHashSet,8:N0} │");
        Console.WriteLine($"  │ FrozenSet<T>       │ {swFrozenSet.ElapsedMilliseconds,10}   │ {hitsFrozen,8:N0} │");
        Console.WriteLine("  └────────────────────┴──────────────┴──────────┘");

        double mejora = (double)(swHashSet.ElapsedMilliseconds - swFrozenSet.ElapsedMilliseconds)
                        / swHashSet.ElapsedMilliseconds * 100;
        Console.WriteLine($"\n  🚀 FrozenSet es ~{mejora:F1}% más rápido en búsqueda.");
        Console.WriteLine("  ℹ️  La mejora real varía según datos y hardware.");
        Console.WriteLine("  ℹ️  A mayor número de consultas, más se amortiza la creación.\n");
    }
}

// ====================================================================
// ENTIDADES DE EJEMPLO
// ====================================================================

// ── 1. Clase con required simple ────────────────────────────────────
// Sin required, podrías crear una Persona con Nombre null sin darte cuenta.
// Con required, el compilador te obliga a inicializar todo.
public class Persona
{
    // required = obligatorio inicializar al crear el objeto
    public required string Nombre { get; init; }
    public required int Edad { get; init; }

    // Opcional: no tiene required
    public string? Email { get; init; }
}

// ── 2. Clase con [SetsRequiredMembers] ──────────────────────────────
// El constructor inicializa todos los required, así que no hace falta
// usar el inicializador de objeto. Pero si quieres, puedes combinar ambos.
public class Producto
{
    public required string Nombre { get; init; }
    public required decimal Precio { get; init; }
    public string? Sku { get; init; } // Opcional, sin required

    // [SetsRequiredMembers] indica al compilador que este constructor
    // establece todos los miembros required de la clase.
    // Si el constructor no inicializa algún required, el compilador avisa.
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public Producto(string nombre, decimal precio)
    {
        Nombre = nombre;
        Precio = precio;
    }
}

// ── 3. Clase con init accessor ──────────────────────────────────────
// Las propiedades con init solo se pueden asignar en el inicializador
// de objeto o en el constructor. Después son de solo lectura.
public class Configuracion
{
    public string Tema { get; init; } = "claro";
    public string Idioma { get; init; } = "en";

    // Constructor por defecto que aplica valores iniciales
    public Configuracion() { }
}

// ── 4. Clase con required + init ────────────────────────────────────
// La combinación perfecta: todas las propiedades obligatorias y además
// inmutables tras la creación.
public class Direccion
{
    public required string Calle { get; init; }
    public required string Ciudad { get; init; }
    public required string CodigoPostal { get; init; }

    // Opcional: propiedad calculada de solo lectura (sin set ni init)
    public string DireccionCompleta => $"{Calle}, {Ciudad}, {CodigoPostal}";
}

// ====================================================================
// RESUMEN DE LA LECCIÓN
// ====================================================================
// 1. required (C# 11): Obliga a inicializar propiedades en el inicializador de objeto
//    - Error en tiempo de compilación si falta algún required
//    - Evita el clásico bug de objeto a medio inicializar
//
// 2. [SetsRequiredMembers]: Decora constructores que inicializan todos los required
//    - Permite new sin inicializador de objeto
//    - Si el ctor no cubre todo, warning/error del compilador
//
// 3. init accessor (C# 9): Solo asignable en inicializador o constructor
//    - Objeto inmutable después de creado
//    - Alternativa a constructores con N parámetros
//    - Seguro en entornos multihilo (no necesita locks para leer)
//
// 4. required + init = objeto inmutable con campos obligatorios
//    - Patrón ideal para DTOs, mensajes, eventos, Value Objects
//    - El compilador garantiza que nunca tendrás una instancia incompleta
//
// 5. FrozenSet<T> (.NET 8): HashSet inmutable optimizado para Contains()
//    - Construcción O(n log n) con optimizaciones internas
//    - Búsqueda O(1) más rápida que HashSet
//    - Usos: stop words, países, categorías, configuraciones
//
// 6. FrozenDictionary<TKey, TValue> (.NET 8): Dictionary inmutable optimizado
//    - Mismas ventajas que FrozenSet, para clave-valor
//    - Usos: catálogos, tablas de lookup, configuraciones clave-valor
//
// Cuándo usar cada uno:
//   - required + init: Siempre que modeles datos inmutables
//   - FrozenSet/Dictionary: Datos de referencia que se cargan al inicio
//     y se consultan frecuentemente durante la vida de la aplicación
//
// Cuándo NO usar:
//   - required con set: Pierde el sentido (si es mutable, usa set normal)
//   - FrozenSet/Dictionary con datos que cambian: Se recrea entero en cada cambio
//   - Frozen colecciones con pocos elementos: La diferencia es negligible
// ====================================================================
