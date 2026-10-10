// ──────────────────────────────────────────────────────────────────────────
// Módulo 3 — SOLID en C#
// Lección 04 — Principio de Segregación de Interfaces (ISP)
//
// ISP: "Ningún cliente debería verse obligado a depender de métodos que no usa."
//
//     Antes (violación)               Después (cumple ISP)
//     ┌──────────────────┐            ┌─────────┐ ┌─────────┐ ┌─────────┐
//     │ IMultiFunction    │            │ IPrinter│ │IScanner │ │  IFax   │
//     │ Printer           │            │ Print() │ │ Scan()  │ │ Fax()   │
//     │ Print(), Scan(),  │            └────▲────┘ └────▲────┘ └────▲────┘
//     │ Fax(), Copy(),    │                ╱           ╱           ╱
//     │ Staple()          │           ┌────────┐  ┌────────┐  ┌───────────┐
//     └────────▲──────────┘           │Basic   │  │Scanner │  │  Premium   │
//              │                      │Printer │  │ Only   │  │  Printer   │
//     ┌────────┴───────────┐          │IPrinter│  │IScanner│  │IPrinter,   │
//     │ BasicPrinter       │          └────────┘  └────────┘  │IScanner,   │
//     │ Print() ✓          │                                   │IFax, ICopy │
//     │ Scan()  → throw    │                                   └────────────┘
//     │ Fax()   → throw    │
//     └────────────────────┘
// ──────────────────────────────────────────────────────────────────────────

namespace SolidISP;

// ──────────────────────────────────────────────────────────────────────────
// CLASES AUXILIARES
// ──────────────────────────────────────────────────────────────────────────

/// <summary>Documento sencillo que manejan las impresoras.</summary>
public class Document
{
    public string Title { get; set; }
    public string Content { get; set; }

    public Document(string title, string content)
    {
        Title = title;
        Content = content;
    }

    public override string ToString() => $"Documento: '{Title}'";
}

/// <summary>Entidad Post usada para los ejemplos de repositorios CMS.</summary>
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public override string ToString() => $"[{Id}] {Title}";
}

/// <summary>Entidad Category usada para los ejemplos de repositorios CMS.</summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public override string ToString() => $"[{Id}] {Name}";
}

// ══════════════════════════════════════════════════════════════════════════
// PARTE 1 — VIOLACIÓN ISP: INTERFAZ GORDA (printer)
// ══════════════════════════════════════════════════════════════════════════

/// <summary>
/// VIOLACIÓN: Una interfaz gorda obliga a todos los implementadores
/// a depender de métodos que NO necesitan.
/// En el mundo real, una BasicPrinter no escanea, no faxea, no copia ni grapa.
/// Sin embargo, está OBLIGADA a implementar esos métodos.
/// </summary>
public interface IMultiFunctionPrinter
{
    void Print(Document doc);
    byte[] Scan();
    void Fax(string number, Document doc);
    void Copy();
    void Staple();
}

/// <summary>
/// Impresora básica: solo imprime. No tiene hardware de escáner, fax, etc.
/// Pero la interfaz IMultiFunctionPrinter la OBLIGA a implementar TODO.
/// Los métodos que no usa lanzan NotImplementedException → ISP roto.
/// </summary>
public class BasicPrinterViolation : IMultiFunctionPrinter
{
    public string Model { get; } = "BasicPrinter-2000";

    public void Print(Document doc)
    {
        Console.WriteLine($"  [{Model}] Imprimiendo: {doc}");
    }

    // ⚠️ Esta impresora NO escanea. Pero la interfaz obliga a implementarlo.
    public byte[] Scan()
    {
        throw new NotImplementedException(
            $"{Model} no tiene escáner. " +
            "Esto es una violación de ISP: el cliente depende de un método que no necesita.");
    }

    // ⚠️ Esta impresora NO faxea.
    public void Fax(string number, Document doc)
    {
        throw new NotImplementedException(
            $"{Model} no tiene módem fax. Violación de ISP.");
    }

    // ⚠️ Esta impresora NO fotocopia.
    public void Copy()
    {
        throw new NotImplementedException(
            $"{Model} no puede fotocopiar. Violación de ISP.");
    }

    // ⚠️ Esta impresora NO grapa.
    public void Staple()
    {
        throw new NotImplementedException(
            $"{Model} no tiene grapadora. Violación de ISP.");
    }
}

