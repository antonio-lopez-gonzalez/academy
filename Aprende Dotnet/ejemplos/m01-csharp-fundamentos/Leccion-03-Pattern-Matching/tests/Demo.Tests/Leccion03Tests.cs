namespace Demo.Tests;

// ====================================================================
// Tests para la Lección 03: Pattern Matching en C#
// Cada test demuestra y verifica un tipo de pattern matching.
// ====================================================================

public class Leccion03Tests
{
    // ────────────────────────────────────────────────────────────────
    // 1. Patrón "is" con verificación de tipo (type check)
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void IsPattern_TypeCheck_ReturnsTrueWhenTypeMatches()
    {
        object valor = "Hola mundo";

        var resultado = valor is string;

        resultado.Should().BeTrue();
    }

    [Fact]
    public void IsPattern_TypeCheck_ReturnsFalseWhenTypeDoesNotMatch()
    {
        object valor = 42;

        var resultado = valor is string;

        resultado.Should().BeFalse();
    }

    // ────────────────────────────────────────────────────────────────
    // 2. Patrón "is" con declaración de variable
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void IsPattern_WithVariableDeclaration_DeclaresAndAssignsInOneStep()
    {
        object dato = 3.14;

        if (dato is double numero)
            numero.Should().Be(3.14);
        else
            Assert.Fail("Se esperaba que dato fuera double");
    }

    [Fact]
    public void IsPattern_WithVariableDeclaration_WorksWithReferenceTypes()
    {
        object dato = "Pattern Matching";

        if (dato is string texto)
        {
            texto.Length.Should().Be(16);
            texto.Should().Be("Pattern Matching");
        }
        else
            Assert.Fail("Se esperaba que dato fuera string");
    }

    // ────────────────────────────────────────────────────────────────
    // 3. Switch expression con múltiples patrones
    // ────────────────────────────────────────────────────────────────
    [Theory]
    [InlineData(1, "Lunes")]
    [InlineData(3, "Miércoles")]
    [InlineData(7, "Fin de semana")]
    [InlineData(0, "Inválido")]
    public void SwitchExpression_MultiplePatterns_ReturnsCorrectLabel(
        int day, string expected)
    {
        var result = day switch
        {
            1 => "Lunes",
            2 => "Martes",
            3 => "Miércoles",
            4 => "Jueves",
            5 => "Viernes",
            6 or 7 => "Fin de semana",
            _ => "Inválido"
        };

        result.Should().Be(expected);
    }

    [Fact]
    public void SwitchExpression_CombinedWithOr_MatchesAnyListedValue()
    {
        var result = 6 switch
        {
            1 or 2 or 3 => "Bajo",
            4 or 5 or 6 => "Medio",
            7 or 8 or 9 => "Alto",
            _ => "Fuera de rango"
        };

        result.Should().Be("Medio");
    }

    // ────────────────────────────────────────────────────────────────
    // 4. Patrones de propiedad (Property patterns)
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void PropertyPattern_MatchingNameAndAge_ReturnsCorrectDescription()
    {
        var persona = new Persona("Ana", 25);

        var descripcion = persona switch
        {
            { Nombre: "Ana", Edad: > 18 } => "Ana mayor de edad",
            { Nombre: "Ana" } => "Ana menor de edad",
            { Edad: > 18 } => "Mayor de edad",
            _ => "Sin criterio"
        };

        descripcion.Should().Be("Ana mayor de edad");
    }

    [Fact]
    public void PropertyPattern_NestedProperties_MatchesCorrectly()
    {
        var direccion = new Direccion("Madrid", "España");
        var cliente = new Cliente("Juan", direccion);

        var infoCliente = cliente switch
        {
            { Direccion: { Ciudad: "Madrid" } } => "Cliente de Madrid",
            { Direccion: { Pais: "España" } } => "Cliente de España",
            _ => "Internacional"
        };

        infoCliente.Should().Be("Cliente de Madrid");
    }

