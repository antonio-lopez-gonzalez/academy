// ──────────────────────────────────────────────────────────────────────────
// Tests para Lección 01 — ¿Qué es .NET? Historia y ecosistema
// ──────────────────────────────────────────────────────────────────────────
// Convención de nombres: [Método]_[Escenario]_[ResultadoEsperado]
// Framework: xUnit + FluentAssertions + Moq
// ──────────────────────────────────────────────────────────────────────────

using FluentAssertions;
using System.Runtime.InteropServices;

namespace IntroduccionDotNet.Tests;

/// <summary>
/// Tests que verifican conceptos fundamentales de .NET presentados en la Lección 01.
/// Cada test valida un concepto explicado en el README y el código de ejemplo.
/// </summary>
public class Leccion01_FundamentosDotNet
{
    // ──────────────────────────────────────────────────────────────────────
    // 1. Información del entorno
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void RuntimeVersion_DebeSerNet8OSuperior()
    {
        // Arrange: obtenemos la versión real del runtime en ejecución
        Version version = Environment.Version;

        // Assert: .NET 8 tiene número de versión mayor o igual a 8.0
        // Environment.Version en .NET 8 devuelve 8.0.X
        version.Major.Should().BeGreaterOrEqualTo(8,
            "porque el curso exige .NET 8 o superior como mínimo");
    }

    [Fact]
    public void OSPlatform_DebeSerIdentificable()
    {
        // Arrange/Act: verificamos en qué SO estamos ejecutando
        bool esWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        bool esLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        bool esMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        // Assert: al menos un SO debe ser true, el resto false
        (esWindows || esLinux || esMac).Should().BeTrue(
            "porque .NET se ejecuta en al menos un sistema operativo válido");

        // Solo uno debe ser true (no puedes estar en Windows y Linux a la vez)
        new[] { esWindows, esLinux, esMac }
            .Count(x => x)
            .Should()
            .Be(1, "porque solo se ejecuta en un sistema operativo a la vez");
    }

    // ──────────────────────────────────────────────────────────────────────
    // 2. BCL — Base Class Library
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void PathCombine_DebeGenerarRutaMultiplataforma()
    {
        // Act: Path.Combine abstrae el separador de ruta del SO
        string ruta = Path.Combine("home", "usuario", "docs", "archivo.md");

        // Assert: la ruta contiene los segmentos y el separador correcto
        ruta.Should().Contain("home");
        ruta.Should().Contain("usuario");
        ruta.Should().Contain("docs");
        ruta.Should().Contain("archivo.md");
        ruta.Should().NotBeEmpty();
    }

    [Fact]
    public void GuidNewGuid_DebeGenerarValorUnico()
    {
        // Act: generamos 100 GUIDs y verificamos que todos son únicos
        HashSet<Guid> guids = [];
        for (int i = 0; i < 100; i++)
        {
            guids.Add(Guid.NewGuid());
        }

        // Assert: 100 iteraciones deben producir 100 GUIDs únicos
        guids.Should().HaveCount(100,
            "porque cada Guid.NewGuid() genera un valor aleatorio único");
    }

    [Fact]
    public void GuidEmpty_DebeSerTodoCeros()
    {
        // Act: Guid.Empty es el valor por defecto (todo ceros)
        Guid vacio = Guid.Empty;

        // Assert: Guid.Empty es equivalente a new Guid() (todo 0s)
        vacio.ToString().Should().Be("00000000-0000-0000-0000-000000000000");
        vacio.Should().Be(Guid.Empty);
        vacio.Should().Be(new Guid()); // Constructor por defecto también es Empty
    }

    [Fact]
    public void TimeProviderSystem_DebeDevolverTiempoRazonable()
    {
        // Arrange: TimeProvider.System usa el reloj real del sistema
        TimeProvider provider = TimeProvider.System;

        // Act: obtenemos la hora actual
        DateTimeOffset ahora = provider.GetUtcNow();

        // Assert: debe ser un año razonable (> 2023, fecha de lanzamiento de .NET 8)
        ahora.Year.Should().BeGreaterOrEqualTo(2024,
            "porque .NET 8 se lanzó en noviembre de 2023");
        ahora.Offset.Should().Be(TimeSpan.Zero,
            "porque GetUtcNow() siempre devuelve offset 0 (UTC)");
    }

    // ──────────────────────────────────────────────────────────────────────
    // 3. Tipado estático y fuerte
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void Var_DebeInferirTipoEnCompilacion_NoEnEjecucion()
    {
        // Act: var infiere el tipo en compilación, NO es dinámico como en JS
        var numero = 42;
        var texto = "Hola .NET";

        // Assert: el tipo se determina en compilación y no cambia
        numero.GetType().Should().Be(typeof(int),
            "porque el literal 42 infiere el tipo int en compilación");
        texto.GetType().Should().Be(typeof(string),
            "porque las comillas dobles infieren string en compilación");
    }

