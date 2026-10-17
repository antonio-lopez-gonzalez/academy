// ====================================================================
// LECCIÓN 04-01 — Span<T>, ReadOnlySpan<T>, Memory<T>
// C# Avanzado — Vistas sobre memoria sin copia
// ====================================================================
// ¿Qué es Span<T>?
//   Un tipo ref struct que representa una vista contigua sobre cualquier
//   tipo de memoria: arrays, stackalloc, memoria no gestionada, strings.
//   La clave: hacer slicing y manipulación SIN COPIAR datos.
//
// ¿Qué es Memory<T>?
//   Similar a Span pero NO es ref struct, así que puede vivir en el heap.
//   Se usa cuando necesitas pasar la vista a métodos async o almacenarla
//   en campos de clase.
// ====================================================================

using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace CSharpSpan;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("╔══════════════════════════════════════════════════╗");
        Console.WriteLine("║   Span<T>, ReadOnlySpan<T> y Memory<T>            ║");
        Console.WriteLine("║   Vistas sobre memoria contigua sin copia         ║");
        Console.WriteLine("╚══════════════════════════════════════════════════╝");
        Console.WriteLine();

        DemostrarSpanConArray();
        DemostrarSpanConString();
        DemostrarStackAlloc();
        DemostrarReadOnlySpanUtf8();
        DemostrarMemoryParaAsync();
        DemostrarParserCsv();
        DemostrarComparativaRendimiento();
    }

    // ─── 1. Span<T> sobre array: slicing sin copia ───────────────────────
    static void DemostrarSpanConArray()
    {
        Console.WriteLine("─── 1. Span<T> con arrays ───");
        Console.WriteLine("Slice sin crear nuevos arrays (sin copia de memoria)\n");

        int[] numbers = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

        // Crear un Span sobre el array entero
        Span<int> fullSpan = numbers.AsSpan();
        Console.WriteLine($"Array original: [{string.Join(", ", fullSpan.ToArray())}]");

        // Slice sin copia: vista desde índice 2, longitud 3
        // Equivalente a numbers[2..5] pero SIN crear nuevo array
        Span<int> slice = numbers.AsSpan(2, 3);
        Console.WriteLine($"Slice [2..5]:   [{string.Join(", ", slice.ToArray())}]");

        // Modificar a través del slice afecta al array original
        // (porque Span es una vista, no una copia)
        slice[1] = 999;
        Console.WriteLine($"  -> Después de slice[1] = 999:");
        Console.WriteLine($"     numbers[3] = {numbers[3]} (se modificó el original)");
        Console.WriteLine($"     slice[1]   = {slice[1]}\n");
    }

    // ─── 2. Span<char> sobre string ───────────────────────────────────────
    static void DemostrarSpanConString()
    {
        Console.WriteLine("─── 2. ReadOnlySpan<char> sobre strings ───");
        Console.WriteLine("Operaciones sobre string sin crear substrings\n");

        string texto = "Hola Mundo desde .NET 8";

        // Crear ReadOnlySpan<char> sobre el string (sin asignaciones)
        ReadOnlySpan<char> spanTexto = texto.AsSpan();

        // Slice de solo lectura: "Mundo" sin crear un nuevo string
        ReadOnlySpan<char> palabra = spanTexto.Slice(5, 5);
        Console.WriteLine($"Texto original:     \"{texto}\"");
        Console.WriteLine($"Slice [5..10]:      \"{new string(palabra)}\"");
        Console.WriteLine($"  (sin asignar un nuevo string para el substring)\n");

        // Búsqueda con Contains() sobre Span sin crear strings intermedios
        ReadOnlySpan<char> buscar = "Mundo".AsSpan();
        bool contiene = spanTexto.Contains(buscar, StringComparison.Ordinal);
        Console.WriteLine($"¿Contiene \"Mundo\"?: {contiene} (búsqueda sobre span, sin substrings)\n");

        // Trim sobre Span<char> (mutable vs readonly)
        char[] textoChars = "   espacios   ".ToCharArray();
        Span<char> spanChars = textoChars.AsSpan();
        spanChars = spanChars.Trim(); // Trim devuelve un nuevo Span apuntando al slice interno
        Console.WriteLine($"Trim sobre Span<char>: \"{new string(spanChars)}\"\n");
    }

    // ─── 3. stackalloc: reservar memoria en el stack ──────────────────────
    static void DemostrarStackAlloc()
    {
        Console.WriteLine("─── 3. stackalloc — memoria en el stack ───");
        Console.WriteLine("Evita la presión sobre el GC para buffers temporales\n");

        // Reservar 64 chars en el stack (no en el heap gestionado)
        // Límite típico: ~1MB por stack. No abuses.
        Span<char> buffer = stackalloc char[64];

        // Escribir en el buffer sin asignaciones en heap
        string nombre = "Antonio";
        int edad = 30;

        // TryFormat escribe en el Span sin concatenar strings
        // CultureInfo.InvariantCulture evita el formateo dependiente de cultura
        nombre.AsSpan().CopyTo(buffer);
        int charsEscritos = nombre.Length;

        buffer[charsEscritos] = ' ';
        charsEscritos++;

        edad.TryFormat(buffer.Slice(charsEscritos), out int escritos);
        charsEscritos += escritos;

        Console.WriteLine($"Buffer formateado: \"{new string(buffer.Slice(0, charsEscritos))}\"");
        Console.WriteLine($"  (0 asignaciones en el heap)\n");
    }

    // ─── 4. ReadOnlySpan<byte> con UTF-8 (C# 11+) ────────────────────────
    static void DemostrarReadOnlySpanUtf8()
    {
        Console.WriteLine("─── 4. UTF-8 literals con ReadOnlySpan<byte> ───");
        Console.WriteLine("Procesar texto sin decodificar a UTF-16\n");

        // Literales UTF-8 (C# 11): el sufijo "u8" crea ReadOnlySpan<byte>
        // sin asignación. Ideal para protocolos, headers HTTP, parsing binario.
        ReadOnlySpan<byte> utf8Header = "Content-Type"u8;
        ReadOnlySpan<byte> utf8Value = "application/json"u8;

        Console.WriteLine($"Header UTF-8:  {System.Text.Encoding.UTF8.GetString(utf8Header)}");
        Console.WriteLine($"Valor UTF-8:   {System.Text.Encoding.UTF8.GetString(utf8Value)}");
        Console.WriteLine($"Bytes del header: {utf8Header.Length} (no hay string UTF-16 intermedio)\n");
    }

    // ─── 5. Memory<T> para async ──────────────────────────────────────────
    static void DemostrarMemoryParaAsync()
    {
        Console.WriteLine("─── 5. Memory<T> para async ───");
        Console.WriteLine("Span no puede usarse en async; Memory<T> sí\n");

        // Span es ref struct, no puede vivir en el heap
        // Memory<T> es un struct normal, puede ser capturado por async state machine

        byte[] datos = new byte[] { 1, 2, 3, 4, 5 };
        Memory<byte> memory = datos.AsMemory();

        // Podemos pasar Memory<T> a métodos async
        Console.WriteLine("Memory<T> creado desde array de bytes.");
        Console.WriteLine($"Longitud: {memory.Length}");
        Console.WriteLine($"Primer byte (vía Span): {memory.Span[0]}\n");
    }

    // ─── 6. Ejemplo real: parser CSV con Span<T> ──────────────────────────
    static void DemostrarParserCsv()
    {
        Console.WriteLine("─── 6. Ejemplo real: Parser CSV sin strings intermedios ───\n");

        string csv = "Antonio;30;Madrid\nBeatriz;25;Barcelona\nCarlos;40;Valencia";
        Console.WriteLine($"CSV de entrada:\n{csv}\n");

        ParseCsv(csv);
    }

    /// <summary>
    /// Parsea líneas CSV separadas por ';' usando solo ReadOnlySpan<char>.
    /// 0 asignaciones de string por campo (salvo el ToString final para imprimir).
    /// </summary>
    static void ParseCsv(string csv)
    {
        ReadOnlySpan<char> texto = csv.AsSpan();
        int lineaNumero = 0;

        while (texto.Length > 0)
        {
            lineaNumero++;

            // Encontrar fin de línea
            int finLinea = texto.IndexOf('\n');
            ReadOnlySpan<char> linea;
            if (finLinea >= 0)
            {
                linea = texto.Slice(0, finLinea);
                texto = texto.Slice(finLinea + 1);
            }
            else
            {
                linea = texto;
                texto = ReadOnlySpan<char>.Empty;
            }

            // Quitar \r si existe (Windows line endings)
            if (linea.Length > 0 && linea[^1] == '\r')
                linea = linea.Slice(0, linea.Length - 1);

            // Parsear campos separados por ';'
            Console.WriteLine($"Línea {lineaNumero}:");
            ReadOnlySpan<char> restoLinea = linea;
            int campoNumero = 0;

            while (restoLinea.Length > 0)
            {
                campoNumero++;
                int finCampo = restoLinea.IndexOf(';');
                ReadOnlySpan<char> campo;

                if (finCampo >= 0)
                {
                    campo = restoLinea.Slice(0, finCampo);
                    restoLinea = restoLinea.Slice(finCampo + 1);
                }
                else
                {
                    campo = restoLinea;
                    restoLinea = ReadOnlySpan<char>.Empty;
                }

                Console.WriteLine($"  Campo {campoNumero}: \"{new string(campo)}\"");
            }
        }

        Console.WriteLine();
    }

    // ─── 7. Comparativa de rendimiento ────────────────────────────────────
    static void DemostrarComparativaRendimiento()
    {
        Console.WriteLine("─── 7. Comparativa: string.Split vs Span ───");

        // Generar datos de prueba: 10.000 líneas CSV
        string[] lineas = new string[10_000];
        for (int i = 0; i < lineas.Length; i++)
        {
            lineas[i] = $"Nombre{i};Apellido{i};{i * 10};ciudad{i}@email.com";
        }
        string csvGrande = string.Join('\n', lineas);

        // --- Método clásico con string.Split ---
        GC.Collect();
        GC.WaitForPendingFinalizers();
        long memoriaAntesSplit = GC.GetTotalMemory(forceFullCollection: true);

        Stopwatch sw = Stopwatch.StartNew();

        foreach (string linea in csvGrande.Split('\n'))
        {
            string[] campos = linea.Split(';');
            for (int j = 0; j < campos.Length; j++)
            {
                string valor = campos[j];
                // simular procesamiento
                int longitud = valor.Length;
            }
        }

        sw.Stop();
        long memoriaDespuesSplit = GC.GetTotalMemory(forceFullCollection: false);
        long tiempoSplit = sw.ElapsedMilliseconds;

        // --- Método con ReadOnlySpan<char> ---
        GC.Collect();
        GC.WaitForPendingFinalizers();
        long memoriaAntesSpan = GC.GetTotalMemory(forceFullCollection: true);

        sw.Restart();

        ReadOnlySpan<char> texto = csvGrande.AsSpan();
        while (texto.Length > 0)
        {
            int finLinea = texto.IndexOf('\n');
            ReadOnlySpan<char> linea;
            if (finLinea >= 0)
            {
                linea = texto.Slice(0, finLinea);
                texto = texto.Slice(finLinea + 1);
            }
            else
            {
                linea = texto;
                texto = ReadOnlySpan<char>.Empty;
            }

            if (linea.Length > 0 && linea[^1] == '\r')
                linea = linea.Slice(0, linea.Length - 1);

            ReadOnlySpan<char> resto = linea;
            while (resto.Length > 0)
            {
                int finCampo = resto.IndexOf(';');
                ReadOnlySpan<char> campo;
                if (finCampo >= 0)
                {
                    campo = resto.Slice(0, finCampo);
                    resto = resto.Slice(finCampo + 1);
                }
                else
                {
                    campo = resto;
                    resto = ReadOnlySpan<char>.Empty;
                }

                // simular procesamiento
                int longitud = campo.Length;
            }
        }

        sw.Stop();
        long memoriaDespuesSpan = GC.GetTotalMemory(forceFullCollection: false);
        long tiempoSpan = sw.ElapsedMilliseconds;

        Console.WriteLine($"\n  Resultados procesando {lineas.Length:N0} líneas CSV:");
        Console.WriteLine($"  ┌───────────────────┬─────────────┬──────────────────┐");
        Console.WriteLine($"  │ Método            │ Tiempo (ms) │ Memoria extra    │");
        Console.WriteLine($"  ├───────────────────┼─────────────┼──────────────────┤");
        Console.WriteLine($"  │ string.Split      │ {tiempoSplit,9}   │ {memoriaDespuesSplit - memoriaAntesSplit,14:N0} B │");
        Console.WriteLine($"  │ ReadOnlySpan<char>│ {tiempoSpan,9}   │ {memoriaDespuesSpan - memoriaAntesSpan,14:N0} B │");
        Console.WriteLine($"  └───────────────────┴─────────────┴──────────────────┘");

        Console.WriteLine("\n  ℹ️  Span no asigna strings intermedios → menos GC pressure.");
        Console.WriteLine("  ℹ️  En escenarios de alto rendimiento (servidores, parsing), la diferencia es notable.\n");
    }
}

