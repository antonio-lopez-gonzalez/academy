using Microsoft.EntityFrameworkCore;

// ============================================================================
// MÓDULO 7: Entity Framework Core
// Lección 04 — Consultas, Performance, AsNoTracking, AsSplitQuery
// ============================================================================
//
// La MAYORÍA de los problemas de rendimiento en EF Core vienen de no entender
// cuándo y cómo se ejecuta realmente el SQL.
//
// CONCEPTOS CLAVE:
//   IQueryable<T> → consulta NO ejecutada (árbol de expresión).
//   ToList()      → materializa la consulta. AQUÍ se ejecuta el SQL.
//
//   Eager Loading (.Include) → JOINs para cargar relaciones en una query.
//   N+1 Problem → foreach post → acceder post.Blog → 1 query extra por post.
//   AsNoTracking() → desactiva change tracker, mejor rendimiento en GETs.
//   AsSplitQuery() → consultas separadas en vez de un JOIN enorme.
//   Select() → proyectar solo columnas necesarias (evitar SELECT *).
//   Paginación → Skip/Take (cuidado con OFFSET grandes).

// ══════════════════════════════════════════════════════════════════════════
// PROGRAMA PRINCIPAL
// ══════════════════════════════════════════════════════════════════════════

Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  MÓDULO 7 — Lección 04: Consultas, Performance, AsNoTracking    ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// ─── PREPARAR DATOS DE PRUEBA (50 posts) ─────────────────────────────────

using (BlogContext seedContext = new())
{
    seedContext.Database.EnsureCreated();

    if (!seedContext.Authors.Any())
    {
        Author author1 = new() { AuthorId = 1, Name = "Ana García" };
        Author author2 = new() { AuthorId = 2, Name = "Carlos López" };
        seedContext.Authors.AddRange(author1, author2);

        Tag[] tags = new[]
        {
            new Tag { TagId = 1, Name = "dotnet" },
            new Tag { TagId = 2, Name = "csharp" },
            new Tag { TagId = 3, Name = "efcore" },
            new Tag { TagId = 4, Name = "aspnetcore" },
            new Tag { TagId = 5, Name = "performance" },
        };
        seedContext.Tags.AddRange(tags);

        DateTime baseDate = new(2024, 1, 1);
        for (int i = 1; i <= 50; i++)
        {
            Post post = new()
            {
                PostId = i,
                Title = $"Post número {i:D2}",
                Content = $"Contenido del post {i}. Lorem ipsum dolor sit amet.",
                Views = Random.Shared.Next(0, 5000),
                IsPublished = i % 5 != 0,
                AuthorId = i <= 25 ? 1 : 2,
                CreatedAt = baseDate.AddDays(i),
            };
            seedContext.Posts.Add(post);
        }

        seedContext.SaveChanges();

        // Relaciones N:N y comentarios
        List<Post> allPosts = seedContext.Posts.ToList();
        List<Tag> allTags = seedContext.Tags.ToList();

        foreach (Post post in allPosts)
        {
            int tagCount = Random.Shared.Next(1, 3);
            for (int t = 0; t < tagCount; t++)
                post.Tags.Add(allTags[Random.Shared.Next(allTags.Count)]);

            int commentCount = Random.Shared.Next(1, 4);
            for (int c = 0; c < commentCount; c++)
            {
                post.Comments.Add(new Comment
                {
                    Text = $"Comentario {c + 1} del post {post.PostId}",
                    IsApproved = c % 2 == 0,
                });
            }
        }

        seedContext.SaveChanges();
    }
}

Console.WriteLine("Datos de prueba preparados: 2 autores, 50 posts, tags y comentarios.");
Console.WriteLine();

// ─── USAR UN NUEVO CONTEXTO PARA LAS DEMOSTRACIONES ──────────────────────

