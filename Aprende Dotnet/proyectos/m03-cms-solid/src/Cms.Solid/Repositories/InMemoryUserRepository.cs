// ──────────────────────────────────────────────────────────────────────────
// InMemoryUserRepository — Implementación en memoria de IUserRepository
// Módulo 3: SOLID — Lección 1: Single Responsibility Principle
// ──────────────────────────────────────────────────────────────────────────
// Responsabilidad única: acceso a datos (CRUD) para User.
// NO valida, NO orquesta, NO envía notificaciones. Solo persiste.
// Incluye búsquedas por Username y Email porque son consultas de datos,
// no lógica de negocio.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Solid.Repositories;

/// <summary>
/// Repositorio de usuarios en memoria.
/// Responsabilidad única: acceso a datos (CRUD).
/// </summary>
public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = [];

    /// <inheritdoc />
    public Task<User?> GetByIdAsync(Guid id)
    {
        User? user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<User>> GetAllAsync()
    {
        List<User> ordered = _users
            .OrderBy(u => u.Username)
            .ToList();

        return Task.FromResult<IReadOnlyList<User>>(ordered);
    }

    /// <inheritdoc />
    public Task<User> AddAsync(User user)
    {
        _users.Add(user);
        return Task.FromResult(user);
    }

    /// <inheritdoc />
    public Task<User?> UpdateAsync(User user)
    {
        int index = _users.FindIndex(u => u.Id == user.Id);

        if (index == -1)
        {
            return Task.FromResult<User?>(null);
        }

        _users[index] = user;
        return Task.FromResult<User?>(user);
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id)
    {
        int removed = _users.RemoveAll(u => u.Id == id);
        return Task.FromResult(removed > 0);
    }

    /// <inheritdoc />
    public Task<User?> GetByUsernameAsync(string username)
    {
        User? user = _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    /// <inheritdoc />
    public Task<User?> GetByEmailAsync(string email)
    {
        User? user = _users.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }
}
