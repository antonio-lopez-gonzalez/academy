// ============================================================================
// Lección 07 — System.Text.Json y HttpClient
// ============================================================================
// System.Text.Json es la biblioteca de serialización JSON de alto rendimiento
// incluida en .NET. HttpClient es el cliente HTTP estándar para consumir APIs.
// Ambos reemplazan a Newtonsoft.Json y WebClient/HttpWebRequest respectivamente.

using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

Console.OutputEncoding = Encoding.UTF8;

// ============================================================================
// PARTE 1 — SYSTEM.TEXT.JSON
// ============================================================================

Console.WriteLine("══════════════════════════════════════════════════════════════");
Console.WriteLine("  PARTE 1 — System.Text.Json");
Console.WriteLine("══════════════════════════════════════════════════════════════\n");

// ─── 1.1 JsonSerializer.Serialize() — Objeto → JSON ─────────────────────
Console.WriteLine("─── 1.1 Serialize — De objeto C# a string JSON ───");

var post = new BlogPost
{
    Id = 1,
    Title = "Primeros pasos con .NET 8",
    Author = "Antonio",
    PublishedAt = new DateTime(2026, 7, 28, 10, 30, 0),
    Tags = ["csharp", "dotnet", "tutorial"],
    IsPublished = true
};

// Serialización básica: convierte un objeto .NET en una cadena JSON.
string json = JsonSerializer.Serialize(post);
Console.WriteLine($"  Serialize básico:\n    {json}");

// ─── 1.2 JsonSerializerOptions — Personalizar la serialización ─────────
Console.WriteLine("\n─── 1.2 JsonSerializerOptions — Personalización ───");

// Las opciones permiten controlar cómo se genera el JSON.
var options = new JsonSerializerOptions
{
    // WriteIndented: genera JSON con indentación y saltos de línea (legible).
    WriteIndented = true,
    // PropertyNamingPolicy: convierte los nombres de propiedad a camelCase.
    // CamelCase convierte "PublishedAt" en "publishedAt", estándar en JSON.
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    // DefaultIgnoreCondition: omite propiedades con valores por defecto.
    // WhenWritingNull omite las propiedades cuyo valor es null.
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

string jsonPretty = JsonSerializer.Serialize(post, options);
Console.WriteLine($"  Con WriteIndented + CamelCase:\n{jsonPretty}");

// Serialización con camelCase sin indentar (caso más común en APIs).
var compactOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
string jsonCamel = JsonSerializer.Serialize(post, compactOptions);
Console.WriteLine($"  CamelCase compacto: {jsonCamel}");

// ─── 1.3 JsonSerializer.Deserialize() — JSON → Objeto ──────────────────
Console.WriteLine("\n─── 1.3 Deserialize — De string JSON a objeto C# ───");

string inputJson = """
    {
        "id": 42,
        "title": "Aprendiendo System.Text.Json",
        "author": "María",
        "publishedAt": "2026-07-28T15:00:00",
        "tags": ["json", "csharp"],
        "isPublished": false
    }
    """;

// Deserialización: reconstruye un objeto .NET a partir de una cadena JSON.
// Usa las mismas opciones (camelCase) para que coincidan los nombres.
BlogPost? deserialized = JsonSerializer.Deserialize<BlogPost>(inputJson, compactOptions);
Console.WriteLine($"  Deserializado: Id={deserialized!.Id}, Title='{deserialized.Title}', " +
                  $"Author='{deserialized.Author}'");

// ─── 1.4 Atributos: JsonPropertyName, JsonIgnore, JsonRequired, JsonConstructor ─
Console.WriteLine("\n─── 1.4 Atributos de serialización ───");

// Demostración con la clase Comment, que usa todos estos atributos.
// Ver definición al final del archivo.

var comment = new Comment(100, "usuario_01", "¡Excelente artículo!")
{
    Email = "usuario@example.com" // JsonIgnore: no se serializa
};

string commentJson = JsonSerializer.Serialize(comment, new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});
Console.WriteLine($"  Comment serializado (Email ignorado, nombres personalizados):\n{commentJson}");

