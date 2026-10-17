using Microsoft.Extensions.DependencyInjection;

// ============================================================================
// LECCIÓN 01 — ¿Qué es DI? + Contenedor de .NET por dentro
// ============================================================================

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║     MÓDULO 5 — Inyección de Dependencias               ║");
Console.WriteLine("║     Lección 01: ¿Qué es DI? + Contenedor por dentro     ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
Console.WriteLine();

PoorMansDiDemo();
ContainerDiDemo();
SwapImplementationDemo();
ResolveDemo();

Console.WriteLine("=== RESUMEN ===");
Console.WriteLine();
Console.WriteLine("1. DI resuelve el acoplamiento: las clases dependen de interfaces, no de implementaciones.");
Console.WriteLine("2. Poor Man's DI funciona pero no escala: el Composition Root se vuelve caótico.");
Console.WriteLine("3. El contenedor (ServiceCollection → ServiceProvider) automatiza el cableado.");
Console.WriteLine("4. Registrar: AddSingleton / AddScoped / AddTransient.");
Console.WriteLine("5. Resolver: GetRequiredService<T>() (preferido) o GetService<T>().");
Console.WriteLine("6. DI es la implementación práctica del DIP (módulo 3).");
Console.WriteLine("7. Cambiar de implementación = cambiar UNA línea de registro, no la clase.");

return; // Fin de top-level statements — todo lo de abajo son tipos y métodos

// ============================================================================
// MÉTODOS ESTÁTICOS (top-level)
// ============================================================================

static void PoorMansDiDemo()
{
    Console.WriteLine("=== POOR MAN'S DI (inyección manual sin contenedor) ===");
    Console.WriteLine();

    IEmailSender emailSender = new EmailSender();
    IOrderRepository orderRepository = new InMemoryOrderRepository();
    var orderService = new OrderService(emailSender, orderRepository);

    orderService.PlaceOrder("Teclado mecánico", 149.99m, "cliente@email.com");
    Console.WriteLine();
}

static void ContainerDiDemo()
{
    Console.WriteLine("=== DI CON EL CONTENEDOR DE .NET ===");
    Console.WriteLine();

    var services = new ServiceCollection();

    services.AddSingleton<IEmailSender, EmailSender>();
    services.AddTransient<IOrderRepository, InMemoryOrderRepository>();
    services.AddScoped<OrderService>();

    ServiceProvider serviceProvider = services.BuildServiceProvider();

    var orderService = serviceProvider.GetRequiredService<OrderService>();
    orderService.PlaceOrder("Ratón inalámbrico", 59.99m, "cliente@email.com");

    Console.WriteLine();
}

static void SwapImplementationDemo()
{
    Console.WriteLine("=== CAMBIAR IMPLEMENTACIÓN (Mock vs Real) ===");
    Console.WriteLine();

    var servicesReal = new ServiceCollection();
    servicesReal.AddSingleton<IEmailSender, EmailSender>();
    servicesReal.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
    servicesReal.AddScoped<OrderService>();

    var providerReal = servicesReal.BuildServiceProvider();
    var orderServiceReal = providerReal.GetRequiredService<OrderService>();
    Console.WriteLine("Con EmailSender real:");
    orderServiceReal.PlaceOrder("Monitor 4K", 399.99m, "cliente@email.com");
    Console.WriteLine();

    var servicesMock = new ServiceCollection();
    servicesMock.AddSingleton<IEmailSender, MockEmailSender>();
    servicesMock.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
    servicesMock.AddScoped<OrderService>();

    var providerMock = servicesMock.BuildServiceProvider();
    var orderServiceMock = providerMock.GetRequiredService<OrderService>();
    Console.WriteLine("Con MockEmailSender (solo cambió el registro):");
    orderServiceMock.PlaceOrder("Monitor 4K", 399.99m, "cliente@email.com");

    Console.WriteLine();
}

static void ResolveDemo()
{
    Console.WriteLine("=== GetService<T> vs GetRequiredService<T> ===");
    Console.WriteLine();

    var services = new ServiceCollection();
    services.AddSingleton<IEmailSender, EmailSender>();
    var provider = services.BuildServiceProvider();

    var emailSender = provider.GetRequiredService<IEmailSender>();
    Console.WriteLine($"GetRequiredService<IEmailSender>: {emailSender.GetType().Name}");

    IOrderRepository? repo = provider.GetService<IOrderRepository>();
    Console.WriteLine($"GetService<IOrderRepository> (no registrado): {(repo is null ? "null" : repo.GetType().Name)}");

    Console.WriteLine();
}

// ============================================================================
// TIPOS (deben ir después de los top-level statements)
// ============================================================================

/// <summary>
/// Abstracción para el envío de emails.
/// </summary>
public interface IEmailSender
{
    void Send(string to, string subject, string body);
}

/// <summary>
/// Abstracción para el repositorio de pedidos.
/// </summary>
public interface IOrderRepository
{
    void Save(Order order);
    Order? GetById(Guid id);
}

/// <summary>
/// Entidad de dominio: un pedido.
/// </summary>
public record Order(Guid Id, string ProductName, decimal Total);

/// <summary>
/// Implementación real de envío de emails (simula envío real).
/// </summary>
public class EmailSender : IEmailSender
{
    public void Send(string to, string subject, string body)
    {
        Console.WriteLine($"[EMAIL REAL] Para: {to}");
        Console.WriteLine($"  Asunto: {subject}");
        Console.WriteLine($"  Cuerpo: {body}");
    }
}

/// <summary>
/// Implementación mock para testing/desarrollo.
/// </summary>
public class MockEmailSender : IEmailSender
{
    public void Send(string to, string subject, string body)
    {
        Console.WriteLine($"[EMAIL MOCK] Para: {to}");
        Console.WriteLine($"  Asunto: {subject}");
        Console.WriteLine($"  (No se envía email real en modo mock)");
    }
}

/// <summary>
/// Repositorio en memoria para desarrollo/testing.
/// </summary>
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _orders = new();

    public void Save(Order order)
    {
        _orders[order.Id] = order;
        Console.WriteLine($"[REPO] Pedido guardado: {order.ProductName} ({order.Total:C})");
    }

    public Order? GetById(Guid id)
    {
        _orders.TryGetValue(id, out var order);
        return order;
    }
}

/// <summary>
/// OrderService depende de abstracciones (IEmailSender, IOrderRepository).
/// </summary>
public class OrderService
{
    private readonly IEmailSender _emailSender;
    private readonly IOrderRepository _orderRepository;

    public OrderService(IEmailSender emailSender, IOrderRepository orderRepository)
    {
        _emailSender = emailSender;
        _orderRepository = orderRepository;
    }

    public void PlaceOrder(string productName, decimal total, string customerEmail)
    {
        var order = new Order(Guid.NewGuid(), productName, total);

        _orderRepository.Save(order);
        _emailSender.Send(customerEmail,
            "Pedido confirmado",
            $"Tu pedido de {productName} por {total:C} ha sido registrado.");
    }
}
