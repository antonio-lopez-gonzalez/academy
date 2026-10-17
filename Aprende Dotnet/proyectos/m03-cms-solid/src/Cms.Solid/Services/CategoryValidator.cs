// ──────────────────────────────────────────────────────────────────────────
// CategoryValidator — Validador de la entidad Category (SRP)
// Módulo 3: SOLID — Lección 1: Single Responsibility Principle
// ──────────────────────────────────────────────────────────────────────────
// Igual que PostValidator, esta clase tiene UNA sola responsabilidad:
// validar los datos de una categoría antes de que lleguen al repositorio.
//
// Ventaja clave: si mañana el equipo decide que Name debe tener máximo 150
// caracteres en vez de 100, solo se cambia AQUÍ. El servicio y el
// repositorio no se enteran.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Solid.Services;

/// <summary>
/// Validador exclusivo para la entidad Category.
/// Contiene todas las reglas de integridad de datos de una categoría.
/// </summary>
public sealed class CategoryValidator
{
    // ──────────────────────────────────────────────────────────────────
    // Constantes de validación
    // ──────────────────────────────────────────────────────────────────
    private const int NameMaxLength = 100;

    /// <summary>
    /// Valida una categoría según las reglas de negocio.
    /// </summary>
    /// <param name="category">La categoría a validar.</param>
    /// <param name="errors">Lista de errores encontrados (out).</param>
    /// <returns>true si es válida; false si hay errores.</returns>
    public bool Validate(Category? category, out List<string> errors)
    {
        errors = [];

        // ── Regla 1: La categoría no puede ser null ──
        if (category is null)
        {
            errors.Add("La categoría no puede ser nula.");
            return false;
        }

        // ── Regla 2: Name requerido, no vacío, máximo 100 caracteres ──
        if (string.IsNullOrWhiteSpace(category.Name))
        {
            errors.Add("El nombre de la categoría es obligatorio y no puede estar vacío.");
        }
        else if (category.Name.Length > NameMaxLength)
        {
            errors.Add(
                $"El nombre de la categoría no puede superar los {NameMaxLength} caracteres. " +
                $"Longitud actual: {category.Name.Length}.");
        }

        // ── Regla 3: Slug requerido y no vacío ──
        // El slug se usa para la URL: /categorias/tecnologia
        if (string.IsNullOrWhiteSpace(category.Slug))
        {
            errors.Add("El slug de la categoría es obligatorio y no puede estar vacío.");
        }

        return errors.Count == 0;
    }
}
