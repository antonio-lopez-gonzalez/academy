// ──────────────────────────────────────────────────────────────────────────
// InMemoryTagRepository — Implementación en memoria de ITagRepository
// Módulo 3: SOLID — Lección 1: Single Responsibility Principle
// ──────────────────────────────────────────────────────────────────────────
// Responsabilidad única: acceso a datos (CRUD) para Tag.
// NO valida, NO orquesta, NO envía notificaciones. Solo persiste.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Solid.Repositories;

/// <summary>
/// Repositorio de etiquetas en memoria.
/// Responsabilidad única: acceso a datos (CRUD).
/// </summary>
public sealed class InMemoryTagRepository : ITagRepository
{
    private readonly List<Tag> _tags = [];

    /// <inheritdoc />
    public Task<Tag?> GetByIdAsync(Guid id)
    {
        Tag? tag = _tags.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(tag);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Tag>> GetAllAsync()
    {
        List<Tag> ordered = _tags
            .OrderBy(t => t.Name)
            .ToList();

        return Task.FromResult<IReadOnlyList<Tag>>(ordered);
    }

    /// <inheritdoc />
    public Task<Tag> AddAsync(Tag tag)
    {
        _tags.Add(tag);
        return Task.FromResult(tag);
    }

    /// <inheritdoc />
    public Task<Tag?> UpdateAsync(Tag tag)
    {
        int index = _tags.FindIndex(t => t.Id == tag.Id);

        if (index == -1)
        {
            return Task.FromResult<Tag?>(null);
        }

        _tags[index] = tag;
        return Task.FromResult<Tag?>(tag);
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id)
    {
        int removed = _tags.RemoveAll(t => t.Id == id);
        return Task.FromResult(removed > 0);
    }
}