/// <summary>
/// Impresora premium: SÍ tiene todas las funcionalidades.
/// Para esta clase la interfaz gorda es correcta, pero obliga a BasicPrinter
/// a implementar métodos que no necesita. El problema es la interfaz, no esta clase.
/// </summary>
public class PremiumPrinterViolation : IMultiFunctionPrinter
{
    public string Model { get; } = "PremiumPrinter-X9000";

    public void Print(Document doc)
    {
        Console.WriteLine($"  [{Model}] Imprimiendo en alta calidad: {doc}");
    }

    public byte[] Scan()
    {
        Console.WriteLine($"  [{Model}] Escaneando documento...");
        return new byte[] { 0x01, 0x02, 0x03 };
    }

    public void Fax(string number, Document doc)
    {
        Console.WriteLine($"  [{Model}] Enviando fax de '{doc.Title}' al número {number}");
    }

    public void Copy()
    {
        Console.WriteLine($"  [{Model}] Fotocopiando...");
    }

    public void Staple()
    {
        Console.WriteLine($"  [{Model}] Grapando documentos...");
    }
}

// ══════════════════════════════════════════════════════════════════════════
// PARTE 2 — VIOLACIÓN ISP: REPOSITORIO GORDO (CMS)
// ══════════════════════════════════════════════════════════════════════════

/// <summary>
/// VIOLACIÓN: Un repositorio que mezcla operaciones de lectura y escritura.
/// Un repositorio de solo lectura (ej. para una pantalla pública de posts)
/// NO debería tener métodos de escritura, pero está OBLIGADO a implementarlos.
/// </summary>
public interface IRepositoryViolation<T> where T : class
{
    // --- Lectura ---
    T? GetById(int id);
    IEnumerable<T> GetAll();
    IEnumerable<T> Search(Func<T, bool> predicate);
    (IEnumerable<T> Items, int TotalCount) GetPaged(int page, int pageSize);
    int Count();
    bool Exists(int id);

    // --- Escritura ---
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
    void BulkInsert(IEnumerable<T> entities);
}

/// <summary>
/// Repositorio de solo lectura para posts públicos.
/// Necesita Exclusivamente métodos de lectura (GetAll, GetById, Search, etc.).
/// Pero la interfaz IRepositoryViolation lo obliga a implementar Add, Update,
/// Delete y BulkInsert → métodos que NUNCA usará.
/// </summary>
public class ReadOnlyRepositoryViolation : IRepositoryViolation<Post>
{
    // Simulamos datos en memoria
    private readonly List<Post> _posts = new()
    {
        new Post { Id = 1, Title = "Introducción a .NET 8",  Body = "...", CreatedAt = DateTime.UtcNow },
        new Post { Id = 2, Title = "Guía de C# 12",          Body = "...", CreatedAt = DateTime.UtcNow },
        new Post { Id = 3, Title = "ASP.NET Core Minimal API", Body = "...", CreatedAt = DateTime.UtcNow },
    };

    // ---- MÉTODOS DE LECTURA (los que realmente necesita) ----

    public Post? GetById(int id)
    {
        return _posts.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<Post> GetAll()
    {
        return _posts.ToList();
    }

    public IEnumerable<Post> Search(Func<Post, bool> predicate)
    {
        return _posts.Where(predicate).ToList();
    }

    public (IEnumerable<Post> Items, int TotalCount) GetPaged(int page, int pageSize)
    {
        int total = _posts.Count;
        IEnumerable<Post> items = _posts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return (items, total);
    }

    public int Count()
    {
        return _posts.Count;
    }

    public bool Exists(int id)
    {
        return _posts.Any(p => p.Id == id);
    }

    // ---- MÉTODOS DE ESCRITURA (NO los necesita) ----

    // ⚠️ Este repositorio es de SOLO LECTURA. No debería poder añadir posts.
    // Pero la interfaz IRepositoryViolation lo OBLIGA.
    public void Add(Post entity)
    {
        throw new NotImplementedException(
            "ReadOnlyRepository no soporta escritura. Violación de ISP.");
    }

    public void Update(Post entity)
    {
        throw new NotImplementedException(
            "ReadOnlyRepository no soporta escritura. Violación de ISP.");
    }

    public void Delete(int id)
    {
        throw new NotImplementedException(
            "ReadOnlyRepository no soporta escritura. Violación de ISP.");
    }

    public void BulkInsert(IEnumerable<Post> entities)
    {
        throw new NotImplementedException(
            "ReadOnlyRepository no soporta escritura. Violación de ISP.");
    }
}

// ══════════════════════════════════════════════════════════════════════════
// PARTE 3 — REFACTOR ISP: INTERFACES PEQUEÑAS Y COHESIVAS (printer)
// ══════════════════════════════════════════════════════════════════════════

// Cada interfaz tiene UNA sola responsabilidad (cohesión máxima).
// Ningún cliente se ve obligado a implementar lo que no necesita.

public interface IPrinter
{
    void Print(Document doc);
}

public interface IScanner
{
    byte[] Scan();
}

public interface IFax
{
    void Fax(string number, Document doc);
}

public interface ICopier
{
    void Copy();
}

/// <summary>
/// CORRECTO (ISP): BasicPrinter solo implementa IPrinter.
/// No tiene NINGÚN método que no necesite. Cero NotImplementedException.
/// </summary>
public class BasicPrinter : IPrinter
{
    public string Model { get; } = "BasicPrinter-2000";

