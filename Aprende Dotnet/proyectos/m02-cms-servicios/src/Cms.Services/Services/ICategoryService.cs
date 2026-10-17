// ──────────────────────────────────────────────────────────────────────────
// ICategoryService — Interfaz para la gestión de categorías del CMS
// Módulo 2: POO en C# — Interfaces y abstracciones
// ──────────────────────────────────────────────────────────────────────────
// Las categorías son jerárquicas: una categoría puede tener una categoría
// padre (ParentCategoryId). Esto permite crear árboles de navegación
// (ej: Tecnología > Backend > .NET).
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Services;

public interface ICategoryService
{
    /// <summary>
    /// Obtiene todas las categorías del sistema, ordenadas por DisplayOrder.
    /// </summary>
    Task<IReadOnlyList<Category>> GetAllCategoriesAsync();

    /// <summary>
    /// Busca una categoría por su identificador único.
    /// Devuelve null si no se encuentra.
    /// </summary>
    Task<Category?> GetCategoryByIdAsync(Guid id);

    /// <summary>
    /// Crea una nueva categoría.
    /// Si el Id está vacío, se asigna uno nuevo automáticamente.
    /// Lanza ArgumentNullException si la categoría es null.
    /// Lanza ArgumentException si el nombre está vacío.
    /// </summary>
    Task<Category> CreateCategoryAsync(Category category);

    /// <summary>
    /// Actualiza una categoría existente.
    /// Devuelve la categoría actualizada, o null si no se encontró.
    /// </summary>
    Task<Category?> UpdateCategoryAsync(Category category);

    /// <summary>
    /// Elimina una categoría por su identificador.
    /// Devuelve true si se eliminó, false si no existía.
    /// </summary>
    Task<bool> DeleteCategoryAsync(Guid id);

    /// <summary>
    /// Obtiene todas las subcategorías de una categoría padre.
    /// </summary>
    Task<IReadOnlyList<Category>> GetSubcategoriesAsync(Guid parentCategoryId);

    /// <summary>
    /// Obtiene las categorías raíz (aquellas sin categoría padre).
    /// </summary>
    Task<IReadOnlyList<Category>> GetRootCategoriesAsync();
}
