using Microsoft.Extensions.DependencyInjection;

// ============================================================================
// LECCIÓN 02 — Service Locator (anti-patrón)
// ============================================================================

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║     MÓDULO 5 — Inyección de Dependencias               ║");
Console.WriteLine("║     Lección 02: Service Locator (anti-patrón)           ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
Console.WriteLine();

DemoConContenedor();
ComparacionTest();
CuandoSiEsAceptable();

Console.WriteLine("=== RESUMEN ===");
Console.WriteLine();
Console.WriteLine("1. Service Locator: pasar IServiceProvider como dependencia y resolver dentro.");
Console.WriteLine("2. Problemas: dependencias ocultas, errores en runtime, difícil de testear.");
Console.WriteLine("3. DI: dependencias visibles en el constructor, errores en compilación.");
Console.WriteLine("4. Service Locator solo es aceptable en el Composition Root.");
Console.WriteLine("5. Pasar IServiceProvider a una clase de dominio = anti-patrón.");

return;

// ============================================================================
// MÉTODOS ESTÁTICOS
// ============================================================================

static void DemoConContenedor()
{
    Console.WriteLine("=== DEMO: Contenedor correctamente configurado ===");
    Console.WriteLine();

    var services = new ServiceCollection();
    services.AddSingleton<IEmailSender, SmtpEmailSender>();
    services.AddSingleton<IPaymentGateway, StripePaymentGateway>();
    services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
    services.AddSingleton<OrderServiceCorrecto>();
    services.AddSingleton<BadOrderService>();

    var provider = services.BuildServiceProvider();

    var correcto = provider.GetRequiredService<OrderServiceCorrecto>();
    Console.WriteLine("OrderServiceCorrecto (DI correcto):");
    correcto.PlaceOrder("Teclado", 89.99m, "cliente@email.com");
    Console.WriteLine();

    var malo = provider.GetRequiredService<BadOrderService>();
    Console.WriteLine("BadOrderService (Service Locator):");
    malo.PlaceOrder("Monitor", 299.99m, "cliente@email.com");
    Console.WriteLine();
}

static void ComparacionTest()
{
    Console.WriteLine("=== Test: OrderServiceCorrecto (DI) vs BadOrderService (Service Locator) ===");
    Console.WriteLine();

    Console.WriteLine("OrderServiceCorrecto (DI):");
    Console.WriteLine("  Para testear: creas mocks de IEmailSender, IPaymentGateway, IOrderRepository.");
    Console.WriteLine("  Se los pasas al constructor directamente.");
    Console.WriteLine("  No necesitas configurar un contenedor.");
    Console.WriteLine();

    Console.WriteLine("BadOrderService (Service Locator):");
    Console.WriteLine("  Para testear: necesitas configurar un ServiceCollection completo.");
    Console.WriteLine("  Registras servicios que NI SIQUIERA sabes si el método va a necesitar.");
    Console.WriteLine("  Si el método pide un servicio no registrado → error en runtime.");
    Console.WriteLine();

    Console.WriteLine("--- Demostración de fallo silencioso ---");
    var servicesFaltantes = new ServiceCollection();
    servicesFaltantes.AddSingleton<IPaymentGateway, StripePaymentGateway>();
    servicesFaltantes.AddSingleton<BadOrderService>();

    var providerFaltantes = servicesFaltantes.BuildServiceProvider();
    var badService = providerFaltantes.GetRequiredService<BadOrderService>();

    try
    {
        badService.PlaceOrder("Producto", 50m, "cliente@email.com");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"  ERROR en runtime: {ex.Message.Split('.')[0]}.");
        Console.WriteLine("  Con DI correcto, esto se habría detectado antes.");
    }

    Console.WriteLine();
}

static void CuandoSiEsAceptable()
{
    Console.WriteLine("=== ¿Cuándo es aceptable Service Locator? ===");
    Console.WriteLine();

    Console.WriteLine("SOLO en el Composition Root (entry point de la aplicación):");
    Console.WriteLine("  - Program.cs de una consola");
    Console.WriteLine("  - Startup.cs / Program.cs de ASP.NET");
    Console.WriteLine("  - Factorías dentro del contenedor");
    Console.WriteLine();
    Console.WriteLine("NUNCA en:");
    Console.WriteLine("  - Servicios de dominio");
    Console.WriteLine("  - Servicios de aplicación");
    Console.WriteLine("  - Repositorios");
    Console.WriteLine("  - Cualquier clase que NO sea el punto de entrada");
    Console.WriteLine();
}

// ============================================================================
// TIPOS
// ============================================================================

public interface IEmailSender
{
    void Send(string to, string subject, string body);
}

public interface IPaymentGateway
{
    bool ProcessPayment(decimal amount);
}

public interface IOrderRepository
{
    void Save(Order order);
}

public record Order(Guid Id, string ProductName, decimal Total, string CustomerEmail);

public class SmtpEmailSender : IEmailSender
{
    public void Send(string to, string subject, string body)
    {
        Console.WriteLine($"[SMTP] Email enviado a {to}: {subject}");
    }
}

public class StripePaymentGateway : IPaymentGateway
{
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"[Stripe] Procesando pago de {amount:C}... OK");
        return true;
    }
}

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new();
    public void Save(Order order)
    {
        _orders.Add(order);
        Console.WriteLine($"[Repo] Pedido {order.Id} guardado.");
    }
}

public class OrderServiceCorrecto
{
    private readonly IEmailSender _emailSender;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IOrderRepository _orderRepository;

    public OrderServiceCorrecto(
        IEmailSender emailSender,
        IPaymentGateway paymentGateway,
        IOrderRepository orderRepository)
    {
        _emailSender = emailSender;
        _paymentGateway = paymentGateway;
        _orderRepository = orderRepository;
    }

    public void PlaceOrder(string productName, decimal total, string customerEmail)
    {
        var paymentOk = _paymentGateway.ProcessPayment(total);
        if (!paymentOk) return;

        var order = new Order(Guid.NewGuid(), productName, total, customerEmail);
        _orderRepository.Save(order);
        _emailSender.Send(customerEmail, "Pedido confirmado", $"Has comprado {productName}");
    }
}

public class BadOrderService
{
    private readonly IServiceProvider _serviceProvider;

    public BadOrderService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void PlaceOrder(string productName, decimal total, string customerEmail)
    {
        var paymentGateway = _serviceProvider.GetRequiredService<IPaymentGateway>();
        var orderRepository = _serviceProvider.GetRequiredService<IOrderRepository>();
        var emailSender = _serviceProvider.GetRequiredService<IEmailSender>();

        var paymentOk = paymentGateway.ProcessPayment(total);
        if (!paymentOk) return;

        var order = new Order(Guid.NewGuid(), productName, total, customerEmail);
        orderRepository.Save(order);
        emailSender.Send(customerEmail, "Pedido confirmado", $"Has comprado {productName}");
    }
}
