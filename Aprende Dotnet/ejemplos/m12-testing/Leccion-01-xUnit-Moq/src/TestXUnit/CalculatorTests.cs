// ===================================================================
// CalculatorTests — Tests unitarios con xUnit + FluentAssertions
//
// CONVENCIONES DE NOMBRES:
//   [Método]_[Escenario]_[ResultadoEsperado]
//   Ejemplo: Add_TwoPositiveNumbers_ReturnsSum
//
// TIPOS DE TESTS EN xUnit:
//   [Fact]     = un test sin parámetros (un caso concreto)
//   [Theory]   = un test con parámetros (múltiples casos)
//     [InlineData]  = datos inline en el atributo
//     [MemberData]  = datos desde un método estático
//     [ClassData]   = datos desde una clase que implementa IEnumerable<object[]>
// ===================================================================
using FluentAssertions;
using Xunit;

namespace TestXUnit;

public class CalculatorTests
{
    private readonly Calculator _calculator = new();

    // ─── FACT: Test simple sin parámetros ─────────────────────────────────

    [Fact(DisplayName = "Add debe sumar dos números positivos correctamente")]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        // Act (ejecutar)
        int result = _calculator.Add(2, 3);

        // Assert (verificar) con FluentAssertions
        result.Should().Be(5);
    }

    [Fact(DisplayName = "Add con números negativos debe funcionar")]
    public void Add_NegativeNumbers_ReturnsCorrectSum()
    {
        int result = _calculator.Add(-5, -3);

        result.Should().Be(-8);
    }

    // ─── THEORY + InlineData: múltiples casos con datos inline ────────────

    [Theory(DisplayName = "Add debe sumar correctamente múltiples combinaciones")]
    [InlineData(0, 0, 0)]       // 0 + 0 = 0
    [InlineData(1, 1, 2)]       // 1 + 1 = 2
    [InlineData(-1, 1, 0)]      // -1 + 1 = 0
    [InlineData(100, 200, 300)] // 100 + 200 = 300
    [InlineData(-5, -5, -10)]   // -5 + -5 = -10
    public void Add_MultipleInputs_ReturnsExpectedSum(int a, int b, int expected)
    {
        int result = _calculator.Add(a, b);

        result.Should().Be(expected);
    }

    // ─── THEORY + MemberData: datos desde un método ───────────────────────

    [Theory(DisplayName = "Multiply con datos desde MemberData")]
    [MemberData(nameof(MultiplyTestData))]
    public void Multiply_MultipleInputs_ReturnsExpectedProduct(int a, int b, int expected)
    {
        int result = _calculator.Multiply(a, b);

        result.Should().Be(expected);
    }

    /// <summary>
    /// Método estático que provee datos a [MemberData].
    /// Debe devolver IEnumerable<object[]>.
    /// </summary>
    public static IEnumerable<object[]> MultiplyTestData()
    {
        yield return new object[] { 1, 1, 1 };
        yield return new object[] { 2, 3, 6 };
        yield return new object[] { 0, 5, 0 };
        yield return new object[] { -2, 3, -6 };
        yield return new object[] { -3, -3, 9 };
    }

    // ─── THEORY + ClassData: datos desde una clase ────────────────────────

    [Theory(DisplayName = "Subtract con datos desde ClassData")]
    [ClassData(typeof(SubtractTestData))]
    public void Subtract_MultipleInputs_ReturnsExpectedDifference(int a, int b, int expected)
    {
        int result = _calculator.Subtract(a, b);

        result.Should().Be(expected);
    }

    // ─── Test de excepciones ──────────────────────────────────────────────

    [Fact(DisplayName = "Divide entre cero debe lanzar DivideByZeroException")]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        // Act: invocamos la acción que debe lanzar excepción
        Action act = () => _calculator.Divide(10, 0);

        // Assert: verificamos que lance la excepción correcta
        act.Should()
            .Throw<DivideByZeroException>()
            .WithMessage("No se puede dividir por cero.");
    }

    [Fact(DisplayName = "Divide con divisor válido no debe lanzar excepción")]
    public void Divide_ValidDivisor_DoesNotThrow()
    {
        // Arrange & Act
        Action act = () => _calculator.Divide(10, 2);

        // Assert
        act.Should().NotThrow();
    }

    // ─── FluentAssertions: comparaciones avanzadas ────────────────────────

    [Fact(DisplayName = "Divide con decimales debe aproximarse correctamente")]
    public void Divide_TenByThree_ReturnsApproximateValue()
    {
        double result = _calculator.Divide(10, 3);

        // BeApproximately permite comparar doubles con tolerancia
        result.Should().BeApproximately(3.333, 0.001);
    }

    [Fact(DisplayName = "Add con números grandes no debe desbordar")]
    public void Add_LargeNumbers_DoesNotOverflow()
    {
        int result = _calculator.Add(int.MaxValue - 1, 1);

        result.Should().Be(int.MaxValue);
    }
}

/// <summary>
/// Clase que provee datos para [ClassData].
/// Debe implementar IEnumerable<object[]>.
/// </summary>
public class SubtractTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 5, 3, 2 };
        yield return new object[] { 0, 5, -5 };
        yield return new object[] { 10, 0, 10 };
        yield return new object[] { -3, -2, -1 };
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