    public void Print(Document doc)
    {
        Console.WriteLine($"  [{Model}] Imprimiendo: {doc}");
    }
}

/// <summary>
/// CORRECTO (ISP): Un escáner independiente solo implementa IScanner.
/// Ideal para entornos donde impresora y escáner son dispositivos separados.
/// </summary>
public class ScannerOnly : IScanner
{
    public string Model { get; } = "ScanMaster-500";

    public byte[] Scan()
    {
        Console.WriteLine($"  [{Model}] Escaneando documento en alta resolución...");
        return new byte[] { 0x0A, 0x0B, 0x0C };
    }
}

/// <summary>
/// CORRECTO (ISP): PremiumPrinter compone MÚLTIPLES interfaces pequeñas.
/// Como SÍ tiene todas las funcionalidades, implementa todas las interfaces
/// que necesita. Cada interfaz es opcional y cohesiva.
///
/// Esto es COMPOSICIÓN DE INTERFACES: la clase elige qué contratos cumplir.
/// </summary>
public class PremiumPrinter : IPrinter, IScanner, IFax, ICopier
{
    public string Model { get; } = "PremiumPrinter-X9000";

    public void Print(Document doc)
    {
        Console.WriteLine($"  [{Model}] Imprimiendo en alta calidad: {doc}");
    }

    public byte[] Scan()
    {
        Console.WriteLine($"  [{Model}] Escaneando documento...");
        return new byte[] { 0x01, 0x02, 0x03 };
    }

    public void Fax(string number, Document doc)
    {
        Console.WriteLine($"  [{Model}] Enviando fax de '{doc.Title}' al número {number}");
    }

    public void Copy()
    {
        Console.WriteLine($"  [{Model}] Fotocopiando...");
    }
}

// ══════════════════════════════════════════════════════════════════════════
// PARTE 4 — REFACTOR ISP: REPOSITORIOS SEGREGADOS (CMS)
// ══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Interfaz de solo lectura. Quien solo necesite consultar datos
/// implementa ESTA interfaz y no depende de métodos de escritura.
/// </summary>
public interface IReadOnlyRepository<T> where T : class
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    IEnumerable<T> Search(Func<T, bool> predicate);
    (IEnumerable<T> Items, int TotalCount) GetPaged(int page, int pageSize);
    int Count();
    bool Exists(int id);
}

/// <summary>
/// Interfaz de solo escritura. Útil para servicios que solo persisten datos
/// (ej. un procesador de eventos o un logger de auditoría).
/// </summary>
public interface IWriteOnlyRepository<T> where T : class
{
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
    void BulkInsert(IEnumerable<T> entities);
}

/// <summary>
/// Interfaz completa que COMPONE lectura + escritura.
/// Solo la usan los repositorios que realmente necesitan AMBAS operaciones.
/// Hereda de las dos interfaces segregadas.
/// </summary>
public interface IRepository<T> : IReadOnlyRepository<T>, IWriteOnlyRepository<T>
    where T : class
{
    // No añade nada nuevo: hereda todo de IReadOnlyRepository y IWriteOnlyRepository.
    // Quien implemente IRepository<T> implementa ambos.
}

