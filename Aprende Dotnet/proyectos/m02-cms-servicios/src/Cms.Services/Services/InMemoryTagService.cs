// ──────────────────────────────────────────────────────────────────────────
// InMemoryTagService — Implementación en memoria de ITagService
// Módulo 2: POO en C# — Implementación concreta de la interfaz
// ──────────────────────────────────────────────────────────────────────────
// Las etiquetas (tags) son planas, sin jerarquía. Se relacionan con los
// posts mediante la lista TagIds en la entidad Post (relación N:N).
//
// A diferencia de las categorías, las etiquetas no tienen orden de
// visualización ni jerarquía: son simplemente palabras clave.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Services;

public sealed class InMemoryTagService : ITagService
{
    // ──────────────────────────────────────────────────────────────────
    // Almacenamiento interno
    // ──────────────────────────────────────────────────────────────────
    private readonly List<Tag> _tags = [];

    // ──────────────────────────────────────────────────────────────────
    // CRUD básico
    // ──────────────────────────────────────────────────────────────────

    /// <inheritdoc />
    public Task<IReadOnlyList<Tag>> GetAllTagsAsync()
    {
        // Ordenadas alfabéticamente por nombre
        List<Tag> ordered = _tags
            .OrderBy(t => t.Name)
            .ToList();

        return Task.FromResult<IReadOnlyList<Tag>>(ordered);
    }

    /// <inheritdoc />
    public Task<Tag?> GetTagByIdAsync(Guid id)
    {
        Tag? tag = _tags.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(tag);
    }

    /// <inheritdoc />
    public Task<Tag> CreateTagAsync(Tag tag)
    {
        // ── Validaciones básicas ──
        if (tag is null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (string.IsNullOrWhiteSpace(tag.Name))
        {
            throw new ArgumentException("El nombre de la etiqueta no puede estar vacío.", nameof(tag));
        }

        if (string.IsNullOrWhiteSpace(tag.Slug))
        {
            throw new ArgumentException("El slug de la etiqueta no puede estar vacío.", nameof(tag));
        }

        // ── Asignar Id si viene vacío ──
        if (tag.Id == Guid.Empty)
        {
            tag = tag with { Id = Guid.NewGuid() };
        }

        // ── Almacenar ──
        _tags.Add(tag);

        return Task.FromResult(tag);
    }

    /// <inheritdoc />
    public Task<Tag?> UpdateTagAsync(Tag tag)
    {
        if (tag is null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        int index = _tags.FindIndex(t => t.Id == tag.Id);

        if (index == -1)
        {
            return Task.FromResult<Tag?>(null);
        }

        _tags[index] = tag;

        return Task.FromResult<Tag?>(tag);
    }

    /// <inheritdoc />
    public Task<bool> DeleteTagAsync(Guid id)
    {
        int removed = _tags.RemoveAll(t => t.Id == id);
        return Task.FromResult(removed > 0);
    }

    // ──────────────────────────────────────────────────────────────────
    // Consultas por post
    // ──────────────────────────────────────────────────────────────────

    /// <inheritdoc />
    public Task<IReadOnlyList<Tag>> GetTagsByPostAsync(Guid postId)
    {
        // NOTA: Esta implementación en memoria no tiene acceso a los posts.
        // Para resolver correctamente esta consulta necesitaríamos una referencia
        // a IPostService o a la lista de posts.
        //
        // En una implementación real con EF Core, esto se resolvería mediante
        // una consulta JOIN entre Posts y Tags.
        //
        // Por ahora, devolvemos una lista vacía ya que no podemos hacer
        // el join sin acceso a los datos de posts.
        // En el módulo 7 (EF Core) esto se implementará correctamente.

        return Task.FromResult<IReadOnlyList<Tag>>([]);
    }
}
