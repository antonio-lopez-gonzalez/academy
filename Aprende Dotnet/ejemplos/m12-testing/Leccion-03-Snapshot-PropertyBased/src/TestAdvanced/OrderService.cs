// ===================================================================
// OrderService — Clase de ejemplo para snapshot testing.
// Genera objetos complejos que queremos verificar.
// ===================================================================
namespace TestAdvanced;

public record OrderItem(string ProductName, int Quantity, decimal UnitPrice);

public record Order(
    Guid Id,
    string CustomerName,
    List<OrderItem> Items,
    decimal TotalAmount,
    DateTime CreatedAt,
    string Status);

public class OrderService
{
    public Order CreateOrder(string customerName, List<(string Product, int Qty, decimal Price)> items)
    {
        List<OrderItem> orderItems = items
            .Select(i => new OrderItem(i.Product, i.Qty, i.Price))
            .ToList();

        decimal total = orderItems.Sum(i => i.Quantity * i.UnitPrice);

        return new Order(
            Id: Guid.NewGuid(),
            CustomerName: customerName,
            Items: orderItems,
            TotalAmount: total,
            CreatedAt: DateTime.UtcNow,
            Status: "Pending");
    }

    public decimal CalculateDiscount(Order order, decimal discountPercent)
    {
        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentOutOfRangeException(nameof(discountPercent), "El descuento debe estar entre 0 y 100.");

        return order.TotalAmount * (discountPercent / 100m);
    }
}
