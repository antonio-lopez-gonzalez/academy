// MÓDULO 9 - LECCIÓN 01: FluentValidation
using FluentValidation;

// Toda la lógica ejecutable primero (top-level statements)

static void PrintValidationResult(string scenario, CreatePostRequest request,
    FluentValidation.Results.ValidationResult result)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"\n=== {scenario} ===");
    Console.ResetColor();
    Console.WriteLine($"  Título: '{request.Title}'");
    Console.WriteLine($"  Contenido: {request.Content.Length} caracteres");
    Console.WriteLine($"  Status: {request.Status}");
    Console.WriteLine($"  Tags: {request.Tags.Count}");
    if (result.IsValid)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  Resultado: VÁLIDO ✓");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  Resultado: INVÁLIDO ✗ ({result.Errors.Count} errores)");
        foreach (var error in result.Errors)
            Console.WriteLine($"    • {error.PropertyName}: {error.ErrorMessage}");
    }
    Console.ResetColor();
}

Console.WriteLine("═══════════════════════════════════════════");
Console.WriteLine("  MÓDULO 9 - LECCIÓN 01: FluentValidation");
Console.WriteLine("═══════════════════════════════════════════");

CreatePostValidator validator = new();

CreatePostRequest r1 = new() { Title = "", Content = "", CategoryId = Guid.Empty, Tags = new() };
PrintValidationResult("Caso 1: Request vacío", r1, await validator.ValidateAsync(r1));

CreatePostRequest r2 = new() { Title = "ABC", Content = "Muy corto", CategoryId = Guid.NewGuid(), Tags = new() { "dotnet" } };
PrintValidationResult("Caso 2: Título y contenido cortos", r2, await validator.ValidateAsync(r2));

CreatePostRequest r3 = new()
{
    Title = "Mi artículo sobre .NET",
    Content = "Contenido suficientemente largo como para pasar la validación mínima de 50 caracteres.",
    CategoryId = Guid.NewGuid(),
    Tags = Enumerable.Range(1, 15).Select(i => $"tag{i}").ToList()
};
PrintValidationResult("Caso 3: Demasiados tags (15)", r3, await validator.ValidateAsync(r3));

CreatePostRequest r4 = new()
{
    Title = "Aprendiendo C# 12",
    Content = "Un contenido de 50 caracteres para pasar validación básica.",
    Status = "Published", CategoryId = Guid.NewGuid(), Tags = new() { "csharp" }
};
PrintValidationResult("Caso 4: Published sin 100 chars", r4, await validator.ValidateAsync(r4));

CreatePostRequest r5 = new()
{
    Title = "Introducción a .NET 8",
    Content = "Contenido suficientemente largo para pasar la validación básica de 50 caracteres.",
    Status = "Draft", CategoryId = Guid.NewGuid(), Tags = new() { "dotnet", "csharp" }
};
var res5 = await validator.ValidateAsync(r5);
PrintValidationResult("Caso 5: Título duplicado (async)", r5, res5);

CreatePostRequest r6 = new()
{
    Title = "Domina FluentValidation en 10 minutos",
    Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
              "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
              "Contenido con más de 100 caracteres para publicación.",
    Status = "Published", CategoryId = Guid.NewGuid(),
    Tags = new() { "dotnet", "validation", "tutorial" },
    AuthorEmail = "autor@ejemplo.com"
};
PrintValidationResult("Caso 6: Request válido", r6, await validator.ValidateAsync(r6));

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("\n═══════════════════════════════════════════");
Console.WriteLine("  BUENAS PRÁCTICAS");
Console.WriteLine("═══════════════════════════════════════════");
Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine("""
  ✓ Separa validación de API de validación de negocio
  ✓ Usa RuleSets para Create vs Update
  ✓ Inyecta dependencias en validadores
  ✓ Testea los validadores de forma aislada
  ✗ No uses DataAnnotations para reglas complejas
  ✗ No lances excepciones desde validadores
""");
Console.ResetColor();

// ================================================================
// DECLARACIONES DE TIPOS (van al final con top-level statements)
// ================================================================

public class CreatePostRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";
    public Guid CategoryId { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? AuthorEmail { get; set; }
}

public class CreatePostValidator : AbstractValidator<CreatePostRequest>
{
    private static readonly HashSet<string> ExistingTitles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Introducción a .NET 8", "Guía de Entity Framework Core"
    };

    public CreatePostValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio")
            .MinimumLength(5).WithMessage("Mínimo 5 caracteres")
            .MaximumLength(200).WithMessage("Máximo 200 caracteres");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("El contenido es obligatorio")
            .MinimumLength(50).WithMessage("Mínimo 50 caracteres");

        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("La categoría es obligatoria");

        RuleFor(x => x.Tags)
            .NotNull().WithMessage("Tags no puede ser null")
            .Must(t => t.Count <= 10).WithMessage("Máximo 10 tags");

        RuleForEach(x => x.Tags)
            .NotEmpty().WithMessage("Cada tag debe tener valor")
            .MaximumLength(50).WithMessage("Máximo 50 chars por tag");

        When(x => !string.IsNullOrWhiteSpace(x.AuthorEmail), () =>
        {
            RuleFor(x => x.AuthorEmail).EmailAddress().WithMessage("Email inválido");
        });

        When(x => x.Status == "Published", () =>
        {
            RuleFor(x => x.Content).MinimumLength(100).WithMessage("Publicado requiere 100 chars");
            RuleFor(x => x.Tags).Must(t => t.Count >= 1).WithMessage("Publicado requiere al menos 1 tag");
        });

        RuleFor(x => x.Title).MustAsync(BeUniqueTitleAsync).WithMessage("Título duplicado");
    }

    private static Task<bool> BeUniqueTitleAsync(string title, CancellationToken ct)
        => Task.FromResult(!ExistingTitles.Contains(title));
}
