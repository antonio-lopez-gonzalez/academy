// ──────────────────────────────────────────────────────────────────────────
// IPostRepository — Interfaz de repositorio para Post (ISP + DIP)
// Módulo 3: SOLID — Lección 4 (ISP) y Lección 5 (DIP)
// ──────────────────────────────────────────────────────────────────────────
// ISP (Interface Segregation): Esta interfaz contiene SOLO métodos
// relacionados con la persistencia de Post. No incluye validación,
// notificaciones, ni lógica de negocio. Quien implemente esta interfaz
// no se ve forzado a implementar métodos que no necesita.
//
// DIP (Dependency Inversion): Tanto el servicio de alto nivel
// (PostService) como el repositorio de bajo nivel (InMemoryPostRepository)
// dependen de esta ABSTRACCIÓN, no de implementaciones concretas.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Solid.Repositories;

/// <summary>
/// Contrato de persistencia exclusivo para la entidad Post.
/// Solo métodos de acceso a datos. Sin lógica de negocio.
/// </summary>
public interface IPostRepository
{
    /// <summary>Busca un post por su identificador único.</summary>
    Task<Post?> GetByIdAsync(Guid id);

    /// <summary>Obtiene todos los posts ordenados por fecha de creación descendente.</summary>
    Task<IReadOnlyList<Post>> GetAllAsync();

    /// <summary>Añade un nuevo post al almacén de datos.</summary>
    Task<Post> AddAsync(Post post);

    /// <summary>Actualiza un post existente. Devuelve null si no existe.</summary>
    Task<Post?> UpdateAsync(Post post);

    /// <summary>Elimina un post por Id. Devuelve true si se eliminó.</summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>Obtiene los posts que pertenecen a una categoría.</summary>
    Task<IReadOnlyList<Post>> GetByCategoryAsync(Guid categoryId);

    /// <summary>Obtiene los posts creados por un autor específico.</summary>
    Task<IReadOnlyList<Post>> GetByAuthorAsync(Guid authorId);
}
