using Microsoft.EntityFrameworkCore;

// ============================================================================
// MÓDULO 7: Entity Framework Core
// Lección 01 — ¿Qué es un ORM? ¿Por qué existe?
// ============================================================================
//
// HISTORIA: En los años 90, los desarrolladores escribían SQL a mano dentro de
// strings en su código. Algo así como:
//
//   string sql = "SELECT Id, Name, Price FROM Products WHERE Price > " + price;
//                 ^--- concatenar strings es PELIGROSO (SQL Injection)
//
// Y luego mapear cada fila a un objeto manualmente:
//
//   while (reader.Read())
//   {
//       Product p = new Product();
//       p.Id = reader.GetInt32(0);
//       p.Name = reader.GetString(1);
//       p.Price = reader.GetDecimal(2);
//   }
//
// Esto tiene varios PROBLEMAS GRAVES:
// 1. SQL Injection si concatenas strings manualmente.
// 2. Errores de tipado: si la columna 1 no es string, revienta en runtime.
// 3. Si cambias el esquema (renombras columna), toca buscar todos los strings.
// 4. Código repetitivo y aburrido (boilerplate).
// 5. La lógica de negocio se mezcla con SQL.

// ══════════════════════════════════════════════════════════════════════════
// PROGRAMA PRINCIPAL (top-level statements)
// ══════════════════════════════════════════════════════════════════════════

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  MÓDULO 7 — Lección 01: ¿Qué es un ORM? ¿Por qué existe? ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
Console.WriteLine();

// ─── COMPARATIVA: Mismo CRUD con ADO.NET (simulado) vs EF Core ──────────

Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("ENFOQUE 1: ADO.NET manual (simulado)");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

AdoNetProductRepository adoRepo = new();

// READ: Obtener todos los productos
List<Product> allProducts = adoRepo.GetAll();
Console.WriteLine("Todos los productos:");
foreach (Product p in allProducts)
    Console.WriteLine(p);

Console.WriteLine();

// READ con filtro: precio > 100
List<Product> expensiveProducts = adoRepo.GetByPriceGreaterThan(100m);
Console.WriteLine("Productos con precio > $100:");
foreach (Product p in expensiveProducts)
    Console.WriteLine(p);

Console.WriteLine();

// CREATE: Insertar un producto
Console.WriteLine("Insertando 'Monitor 4K'...");
adoRepo.Add("Monitor 4K", 399.99m, "Electrónica");

// DELETE: Eliminar producto
Console.WriteLine("Eliminando producto Id=2...");
adoRepo.Delete(2);

Console.WriteLine();

// ─── Ahora mostramos lo MISMO pero con EF Core — mucho más limpio. ──────

Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("ENFOQUE 2: Entity Framework Core (InMemory)");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

// El bloque using asegura que el DbContext se libere correctamente.
// En aplicaciones reales, el contenedor de DI maneja esto por nosotros.
using (ShopContext context = new())
{
    // 1) ASEGURARNOS DE QUE LA BD INMEMORY ESTÉ CREADA
    context.Database.EnsureCreated();

    // 2) SEMBRAR DATOS INICIALES (seed data)
    if (!context.Products.Any())
    {
        context.Products.AddRange(
            new Product { Name = "Laptop", Price = 1200.00m, Category = "Electrónica" },
            new Product { Name = "Mouse", Price = 25.50m, Category = "Electrónica" },
            new Product { Name = "Teclado mecánico", Price = 89.99m, Category = "Electrónica" },
            new Product { Name = "Libro C# avanzado", Price = 49.90m, Category = "Libros" },
            new Product { Name = "Silla ergonómica", Price = 450.00m, Category = "Muebles" }
        );
        context.SaveChanges();
    }

    // 3) READ: Obtener todos los productos (equivalente a SELECT *)
    Console.WriteLine("Todos los productos con EF Core:");
    List<Product> efAllProducts = context.Products.ToList();
    foreach (Product p in efAllProducts)
        Console.WriteLine(p);

    Console.WriteLine();

    // 4) READ CON FILTRO: precio > 100
    Console.WriteLine("Productos con precio > $100 usando EF Core:");
    decimal threshold = 100m;
    List<Product> efExpensiveProducts = context.Products
        .Where(p => p.Price > threshold)
        .OrderBy(p => p.Price)
        .ToList();

    foreach (Product p in efExpensiveProducts)
        Console.WriteLine(p);

    Console.WriteLine();

    // 5) CREATE: Insertar un nuevo producto
    Console.WriteLine("Insertando 'Monitor 4K' con EF Core...");
    Product newProduct = new()
    {
        Name = "Monitor 4K",
        Price = 399.99m,
        Category = "Electrónica"
    };
    context.Products.Add(newProduct);
    context.SaveChanges();
    Console.WriteLine($"  Nuevo producto: {newProduct}");

    Console.WriteLine();

    // 6) UPDATE: Modificar un producto existente
    Console.WriteLine("Actualizando precio del Mouse a $29.99...");
    Product? mouse = context.Products.FirstOrDefault(p => p.Name == "Mouse");
    if (mouse is not null)
    {
        mouse.Price = 29.99m;
        context.SaveChanges();
        Console.WriteLine($"  Producto actualizado: {mouse}");
    }

    Console.WriteLine();

    // 7) DELETE: Eliminar un producto
    Console.WriteLine("Eliminando 'Silla ergonómica'...");
    Product? chair = context.Products.FirstOrDefault(p => p.Name == "Silla ergonómica");
    if (chair is not null)
    {
        context.Products.Remove(chair);
        context.SaveChanges();
        Console.WriteLine("  Producto eliminado.");
    }
}