/// <summary>
/// CORRECTO (ISP): Repositorio de solo lectura para posts.
/// Implementa ÚNICAMENTE IReadOnlyRepository<Post>.
/// Cero dependencias en métodos de escritura.
/// Cero NotImplementedException.
/// </summary>
public class ReadOnlyPostRepository : IReadOnlyRepository<Post>
{
    private readonly List<Post> _posts = new()
    {
        new Post { Id = 1, Title = "Introducción a .NET 8",  Body = "...", CreatedAt = DateTime.UtcNow },
        new Post { Id = 2, Title = "Guía de C# 12",          Body = "...", CreatedAt = DateTime.UtcNow },
        new Post { Id = 3, Title = "ASP.NET Core Minimal API", Body = "...", CreatedAt = DateTime.UtcNow },
    };

    public Post? GetById(int id)
    {
        return _posts.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<Post> GetAll()
    {
        return _posts.ToList();
    }

    public IEnumerable<Post> Search(Func<Post, bool> predicate)
    {
        return _posts.Where(predicate).ToList();
    }

    public (IEnumerable<Post> Items, int TotalCount) GetPaged(int page, int pageSize)
    {
        int total = _posts.Count;
        IEnumerable<Post> items = _posts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return (items, total);
    }

    public int Count()
    {
        return _posts.Count;
    }

    public bool Exists(int id)
    {
        return _posts.Any(p => p.Id == id);
    }
}

/// <summary>
/// CORRECTO (ISP): Repositorio completo de posts.
/// Necesita tanto lectura como escritura, así que implementa IRepository<Post>,
/// que a su vez hereda de IReadOnlyRepository<Post> e IWriteOnlyRepository<Post>.
/// </summary>
public class PostRepository : IRepository<Post>
{
    private readonly List<Post> _posts = new()
    {
        new Post { Id = 1, Title = "Intro .NET", Body = "...", CreatedAt = DateTime.UtcNow },
    };
    private int _nextId = 2;

    // ── IReadOnlyRepository<Post> ──

    public Post? GetById(int id)
    {
        return _posts.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<Post> GetAll()
    {
        return _posts.ToList();
    }

    public IEnumerable<Post> Search(Func<Post, bool> predicate)
    {
        return _posts.Where(predicate).ToList();
    }

    public (IEnumerable<Post> Items, int TotalCount) GetPaged(int page, int pageSize)
    {
        int total = _posts.Count;
        IEnumerable<Post> items = _posts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return (items, total);
    }

    public int Count()
    {
        return _posts.Count;
    }

    public bool Exists(int id)
    {
        return _posts.Any(p => p.Id == id);
    }

    // ── IWriteOnlyRepository<Post> ──

    public void Add(Post entity)
    {
        entity.Id = _nextId++;
        _posts.Add(entity);
        Console.WriteLine($"  [PostRepository] Añadido: {entity}");
    }

    public void Update(Post entity)
    {
        Post? existing = _posts.FirstOrDefault(p => p.Id == entity.Id);
        if (existing is not null)
        {
            existing.Title = entity.Title;
            existing.Body = entity.Body;
            Console.WriteLine($"  [PostRepository] Actualizado: {entity}");
        }
    }

    public void Delete(int id)
    {
        Post? existing = _posts.FirstOrDefault(p => p.Id == id);
        if (existing is not null)
        {
            _posts.Remove(existing);
            Console.WriteLine($"  [PostRepository] Eliminado: {existing}");
        }
    }

    public void BulkInsert(IEnumerable<Post> entities)
    {
        foreach (Post post in entities)
        {
            post.Id = _nextId++;
            _posts.Add(post);
        }
        Console.WriteLine($"  [PostRepository] BulkInsert de {entities.Count()} posts.");
    }
}

/// <summary>
/// Servicio de solo lectura para categorías.
/// Solo necesita IReadOnlyRepository<Category>, no depende de escritura.
/// </summary>
public class ReadOnlyCategoryRepository : IReadOnlyRepository<Category>
{
    private readonly List<Category> _categories = new()
    {
        new Category { Id = 1, Name = "Tecnología", Slug = "tecnologia" },
        new Category { Id = 2, Name = "Ciencia",    Slug = "ciencia" },
        new Category { Id = 3, Name = "Arte",       Slug = "arte" },
    };

