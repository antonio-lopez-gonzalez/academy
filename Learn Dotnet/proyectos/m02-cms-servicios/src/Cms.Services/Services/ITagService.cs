// ──────────────────────────────────────────────────────────────────────────
// ITagService — Interfaz para la gestión de etiquetas del CMS
// Módulo 2: POO en C# — Interfaces y abstracciones
// ──────────────────────────────────────────────────────────────────────────
// Las etiquetas son planas (sin jerarquía) a diferencia de las categorías.
// Se usan para clasificación transversal: un post puede tener múltiples tags
// y un tag puede estar en múltiples posts (relación N:N).
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Services;

public interface ITagService
{
    /// <summary>
    /// Obtiene todas las etiquetas del sistema, ordenadas por nombre.
    /// </summary>
    Task<IReadOnlyList<Tag>> GetAllTagsAsync();

    /// <summary>
    /// Busca una etiqueta por su identificador único.
    /// Devuelve null si no se encuentra.
    /// </summary>
    Task<Tag?> GetTagByIdAsync(Guid id);

    /// <summary>
    /// Crea una nueva etiqueta.
    /// Si el Id está vacío, se asigna uno nuevo automáticamente.
    /// Lanza ArgumentNullException si la etiqueta es null.
    /// Lanza ArgumentException si el nombre está vacío.
    /// </summary>
    Task<Tag> CreateTagAsync(Tag tag);

    /// <summary>
    /// Actualiza una etiqueta existente.
    /// Devuelve la etiqueta actualizada, o null si no se encontró.
    /// </summary>
    Task<Tag?> UpdateTagAsync(Tag tag);

    /// <summary>
    /// Elimina una etiqueta por su identificador.
    /// Devuelve true si se eliminó, false si no existía.
    /// </summary>
    Task<bool> DeleteTagAsync(Guid id);

    /// <summary>
    /// Obtiene todas las etiquetas asociadas a un post específico.
    /// Busca en los TagIds del post y devuelve las etiquetas correspondientes.
    /// </summary>
    Task<IReadOnlyList<Tag>> GetTagsByPostAsync(Guid postId);
}