// ====================================================================
// RESUMEN DE LA LECCIÓN
// ====================================================================
// 1. Span<T> = vista mutable sobre memoria contigua (array, stack, unmanaged)
// 2. ReadOnlySpan<T> = vista inmutable sobre memoria contigua
// 3. Memory<T> = igual que Span pero puede ir al heap (apto para async)
// 4. Slice no copia datos, solo ajusta puntero y longitud
// 5. stackalloc reserva en el stack (más rápido, sin GC, pero stack es pequeño)
// 6. Span es ref struct → no puede ser campo de clase, no puede boxearse, no va al heap
// 7. Memory<T>.Span obtiene el Span subyacente para operaciones síncronas
//
// Cuándo usar Span:
//   - Parsing de protocolos, archivos, datos binarios
//   - Manipulación de strings sin crear substrings
//   - Métodos de extensión sobre arrays/strings con alto rendimiento
//   - Librerías (el usuario de tu API no debería saber que usas Span)
//
// Cuándo NO usar Span:
//   - Necesitas almacenar la vista en un campo de clase o pasarla a async
//     → usa Memory<T>
//   - Código de negocio normal donde la claridad > rendimiento
//     → usa string/array normales
//   - El buffer vive en el stack y es grande (stack overflow)
//     → usa ArrayPool<T>.Shared.Rent()
// ====================================================================
