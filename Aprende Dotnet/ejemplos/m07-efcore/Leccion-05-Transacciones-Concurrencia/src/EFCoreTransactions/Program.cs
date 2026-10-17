using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

// ============================================================================
// MÓDULO 7: Entity Framework Core
// Lección 05 — Transacciones, SaveChanges, Concurrencia Optimista
// ============================================================================
//
// TRANSACCIONES:
//   SaveChanges() ya es transaccional. Todas las operaciones pendientes
//   (INSERT, UPDATE, DELETE) se ejecutan dentro de UNA transacción implícita.
//   Si algo falla, se hace ROLLBACK de todo.
//
//   Transacciones explícitas:
//     await using var tx = await context.Database.BeginTransactionAsync();
//     // ... operaciones ...
//     await tx.CommitAsync();  // o RollbackAsync()
//
// CONCURRENCIA OPTIMISTA:
//   Problema: Dos usuarios editan el mismo registro simultáneamente.
//   Sin control, el último que guarda SOBRESCRIBE los cambios del primero.
//
//   Solución: [Timestamp] / IsRowVersion() → columna que cambia cada vez.
//   EF Core incluye esta columna en el WHERE del UPDATE/DELETE.
//   Si otro usuario modificó la fila → DbUpdateConcurrencyException.

// ══════════════════════════════════════════════════════════════════════════
// PROGRAMA PRINCIPAL
// ══════════════════════════════════════════════════════════════════════════

Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  MÓDULO 7 — Lección 05: Transacciones, SaveChanges, Concurrencia    ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// ══════════════════════════════════════════════════════════════════════════
// PARTE 1: TRANSACCIONES
// ══════════════════════════════════════════════════════════════════════════

