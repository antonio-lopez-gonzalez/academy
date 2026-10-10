// ====================================================================
// LECCIÓN 04-03 — ref struct, ref return, in parameters
// C# Avanzado — Structs en el stack, referencias y paso eficiente
// ====================================================================
// ¿Qué es ref struct?
//   Un struct que SOLO puede vivir en el stack. El compilador garantiza que:
//   - No puede ser campo de una clase
//   - No puede ser boxeado (convertido a object)
//   - No puede ser elemento de un array
//   - No puede ser capturado por una lambda o async method
//   - No puede implementar interfaces
//   Span<T> es el ejemplo más famoso de ref struct.
//
// ¿Qué es ref return?
//   Un método puede devolver una REFERENCIA a una variable, no una copia.
//   Permite modificar el valor original sin copiarlo.
//
// ¿Qué son in parameters?
//   Paso por referencia de SOLO LECTURA. No se copia el struct, pero tampoco
//   se puede modificar. Ideal para structs grandes pasados a métodos.
//
// ¿Qué es ref readonly?
//   Devuelve una referencia de solo lectura. El llamante no puede modificar.
// ====================================================================

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CSharpRefStruct;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("╔══════════════════════════════════════════════════╗");
        Console.WriteLine("║   ref struct, ref return, in parameters           ║");
        Console.WriteLine("║   Structs en stack, referencias y paso eficiente  ║");
        Console.WriteLine("╚══════════════════════════════════════════════════╝");
        Console.WriteLine();

        DemostrarRefStruct();
        DemostrarRefReturn();
        DemostrarInParameters();
        DemostrarRefReadonly();
        DemostrarEnumeratorSobreSpan();
        DemostrarComparativaRendimiento();
    }

    // ─── 1. ref struct: solo en el stack ──────────────────────────────────
    static void DemostrarRefStruct()
    {
        Console.WriteLine("─── 1. ref struct — solo en el stack ───");
        Console.WriteLine("El compilador garantiza que no escape al heap\n");

        // RefPoint es un ref struct, puede crearse como variable local
        RefPoint punto = new RefPoint(10, 20);

        Console.WriteLine($"RefPoint creado en el stack: X={punto.X}, Y={punto.Y}");

        // Span<T> es el ref struct más usado
        Span<int> numeros = stackalloc int[] { 1, 2, 3, 4, 5 };
        Console.WriteLine($"Span<int> (ref struct): [{string.Join(", ", numeros.ToArray())}]");

        // ── LO QUE NO SE PUEDE HACER ──
        // RefPoint[] array = new RefPoint[1];          // ❌ ref struct no puede ser elemento de array
        // object box = punto;                           // ❌ ref struct no puede boxearse
        // List<RefPoint> lista = new List<RefPoint>();  // ❌ ref struct no puede ser T genérico
        // Action a = () => Console.WriteLine(punto);    // ❌ ref struct no puede ser capturado por lambda

        Console.WriteLine();
        Console.WriteLine("  ℹ️  Restricciones de ref struct (el compilador las impone):");
        Console.WriteLine("  ❌ No puede ser campo de una clase");
        Console.WriteLine("  ❌ No puede ser elemento de un array");
        Console.WriteLine("  ❌ No puede ser boxeado (convertido a object)");
        Console.WriteLine("  ❌ No puede ser parámetro de tipo genérico");
        Console.WriteLine("  ❌ No puede ser capturado por lambda o async method");
        Console.WriteLine("  ❌ No puede implementar interfaces (hasta C# 13)\n");
    }

    // ─── 2. ref return ────────────────────────────────────────────────────
    static void DemostrarRefReturn()
    {
        Console.WriteLine("─── 2. ref return — devolver referencia, no copia ───\n");

        int[] valores = { 100, 200, 300, 400, 500 };

        Console.WriteLine($"Array original: [{string.Join(", ", valores)}]");

        // Obtener referencia al elemento índice 2
        // Sin ref:         int   x = GetByIndex(valores, 2); → copia el valor 300
        // Con ref: ref int x = ref GetByIndex(valores, 2);  → referencia al slot del array
        ref int referencia = ref ObtenerPorIndice(valores, 2);

        Console.WriteLine($"Antes:  valores[2] = {valores[2]}");

        referencia = 999; // Modifica el array original (no una copia)

        Console.WriteLine($"Después: valores[2] = {valores[2]} (modificado vía ref return)");
        Console.WriteLine($"Array final:   [{string.Join(", ", valores)}]\n");

        // Otro ejemplo: buscar y modificar
        ref int encontrado = ref BuscarYModificar(valores, 200);
        encontrado = 777;

        Console.WriteLine($"Después de BuscarYModificar: [{string.Join(", ", valores)}]\n");
    }

    /// <summary>
    /// Devuelve una REFERENCIA al elemento en el índice especificado.
    /// El llamante puede leer Y escribir en el array original sin copias.
    /// </summary>
    static ref int ObtenerPorIndice(int[] array, int indice)
    {
        return ref array[indice]; // ref return: devuelve referencia, no copia
    }

    /// <summary>
    /// Busca un valor en el array y devuelve referencia a su posición.
    /// Si no lo encuentra, devuelve referencia al primer elemento (como fallback).
    /// </summary>
    static ref int BuscarYModificar(int[] array, int buscar)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == buscar)
                return ref array[i];
        }

        // Si no se encuentra, devolvemos referencia al primer elemento
        // En código real, lanzarías excepción o usarías un patrón diferente
        return ref array[0];
    }

    // ─── 3. in parameters ─────────────────────────────────────────────────
    static void DemostrarInParameters()
    {
        Console.WriteLine("─── 3. in parameters — paso por referencia de solo lectura ───\n");

        // Creamos un struct grande
        LargeStruct datos = new LargeStruct(
            Id: 1,
            Name: "Procesamiento de datos",
            X: 100.0, Y: 200.0, Z: 300.0,
            R: 255, G: 128, B: 64, A: 255,
            Flag1: true, Flag2: false, Flag3: true, Flag4: false
        );

        // in: pasa por referencia (no copia) pero no permite modificar
        double resultado = ProcesarLargeStruct(in datos);

        Console.WriteLine($"  Struct original: {datos}");
        Console.WriteLine($"  Resultado del procesamiento: {resultado:F2}");

        // Intentar modificar dentro de ProcesarLargeStruct daría error de compilación:
        // datos.X = 999; // ❌ CS8332: Cannot assign to a member of
        //                //    variable 'in LargeStruct' because it is a readonly variable

        Console.WriteLine();
        Console.WriteLine("  ℹ️  'in' evita la copia del struct (eficiencia) y evita modificarlo (seguridad)");
        Console.WriteLine("  ℹ️  El compilador emite CS8332 si intentas modificar un parámetro 'in'\n");
    }

    static double ProcesarLargeStruct(in LargeStruct datos)
    {
        // datos es readonly, no podemos modificarlo
        return Math.Sqrt(datos.X * datos.X + datos.Y * datos.Y + datos.Z * datos.Z);
    }

    // ─── 4. ref readonly ──────────────────────────────────────────────────
    static void DemostrarRefReadonly()
    {
        Console.WriteLine("─── 4. ref readonly — referencia de solo lectura ───\n");

        int[] valores = { 10, 20, 30, 40, 50 };

        // ref readonly: obtiene referencia pero no permite modificar
        ref readonly int soloLectura = ref ObtenerSoloLectura(valores, 3);

        Console.WriteLine($"Valor vía ref readonly: {soloLectura}");
        Console.WriteLine($"  valores[3] = {valores[3]}");

        // soloLectura = 999; // ❌ No compila: readonly reference

        Console.WriteLine("\n  ℹ️  ref readonly permite acceder sin copiar y sin riesgo de modificación");
        Console.WriteLine("  ℹ️  Útil para exponer datos internos protegidos de colecciones\n");
    }

    static ref readonly int ObtenerSoloLectura(int[] array, int indice)
    {
        return ref array[indice]; // ref readonly return
    }

    // ─── 5. Ejemplo real: ref struct Enumerator<T> ────────────────────────
    static void DemostrarEnumeratorSobreSpan()
    {
        Console.WriteLine("─── 5. Ejemplo real: SpanEnumerator<T> (ref struct) ───");
        Console.WriteLine("Iterador sobre Span<T> sin asignaciones\n");

        Span<int> datos = stackalloc int[] { 10, 20, 30, 40, 50 };

        Console.Write("Iteración con SpanEnumerator: ");
        SpanEnumerator<int> enumerador = new SpanEnumerator<int>(datos);
        while (enumerador.MoveNext())
        {
            // Current es ref readonly, accedemos sin copiar
            Console.Write($"{enumerador.Current} ");
        }
        Console.WriteLine();

        // También podemos modificar a través del enumerador (versión mutable)
        SpanEnumeratorMutable<int> enumeradorMut = new SpanEnumeratorMutable<int>(datos);
        while (enumeradorMut.MoveNext())
        {
            enumeradorMut.CurrentRef = enumeradorMut.Current * 2;
        }
        Console.WriteLine($"Duplicados: [{string.Join(", ", datos.ToArray())}]\n");
    }

    // ─── 6. Comparativa de rendimiento ────────────────────────────────────
    static void DemostrarComparativaRendimiento()
    {
        Console.WriteLine("─── 6. Comparativa: paso por valor vs por 'in' ───\n");

        const int iteraciones = 10_000_000;

        LargeStruct datos = new LargeStruct(
            Id: 1,
            Name: "Benchmark",
            X: 100.0, Y: 200.0, Z: 300.0,
            R: 255, G: 128, B: 64, A: 255,
            Flag1: true, Flag2: false, Flag3: true, Flag4: false
        );

        // --- Paso por valor (copia todo el struct cada llamada) ---
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Stopwatch sw = Stopwatch.StartNew();
        double totalValor = 0;
        for (int i = 0; i < iteraciones; i++)
        {
            totalValor += ProcesarPorValor(datos); // Copia 64+ bytes por llamada
        }
        sw.Stop();
        long tiempoValor = sw.ElapsedMilliseconds;

        // --- Paso por in (referencia de solo lectura, sin copia) ---
        GC.Collect();
        GC.WaitForPendingFinalizers();

        sw.Restart();
        double totalIn = 0;
        for (int i = 0; i < iteraciones; i++)
        {
            totalIn += ProcesarPorIn(in datos); // Pasa puntero (8 bytes), no copia
        }
        sw.Stop();
        long tiempoIn = sw.ElapsedMilliseconds;

        Console.WriteLine($"  {iteraciones:N0} iteraciones con LargeStruct (~80 bytes)");
        Console.WriteLine($"  ┌─────────────────────┬──────────────┐");
        Console.WriteLine($"  │ Método              │ Tiempo (ms)  │");
        Console.WriteLine($"  ├─────────────────────┼──────────────┤");
        Console.WriteLine($"  │ Por valor (copia)   │ {tiempoValor,9}     │");
        Console.WriteLine($"  │ Por 'in' (ref)      │ {tiempoIn,9}     │");
        Console.WriteLine($"  └─────────────────────┴──────────────┘");

        if (tiempoIn < tiempoValor)
        {
            double mejora = (1.0 - (double)tiempoIn / tiempoValor) * 100;
            Console.WriteLine($"  Mejora con 'in': ~{mejora:F1}%");
        }

        Console.WriteLine();
        Console.WriteLine("  ℹ️  'in' brilla con structs grandes (>16 bytes) en bucles intensivos.");
        Console.WriteLine("  ℹ️  Con structs pequeños (<=16 bytes), el JIT suele pasarlos en registros.");
        Console.WriteLine("  ℹ️  Regla general: > 16 bytes → considerar 'in', > 64 bytes → usar 'in'\n");
    }

    static double ProcesarPorValor(LargeStruct datos)
    {
        return Math.Sqrt(datos.X * datos.X + datos.Y * datos.Y + datos.Z * datos.Z);
    }

    static double ProcesarPorIn(in LargeStruct datos)
    {
        return Math.Sqrt(datos.X * datos.X + datos.Y * datos.Y + datos.Z * datos.Z);
    }
}

