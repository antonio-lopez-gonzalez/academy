using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ============================================================================
// MÓDULO 7: Entity Framework Core
// Lección 02 — DbContext, DbSet, Fluent API vs Data Annotations
// ============================================================================
//
// DbContext es el CORAZÓN de EF Core. Representa:
//   - Una sesión con la base de datos (conexión).
//   - Una "unidad de trabajo" (Unit of Work): agrupa cambios y los persiste juntos.
//   - El "change tracker": detecta qué entidades fueron modificadas.
//
// DbSet<T> representa una tabla en la base de datos.
//
// CONFIGURACIÓN DEL MODELO — hay 3 formas:
//   1. POR CONVENCIÓN: EF Core adivina nombres de tabla, claves, relaciones.
//   2. DATA ANNOTATIONS: [Table], [Key], [Required], [MaxLength] en la entidad.
//   3. FLUENT API: modelBuilder en OnModelCreating. Recomendado para proyectos reales.

// ══════════════════════════════════════════════════════════════════════════
// PROGRAMA PRINCIPAL (top-level statements)
// ══════════════════════════════════════════════════════════════════════════

Console.WriteLine("╔═══════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  MÓDULO 7 — Lección 02: DbContext, DbSet, Fluent API vs Data...  ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// ─── DEMOSTRACIÓN DE DbContext Y DbSet ───────────────────────────────────

using (BlogContext context = new())
{
    context.Database.EnsureCreated();

    // ─── CONSULTAS CON DbSet ────────────────────────────────────────────

    Console.WriteLine("━━━ BLOGS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    List<Blog> blogs = context.Blogs.ToList();
    foreach (Blog blog in blogs)
        Console.WriteLine(blog);

    Console.WriteLine();
    Console.WriteLine("━━━ POSTS (con Include para cargar el Blog) ━━━━━━━━━━━━");

    List<Post> posts = context.Posts
        .Include(p => p.Blog)
        .OrderBy(p => p.PublishedAt)
        .ToList();

    foreach (Post post in posts)
    {
        Console.WriteLine(post);
        Console.WriteLine($"     → Pertenece al blog: {post.Blog.Name}");
    }

    Console.WriteLine();

    // ─── INSERTAR UN NUEVO POST ──────────────────────────────────────────

    Console.WriteLine("━━━ INSERTAR NUEVO POST ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

    Post newPost = new()
    {
        Title = "Novedades en C# 12",
        Content = "Primary constructors, collection expressions...",
        BlogId = 1,
        PublishedAt = DateTime.UtcNow
    };

    context.Posts.Add(newPost);
    context.SaveChanges();

    Console.WriteLine($"  Insertado: {newPost}");

    Console.WriteLine();

    // ─── ACTUALIZAR UN POST ──────────────────────────────────────────────

    Console.WriteLine("━━━ ACTUALIZAR POST ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

    Post? postToUpdate = context.Posts.FirstOrDefault(p => p.PostId == 1);
    if (postToUpdate is not null)
    {
        Console.WriteLine($"  Antes: {postToUpdate.Title}");
        postToUpdate.Title = "Guía completa de EF Core 8";
        context.SaveChanges();
        Console.WriteLine($"  Después: {postToUpdate.Title}");
    }

    Console.WriteLine();

    // ─── MOSTRAR SQL GENERADO CON ToQueryString() ────────────────────────

    Console.WriteLine("━━━ SQL GENERADO (para debug) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

    IQueryable<Post> expensivePosts = context.Posts
        .Where(p => p.Title.Contains("EF Core"))
        .OrderBy(p => p.PublishedAt);

    Console.WriteLine($"  {expensivePosts.ToQueryString()}");
    Console.WriteLine();

    List<Post> results = expensivePosts.ToList();
    Console.WriteLine($"  Resultados: {results.Count} post(s)");
    foreach (Post p in results)
        Console.WriteLine(p);
}

// ─── RESUMEN ─────────────────────────────────────────────────────────────

Console.WriteLine();
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("FLUENT API vs DATA ANNOTATIONS — ¿Cuál usar?");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine();
Console.WriteLine("  ┌──────────────────┬────────────────────────┬──────────────────────────┐");
Console.WriteLine("  │                  │ Data Annotations       │ Fluent API               │");
Console.WriteLine("  ├──────────────────┼────────────────────────┼──────────────────────────┤");
Console.WriteLine("  │ Ubicación        │ Atributos en entidad   │ OnModelCreating central  │");
Console.WriteLine("  │ Acoplamiento     │ Entidad depende de EF  │ Entidad limpia (POCO)    │");
Console.WriteLine("  │ Potencia         │ Limitada               │ Índices, filtros, etc.   │");
Console.WriteLine("  │ Clean Architect. │ Rompe la separación    │ Respeta la separación    │");
Console.WriteLine("  │ Recomendación    │ Proyectos pequeños      │ Proyectos profesionales  │");
Console.WriteLine("  └──────────────────┴────────────────────────┴──────────────────────────┘");
Console.WriteLine();
Console.WriteLine("REGLA DE ORO:");
Console.WriteLine("  Fluent API siempre gana si hay conflicto con Data Annotations.");
Console.WriteLine("  En Clean Architecture: entidades LIMPIAS + Fluent API en Infraestructura.");

Console.WriteLine();
Console.WriteLine("✅ Fin de la lección 02.");


// ══════════════════════════════════════════════════════════════════════════
// TIPOS DEFINIDOS AL FINAL DEL ARCHIVO
// ══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Representa un blog. Un blog tiene muchos posts.
/// </summary>
public class Blog
{
    public int BlogId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Post> Posts { get; set; } = new List<Post>();

    public override string ToString() => $"Blog '{Name}' ({Url}) — creado {CreatedAt:yyyy-MM-dd}";
}

/// <summary>
/// Representa un post dentro de un blog.
/// </summary>
public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int BlogId { get; set; }
    public Blog Blog { get; set; } = null!;
    public DateTime PublishedAt { get; set; }

    public override string ToString() => $"  Post '{Title}' — publicado {PublishedAt:yyyy-MM-dd}";
}

/// <summary>
/// Entidad configurada con Data Annotations (comparativa).
/// </summary>
[Table("Categories")]
public class Category
{
    [Key]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("category_name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [NotMapped]
    public string DisplayName => $"{Name} ({CategoryId})";
}

/// <summary>
/// DbContext con Fluent API (enfoque recomendado).
/// </summary>
public class BlogContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("BlogDb");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // NOTA: ToTable(), HasDefaultValueSql(), HasColumnName(), HasColumnType()
        // son métodos RELACIONALES (PostgreSQL, SQL Server). InMemory no los soporta.
        // En producción sí funcionan. Aquí comentamos su equivalente.

        modelBuilder.Entity<Blog>(entity =>
        {
            // Con proveedor real: entity.ToTable("Blogs");
            entity.HasKey(b => b.BlogId);
            entity.Property(b => b.Name).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Url).IsRequired().HasMaxLength(500);
            // Con proveedor real: entity.Property(b => b.CreatedAt).HasDefaultValueSql("NOW()");
            // Con proveedor real: entity.Property(b => b.CreatedAt).HasDefaultValueSql("NOW()");
            entity.HasIndex(b => b.Name).IsUnique();
        });

        modelBuilder.Entity<Post>(entity =>
        {
            // Con proveedor real: entity.ToTable("Posts");
            entity.HasKey(p => p.PostId);
            entity.Property(p => p.Title).IsRequired().HasMaxLength(300);
            entity.Property(p => p.Content).IsRequired();
            // Con proveedor real: entity.Property(p => p.Content).HasColumnType("text");
            entity.HasOne(p => p.Blog)
                .WithMany(b => b.Posts)
                .HasForeignKey(p => p.BlogId)
                .OnDelete(DeleteBehavior.Cascade);
            // Con proveedor real: entity.Property(p => p.PublishedAt).HasDefaultValueSql("NOW()");
            // Con proveedor real: entity.Property(p => p.PublishedAt).HasDefaultValueSql("NOW()");
            entity.HasIndex(p => new { p.BlogId, p.PublishedAt });
        });

        modelBuilder.Entity<Category>(entity =>
        {
            // Con proveedor real: entity.ToTable("Categories");
            entity.HasKey(c => c.CategoryId);
            // Con proveedor real: entity.Property(c => c.CategoryId).HasColumnName("category_id");
            // Con proveedor real: entity.Property(c => c.Name).HasColumnName("category_name");
            entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Description).HasMaxLength(500);
            entity.Ignore(c => c.DisplayName);
        });

        // Seed data
        modelBuilder.Entity<Blog>().HasData(
            new Blog { BlogId = 1, Name = "Tech Blog", Url = "https://tech.blog", CreatedAt = new DateTime(2024, 1, 1) },
            new Blog { BlogId = 2, Name = "Cooking Blog", Url = "https://cooking.blog", CreatedAt = new DateTime(2024, 2, 1) }
        );

        modelBuilder.Entity<Post>().HasData(
            new Post { PostId = 1, Title = "Introducción a EF Core", Content = "EF Core es un ORM...", BlogId = 1, PublishedAt = new DateTime(2024, 3, 1) },
            new Post { PostId = 2, Title = "Receta de paella", Content = "Ingredientes: arroz, azafrán...", BlogId = 2, PublishedAt = new DateTime(2024, 3, 15) }
        );
    }
}
