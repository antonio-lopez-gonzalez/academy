// ──────────────────────────────────────────────────────────────────────────
// IUserRepository — Interfaz de repositorio para User (ISP + DIP)
// Módulo 3: SOLID — Lección 4 y 5
// ──────────────────────────────────────────────────────────────────────────
// Incluye búsquedas por campos únicos (Username, Email) porque son
// operaciones de consulta a datos, no lógica de negocio.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Solid.Repositories;

/// <summary>
/// Contrato de persistencia exclusivo para la entidad User.
/// </summary>
public interface IUserRepository
{
    /// <summary>Busca un usuario por su Id.</summary>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>Obtiene todos los usuarios ordenados por Username.</summary>
    Task<IReadOnlyList<User>> GetAllAsync();

    /// <summary>Añade un nuevo usuario.</summary>
    Task<User> AddAsync(User user);

    /// <summary>Actualiza un usuario existente. Devuelve null si no existe.</summary>
    Task<User?> UpdateAsync(User user);

    /// <summary>Elimina un usuario por Id.</summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>Busca un usuario por su nombre de usuario (case-insensitive).</summary>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>Busca un usuario por su email (case-insensitive).</summary>
    Task<User?> GetByEmailAsync(string email);
}