// Deserialización con JsonConstructor: el JSON se mapea al constructor.
string commentInput = """
    {
        "comment_id": 100,
        "user_name": "usuario_01",
        "body": "¡Excelente artículo!"
    }
    """;
Comment? deserializedComment = JsonSerializer.Deserialize<Comment>(commentInput, new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});
Console.WriteLine($"  Comment deserializado: Body='{deserializedComment!.Body}', User='{deserializedComment.UserName}'");

// JsonRequired: si falta la propiedad, lanza JsonException.
try
{
    string missingField = """{ "body": "falta comment_id" }""";
    JsonSerializer.Deserialize<Comment>(missingField, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });
}
catch (JsonException ex)
{
    Console.WriteLine($"  JsonRequired lanza excepción: {ex.Message[..Math.Min(ex.Message.Length, 80)]}...");
}

// ─── 1.5 JsonDocument — Parsear JSON sin deserializar a tipos ────────
Console.WriteLine("\n─── 1.5 JsonDocument — Lectura sin tipos ───");

// JsonDocument permite leer JSON sin necesidad de definir clases.
// Útil para inspeccionar la estructura o extraer campos puntuales.
// Es read-only, rápido y memory-efficient (usa pooled memory).

string apiResponse = """
    {
        "status": "success",
        "data": {
            "users": [
                { "id": 1, "name": "Ana" },
                { "id": 2, "name": "Luis" },
                { "id": 3, "name": "Carlos" }
            ],
            "total": 3
        },
        "metadata": { "version": "1.0", "generatedAt": "2026-07-28" }
    }
    """;

using JsonDocument document = JsonDocument.Parse(apiResponse);
JsonElement root = document.RootElement;

// Acceder a propiedades por nombre.
string status = root.GetProperty("status").GetString()!;
Console.WriteLine($"  Status: {status}");

// Navegar por objetos anidados.
JsonElement data = root.GetProperty("data");
int total = data.GetProperty("total").GetInt32();
Console.WriteLine($"  Total usuarios: {total}");

// Iterar un array.
Console.WriteLine("  Usuarios:");
foreach (JsonElement user in data.GetProperty("users").EnumerateArray())
{
    int id = user.GetProperty("id").GetInt32();
    string name = user.GetProperty("name").GetString()!;
    Console.WriteLine($"    [{id}] {name}");
}

// TryGetProperty evita excepciones si la propiedad no existe.
if (root.TryGetProperty("error", out JsonElement errorElement))
    Console.WriteLine($"  Error: {errorElement.GetString()}");
else
    Console.WriteLine("  No hay errores en la respuesta.");

// ─── 1.6 JsonNode — JSON mutable (System.Text.Json.Nodes) ────────────
Console.WriteLine("\n─── 1.6 JsonNode / JsonArray — JSON modificable ───");

// JsonNode permite crear y modificar estructuras JSON de forma dinámica,
// similar a JObject/JArray de Newtonsoft.Json. A diferencia de JsonDocument,
// JsonNode es mutable.

// Crear un objeto JSON desde cero.
var jsonObject = new JsonObject
{
    ["name"] = "Proyecto Alpha",
    ["version"] = "1.0.0",
    ["active"] = true,
    ["tags"] = new JsonArray("web", "api", "cloud"),
    ["metadata"] = new JsonObject
    {
        ["createdBy"] = "Antonio",
        ["stars"] = 42
    }
};

Console.WriteLine($"  JsonNode creado:\n    {jsonObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true })}");

// Modificar un valor existente.
jsonObject["version"] = "2.0.0";
Console.WriteLine($"  Versión actualizada: {jsonObject["version"]}");

