// ──────────────────────────────────────────────────────────────────────────
// InMemoryCategoryService — Implementación en memoria de ICategoryService
// Módulo 2: POO en C# — Implementación concreta de la interfaz
// ──────────────────────────────────────────────────────────────────────────
// Gestiona categorías jerárquicas en memoria.
// Las categorías pueden tener una categoría padre (ParentCategoryId),
// formando una estructura de árbol que permite navegación por niveles.
//
// Ejemplo de jerarquía:
//   Tecnología (raíz)
//     ├── Backend (subcategoría de Tecnología)
//     │     └── .NET (subcategoría de Backend)
//     └── Frontend (subcategoría de Tecnología)
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Services;

public sealed class InMemoryCategoryService : ICategoryService
{
    // ──────────────────────────────────────────────────────────────────
    // Almacenamiento interno
    // ──────────────────────────────────────────────────────────────────
    private readonly List<Category> _categories = [];

    // ──────────────────────────────────────────────────────────────────
    // CRUD básico
    // ──────────────────────────────────────────────────────────────────

    /// <inheritdoc />
    public Task<IReadOnlyList<Category>> GetAllCategoriesAsync()
    {
        // Ordenadas por DisplayOrder (menor primero) y luego por nombre
        List<Category> ordered = _categories
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToList();

        return Task.FromResult<IReadOnlyList<Category>>(ordered);
    }

    /// <inheritdoc />
    public Task<Category?> GetCategoryByIdAsync(Guid id)
    {
        Category? category = _categories.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(category);
    }

    /// <inheritdoc />
    public Task<Category> CreateCategoryAsync(Category category)
    {
        // ── Validaciones básicas ──
        if (category is null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        if (string.IsNullOrWhiteSpace(category.Name))
        {
            throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(category));
        }

        if (string.IsNullOrWhiteSpace(category.Slug))
        {
            throw new ArgumentException("El slug de la categoría no puede estar vacío.", nameof(category));
        }

        // ── Asignar Id si viene vacío ──
        if (category.Id == Guid.Empty)
        {
            category = category with { Id = Guid.NewGuid() };
        }

        // ── Almacenar ──
        _categories.Add(category);

        return Task.FromResult(category);
    }

    /// <inheritdoc />
    public Task<Category?> UpdateCategoryAsync(Category category)
    {
        if (category is null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        int index = _categories.FindIndex(c => c.Id == category.Id);

        if (index == -1)
        {
            return Task.FromResult<Category?>(null);
        }

        // Reemplazar la categoría existente por la nueva
        _categories[index] = category;

        return Task.FromResult<Category?>(category);
    }

    /// <inheritdoc />
    public Task<bool> DeleteCategoryAsync(Guid id)
    {
        int removed = _categories.RemoveAll(c => c.Id == id);
        return Task.FromResult(removed > 0);
    }

    // ──────────────────────────────────────────────────────────────────
    // Consultas jerárquicas
    // ──────────────────────────────────────────────────────────────────

    /// <inheritdoc />
    public Task<IReadOnlyList<Category>> GetSubcategoriesAsync(Guid parentCategoryId)
    {
        // Filtramos categorías cuyo ParentCategoryId coincida con el ID dado
        List<Category> subcategories = _categories
            .Where(c => c.ParentCategoryId == parentCategoryId)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToList();

        return Task.FromResult<IReadOnlyList<Category>>(subcategories);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Category>> GetRootCategoriesAsync()
    {
        // Las categorías raíz son aquellas sin categoría padre (ParentCategoryId == null)
        List<Category> rootCategories = _categories
            .Where(c => c.ParentCategoryId is null)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToList();

        return Task.FromResult<IReadOnlyList<Category>>(rootCategories);
    }
}
