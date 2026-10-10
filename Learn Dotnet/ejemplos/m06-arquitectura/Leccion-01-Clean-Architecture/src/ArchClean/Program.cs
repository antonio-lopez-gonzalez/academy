// =============================================================================
// Módulo 06 — Arquitectura y organización
// Lección 01 — Clean Architecture
// =============================================================================
// Clean Architecture (propuesta por Robert C. Martin) organiza el código en capas
// concéntricas donde las dependencias siempre apuntan hacia el centro.
//
//                  ┌──────────────────────────────────┐
//                  │        Frameworks & Drivers      │
//                  │   ┌──────────────────────────┐   │
//                  │   │    Interface Adapters     │   │
//                  │   │   ┌──────────────────┐   │   │
//                  │   │   │  Application     │   │   │
//                  │   │   │  ┌──────────┐    │   │   │
//                  │   │   │  │ DOMAIN   │    │   │   │
//                  │   │   │  │ (Entities,│    │   │   │
//                  │   │   │  │  Value    │    │   │   │
//                  │   │   │  │  Objects) │    │   │   │
//                  │   │   │  └──────────┘    │   │   │
//                  │   │   └──────────────────┘   │   │
//                  │   └──────────────────────────┘   │
//                  └──────────────────────────────────┘
//
// REGLA DE DEPENDENCIA:
//   Las dependencias de código fuente SOLO pueden apuntar hacia dentro.
//   Nada en el círculo interior puede conocer algo del círculo exterior.
//
//   Domain → no depende de nadie (ni EF Core, ni ASP.NET, ni System.Net.Http)
//   Application → depende de Domain (conoce entidades e interfaces)
//   Infrastructure → depende de Application y Domain (implementa interfaces)
//   Presentation → depende de Application (inyecta las dependencias concretas)
//
// En este ejemplo simulamos las 4 capas con namespaces en un solo archivo.
// En un proyecto real, cada capa sería un proyecto .csproj separado.
//
// CAPA  ──────────────  Proyecto .csproj        Dependencias
// ─────────────────────────────────────────────────────────────────
// Domain               Cms.Domain              (ninguna)
// Application          Cms.Application         Cms.Domain
// Infrastructure       Cms.Infrastructure      Cms.Application (para interfaces)
// Presentation         Cms.Api                 Cms.Application, Cms.Infrastructure
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ═══════════════════════════════════════════════════════════════════
// CAPA: DOMAIN (el corazón de la aplicación)
// No depende de nada externo. Solo tipos puros de C#.
// ═══════════════════════════════════════════════════════════════════
namespace ArchClean.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio: User.
    /// Representa un usuario del sistema. No sabe nada de base de datos ni de HTTP.
    /// </summary>
    public sealed class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public User(string name, string email)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            CreatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Entidad de dominio: Order.
    /// Representa un pedido. Contiene lógica de negocio (AddItem, Total).
    /// </summary>
    public sealed class Order
    {
        private readonly List<OrderItem> _items = new();

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
        public decimal Total => _items.Sum(item => item.Price * item.Quantity);

        public Order(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Lógica de dominio: añadir un item al pedido.
        /// Solo se puede añadir si el pedido está pendiente.
        /// </summary>
        public void AddItem(string productName, decimal price, int quantity)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("No se pueden añadir items a un pedido que no está pendiente.");

            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero.", nameof(quantity));

            var item = new OrderItem(productName, price, quantity);
            _items.Add(item);
        }

        public void Confirm()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("No se puede confirmar un pedido sin items.");

            Status = OrderStatus.Confirmed;
        }
    }

    public sealed class OrderItem
    {
        public string ProductName { get; }
        public decimal Price { get; }
        public int Quantity { get; }

        public OrderItem(string productName, decimal price, int quantity)
        {
            ProductName = productName;
            Price = price;
            Quantity = quantity;
        }
    }

    public enum OrderStatus { Pending, Confirmed, Shipped, Delivered, Cancelled }
}

namespace ArchClean.Domain.Interfaces
{
    using ArchClean.Domain.Entities;

