// ===================================================================
// FakeApi — API web mínima para tests de integración.
//
// WebApplicationFactory<T> necesita una clase Program accesible.
// Gracias a InternalsVisibleTo, el proyecto de tests puede acceder
// a esta clase y arrancar la aplicación en memoria.
// ===================================================================

using System.Text.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

// GET /hello — Endpoint simple para test de integración básico
app.MapGet("/hello", () =>
    Results.Ok(new { message = "Hola desde FakeApi!" }));

// GET /hello/{name} — Endpoint con parámetro de ruta
app.MapGet("/hello/{name}", (string name) =>
    Results.Ok(new { message = $"Hola, {name}!" }));

// GET /users — Devuelve lista de usuarios simulados
app.MapGet("/users", () =>
{
    List<object> users =
    [
        new { id = 1, name = "Alicia", email = "alicia@email.com" },
        new { id = 2, name = "Bruno", email = "bruno@email.com" },
        new { id = 3, name = "Carlos", email = "carlos@email.com" }
    ];

    return Results.Ok(users);
});

// POST /echo — Devuelve el body recibido (útil para testear POST)
app.MapPost("/echo", async (HttpRequest request) =>
{
    using StreamReader reader = new(request.Body);
    string body = await reader.ReadToEndAsync();
    return Results.Ok(new { received = body });
});

// GET /error — Simula un error 500
app.MapGet("/error", () =>
{
    throw new InvalidOperationException("Error simulado para tests.");
});

app.Run();

// ⚠️ IMPORTANTE: Declaramos Program como public partial para que
// WebApplicationFactory<Program> pueda acceder desde el proyecto de tests.
// Alternativa: usar InternalsVisibleTo en el .csproj.
public partial class Program { }
