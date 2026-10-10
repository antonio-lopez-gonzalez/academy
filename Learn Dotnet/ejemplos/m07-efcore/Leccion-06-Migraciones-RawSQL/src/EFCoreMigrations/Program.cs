using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// ============================================================================
// MÓDULO 7: Entity Framework Core
// Lección 06 — Migraciones, Raw SQL, Value Converters
// ============================================================================
//
// MIGRACIONES: Sistema para evolucionar el esquema de BD con el código.
//   dotnet ef migrations add Nombre       → crea migración
//   dotnet ef database update             → aplica migraciones pendientes
//   dotnet ef migrations script           → genera SQL (revisar en PRs)
//   dotnet ef migrations bundle           → ejecutable autónomo (producción)
//
//   OPERACIONES SEGURAS: añadir columna nullable, nueva tabla, índice.
//   OPERACIONES PELIGROSAS: renombrar columna, cambiar tipo, eliminar.
//
// RAW SQL:
//   FromSqlInterpolated() → SELECT con parámetros (SEGURO).
//   ExecuteSqlInterpolatedAsync() → INSERT/UPDATE/DELETE sin change tracker.
//   NUNCA concatenar strings en SQL → SQL Injection.
//
// VALUE CONVERTERS:
//   Transforman cómo se guarda/lee un valor entre C# y la BD.
//   Ej: enum → string, Value Object → JSON, encriptar/desencriptar.

// ══════════════════════════════════════════════════════════════════════════
// PROGRAMA PRINCIPAL
// ══════════════════════════════════════════════════════════════════════════

Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  MÓDULO 7 — Lección 06: Migraciones, Raw SQL, Value Converters  ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

