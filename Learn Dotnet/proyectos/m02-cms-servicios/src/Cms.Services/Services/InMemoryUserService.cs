// ──────────────────────────────────────────────────────────────────────────
// InMemoryUserService — Implementación en memoria de IUserService
// Módulo 2: POO en C# — Implementación concreta de la interfaz
// ──────────────────────────────────────────────────────────────────────────
// Gestiona los usuarios del CMS. Incluye búsquedas por Username y Email
// (campos únicos usados para login y registro).
//
// NOTA: Esta implementación no hashea contraseñas. En el módulo 11
// (Autenticación) añadiremos BCrypt para el hashing seguro.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Services;

public sealed class InMemoryUserService : IUserService
{
    // ──────────────────────────────────────────────────────────────────
    // Almacenamiento interno
    // ──────────────────────────────────────────────────────────────────
    private readonly List<User> _users = [];

    // ──────────────────────────────────────────────────────────────────
    // CRUD básico
    // ──────────────────────────────────────────────────────────────────

    /// <inheritdoc />
    public Task<IReadOnlyList<User>> GetAllUsersAsync()
    {
        // Ordenados por nombre de usuario alfabéticamente
        List<User> ordered = _users
            .OrderBy(u => u.Username)
            .ToList();

        return Task.FromResult<IReadOnlyList<User>>(ordered);
    }

    /// <inheritdoc />
    public Task<User?> GetUserByIdAsync(Guid id)
    {
        User? user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    /// <inheritdoc />
    public Task<User?> GetUserByUsernameAsync(string username)
    {
        // Búsqueda case-insensitive por username
        // En un sistema real con BD, esto usaría un índice único
        User? user = _users.FirstOrDefault(
            u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    /// <inheritdoc />
    public Task<User?> GetUserByEmailAsync(string email)
    {
        // Búsqueda case-insensitive por email
        // En un sistema real con BD, esto usaría un índice único
        User? user = _users.FirstOrDefault(
            u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    /// <inheritdoc />
    public Task<User> CreateUserAsync(User user)
    {
        // ── Validaciones básicas ──
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (string.IsNullOrWhiteSpace(user.Username))
        {
            throw new ArgumentException("El nombre de usuario no puede estar vacío.", nameof(user));
        }

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new ArgumentException("El email no puede estar vacío.", nameof(user));
        }

        if (string.IsNullOrWhiteSpace(user.DisplayName))
        {
            throw new ArgumentException("El nombre público (DisplayName) no puede estar vacío.", nameof(user));
        }

        // ── Validar unicidad de Username ──
        bool usernameExists = _users.Any(
            u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));

        if (usernameExists)
        {
            throw new ArgumentException(
                $"Ya existe un usuario con el nombre de usuario '{user.Username}'.",
                nameof(user));
        }

        // ── Validar unicidad de Email ──
        bool emailExists = _users.Any(
            u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));

        if (emailExists)
        {
            throw new ArgumentException(
                $"Ya existe un usuario con el email '{user.Email}'.",
                nameof(user));
        }

        // ── Asignar Id si viene vacío ──
        if (user.Id == Guid.Empty)
        {
            user = user with { Id = Guid.NewGuid() };
        }

        // ── Asignar fecha de creación si no viene establecida ──
        if (user.CreatedAt == default)
        {
            user = user with { CreatedAt = DateTime.UtcNow };
        }

        // ── Almacenar ──
        _users.Add(user);

        return Task.FromResult(user);
    }

    /// <inheritdoc />
    public Task<User?> UpdateUserAsync(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        int index = _users.FindIndex(u => u.Id == user.Id);

        if (index == -1)
        {
            return Task.FromResult<User?>(null);
        }

        // ── Validar unicidad de Username (excluyendo el propio usuario) ──
        bool usernameExists = _users
            .Where(u => u.Id != user.Id)
            .Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));

        if (usernameExists)
        {
            throw new ArgumentException(
                $"Ya existe otro usuario con el nombre de usuario '{user.Username}'.",
                nameof(user));
        }

        // ── Validar unicidad de Email (excluyendo el propio usuario) ──
        bool emailExists = _users
            .Where(u => u.Id != user.Id)
            .Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));

        if (emailExists)
        {
            throw new ArgumentException(
                $"Ya existe otro usuario con el email '{user.Email}'.",
                nameof(user));
        }

        // ── Reemplazar el usuario en la lista ──
        _users[index] = user;

        return Task.FromResult<User?>(user);
    }

    /// <inheritdoc />
    public Task<bool> DeleteUserAsync(Guid id)
    {
        int removed = _users.RemoveAll(u => u.Id == id);
        return Task.FromResult(removed > 0);
    }
}
