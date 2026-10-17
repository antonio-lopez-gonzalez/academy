// ──────────────────────────────────────────────────────────────────────────
// Cms.Domain — Modelos de dominio del CMS
// Módulo 1: Fundamentos de C#
// ──────────────────────────────────────────────────────────────────────────
// Los modelos de dominio representan las entidades principales del CMS.
// Se implementan como record (inmutables por defecto, value equality)
// para seguir buenas prácticas de DDD.
// En módulos posteriores añadiremos Value Objects, Aggregates, etc.
// ──────────────────────────────────────────────────────────────────────────

namespace Cms.Domain;

/// <summary>
/// Representa un artículo o página del CMS.
/// </summary>
public sealed record Post
{
    /// <summary>Identificador único del post.</summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>Título del post. Obligatorio, máximo 200 caracteres.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Slug para la URL amigable (ej: "mi-primer-post").</summary>
    public string Slug { get; init; } = string.Empty;

    /// <summary>Contenido completo del post en formato Markdown o HTML.</summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>Resumen o extracto del post (máx. 500 caracteres).</summary>
    public string? Excerpt { get; init; }

    /// <summary>Autor del post. Referencia al User que lo creó.</summary>
    public Guid AuthorId { get; init; }

    /// <summary>Estado de publicación: Draft, Published, Archived.</summary>
    public PostStatus Status { get; init; } = PostStatus.Draft;

    /// <summary>Fecha de publicación. Null si aún es borrador.</summary>
    public DateTime? PublishedAt { get; init; }

    /// <summary>Fecha de creación (automática).</summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>Fecha de última modificación (automática).</summary>
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>Categoría principal del post.</summary>
    public Guid? CategoryId { get; init; }

    /// <summary>Etiquetas asociadas al post (relación N:N).</summary>
    public IReadOnlyList<Guid> TagIds { get; init; } = [];
}

/// <summary>
/// Estados posibles de un post en el flujo editorial.
/// </summary>
public enum PostStatus
{
    /// <summary>Borrador: visible solo para el autor y editores.</summary>
    Draft = 0,

    /// <summary>Publicado: visible para todos los lectores.</summary>
    Published = 1,

    /// <summary>Archivado: oculto pero conservado en base de datos.</summary>
    Archived = 2
}

/// <summary>
/// Representa una categoría para organizar los posts.
/// Las categorías son jerárquicas (pueden tener una categoría padre).
/// </summary>
public sealed record Category
{
    /// <summary>Identificador único de la categoría.</summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>Nombre de la categoría (ej: "Tecnología", "Tutoriales").</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Slug para URL amigable (ej: "tecnologia").</summary>
    public string Slug { get; init; } = string.Empty;

    /// <summary>Descripción opcional de la categoría.</summary>
    public string? Description { get; init; }

    /// <summary>ID de la categoría padre (null si es categoría raíz).</summary>
    public Guid? ParentCategoryId { get; init; }

    /// <summary>Orden de visualización (menor = primero).</summary>
    public int DisplayOrder { get; init; }
}

/// <summary>
/// Representa una etiqueta para clasificar posts.
/// Las etiquetas son planas (sin jerarquía) a diferencia de las categorías.
/// </summary>
public sealed record Tag
{
    /// <summary>Identificador único de la etiqueta.</summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>Nombre de la etiqueta (ej: "csharp", "dotnet", "tutorial").</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Slug para URL (ej: "csharp").</summary>
    public string Slug { get; init; } = string.Empty;
}

/// <summary>
/// Representa un usuario del CMS (autor, editor o administrador).
/// </summary>
public sealed record User
{
    /// <summary>Identificador único del usuario.</summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>Nombre de usuario único para login.</summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>Email del usuario, único.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Hash de la contraseña (nunca texto plano).</summary>
    public string PasswordHash { get; init; } = string.Empty;

    /// <summary>Nombre público mostrado en los posts.</summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>Biografía corta del autor (opcional).</summary>
    public string? Bio { get; init; }

    /// <summary>URL del avatar del usuario (opcional).</summary>
    public string? AvatarUrl { get; init; }

    /// <summary>Rol del usuario en el sistema.</summary>
    public UserRole Role { get; init; } = UserRole.Author;

    /// <summary>Fecha de registro.</summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>Indica si la cuenta está activa (no baneada/desactivada).</summary>
    public bool IsActive { get; init; } = true;
}

/// <summary>
/// Roles disponibles en el CMS.
/// </summary>
public enum UserRole
{
    /// <summary>Autor: puede crear y gestionar sus propios posts.</summary>
    Author = 0,

    /// <summary>Editor: puede gestionar posts de cualquier autor.</summary>
    Editor = 1,

    /// <summary>Administrador: acceso total al sistema.</summary>
    Admin = 2
}
