// ──────────────────────────────────────────────────────────────────────────
// IPostService — Interfaz para la gestión de posts del CMS
// Módulo 2: POO en C# — Interfaces y abstracciones
// ──────────────────────────────────────────────────────────────────────────
// Define el contrato que cualquier implementación de servicio de posts
// debe cumplir. Esto permite cambiar la implementación (memoria, BD, API
// externa) sin modificar el código que consume esta interfaz.
//
// Principio aplicado: Programar contra interfaces, no contra implementaciones.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Services;

public interface IPostService
{
    /// <summary>
    /// Obtiene todos los posts del sistema, ordenados por fecha de creación descendente.
    /// </summary>
    Task<IReadOnlyList<Post>> GetAllPostsAsync();

    /// <summary>
    /// Busca un post por su identificador único.
    /// Devuelve null si no se encuentra.
    /// </summary>
    Task<Post?> GetPostByIdAsync(Guid id);

    /// <summary>
    /// Crea un nuevo post en el sistema.
    /// Si el Id está vacío, se asigna uno nuevo automáticamente.
    /// Lanza ArgumentNullException si el post es null.
    /// Lanza ArgumentException si el título está vacío.
    /// </summary>
    Task<Post> CreatePostAsync(Post post);

    /// <summary>
    /// Actualiza un post existente.
    /// Devuelve el post actualizado, o null si no se encontró.
    /// La fecha UpdatedAt se actualiza automáticamente.
    /// </summary>
    Task<Post?> UpdatePostAsync(Post post);

    /// <summary>
    /// Elimina un post por su identificador.
    /// Devuelve true si se eliminó correctamente, false si no existía.
    /// </summary>
    Task<bool> DeletePostAsync(Guid id);

    /// <summary>
    /// Obtiene todos los posts que pertenecen a una categoría específica.
    /// </summary>
    Task<IReadOnlyList<Post>> GetPostsByCategoryAsync(Guid categoryId);

    /// <summary>
    /// Obtiene todos los posts que tienen una etiqueta específica.
    /// </summary>
    Task<IReadOnlyList<Post>> GetPostsByTagAsync(Guid tagId);

    /// <summary>
    /// Obtiene todos los posts creados por un autor específico.
    /// </summary>
    Task<IReadOnlyList<Post>> GetPostsByAuthorAsync(Guid authorId);
}