// ════════════════════════════════════════════════════════════════════
// TIPOS DE DEMOSTRACIÓN
// ════════════════════════════════════════════════════════════════════

/// <summary>
/// ref struct: solo puede vivir en el stack.
/// El compilador impide que se escape al heap.
/// </summary>
ref struct RefPoint
{
    public int X;
    public int Y;

    public RefPoint(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X}, {Y})";
}

/// <summary>
/// Struct "grande" para demostrar la diferencia entre paso por valor y por 'in'.
/// Con 12 campos (~80 bytes), copiarlo en cada llamada tiene coste real.
/// </summary>
readonly struct LargeStruct
{
    public readonly int Id;
    public readonly string Name;   // referencia (8 bytes en x64), no se copia el string solo el puntero
    public readonly double X;
    public readonly double Y;
    public readonly double Z;
    public readonly byte R;
    public readonly byte G;
    public readonly byte B;
    public readonly byte A;
    public readonly bool Flag1;
    public readonly bool Flag2;
    public readonly bool Flag3;
    public readonly bool Flag4;

    public LargeStruct(int Id, string Name, double X, double Y, double Z,
                       byte R, byte G, byte B, byte A,
                       bool Flag1, bool Flag2, bool Flag3, bool Flag4)
    {
        this.Id = Id;
        this.Name = Name;
        this.X = X;
        this.Y = Y;
        this.Z = Z;
        this.R = R;
        this.G = G;
        this.B = B;
        this.A = A;
        this.Flag1 = Flag1;
        this.Flag2 = Flag2;
        this.Flag3 = Flag3;
        this.Flag4 = Flag4;
    }

    public override string ToString() =>
        $"LargeStruct(Id={Id}, Name={Name}, X={X}, Y={Y}, Z={Z})";
}