// ─── RESUMEN COMPARATIVO ─────────────────────────────────────────────────

Console.WriteLine();
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("COMPARACIÓN: ADO.NET vs EF Core");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine();
Console.WriteLine("  ┌───────────────┬──────────────────────┬───────────────────────────┐");
Console.WriteLine("  │ Aspecto       │ ADO.NET (manual)     │ Entity Framework Core     │");
Console.WriteLine("  ├───────────────┼──────────────────────┼───────────────────────────┤");
Console.WriteLine("  │ Type safety   │ cast manual, errores │ LINQ fuertemente tipado   │");
Console.WriteLine("  │ Productividad │ mucho boilerplate    │ pocas líneas de código    │");
Console.WriteLine("  │ Refactoring   │ buscar strings       │ el compilador avisa       │");
Console.WriteLine("  │ Cambio esquema│ tocar todos los SQL  │ migraciones automáticas   │");
Console.WriteLine("  │ Rendimiento   │ máximo control       │ puede generar SQL lento   │");
Console.WriteLine("  │ Curva aprend. │ SQL estándar         │ hay que entender EF       │");
Console.WriteLine("  │ SQL Injection │ si concatenas strings│ parámetros automáticos    │");
Console.WriteLine("  └───────────────┴──────────────────────┴───────────────────────────┘");

Console.WriteLine();
Console.WriteLine("¿CUÁNDO USAR CADA UNO?");
Console.WriteLine("  EF Core → aplicaciones empresariales, CRUD complejo, cambios frecuentes.");
Console.WriteLine("  Dapper  → micro-ORM para máximo rendimiento, consultas complejas manuales.");
Console.WriteLine("  ADO.NET → control total, APIs de muy alto rendimiento (ej: trading).");

Console.WriteLine();
Console.WriteLine("✅ Fin de la lección 01.");

// ══════════════════════════════════════════════════════════════════════════
// TIPOS DEFINIDOS AL FINAL DEL ARCHIVO
// ══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Representa un producto en nuestro dominio.
/// Misma clase para ambos enfoques — la diferencia está en cómo la persistimos.
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;

    public override string ToString() => $"  [{Id}] {Name} — ${Price:F2} ({Category})";
}

/// <summary>
/// Simula un repositorio ADO.NET con una lista en memoria.
/// En producción real usaríamos SqlConnection, SqlCommand, SqlDataReader.
/// </summary>
public class AdoNetProductRepository
{
    private readonly List<Product> _dataStore;

    public AdoNetProductRepository()
    {
        _dataStore = new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Price = 1200.00m, Category = "Electrónica" },
            new() { Id = 2, Name = "Mouse", Price = 25.50m, Category = "Electrónica" },
            new() { Id = 3, Name = "Teclado mecánico", Price = 89.99m, Category = "Electrónica" },
            new() { Id = 4, Name = "Libro C# avanzado", Price = 49.90m, Category = "Libros" },
            new() { Id = 5, Name = "Silla ergonómica", Price = 450.00m, Category = "Muebles" },
        };
    }

    public List<Product> GetAll()
    {
        Console.WriteLine("  [ADO.NET] Ejecutando: SELECT * FROM Products");
        return _dataStore;
    }

    public List<Product> GetByPriceGreaterThan(decimal minPrice)
    {
        Console.WriteLine($"  [ADO.NET] Ejecutando: SELECT * FROM Products WHERE Price > @minPrice (={minPrice})");
        List<Product> result = new();
        foreach (Product p in _dataStore)
        {
            if (p.Price > minPrice)
                result.Add(p);
        }
        return result;
    }

    public void Add(string name, decimal price, string category)
    {
        int newId = _dataStore.Max(p => p.Id) + 1;
        Product product = new() { Id = newId, Name = name, Price = price, Category = category };
        _dataStore.Add(product);
        Console.WriteLine($"  [ADO.NET] INSERT ejecutado. Nuevo producto con Id={newId}");
    }

    public void Delete(int id)
    {
        Product? product = _dataStore.Find(p => p.Id == id);
        if (product is not null)
        {
            _dataStore.Remove(product);
            Console.WriteLine($"  [ADO.NET] DELETE ejecutado para Id={id}");
        }
    }
}

/// <summary>
/// El DbContext es la "unidad de trabajo" en EF Core.
/// Administra la conexión, el change tracker, y expone los DbSet (tablas).
/// </summary>
public class ShopContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("ShopDb");
    }
}