// Añadir una propiedad nueva.
jsonObject["description"] = "Sistema de gestión documental";
Console.WriteLine($"  Propiedad añadida: description={jsonObject["description"]}");

// Parsear un string JSON a JsonNode (como JObject.Parse).
string jsonString = """{"product":"Teclado","price":79.99,"stock":25}""";
JsonNode? parsed = JsonNode.Parse(jsonString);
Console.WriteLine($"  Parseado desde string: producto={parsed!["product"]}, precio={parsed["price"]}");

// JsonArray: manipular arrays.
var items = new JsonArray("item1", "item2");
items.Add("item3");
items.Remove("item1");
Console.WriteLine($"  JsonArray: {string.Join(", ", items.Select(i => i!.ToString()))}");

// ─── 1.7 JsonSerializerContext — Source generator (compilado) ────────
Console.WriteLine("\n─── 1.7 JsonSerializerContext — Source Generator ───");

// El source generator genera código de serialización en tiempo de compilación.
// Ventajas:
//   - Más rápido (sin reflexión en runtime).
//   - AOT-friendly (Native AOT necesita source generators).
//   - Trimmable (el linker puede eliminar código no usado).

// Usamos el contexto generado automáticamente por el atributo [JsonSerializable].
string contextJson = JsonSerializer.Serialize(post, BlogPostContext.Default.BlogPost);
Console.WriteLine($"  Serializado con source generator (CamelCase + Indented):\n{contextJson}");

BlogPost? contextDeserialized = JsonSerializer.Deserialize(contextJson, BlogPostContext.Default.BlogPost);
Console.WriteLine($"  Deserializado con source generator: {contextDeserialized!.Title}");

// ─── 1.8 Utf8JsonReader y Utf8JsonWriter — Alto rendimiento ─────────
Console.WriteLine("\n─── 1.8 Utf8JsonReader / Utf8JsonWriter — Bajo nivel ───");

// Utf8JsonReader: lectura forward-only, sin asignaciones en heap.
// Ideal para escenarios de alto rendimiento donde no se necesita deserializar
// todo el objeto (ej: leer un campo concreto de un JSON enorme).

byte[] utf8Json = Encoding.UTF8.GetBytes(jsonCamel);

var reader = new Utf8JsonReader(utf8Json);
Console.WriteLine("  Leyendo tokens con Utf8JsonReader:");
while (reader.Read())
{
    // TokenType indica el tipo de token actual (StartObject, PropertyName, String, etc.)
    string indent = new(' ', reader.CurrentDepth * 2);
    switch (reader.TokenType)
    {
        case JsonTokenType.PropertyName:
            Console.WriteLine($"{indent}{reader.GetString()}:");
            break;
        case JsonTokenType.String:
            Console.WriteLine($"{indent}  \"{reader.GetString()}\"");
            break;
        case JsonTokenType.Number:
            Console.WriteLine($"{indent}  {reader.GetInt32()}");
            break;
        case JsonTokenType.True:
        case JsonTokenType.False:
            Console.WriteLine($"{indent}  {reader.GetBoolean()}");
            break;
        case JsonTokenType.StartObject:
            Console.WriteLine($"{indent}{{");
            break;
        case JsonTokenType.EndObject:
            Console.WriteLine($"{indent}}}");
            break;
        case JsonTokenType.StartArray:
            Console.WriteLine($"{indent}[");
            break;
        case JsonTokenType.EndArray:
            Console.WriteLine($"{indent}]");
            break;
    }
}

