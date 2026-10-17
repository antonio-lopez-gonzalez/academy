using FluentAssertions;

namespace Demo.Tests;

// ====================================================================
// Tests para la Lección 06: Extension Methods, IDisposable, IAsyncDisposable
// ====================================================================

public class Leccion06Tests
{
    // ═══════════════════════════════════════════════════════════════
    // EXTENSION METHODS
    // ═══════════════════════════════════════════════════════════════

    // ────────────────────────────────────────────────────────────────
    // 1. WordCount: extensión sobre string que cuenta palabras
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void WordCount_FraseConEspaciosMultiples_CuentaCorrectamente()
    {
        string frase = "  hola   mundo   desde .NET  ";

        int resultado = frase.WordCount();

        resultado.Should().Be(4); // hola, mundo, desde, .NET
    }

    [Fact]
    public void WordCount_FraseVacia_RetornaCero()
    {
        string vacia = "";

        int resultado = vacia.WordCount();

        resultado.Should().Be(0);
    }

    [Fact]
    public void WordCount_UnaSolaPalabra_RetornaUno()
    {
        string palabra = "Hola";

        int resultado = palabra.WordCount();

        resultado.Should().Be(1);
    }

    // ────────────────────────────────────────────────────────────────
    // 2. ToTitleCase: extensión sobre string que capitaliza cada palabra
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void ToTitleCase_FraseEnMinusculas_CapitalizaCadaPalabra()
    {
        string frase = "hola mundo desde csharp";

        string resultado = frase.ToTitleCase();

        resultado.Should().Be("Hola Mundo Desde Csharp");
    }

    [Fact]
    public void ToTitleCase_FraseMezclada_NormalizaMayusculas()
    {
        string frase = "hOlA MuNdO cShArP";

        string resultado = frase.ToTitleCase();

        // Solo la primera letra de cada palabra en mayúscula, resto minúscula
        resultado.Should().Be("Hola Mundo Csharp");
    }

    // ────────────────────────────────────────────────────────────────
    // 3. Median: extensión sobre IEnumerable<int>
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void Median_ConjuntoImpar_RetornaElementoCentral()
    {
        int[] numeros = { 1, 3, 5, 7, 9 };

        double mediana = numeros.Median();

        mediana.Should().Be(5.0);
    }

    [Fact]
    public void Median_ConjuntoPar_RetornaPromedioCentrales()
    {
        int[] numeros = { 1, 2, 3, 4 };

        double mediana = numeros.Median();

        // (2 + 3) / 2 = 2.5
        mediana.Should().Be(2.5);
    }

    [Fact]
    public void Median_ConjuntoVacio_RetornaCero()
    {
        int[] vacio = Array.Empty<int>();

        double mediana = vacio.Median();

        mediana.Should().Be(0);
    }

    [Fact]
    public void Median_Desordenado_FuncionaPorqueOrdenaInternamente()
    {
        int[] numeros = { 9, 1, 5, 7, 3 };

        double mediana = numeros.Median();

        // Ordena internamente → [1, 3, 5, 7, 9] → mediana = 5
        mediana.Should().Be(5.0);
    }

    // ────────────────────────────────────────────────────────────────
    // 4. Encadenamiento de extension methods
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void Encadenamiento_TrimToTitleCaseWordCount_ProduceResultadoCorrecto()
    {
        string texto = "  hola MUNDO desde .NET  ";

        int resultado = texto.Trim().ToTitleCase().WordCount();

        resultado.Should().Be(4);
    }

    [Fact]
    public void Encadenamiento_ToTitleCaseSeguidoDeWordCount_RetornaMismoNumeroPalabras()
    {
        string texto = "uno dos tres";

        int antes = texto.WordCount();
        int despues = texto.ToTitleCase().WordCount();

        antes.Should().Be(despues);
    }

    // ────────────────────────────────────────────────────────────────
    // 5. El compilador traduce extensión a llamada estática
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void ExtensionMethod_LlamadaPorInstanciaYLlamadaEstatica_ProducenMismoResultado()
    {
        string frase = "hola mundo";

        int viaInstancia = frase.WordCount();
        int viaEstatica = StringExtensions.WordCount(frase);

        viaInstancia.Should().Be(viaEstatica);
    }

    // ═══════════════════════════════════════════════════════════════
    // IDISPOSABLE
    // ═══════════════════════════════════════════════════════════════

