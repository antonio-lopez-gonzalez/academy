// ===================================================================
// WebApiTests — Tests de integración con WebApplicationFactory
//
// WebApplicationFactory<T> arranca tu aplicación ASP.NET Core
// EN MEMORIA (sin puerto real, sin servidor Kestrel).
//
// Ventajas:
//   - Pruebas end-to-end realistas (middleware, routing, serialización).
//   - No necesita despliegue externo.
//   - Más rápido que tests contra un servidor real.
//   - Se puede configurar (reemplazar servicios, BD en memoria).
//
// Limitaciones:
//   - No prueba networking real (firewalls, DNS, balanceadores).
//   - No reemplaza tests de carga o smoke tests en producción.
//
// TestContainers (mencionado, no implementado):
//   - Para tests con BD real (PostgreSQL, Redis) en Docker.
//   - Se levanta un contenedor temporal antes de los tests.
//   - Más realista que BD en memoria (InMemoryDatabase de EF Core).
// ===================================================================
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TestIntegration;

public class WebApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    /// <summary>
    /// WebApplicationFactory<Program> se inyecta como class fixture.
    /// Crea un servidor en memoria con toda la pipeline de ASP.NET Core.
    /// IClassFixture garantiza que se comparte entre tests de esta clase.
    /// </summary>
    public WebApiTests(WebApplicationFactory<Program> factory)
    {
        // CreateClient() devuelve un HttpClient que habla con la app en memoria
        _client = factory.CreateClient();
    }

    // ─── GET: Verificar status code y contenido ──────────────────────────

    [Fact(DisplayName = "GET /hello debe devolver 200 OK con mensaje")]
    public async Task GetHello_ReturnsOk_WithMessage()
    {
        // Act: hacemos la petición HTTP contra la app en memoria
        HttpResponseMessage response = await _client.GetAsync("/hello");

        // Assert: verificamos el status code
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Leer y deserializar el contenido JSON
        string json = await response.Content.ReadAsStringAsync();
        JsonDocument doc = JsonDocument.Parse(json);

        // Verificar propiedades del JSON
        string? message = doc.RootElement.GetProperty("message").GetString();
        message.Should().Be("Hola desde FakeApi!");
    }

    // ─── GET con parámetro de ruta ─────────────────────────────────────────

    [Fact(DisplayName = "GET /hello/{name} debe devolver saludo personalizado")]
    public async Task GetHelloByName_ReturnsPersonalizedGreeting()
    {
        HttpResponseMessage response = await _client.GetAsync("/hello/Antonio");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonDocument doc = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        doc.RootElement.GetProperty("message").GetString()
            .Should().Be("Hola, Antonio!");
    }

    // ─── GET: Verificar lista de objetos ──────────────────────────────────

    [Fact(DisplayName = "GET /users debe devolver lista de usuarios")]
    public async Task GetUsers_ReturnsListOfUsers()
    {
        HttpResponseMessage response = await _client.GetAsync("/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonDocument doc = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        // Verificar que devuelve un array de 3 elementos
        doc.RootElement.GetArrayLength().Should().Be(3);

        // Verificar el primer usuario
        JsonElement firstUser = doc.RootElement[0];
        firstUser.GetProperty("id").GetInt32().Should().Be(1);
        firstUser.GetProperty("name").GetString().Should().Be("Alicia");
    }

    // ─── POST: Enviar datos y verificar respuesta ─────────────────────────

    [Fact(DisplayName = "POST /echo debe devolver el body recibido")]
    public async Task PostEcho_ReturnsReceivedBody()
    {
        // Arrange: creamos contenido JSON para enviar
        string requestBody = """{"nombre":"Antonio","edad":30}""";
        StringContent content = new(requestBody, Encoding.UTF8, "application/json");

        // Act: POST con body
        HttpResponseMessage response = await _client.PostAsync("/echo", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonDocument doc = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        doc.RootElement.GetProperty("received").GetString()
            .Should().Be(requestBody);
    }

    // ─── Content-Type header ──────────────────────────────────────────────

    [Fact(DisplayName = "GET /hello debe devolver Content-Type application/json")]
    public async Task GetHello_ReturnsJsonContentType()
    {
        HttpResponseMessage response = await _client.GetAsync("/hello");

        response.Content.Headers.ContentType!.MediaType
            .Should().Be("application/json");
    }

    // ─── Error 500 ────────────────────────────────────────────────────────

    [Fact(DisplayName = "GET /error debe devolver 500 Internal Server Error")]
    public async Task GetError_ReturnsInternalServerError()
    {
        HttpResponseMessage response = await _client.GetAsync("/error");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    // ─── 404: Ruta no encontrada ──────────────────────────────────────────

    [Fact(DisplayName = "GET a ruta inexistente debe devolver 404")]
    public async Task GetNonExistentRoute_ReturnsNotFound()
    {
        HttpResponseMessage response = await _client.GetAsync("/ruta-que-no-existe");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
