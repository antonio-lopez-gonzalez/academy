// ===================================================================
// UserServiceTests — Tests con Moq para simular dependencias
//
// Moq permite crear objetos "mock" que simulan interfaces.
// Podemos configurar qué devuelven (Setup) y verificar que
// se llamaron correctamente (Verify).
//
// ¿Por qué mockear?
//   - No queremos usar una BD real en tests unitarios (lentos, frágiles).
//   - No queremos enviar emails reales en tests.
//   - Queremos controlar exactamente qué devuelven las dependencias.
//   - Queremos verificar QUE se llamaron los métodos correctos.
// ===================================================================
using FluentAssertions;
using Moq;
using Xunit;

namespace TestXUnit;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        // Creamos mocks de las dependencias
        _userRepoMock = new Mock<IUserRepository>();
        _emailServiceMock = new Mock<IEmailService>();

        // Inyectamos los mocks en el servicio (los mocks implementan las interfaces)
        // Mock.Object devuelve una instancia que implementa la interfaz
        _userService = new UserService(_userRepoMock.Object, _emailServiceMock.Object);
    }

    // ─── SETUP: Configurar qué devuelve un mock ────────────────────────────

    [Fact(DisplayName = "RegisterAsync con username nuevo debe guardar y enviar email")]
    public async Task RegisterAsync_NewUser_SavesAndSendsEmail()
    {
        // Arrange: configuramos el mock para que devuelva false
        // (el usuario NO existe, por tanto se puede registrar)
        _userRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act: ejecutamos el método a testear
        User result = await _userService.RegisterAsync("nuevo", "nuevo@email.com");

        // Assert con FluentAssertions
        result.Username.Should().Be("nuevo");
        result.Email.Should().Be("nuevo@email.com");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        // ─── VERIFY: Verificar que los mocks se llamaron ──────────────────
        // Verificamos que SaveAsync se llamó UNA vez con cualquier User
        _userRepoMock.Verify(r => r.SaveAsync(It.IsAny<User>()), Times.Once);

        // Verificamos que SendWelcomeEmailAsync se llamó UNA vez con el email correcto
        _emailServiceMock.Verify(
            e => e.SendWelcomeEmailAsync("nuevo@email.com"),
            Times.Once);
    }

    // ─── Test de excepción con mock ────────────────────────────────────────

    [Fact(DisplayName = "RegisterAsync con username existente debe lanzar excepción")]
    public async Task RegisterAsync_ExistingUser_ThrowsException()
    {
        // Arrange: simulamos que el usuario YA existe
        _userRepoMock
            .Setup(r => r.ExistsAsync("existente"))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = () => _userService.RegisterAsync("existente", "test@email.com");

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("El usuario 'existente' ya existe.");

        // Verificar que NUNCA se llamó a SaveAsync (porque lanzó excepción antes)
        _userRepoMock.Verify(r => r.SaveAsync(It.IsAny<User>()), Times.Never);

        // Verificar que NUNCA se envió email
        _emailServiceMock.Verify(e => e.SendWelcomeEmailAsync(It.IsAny<string>()), Times.Never);
    }

    // ─── CALLBACK: Capturar argumentos pasados al mock ─────────────────────

    [Fact(DisplayName = "RegisterAsync debe guardar usuario con datos correctos (callback)")]
    public async Task RegisterAsync_ValidData_SavesCorrectUser()
    {
        // Arrange
        _userRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        User? capturedUser = null;

        // Callback: capturamos el argumento cuando se llame a SaveAsync
        _userRepoMock
            .Setup(r => r.SaveAsync(It.IsAny<User>()))
            .Callback<User>(user => capturedUser = user);

        // Act
        await _userService.RegisterAsync("testuser", "test@email.com");

        // Assert: verificamos los datos capturados
        capturedUser.Should().NotBeNull();
        capturedUser!.Username.Should().Be("testuser");
        capturedUser.Email.Should().Be("test@email.com");
    }

    // ─── Mock de excepciones ──────────────────────────────────────────────

    [Fact(DisplayName = "RegisterAsync cuando el repositorio falla debe propagar la excepción")]
    public async Task RegisterAsync_RepositoryThrows_PropagatesException()
    {
        // Arrange: configuramos el mock para que LANCE una excepción
        _userRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Error de conexión a BD"));

        // Act
        Func<Task> act = () => _userService.RegisterAsync("alguien", "alguien@email.com");

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Error de conexión a BD");
    }

    // ─── Setup condicional con It.Is<T> ────────────────────────────────────

    [Fact(DisplayName = "ExistsAsync debe devolver true solo para usuarios específicos")]
    public async Task RegisterAsync_SpecificUser_ChecksCorrectly()
    {
        // Arrange: configuramos respuestas diferentes según el argumento
        _userRepoMock
            .Setup(r => r.ExistsAsync("admin"))
            .ReturnsAsync(true);

        _userRepoMock
            .Setup(r => r.ExistsAsync(It.Is<string>(s => s != "admin")))
            .ReturnsAsync(false);

        // Act & Assert: admin ya existe → debe fallar
        Func<Task> actAdmin = () => _userService.RegisterAsync("admin", "admin@email.com");
        await actAdmin.Should().ThrowAsync<InvalidOperationException>();

        // Act & Assert: otro usuario → debe funcionar
        _emailServiceMock
            .Setup(e => e.SendWelcomeEmailAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        Func<Task> actOther = () => _userService.RegisterAsync("nuevo", "nuevo@email.com");
        await actOther.Should().NotThrowAsync();
    }
}
