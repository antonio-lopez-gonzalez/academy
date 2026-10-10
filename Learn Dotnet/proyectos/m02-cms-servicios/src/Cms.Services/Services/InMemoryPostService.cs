// ──────────────────────────────────────────────────────────────────────────
// InMemoryPostService — Implementación en memoria de IPostService
// Módulo 2: POO en C# — Implementación concreta de la interfaz
// ──────────────────────────────────────────────────────────────────────────
// Esta implementación almacena los posts en una List<Post> en memoria.
// Es útil para desarrollo, testing y prototipado rápido.
// En módulos posteriores (M07) se reemplazará por una implementación
// con Entity Framework Core + PostgreSQL.
//
// Patrón aplicado: las implementaciones concretas dependen de la abstracción
// (IPostService), no al revés (DIP — Dependency Inversion Principle).
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Services;

public sealed class InMemoryPostService : IPostService
{
    // ──────────────────────────────────────────────────────────────────
    // Almacenamiento interno: lista en memoria
    // En un escenario real, esto sería un DbContext de EF Core.
    // ──────────────────────────────────────────────────────────────────
    private readonly List<Post> _posts = [];

    // ──────────────────────────────────────────────────────────────────
    // CRUD básico
    // ──────────────────────────────────────────────────────────────────

    /// <inheritdoc />
    public Task<IReadOnlyList<Post>> GetAllPostsAsync()
    {
        // Ordenamos por fecha de creación descendente (más reciente primero)
        List<Post> ordered = _posts
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<Post>>(ordered);
    }

    /// <inheritdoc />
    public Task<Post?> GetPostByIdAsync(Guid id)
    {
        // Buscamos el primer post cuyo Id coincida
        // Equals() en records compara por valor, pero Guid usa == estructural
        Post? post = _posts.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(post);
    }

    /// <inheritdoc />
    public Task<Post> CreatePostAsync(Post post)
    {
        // ── Validaciones básicas ──
        if (post is null)
        {
            throw new ArgumentNullException(nameof(post));
        }

        if (string.IsNullOrWhiteSpace(post.Title))
        {
            throw new ArgumentException("El título del post no puede estar vacío.", nameof(post));
        }

        // ── Asignar Id si viene vacío ──
        if (post.Id == Guid.Empty)
        {
            post = post with { Id = Guid.NewGuid() };
        }

        // ── Asignar fechas de creación y modificación ──
        DateTime now = DateTime.UtcNow;
        post = post with
        {
            CreatedAt = post.CreatedAt == default ? now : post.CreatedAt,
            UpdatedAt = now
        };

        // ── Almacenar ──
        _posts.Add(post);

        return Task.FromResult(post);
    }

    /// <inheritdoc />
    public Task<Post?> UpdatePostAsync(Post post)
    {
        if (post is null)
        {
            throw new ArgumentNullException(nameof(post));
        }

        // Buscar el índice del post existente
        int index = _posts.FindIndex(p => p.Id == post.Id);

        if (index == -1)
        {
            // No se encontró el post a actualizar
            return Task.FromResult<Post?>(null);
        }

        // ── Actualizar con nueva fecha de modificación ──
        // Usamos 'with' para crear una copia inmutable con UpdatedAt actualizado
        Post updated = post with { UpdatedAt = DateTime.UtcNow };

        // Reemplazar en la lista
        _posts[index] = updated;

        return Task.FromResult<Post?>(updated);
    }

    /// <inheritdoc />
    public Task<bool> DeletePostAsync(Guid id)
    {
        // Buscar el post por Id y eliminarlo si existe
        int removed = _posts.RemoveAll(p => p.Id == id);

        // removed > 0 significa que se eliminó al menos un elemento
        return Task.FromResult(removed > 0);
    }

    // ──────────────────────────────────────────────────────────────────
    // Consultas filtradas
    // ──────────────────────────────────────────────────────────────────

    /// <inheritdoc />
    public Task<IReadOnlyList<Post>> GetPostsByCategoryAsync(Guid categoryId)
    {
        List<Post> filtered = _posts
            .Where(p => p.CategoryId == categoryId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<Post>>(filtered);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Post>> GetPostsByTagAsync(Guid tagId)
    {
        // Un post tiene una lista de TagIds; filtramos los que contienen el tagId
        List<Post> filtered = _posts
            .Where(p => p.TagIds.Contains(tagId))
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<Post>>(filtered);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Post>> GetPostsByAuthorAsync(Guid authorId)
    {
        List<Post> filtered = _posts
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<Post>>(filtered);
    }
}
