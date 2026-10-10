using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

// ─────────────────────────────────────────────────────────────
// Lección 03 — System.Text.Json en profundidad + Swagger
// Configuración avanzada de serialización JSON y documentación
// interactiva de la API con Swagger/OpenAPI.
// ─────────────────────────────────────────────────────────────

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════════════
// CONFIGURACIÓN DE System.Text.Json
// ═══════════════════════════════════════════════════════════════

builder.Services.ConfigureHttpJsonOptions(options =>
{
    JsonSerializerOptions o = options.SerializerOptions;

    // ── camelCase: "firstName" en vez de "FirstName" ────────
    // Es el estándar en JSON/REST APIs. Ya viene por defecto con ASP.NET Core.
    o.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    // ── Indentación para desarrollo (en prod se quita) ──────
    o.WriteIndented = true;

    // ── Ignorar propiedades null al serializar ───────────────
    // Si una propiedad es null, no se incluye en el JSON de salida.
    o.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

    // ── Evitar ciclos de referencia (ej: Post → Author → Posts → ...) ──
    // En vez de lanzar excepción, omite la referencia circular.
    o.ReferenceHandler = ReferenceHandler.IgnoreCycles;

    // ── Permitir comas finales y comentarios (no estándar, útil en dev) ──
    o.AllowTrailingCommas = true;
    o.ReadCommentHandling = JsonCommentHandling.Skip;

    // ── Case-insensitive al deserializar ─────────────────────
    // "FirstName", "firstname", "FIRSTNAME" → todos mapean a la propiedad FirstName
    o.PropertyNameCaseInsensitive = true;

    // ── Registrar el source generator para AOT ───────────────
    // Esto reemplaza la reflection en runtime → más rápido y compatible con Native AOT.
    o.TypeInfoResolver = AppJsonSerializerContext.Default;
});

// ═══════════════════════════════════════════════════════════════
// CONFIGURACIÓN DE SWAGGER / OPENAPI
// ═══════════════════════════════════════════════════════════════

builder.Services.AddEndpointsApiExplorer(); // Necesario para Minimal APIs
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CMS API - Demo JSON + Swagger",
        Version = "v1",
        Description = "API de demostración para la lección 03 del Módulo 8. " +
                      "Muestra configuración avanzada de System.Text.Json y documentación Swagger.",
        Contact = new OpenApiContact
        {
            Name = "Curso .NET 8",
            Email = "profesor@curso-dotnet.com"
        }
    });

    // ── Incluir comentarios XML en Swagger ──────────────────
    // Para que esto funcione, el .csproj debe tener <GenerateDocumentationFile>true</GenerateDocumentationFile>
    // Los comentarios /// <summary> se muestran en Swagger UI.
    string xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

WebApplication app = builder.Build();
app.Urls.Add("http://localhost:5000");

// ── Swagger UI en /swagger ──────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CMS API v1");
    options.RoutePrefix = "swagger"; // Accesible en /swagger
});

// ═══════════════════════════════════════════════════════════════
// ENDPOINTS DE DEMOSTRACIÓN
// ═══════════════════════════════════════════════════════════════

// ── GET /api/demo-json — Demuestra serialización con las opciones configuradas ──
app.MapGet("/api/demo-json", () =>
{
    // Al serializar: camelCase + ignora nulls + indentado
    BlogPost post = new()
    {
        Title = "System.Text.Json en .NET 8",
        Author = new Author("Profesor .NET", "profe@email.com"),
        Tags = ["csharp", "json", "dotnet", "api"],
        PublishedAt = new DateTime(2025, 3, 15, 10, 30, 0, DateTimeKind.Utc),
        Metadata = null, // No aparecerá en el JSON porque es null
        Content = new TextContent { Value = "Artículo completo aquí..." },
        Attachments =
        [
            new ImageAttachment("https://picsum.photos/800/600", "Imagen de portada", 1024, 768),
            new FileAttachment("documento.pdf", 2048576)
        ]
    };
    return Results.Ok(post);
});

// ── POST /api/demo-json — Demuestra deserialización (case-insensitive) ──
app.MapPost("/api/demo-json", (BlogPost post) =>
{
    // El JSON de entrada puede usar cualquier casing: Title, title, TITLE → todos funcionan
    return Results.Ok(new
    {
        Message = "Post recibido correctamente (case-insensitive)",
        post.Title,
        TagsCount = post.Tags?.Length ?? 0,
        IsPublished = post.PublishedAt != default
    });
});

// ── GET /api/polymorphism — Demuestra polimorfismo JSON ──
app.MapGet("/api/polymorphism", () =>
{
    List<Attachment> attachments =
    [
        new ImageAttachment("https://picsum.photos/800/600", "Portada", 800, 600),
        new FileAttachment("guia.pdf", 1048576)
    ];

    return Results.Ok(new
    {
        Message = "El campo 'attachments' usa discriminación polimórfica: cada elemento tiene $type",
        Attachments = attachments
    });
});