    public Category? GetById(int id)
    {
        return _categories.FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<Category> GetAll()
    {
        return _categories.ToList();
    }

    public IEnumerable<Category> Search(Func<Category, bool> predicate)
    {
        return _categories.Where(predicate).ToList();
    }

    public (IEnumerable<Category> Items, int TotalCount) GetPaged(int page, int pageSize)
    {
        int total = _categories.Count;
        IEnumerable<Category> items = _categories.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return (items, total);
    }

    public int Count()
    {
        return _categories.Count;
    }

    public bool Exists(int id)
    {
        return _categories.Any(c => c.Id == id);
    }
}

/// <summary>
/// Servicio de aplicación que procesa las visitas de una página pública del CMS.
/// Solo necesita LEER posts y categorías → depende de IReadOnlyRepository<T>,
/// NO de IRepository<T>. Esto es ISP aplicado en las dependencias.
/// </summary>
public class PublicPageService
{
    private readonly IReadOnlyRepository<Post> _postRepo;
    private readonly IReadOnlyRepository<Category> _categoryRepo;

    // El constructor solo pide IReadOnlyRepository.
    // Si alguien le pasa un repositorio de solo lectura → funciona.
    // Si alguien le pasa un repositorio completo (IRepository) → también funciona,
    // porque IRepository<T> hereda de IReadOnlyRepository<T>.
    public PublicPageService(
        IReadOnlyRepository<Post> postRepo,
        IReadOnlyRepository<Category> categoryRepo)
    {
        _postRepo = postRepo;
        _categoryRepo = categoryRepo;
    }

    public void ShowHomePage()
    {
        Console.WriteLine("\n  --- PÁGINA PÚBLICA DEL CMS ---");
        Console.WriteLine("  Categorías:");
        foreach (Category cat in _categoryRepo.GetAll())
        {
            Console.WriteLine($"    • {cat.Name} ({cat.Slug})");
        }

        Console.WriteLine("  Últimos posts:");
        foreach (Post post in _postRepo.GetPaged(1, 5).Items)
        {
            Console.WriteLine($"    • {post}");
        }
        Console.WriteLine("  --- FIN PÁGINA ---\n");
    }
}

// ══════════════════════════════════════════════════════════════════════════
// PARTE 5 — DEMOSTRACIÓN COMPLETA
// ══════════════════════════════════════════════════════════════════════════

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        DemoViolacionPrinter();
        DemoViolacionRepositorio();
        DemoRefactorPrinter();
        DemoRefactorRepositorio();
        DemoReglaPractica();
    }

    // ──────────────────────────────────────────────────────────────────────
    // DEMO 1: Violación ISP con impresoras
    // ──────────────────────────────────────────────────────────────────────

    static void DemoViolacionPrinter()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("  DEMO 1: VIOLACIÓN ISP — Interfaz gorda de impresora");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        Document doc = new Document("Informe anual", "Contenido del informe...");

        // PremiumPrinter funciona bien con la interfaz gorda...
        IMultiFunctionPrinter premium = new PremiumPrinterViolation();
        Console.WriteLine("[PremiumPrinter — todo funciona]");
        premium.Print(doc);
        premium.Scan();
        premium.Fax("+34 912 345 678", doc);
        premium.Copy();
        Console.WriteLine();

        // Pero BasicPrinter, que solo imprime, también está OBLIGADA
        // a implementar Scan, Fax, Copy y Staple.
        IMultiFunctionPrinter basic = new BasicPrinterViolation();
        Console.WriteLine("[BasicPrinter — solo debería imprimir]");
        basic.Print(doc); // ✅ Esto funciona

        Console.WriteLine("\n  ⚠️ Ahora intentamos llamar a métodos que BasicPrinter no soporta:");
        try
        {
            basic.Fax("+34 912 345 678", doc); // 💥 NotImplementedException
        }
        catch (NotImplementedException ex)
        {
            Console.WriteLine($"  ❌ EXCEPCIÓN: {ex.Message}");
        }

        Console.WriteLine("\n  👉 Conclusión: BasicPrinter depende de métodos que no usa.");
        Console.WriteLine("     Esto rompe el Principio de Segregación de Interfaces.\n");
    }

    // ──────────────────────────────────────────────────────────────────────
    // DEMO 2: Violación ISP con repositorio CMS
    // ──────────────────────────────────────────────────────────────────────

    static void DemoViolacionRepositorio()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("  DEMO 2: VIOLACIÓN ISP — Repositorio gordo en CMS");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        IRepositoryViolation<Post> readOnlyRepo = new ReadOnlyRepositoryViolation();

