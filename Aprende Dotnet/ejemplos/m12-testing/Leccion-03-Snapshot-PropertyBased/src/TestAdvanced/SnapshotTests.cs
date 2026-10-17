// ===================================================================
// SnapshotTests — Snapshot Testing (concepto)
//
// SNAPSHOT TESTING:
//   - Se ejecuta el código que produce un objeto complejo.
//   - Se serializa a JSON y se compara con un "snapshot" guardado.
//   - Si coinciden, el test pasa.
//   - Si difieren, se revisa si el cambio es esperado.
//
// HERRAMIENTA REAL: Verify (https://github.com/VerifyTests/Verify)
//   - NuGet: Verify.Xunit
//   - Guarda automáticamente snapshots en archivos .verified.txt
//   - Si hay diferencias, muestra un diff.
//   - Soporta JSON, XML, imágenes, PDFs.
//
// Aquí mostramos el CONCEPTO con un snapshot manual (JSON string).
// En un proyecto real usaríamos el paquete Verify.
// ===================================================================
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace TestAdvanced;

public class SnapshotTests
{
    private readonly OrderService _orderService = new();

    // ─── Snapshot manual (simulando Verify) ───────────────────────────────

    [Fact(DisplayName = "CreateOrder debe generar el JSON esperado (snapshot manual)")]
    public void CreateOrder_ValidItems_MatchesSnapshot()
    {
        // Arrange
        List<(string Product, int Qty, decimal Price)> items =
        [
            ("Laptop", 1, 999.99m),
            ("Mouse", 2, 29.99m),
            ("Teclado", 1, 89.99m)
        ];

        // Act: creamos la orden (sin Guid ni fecha porque son variables)
        Order order = _orderService.CreateOrder("Alicia", items);

        // Act: serializamos a JSON con opciones deterministas
        JsonSerializerOptions options = new()
        {
            WriteIndented = false, // Una sola línea para comparar fácil
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        string json = JsonSerializer.Serialize(order, options);

        // Assert: verificamos campos clave que NO son aleatorios
        // El snapshot completo no se puede comparar porque Id y CreatedAt cambian
        json.Should().Contain("\"customerName\":\"Alicia\"");
        json.Should().Contain("\"status\":\"Pending\"");
        json.Should().Contain("\"totalAmount\":1149.96");

        // Verificar que hay 3 items
        List<OrderItem>? items_list = order.Items;
        items_list.Should().HaveCount(3);
        items_list![0].ProductName.Should().Be("Laptop");
        items_list[1].ProductName.Should().Be("Mouse");
        items_list[2].ProductName.Should().Be("Teclado");

        // Total debe ser la suma correcta
        order.TotalAmount.Should().Be(1149.96m);
    }

    [Fact(DisplayName = "CreateOrder con items vacíos debe tener total 0")]
    public void CreateOrder_EmptyItems_HasZeroTotal()
    {
        Order order = _orderService.CreateOrder("Bruno", new List<(string, int, decimal)>());

        order.Items.Should().BeEmpty();
        order.TotalAmount.Should().Be(0);
    }

    [Fact(DisplayName = "Verificar estructura de un objeto complejo con BeEquivalentTo")]
    public void CreateOrder_StructureCheck_WithBeEquivalentTo()
    {
        // Arrange
        List<(string Product, int Qty, decimal Price)> items =
        [
            ("Tablet", 1, 499.99m)
        ];

        // Act
        Order order = _orderService.CreateOrder("Carlos", items);

        // Assert: BeEquivalentTo compara estructura ignorando valores no especificados
        order.Should().BeEquivalentTo(new
        {
            CustomerName = "Carlos",
            TotalAmount = 499.99m,
            Status = "Pending"
        }, options => options.ExcludingMissingMembers());
    }
}
