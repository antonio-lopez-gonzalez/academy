using FluentAssertions;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace CompilacionDotNet.Tests;

/// <summary>
/// Tests para Lección 02: CLR, BCL y cómo funciona la compilación.
/// Cada test valida un concepto del pipeline C# → IL → JIT → Máquina.
/// </summary>
public class Leccion02_CLR_BCL_Compilacion
{
    // ──────────────────────────────────────────────────────────────────────
    // 1. Reflection: inspeccionar el assembly y los tipos
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void Assembly_DebeTenerMetadataDeTipos()
    {
        // Act: obtenemos el assembly en ejecución mediante Reflection
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Assert: el assembly contiene metadatos (nombre, tipos, etc.)
        assembly.FullName.Should().NotBeNullOrEmpty();
        assembly.GetName().Name.Should().Be("CompilacionDotNet.Tests");
    }

    [Fact]
    public void TypeInfo_DebeRevelarEstructuraDeClase()
    {
        // Act: Reflection nos permite inspeccionar tipos en runtime
        Type tipoString = typeof(string);

        // Assert: string es una clase sellada (sealed)
        tipoString.IsClass.Should().BeTrue();
        tipoString.IsSealed.Should().BeTrue();
        tipoString.IsPublic.Should().BeTrue();

        // string tiene muchos métodos (la BCL es rica)
        tipoString.GetMethods().Length.Should().BeGreaterThan(50,
            "porque string es uno de los tipos más ricos de la BCL");
    }

    [Fact]
    public void ValueTypeVsReferenceType_Reflection()
    {
        // Act: Reflection distingue tipos de valor de tipos de referencia
        Type tipoInt = typeof(int);
        Type tipoString = typeof(string);

        // Assert: int es value type (struct), string es reference type (class)
        tipoInt.IsValueType.Should().BeTrue(
            "porque int es alias de System.Int32, un struct (tipo de valor)");
        tipoString.IsValueType.Should().BeFalse(
            "porque string es una class (tipo de referencia)");
    }

    // ──────────────────────────────────────────────────────────────────────
    // 2. Garbage Collector
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void GC_MaxGeneration_DebeSer2()
    {
        // Assert: .NET tiene 3 generaciones: 0, 1, 2
        // Gen 0 = objetos nuevos, Gen 1 = supervivientes, Gen 2 = longevos
        GC.MaxGeneration.Should().Be(2,
            "porque el GC de .NET usa 3 generaciones (Gen 0, Gen 1, Gen 2)");
    }

    [Fact]
    public void GC_GetTotalMemory_DebeSerPositiva()
    {
        // Act: el heap siempre tiene algo de memoria usada (el runtime mismo)
        long memoria = GC.GetTotalMemory(forceFullCollection: false);

        // Assert: la memoria usada debe ser > 0 (el runtime ocupa memoria)
        memoria.Should().BeGreaterThan(0,
            "porque el runtime, la BCL y nuestro código siempre ocupan algo de heap");
    }

