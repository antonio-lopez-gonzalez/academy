// ──────────────────────────────────────────────────────────────────────────
// CategoryService — Servicio de categorías refactorizado con SRP y DIP
// Módulo 3: SOLID — Todas las lecciones aplicadas
// ──────────────────────────────────────────────────────────────────────────
// Mismo patrón que PostService: el servicio orquesta, el validador
// valida, el repositorio persiste. Cada clase tiene UNA sola razón
// de cambio.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;
using Cms.Services;
using Cms.Solid.Repositories;

namespace Cms.Solid.Services;

/// <summary>
/// Servicio de aplicación para categorías.
/// Orquesta validación y persistencia.
/// </summary>
public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly CategoryValidator _validator;

    /// <summary>
    /// Inyección de dependencias por constructor.
    /// </summary>
    public CategoryService(ICategoryRepository repository, CategoryValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Category>> GetAllCategoriesAsync()
    {
        return _repository.GetAllAsync();
    }

    /// <inheritdoc />
    public Task<Category?> GetCategoryByIdAsync(Guid id)
    {
        return _repository.GetByIdAsync(id);
    }

    /// <inheritdoc />
    public async Task<Category> CreateCategoryAsync(Category category)
    {
        // ── Validar ──
        if (!_validator.Validate(category, out List<string> errors))
        {
            string errorMessage = string.Join(" ", errors);
            throw new ArgumentException(errorMessage, nameof(category));
        }

        // ── Asignar Id si viene vacío ──
        if (category.Id == Guid.Empty)
        {
            category = category with { Id = Guid.NewGuid() };
        }

        // ── Persistir ──
        return await _repository.AddAsync(category);
    }

    /// <inheritdoc />
    public async Task<Category?> UpdateCategoryAsync(Category category)
    {
        // ── Validar ──
        if (!_validator.Validate(category, out List<string> errors))
        {
            string errorMessage = string.Join(" ", errors);
            throw new ArgumentException(errorMessage, nameof(category));
        }

        // ── Persistir ──
        return await _repository.UpdateAsync(category);
    }

    /// <inheritdoc />
    public Task<bool> DeleteCategoryAsync(Guid id)
    {
        return _repository.DeleteAsync(id);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Category>> GetSubcategoriesAsync(Guid parentCategoryId)
    {
        return _repository.GetSubcategoriesAsync(parentCategoryId);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Category>> GetRootCategoriesAsync()
    {
        return _repository.GetRootCategoriesAsync();
    }
}
