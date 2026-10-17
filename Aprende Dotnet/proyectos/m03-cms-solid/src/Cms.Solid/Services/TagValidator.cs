// ──────────────────────────────────────────────────────────────────────────
// TagValidator — Validador de la entidad Tag (SRP)
// Módulo 3: SOLID — Lección 1: Single Responsibility Principle
// ──────────────────────────────────────────────────────────────────────────
// Las etiquetas (tags) son más simples que los posts y categorías,
// pero aún así merecen su propio validador. Cada entidad tiene su
// propio validador porque las reglas de cada una pueden evolucionar
// de forma independiente.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Solid.Services;

/// <summary>
/// Validador exclusivo para la entidad Tag.
/// </summary>
public sealed class TagValidator
{
    // ──────────────────────────────────────────────────────────────────
    // Constantes de validación
    // ──────────────────────────────────────────────────────────────────
    private const int NameMaxLength = 50;

    /// <summary>
    /// Valida una etiqueta según las reglas de negocio.
    /// </summary>
    /// <param name="tag">La etiqueta a validar.</param>
    /// <param name="errors">Lista de errores encontrados (out).</param>
    /// <returns>true si es válida; false si hay errores.</returns>
    public bool Validate(Tag? tag, out List<string> errors)
    {
        errors = [];

        // ── Regla 1: La etiqueta no puede ser null ──
        if (tag is null)
        {
            errors.Add("La etiqueta no puede ser nula.");
            return false;
        }

        // ── Regla 2: Name requerido, no vacío, máximo 50 caracteres ──
        // Las etiquetas son palabras clave cortas (ej: "csharp", "dotnet")
        if (string.IsNullOrWhiteSpace(tag.Name))
        {
            errors.Add("El nombre de la etiqueta es obligatorio y no puede estar vacío.");
        }
        else if (tag.Name.Length > NameMaxLength)
        {
            errors.Add(
                $"El nombre de la etiqueta no puede superar los {NameMaxLength} caracteres. " +
                $"Longitud actual: {tag.Name.Length}.");
        }

        // ── Regla 3: Slug requerido y no vacío ──
        if (string.IsNullOrWhiteSpace(tag.Slug))
        {
            errors.Add("El slug de la etiqueta es obligatorio y no puede estar vacío.");
        }

        return errors.Count == 0;
    }
}