Console.WriteLine("━━━ PARTE 1: Transacciones ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine();
Console.WriteLine("1.1) SaveChanges — transacción implícita:");
Console.WriteLine("     Cada SaveChanges() agrupa cambios en UNA transacción.");
Console.WriteLine();

// ─── 1.2) Transferencia entre billeteras ────────────────────────────────

Console.WriteLine("1.2) Transferencia entre billeteras:");

using (CmsContext context = new())
{
    context.Database.EnsureCreated();

    Wallet? alice = context.Wallets.Find(1);
    Wallet? bob = context.Wallets.Find(2);

    if (alice is not null && bob is not null)
    {
        Console.WriteLine($"  Saldo inicial: Alice = ${alice.Balance}, Bob = ${bob.Balance}");

        decimal transferAmount = 200m;
        Console.WriteLine($"  Transfiriendo ${transferAmount} de Alice a Bob...");

        if (alice.Balance >= transferAmount)
        {
            alice.Balance -= transferAmount;
            bob.Balance += transferAmount;

            // Ambos cambios se guardan en UNA transacción.
            // Si la BD falla entre el UPDATE de Alice y el de Bob,
            // se hace ROLLBACK automático — no hay dinero "perdido".
            context.SaveChanges();

            Console.WriteLine($"  Transferencia exitosa.");
            Console.WriteLine($"  Saldo final: Alice = ${alice.Balance}, Bob = ${bob.Balance}");
        }
        else
        {
            Console.WriteLine("  Fondos insuficientes.");
        }
    }
}

Console.WriteLine();

// ─── 1.3) Transferencia fallida ─────────────────────────────────────────

Console.WriteLine("1.3) Intento de transferencia fallida:");

using (CmsContext context = new())
{
    context.Database.EnsureCreated();

    Wallet? alice = context.Wallets.Find(1);
    Wallet? bob = context.Wallets.Find(2);

    if (alice is not null && bob is not null)
    {
        Console.WriteLine($"  Saldo: Alice = ${alice.Balance}, Bob = ${bob.Balance}");

        decimal amount = 5000m;
        Console.WriteLine($"  Intentando transferir ${amount}...");

        if (alice.Balance >= amount)
        {
            alice.Balance -= amount;
            bob.Balance += amount;
            context.SaveChanges();
        }
        else
        {
            Console.WriteLine($"  Fondos insuficientes (Alice tiene ${alice.Balance}).");
            Console.WriteLine("  En BD real: RollbackAsync() deshace cambios parciales.");
        }
    }
}

Console.WriteLine();
Console.WriteLine("━━━ PARTE 2: Concurrencia optimista ━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine();

// ══════════════════════════════════════════════════════════════════════════
// PARTE 2: CONCURRENCIA OPTIMISTA
// ══════════════════════════════════════════════════════════════════════════

// Preparar un post fresco
using (CmsContext setupCtx = new())
{
    setupCtx.Database.EnsureCreated();
    Post? existingPost = setupCtx.Posts.Find(1);
    if (existingPost is not null)
    {
        existingPost.Title = "Post Original";
        existingPost.Content = "Contenido original del post.";
        setupCtx.SaveChanges();
    }
}

Console.WriteLine("Escenario: Dos usuarios editan el mismo post simultáneamente.");
Console.WriteLine("  Usuario A cambia Title.");
Console.WriteLine("  Usuario B cambia Content y guarda PRIMERO.");
Console.WriteLine("  Usuario A guarda DESPUÉS → ¿conflicto?");
Console.WriteLine();

// Usuario A lee el post.
Console.WriteLine("--- Usuario A ---");
using (CmsContext userAContext = new())
{
    userAContext.Database.EnsureCreated();

    Post? postA = userAContext.Posts.Find(1);
    if (postA is not null)
    {
        Console.WriteLine($"  Lee: Title='{postA.Title}', Content='{postA.Content}'");
        Console.WriteLine("  Modifica Title a 'Título por Usuario A'...");
        postA.Title = "Título por Usuario A";
        postA.LastModified = DateTime.UtcNow;
    }

    // Usuario B con OTRO contexto (simula otro usuario)
    Console.WriteLine();
    Console.WriteLine("--- Usuario B ---");
    using (CmsContext userBContext = new())
    {
        userBContext.Database.EnsureCreated();

        Post? postB = userBContext.Posts.Find(1);
        if (postB is not null)
        {
            Console.WriteLine($"  Lee: Title='{postB.Title}', Content='{postB.Content}'");
            Console.WriteLine("  Modifica Content a 'Contenido por Usuario B'...");
            postB.Content = "Contenido por Usuario B";
            postB.LastModified = DateTime.UtcNow;

            // Usuario B guarda PRIMERO.
            Console.WriteLine("  Guardando cambios...");
            int savedB = userBContext.SaveChanges();
            Console.WriteLine($"  Usuario B guardó exitosamente ({savedB} registro(s)).");
        }
    }

    // Usuario A guarda DESPUÉS.
    Console.WriteLine();
    Console.WriteLine("--- Usuario A intenta guardar ---");

    try
    {
        Console.WriteLine("  Guardando cambios de Usuario A...");

        // EF Core genera: UPDATE Posts SET Title = @title WHERE PostId = 1 AND RowVersion = @originalRowVersion
        // Si RowVersion cambió (porque Usuario B guardó) → 0 filas afectadas → DbUpdateConcurrencyException.
        int savedA = userAContext.SaveChanges();
        Console.WriteLine($"  Usuario A guardó exitosamente ({savedA} registro(s)).");
        Console.WriteLine("  NOTA: En InMemory, [Timestamp] puede no lanzar excepción.");
        Console.WriteLine("  Con PostgreSQL/SQL Server SÍ se lanza DbUpdateConcurrencyException.");
    }
    catch (DbUpdateConcurrencyException ex)
    {
        Console.WriteLine($"  DbUpdateConcurrencyException: conflicto de concurrencia.");
        Console.WriteLine($"  Mensaje: {ex.Message}");
        Console.WriteLine();
        Console.WriteLine("  ESTRATEGIAS PARA MANEJAR EL CONFLICTO:");
        Console.WriteLine("  1) 'Last wins': refrescar la entidad y reintentar.");
        Console.WriteLine("  2) 'First wins': informar al usuario que sus cambios no se guardaron.");
        Console.WriteLine("  3) 'Merge': mostrar ambos valores y que el usuario decida.");
    }
}

// Mostrar el estado final del post
Console.WriteLine();
Console.WriteLine("--- Estado final del post ---");
using (CmsContext finalContext = new())
{
    finalContext.Database.EnsureCreated();
    Post? finalPost = finalContext.Posts.Find(1);
    if (finalPost is not null)
    {
        Console.WriteLine($"  Title: '{finalPost.Title}'");
        Console.WriteLine($"  Content: '{finalPost.Content}'");
    }
}

// ─── RESUMEN ─────────────────────────────────────────────────────────────

Console.WriteLine();
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("RESUMEN: Transacciones y Concurrencia");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine();
Console.WriteLine("  TRANSACCIONES:");
Console.WriteLine("    SaveChanges()      → transacción implícita (INSERT+UPDATE+DELETE juntos).");
Console.WriteLine("    BeginTransactionAsync() → transacción explícita manual.");
Console.WriteLine();
Console.WriteLine("  CONCURRENCIA OPTIMISTA:");
Console.WriteLine("    [Timestamp] / IsRowVersion() → columna byte[] que cambia cada vez.");
Console.WriteLine("    DbUpdateConcurrencyException → excepción si hay conflicto.");
Console.WriteLine();
Console.WriteLine("  ESTRATEGIAS DE RESOLUCIÓN:");
Console.WriteLine("    Last wins:  ReloadAsync() + aplicar cambios + SaveChanges().");
Console.WriteLine("    First wins: informar al usuario → 'alguien ya modificó esto'.");
Console.WriteLine("    Merge:      mostrar ambos valores, que el usuario decida.");

Console.WriteLine();
Console.WriteLine("✅ Fin de la lección 05.");


// ══════════════════════════════════════════════════════════════════════════
// TIPOS DEFINIDOS AL FINAL DEL ARCHIVO
// ══════════════════════════════════════════════════════════════════════════

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}

public class Wallet
{
    public int WalletId { get; set; }
    public string Owner { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}

public class CmsContext : DbContext
{
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Wallet> Wallets { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("CmsDb");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.PostId);
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(w => w.WalletId);
        });

        modelBuilder.Entity<Post>().HasData(
            new Post
            {
                PostId = 1,
                Title = "Post Original",
                Content = "Contenido original del post.",
                LastModified = new DateTime(2024, 1, 1),
            }
        );

        modelBuilder.Entity<Wallet>().HasData(
            new Wallet { WalletId = 1, Owner = "Alice", Balance = 1000m },
            new Wallet { WalletId = 2, Owner = "Bob", Balance = 500m }
        );
    }
}
