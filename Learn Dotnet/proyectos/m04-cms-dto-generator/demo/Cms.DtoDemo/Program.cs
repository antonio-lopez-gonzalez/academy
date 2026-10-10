using Cms.DtoGenerator;

// ================================================================
// Demo del Source Generator de DTOs del CMS
// ================================================================
// El generador busca clases marcadas con [GenerateDto] y genera automáticamente:
//   1. Un DTO con las mismas propiedades públicas (en namespace *.Dtos)
//   2. Un método de extensión ToDto() para mapear la entidad al DTO
//
// Los archivos generados se crean en: obj/Generated/Cms.DtoGenerator/
// ================================================================

// ---------- PUNTO DE ENTRADA (top-level statements) ----------
// Nota: los tipos (enums, clases) se declaran al final del archivo.
// En top-level statements, el código ejecutable debe ir antes que las declaraciones de tipo.

Console.WriteLine("=== Source Generator de DTOs — Demo funcional ===\n");

// Crear objetos de dominio
var post = new Post
{
    Id = Guid.NewGuid(),
    Title = "Introducción a Source Generators",
    Slug = "intro-source-generators",
    Content = "Los Source Generators ejecutan código en tiempo de compilación...",
    AuthorId = "u-001",
    Status = PostStatus.Published,
    CreatedAt = new DateTime(2026, 7, 29, 12, 0, 0)
};

var category = new Category
{
    Id = Guid.NewGuid(),
    Name = ".NET Avanzado",
    Slug = "dotnet-avanzado",
    Description = "Artículos sobre temas avanzados de .NET"
};

var user = new User
{
    Id = Guid.NewGuid(),
    Username = "antonio_dev",
    DisplayName = "Antonio García",
    Email = "antonio@ejemplo.com",
    Role = UserRole.Admin,
    CreatedAt = DateTime.UtcNow
};

// Usar los DTOs generados automáticamente por el Source Generator
// PostDto, PostExtensions.ToDto(), CategoryDto, etc. NO existen en el código fuente:
// los genera el compilador durante la compilación.

var postDto = post.ToDto();
var categoryDto = category.ToDto();
var userDto = user.ToDto();

// Mostrar resultados
Console.WriteLine("--- PostDto (generado automáticamente) ---");
Console.WriteLine($"  Id:        {postDto.Id}");
Console.WriteLine($"  Title:     {postDto.Title}");
Console.WriteLine($"  Slug:      {postDto.Slug}");
Console.WriteLine($"  Content:   {postDto.Content[..Math.Min(50, postDto.Content.Length)]}...");
Console.WriteLine($"  AuthorId:  {postDto.AuthorId}");
Console.WriteLine($"  Status:    {postDto.Status}");
Console.WriteLine($"  CreatedAt: {postDto.CreatedAt:yyyy-MM-dd HH:mm}");

Console.WriteLine();
Console.WriteLine("--- CategoryDto (generado automáticamente) ---");
Console.WriteLine($"  Id:          {categoryDto.Id}");
Console.WriteLine($"  Name:        {categoryDto.Name}");
Console.WriteLine($"  Slug:        {categoryDto.Slug}");
Console.WriteLine($"  Description: {categoryDto.Description}");

Console.WriteLine();
Console.WriteLine("--- UserDto (generado automáticamente) ---");
Console.WriteLine($"  Id:          {userDto.Id}");
Console.WriteLine($"  Username:    {userDto.Username}");
Console.WriteLine($"  DisplayName: {userDto.DisplayName}");
Console.WriteLine($"  Email:       {userDto.Email}");
Console.WriteLine($"  Role:        {userDto.Role}");
Console.WriteLine($"  CreatedAt:   {userDto.CreatedAt:yyyy-MM-dd HH:mm:ss}");

Console.WriteLine();
Console.WriteLine("=== El Source Generator ha generado correctamente los DTOs ===");

// ---------- DECLARACIONES DE TIPO ----------
// En top-level statements, las declaraciones de tipo van después del código ejecutable.

enum PostStatus
{
    Draft,
    Published,
    Archived
}

enum UserRole
{
    Author,
    Editor,
    Admin
}

[GenerateDto]
class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public PostStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

[GenerateDto]
class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

[GenerateDto]
class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