    // ────────────────────────────────────────────────────────────────
    // 6. using statement (tradicional) libera el recurso al salir
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void UsingStatement_AlSalirDelBloque_DisposeEsLlamado()
    {
        FileSimulator? recurso;

        using (recurso = new FileSimulator("test.log"))
        {
            recurso.IsDisposed.Should().BeFalse("dentro del bloque using aún no se ha llamado Dispose");
        }

        recurso.IsDisposed.Should().BeTrue("al salir del bloque using, Dispose es llamado automáticamente");
    }

    // ────────────────────────────────────────────────────────────────
    // 7. using declaration (C# 8+) libera al final del scope
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void UsingDeclaration_AlSalirDelScope_DisposeEsLlamado()
    {
        FileSimulator recurso;

        {
            using var fs = new FileSimulator("datos.txt");
            recurso = fs;
            recurso.IsDisposed.Should().BeFalse("dentro del scope aún no se ha llamado Dispose");
        } // ← Dispose se llama aquí

        recurso.IsDisposed.Should().BeTrue("al salir del scope, Dispose es llamado automáticamente");
    }

    // ────────────────────────────────────────────────────────────────
    // 8. Dispose se llama exactamente una vez (idempotencia)
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void Dispose_LlamadoMultiplesVeces_DisposeCountEsUno()
    {
        var tracker = new DisposeTracker();

        tracker.Dispose();
        tracker.Dispose(); // Segunda llamada: no debe hacer nada
        tracker.Dispose(); // Tercera llamada: no debe hacer nada

        tracker.DisposeCount.Should().Be(1, "Dispose debe ser idempotente");
        tracker.IsDisposed.Should().BeTrue();
    }

    // ────────────────────────────────────────────────────────────────
    // 9. using statement llama a Dispose exactamente una vez
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void UsingStatement_DisposeEsLlamadoExactamenteUnaVez()
    {
        DisposeTracker tracker;

        using (tracker = new DisposeTracker())
        {
            tracker.IsDisposed.Should().BeFalse();
        }

        tracker.DisposeCount.Should().Be(1, "el using llama a Dispose una sola vez");
        tracker.IsDisposed.Should().BeTrue();
    }

    // ────────────────────────────────────────────────────────────────
    // 10. IAsyncDisposable con await using
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public async Task AwaitUsing_AlSalirDelBloque_DisposeAsyncEsLlamado()
    {
        AsyncDatabaseConnection? conexion;

        await using (conexion = new AsyncDatabaseConnection())
        {
            await conexion.WriteAsync("SELECT 1");
            conexion.IsDisposed.Should().BeFalse("dentro del await using aún no se ha llamado DisposeAsync");
        }

        conexion.IsDisposed.Should().BeTrue("al salir del bloque await using, DisposeAsync es llamado");
        conexion.DisposeAsyncCount.Should().Be(1);
    }

    // ────────────────────────────────────────────────────────────────
    // 11. Clase IDisposable lanza ObjectDisposedException tras Dispose
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void FileSimulator_TrasDispose_LanzaObjectDisposedException()
    {
        var fs = new FileSimulator("test.log");
        fs.Dispose();

        Action act = () => fs.Write("no debería permitir escribir");

        act.Should().Throw<ObjectDisposedException>();
    }

    // ────────────────────────────────────────────────────────────────
    // 12. No llamar a Dispose deja el recurso abierto
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void FileSimulator_SinLlamarDispose_IsDisposedEsFalse()
    {
        var fs = new FileSimulator("nunca_cerrado.log");

        fs.IsDisposed.Should().BeFalse("sin llamar a Dispose, el recurso sigue abierto");

        // Limpiamos manualmente al final del test
        fs.Dispose();
    }

    // ────────────────────────────────────────────────────────────────
    // 13. using con múltiples recursos
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void Using_MultiplesRecursos_TodosSonLiberados()
    {
        FileSimulator? fs1, fs2;

        using (fs1 = new FileSimulator("a.log"))
        using (fs2 = new FileSimulator("b.log"))
        {
            fs1.IsDisposed.Should().BeFalse();
            fs2.IsDisposed.Should().BeFalse();
        }

        fs1.IsDisposed.Should().BeTrue();
        fs2.IsDisposed.Should().BeTrue();
    }
}