app.Run();

// ═══════════════════════════════════════════════════════════════
// MODELOS DE DATOS CON ATRIBUTOS JSON
// ═══════════════════════════════════════════════════════════════

/// <summary>Post de blog con varios atributos JSON de demostración.</summary>
public class BlogPost
{
    /// <summary>Título del post.</summary>
    public required string Title { get; init; }

    /// <summary>Autor del post.</summary>
    public required Author Author { get; init; }

    /// <summary>
    /// Tags asociados. [JsonIgnore] condicional: no se serializa si es null.
    /// Al estar configurado DefaultIgnoreCondition = WhenWritingNull, los null ya se omiten.
    /// [JsonPropertyName] permite renombrar la propiedad en JSON.
    /// </summary>
    [JsonPropertyName("tags")]
    public string[]? Tags { get; init; }

    /// <summary>
    /// Fecha de publicación. [JsonIgnore] excluye completamente esta propiedad del JSON.
    /// Se usaría para campos internos que no deben exponerse en la API.
    /// </summary>
    [JsonIgnore]
    public string? InternalNotes { get; init; }

    /// <summary>Fecha de publicación.</summary>
    public DateTime PublishedAt { get; init; }

    /// <summary>
    /// Metadatos opcionales. No aparecerán en el JSON si son null
    /// gracias a DefaultIgnoreCondition.WhenWritingNull.
    /// </summary>
    public Metadata? Metadata { get; init; }

    /// <summary>Contenido del post (polimórfico).</summary>
    public Content? Content { get; init; }

    /// <summary>Adjuntos con discriminación polimórfica.</summary>
    public List<Attachment>? Attachments { get; init; }
}

/// <summary>Autor del post.</summary>
/// <param name="Name">Nombre completo del autor.</param>
/// <param name="Email">Correo electrónico.</param>
public record Author(string Name, string Email);

/// <summary>Metadatos opcionales (pueden ser null).</summary>
public record Metadata(string Source, DateTime LastModified);

// ── Jerarquía polimórfica para Content (contenido del post) ──

/// <summary>
/// Clase base para el contenido de un post.
/// [JsonPolymorphic] configura la discriminación de tipos.
/// [JsonDerivedType] registra los subtipos conocidos.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$contentType")]
[JsonDerivedType(typeof(TextContent), nameof(TextContent))]
[JsonDerivedType(typeof(HtmlContent), nameof(HtmlContent))]
public abstract class Content
{
    public required string Value { get; init; }
}

/// <summary>Contenido en texto plano.</summary>
public class TextContent : Content { }

/// <summary>Contenido en HTML.</summary>
public class HtmlContent : Content { }

// ── Jerarquía polimórfica para Attachment (adjuntos) ──

/// <summary>
/// Clase base para adjuntos con discriminación polimórfica.
/// Cada subtipo añade un campo "$type" automáticamente en el JSON.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ImageAttachment), nameof(ImageAttachment))]
[JsonDerivedType(typeof(FileAttachment), nameof(FileAttachment))]
public abstract record Attachment(string Url, long SizeBytes);

/// <summary>Adjunto de tipo imagen.</summary>
public record ImageAttachment(string Url, string AltText, int Width, int Height)
    : Attachment(Url, SizeBytes: 0);

/// <summary>Adjunto de tipo archivo genérico.</summary>
public record FileAttachment(string Url, long SizeBytes)
    : Attachment(Url, SizeBytes);

// ═══════════════════════════════════════════════════════════════
// JSON SOURCE GENERATOR (para AOT y rendimiento)
// ═══════════════════════════════════════════════════════════════

/// <summary>
/// Contexto de serialización generado en tiempo de compilación.
/// Reemplaza la reflection por código generado → más rápido y compatible con Native AOT.
/// 
/// [JsonSerializable] registra cada tipo que se va a serializar/deserializar.
/// La clase debe ser partial — el source generator completa la otra mitad.
/// </summary>
[JsonSerializable(typeof(BlogPost))]
[JsonSerializable(typeof(Author))]
[JsonSerializable(typeof(Metadata))]
[JsonSerializable(typeof(Content))]
[JsonSerializable(typeof(TextContent))]
[JsonSerializable(typeof(HtmlContent))]
[JsonSerializable(typeof(Attachment))]
[JsonSerializable(typeof(ImageAttachment))]
[JsonSerializable(typeof(FileAttachment))]
[JsonSerializable(typeof(List<BlogPost>))]
[JsonSerializable(typeof(Dictionary<string, object>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
    // El source generator genera automáticamente:
    // - Default → propiedad estática con la configuración optimizada
    // - Getters para cada tipo registrado
    // No necesitas escribir nada aquí.
}
