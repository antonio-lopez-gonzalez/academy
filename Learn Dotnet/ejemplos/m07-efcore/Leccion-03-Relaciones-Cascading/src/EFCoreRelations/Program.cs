using Microsoft.EntityFrameworkCore;

// ============================================================================
// MÓDULO 7: Entity Framework Core
// Lección 03 — Relaciones (1:1, 1:N, N:N) y Cascading Deletes
// ============================================================================
//
// Las bases de datos relacionales se basan en... ¡relaciones!
// EF Core mapea estas relaciones a propiedades de navegación en C#.
//
// TIPOS DE RELACIONES:
//   1:N (Uno a Muchos) — Blog → Posts (la más común).
//   1:1 (Uno a Uno)     — User → UserProfile.
//   N:N (Muchos a Muchos) — Post ↔ Tag (desde EF Core 5, sin entidad intermedia).
//
// CASCADING DELETES: ¿Qué pasa con los Posts si borras un Blog?
//   - Cascade:       borra los posts automáticamente.
//   - Restrict:      NO permite borrar si tiene posts (error).
//   - SetNull:       pone FK = NULL en los posts.
//   - ClientCascade: cascade solo en aplicación (no en BD).

// ══════════════════════════════════════════════════════════════════════════
// PROGRAMA PRINCIPAL
// ══════════════════════════════════════════════════════════════════════════

Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  MÓDULO 7 — Lección 03: Relaciones (1:1, 1:N, N:N) y Cascading Deletes ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

using (CmsContext context = new())
{
    context.Database.EnsureCreated();

    // SEED DE RELACIÓN N:N (no se puede con HasData — la tabla intermedia es automática)
    if (!context.Posts.Any(p => p.Tags.Any()))
    {
        Post? p1 = context.Posts.Find(1);
        Post? p2 = context.Posts.Find(2);
        Post? p3 = context.Posts.Find(3);
        Tag? dotnet = context.Tags.Find(1);
        Tag? csharp = context.Tags.Find(2);
        Tag? efcore = context.Tags.Find(3);
        Tag? recetas = context.Tags.Find(4);

        if (p1 is not null && dotnet is not null) p1.Tags.Add(dotnet);
        if (p1 is not null && csharp is not null) p1.Tags.Add(csharp);
        if (p1 is not null && efcore is not null) p1.Tags.Add(efcore);
        if (p2 is not null && csharp is not null) p2.Tags.Add(csharp);
        if (p2 is not null && dotnet is not null) p2.Tags.Add(dotnet);
        if (p3 is not null && recetas is not null) p3.Tags.Add(recetas);
        context.SaveChanges();
    }

    // ─── RELACIÓN 1:N — Blog → Posts ────────────────────────────────────

    Console.WriteLine("━━━ RELACIÓN 1:N: Blog → Posts ━━━━━━━━━━━━━━━━━━━━━━━━━━━");

    Blog? techBlog = context.Blogs
        .Include(b => b.Posts)
        .FirstOrDefault(b => b.Name == "Tech Blog");

    if (techBlog is not null)
    {
        Console.WriteLine($"Blog: {techBlog.Name}");
        Console.WriteLine($"  Tiene {techBlog.Posts.Count} posts:");
        foreach (Post post in techBlog.Posts)
            Console.WriteLine($"    - {post.Title}");
    }

    Console.WriteLine();

    // ─── RELACIÓN 1:1 — User → UserProfile ──────────────────────────────

    Console.WriteLine("━━━ RELACIÓN 1:1: User → UserProfile ━━━━━━━━━━━━━━━━━━━━━");

    User? antonio = context.Users
        .Include(u => u.Profile)
        .FirstOrDefault(u => u.Username == "antonio");

    if (antonio is not null)
    {
        Console.WriteLine(antonio);
        if (antonio.Profile is not null)
            Console.WriteLine($"  Perfil: {antonio.Profile.FullName}");
    }

    Console.WriteLine();

    // ─── RELACIÓN N:N — Post ↔ Tag ──────────────────────────────────────

    Console.WriteLine("━━━ RELACIÓN N:N: Post ↔ Tag ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

    List<Post> allPosts = context.Posts
        .Include(p => p.Tags)
        .Include(p => p.Blog)
        .ToList();

    foreach (Post post in allPosts)
    {
        Console.WriteLine($"Post: '{post.Title}' (Blog: {post.Blog.Name})");
        Console.Write("  Tags: ");
        foreach (Tag tag in post.Tags)
            Console.Write($"#{tag.Name} ");
        Console.WriteLine();
    }

    Console.WriteLine();

    Console.WriteLine("Posts con el tag #csharp:");
    Tag? csharpTag = context.Tags
        .Include(t => t.Posts)
        .FirstOrDefault(t => t.Name == "csharp");

    if (csharpTag is not null)
    {
        foreach (Post post in csharpTag.Posts)
            Console.WriteLine($"  - {post.Title}");
    }

    Console.WriteLine();

    // ─── CASCADING DELETES ────────────────────────────────────────────────

    Console.WriteLine("━━━ CASCADING DELETES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    Console.WriteLine("1. Borrando 'Cooking Blog' (tiene posts → Cascade los borra):");

    Blog? cookingBlog = context.Blogs
        .Include(b => b.Posts)
        .FirstOrDefault(b => b.Name == "Cooking Blog");

    if (cookingBlog is not null)
    {
        Console.WriteLine($"   Blog a borrar: {cookingBlog.Name}");
        Console.WriteLine($"   Posts antes: {context.Posts.Count()}");
        context.Blogs.Remove(cookingBlog);
        context.SaveChanges();
        Console.WriteLine($"   Posts después: {context.Posts.Count()} (se borró el post hijo)");
    }

    Console.WriteLine();
    Console.WriteLine("2. Intentando borrar al User 'antonio' (tiene Blogs → Restrict lo impide):");

    User? userToDelete = context.Users
        .Include(u => u.Blogs)
        .FirstOrDefault(u => u.Username == "antonio");

    if (userToDelete is not null)
    {
        Console.WriteLine($"   User: {userToDelete.Username}");
        Console.WriteLine($"   Tiene {userToDelete.Blogs.Count} blog(s)");

        try
        {
            context.Users.Remove(userToDelete);
            context.SaveChanges();
            Console.WriteLine("   Borrado (no debería pasar)");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"   ERROR: {ex.Message}");
        }
    }
}

// ─── RESUMEN ─────────────────────────────────────────────────────────────

Console.WriteLine();
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("RESUMEN DE RELACIONES Y DELETE BEHAVIORS");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine();
Console.WriteLine("  RELACIONES:");
Console.WriteLine("    1:N  → HasOne().WithMany()  → Blog (1) → Post (N)");
Console.WriteLine("    1:1  → HasOne().WithOne()   → User (1) → Profile (1)");
Console.WriteLine("    N:N  → HasMany().WithMany() → Post (N) ↔ Tag (N)");
Console.WriteLine();
Console.WriteLine("  DELETE BEHAVIORS:");
Console.WriteLine("    Cascade       → borra hijos automáticamente");
Console.WriteLine("    Restrict      → lanza error si hay hijos (recomendado)");
Console.WriteLine("    SetNull       → pone FK = NULL");
Console.WriteLine("    ClientCascade → cascade solo en aplicación EF");

Console.WriteLine();
Console.WriteLine("✅ Fin de la lección 03.");


// ══════════════════════════════════════════════════════════════════════════
// TIPOS DEFINIDOS AL FINAL DEL ARCHIVO
// ══════════════════════════════════════════════════════════════════════════

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserProfile? Profile { get; set; }
    public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    public override string ToString() => $"User: {Username} ({Email})";
}

