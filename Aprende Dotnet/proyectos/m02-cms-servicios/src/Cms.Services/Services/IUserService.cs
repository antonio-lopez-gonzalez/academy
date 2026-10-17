// ──────────────────────────────────────────────────────────────────────────
// IUserService — Interfaz para la gestión de usuarios del CMS
// Módulo 2: POO en C# — Interfaces y abstracciones
// ──────────────────────────────────────────────────────────────────────────
// Gestiona el ciclo de vida completo de los usuarios: creación, consulta,
// actualización y eliminación. Incluye búsquedas por username y email
// porque son campos únicos usados para autenticación.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Services;

public interface IUserService
{
    /// <summary>
    /// Obtiene todos los usuarios del sistema, ordenados por nombre de usuario.
    /// </summary>
    Task<IReadOnlyList<User>> GetAllUsersAsync();

    /// <summary>
    /// Busca un usuario por su identificador único.
    /// Devuelve null si no se encuentra.
    /// </summary>
    Task<User?> GetUserByIdAsync(Guid id);

    /// <summary>
    /// Busca un usuario por su nombre de usuario (username).
    /// El username es único en el sistema.
    /// Devuelve null si no se encuentra.
    /// </summary>
    Task<User?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Busca un usuario por su dirección de correo electrónico.
    /// El email es único en el sistema.
    /// Devuelve null si no se encuentra.
    /// </summary>
    Task<User?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Crea un nuevo usuario en el sistema.
    /// Si el Id está vacío, se asigna uno nuevo automáticamente.
    /// Lanza ArgumentNullException si el usuario es null.
    /// Lanza ArgumentException si Username o Email están vacíos.
    /// </summary>
    Task<User> CreateUserAsync(User user);

    /// <summary>
    /// Actualiza un usuario existente.
    /// Devuelve el usuario actualizado, o null si no se encontró.
    /// </summary>
    Task<User?> UpdateUserAsync(User user);

    /// <summary>
    /// Elimina un usuario por su identificador.
    /// Devuelve true si se eliminó, false si no existía.
    /// </summary>
    Task<bool> DeleteUserAsync(Guid id);
}