    /// <summary>
    /// Abstracción del repositorio de órdenes.
    /// Está en la capa Domain porque define QUÉ operaciones necesitamos,
    /// pero no CÓMO se implementan (eso va en Infrastructure).
    /// </summary>
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId);
        Task SaveAsync(Order order); // En DDD esto se llama "Add" o el Unit of Work lo gestiona
        Task DeleteAsync(Guid id);
    }

    /// <summary>
    /// Interfaz para enviar notificaciones (otro puerto de salida).
    /// </summary>
    public interface INotificationService
    {
        Task SendOrderConfirmationAsync(Guid userId, Guid orderId, decimal total);
    }
}

// ═══════════════════════════════════════════════════════════════════
// CAPA: APPLICATION (casos de uso / application services)
// Orquesta las entidades de dominio y los puertos/interfaces.
// ═══════════════════════════════════════════════════════════════════
namespace ArchClean.Application.DTOs
{
    /// <summary>
    /// DTO para devolver un resumen del pedido (sin exponer la entidad directamente).
    /// </summary>
    public sealed record OrderSummaryDto(
        Guid Id,
        Guid UserId,
        string Status,
        int ItemCount,
        decimal Total,
        DateTime CreatedAt
    );

    public sealed record CreateOrderDto(Guid UserId, List<CreateOrderItemDto> Items);

    public sealed record CreateOrderItemDto(string ProductName, decimal Price, int Quantity);
}

namespace ArchClean.Application.Services
{
    using ArchClean.Application.DTOs;
    using ArchClean.Domain.Entities;
    using ArchClean.Domain.Interfaces;

    /// <summary>
    /// Caso de uso: crear un pedido.
    /// Depende SOLO de abstracciones (IOrderRepository, INotificationService),
    /// nunca de implementaciones concretas.
    /// </summary>
    public sealed class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly INotificationService _notificationService;

        // Inyección de dependencias: recibe las abstracciones, no las implementaciones
        public OrderService(IOrderRepository orderRepository, INotificationService notificationService)
        {
            _orderRepository = orderRepository;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Caso de uso principal: crear un pedido a partir de un DTO.
        /// </summary>
        public async Task<OrderSummaryDto> CreateOrderAsync(CreateOrderDto dto)
        {
            // 1. Crear la entidad de dominio (aplicando reglas de negocio)
            Order order = new(dto.UserId);

            foreach (CreateOrderItemDto itemDto in dto.Items)
            {
                order.AddItem(itemDto.ProductName, itemDto.Price, itemDto.Quantity);
            }

            // 2. Confirmar el pedido (aplica reglas: debe tener al menos un item)
            order.Confirm();

            // 3. Persistir
            await _orderRepository.SaveAsync(order);

            // 4. Notificar (efecto secundario)
            await _notificationService.SendOrderConfirmationAsync(
                order.UserId, order.Id, order.Total);

            // 5. Devolver DTO (nunca devolvemos la entidad directamente)
            return new OrderSummaryDto(
                order.Id,
                order.UserId,
                order.Status.ToString(),
                order.Items.Count,
                order.Total,
                order.CreatedAt
            );
        }

        public async Task<OrderSummaryDto?> GetOrderAsync(Guid orderId)
        {
            Order? order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null) return null;

            return new OrderSummaryDto(
                order.Id,
                order.UserId,
                order.Status.ToString(),
                order.Items.Count,
                order.Total,
                order.CreatedAt
            );
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// CAPA: INFRASTRUCTURE (implementaciones concretas)
// Aquí van EF Core, HttpClient, envío de emails, etc.
// ═══════════════════════════════════════════════════════════════════
namespace ArchClean.Infrastructure.Repositories
{
    using ArchClean.Domain.Entities;
    using ArchClean.Domain.Interfaces;

    /// <summary>
    /// Implementación en memoria del repositorio de órdenes.
    /// En producción esto sería con EF Core + PostgreSQL.
    /// </summary>
    public sealed class InMemoryOrderRepository : IOrderRepository
    {
        private readonly Dictionary<Guid, Order> _orders = new();

        public Task<Order?> GetByIdAsync(Guid id)
        {
            _orders.TryGetValue(id, out Order? order);
            return Task.FromResult(order);
        }

        public Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId)
        {
            IReadOnlyList<Order> result = _orders.Values
                .Where(o => o.UserId == userId)
                .ToList();
            return Task.FromResult(result);
        }

        public Task SaveAsync(Order order)
        {
            _orders[order.Id] = order;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _orders.Remove(id);
            return Task.CompletedTask;
        }
    }
}

namespace ArchClean.Infrastructure.Notifications
{
    using ArchClean.Domain.Interfaces;

