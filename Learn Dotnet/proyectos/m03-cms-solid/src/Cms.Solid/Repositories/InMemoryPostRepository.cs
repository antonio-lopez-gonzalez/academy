// ──────────────────────────────────────────────────────────────────────────
// InMemoryPostRepository — Implementación en memoria de IPostRepository (SRP)
// Módulo 3: SOLID — Lección 1: Single Responsibility Principle
// ──────────────────────────────────────────────────────────────────────────
// Esta clase tiene UNA sola responsabilidad: almacenar y recuperar Posts.
// NO valida (el validador lo hace), NO orquesta (el servicio lo hace),
// NO envía notificaciones. Solo CRUD sobre una lista en memoria.
//
// COMPARACIÓN con M02:
//   Antes: InMemoryPostService hacía TODO (validar + persistir + fechas)
//   Ahora: InMemoryPostRepository solo persiste. El servicio orquesta.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Solid.Repositories;

/// <summary>
/// Repositorio de posts en memoria.
/// Responsabilidad única: acceso a datos (CRUD).
/// </summary>
public sealed class InMemoryPostRepository : IPostRepository
{
    // ── Almacenamiento interno: lista en memoria ──
    // En el Módulo 7 (EF Core) esto se reemplazará por un DbContext.
    private readonly List<Post> _posts = [];

    /// <inheritdoc />
    public Task<Post?> GetByIdAsync(Guid id)
    {
        Post? post = _posts.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(post);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Post>> GetAllAsync()
    {
        List<Post> ordered = _posts
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<Post>>(ordered);
    }

    /// <inheritdoc />
    public Task<Post> AddAsync(Post post)
    {
        _posts.Add(post);
        return Task.FromResult(post);
    }

    /// <inheritdoc />
    public Task<Post?> UpdateAsync(Post post)
    {
        int index = _posts.FindIndex(p => p.Id == post.Id);

        if (index == -1)
        {
            return Task.FromResult<Post?>(null);
        }

        _posts[index] = post;
        return Task.FromResult<Post?>(post);
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id)
    {
        int removed = _posts.RemoveAll(p => p.Id == id);
        return Task.FromResult(removed > 0);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Post>> GetByCategoryAsync(Guid categoryId)
    {
        List<Post> filtered = _posts
            .Where(p => p.CategoryId == categoryId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<Post>>(filtered);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Post>> GetByAuthorAsync(Guid authorId)
    {
        List<Post> filtered = _posts
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<Post>>(filtered);
    }
}
