// ──────────────────────────────────────────────────────────────────────────
// InMemoryCategoryRepository — Implementación en memoria de ICategoryRepository
// Módulo 3: SOLID — Lección 1: Single Responsibility Principle
// ──────────────────────────────────────────────────────────────────────────
// Responsabilidad única: acceso a datos (CRUD) para Category.
// NO valida, NO orquesta, NO envía notificaciones. Solo persiste.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Solid.Repositories;

/// <summary>
/// Repositorio de categorías en memoria.
/// Responsabilidad única: acceso a datos (CRUD).
/// </summary>
public sealed class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly List<Category> _categories = [];

    /// <inheritdoc />
    public Task<Category?> GetByIdAsync(Guid id)
    {
        Category? category = _categories.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(category);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Category>> GetAllAsync()
    {
        List<Category> ordered = _categories
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToList();

        return Task.FromResult<IReadOnlyList<Category>>(ordered);
    }

    /// <inheritdoc />
    public Task<Category> AddAsync(Category category)
    {
        _categories.Add(category);
        return Task.FromResult(category);
    }

    /// <inheritdoc />
    public Task<Category?> UpdateAsync(Category category)
    {
        int index = _categories.FindIndex(c => c.Id == category.Id);

        if (index == -1)
        {
            return Task.FromResult<Category?>(null);
        }

        _categories[index] = category;
        return Task.FromResult<Category?>(category);
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id)
    {
        int removed = _categories.RemoveAll(c => c.Id == id);
        return Task.FromResult(removed > 0);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Category>> GetRootCategoriesAsync()
    {
        List<Category> roots = _categories
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToList();

        return Task.FromResult<IReadOnlyList<Category>>(roots);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Category>> GetSubcategoriesAsync(Guid parentCategoryId)
    {
        List<Category> children = _categories
            .Where(c => c.ParentCategoryId == parentCategoryId)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToList();

        return Task.FromResult<IReadOnlyList<Category>>(children);
    }
}
