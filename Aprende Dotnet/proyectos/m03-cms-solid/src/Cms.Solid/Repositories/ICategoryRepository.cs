// ──────────────────────────────────────────────────────────────────────────
// ICategoryRepository — Interfaz de repositorio para Category (ISP + DIP)
// Módulo 3: SOLID — Lección 4 y 5
// ──────────────────────────────────────────────────────────────────────────
// Interfaz segregada: solo métodos de persistencia para Category.
// No incluye lógica de jerarquía (eso va en el servicio).
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Solid.Repositories;

/// <summary>
/// Contrato de persistencia exclusivo para la entidad Category.
/// </summary>
public interface ICategoryRepository
{
    /// <summary>Busca una categoría por su Id.</summary>
    Task<Category?> GetByIdAsync(Guid id);

    /// <summary>Obtiene todas las categorías ordenadas por DisplayOrder.</summary>
    Task<IReadOnlyList<Category>> GetAllAsync();

    /// <summary>Añade una nueva categoría.</summary>
    Task<Category> AddAsync(Category category);

    /// <summary>Actualiza una categoría existente. Devuelve null si no existe.</summary>
    Task<Category?> UpdateAsync(Category category);

    /// <summary>Elimina una categoría por Id.</summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>Obtiene las categorías raíz (sin categoría padre).</summary>
    Task<IReadOnlyList<Category>> GetRootCategoriesAsync();

    /// <summary>Obtiene las subcategorías de una categoría padre.</summary>
    Task<IReadOnlyList<Category>> GetSubcategoriesAsync(Guid parentCategoryId);
}
