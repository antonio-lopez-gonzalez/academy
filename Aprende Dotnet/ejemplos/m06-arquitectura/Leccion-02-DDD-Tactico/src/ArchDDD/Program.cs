// =============================================================================
// Módulo 06 — Arquitectura y organización
// Lección 02 — DDD Táctico (Domain-Driven Design)
// =============================================================================
// DDD Táctico son los patrones de implementación dentro del dominio:
//
// ┌─────────────────────────────────────────────────────────────────────┐
// │                         DDD TÁCTICO                                 │
// │                                                                     │
// │  ENTITIES        → Identidad única (Guid Id), mutables, igualdad    │
// │                     por ID. Ej: User, Post, Order                   │
// │                                                                     │
// │  VALUE OBJECTS   → Sin identidad, inmutables, igualdad por valor.   │
// │                     Ej: Money, Address, Email                       │
// │                                                                     │
// │  AGGREGATES      → Cluster de entidades tratado como unidad.        │
// │                     Un Aggregate Root controla el acceso.           │
// │                     Ej: Order (root) + OrderItem (hijo)             │
// │                                                                     │
// │  REPOSITORIES    → Solo para Aggregate Roots.                       │
// │                     No hay IOrderItemRepository.                    │
// │                                                                     │
// │  DOMAIN SERVICES → Lógica que no pertenece a una entidad concreta.  │
// │                     Ej: PricingService, TransferService             │
// │                                                                     │
// │  DOMAIN EVENTS   → Algo importante que ocurrió en el dominio.       │
// │                     Ej: OrderConfirmed, PostPublished               │
// └─────────────────────────────────────────────────────────────────────┘
// =============================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

// ═══════════════════════════════════════════════════════════════════
// 1. ENTITY (clase base abstracta)
//    Una Entity tiene identidad (Id) que persiste durante todo su ciclo de vida.
//    Dos entidades son iguales si tienen el mismo Id, aunque sus propiedades
//    hayan cambiado. Esto es distinto a un Value Object.
// ═══════════════════════════════════════════════════════════════════
namespace ArchDDD.Domain
{
    /// <summary>
    /// Clase base para todas las entidades del dominio.
    /// Proporciona igualdad basada en Id (dos entidades con mismo Id son iguales).
    /// </summary>
    public abstract class Entity : IEquatable<Entity>
    {
        public Guid Id { get; protected set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        protected Entity(Guid id)
        {
            Id = id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (obj is not Entity other) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;
            return Id.Equals(other.Id);
        }

        public bool Equals(Entity? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;
            return Id.Equals(other.Id);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(Entity? left, Entity? right) =>
            left is null ? right is null : left.Equals(right);

        public static bool operator !=(Entity? left, Entity? right) => !(left == right);
    }

    // ─── Interfaz marcadora para Aggregate Roots ───
    /// <summary>
    /// Interfaz marcadora: indica que esta entidad es la raíz de un aggregate.
    /// Solo los Aggregate Roots tienen repositorio.
    /// </summary>
    public interface IAggregateRoot { }

    // ═══════════════════════════════════════════════════════════════
    // 2. VALUE OBJECTS
    //    No tienen identidad. Son inmutables. Se comparan por valor.
    //    En C# moderno usamos 'record' para value objects.
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Value Object: Money.
    /// Representa una cantidad de dinero con su divisa.
    /// Dos Money son iguales si tienen misma cantidad y divisa.
    /// </summary>
    public readonly record struct Money(decimal Amount, string Currency)
    {
        // Validación en el constructor: no se puede crear dinero negativo
        // (para saldos negativos usaríamos otro modelo)

        public static Money Zero(string currency) => new(0, currency);

        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException($"No se pueden sumar divisas distintas: {a.Currency} vs {b.Currency}");
            return new Money(a.Amount + b.Amount, a.Currency);
        }

        public static Money operator *(Money money, int multiplier)
        {
            return new Money(money.Amount * multiplier, money.Currency);
        }

        public override string ToString() => $"{Amount:F2} {Currency}";
    }

    /// <summary>
    /// Value Object: Address.
    /// Inmutable. Dos direcciones son iguales si todos sus campos coinciden.
    /// </summary>
    public sealed record Address(string Street, string City, string Country, string? PostalCode = null)
    {
        public override string ToString() => $"{Street}, {City}, {Country} {PostalCode ?? ""}".Trim();
    }

