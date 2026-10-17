using FluentAssertions;

namespace Demo.Tests;

public class Leccion02_NullableReferenceTypes
{
    // ═══════════════════════════════════════════════════════════════
    // 1. string? acepta null, string no-no-no (comprobado en runtime)
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void NullableString_PuedeSerNull_SinExcepcion()
    {
        // string? declara explícitamente que la variable puede ser null
        string? nombre = null;

        nombre.Should().BeNull();
    }

    [Fact]
    public void NonNullableString_ConNullForgiving_EsNullEnRuntime()
    {
        // El operador ! silencia al compilador pero NO cambia el valor en runtime.
        // Si el valor realmente es null, cualquier acceso a miembros lanza NullReferenceException.
        string? valor = null;
        string noSeguro = valor!;

        Action act = () => _ = noSeguro.Length;

        act.Should().Throw<NullReferenceException>();
    }

    // ═══════════════════════════════════════════════════════════════
    // 2. Operador ?. (null-conditional): acceso seguro a miembros
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void NullConditional_OperandoEsNull_RetornaNull()
    {
        Persona? persona = null;

        string? nombre = persona?.Nombre;

        nombre.Should().BeNull();
    }

    [Fact]
    public void NullConditional_Encadenado_RetornaNullSiIntermedioEsNull()
    {
        // Si cualquier eslabón de la cadena es null, toda la expresión devuelve null
        var persona = new Persona { Nombre = "Ana", Direccion = null };

        string? ciudad = persona?.Direccion?.Ciudad;

        ciudad.Should().BeNull();
    }

    [Fact]
    public void NullConditional_OperandoNoEsNull_RetornaValor()
    {
        var persona = new Persona { Nombre = "Luis" };

        string? nombre = persona?.Nombre;

        nombre.Should().Be("Luis");
    }

    // ═══════════════════════════════════════════════════════════════
    // 3. Operador ?? (null-coalescing): valor por defecto si null
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void NullCoalescing_OperandoIzquierdoEsNull_UsaValorDerecha()
    {
        string? nombre = null;

        string resultado = nombre ?? "Anónimo";

        resultado.Should().Be("Anónimo");
    }

    [Fact]
    public void NullCoalescing_OperandoIzquierdoNoEsNull_UsaValorIzquierda()
    {
        string? nombre = "Carlos";

        string resultado = nombre ?? "Anónimo";

        resultado.Should().Be("Carlos");
    }

    [Fact]
    public void NullCoalescing_EnCadena_UsaPrimerNoNull()
    {
        // La coalescencia en cascada evalúa de izquierda a derecha
        string? a = null;
        string? b = null;
        string c = "Final";

        string resultado = a ?? b ?? c;

        resultado.Should().Be("Final");
    }

    // ═══════════════════════════════════════════════════════════════
    // 4. Operador ??= (null-coalescing assignment): asignar solo si null
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void NullCoalescingAssignment_VariableEsNull_AsignaValor()
    {
        string? nombre = null;

        nombre ??= "Por defecto";

        nombre.Should().Be("Por defecto");
    }

    [Fact]
    public void NullCoalescingAssignment_VariableTieneValor_NoSobrescribe()
    {
        string? nombre = "Original";

        nombre ??= "Nuevo";

        nombre.Should().Be("Original");
    }

    [Fact]
    public void NullCoalescingAssignment_ConColeccion_InicializaLazy()
    {
        List<int>? numeros = null;

        numeros ??= new List<int>();
        numeros.Add(42);

        numeros.Should().HaveCount(1).And.Contain(42);
    }