// Utf8JsonWriter: escritura forward-only de JSON a un buffer.
// Útil para generar JSON de forma eficiente sin crear objetos intermedios.
using var stream = new MemoryStream();
using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
{
    writer.WriteStartObject();
    writer.WriteString("mensaje", "Generado con Utf8JsonWriter");
    writer.WriteNumber("timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    writer.WriteStartArray("lenguajes");
    writer.WriteStringValue("C#");
    writer.WriteStringValue("F#");
    writer.WriteStringValue("VB.NET");
    writer.WriteEndArray();
    writer.WriteEndObject();
    writer.Flush();
}
string writtenJson = Encoding.UTF8.GetString(stream.ToArray());
Console.WriteLine($"\n  JSON generado con Utf8JsonWriter:\n{writtenJson}");

// ─── 1.9 Comparación System.Text.Json vs Newtonsoft.Json ─────────────
Console.WriteLine("─── 1.9 Comparación: System.Text.Json vs Newtonsoft.Json ───");
Console.WriteLine("""
  
  ┌─────────────────────────┬──────────────────────┬────────────────────────┐
  │ Característica          │ System.Text.Json     │ Newtonsoft.Json        │
  ├─────────────────────────┼──────────────────────┼────────────────────────┤
  │ Rendimiento             │ 1.3x–2x más rápido   │ Base                   │
  │ Memoria (allocations)   │ Menos (Span<byte>)   │ Más (string-based)     │
  │ Native AOT              │ Sí (source gen)      │ No                     │
  │ Polimorfismo            │ Limitado (mejoró v8) │ Completo               │
  │ Convertidores custom    │ JsonConverter<T>     │ JsonConverter<T>       │
  │ JSON dinámico           │ JsonNode/JsonDocument│ JObject/JArray/JToken  │
  │ Licencia                │ MIT (incluido en SDK)│ MIT (NuGet externo)    │
  │ Larga vida (>15 años)   │ .NET Core 3.0+       │ Legacy/Unity           │
  └─────────────────────────┴──────────────────────┴────────────────────────┘

  Regla general:
    - Proyectos nuevos .NET → System.Text.Json (siempre).
    - Legacy migrando desde .NET Framework → Newtonsoft → System.Text.Json.
    - Solo usar Newtonsoft si necesitas polimorfismo complejo con tipos
      desconocidos en tiempo de compilación.
  """);

// ============================================================================
// PARTE 2 — HTTPCLIENT
// ============================================================================

Console.WriteLine("══════════════════════════════════════════════════════════════");
Console.WriteLine("  PARTE 2 — HttpClient");
Console.WriteLine("══════════════════════════════════════════════════════════════\n");

// ─── 2.1 HttpClient básico — Creación y configuración ────────────────
Console.WriteLine("─── 2.1 HttpClient básico — GET, POST, PUT, DELETE ───");

// HttpClient encapsula HTTP. Es thread-safe y reutilizable.
// IMPORTANTE: NO crear un nuevo HttpClient por cada petición
// (provoca Socket Exhaustion). Ver sección 2.4.

// HttpClient se configura con BaseAddress, DefaultRequestHeaders, Timeout.
using var client = new HttpClient
{
    BaseAddress = new Uri("https://jsonplaceholder.typicode.com"),
    Timeout = TimeSpan.FromSeconds(10)
};

// Cabeceras por defecto: se envían en todas las peticiones.
client.DefaultRequestHeaders.Add("Accept", "application/json");
client.DefaultRequestHeaders.Add("User-Agent", "DemoApp/1.0");

Console.WriteLine($"  BaseAddress: {client.BaseAddress}");
Console.WriteLine($"  Timeout: {client.Timeout.TotalSeconds}s");
Console.WriteLine($"  Default Headers: Accept={client.DefaultRequestHeaders.Accept.First()}, " +
                  $"User-Agent={client.DefaultRequestHeaders.UserAgent}");

// ─── 2.2 HttpRequestMessage — Construir peticiones ──────────────────
Console.WriteLine("\n─── 2.2 HttpRequestMessage — Petición con cabeceras ───");

// HttpRequestMessage representa una petición HTTP completa:
// método, URL, cabeceras, contenido.
var request = new HttpRequestMessage(HttpMethod.Get, "/posts/1");
request.Headers.Add("X-Custom-Header", "DemoValue");

Console.WriteLine($"  Método: {request.Method}");
Console.WriteLine($"  URL: {request.RequestUri}");
Console.WriteLine($"  Cabecera custom: {request.Headers.GetValues("X-Custom-Header").First()}");

// ─── 2.3 Enviar JSON con StringContent ──────────────────────────────
Console.WriteLine("\n─── 2.3 StringContent — Enviar cuerpo JSON ───");

// Para enviar JSON, usamos StringContent con el media type application/json.
var newPost = new { title = "Nuevo Post", body = "Contenido del post", userId = 1 };
string newPostJson = JsonSerializer.Serialize(newPost, new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});