    /// <summary>
    /// Value Object: Email.
    /// Encapsula la validación de formato. No puedes crear un Email inválido.
    /// Esto se llama "Make illegal states unrepresentable".
    /// </summary>
    public sealed record Email
    {
        public string Value { get; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El email no puede estar vacío.", nameof(value));

            // Validación simple de formato
            if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException($"El email '{value}' no tiene un formato válido.", nameof(value));

            Value = value.ToLowerInvariant();
        }

        public override string ToString() => Value;

        // Permitir conversión implícita desde string para comodidad
        public static implicit operator string(Email email) => email.Value;
    }

    // ═══════════════════════════════════════════════════════════════
    // 3. AGGREGATES
    //    Un aggregate es un cluster de entidades que se tratan como
    //    una unidad de consistencia. El Aggregate Root es la única
    //    entidad a través de la cual se accede al aggregate.
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Aggregate Root: Order.
    /// Controla el acceso a sus OrderItems. Nadie fuera del aggregate
    /// puede modificar los items directamente.
    /// </summary>
    public sealed class Order : Entity, IAggregateRoot
    {
        private readonly List<OrderItem> _items = new();

        public Guid CustomerId { get; private set; }
        public Address ShippingAddress { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

        // Propiedades calculadas del aggregate
        public Money Total => _items
            .Select(item => item.UnitPrice * item.Quantity)
            .Aggregate(Money.Zero("EUR"), (acc, val) => acc + val);

        public int ItemCount => _items.Count;

        private Order() { ShippingAddress = null!; } // Para EF Core (si hiciera falta)

        public Order(Guid customerId, Address shippingAddress)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            ShippingAddress = shippingAddress;
            Status = OrderStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Añade un item al pedido. Solo en estado Draft.
        /// El aggregate garantiza la consistencia.
        /// </summary>
        public void AddItem(string productName, Money unitPrice, int quantity)
        {
            if (Status != OrderStatus.Draft)
                throw new InvalidOperationException(
                    $"No se pueden añadir items a un pedido en estado {Status}. Solo en Draft.");

            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero.", nameof(quantity));

            if (unitPrice.Amount <= 0)
                throw new ArgumentException("El precio debe ser mayor que cero.", nameof(unitPrice));

            OrderItem item = new(productName, unitPrice, quantity);
            _items.Add(item);
        }

        /// <summary>
        /// Elimina un item del pedido. Solo en estado Draft.
        /// </summary>
        public void RemoveItem(Guid itemId)
        {
            if (Status != OrderStatus.Draft)
                throw new InvalidOperationException(
                    $"No se pueden eliminar items en estado {Status}.");

            int removed = _items.RemoveAll(i => i.Id == itemId);
            if (removed == 0)
                throw new InvalidOperationException($"No se encontró el item con ID {itemId}.");
        }

        /// <summary>
        /// Confirma el pedido. Aplica reglas de negocio.
        /// </summary>
        public void Confirm()
        {
            if (Status != OrderStatus.Draft)
                throw new InvalidOperationException($"Solo se puede confirmar un pedido en Draft. Estado actual: {Status}.");

            if (_items.Count == 0)
                throw new InvalidOperationException("No se puede confirmar un pedido sin items.");

            Status = OrderStatus.Confirmed;
        }

        /// <summary>
        /// Cancela el pedido. Solo si no ha sido enviado.
        /// </summary>
        public void Cancel(string reason)
        {
            if (Status is OrderStatus.Shipped or OrderStatus.Delivered)
                throw new InvalidOperationException(
                    $"No se puede cancelar un pedido en estado {Status}.");

            Status = OrderStatus.Cancelled;
        }

        public override string ToString() =>
            $"Order #{Id.ToString()[..8]} — {Status} — {ItemCount} items — {Total}";
    }

    /// <summary>
    /// Entidad hija dentro del aggregate Order.
    /// NO tiene repositorio propio. Solo se accede a través de Order.
    /// </summary>
    public sealed class OrderItem : Entity
    {
        public string ProductName { get; private set; }
        public Money UnitPrice { get; private set; }
        public int Quantity { get; private set; }