    // ═══════════════════════════════════════════════════════════════
    // 5. ArgumentNullException.ThrowIfNull() (.NET 6+)
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void ThrowIfNull_ArgumentoEsNull_LanzaArgumentNullException()
    {
        string? valor = null;

        Action act = () => ArgumentNullException.ThrowIfNull(valor);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ThrowIfNull_ArgumentoNoEsNull_NoLanzaExcepcion()
    {
        string valor = "Hola";

        Action act = () => ArgumentNullException.ThrowIfNull(valor);

        act.Should().NotThrow();
    }

    // ═══════════════════════════════════════════════════════════════
    // 6. Null-forgiving operator (!): suprime advertencia, no el null
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void NullForgiving_SolucionDeAdvertencia_PeroNoEvitaNullRefSiRealmenteEsNull()
    {
        string? realmenteNull = null;

        // ! suprime CS8602 en compilación, pero en runtime el valor sigue siendo null
        Action act = () => _ = realmenteNull!.Length;

        act.Should().Throw<NullReferenceException>();
    }

    // ═══════════════════════════════════════════════════════════════
    // 7. [NotNullWhen] attribute: informar narrowing al compilador
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void NotNullWhen_CuandoRetornaTrue_OutParamEsNoNulo()
    {
        // ValidarYTransformar tiene [NotNullWhen(true)] en input y result.
        // Cuando retorna true, el compilador sabe que ambos no son null.
        bool ok = Program.ValidarYTransformar("Hola", out string? result);

        ok.Should().BeTrue();
        result.Should().NotBeNull();
        result.Should().Be("HOLA");
    }

    [Fact]
    public void NotNullWhen_CuandoRetornaFalse_OutParamEsNull()
    {
        bool ok = Program.ValidarYTransformar(null, out string? result);

        ok.Should().BeFalse();
        result.Should().BeNull();
    }

    // ═══════════════════════════════════════════════════════════════
    // 8. Null coalescing con expresiones complejas
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void NullCoalescing_ConExpresionCompleja_EvaluaPerezosamente()
    {
        // La expresión de la derecha SOLO se evalúa si la izquierda es null
        string? entrada = null;

        string resultado = entrada ?? $"Generado: {DateTime.Now:yyyy-MM-dd}";

        resultado.Should().StartWith("Generado: ");
    }

    [Fact]
    public void NullCoalescing_CombinadoConNullConditional_PatronTipico()
    {
        // Patrón real: acceder a un objeto potencialmente null y dar valor por defecto
        Persona? persona = null;

        string display = persona?.Nombre?.ToUpperInvariant() ?? "ANÓNIMO";

        display.Should().Be("ANÓNIMO");
    }

    // ═══════════════════════════════════════════════════════════════
    // 9. string.IsNullOrEmpty con NRT
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void IsNullOrEmpty_ConNull_RetornaTrue()
    {
        // string.IsNullOrEmpty acepta string? sin advertencia.
        // La BCL está anotada con atributos NRT que reconocen este patrón.
        string? valor = null;

        bool vacio = string.IsNullOrEmpty(valor);

        vacio.Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_ConStringVacio_RetornaTrue()
    {
        string valor = "";

        bool vacio = string.IsNullOrEmpty(valor);

        vacio.Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_ConStringConValor_RetornaFalse()
    {
        string? valor = "Hola";

        bool vacio = string.IsNullOrEmpty(valor);

        vacio.Should().BeFalse();
    }

    // ═══════════════════════════════════════════════════════════════
    // 10. [MemberNotNull] attribute: inicialización en dos fases
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void MemberNotNull_TrasInicializar_LosMiembrosNoSonNull()
    {
        var repo = new Repository();

        // Initialize() está anotado con [MemberNotNull(nameof(_data))]
        // Tras llamarlo, el compilador sabe que _data no es null.
        repo.Initialize();

        repo.Count.Should().Be(3);
        repo.First().Should().Be("Elemento1");
    }

    [Fact]
    public void MemberNotNull_SinInicializar_AccesoACampoNullLanzaExcepcion()
    {
        var repo = new Repository();

        // Sin llamar a Initialize(), _data sigue siendo null.
        // El compilador advierte de esto (CS8602) si no usamos [MemberNotNull].
        Action act = () => _ = repo.Count;

        act.Should().Throw<NullReferenceException>();
    }
}
