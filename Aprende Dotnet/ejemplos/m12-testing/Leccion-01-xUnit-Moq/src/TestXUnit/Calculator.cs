// ===================================================================
// Calculadora — Clase simple para demostrar tests unitarios básicos.
// ===================================================================
namespace TestXUnit;

public class Calculator
{
    /// <summary>
    /// Suma dos números enteros.
    /// </summary>
    public int Add(int a, int b) => a + b;

    /// <summary>
    /// Resta dos números enteros.
    /// </summary>
    public int Subtract(int a, int b) => a - b;

    /// <summary>
    /// Multiplica dos números enteros.
    /// </summary>
    public int Multiply(int a, int b) => a * b;

    /// <summary>
    /// Divide dos números enteros.
    /// Lanza DivideByZeroException si el divisor es 0.
    /// </summary>
    public double Divide(int a, int b)
    {
        if (b == 0)
            throw new DivideByZeroException("No se puede dividir por cero.");

        return (double)a / b;
    }
}
