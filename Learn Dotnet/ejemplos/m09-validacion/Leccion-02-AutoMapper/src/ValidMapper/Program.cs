// MÓDULO 9 - LECCIÓN 02: AutoMapper vs Mapeo Manual
using AutoMapper;

// ---- PROGRAMA (top-level) ----
Console.WriteLine("══════════════════════════════════════════════════");
Console.WriteLine("  MÓDULO 9 - LECCIÓN 02: AutoMapper vs Manual");
Console.WriteLine("══════════════════════════════════════════════════");

Post post = new()
{
    Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
    Title = "Introducción a AutoMapper en .NET 8",
    Content = "AutoMapper es una biblioteca que automatiza el mapeo entre objetos. " +
              "Ahorra escribir código repetitivo pero introduce complejidad y magia. " +
              "En esta lección exploramos cuándo usarlo y cuándo no.",
    Slug = "introduccion-a-automapper-en-net-8",
    IsPublished = true,
    PublishedAt = new DateTime(2024, 1, 15),
    CreatedAt = new DateTime(2024, 1, 10),
    AuthorId = Guid.NewGuid(),
    AuthorFirstName = "María",
    AuthorLastName = "González",
    ViewCount = 1247,
    InternalNotes = "Revisar SEO antes de republicar Q2 2024"
};

CreatePostRequest request = new()
{
    Title = "Nuevo Artículo Técnico",
    Content = "Contenido del nuevo artículo...",
    AuthorFirstName = "Carlos",
    AuthorLastName = "López"
};

// A) Mapeo manual
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n--- A) MAPEO MANUAL ---");
Console.ResetColor();
PostDto manualDto = post.ToDto();
Post manualEntity = request.ToEntity();
Console.WriteLine($"  Título: {manualDto.Title}");
Console.WriteLine($"  AuthorFullName: {manualDto.AuthorFullName}");
Console.WriteLine($"  Status: {manualDto.Status}");
Console.WriteLine($"  Slug generado: {manualEntity.Slug}");

// B) AutoMapper
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n--- B) AUTOMAPPER ---");
Console.ResetColor();
MapperConfiguration config = new(cfg => cfg.AddProfile<PostMappingProfile>());
config.AssertConfigurationIsValid();
IMapper mapper = config.CreateMapper();
PostDto autoDto = mapper.Map<PostDto>(post);
Post autoEntity = mapper.Map<Post>(request);
Console.WriteLine($"  Título: {autoDto.Title}");
Console.WriteLine($"  AuthorFullName: {autoDto.AuthorFullName}");
Console.WriteLine($"  Status: {autoDto.Status}");
Console.WriteLine($"  Slug generado: {autoEntity.Slug}");

// C) Colecciones
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n--- C) MAPEO DE COLECCIONES ---");
Console.ResetColor();
List<Post> posts = new()
{
    new() { Title = "Post 1", Content = "AAAA", AuthorFirstName = "A", AuthorLastName = "B", IsPublished = true, Slug = "post-1" },
    new() { Title = "Post 2", Content = "BBBB", AuthorFirstName = "C", AuthorLastName = "D", IsPublished = false, Slug = "post-2" },
    new() { Title = "Post 3", Content = "CCCC", AuthorFirstName = "E", AuthorLastName = "F", IsPublished = true, Slug = "post-3" },
};
List<PostDto> autoDtos = mapper.Map<List<PostDto>>(posts);
List<PostDto> manualDtos = posts.Select(p => p.ToDto()).ToList();
foreach (var (auto, manual) in autoDtos.Zip(manualDtos))
    Console.WriteLine($"  {auto.Title} | Status={auto.Status} | Author={auto.AuthorFullName}");

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("\n══════════════════════════════════════════════════");
Console.WriteLine("  ¿CUÁNDO USAR CADA ENFOQUE?");
Console.WriteLine("══════════════════════════════════════════════════");
Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine("""
  MANUAL: proyectos pequeños, lógica compleja, trazabilidad
  AUTOMAPPER: APIs con muchos DTOs, mapeos simples, CRUDs
  REGLA: 80% mapeos triviales con AutoMapper, 20% complejos manual
""");
Console.ResetColor();

// ================================================================
// TIPOS
// ================================================================

public class Post
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorFirstName { get; set; } = string.Empty;
    public string AuthorLastName { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public string InternalNotes { get; set; } = "Notas privadas";
}

public class PostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string AuthorFullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ViewCount { get; set; }
}

public class CreatePostRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorFirstName { get; set; } = string.Empty;
    public string AuthorLastName { get; set; } = string.Empty;
}

public static class PostMappingExtensions
{
    public static PostDto ToDto(this Post post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Summary = post.Content.Length > 100 ? post.Content[..100] + "..." : post.Content,
        Slug = post.Slug,
        AuthorFullName = $"{post.AuthorFirstName} {post.AuthorLastName}",
        CreatedAt = post.CreatedAt,
        Status = post.IsPublished ? "Published" : "Draft",
        ViewCount = post.ViewCount
    };

    public static Post ToEntity(this CreatePostRequest request) => new()
    {
        Title = request.Title,
        Content = request.Content,
        Slug = request.Title.ToLowerInvariant().Replace(" ", "-").Replace(".", "").Replace(",", ""),
        AuthorFirstName = request.AuthorFirstName,
        AuthorLastName = request.AuthorLastName,
        CreatedAt = DateTime.UtcNow
    };
}

public class PostMappingProfile : Profile
{
    public PostMappingProfile()
    {
        CreateMap<Post, PostDto>()
            .ForMember(d => d.Summary, o => o.MapFrom(s =>
                s.Content.Length > 100 ? s.Content.Substring(0, 100) + "..." : s.Content))
            .ForMember(d => d.AuthorFullName, o => o.MapFrom(s =>
                $"{s.AuthorFirstName} {s.AuthorLastName}"))
            .ForMember(d => d.Status, o => o.MapFrom(s =>
                s.IsPublished ? "Published" : "Draft"));

        CreateMap<CreatePostRequest, Post>()
            .ForMember(d => d.Slug, o => o.MapFrom(s =>
                s.Title.ToLowerInvariant().Replace(" ", "-").Replace(".", "").Replace(",", "")))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.AuthorId, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.IsPublished, o => o.Ignore())
            .ForMember(d => d.PublishedAt, o => o.Ignore())
            .ForMember(d => d.ViewCount, o => o.Ignore())
            .ForMember(d => d.InternalNotes, o => o.Ignore());
    }
}