    // ────────────────────────────────────────────────────────────────
    // 5. Patrones de lista: longitud fija (List patterns)
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void ListPattern_FixedLength_MatchesExactArray()
    {
        int[] numeros = { 1, 2, 3 };

        var resultado = numeros switch
        {
            [1, 2, 3] => "Exacto",
            [1, 2] => "Dos elementos",
            _ => "Otro"
        };

        resultado.Should().Be("Exacto");
    }

    [Fact]
    public void ListPattern_FixedLength_DoesNotMatchLongerArray()
    {
        int[] numeros = { 1, 2, 3, 4 };

        var resultado = numeros switch
        {
            [1, 2, 3] => "Exacto tres",
            _ => "No coincide"
        };

        resultado.Should().Be("No coincide");
    }

    // ────────────────────────────────────────────────────────────────
    // 6. Patrones de lista: slice con ".."
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void ListPattern_SliceElement_CapturesRemainingElements()
    {
        int[] numeros = { 1, 2, 3, 4, 5 };

        var resultado = numeros switch
        {
            [1, ..] => "Empieza con 1",
            [.., 5] => "Termina en 5",
            _ => "Otro"
        };

        resultado.Should().Be("Empieza con 1");
    }

    [Fact]
    public void ListPattern_SliceWithDiscardAndCapture_ExtractsSecondElement()
    {
        string[] frutas = { "manzana", "naranja", "plátano" };

        var resultado = frutas switch
        {
            [_, var segunda, ..] => segunda,
            _ => "ninguna"
        };

        resultado.Should().Be("naranja");
    }

    [Fact]
    public void ListPattern_EmptyList_MatchesEmptyPattern()
    {
        int[] vacia = Array.Empty<int>();

        var resultado = vacia switch
        {
            [] => "Vacía",
            _ => "Con elementos"
        };

        resultado.Should().Be("Vacía");
    }

    // ────────────────────────────────────────────────────────────────
    // 7. Patrones relacionales con < y >
    // ────────────────────────────────────────────────────────────────
    [Theory]
    [InlineData(95, "Sobresaliente")]
    [InlineData(85, "Notable")]
    [InlineData(55, "Insuficiente")]
    [InlineData(-5, "Inválida")]
    public void RelationalPattern_LessThanAndGreaterThan_ClassifiesCorrectly(
        int score, string expected)
    {
        var calificacion = score switch
        {
            >= 90 => "Sobresaliente",
            >= 80 and < 90 => "Notable",
            >= 70 and < 80 => "Bien",
            >= 60 and < 70 => "Suficiente",
            >= 0 and < 60 => "Insuficiente",
            < 0 => "Inválida"
        };

        calificacion.Should().Be(expected);
    }

    // ────────────────────────────────────────────────────────────────
    // 8. Patrón lógico "and"
    // ────────────────────────────────────────────────────────────────
    [Theory]
    [InlineData('A', "Mayúscula")]
    [InlineData('z', "Minúscula")]
    [InlineData('5', "Dígito")]
    [InlineData('!', "Otro")]
    public void LogicalPattern_And_RequiresBothConditions(char c, string expected)
    {
        var tipo = c switch
        {
            >= 'A' and <= 'Z' => "Mayúscula",
            >= 'a' and <= 'z' => "Minúscula",
            >= '0' and <= '9' => "Dígito",
            _ => "Otro"
        };

        tipo.Should().Be(expected);
    }

    // ────────────────────────────────────────────────────────────────
    // 9. Patrón lógico "or"
    // ────────────────────────────────────────────────────────────────
    [Theory]
    [InlineData(12, "Invierno")]
    [InlineData(4, "Primavera")]
    [InlineData(7, "Verano")]
    [InlineData(10, "Otoño")]
    public void LogicalPattern_Or_MatchesAnyOfTheListedValues(int month, string expected)
    {
        var estacion = month switch
        {
            12 or 1 or 2 => "Invierno",
            3 or 4 or 5 => "Primavera",
            6 or 7 or 8 => "Verano",
            9 or 10 or 11 => "Otoño",
            _ => "Inválido"
        };

        estacion.Should().Be(expected);
    }