/// <summary>
/// Enumerator de solo lectura sobre Span<T>.
/// ref struct porque contiene un Span<T> (que es ref struct).
/// 0 asignaciones en el heap.
/// </summary>
ref struct SpanEnumerator<T>
{
    private readonly ReadOnlySpan<T> _span; // Span sobre los datos
    private int _index;                      // posición actual

    public SpanEnumerator(Span<T> span)
    {
        _span = span;
        _index = -1; // antes del primer elemento
    }

    /// <summary>
    /// Avanza al siguiente elemento. Devuelve false si llegó al final.
    /// </summary>
    public bool MoveNext()
    {
        _index++;
        return _index < _span.Length;
    }

    /// <summary>
    /// Elemento actual (solo lectura, sin copia).
    /// </summary>
    public readonly T Current => _span[_index];

    /// <summary>
    /// Reinicia el enumerador al principio.
    /// </summary>
    public void Reset()
    {
        _index = -1;
    }
}

/// <summary>
/// Enumerator mutable sobre Span<T>.
/// Permite modificar los elementos del Span a través de CurrentRef.
/// También ref struct por contener un Span<T>.
/// </summary>
ref struct SpanEnumeratorMutable<T>
{
    private Span<T> _span;
    private int _index;

    public SpanEnumeratorMutable(Span<T> span)
    {
        _span = span;
        _index = -1;
    }

    public bool MoveNext()
    {
        _index++;
        return _index < _span.Length;
    }

    /// <summary>
    /// Devuelve referencia de solo lectura al elemento actual.
    /// </summary>
    public readonly T Current => _span[_index];

    /// <summary>
    /// Devuelve referencia mutable al elemento actual para poder modificarlo.
    /// </summary>
    public ref T CurrentRef => ref _span[_index];

    public void Reset()
    {
        _index = -1;
    }
}

