// ──────────────────────────────────────────────────────────────────────────
// TagService — Servicio de etiquetas refactorizado con SRP y DIP
// Módulo 3: SOLID — Todas las lecciones aplicadas
// ──────────────────────────────────────────────────────────────────────────
// Servicio de aplicación para etiquetas. Orquesta validación y
// persistencia. Las etiquetas son la entidad más simple del sistema.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;
using Cms.Services;
using Cms.Solid.Repositories;

namespace Cms.Solid.Services;

/// <summary>
/// Servicio de aplicación para etiquetas.
/// </summary>
public sealed class TagService : ITagService
{
    private readonly ITagRepository _repository;
    private readonly TagValidator _validator;

    /// <summary>
    /// Inyección de dependencias por constructor.
    /// </summary>
    public TagService(ITagRepository repository, TagValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Tag>> GetAllTagsAsync()
    {
        return _repository.GetAllAsync();
    }

    /// <inheritdoc />
    public Task<Tag?> GetTagByIdAsync(Guid id)
    {
        return _repository.GetByIdAsync(id);
    }

    /// <inheritdoc />
    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        // ── Validar ──
        if (!_validator.Validate(tag, out List<string> errors))
        {
            string errorMessage = string.Join(" ", errors);
            throw new ArgumentException(errorMessage, nameof(tag));
        }

        // ── Asignar Id si viene vacío ──
        if (tag.Id == Guid.Empty)
        {
            tag = tag with { Id = Guid.NewGuid() };
        }

        // ── Persistir ──
        return await _repository.AddAsync(tag);
    }

    /// <inheritdoc />
    public async Task<Tag?> UpdateTagAsync(Tag tag)
    {
        // ── Validar ──
        if (!_validator.Validate(tag, out List<string> errors))
        {
            string errorMessage = string.Join(" ", errors);
            throw new ArgumentException(errorMessage, nameof(tag));
        }

        // ── Persistir ──
        return await _repository.UpdateAsync(tag);
    }

    /// <inheritdoc />
    public Task<bool> DeleteTagAsync(Guid id)
    {
        return _repository.DeleteAsync(id);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Tag>> GetTagsByPostAsync(Guid postId)
    {
        // NOTA: Esta implementación en memoria no puede resolver la relación
        // N:N entre posts y tags sin acceso a los posts.
        // En M07 (EF Core) esto se resolverá con una consulta JOIN.
        return Task.FromResult<IReadOnlyList<Tag>>([]);
    }
}
