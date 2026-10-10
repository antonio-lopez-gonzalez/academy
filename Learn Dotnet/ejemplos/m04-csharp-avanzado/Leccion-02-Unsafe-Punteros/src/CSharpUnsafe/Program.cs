// ====================================================================
// LECCIÓN 04-02 — unsafe y punteros en C#
// C# Avanzado — Código no seguro, punteros, interoperabilidad nativa
// ====================================================================
// ¿Qué es unsafe?
//   C# es un lenguaje "memory-safe" por defecto. El runtime gestiona la memoria
//   y previene accesos fuera de rango, dangling pointers, etc.
//   Con `unsafe` puedes usar punteros como en C/C++: aritmética de punteros,
//   acceso directo a memoria, interoperabilidad con APIs nativas.
//   El precio: renuncias a la seguridad de memoria del runtime.
//
// ⚠️  ADVERTENCIA: unsafe NO es para uso diario. Solo para:
//   - Interoperabilidad con código nativo (C/C++/Rust via P/Invoke)
//   - Optimizaciones extremas donde cada nanosegundo cuenta
//   - Manipulación de bits y buffers que no se puede hacer con Span<T>
//
// Para usar unsafe necesitas:
//   <AllowUnsafeBlocks>true</AllowUnsafeBlocks> en el .csproj
// ====================================================================

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CSharpUnsafe;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("╔══════════════════════════════════════════════════╗");
        Console.WriteLine("║   unsafe y punteros en C#                         ║");
        Console.WriteLine("║   Acceso directo a memoria, interoperabilidad     ║");
        Console.WriteLine("╚══════════════════════════════════════════════════╝");
        Console.WriteLine();

        DemostrarPunterosBasicos();
        DemostrarFixedYArraysEnHeap();
        DemostrarInvertirArrayInPlace();
        DemostrarStackAllocConPunteros();
        DemostrarMarshalAllocHGlobal();
        DemostrarFunctionPointers();
        DemostrarComparativaRendimiento();
    }

    // ─── 1. Conceptos básicos de punteros ─────────────────────────────────
    static unsafe void DemostrarPunterosBasicos()
    {
        Console.WriteLine("─── 1. Punteros: &, *, -> ───");
        Console.WriteLine("Sintaxis idéntica a C: & toma dirección, * desreferencia\n");

        int x = 42;

        // &x  → obtiene la dirección de memoria de x
        // int* → puntero a entero (solo en contexto unsafe)
        int* p = &x;

        Console.WriteLine($"Valor de x:            {x}");
        Console.WriteLine($"Dirección de x (&x):   {(nint)p}");                // nint = IntPtr en modo nativo
        Console.WriteLine($"Valor a través de *p:  {*p}");                        // desreferenciar puntero

        // Modificar el valor a través del puntero (como en C)
        *p = 100;
        Console.WriteLine($"Después de *p = 100:   x = {x}\n");
    }

    // ─── 2. fixed: pinning de objetos en el heap ─────────────────────────
    static unsafe void DemostrarFixedYArraysEnHeap()
    {
        Console.WriteLine("─── 2. fixed y arrays en el heap ───");
        Console.WriteLine("fixed evita que el GC mueva el objeto mientras usamos punteros\n");

        int[] numeros = { 10, 20, 30, 40, 50 };

        // fixed pinnea el array para que el GC no lo mueva
        // Si el GC moviera el array, el puntero apuntaría a memoria inválida
        // fixed garantiza que el array se queda quieto dentro de su bloque
        fixed (int* ptr = numeros)
        {
            Console.WriteLine($"Primer elemento vía puntero: {*ptr}");
            Console.WriteLine($"Segundo elemento:             {*(ptr + 1)}");  // aritmética de punteros
            Console.WriteLine($"Tercer elemento:              {*(ptr + 2)}");

            // Modificar a través del puntero
            *(ptr + 3) = 999;
            Console.WriteLine($"numeros[3] después de modificar: {numeros[3]}");
        }

        Console.WriteLine();

        // fixed también funciona con strings
        string texto = "Hello unsafe!";
        fixed (char* ptrTexto = texto)
        {
            Console.WriteLine($"Primer char del string: '{*ptrTexto}'");
            Console.WriteLine($"Dirección del string:   {(nint)ptrTexto}");
        }

        Console.WriteLine();
    }

    // ─── 3. Ejemplo real: invertir array in-place con punteros ──────────
    static unsafe void DemostrarInvertirArrayInPlace()
    {
        Console.WriteLine("─── 3. Invertir array in-place con punteros ───\n");

        int[] datos = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        Console.WriteLine($"Original:  [{string.Join(", ", datos)}]");

        // Algoritmo clásico de inversión con dos punteros (inicio y final)
        ReverseArrayInPlace(datos);

        Console.WriteLine($"Invertido: [{string.Join(", ", datos)}]\n");
    }

    /// <summary>
    /// Invierte un array de enteros en su lugar usando punteros.
    /// Dos punteros: uno al inicio y otro al final, intercambian valores
    /// y se acercan hasta cruzarse.
    /// </summary>
    static unsafe void ReverseArrayInPlace(int[] array)
    {
        // fixed pinnea el array para que el GC no lo mueva
        fixed (int* ptr = array)
        {
            // Copiamos a variables locales porque las variables 'fixed' son inmutables
            int* inicio = ptr;
            int* final = ptr + array.Length - 1; // puntero al último elemento

            while (inicio < final)
            {
                // Intercambio clásico con punteros
                (*inicio, *final) = (*final, *inicio); // tuple swap también funciona con punteros

                inicio++;
                final--;
            }
        }
    }

    // ─── 4. stackalloc con punteros ──────────────────────────────────────
    static unsafe void DemostrarStackAllocConPunteros()
    {
        Console.WriteLine("─── 4. stackalloc con punteros ───");
        Console.WriteLine("Reserva en el stack acceso con aritmética de punteros\n");

        // Reservar 5 enteros en el stack (no en el heap, sin GC)
        const int cantidad = 5;
        int* buffer = stackalloc int[cantidad];

        // Inicializar con punteros
        for (int i = 0; i < cantidad; i++)
        {
            buffer[i] = (i + 1) * 10; // equivalente a *(buffer + i) = (i + 1) * 10
        }

        Console.WriteLine("Valores en el stackalloc:");
        for (int i = 0; i < cantidad; i++)
        {
            Console.WriteLine($"  buffer[{i}] = {buffer[i]} (dirección: {(nint)(buffer + i)})");
        }

        // Sumar todos los elementos recorriendo con puntero
        int suma = 0;
        int* ptrFin = buffer + cantidad;
        for (int* p = buffer; p < ptrFin; p++)
        {
            suma += *p;
        }
        Console.WriteLine($"  Suma recorriendo con puntero: {suma}");
        Console.WriteLine($"  Heap allocations: 0 (stackalloc + punteros)\n");
    }

    // ─── 5. Marshal: memoria no gestionada ────────────────────────────────
    static unsafe void DemostrarMarshalAllocHGlobal()
    {
        Console.WriteLine("─── 5. Marshal.AllocHGlobal / FreeHGlobal ───");
        Console.WriteLine("Memoria no gestionada (fuera del GC) para interoperabilidad con C/C++\n");

        // AllocHGlobal reserva memoria en el heap del proceso (no gestionado por .NET)
        // Útil para pasar buffers a APIs nativas (Win32, librerías C, etc.)
        const int size = 100;
        nint ptr = Marshal.AllocHGlobal(size);

        try
        {
            // Convertir IntPtr a puntero para manipular la memoria
            byte* p = (byte*)ptr;

            // Escribir patrón en la memoria no gestionada
            for (int i = 0; i < size; i++)
            {
                p[i] = (byte)(i % 256);
            }

            // Leer algunos valores
            Console.WriteLine($"Primeros 5 bytes: {p[0]}, {p[1]}, {p[2]}, {p[3]}, {p[4]}");
            Console.WriteLine($"Tamaño reservado: {size} bytes en heap no gestionado\n");
        }
        finally
        {
            // SIEMPRE liberar la memoria no gestionada (memory leak si no lo haces)
            Marshal.FreeHGlobal(ptr);
            Console.WriteLine("Memoria no gestionada liberada con Marshal.FreeHGlobal\n");
        }
    }

    // ─── 6. Function Pointers (C# 9+) ─────────────────────────────────────
    static unsafe void DemostrarFunctionPointers()
    {
        Console.WriteLine("─── 6. Function Pointers (delegate*<T, TResult>) ───");
        Console.WriteLine("Punteros a funciones al estilo C, más rápidos que delegates\n");

        // delegate*<int, int, int> = puntero a función que recibe 2 ints y devuelve int
        // &Suma = dirección del método estático Suma
        delegate*<int, int, int> ptrSuma = &Suma;
        delegate*<int, int, int> ptrMultiplica = &Multiplica;

        int a = 10;
        int b = 5;

        // Invocar puntero a función directamente (más rápido que delegate)
        int resultadoSuma = ptrSuma(a, b);
        int resultadoMulti = ptrMultiplica(a, b);

        Console.WriteLine($"ptrSuma({a}, {b})      = {resultadoSuma}");
        Console.WriteLine($"ptrMultiplica({a}, {b}) = {resultadoMulti}");

        // Function pointer almacenado en variable (como puntero normal)
        Console.WriteLine($"Dirección de Suma:      {(nint)ptrSuma}");
        Console.WriteLine($"Dirección de Multiplica: {(nint)ptrMultiplica}");

        Console.WriteLine("\n  ℹ️  Los function pointers evitan la indirección de los delegates.");
        Console.WriteLine("  ℹ️  Útiles para interoperabilidad con librerías nativas y callbacks rápidos.\n");
    }

    // Métodos estáticos usados como function pointers
    static int Suma(int x, int y) => x + y;
    static int Multiplica(int x, int y) => x * y;

    // ─── 7. Comparativa de rendimiento ────────────────────────────────────
    static unsafe void DemostrarComparativaRendimiento()
    {
        Console.WriteLine("─── 7. Comparativa: índices vs punteros ───");
        Console.WriteLine("Misma operación (suma de elementos) con y sin punteros\n");

        const int tamaño = 10_000_000;
        int[] datos = new int[tamaño];

        // Rellenar con valores
        for (int i = 0; i < datos.Length; i++)
        {
            datos[i] = i + 1;
        }

        // --- Método seguro: acceso por índice ---
        Stopwatch sw = Stopwatch.StartNew();
        long sumaIndice = 0;
        for (int i = 0; i < datos.Length; i++)
        {
            sumaIndice += datos[i];
        }
        sw.Stop();
        long tiempoIndice = sw.ElapsedMilliseconds;

        // --- Método unsafe: acceso por puntero ---
        sw.Restart();
        unsafe
        {
            long sumaPuntero = 0;
            fixed (int* p = datos)
            {
                int* fin = p + datos.Length;
                for (int* ptr = p; ptr < fin; ptr++)
                {
                    sumaPuntero += *ptr;
                }
            }

            sw.Stop();
            long tiempoPuntero = sw.ElapsedMilliseconds;

            Console.WriteLine($"  Array de {tamaño:N0} elementos");
            Console.WriteLine($"  Suma por índice:  {sumaIndice:N0} calculada en {tiempoIndice} ms");
            Console.WriteLine($"  Suma por puntero: {sumaPuntero:N0} calculada en {tiempoPuntero} ms");

            if (tiempoPuntero < tiempoIndice)
            {
                double mejora = (1.0 - (double)tiempoPuntero / tiempoIndice) * 100;
                Console.WriteLine($"  Mejora con puntero: ~{mejora:F1}%");
            }
        }

        Console.WriteLine();
        Console.WriteLine("  ⚠️  La diferencia es pequeña porque el JIT ya optimiza mucho el acceso por índice.");
        Console.WriteLine("  ⚠️  Solo usar unsafe cuando el profiling (BenchmarkDotNet) demuestre que merece la pena.");
        Console.WriteLine("  ⚠️  En la mayoría de aplicaciones web/API, unsafe NUNCA es necesario.\n");
    }
}

// ====================================================================
// RESUMEN DE LA LECCIÓN
// ====================================================================
// 1. unsafe permite usar punteros como en C: &, *, ->, aritmética de punteros
// 2. fixed "pinnea" objetos en el heap para que el GC no los mueva
// 3. stackalloc + punteros: buffers temporales sin presión de GC
// 4. Marshal.AllocHGlobal: memoria no gestionada (libera con FreeHGlobal)
// 5. Function pointers (C# 9+): delegate*<T, TResult> más rápido que delegates
//
// Cuándo usar unsafe:
//   - P/Invoke e interoperabilidad con C/C++/Rust
//   - Procesamiento de imágenes, audio, video (píxeles, frames)
//   - Implementación de estructuras de datos de alto rendimiento
//   - Serialización/deserialización binaria custom
//
// Cuándo NO usar unsafe:
//   - Código de negocio normal (usa Span<T>, arrays e índices)
//   - Micro-optimizaciones sin profiling previo
//   - Si no sabes exactamente lo que haces con la memoria
//
// Seguridad:
//   - unsafe puede causar AccessViolationException, corrupción de memoria
//   - Siempre encapsula el código unsafe en APIs seguras
//   - Añade tests exhaustivos al código unsafe
// ====================================================================