// ====================================================================
// RESUMEN DE LA LECCIÓN
// ====================================================================
// 1. ref struct: struct que solo puede vivir en el stack (Span<T> es el ejemplo canónico)
//    Restricciones: no campo de clase, no array, no boxing, no lambda capture, no genéricos
// 2. ref return: método devuelve referencia a variable existente, no una copia
//    Permite modificar el valor original desde fuera
// 3. in parameter: paso por referencia de solo lectura
//    Sin copia (eficiencia) + sin modificación (seguridad)
// 4. ref parameter: paso por referencia de lectura/escritura
// 5. ref readonly: devuelve referencia de solo lectura (no permite modificar)
//
// Cuándo usar cada uno:
//   ref struct  → Cuando necesitas un tipo que contenga Span<T> o referencias gestionadas
//             → Iteradores, enumeradores, parsers de alto rendimiento
//   ref return  → Acceso directo a slots de arrays/colecciones sin copia
//             → Métodos como GetByIndex, Find en colecciones de structs grandes
//   in param    → Structs > 16-24 bytes en métodos llamados frecuentemente
//             → APIs que reciben structs grandes y no necesitan modificarlos
//   ref param   → Métodos que necesitan modificar el struct original del llamante
//
// Cuándo NO usar:
//   - ref struct para tipos que necesitan ir en el heap (usa struct normal)
//   - ref return si el llamante no necesita modificar (usa ref readonly)
//   - in parameter con structs pequeños (< 16 bytes): el JIT optimiza mejor por valor
//   - 'in' con tipos de referencia (class): las referencias ya se pasan por valor
// ====================================================================