    // ────────────────────────────────────────────────────────────────
    // 10. Patrón lógico "not"
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void LogicalPattern_Not_NegatesThePattern()
    {
        object? valor = null;

        var resultado = valor switch
        {
            not null => "Tiene valor",
            null => "Es null"
        };

        resultado.Should().Be("Es null");
    }

    [Fact]
    public void LogicalPattern_Not_WithNonNullValue_ReturnsNegatedResult()
    {
        object? valor = 42;

        var resultado = valor switch
        {
            not null => "Tiene valor",
            null => "Es null"
        };

        resultado.Should().Be("Tiene valor");
    }

    // ────────────────────────────────────────────────────────────────
    // 11. Cláusula "when" en switch
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void WhenClause_InSwitch_AppliesAdditionalCondition()
    {
        object entrada = "Hola mundo de C#";

        var resultado = entrada switch
        {
            string s when s.Length > 10 => "String largo",
            string s when s.Length <= 10 => "String corto",
            _ => "No es string"
        };

        resultado.Should().Be("String largo");
    }

    [Fact]
    public void WhenClause_InSwitch_FallsThroughWhenConditionNotMet()
    {
        object entrada = "Hola";

        var resultado = entrada switch
        {
            string s when s.Length > 10 => "String largo",
            string s => "String", // sin when, captura cualquier string que no cumpla el when anterior
            _ => "No es string"
        };

        resultado.Should().Be("String");
    }

    // ────────────────────────────────────────────────────────────────
    // 12. Patrones de tupla (Tuple patterns)
    // ────────────────────────────────────────────────────────────────
    [Theory]
    [InlineData(0, 0, "Origen")]
    [InlineData(5, 0, "Eje X")]
    [InlineData(0, 10, "Eje Y")]
    [InlineData(3, 4, "Cuadrante I")]
    [InlineData(-3, 4, "Cuadrante II")]
    [InlineData(-2, -5, "Cuadrante III")]
    [InlineData(6, -1, "Cuadrante IV")]
    public void TuplePattern_MatchesEachPositionInTuple(
        int x, int y, string expected)
    {
        var resultado = (x, y) switch
        {
            (0, 0) => "Origen",
            (_, 0) => "Eje X",
            (0, _) => "Eje Y",
            ( > 0, > 0) => "Cuadrante I",
            ( < 0, > 0) => "Cuadrante II",
            ( < 0, < 0) => "Cuadrante III",
            ( > 0, < 0) => "Cuadrante IV",
        };

        resultado.Should().Be(expected);
    }

    [Fact]
    public void TuplePattern_WithThreeElements_UsesWhenClause()
    {
        var terna = (1, 2, 3);

        var resultado = terna switch
        {
            (1, 2, 3) => "Secuencia fija",
            (var a, var b, var c) when a < b && b < c => "Creciente",
            _ => "Otro"
        };

        resultado.Should().Be("Secuencia fija");
    }

    // ────────────────────────────────────────────────────────────────
    // Bonus: Pattern matching con Span<char>
    // ────────────────────────────────────────────────────────────────
    [Fact]
    public void SpanPattern_MatchesFirstElementOfSpan()
    {
        ReadOnlySpan<char> span = "Hola Mundo".AsSpan();

        var resultado = span switch
        {
            ['H', ..] => "Empieza con H",
            _ => "No empieza con H"
        };

        resultado.Should().Be("Empieza con H");
    }

    [Fact]
    public void SpanPattern_StackAllocSlice_MatchesExact()
    {
        Span<char> buffer = stackalloc char[5] { 'H', 'e', 'l', 'l', 'o' };

        var resultado = buffer switch
        {
            ['H', 'e', 'l', 'l', 'o'] => "Exacto",
            _ => "No coincide"
        };

        resultado.Should().Be("Exacto");
    }
}

// ──────────────────────────────────────────────────────────────────
// Tipos auxiliares para los tests de property patterns
// ──────────────────────────────────────────────────────────────────

internal record Persona(string Nombre, int Edad);

internal record Direccion(string Ciudad, string Pais);

internal record Cliente(string Nombre, Direccion Direccion);