public class UserProfile
{
    public int UserProfileId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public override string ToString() => $"Profile: {FullName} — {Bio}";
}

public class Blog
{
    public int BlogId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public override string ToString() => $"Blog: {Name}";
}

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int BlogId { get; set; }
    public Blog Blog { get; set; } = null!;
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public override string ToString() => $"Post: {Title}";
}

public class Tag
{
    public int TagId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public override string ToString() => $"Tag: #{Name}";
}

public class CmsContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<Blog> Blogs { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("CmsDb");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1:1 — User ↔ UserProfile
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);
            entity.HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(u => u.Email).IsUnique();
        });
        modelBuilder.Entity<UserProfile>(entity => entity.HasKey(p => p.UserProfileId));

        // 1:N — User → Blogs
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(b => b.BlogId);
            entity.HasOne(b => b.Owner)
                .WithMany(u => u.Blogs)
                .HasForeignKey(b => b.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 1:N — Blog → Posts
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.PostId);
            entity.HasOne(p => p.Blog)
                .WithMany(b => b.Posts)
                .HasForeignKey(p => p.BlogId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // N:N — Post ↔ Tag (EF Core 5+ sin entidad intermedia)
        modelBuilder.Entity<Post>()
            .HasMany(p => p.Tags)
            .WithMany(t => t.Posts)
            .UsingEntity("PostTags"); // Con proveedor real: j => j.ToTable("PostTags")

        // Seed data
        modelBuilder.Entity<Tag>().HasData(
            new Tag { TagId = 1, Name = "dotnet" },
            new Tag { TagId = 2, Name = "csharp" },
            new Tag { TagId = 3, Name = "efcore" },
            new Tag { TagId = 4, Name = "recetas" }
        );
        modelBuilder.Entity<User>().HasData(
            new User { UserId = 1, Username = "antonio", Email = "antonio@email.com" }
        );
        modelBuilder.Entity<UserProfile>().HasData(
            new UserProfile { UserProfileId = 1, FullName = "Antonio García", Bio = "Dev .NET", UserId = 1 }
        );
        modelBuilder.Entity<Blog>().HasData(
            new Blog { BlogId = 1, Name = "Tech Blog", OwnerId = 1 },
            new Blog { BlogId = 2, Name = "Cooking Blog", OwnerId = 1 }
        );
        modelBuilder.Entity<Post>().HasData(
            new Post { PostId = 1, Title = "Guía EF Core", Content = "...", BlogId = 1 },
            new Post { PostId = 2, Title = "C# 12 novedades", Content = "...", BlogId = 1 },
            new Post { PostId = 3, Title = "Receta paella", Content = "...", BlogId = 2 }
        );
    }
}