    [Fact]
    public void TiposPorValor_DebenCopiarse_NoReferenciarse()
    {
        // Act: los tipos de valor (int, struct, bool, etc.) se copian
        int original = 10;
        int copia = original;  // Copia el valor, no la referencia

        copia = 20; // Modificamos la copia

        // Assert: el original NO cambia (a diferencia de los tipos de referencia)
        original.Should().Be(10, "porque int es un tipo de valor y se copia");
        copia.Should().Be(20);
    }

    [Fact]
    public void TiposPorReferencia_DebenCompartirReferencia()
    {
        // Act: los tipos de referencia (class) comparten la misma instancia
        int[] original = [1, 2, 3];
        int[] referencia = original; // Ambas variables apuntan al MISMO array

        referencia[0] = 99; // Modificamos a través de una variable

        // Assert: el original TAMBIÉN cambia, porque es la misma instancia
        original[0].Should().Be(99,
            "porque los arrays son tipos de referencia y ambas variables apuntan al mismo objeto");
    }

    // ──────────────────────────────────────────────────────────────────────
    // 4. Nullable Reference Types (NRT)
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void StringNulleable_ConValorNulo_LongitudDebeSerNull()
    {
        // Arrange: string? declara explícitamente que puede ser null
        string? valor = null;

        // Act: el operador null-conditional (?.) devuelve null si el operando es null
        int? longitud = valor?.Length;

        // Assert: la longitud es null porque el string era null
        longitud.Should().BeNull(
            "porque el operador ?. propaga el null en lugar de lanzar NullReferenceException");
    }

    [Fact]
    public void NullCoalescingOperator_DebeProveerValorPorDefecto()
    {
        // Arrange: string? nulleable
        string? valorNulo = null;
        string? valorValido = "Hola";

        // Act: ?? proporciona un valor por defecto si la expresión es null
        int longitudNulo = valorNulo?.Length ?? -1;
        int longitudValido = valorValido?.Length ?? -1;

        // Assert
        longitudNulo.Should().Be(-1,
            "porque ?? devuelve el valor por defecto cuando la expresión es null");
        longitudValido.Should().Be(4,
            "porque ?? devuelve el valor de la izquierda cuando NO es null");
    }

    // ──────────────────────────────────────────────────────────────────────
    // 5. LINQ
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void LinqWhere_DebeFiltrarCorrectamente()
    {
        // Arrange
        int[] numeros = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        // Act: LINQ con lambda expression
        int[] pares = numeros.Where(n => n % 2 == 0).ToArray();
        int[] mayoresQue5 = numeros.Where(n => n > 5).ToArray();

        // Assert
        pares.Should().BeEquivalentTo([2, 4, 6, 8, 10]);
        mayoresQue5.Should().BeEquivalentTo([6, 7, 8, 9, 10]);
    }

    [Fact]
    public void LinqSelect_DebeProyectarCorrectamente()
    {
        // Arrange
        string[] nombres = ["ana", "luis", "maria"];

        // Act: Select proyecta cada elemento a una nueva forma
        string[] mayusculas = nombres.Select(n => n.ToUpper()).ToArray();
        int[] longitudes = nombres.Select(n => n.Length).ToArray();

        // Assert
        mayusculas.Should().BeEquivalentTo(["ANA", "LUIS", "MARIA"]);
        longitudes.Should().BeEquivalentTo([3, 4, 5]);
    }

    [Fact]
    public void LinqQuerySyntax_DebeSerEquivalenteAMethodSyntax()
    {
        // Arrange
        int[] numeros = [5, 3, 8, 1, 9, 2];

        // Act: query syntax (el compilador lo traduce a method syntax)
        var querySyntax = from n in numeros
                          where n > 3
                          orderby n
                          select n * 10;

        // method syntax equivalente
        var methodSyntax = numeros
            .Where(n => n > 3)
            .OrderBy(n => n)
            .Select(n => n * 10);

        // Assert: ambos estilos producen el mismo resultado
        querySyntax.Should().BeEquivalentTo(methodSyntax);
        querySyntax.Should().BeEquivalentTo([50, 80, 90]);
    }

    [Fact]
    public void Linq_DebeSerEjecucionDiferida()
    {
        // Arrange: LINQ usa ejecución diferida (lazy evaluation)
        List<int> fuente = [1, 2, 3];

        // Creamos una consulta LINQ (aún no se ejecuta)
        IEnumerable<int> consulta = fuente.Where(n => n > 1);

        // Modificamos la fuente DESPUÉS de crear la consulta
        fuente.Add(4);

        // Act: la consulta se ejecuta AHORA (al iterar con ToArray())
        int[] resultado = consulta.ToArray();

        // Assert: la consulta refleja el estado ACTUAL de la fuente
        resultado.Should().BeEquivalentTo([2, 3, 4],
            "porque LINQ usa ejecución diferida: la consulta se evalúa al iterar, no al declararla");
    }
}