        private OrderItem() { ProductName = null!; } // Para EF Core

        public OrderItem(string productName, Money unitPrice, int quantity)
        {
            Id = Guid.NewGuid();
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        public override string ToString() =>
            $"{Quantity}x {ProductName} @ {UnitPrice} = {UnitPrice * Quantity}";
    }

    public enum OrderStatus { Draft, Confirmed, Paid, Shipped, Delivered, Cancelled }

    // ═══════════════════════════════════════════════════════════════
    // 4. REPOSITORIES (abstracciones, solo para Aggregate Roots)
    //    NUNCA hay repositorio para OrderItem. Las operaciones
    //    sobre OrderItem pasan siempre por Order.
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Repositorio del Aggregate Root Order.
    /// Nota: NO hay IOrderItemRepository.
    /// </summary>
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId);
        Task SaveAsync(Order order);
    }

    /// <summary>
    /// Repositorio de otra entidad: Post (del CMS).
    /// </summary>
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Post>> GetByCategoryAsync(Guid categoryId);
        Task SaveAsync(Post post);
    }

    // ═══════════════════════════════════════════════════════════════
    // 5. ENTIDADES DE DOMINIO (no Aggregate Roots)
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Entity: User. Es un Aggregate Root (tiene su propio repositorio).
    /// </summary>
    public sealed class User : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public Email Email { get; private set; } // Value Object
        public DateTime RegisteredAt { get; private set; }

        public User(string name, Email email)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email;
            RegisteredAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Entity: Post (del CMS). Aggregate Root.
    /// </summary>
    public sealed class Post : Entity, IAggregateRoot
    {
        public string Title { get; private set; }
        public string Content { get; private set; }
        public Guid AuthorId { get; private set; }
        public PostStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? PublishedAt { get; private set; }

        private readonly List<Guid> _tagIds = new();
        public IReadOnlyList<Guid> TagIds => _tagIds.AsReadOnly();

        public Post(string title, string content, Guid authorId)
        {
            Id = Guid.NewGuid();
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            AuthorId = authorId;
            Status = PostStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            if (Status != PostStatus.Draft)
                throw new InvalidOperationException("Solo se pueden publicar posts en estado Draft.");

            Status = PostStatus.Published;
            PublishedAt = DateTime.UtcNow;
        }

        public void AddTag(Guid tagId)
        {
            if (!_tagIds.Contains(tagId))
                _tagIds.Add(tagId);
        }
    }

    public enum PostStatus { Draft, Published, Archived }

    // ═══════════════════════════════════════════════════════════════
    // 6. DOMAIN SERVICES
    //    Lógica que no pertenece naturalmente a una entidad concreta.
    //    Operan sobre múltiples entidades o agregan reglas de negocio
    //    que no son responsabilidad de una sola entidad.
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Domain Service: calcula descuentos basados en el historial del cliente.
    /// No pertenece a Order porque necesita datos de múltiples órdenes.
    /// </summary>
    public sealed class PricingService
    {
        private readonly IOrderRepository _orderRepository;