Console.WriteLine($"  JSON a enviar: {newPostJson}");

// StringContent: representa el cuerpo de la petición HTTP como string.
var content = new StringContent(newPostJson, Encoding.UTF8, "application/json");
Console.WriteLine($"  Content-Type: {content.Headers.ContentType}");

// ── Enviar con HttpClient usando PostAsJsonAsync (método de extensión) ──
Console.WriteLine($"\n  PostAsJsonAsync: serializa y envía en un solo paso.");
Console.WriteLine($"  Equivalente manual: new StringContent(JsonSerializer.Serialize(obj), " +
    "Encoding.UTF8, \"application/json\")");

// ─── 2.4 HttpClient como Singleton — Socket Exhaustion ──────────────
Console.WriteLine("\n─── 2.4 HttpClient Singleton — El problema de los sockets ───");
Console.WriteLine("""
  
  ⚠️  ERROR COMÚN: Crear un nuevo HttpClient por cada petición.

  ```csharp
  // ❌ INCORRECTO — Cada instancia abre un nuevo socket
  for (int i = 0; i < 1000; i++)
  {
      using var c = new HttpClient();
      var r = await c.GetAsync("...");
  }
  ```

  ¿Por qué está mal?
  - Cada HttpClient abre una conexión TCP (socket) en el sistema operativo.
  - Después de Dispose(), el socket entra en estado TIME_WAIT (~240s en Windows).
  - Tras ~1000 peticiones rápidas, se agotan los sockets del sistema.
  - Resultado: SocketException — "No se puede establecer conexión".

  ✅ CORRECTO: Una única instancia estática (o reutilizada).

  ```csharp
  // HttpClient como campo static readonly (Singleton manual).
  private static readonly HttpClient _client = new()
  {
      BaseAddress = new Uri("https://api.example.com")
  };
  ```

  ¿Por qué funciona?
  - HttpClient reutiliza las conexiones subyacentes (connection pooling).
  - .NET administra los sockets por debajo con SocketsHttpHandler.
  - Un único HttpClient puede manejar miles de peticiones concurrentes.
  """);

// ─── 2.5 IHttpClientFactory — La solución profesional ────────────────
Console.WriteLine("─── 2.5 IHttpClientFactory — Gestión profesional ───");
Console.WriteLine("""
  
  IHttpClientFactory resuelve todos los problemas de HttpClient:
  - Evita Socket Exhaustion (el pool centraliza y reutiliza sockets).
  - Rotación de DNS (una instancia única no detecta cambios de DNS).
  - Configuración centralizada (named/typed clients).
  - Integración con Polly para resiliencia (retry, circuit breaker).

  Se verá en profundidad en el Módulo 4 (C# Avanzado).

  Registro en DI:
  ```csharp
  services.AddHttpClient("jsonplaceholder", client =>
  {
      client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
      client.DefaultRequestHeaders.Add("Accept", "application/json");
      client.Timeout = TimeSpan.FromSeconds(5);
  });
  ```

  Uso:
  ```csharp
  var factory = serviceProvider.GetRequiredService<IHttpClientFactory>();
  HttpClient client = factory.CreateClient("jsonplaceholder");
  ```
  """);

// ─── 2.6 HttpResponseMessage — Leer la respuesta ─────────────────────
Console.WriteLine("─── 2.6 HttpResponseMessage — Leer la respuesta ───");

