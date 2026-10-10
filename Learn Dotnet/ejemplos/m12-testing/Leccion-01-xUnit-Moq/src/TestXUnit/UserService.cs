// ===================================================================
// Interfaces y clases de servicio para demostrar testing con Moq.
//
// UserService depende de dos interfaces:
//   - IUserRepository: acceso a datos (simulado con Moq)
//   - IEmailService: envío de emails (simulado con Moq)
//
// Al testear UserService, mockeamos sus dependencias para
// aislar la lógica de negocio y verificar el comportamiento.
// ===================================================================
namespace TestXUnit;

/// <summary>
/// Repositorio de usuarios (capa de datos).
/// En tests, se simula con Moq para no usar una BD real.
/// </summary>
public interface IUserRepository
{
    Task<bool> ExistsAsync(string username);
    Task SaveAsync(User user);
}

/// <summary>
/// Servicio de envío de emails.
/// En tests, se simula con Moq para no enviar emails reales.
/// </summary>
public interface IEmailService
{
    Task SendWelcomeEmailAsync(string email);
}

/// <summary>
/// Entidad de usuario simple.
/// </summary>
public class User
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Servicio de registro de usuarios.
/// Contiene la lógica de negocio que queremos testear.
/// Depende de IUserRepository e IEmailService (inyectadas por constructor).
/// </summary>
/// <param name="userRepository">Acceso a datos de usuarios.</param>
/// <param name="emailService">Servicio de envío de emails.</param>
public class UserService(IUserRepository userRepository, IEmailService emailService)
{
    /// <summary>
    /// Registra un nuevo usuario.
    /// - Valida que el username no exista.
    /// - Guarda en repositorio.
    /// - Envía email de bienvenida.
    /// </summary>
    /// <returns>El usuario registrado.</returns>
    /// <exception cref="InvalidOperationException">Si el username ya existe.</exception>
    public async Task<User> RegisterAsync(string username, string email)
    {
        // Validar si el usuario ya existe
        bool exists = await userRepository.ExistsAsync(username);
        if (exists)
        {
            throw new InvalidOperationException($"El usuario '{username}' ya existe.");
        }

        // Crear el usuario
        User user = new()
        {
            Username = username,
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        // Guardar en repositorio
        await userRepository.SaveAsync(user);

        // Enviar email de bienvenida
        await emailService.SendWelcomeEmailAsync(email);

        return user;
    }
}