    /// <summary>
    /// Implementación simulada del servicio de notificaciones.
    /// En producción enviaría emails vía SMTP o SendGrid.
    /// </summary>
    public sealed class ConsoleNotificationService : INotificationService
    {
        public Task SendOrderConfirmationAsync(Guid userId, Guid orderId, decimal total)
        {
            Console.WriteLine($"  📧 [NOTIFICACIÓN] Email enviado al usuario {userId}:");
            Console.WriteLine($"     Pedido #{orderId} confirmado por {total:C}.");
            return Task.CompletedTask;
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// CAPA: PRESENTATION (Composition Root + ejecución)
// Aquí se "conectan" todas las dependencias. Es el punto de entrada.
// ═══════════════════════════════════════════════════════════════════
namespace ArchClean
{
    using ArchClean.Application.DTOs;
    using ArchClean.Application.Services;
    using ArchClean.Domain.Entities;
    using ArchClean.Domain.Interfaces;
    using ArchClean.Infrastructure.Notifications;
    using ArchClean.Infrastructure.Repositories;

    public static class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║  Módulo 06 — Lección 01: Clean Architecture ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.WriteLine();

            // ─── COMPOSITION ROOT: aquí se crean las dependencias concretas ───
            // En un proyecto real esto lo haría el contenedor DI de .NET.
            // Esto es "Poor Man's Dependency Injection" (inyección manual).
            IOrderRepository orderRepository = new InMemoryOrderRepository();
            INotificationService notificationService = new ConsoleNotificationService();
            OrderService orderService = new(orderRepository, notificationService);

            // ─── CREAR UN USUARIO (sería otra entidad, lo simulamos con un ID) ───
            Guid userId = Guid.NewGuid();
            Console.WriteLine($"👤 Usuario creado: {userId}");
            Console.WriteLine();

            // ─── CASO DE USO: CREAR UN PEDIDO ───
            CreateOrderDto createOrderDto = new(
                UserId: userId,
                Items: new List<CreateOrderItemDto>
                {
                    new("Teclado mecánico", 89.99m, 1),
                    new("Monitor 27\"", 299.99m, 2),
                    new("Hub USB-C", 34.50m, 1)
                }
            );

            Console.WriteLine("📦 Creando pedido...");
            OrderSummaryDto orderSummary = await orderService.CreateOrderAsync(createOrderDto);

            Console.WriteLine();
            Console.WriteLine("✅ Pedido creado exitosamente:");
            Console.WriteLine($"   ID:        {orderSummary.Id}");
            Console.WriteLine($"   Usuario:   {orderSummary.UserId}");
            Console.WriteLine($"   Estado:    {orderSummary.Status}");
            Console.WriteLine($"   Items:     {orderSummary.ItemCount}");
            Console.WriteLine($"   Total:     {orderSummary.Total:C}");
            Console.WriteLine($"   Creado:    {orderSummary.CreatedAt:u}");
            Console.WriteLine();

            // ─── CONSULTAR EL PEDIDO ───
            Console.WriteLine("🔍 Consultando pedido...");
            OrderSummaryDto? fetched = await orderService.GetOrderAsync(orderSummary.Id);
            if (fetched is not null)
            {
                Console.WriteLine($"   Pedido #{fetched.Id} — {fetched.Status} — {fetched.Total:C}");
            }
            Console.WriteLine();

            // ─── EXPLICACIÓN DE LA ARQUITECTURA ───
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("  ARQUITECTURA DE CAPAS (dependencias →)");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("  Presentation → Application → Domain ← Infrastructure");
            Console.WriteLine();
            Console.WriteLine("  Domain:       User, Order, OrderItem, IOrderRepository");
            Console.WriteLine("  Application:  OrderService, DTOs (casos de uso)");
            Console.WriteLine("  Infrastructure: InMemoryOrderRepository, ConsoleNotification");
            Console.WriteLine("  Presentation: Program.Main (Composition Root)");
            Console.WriteLine();
            Console.WriteLine("  ✅ Domain no conoce Entity Framework ni ASP.NET");
            Console.WriteLine("  ✅ Application solo depende de abstracciones (interfaces)");
            Console.WriteLine("  ✅ Infrastructure implementa las interfaces del Domain");
            Console.WriteLine("  ✅ Cambiar PostgreSQL por MongoDB → solo cambia Infrastructure");
            Console.WriteLine("═══════════════════════════════════════════════");

            // =========================================================================
            // COMPARACIÓN: código MAL acoplado vs Clean Architecture
            // =========================================================================
            Console.WriteLine();
            Console.WriteLine("─── CÓDIGO MAL ACOPLADO (viola Clean Architecture) ───");
            Console.WriteLine();
            Console.WriteLine("  // ❌ MAL: Domain conoce Entity Framework");
            Console.WriteLine("  public class Order {");
            Console.WriteLine("      [Key]  // ← Atributo de EF Core en la capa de dominio");
            Console.WriteLine("      public Guid Id { get; set; }");
            Console.WriteLine("  }");
            Console.WriteLine();
            Console.WriteLine("  // ❌ MAL: Application hace new de la implementación concreta");
            Console.WriteLine("  var repo = new SqlServerOrderRepository(connectionString);");
            Console.WriteLine();
            Console.WriteLine("  // ❌ MAL: No hay interfaz IOrderRepository, no se puede testear");
            Console.WriteLine();
            Console.WriteLine("─── CÓDIGO CORRECTO (sigue Clean Architecture) ───");
            Console.WriteLine();
            Console.WriteLine("  // ✅ BIEN: Domain define la interfaz");
            Console.WriteLine("  public interface IOrderRepository { ... }");
            Console.WriteLine();
            Console.WriteLine("  // ✅ BIEN: Application recibe la abstracción por constructor");
            Console.WriteLine("  public OrderService(IOrderRepository repo) { ... }");
            Console.WriteLine();
            Console.WriteLine("  // ✅ BIEN: Infrastructure implementa la abstracción");
            Console.WriteLine("  public class SqlServerOrderRepository : IOrderRepository { ... }");
            Console.WriteLine();
            Console.WriteLine("  // ✅ BIEN: Presentation inyecta la implementación concreta");
            Console.WriteLine("  services.AddScoped<IOrderRepository, SqlServerOrderRepository>();");

            // =========================================================================
            // SEPARACIÓN FÍSICA: carpetas vs proyectos .csproj
            // =========================================================================
            Console.WriteLine();
            Console.WriteLine("─── SEPARACIÓN FÍSICA EN PROYECTOS .csproj ───");
            Console.WriteLine();
            Console.WriteLine("  En proyectos pequeños/medianos:");
            Console.WriteLine("    ✅ Carpetas dentro de un solo .csproj (como este ejemplo)");
            Console.WriteLine("    ✅ Namespaces para separar las capas lógicamente");
            Console.WriteLine();
            Console.WriteLine("  En proyectos grandes/enterprise:");
            Console.WriteLine("    ✅ Proyectos .csproj separados:");
            Console.WriteLine("       src/Cms.Domain/Cms.Domain.csproj");
            Console.WriteLine("       src/Cms.Application/Cms.Application.csproj");
            Console.WriteLine("       src/Cms.Infrastructure/Cms.Infrastructure.csproj");
            Console.WriteLine("       src/Cms.Api/Cms.Api.csproj");
            Console.WriteLine();
            Console.WriteLine("    Ventajas de .csproj separados:");
            Console.WriteLine("      • El compilador fuerza la regla de dependencia");
            Console.WriteLine("      • No puedes usar EF Core en Domain (ni siquiera por error)");
            Console.WriteLine("      • Compilación más rápida (solo recompila lo que cambia)");
            Console.WriteLine("      • Reutilización entre aplicaciones (compartir Domain)");
        }
    }
}