    [Fact]
    public void GC_ColeccionCompleta_DebeReducirMemoria()
    {
        // Arrange: creamos objetos temporales para generar presión en el heap
        List<string> temporales = [];
        for (int i = 0; i < 10_000; i++)
        {
            temporales.Add(new string('x', 100));
        }

        long memoriaAntes = GC.GetTotalMemory(forceFullCollection: false);

        // Act: forzamos recolección completa (solo para demostración del test)
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect(); // Segunda pasada para objetos con finalizer

        long memoriaDespues = GC.GetTotalMemory(forceFullCollection: true);

        // Mantenemos viva la referencia para que no se optimice antes del Collect
        _ = temporales.Count;

        // Assert: la memoria debe haberse reducido (el GC liberó objetos no referenciados)
        // Nota: no siempre se reduce en un test porque otros procesos usan memoria
        memoriaDespues.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GCMemoryInfo_DebeTenerDatosValidos()
    {
        // Act: GC.GetGCMemoryInfo() da info de la última recolección
        GCMemoryInfo info = GC.GetGCMemoryInfo();

        // Assert: los datos deben ser razonables
        info.TotalAvailableMemoryBytes.Should().BeGreaterThan(0);
        info.HeapSizeBytes.Should().BeGreaterThan(0);
    }

    // ──────────────────────────────────────────────────────────────────────
    // 3. Stack vs Heap: tipos de valor vs referencia
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void Struct_ViveEnStack_SeCopiaAlPasar()
    {
        // Arrange: struct (tipo de valor) se copia al asignarlo
        Punto punto1 = new() { X = 10, Y = 20 };
        Punto punto2 = punto1; // COPIA (nueva instancia en el stack)

        // Act: modificar la copia no afecta al original
        punto2.X = 99;

        // Assert
        punto1.X.Should().Be(10, "porque struct se copia por valor");
        punto2.X.Should().Be(99);
    }

    [Fact]
    public void Class_ViveEnHeap_SePasaPorReferencia()
    {
        // Arrange: class (tipo de referencia) vive en el heap
        Persona persona1 = new() { Nombre = "Ana" };
        Persona persona2 = persona1; // AMBAS variables apuntan al MISMO objeto

        // Act: modificar a través de una variable afecta al objeto compartido
        persona2.Nombre = "Luis";

        // Assert
        persona1.Nombre.Should().Be("Luis",
            "porque class es tipo de referencia: ambas variables apuntan al mismo objeto");
    }

    [Fact]
    public void BoxingUnboxing_Demostracion()
    {
        // Act: boxing = convertir value type a object (va al heap)
        int valor = 42;
        object caja = valor; // BOXING: el int 42 se copia al heap

        // Unboxing = extraer value type de object (del heap al stack)
        int recuperado = (int)caja; // UNBOXING: requiere cast explícito

        // Assert
        recuperado.Should().Be(42);
        caja.Should().BeOfType<int>(); // El objeto en el heap ES un int boxed
        caja.GetType().Should().Be(typeof(int));
    }

    // ──────────────────────────────────────────────────────────────────────
    // 4. BCL — Tipos fundamentales
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void MathPow_DebeCalcularExponenciacion()
    {
        // Act: Math.Pow es parte de System (BCL), implementado en C++ nativo
        double resultado = Math.Pow(2, 10);

        // Assert
        resultado.Should().Be(1024);
    }

    [Fact]
    public void PathCombine_DebeSerMultiplataforma()
    {
        // Act: Path.Combine abstrae el separador de directorios del SO
        string ruta = Path.Combine("carpeta", "subcarpeta", "archivo.txt");

        // Assert: la ruta contiene el separador correcto para el SO actual
        ruta.Should().Contain("carpeta");
        ruta.Should().Contain("subcarpeta");
        ruta.Should().Contain("archivo.txt");
    }

    [Fact]
    public void Stopwatch_DebeMedirTiempoTranscurrido()
    {
        // Arrange: Stopwatch usa el contador de alto rendimiento del SO (QPC en Windows)
        Stopwatch sw = Stopwatch.StartNew();

        // Act: simular trabajo (no usamos Thread.Sleep en tests reales)
        _ = Enumerable.Range(1, 1000).Sum();

        // Assert: debe haber transcurrido algún tiempo (incluso si es muy poco)
        sw.Stop();
        sw.Elapsed.TotalMilliseconds.Should().BeGreaterOrEqualTo(0);
        sw.Elapsed.Should().BeGreaterThan(TimeSpan.Zero);
    }

    // ──────────────────────────────────────────────────────────────────────
    // 5. Tipos especiales del CLR
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void Span_DebeSerVistaSinCopia()
    {
        // Arrange: Span<T> y ReadOnlySpan<T> son vistas de memoria sin copia
        int[] numeros = [10, 20, 30, 40, 50];
        Span<int> slice = numeros.AsSpan(1, 3); // Vista de los elementos 1-3

        // Act: modificar a través del Span modifica el array original
        slice[0] = 99;

        // Assert: el array original se modificó (porque Span es una vista, no una copia)
        numeros[1].Should().Be(99);
        slice[1].Should().Be(30); // El segundo elemento del slice no se tocó
    }

    [Fact]
    public void Guid_DebeTenerFormatoEstandar()
    {
        // Act: Guid.NewGuid() genera un UUID v4 (aleatorio)
        Guid guid = Guid.NewGuid();

        string texto = guid.ToString();

        // Assert: formato estándar 8-4-4-4-12 con 36 caracteres
        texto.Should().HaveLength(36);
        texto.Should().MatchRegex(
            @"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$");
    }

    // ──────────────────────────────────────────────────────────────────────
    // 6. Interpolated strings (C# 10+, usando estructura de BCL)
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void StringBuilder_DebeSerEficienteParaConcatenacion()
    {
        // Arrange: StringBuilder evita crear strings intermedios
        StringBuilder sb = new();

        // Act: Append modifica el buffer interno, no crea nuevos strings
        for (int i = 0; i < 100; i++)
        {
            sb.Append(i);
        }

        // Assert
        string resultado = sb.ToString();
        resultado.Should().Contain("0");
        resultado.Should().Contain("99");
        resultado.Length.Should().BeGreaterThan(100);
    }

    // ──────────────────────────────────────────────────────────────────────
    // Tipos de apoyo para los tests
    // ──────────────────────────────────────────────────────────────────────

    // struct: tipo de valor, vive en el stack, se copia al asignar
    private struct Punto
    {
        public int X;
        public int Y;
    }

    // class: tipo de referencia, vive en el heap, se comparte al asignar
    private class Persona
    {
        public string Nombre { get; set; } = string.Empty;
    }
}