// HttpResponseMessage contiene StatusCode, Content, y Headers.
var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
{
    Content = new StringContent(
        """{"id":1,"title":"Post de ejemplo","body":"Contenido"}""",
        Encoding.UTF8,
        "application/json")
};

Console.WriteLine($"  StatusCode: {response.StatusCode} ({(int)response.StatusCode})");
Console.WriteLine($"  IsSuccessStatusCode: {response.IsSuccessStatusCode}");
Console.WriteLine($"  Content-Type: {response.Content.Headers.ContentType}");

// Leer el contenido como string (síncrono para demo, usar async en producción).
string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
Console.WriteLine($"  Body: {responseBody}");

// Leer el contenido como stream (más eficiente para respuestas grandes).
// await response.Content.ReadAsStreamAsync();

// ─── 2.7 Cancellation y Timeout ──────────────────────────────────────
Console.WriteLine("\n─── 2.7 CancellationToken y Timeout ───");

// CancellationToken permite cancelar peticiones en curso.
Console.WriteLine("""
  
  CancellationToken se pasa a GetAsync, PostAsync, etc.
  Se combina con el Timeout de HttpClient:

  ```csharp
  var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
  var response = await client.GetAsync("/posts/1", cts.Token);
  ```

  - Si el timeout se alcanza, se lanza TaskCanceledException.
  - El HttpClient.Timeout global también lanza TaskCanceledException.
  - La diferencia: CancellationToken es por petición, Timeout es global.
  """);

// ─── 2.8 Comparación HttpClient vs fetch (JS) vs requests (Python) ──
Console.WriteLine("═══ 2.8 Comparación: HttpClient vs fetch vs requests ═══");
Console.WriteLine("""
  
  ┌──────────────────────┬──────────────────┬─────────────────┬──────────────────┐
  │                      │ C# HttpClient    │ JS fetch        │ Python requests  │
  ├──────────────────────┼──────────────────┼─────────────────┼──────────────────┤
  │ Async                │ async/await      │ async/await     │ Síncrono (por def)│
  │ Thread-safe          │ Sí               │ Sí (single thr) │ Sí (GIL)         │
  │ Timeout por defecto  │ 100 segundos     │ 300 segundos    │ Sin límite (!)   │
  │ Cancelación          │ CancellationToken│ AbortController │ Timeout/exc info │
  │ Headers por defecto  │ Declarativos     │ Declarativos    │ Diccionario      │
  │ Reutilizar instancia │ ¡OBLIGATORIO!    │ N/A (stateless) │ N/A (stateless)  │
  │ Pooling de conexiones│ Automático       │ Automático      │ Sessions         │
  │ Serialización JSON   │ PostAsJsonAsync  │ JSON.stringify  │ json= parámetro  │
  └──────────────────────┴──────────────────┴─────────────────┴──────────────────┘
  """);

// ============================================================================
// Final
// ============================================================================

Console.WriteLine("══════════════════════════════════════════════════════════════");
Console.WriteLine("  Fin de la demo — Lección 07 completada.");
Console.WriteLine("  Ejecuta: dotnet test para validar los tests.");
Console.WriteLine("══════════════════════════════════════════════════════════════");

// ============================================================================
// MODELOS DE DATOS Y CONTEXTOS
// ============================================================================

/// <summary>
/// Representa un artículo de blog con atributos de serialización JSON.
/// </summary>
public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";

    // JsonPropertyName: el nombre en JSON será "publishedAt" (camelCase manual).
    // Si además usas PropertyNamingPolicy.CamelCase, no es necesario;
    // aquí se muestra para casos donde quieres un nombre distinto al generado.
    [JsonPropertyName("publishedAt")]
    public DateTime PublishedAt { get; set; }

    public List<string> Tags { get; set; } = [];

    [JsonPropertyName("isPublished")]
    public bool IsPublished { get; set; }
}