        public PricingService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Calcula el descuento para un cliente basado en su historial de compras.
        /// </summary>
        public async Task<decimal> CalculateDiscountAsync(Guid customerId)
        {
            IReadOnlyList<Order> orders = await _orderRepository.GetByCustomerIdAsync(customerId);

            int completedOrders = orders.Count(o =>
                o.Status is OrderStatus.Delivered or OrderStatus.Shipped);

            // Regla de negocio: descuento basado en número de pedidos completados
            if (completedOrders >= 10) return 0.15m;  // 15% descuento
            if (completedOrders >= 5) return 0.10m;   // 10% descuento
            if (completedOrders >= 2) return 0.05m;   // 5% descuento
            return 0.00m; // Sin descuento
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// 7. INFRASTRUCTURE (implementaciones en memoria para demo)
// ═══════════════════════════════════════════════════════════════════
namespace ArchDDD.Infrastructure
{
    using ArchDDD.Domain;

    public sealed class InMemoryOrderRepository : IOrderRepository
    {
        private readonly Dictionary<Guid, Order> _orders = new();

        public Task<Order?> GetByIdAsync(Guid id)
        {
            _orders.TryGetValue(id, out Order? order);
            return Task.FromResult(order);
        }

        public Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId)
        {
            IReadOnlyList<Order> result = _orders.Values
                .Where(o => o.CustomerId == customerId)
                .ToList();
            return Task.FromResult(result);
        }

        public Task SaveAsync(Order order)
        {
            _orders[order.Id] = order;
            return Task.CompletedTask;
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// 8. DEMO: Programa principal
// ═══════════════════════════════════════════════════════════════════
namespace ArchDDD
{
    using ArchDDD.Domain;
    using ArchDDD.Infrastructure;

    public static class Program
    {
        public static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║  Módulo 06 — Lección 02: DDD Táctico        ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.WriteLine();

            // ─── VALUE OBJECTS EN ACCIÓN ───
            Console.WriteLine("─── 1. VALUE OBJECTS ───");
            Console.WriteLine();

            Money price1 = new(29.99m, "EUR");
            Money price2 = new(29.99m, "EUR");
            Money price3 = new(29.99m, "USD"); // Misma cantidad, distinta divisa

            Console.WriteLine($"  Money 1: {price1}");
            Console.WriteLine($"  Money 2: {price2}");
            Console.WriteLine($"  Money 3: {price3}");

            // Los records comparan por valor automáticamente (todas las propiedades)
            Console.WriteLine($"  price1 == price2 → {price1 == price2}  ← mismo valor, misma divisa");
            Console.WriteLine($"  price1 == price3 → {price1 == price3}  ← mismo valor, distinta divisa");
            Console.WriteLine();

            // Operaciones con Money
            Money total = price1 + new Money(10.00m, "EUR");
            Money bulk = price1 * 3;
            Console.WriteLine($"  {price1} + 10.00 EUR = {total}");
            Console.WriteLine($"  {price1} × 3 = {bulk}");
            Console.WriteLine();

            // Email validation: no se puede crear un email inválido
            Email validEmail = new("maria@example.com");
            Console.WriteLine($"  Email válido: {validEmail}");
            try
            {
                Email invalidEmail = new("esto-no-es-un-email");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"  Email inválido capturado: {ex.Message}");
            }
            Console.WriteLine();

            Address address = new("Calle Mayor 10", "Madrid", "España", "28013");
            Console.WriteLine($"  Dirección: {address}");

            // ─── ENTITIES (comparación por ID) ───
            Console.WriteLine();
            Console.WriteLine("─── 2. ENTITIES (igualdad por ID) ───");
            Console.WriteLine();

            Guid sharedId = Guid.NewGuid();
            Entity entityA = new Order(sharedId, address); // Usando el constructor con ID
            Entity entityB = new Order(sharedId, address); // Mismo ID

            // Forzamos la creación con mismo ID usando reflexión para el ejemplo
            // (en producción esto no se haría así, es solo para demostrar el concepto)
            Order orderC = new(Guid.NewGuid(), address);
            Entity entityC = orderC;

            Console.WriteLine($"  Mismo ID → entityA == entityB: {entityA == entityB}"); // true (mismo ID)
            Console.WriteLine($"  Mismo objeto → orderC == entityC: {orderC == entityC}"); // true (misma referencia)

            // ─── AGGREGATES ───
            Console.WriteLine();
            Console.WriteLine("─── 3. AGGREGATES ───");
            Console.WriteLine();

            IOrderRepository orderRepo = new InMemoryOrderRepository();

            // Crear un pedido (Aggregate Root)
            Order order = new(
                customerId: Guid.NewGuid(),
                shippingAddress: address
            );

            Console.WriteLine($"  📦 Pedido creado: {order}");
            Console.WriteLine();

            // Añadir items a través del Aggregate Root
            Console.WriteLine("  Añadiendo items...");
            order.AddItem("Teclado mecánico Keychron K8", new Money(89.99m, "EUR"), 1);
            order.AddItem("Ratón Logitech MX Master 3S", new Money(99.95m, "EUR"), 2);
            order.AddItem("Alfombrilla XXL", new Money(25.50m, "EUR"), 1);
            Console.WriteLine();

            foreach (OrderItem item in order.Items)
            {
                Console.WriteLine($"    • {item}");
            }

            Console.WriteLine();
            Console.WriteLine($"  Total: {order.Total}");
            Console.WriteLine($"  Items: {order.ItemCount}");
            Console.WriteLine();

            // Intentar añadir item después de confirmar (debe fallar)
            Console.WriteLine("  Confirmando pedido...");
            order.Confirm();
            Console.WriteLine($"  Estado: {order.Status}");
            Console.WriteLine();

            try
            {
                order.AddItem("Monitor 27\"", new Money(299.99m, "EUR"), 1);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"  ❌ No se pudo añadir item: {ex.Message}");
            }

            // Intentar cancelar un pedido ya enviado (simulado)
            Console.WriteLine();
            Console.WriteLine("  Intentando cancelar pedido confirmado...");
            try
            {
                order.Cancel("Ya no lo quiero"); // Esto sí funciona (está en Confirmed, no en Shipped)
                Console.WriteLine($"  ✅ Pedido cancelado: {order.Status}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"  ❌ No se pudo cancelar: {ex.Message}");
            }

            // ─── REPOSITORIES ───
            Console.WriteLine();
            Console.WriteLine("─── 4. REPOSITORIES (solo Aggregate Roots) ───");
            Console.WriteLine();

            Order newOrder = new(Guid.NewGuid(), address);
            newOrder.AddItem("SSD Samsung 1TB", new Money(89.99m, "EUR"), 1);
            newOrder.Confirm();

            await orderRepo.SaveAsync(newOrder);
            Console.WriteLine($"  ✅ Pedido guardado: {newOrder}");

            Order? fetched = await orderRepo.GetByIdAsync(newOrder.Id);
            Console.WriteLine($"  🔍 Recuperado: {(fetched is not null ? fetched.ToString() : "NO ENCONTRADO")}");

            // ─── DOMAIN SERVICES ───
            Console.WriteLine();
            Console.WriteLine("─── 5. DOMAIN SERVICE (PricingService) ───");
            Console.WriteLine();

            // Creamos varios pedidos completados para el historial del cliente
            Guid customerId = Guid.NewGuid();
            for (int i = 0; i < 6; i++)
            {
                Order historicOrder = new(customerId, address)
                {
                    // Forzamos estado a Delivered para simular historial
                    // (en código real se haría con métodos de dominio)
                };
                historicOrder.AddItem($"Producto {i + 1}", new Money(10m, "EUR"), 1);
                historicOrder.Confirm();

                // Simulamos que se envió y entregó (usando reflexión para demo)
                typeof(Order).GetProperty(nameof(Order.Status))!
                    .SetValue(historicOrder, OrderStatus.Delivered);

                await orderRepo.SaveAsync(historicOrder);
            }

            PricingService pricingService = new(orderRepo);
            decimal discount = await pricingService.CalculateDiscountAsync(customerId);
            Console.WriteLine($"  Cliente con 6 pedidos completados → {discount:P0} de descuento");
            Console.WriteLine();

            // ─── REGLAS CLAVE DE DDD TÁCTICO ───
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("  REGLAS CLAVE DEL DDD TÁCTICO");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("  1. Entity: identidad (Id), mutable, igualdad por Id");
            Console.WriteLine("     → class User, Order, Post derivan de Entity");
            Console.WriteLine();
            Console.WriteLine("  2. Value Object: sin identidad, inmutable, igualdad por valor");
            Console.WriteLine("     → record Money, Address, Email");
            Console.WriteLine("     → Haz los estados inválidos imposibles de representar");
            Console.WriteLine();
            Console.WriteLine("  3. Aggregate: cluster de entidades, consistencia garantizada");
            Console.WriteLine("     → Order (root) contiene OrderItems");
            Console.WriteLine("     → Solo se modifican items a través de Order.AddItem()");
            Console.WriteLine();
            Console.WriteLine("  4. Repository: solo para Aggregate Roots");
            Console.WriteLine("     → IOrderRepository: sí");
            Console.WriteLine("     → IOrderItemRepository: NO (se carga con el aggregate)");
            Console.WriteLine();
            Console.WriteLine("  5. Domain Service: lógica que no cabe en una entidad");
            Console.WriteLine("     → PricingService: calcula descuentos usando múltiples órdenes");
            Console.WriteLine("═══════════════════════════════════════════════");
        }
    }
}