        // Métodos de lectura: funcionan bien
        Console.WriteLine("[ReadOnlyRepository — métodos de lectura]");
        Console.WriteLine($"  Total posts: {readOnlyRepo.Count()}");
        foreach (Post post in readOnlyRepo.GetAll())
        {
            Console.WriteLine($"  • {post}");
        }
        Console.WriteLine();

        // Métodos de escritura: lanzan excepción aunque el tipo lo permite
        Console.WriteLine("  ⚠️ El tipo IRepositoryViolation<Post> permite llamar a Add():");
        try
        {
            readOnlyRepo.Add(new Post { Title = "Nuevo post" }); // 💥 NotImplementedException
        }
        catch (NotImplementedException ex)
        {
            Console.WriteLine($"  ❌ EXCEPCIÓN: {ex.Message}");
        }

        Console.WriteLine("\n  👉 Conclusión: ReadOnlyRepository depende de Add/Update/Delete");
        Console.WriteLine("     que NUNCA usará. La interfaz IRepositoryViolation es demasiado");
        Console.WriteLine("     grande. Rompe ISP.\n");
    }

    // ──────────────────────────────────────────────────────────────────────
    // DEMO 3: Refactor ISP con impresoras (interfaces segregadas)
    // ──────────────────────────────────────────────────────────────────────

    static void DemoRefactorPrinter()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("  DEMO 3: ISP CORRECTO — Interfaces segregadas de impresora");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        Document doc = new Document("Informe anual", "Contenido del informe...");

        // BasicPrinter SOLO implementa IPrinter → solo tiene Print()
        IPrinter basic = new BasicPrinter();
        Console.WriteLine("[BasicPrinter : IPrinter]");
        basic.Print(doc);
        // basic.Scan() ← NI SIQUIERA COMPILA. El compilador nos protege.
        Console.WriteLine("  ✅ BasicPrinter solo expone Print(). El compilador impide llamar a Scan().\n");

        // ScannerOnly SOLO implementa IScanner → solo tiene Scan()
        IScanner scanner = new ScannerOnly();
        Console.WriteLine("[ScannerOnly : IScanner]");
        scanner.Scan();
        Console.WriteLine("  ✅ ScannerOnly solo expone Scan().\n");

        // PremiumPrinter compone MÚLTIPLES interfaces → tiene todo
        PremiumPrinter premium = new PremiumPrinter();
        Console.WriteLine("[PremiumPrinter : IPrinter, IScanner, IFax, ICopier]");
        premium.Print(doc);
        premium.Scan();
        premium.Fax("+34 912 345 678", doc);
        premium.Copy();
        Console.WriteLine("  ✅ PremiumPrinter expone exactamente lo que necesita.\n");

        Console.WriteLine("  👉 Conclusión: Cada clase implementa SOLO las interfaces que necesita.");
        Console.WriteLine("     Cero NotImplementedException. Cero métodos innecesarios.\n");
    }

    // ──────────────────────────────────────────────────────────────────────
    // DEMO 4: Refactor ISP con repositorios CMS (interfaces segregadas)
    // ──────────────────────────────────────────────────────────────────────

    static void DemoRefactorRepositorio()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("  DEMO 4: ISP CORRECTO — Repositorios segregados en CMS");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        // Repositorio de solo lectura → solo expone métodos de lectura
        IReadOnlyRepository<Post> readOnlyRepo = new ReadOnlyPostRepository();
        Console.WriteLine("[ReadOnlyPostRepository : IReadOnlyRepository<Post>]");
        Console.WriteLine($"  Total posts: {readOnlyRepo.Count()}");
        Post? found = readOnlyRepo.GetById(1);
        Console.WriteLine($"  Post #1: {found}");
        // readOnlyRepo.Add(...) ← NI SIQUIERA COMPILA
        Console.WriteLine("  ✅ Solo métodos de lectura. Add/Update/Delete NO compilan.\n");

        // Repositorio completo → expone lectura Y escritura
        IRepository<Post> fullRepo = new PostRepository();
        Console.WriteLine("[PostRepository : IRepository<Post>]");
        fullRepo.Add(new Post { Title = "Nuevo post desde CMS", Body = "Contenido..." });
        Console.WriteLine($"  Total posts tras añadir: {fullRepo.Count()}");
        fullRepo.Update(new Post { Id = 1, Title = "Título actualizado", Body = "Nuevo body" });
        Console.WriteLine($"  Post #1 actualizado: {fullRepo.GetById(1)}");
        Console.WriteLine("  ✅ Lectura y escritura disponibles.\n");

        // ── Composición en acción: PublicPageService ──
        // El servicio solo pide IReadOnlyRepository<>, pero podemos pasarle
        // tanto ReadOnlyPostRepository como PostRepository (gracias a la herencia).
        IReadOnlyRepository<Category> categoryRepo = new ReadOnlyCategoryRepository();

        Console.WriteLine("[PublicPageService con ReadOnlyPostRepository + ReadOnlyCategoryRepository]");
        PublicPageService pageService1 = new PublicPageService(readOnlyRepo, categoryRepo);
        pageService1.ShowHomePage();

        Console.WriteLine("[PublicPageService con PostRepository (IRepository) + ReadOnlyCategoryRepository]");
        PublicPageService pageService2 = new PublicPageService(fullRepo, categoryRepo);
        pageService2.ShowHomePage();

        Console.WriteLine("  👉 Conclusión: El servicio de página pública solo necesita IReadOnlyRepository.");
        Console.WriteLine("     No le obligamos a depender de métodos de escritura que no usa.\n");
    }

    // ──────────────────────────────────────────────────────────────────────
    // DEMO 5: Regla práctica — Cómo detectar y corregir ISP
    // ──────────────────────────────────────────────────────────────────────

    static void DemoReglaPractica()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("  DEMO 5: REGLA PRÁCTICA — Cómo aplicar ISP");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        Console.WriteLine("  🔴 SEÑALES DE QUE ROMPES ISP:");
        Console.WriteLine("     1. Ves 'throw new NotImplementedException()' en una implementación.");
        Console.WriteLine("     2. Una interfaz tiene más de 3-5 métodos sin relación directa.");
        Console.WriteLine("     3. Varias implementaciones dejan métodos vacíos o lanzan excepciones.");
        Console.WriteLine("     4. Los tests necesitan mocks con métodos que el SUT nunca llama.");
        Console.WriteLine();

        Console.WriteLine("  🟢 CÓMO ARREGLARLO:");
        Console.WriteLine("     1. Divide la interfaz grande en varias pequeñas y cohesivas.");
        Console.WriteLine("     2. Cada interfaz debe tener UNA sola razón de cambio (cohesión).");
        Console.WriteLine("     3. Usa herencia de interfaces para componer (IRepository : IReadOnly + IWriteOnly).");
        Console.WriteLine("     4. Las clases implementan SOLO las interfaces que necesitan.");
        Console.WriteLine("     5. Los consumidores declaran SOLO la interfaz mínima que requieren.");
        Console.WriteLine();

        Console.WriteLine("  📐 EJEMPLO DE HERENCIA DE INTERFACES:");
        Console.WriteLine("     interface IReader<T>        { T? GetById(int id); }");
        Console.WriteLine("     interface IWriter<T>        { void Add(T entity); }");
        Console.WriteLine("     interface IRepository<T> : IReader<T>, IWriter<T> { }");
        Console.WriteLine();
        Console.WriteLine("     • Quien solo lee  → depende de IReader<T>");
        Console.WriteLine("     • Quien solo escribe → depende de IWriter<T>");
        Console.WriteLine("     • Quien hace ambas → depende de IRepository<T>");
        Console.WriteLine();

        Console.WriteLine("  📐 EJEMPLO DE COMPOSICIÓN POR CLASE:");
        Console.WriteLine("     class BasicPrinter : IPrinter                  // Solo 1 interfaz");
        Console.WriteLine("     class ScannerOnly : IScanner                   // Solo 1 interfaz");
        Console.WriteLine("     class PremiumMFP : IPrinter, IScanner, IFax    // Compone varias");
        Console.WriteLine();

        Console.WriteLine("  💡 FRASE PARA RECORDAR:");
        Console.WriteLine("     \"No obligues a un cliente a implementar métodos que no necesita.\"");
        Console.WriteLine("     \"Las interfaces gordas generan acoplamiento innecesario.\"");
        Console.WriteLine("     \"Prefiere muchas interfaces pequeñas a una interfaz gigante.\"");
        Console.WriteLine();

        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("  FIN DE LA DEMOSTRACIÓN — ISP (Interface Segregation Principle)");
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
    }
}
