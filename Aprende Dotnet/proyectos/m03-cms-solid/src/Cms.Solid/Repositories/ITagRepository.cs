// ──────────────────────────────────────────────────────────────────────────
// ITagRepository — Interfaz de repositorio para Tag (ISP + DIP)
// Módulo 3: SOLID — Lección 4 y 5
// ──────────────────────────────────────────────────────────────────────────
// Interfaz pequeña y cohesiva. Solo persistencia de tags.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Solid.Repositories;

/// <summary>
/// Contrato de persistencia exclusivo para la entidad Tag.
/// </summary>
public interface ITagRepository
{
    /// <summary>Busca una etiqueta por su Id.</summary>
    Task<Tag?> GetByIdAsync(Guid id);

    /// <summary>Obtiene todas las etiquetas ordenadas alfabéticamente.</summary>
    Task<IReadOnlyList<Tag>> GetAllAsync();

    /// <summary>Añade una nueva etiqueta.</summary>
    Task<Tag> AddAsync(Tag tag);

    /// <summary>Actualiza una etiqueta existente. Devuelve null si no existe.</summary>
    Task<Tag?> UpdateAsync(Tag tag);

    /// <summary>Elimina una etiqueta por Id.</summary>
    Task<bool> DeleteAsync(Guid id);
}
