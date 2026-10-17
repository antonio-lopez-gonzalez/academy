// ===================================================================
// PropertyBasedTests — Property-Based Testing con FsCheck
//
// PROPERTY-BASED TESTING:
//   En lugar de escribir casos concretos (1+1=2, 2+3=5),
//   defines PROPIEDADES que deben cumplirse SIEMPRE,
//   y FsCheck genera miles de inputs aleatorios para verificarlas.
//
// Ejemplo clásico:
//   "a + b == b + a" (conmutatividad de la suma)
//   FsCheck prueba con miles de valores de a y b (positivos, negativos, 0, grandes).
//
// CUÁNDO USARLO:
//   - Algoritmos matemáticos (suma, ordenación, compresión).
//   - Serialización/deserialización (roundtrip: serializar → deserializar == original).
//   - Idempotencia (f(f(x)) == f(x)).
//   - Inversas (encode → decode == original).
//
// VENTAJAS:
//   - Encuentra edge cases que no habrías pensado.
//   - Menos código de test, más cobertura.
//
// DESVENTAJAS:
//   - Más lento (ejecuta miles de casos).
//   - No apto para tests con efectos secundarios (BD, red).
//   - Requiere pensar en propiedades, no en casos.
//
// FsCheck usa el atributo [Property] (de FsCheck.Xunit) en lugar de [Fact].
// Genera automáticamente valores aleatorios para los parámetros.
// ===================================================================
using FsCheck;
using FsCheck.Xunit;
using FluentAssertions;
using Xunit;

namespace TestAdvanced;

public class PropertyBasedTests
{
    private readonly OrderService _orderService = new();

    // ─── Propiedad: conmutatividad de la suma ─────────────────────────────
    // a + b == b + a para CUALQUIER a, b enteros.
    // FsCheck probará con positivos, negativos, cero, int.MaxValue, etc.

    [Property(DisplayName = "La suma debe ser conmutativa: a + b == b + a")]
    public bool AddCommutative(int a, int b)
    {
        return (a + b) == (b + a);
    }

    // ─── Propiedad: asociatividad de la suma ──────────────────────────────

    [Property(DisplayName = "La suma debe ser asociativa: (a + b) + c == a + (b + c)")]
    public bool AddAssociative(int a, int b, int c)
    {
        return ((a + b) + c) == (a + (b + c));
    }

    // ─── Propiedad: elemento neutro de la suma ────────────────────────────

    [Property(DisplayName = "Sumar 0 no cambia el valor: a + 0 == a")]
    public bool AddIdentity(int a)
    {
        return (a + 0) == a;
    }

    // ─── Propiedad: la multiplicación también es conmutativa ──────────────

    [Property(DisplayName = "La multiplicación debe ser conmutativa: a * b == b * a")]
    public bool MultiplyCommutative(int a, int b)
    {
        return (a * b) == (b * a);
    }

    // ─── Propiedad con filtro: división nunca lanza si b != 0 ─────────────
    // Usamos un helper para filtrar los casos donde b == 0

    [Property(DisplayName = "Dividir por un número no cero nunca lanza excepción")]
    public Property DivideNeverThrowsForNonZero(int a, int b)
    {
        // Filtramos: solo probamos cuando b != 0
        // FsCheck salta los casos filtrados automáticamente
        if (b == 0)
            return true.ToProperty();

        double result = (double)a / b;
        return true.ToProperty();
    }

    // ─── Propiedad: concatenación de strings ──────────────────────────────
    // Length(a + b) == Length(a) + Length(b)

    [Property(DisplayName = "Longitud de concatenación = suma de longitudes")]
    public bool ConcatLength(string a, string b)
    {
        // Null strings los convertimos a vacío para evitar NullReferenceException
        string sa = a ?? "";
        string sb = b ?? "";

        return (sa + sb).Length == sa.Length + sb.Length;
    }

    // ─── Propiedad: roundtrip de string (Trim no debe alterar si no hay espacios) ──
    // Si un string no empieza ni termina con espacio, Trim() no lo modifica.

    [Property(DisplayName = "Trim no modifica strings sin espacios en los extremos")]
    public Property TrimDoesNotModifyNonWhitespaceStrings(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return true.ToProperty();

        // Si no tiene espacios al inicio ni al final, Trim no lo cambia
        char first = s[0];
        char last = s[^1];
        bool noLeadingOrTrailingWhitespace = !char.IsWhiteSpace(first) && !char.IsWhiteSpace(last);

        return noLeadingOrTrailingWhitespace.When(s.Trim() == s);
    }

    // ─── Propiedad descuento nunca negativo para descuento válido ─────────

    [Property(DisplayName = "CalculateDiscount nunca devuelve negativo para valores válidos")]
    public bool CalculateDiscountNeverNegative(NonNegativeInt quantity, NonNegativeInt unitPriceCents, byte discountPercent)
    {
        // Convertir discountPercent de byte (0-255) a rango 0-100
        decimal discount = discountPercent > 100 ? 100m : discountPercent;

        var items = new List<(string, int, decimal)>
        {
            ("Producto", quantity.Item, unitPriceCents.Item / 100m)
        };

        Order order = _orderService.CreateOrder("Test", items);
        decimal discountAmount = _orderService.CalculateDiscount(order, discount);

        return discountAmount >= 0;
    }

    // ─── Propiedad: descuento 0% implica 0 descuento ──────────────────────

    [Property(DisplayName = "CalculateDiscount con 0% devuelve 0")]
    public bool CalculateDiscountZeroPercentReturnsZero(PositiveInt quantity, NonNegativeInt unitPriceCents)
    {
        var items = new List<(string, int, decimal)>
        {
            ("Producto", quantity.Item, unitPriceCents.Item / 100m)
        };

        Order order = _orderService.CreateOrder("Test", items);
        decimal discountAmount = _orderService.CalculateDiscount(order, 0);

        return discountAmount == 0;
    }

    // ─── Propiedad: descuento 100% descuenta todo ─────────────────────────

    [Fact(DisplayName = "CalculateDiscount con 100% descuenta el total completo")]
    public void CalculateDiscount_FullDiscount_EqualsTotal()
    {
        List<(string, int, decimal)> items = [("Laptop", 1, -1)];
        // Simulamos descuento 100% - no podemos usar FsCheck fácil
        // porque necesitamos valores no aleatorios para el total
        Order order = _orderService.CreateOrder("Test", items);
        decimal discount = _orderService.CalculateDiscount(order, 100);

        discount.Should().Be(order.TotalAmount);
    }
}

// ===================================================================
// CODE COVERAGE (comandos, no ejecutables aquí):
//
// Instalar herramientas:
//   dotnet tool install -g coverlet.console
//   dotnet tool install -g dotnet-reportgenerator-globaltool
//
// Generar cobertura:
//   dotnet test --collect:"XPlat Code Coverage"
//
// O usando Coverlet directamente:
//   coverlet .\bin\Debug\net8.0\TestAdvanced.dll --target "dotnet" --targetargs "test --no-build"
//
// Generar reporte HTML:
//   reportgenerator -reports:coverage.cobertura.xml -targetdir:coveragereport
//
// Ver reporte:
//   start coveragereport\index.html
// ===================================================================
