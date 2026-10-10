// ──────────────────────────────────────────────────────────────────────────
// UserService — Servicio de usuarios refactorizado con SRP y DIP
// Módulo 3: SOLID — Todas las lecciones aplicadas
// ──────────────────────────────────────────────────────────────────────────
// SRP: UserService solo orquesta. La validación está en UserValidator,
//      la persistencia en IUserRepository.
//
// DIP: UserService depende de IUserRepository (abstracción), no de una
//      implementación concreta. Se puede cambiar el repositorio sin
//      tocar el servicio.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;
using Cms.Services;
using Cms.Solid.Repositories;

namespace Cms.Solid.Services;

/// <summary>
/// Servicio de aplicación para usuarios.
/// Orquesta validación y persistencia.
/// </summary>
public sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly UserValidator _validator;

    /// <summary>
    /// Inyección de dependencias por constructor.
    /// </summary>
    public UserService(IUserRepository repository, UserValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<User>> GetAllUsersAsync()
    {
        return _repository.GetAllAsync();
    }

    /// <inheritdoc />
    public Task<User?> GetUserByIdAsync(Guid id)
    {
        return _repository.GetByIdAsync(id);
    }

    /// <inheritdoc />
    public Task<User?> GetUserByUsernameAsync(string username)
    {
        return _repository.GetByUsernameAsync(username);
    }

    /// <inheritdoc />
    public Task<User?> GetUserByEmailAsync(string email)
    {
        return _repository.GetByEmailAsync(email);
    }

    /// <inheritdoc />
    public async Task<User> CreateUserAsync(User user)
    {
        // ── Validar con el validador dedicado (SRP) ──
        if (!_validator.Validate(user, out List<string> errors))
        {
            string errorMessage = string.Join(" ", errors);
            throw new ArgumentException(errorMessage, nameof(user));
        }

        // ── Asignar Id si viene vacío ──
        if (user.Id == Guid.Empty)
        {
            user = user with { Id = Guid.NewGuid() };
        }

        // ── Asignar fecha de creación ──
        if (user.CreatedAt == default)
        {
            user = user with { CreatedAt = DateTime.UtcNow };
        }

        // ── Persistir ──
        return await _repository.AddAsync(user);
    }

    /// <inheritdoc />
    public async Task<User?> UpdateUserAsync(User user)
    {
        // ── Validar ──
        if (!_validator.Validate(user, out List<string> errors))
        {
            string errorMessage = string.Join(" ", errors);
            throw new ArgumentException(errorMessage, nameof(user));
        }

        // ── Persistir ──
        return await _repository.UpdateAsync(user);
    }

    /// <inheritdoc />
    public Task<bool> DeleteUserAsync(Guid id)
    {
        return _repository.DeleteAsync(id);
    }
}
