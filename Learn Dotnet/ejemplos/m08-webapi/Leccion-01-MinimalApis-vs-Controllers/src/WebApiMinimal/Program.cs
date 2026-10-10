using WebApiMinimal.Models; // Acceso al modelo Post (archivo separado)

// ─────────────────────────────────────────────────────────────
// Lección 01 — Minimal APIs vs Controllers
// Comparativa completa entre los dos enfoques para crear APIs en ASP.NET Core 8.
//
// IMPORTANTE: app.MapControllers() debe ir DESPUÉS de los
// endpoints Minimal API para evitar conflictos de resolución
// de delegados. Usamos Results.* (IResult) en vez de
// TypedResults.* para máxima compatibilidad de compilación.
// ─────────────────────────────────────────────────────────────

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ── REGISTRO DE SERVICIOS ────────────────────────────────────
// AddControllers registra todos los controladores en el contenedor de DI.
// Escanea el assembly en busca de clases con [ApiController].
builder.Services.AddControllers();

WebApplication app = builder.Build();
app.Urls.Add("http://localhost:5000");

// ═══════════════════════════════════════════════════════════════
// PARTE 1: MINIMAL APIs
// ═══════════════════════════════════════════════════════════════

// ── 1.1 Endpoint mínimo ──────────────────────────────────────
// El endpoint más simple posible. Sin controlador, solo una lambda.
app.MapGet("/hello", () => "Hello World from Minimal API!");

// ── 1.2 Datos en memoria (simula una base de datos) ──────────
List<Post> posts = new()
{
    new Post(1, "Primer post", "Contenido del primer post", DateTime.UtcNow),
    new Post(2, "Segundo post", "Contenido del segundo post", DateTime.UtcNow),
    new Post(3, "C# 12 en profundidad", "Aprendiendo Minimal APIs", DateTime.UtcNow.AddDays(-1))
};

// ── 1.3 Endpoints CRUD con Minimal APIs ─────────────────────
// Binding automático: id de la ruta {id}, Post del body JSON.
// Results.* devuelve IResult (interface simple, máxima compatibilidad).
// TypedResults.* devuelve tipos concretos (mejor rendimiento AOT pero
// más restrictivo con el compilador).

// GET /api/v1/posts — Obtener todos los posts
app.MapGet("/api/v1/posts", () => Results.Ok(posts));

// GET /api/v1/posts/{id} — Obtener un post por ID
app.MapGet("/api/v1/posts/{id:int}", (int id) =>
{
    Post? post = posts.Find(p => p.Id == id);
    return post is not null
        ? Results.Ok(post)
        : Results.NotFound(new { Message = $"Post con ID {id} no encontrado" });
});

// POST /api/v1/posts — Crear un nuevo post
app.MapPost("/api/v1/posts", (Post newPost) =>
{
    int newId = posts.Count > 0 ? posts.Max(p => p.Id) + 1 : 1;
    Post post = newPost with { Id = newId, CreatedAt = DateTime.UtcNow };
    posts.Add(post);
    return Results.Created($"/api/v1/posts/{post.Id}", post);
});

// PUT /api/v1/posts/{id} — Actualizar un post existente
app.MapPut("/api/v1/posts/{id:int}", (int id, Post updatedPost) =>
{
    int index = posts.FindIndex(p => p.Id == id);
    if (index == -1)
        return Results.NotFound(new { Message = $"Post con ID {id} no encontrado" });

    posts[index] = updatedPost with { Id = id, CreatedAt = posts[index].CreatedAt };
    return Results.Ok(posts[index]);
});

// DELETE /api/v1/posts/{id} — Eliminar un post
app.MapDelete("/api/v1/posts/{id:int}", (int id) =>
{
    int removed = posts.RemoveAll(p => p.Id == id);
    return removed > 0
        ? Results.NoContent() // 204: operación exitosa, sin cuerpo
        : Results.NotFound(new { Message = $"Post con ID {id} no encontrado" });
});

// ═══════════════════════════════════════════════════════════════
// PARTE 2: CONTROLLERS (ControllerBase)
// ═══════════════════════════════════════════════════════════════
// El controlador está en Controllers/PostsController.cs
// Se accede en: /api/v1/controller-posts
//
// DIFERENCIAS CLAVE:
// ┌──────────────────────┬─────────────────────────┬──────────────────────────┐
// │ Característica       │ Minimal APIs            │ Controllers              │
// ├──────────────────────┼─────────────────────────┼──────────────────────────┤
// │ Sintaxis             │ Lambdas inline          │ Clases + atributos       │
// │ AOT (Native AOT)     │ ✅ 100% compatible      │ ⚠️ Limitado              │
// │ Filtros              │ IEndpointFilter         │ IActionFilter, etc.      │
// │ Model Binding        │ Automático por parámetro│ [FromQuery], etc.        │
// │ Organización         │ MapGroup / extensiones  │ Carpetas por feature     │
// │ Rendimiento          │ Ligeramente mejor       │ Bueno                    │
// │ Herencia             │ No (composición)        │ Sí (herencia de clases)  │
// │ Curva aprendizaje    │ Más simple al inicio    │ Familiar para MVC/Java   │
// └──────────────────────┴─────────────────────────┴──────────────────────────┘

// ── MAPEAR CONTROLADORES (debe ir DESPUÉS de los endpoints Minimal API) ──
app.MapControllers();

app.Run();