/// <summary>
/// Representa un comentario. Demuestra JsonConstructor, JsonIgnore y JsonRequired.
/// </summary>
public class Comment
{
    // JsonRequired: el JSON DEBE contener esta propiedad o se lanza JsonException.
    [JsonRequired]
    [JsonPropertyName("comment_id")]
    public int CommentId { get; set; }

    [JsonPropertyName("user_name")]
    public string UserName { get; set; }

    // JsonIgnore: esta propiedad NO se serializa ni deserializa.
    // Útil para datos sensibles (contraseñas) o calculados (propiedades de navegación).
    [JsonIgnore]
    public string? Email { get; set; }

    // Propiedad sin atributos: se serializa con el mismo nombre.
    public string Body { get; set; }

    // Constructor por defecto (necesario para deserialización sin JsonConstructor).
    public Comment() { Body = ""; UserName = ""; }

    // JsonConstructor: indica qué constructor usar al deserializar.
    // Los parámetros se mapean a las propiedades JSON por nombre.
    [JsonConstructor]
    public Comment(int commentId, string userName, string body)
    {
        CommentId = commentId;
        UserName = userName;
        Body = body;
    }
}

/// <summary>
/// Modelo para demostrar serialización de objetos anidados.
/// </summary>
public class Author
{
    public string Name { get; set; } = "";
    public string? Bio { get; set; }
    public DateTime JoinedAt { get; set; }
}

public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public Author Author { get; set; } = new();
    public List<string> Categories { get; set; } = [];
}

// ─── Source Generator Context ───────────────────────────────────────────────

/// <summary>
/// Contexto de serialización generado en tiempo de compilación.
/// El atributo [JsonSerializable] indica al source generator qué tipos incluir.
/// La clase debe ser partial y heredar de JsonSerializerContext.
/// </summary>
[JsonSerializable(typeof(BlogPost))]
[JsonSerializable(typeof(Comment))]
[JsonSerializable(typeof(Article))]
[JsonSerializable(typeof(Author))]
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class BlogPostContext : JsonSerializerContext
{
}

// ─── HttpClientHelper — Demostración de configuración testeable ─────────────

/// <summary>
/// Helper que construye HttpRequestMessage para demostrar creación de peticiones.
/// No realiza peticiones reales; está pensado para testing.
/// </summary>
public static class HttpClientHelper
{
    /// <summary>
    /// Construye un HttpRequestMessage GET con la URL relativa.
    /// </summary>
    public static HttpRequestMessage CreateGetRequest(string baseUrl, string endpoint)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        return new HttpRequestMessage(HttpMethod.Get, endpoint);
    }

    /// <summary>
    /// Construye un HttpRequestMessage POST con cuerpo JSON.
    /// </summary>
    public static HttpRequestMessage CreatePostRequest<T>(string baseUrl, string endpoint, T payload)
    {
        string json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    /// <summary>
    /// Crea un HttpClient configurado para testing de su estado.
    /// </summary>
    public static HttpClient CreateConfiguredClient(string baseUrl, TimeSpan? timeout = null)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = timeout ?? TimeSpan.FromSeconds(30)
        };
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("User-Agent", "DemoApp/1.0");
        return client;
    }

    /// <summary>
    /// Serializa un objeto a JSON usando camelCase por defecto.
    /// </summary>
    public static string SerializeToJson<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
    }

    /// <summary>
    /// Construye un HttpRequestMessage PUT con cuerpo JSON.
    /// </summary>
    public static HttpRequestMessage CreatePutRequest<T>(string endpoint, T payload)
    {
        string json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return new HttpRequestMessage(HttpMethod.Put, endpoint)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    /// <summary>
    /// Construye un HttpRequestMessage DELETE.
    /// </summary>
    public static HttpRequestMessage CreateDeleteRequest(string endpoint)
    {
        return new HttpRequestMessage(HttpMethod.Delete, endpoint);
    }
}
