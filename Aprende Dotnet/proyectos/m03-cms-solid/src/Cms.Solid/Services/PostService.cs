// ──────────────────────────────────────────────────────────────────────────
// PostService — Servicio de posts refactorizado con SRP y DIP
// Módulo 3: SOLID — Todas las lecciones aplicadas
// ──────────────────────────────────────────────────────────────────────────
// SRP: PostService ahora solo ORQUESTA. No valida (responsabilidad del
//      PostValidator) ni persiste directamente (responsabilidad del
//      IPostRepository).
//
// DIP: PostService depende de las abstracciones IPostRepository y
//      PostValidator, NO de implementaciones concretas. Si mañana
//      cambiamos a PostgreSQL, esta clase no se modifica.
//
// COMPARACIÓN con M02:
//   Antes: InMemoryPostService tenía acopladas validación + persistencia
//          en una sola clase. Imposible testear la validación sin la BD.
//   Ahora: Cada responsabilidad está en su propia clase. Se pueden
//          testear por separado (mock del repo, validador aislado).
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;
using Cms.Services;
using Cms.Solid.Repositories;

namespace Cms.Solid.Services;

/// <summary>
/// Servicio de aplicación para posts.
/// Orquesta validación y persistencia usando DIP.
/// </summary>
public sealed class PostService : IPostService
{
    // ── Dependencias inyectadas por constructor (DIP) ──
    // Dependemos de abstracciones (interfaces), no de implementaciones.
    private readonly IPostRepository _repository;
    private readonly PostValidator _validator;

    /// <summary>
    /// Constructor que recibe las dependencias por inyección.
    /// Esto es Poor Man's DI: las dependencias se pasan explícitamente.
    /// En el Módulo 5 usaremos el contenedor de DI de .NET.
    /// </summary>
    public PostService(IPostRepository repository, PostValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Post>> GetAllPostsAsync()
    {
        // Delegación pura al repositorio. Sin lógica adicional.
        return _repository.GetAllAsync();
    }

    /// <inheritdoc />
    public Task<Post?> GetPostByIdAsync(Guid id)
    {
        return _repository.GetByIdAsync(id);
    }

    /// <inheritdoc />
    public async Task<Post> CreatePostAsync(Post post)
    {
        // ── Paso 1: Validar (responsabilidad del validador) ──
        if (!_validator.Validate(post, out List<string> errors))
        {
            // Unimos todos los errores en un solo mensaje para la excepción
            string errorMessage = string.Join(" ", errors);
            throw new ArgumentException(errorMessage, nameof(post));
        }

        // ── Paso 2: Preparar datos (responsabilidad del servicio) ──
        // Asignar Id si viene vacío
        if (post.Id == Guid.Empty)
        {
            post = post with { Id = Guid.NewGuid() };
        }

        // Asignar fechas si no vienen establecidas
        DateTime now = DateTime.UtcNow;
        post = post with
        {
            CreatedAt = post.CreatedAt == default ? now : post.CreatedAt,
            UpdatedAt = now
        };

        // ── Paso 3: Persistir (responsabilidad del repositorio) ──
        return await _repository.AddAsync(post);
    }

    /// <inheritdoc />
    public async Task<Post?> UpdatePostAsync(Post post)
    {
        // ── Validar antes de actualizar ──
        if (!_validator.Validate(post, out List<string> errors))
        {
            string errorMessage = string.Join(" ", errors);
            throw new ArgumentException(errorMessage, nameof(post));
        }

        // ── Actualizar timestamp ──
        post = post with { UpdatedAt = DateTime.UtcNow };

        // ── Persistir ──
        return await _repository.UpdateAsync(post);
    }

    /// <inheritdoc />
    public Task<bool> DeletePostAsync(Guid id)
    {
        return _repository.DeleteAsync(id);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Post>> GetPostsByCategoryAsync(Guid categoryId)
    {
        return _repository.GetByCategoryAsync(categoryId);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Post>> GetPostsByTagAsync(Guid tagId)
    {
        // NOTA: En esta implementación, la búsqueda por tag no está en el
        // repositorio porque el repositorio de Post no conoce los Tags.
        // Obtenemos todos los posts y filtramos en memoria.
        // En M07 (EF Core) esto será una consulta JOIN eficiente.
        return Task.FromResult<IReadOnlyList<Post>>(
            _repository.GetAllAsync().Result
                .Where(p => p.TagIds.Contains(tagId))
                .OrderByDescending(p => p.CreatedAt)
                .ToList());
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Post>> GetPostsByAuthorAsync(Guid authorId)
    {
        return _repository.GetByAuthorAsync(authorId);
    }
}