using (BlogContext context = new())
{
    // 1) IQUERYABLE VS IENUMERABLE ────────────────────────────────────────

    Console.WriteLine("━━━ 1. IQueryable vs IEnumerable ━━━━━━━━━━━━━━━━━━━━━━━");

    IQueryable<Post> query = context.Posts
        .Where(p => p.Views > 1000)
        .OrderByDescending(p => p.Views);

    Console.WriteLine($"  SQL generado: {query.ToQueryString()}");
    Console.WriteLine();

    List<Post> popularPosts = query.ToList();
    Console.WriteLine($"  Posts con >1000 views: {popularPosts.Count}");
    Console.WriteLine("  Regla: NUNCA .ToList() ANTES de .Where()");
    Console.WriteLine();

    // 2) EAGER LOADING: Include / ThenInclude ─────────────────────────────

    Console.WriteLine("━━━ 2. Eager Loading: Include / ThenInclude ━━━━━━━━━━━");

    List<Post> postsWithDetails = context.Posts
        .Include(p => p.Author)
        .Include(p => p.Tags)
        .Include(p => p.Comments)
        .Where(p => p.IsPublished)
        .Take(3)
        .ToList();

    Console.WriteLine("  3 posts con autor, tags y comentarios:");
    foreach (Post post in postsWithDetails)
    {
        Console.WriteLine($"  {post.Title}");
        Console.WriteLine($"    Autor: {post.Author.Name}");
        Console.WriteLine($"    Tags: {string.Join(", ", post.Tags.Select(t => t.Name))}");
        Console.WriteLine($"    Comentarios: {post.Comments.Count}");
    }

    Console.WriteLine();

    // 3) FILTERED INCLUDE ─────────────────────────────────────────────────

    Console.WriteLine("━━━ 3. Filtered Include (comentarios aprobados) ━━━━━━━");

    Post? postWithApprovedComments = context.Posts
        .Include(p => p.Comments.Where(c => c.IsApproved))
        .FirstOrDefault(p => p.PostId == 10);

    if (postWithApprovedComments is not null)
    {
        Console.WriteLine($"  Post: {postWithApprovedComments.Title}");
        Console.WriteLine($"  Comentarios aprobados: {postWithApprovedComments.Comments.Count}");
    }

    Console.WriteLine();

    // 4) ASNOTRACKING ─────────────────────────────────────────────────────

    Console.WriteLine("━━━ 4. AsNoTracking (solo lectura, mejor rendimiento) ━━━");

    List<Post> readOnlyPosts = context.Posts
        .AsNoTracking()
        .Where(p => p.Views > 2000)
        .ToList();

    Console.WriteLine($"  Posts con >2000 views (sin tracking): {readOnlyPosts.Count}");
    Console.WriteLine("  AsNoTracking: menos memoria, más rápido. Ideal para GETs.");
    Console.WriteLine("  AsNoTrackingWithIdentityResolution: evita duplicados en memoria.");
    Console.WriteLine();

    // 5) PAGINACIÓN ───────────────────────────────────────────────────────

    Console.WriteLine("━━━ 5. Paginación con Skip/Take ━━━━━━━━━━━━━━━━━━━━━━━━━━");

    int pageSize = 5;
    int page = 0;

    List<Post> pagePosts = context.Posts
        .AsNoTracking()
        .OrderBy(p => p.CreatedAt)
        .Skip(page * pageSize)
        .Take(pageSize)
        .ToList();

    Console.WriteLine($"  Página {page + 1} (tamaño {pageSize}):");
    foreach (Post p in pagePosts)
        Console.WriteLine($"    {p}");

    Console.WriteLine("  Keyset Pagination: WHERE CreatedAt > @lastDate TAKE @size");
    Console.WriteLine();

    // 6) PROYECCIÓN CON Select — solo columnas necesarias ─────────────────

    Console.WriteLine("━━━ 6. Proyección con Select (solo columnas necesarias) ━━");

    var postSummaries = context.Posts
        .AsNoTracking()
        .Where(p => p.IsPublished)
        .Select(p => new
        {
            p.Title,
            AuthorName = p.Author.Name,
            CommentCount = p.Comments.Count,
            TagNames = p.Tags.Select(t => t.Name).ToList(),
        })
        .Take(5)
        .ToList();

    foreach (var summary in postSummaries)
    {
        Console.WriteLine($"  '{summary.Title}' por {summary.AuthorName}");
        Console.WriteLine($"    {summary.CommentCount} comentarios, tags: {string.Join(", ", summary.TagNames)}");
    }

    Console.WriteLine();

    // 7) CONSULTAS AGREGADAS ──────────────────────────────────────────────

    Console.WriteLine("━━━ 7. Consultas agregadas (Count, Sum, Average) ━━━━━━━");

    int totalPosts = context.Posts.Count();
    int publishedPosts = context.Posts.Count(p => p.IsPublished);
    double avgViews = context.Posts.Average(p => p.Views);
    int maxViews = context.Posts.Max(p => p.Views);

    Console.WriteLine($"  Total posts: {totalPosts}");
    Console.WriteLine($"  Publicados: {publishedPosts}");
    Console.WriteLine($"  Media de vistas: {avgViews:F0}");
    Console.WriteLine($"  Máximo de vistas: {maxViews}");

    Console.WriteLine();

    // 8) GROUP BY ─────────────────────────────────────────────────────────

    Console.WriteLine("━━━ 8. GroupBy ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

    var postsByAuthor = context.Posts
        .AsNoTracking()
        .GroupBy(p => p.Author.Name)
        .Select(g => new
        {
            Author = g.Key,
            Count = g.Count(),
            TotalViews = g.Sum(p => p.Views),
            AvgViews = g.Average(p => p.Views),
        })
        .ToList();

    foreach (var group in postsByAuthor)
    {
        Console.WriteLine($"  {group.Author}: {group.Count} posts, " +
                          $"{group.TotalViews} vistas totales, " +
                          $"media {group.AvgViews:F0}");
    }
}

// ─── RESUMEN ─────────────────────────────────────────────────────────────

Console.WriteLine();
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("BUENAS PRÁCTICAS DE PERFORMANCE");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine();
Console.WriteLine("  AsNoTracking() para consultas de solo lectura (GET endpoints).");
Console.WriteLine("  Include() para evitar el problema N+1.");
Console.WriteLine("  Select() para proyectar solo columnas necesarias.");
Console.WriteLine("  AsSplitQuery() con múltiples Includes en colecciones.");
Console.WriteLine("  Paginación siempre en endpoints que devuelven listas.");
Console.WriteLine("  ToQueryString() para debuggear el SQL generado.");
Console.WriteLine();
Console.WriteLine("  NUNCA: .ToList() ANTES de .Where()");
Console.WriteLine("  NUNCA: acceder nav props en bucles sin Include (N+1)");

Console.WriteLine();
Console.WriteLine("✅ Fin de la lección 04.");


// ══════════════════════════════════════════════════════════════════════════
// TIPOS DEFINIDOS AL FINAL DEL ARCHIVO
// ══════════════════════════════════════════════════════════════════════════

public class Author
{
    public int AuthorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Views { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public override string ToString() => $"[{PostId}] {Title} ({Views} views)";
}

public class Comment
{
    public int CommentId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; } = null!;
}

public class Tag
{
    public int TagId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}

public class BlogContext : DbContext
{
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("BlogDb");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(a => a.AuthorId);
            entity.HasMany(a => a.Posts)
                .WithOne(p => p.Author)
                .HasForeignKey(p => p.AuthorId);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.PostId);
            entity.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId);
            entity.HasMany(p => p.Tags)
                .WithMany(t => t.Posts)
                .UsingEntity("PostTags"); // Con proveedor real: j => j.ToTable("PostTags")
        });

        modelBuilder.Entity<Comment>(entity => entity.HasKey(c => c.CommentId));
        modelBuilder.Entity<Tag>(entity => entity.HasKey(t => t.TagId));
    }
}