using (CmsContext context = new())
{
    context.Database.EnsureCreated();

    // ══════════════════════════════════════════════════════════════════════
    // PARTE 1: VALUE CONVERTERS
    // ══════════════════════════════════════════════════════════════════════

    Console.WriteLine("━━━ PARTE 1: Value Converters ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    Console.WriteLine();

    // ─── 1.1) Enum → string ────────────────────────────────────────────

    Console.WriteLine("1.1) Enum guardado como string:");
    Console.WriteLine("     Configuración: .HasConversion<string>()");
    Console.WriteLine("     En BD: 'Draft', 'Published', 'Archived' (texto)");
    Console.WriteLine("     Sin converter: 0, 1, 2 (int)");
    Console.WriteLine();

    List<Post> publishedPosts = context.Posts
        .Where(p => p.Status == PostStatus.Published)
        .ToList();

    Console.WriteLine($"  Posts publicados: {publishedPosts.Count}");
    foreach (Post p in publishedPosts)
        Console.WriteLine($"    - {p.Title} (Status: {p.Status})");

    Console.WriteLine();

    // ─── 1.2) Value Object (Money) → string ────────────────────────────

    Console.WriteLine("1.2) Value Object (Money) guardado como string:");
    Console.WriteLine("     Configuración: .HasConversion(new MoneyToStringConverter())");
    Console.WriteLine("     En BD: '29.99|USD', '49.99|EUR'");
    Console.WriteLine();

    List<Post> paidPosts = context.Posts
        .Where(p => p.Price != null)
        .ToList();

    Console.WriteLine($"  Posts de pago: {paidPosts.Count}");
    foreach (Post p in paidPosts)
        Console.WriteLine($"    - {p.Title} → {p.Price}");

    Console.WriteLine();

    // ─── 1.3) Insertar con Value Converter ─────────────────────────────

    Console.WriteLine("1.3) Insertar un nuevo post con Money y enum:");
    Post newPost = new()
    {
        Title = "Curso avanzado de EF Core",
        Content = "Aprende todo sobre Value Converters, Raw SQL...",
        Status = PostStatus.Published,
        Price = new Money(99.99m, "MXN"),
        PublishedAt = DateTime.UtcNow,
    };

    context.Posts.Add(newPost);
    context.SaveChanges();
    Console.WriteLine($"  Insertado: {newPost}");

    Console.WriteLine();

    // ══════════════════════════════════════════════════════════════════════
    // PARTE 2: RAW SQL
    // ══════════════════════════════════════════════════════════════════════

    Console.WriteLine("━━━ PARTE 2: Raw SQL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    Console.WriteLine();

    Console.WriteLine("2.1) FromSqlInterpolated — consulta SQL directa:");
    Console.WriteLine("     // Con PostgreSQL real:");
    Console.WriteLine("     // string status = \"Published\";");
    Console.WriteLine("     // List<Post> posts = await context.Posts");
    Console.WriteLine("     //     .FromSqlInterpolated($\"SELECT * FROM Posts WHERE Status = {status}\")");
    Console.WriteLine("     //     .ToListAsync();");
    Console.WriteLine();
    Console.WriteLine("     Genera: SELECT * FROM Posts WHERE Status = @p0");
    Console.WriteLine("     ¡SIN riesgo de SQL Injection!");
    Console.WriteLine();

    // ─── SQL Injection — LO QUE NUNCA DEBES HACER ──────────────────────

    Console.WriteLine("2.2) SQL INJECTION — ejemplo de lo que JAMÁS debes hacer:");
    Console.WriteLine();
    Console.WriteLine("     string userInput = \"'; DROP TABLE Posts; --\";");
    Console.WriteLine("     // NUNCA concatenes strings en SQL:");
    Console.WriteLine("     // context.Posts.FromSqlRaw($\"SELECT * FROM Posts WHERE Title = '{userInput}'\");");
    Console.WriteLine();
    Console.WriteLine("     // Esto ejecutaría: ");
    Console.WriteLine("     // SELECT * FROM Posts WHERE Title = ''; DROP TABLE Posts; --'");
    Console.WriteLine();

    Console.WriteLine("2.3) Forma segura con interpolación:");
    Console.WriteLine("     // context.Posts.FromSqlInterpolated(");
    Console.WriteLine("     //     $\"SELECT * FROM Posts WHERE Title = {userInput}\");");
    Console.WriteLine("     // EF Core escapa el parámetro automáticamente.");
    Console.WriteLine();

    // ─── ExecuteSql ────────────────────────────────────────────────────

    Console.WriteLine("2.4) ExecuteSqlInterpolatedAsync:");
    Console.WriteLine("     // await context.Database.ExecuteSqlInterpolatedAsync(");
    Console.WriteLine("     //     $\"UPDATE Posts SET Views = Views + 1 WHERE PostId = {postId}\");");
    Console.WriteLine("     // Más eficiente que cargar entidad, modificar y guardar.");
    Console.WriteLine();

    // ─── Stored Procedures ─────────────────────────────────────────────

    Console.WriteLine("2.5) Stored Procedures:");
    Console.WriteLine("     // var posts = await context.Posts");
    Console.WriteLine("     //     .FromSqlRaw(\"SELECT * FROM get_posts_by_category({0})\", categoryId)");
    Console.WriteLine("     //     .ToListAsync();");
    Console.WriteLine();

    // ══════════════════════════════════════════════════════════════════════
    // PARTE 3: MIGRACIONES
    // ══════════════════════════════════════════════════════════════════════

    Console.WriteLine("━━━ PARTE 3: Migraciones ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    Console.WriteLine();
    Console.WriteLine("COMANDOS ESENCIALES:");
    Console.WriteLine("  dotnet ef migrations add NombreMigracion");
    Console.WriteLine("    → Crea una nueva migración (compara modelo vs snapshot).");
    Console.WriteLine();
    Console.WriteLine("  dotnet ef database update");
    Console.WriteLine("    → Aplica todas las migraciones pendientes.");
    Console.WriteLine();
    Console.WriteLine("  dotnet ef migrations remove");
    Console.WriteLine("    → Elimina la ÚLTIMA migración (solo si NO se ha aplicado).");
    Console.WriteLine();
    Console.WriteLine("  dotnet ef migrations script");
    Console.WriteLine("    → Genera el script SQL de todas las migraciones.");
    Console.WriteLine();
    Console.WriteLine("  dotnet ef migrations bundle");
    Console.WriteLine("    → Genera ejecutable autónomo (producción).");
    Console.WriteLine();

    Console.WriteLine("ESTRUCTURA DE MIGRACIÓN:");
    Console.WriteLine("  Migrations/");
    Console.WriteLine("  ├── 20240101120000_InitialCreate.cs");
    Console.WriteLine("  │     Up()   → CreateTable, AddColumn... (aplicar)");
    Console.WriteLine("  │     Down() → DropTable, DropColumn...  (revertir)");
    Console.WriteLine("  └── CmsContextModelSnapshot.cs");

    Console.WriteLine();
    Console.WriteLine("━━━ SEGURIDAD EN PRODUCCIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    Console.WriteLine();
    Console.WriteLine("  OPERACIONES SEGURAS (sin downtime):");
    Console.WriteLine("    Añadir columna nullable con valor por defecto.");
    Console.WriteLine("    Añadir tabla nueva.");
    Console.WriteLine("    Crear índice (CONCURRENTLY en PostgreSQL).");
    Console.WriteLine();
    Console.WriteLine("  OPERACIONES PELIGROSAS (plan B necesario):");
    Console.WriteLine("    Renombrar columna → EF ve DROP + ADD = PÉRDIDA DE DATOS.");
    Console.WriteLine("    Cambiar tipo de columna → ALTER COLUMN puede fallar.");
    Console.WriteLine("    Eliminar columna/tabla → irreversible sin backup.");
    Console.WriteLine();
    Console.WriteLine("  REGLA DE ORO: Revisar SIEMPRE con:");
    Console.WriteLine("    dotnet ef migrations script --idempotent -o migrations.sql");
    Console.WriteLine();

    Console.WriteLine("━━━ MIGRACIONES EN CI/CD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    Console.WriteLine();
    Console.WriteLine("  Opción 1 (dev/staging): context.Database.MigrateAsync();");
    Console.WriteLine("  Opción 2 (producción):  dotnet ef migrations bundle");
    Console.WriteLine("    ./efbundle --connection \"$CONNECTION_STRING\"");
    Console.WriteLine("  Opción 3 (máximo control): dotnet ef migrations script");
    Console.WriteLine("    psql -f migrations.sql");

    Console.WriteLine();

    // ══════════════════════════════════════════════════════════════════════
    // PARTE 4: EJEMPLOS ADICIONALES DE VALUE CONVERTERS
    // ══════════════════════════════════════════════════════════════════════

    Console.WriteLine("━━━ PARTE 4: Otros ejemplos de Value Converters ━━━━━━━━━━━━");
    Console.WriteLine();
    Console.WriteLine("  // URL → string (guardar Uri como texto)");
    Console.WriteLine("  entity.Property(b => b.SiteUrl)");
    Console.WriteLine("      .HasConversion(uri => uri.ToString(), s => new Uri(s));");
    Console.WriteLine();
    Console.WriteLine("  // Guid → string (más legible en SQL)");
    Console.WriteLine("  entity.Property(p => p.TrackingId).HasConversion<string>();");
    Console.WriteLine();
    Console.WriteLine("  // DateTime → Unix timestamp (long)");
    Console.WriteLine("  entity.Property(p => p.EventDate)");
    Console.WriteLine("      .HasConversion(");
    Console.WriteLine("          dt => dt.ToUnixTimeSeconds(),");
    Console.WriteLine("          ts => DateTimeOffset.FromUnixTimeSeconds(ts));");
    Console.WriteLine();
    Console.WriteLine("  // Cifrado/descifrado automático");
    Console.WriteLine("  entity.Property(u => u.CreditCard)");
    Console.WriteLine("      .HasConversion(");
    Console.WriteLine("          plain => Encrypt(plain), encrypted => Decrypt(encrypted));");
    Console.WriteLine();
    Console.WriteLine("  // Enum con valor por defecto (HasSentinel)");
    Console.WriteLine("  entity.Property(p => p.Role)");
    Console.WriteLine("      .HasSentinel(UserRole.Unknown)");
    Console.WriteLine("      .HasConversion<string>();");
}

// ─── RESUMEN ─────────────────────────────────────────────────────────────

Console.WriteLine();
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("RESUMEN: Migraciones, Raw SQL y Value Converters");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine();
Console.WriteLine("  MIGRACIONES:");
Console.WriteLine("    dotnet ef migrations add → crea migración");
Console.WriteLine("    dotnet ef database update → aplica migraciones");
Console.WriteLine("    dotnet ef migrations script → revisar antes de aplicar");
Console.WriteLine();
Console.WriteLine("  RAW SQL:");
Console.WriteLine("    FromSqlInterpolated()          → SELECT con parámetros (seguro)");
Console.WriteLine("    ExecuteSqlInterpolatedAsync()  → INSERT/UPDATE/DELETE");
Console.WriteLine("    NUNCA concatenar strings en SQL → SQL Injection");
Console.WriteLine();
Console.WriteLine("  VALUE CONVERTERS:");
Console.WriteLine("    HasConversion<string>() → enum/guid como string");
Console.WriteLine("    Custom converter        → transformación propia (JSON, cifrado)");
Console.WriteLine("    HasSentinel()           → valor por defecto del converter");
Console.WriteLine();

Console.WriteLine("  SQL INJECTION:");
Console.WriteLine("    FromSqlRaw($\"...'{userInput}'\") → concatenación (PELIGRO).");
Console.WriteLine("    FromSqlInterpolated($\"...{userInput}\") → parámetro (SEGURO).");

Console.WriteLine();
Console.WriteLine("✅ Fin de la lección 06.");
Console.WriteLine("🎉 Fin del Módulo 7 — Entity Framework Core.");


// ══════════════════════════════════════════════════════════════════════════
// TIPOS DEFINIDOS AL FINAL DEL ARCHIVO
// ══════════════════════════════════════════════════════════════════════════

public enum PostStatus
{
    Draft,
    Published,
    Archived,
}

public class Money
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public PostStatus Status { get; set; } = PostStatus.Draft;
    public Money? Price { get; set; }
    public DateTime PublishedAt { get; set; }

    public override string ToString()
        => $"[{PostId}] {Title} — Status: {Status}, Price: {Price?.ToString() ?? "N/A"}";
}

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

/// <summary>
/// Convierte el Value Object "Money" a string y viceversa.
/// </summary>
public class MoneyToStringConverter : ValueConverter<Money?, string>
{
    private static Money? ToMoney(string dbValue)
    {
        if (string.IsNullOrEmpty(dbValue)) return null;
        string[] parts = dbValue.Split('|');
        return new Money(decimal.Parse(parts[0]), parts[1]);
    }

    public MoneyToStringConverter()
        : base(
            money => money == null ? "" : $"{money.Amount}|{money.Currency}",
            dbValue => ToMoney(dbValue))
    { }
}

public class CmsContext : DbContext
{
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("CmsDb");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.PostId);

            entity.Property(p => p.Title).IsRequired().HasMaxLength(300);
            entity.Property(p => p.Content).IsRequired();

            // Value Converter: enum → string
            entity.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Value Converter: Money → string
            entity.Property(p => p.Price)
                .HasConversion(new MoneyToStringConverter())
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.CategoryId);
            entity.HasIndex(c => c.Slug).IsUnique();
        });

        // Seed data
        modelBuilder.Entity<Post>().HasData(
            new Post
            {
                PostId = 1,
                Title = "Guía de EF Core Migrations",
                Content = "Las migraciones son esenciales para CI/CD...",
                Status = PostStatus.Published,
                Price = new Money(29.99m, "USD"),
                PublishedAt = new DateTime(2024, 6, 1),
            },
            new Post
            {
                PostId = 2,
                Title = "Value Converters en EF Core",
                Content = "Los value converters permiten transformar...",
                Status = PostStatus.Published,
                Price = new Money(49.99m, "EUR"),
                PublishedAt = new DateTime(2024, 7, 15),
            },
            new Post
            {
                PostId = 3,
                Title = "Nuevo post en borrador",
                Content = "Este post aún no está publicado...",
                Status = PostStatus.Draft,
                Price = null,
                PublishedAt = new DateTime(2024, 8, 1),
            }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = ".NET", Slug = "dotnet" },
            new Category { CategoryId = 2, Name = "Entity Framework", Slug = "entity-framework" },
            new Category { CategoryId = 3, Name = "Bases de Datos", Slug = "bases-datos" }
        );
    }
}
