using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using FluentAssertions;

namespace Demo.Tests;

// ============================================================================
// Tests de la lección 07 — System.Text.Json y HttpClient
// ============================================================================
// Se prueban las operaciones de serialización JSON y configuración de
// HttpClient. Los tests de HTTP no realizan peticiones reales; verifican
// la construcción de mensajes, URL, cabeceras y configuración.

public class Leccion07Tests
{
    // ── Opciones comunes reutilizables ──────────────────────────────────

    private static readonly JsonSerializerOptions CamelCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonSerializerOptions IndentedOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // ====================================================================
    // 1. Serialize: objeto a string JSON
    // ====================================================================
    [Fact]
    public void Serialize_DeberiaConvertirObjetoAJsonString()
    {
        var post = new BlogPost
        {
            Id = 1,
            Title = "Hola .NET",
            Author = "Antonio",
            PublishedAt = new DateTime(2026, 7, 28),
            Tags = ["csharp", "dotnet"],
            IsPublished = true
        };

        string json = JsonSerializer.Serialize(post, CamelCaseOptions);

        json.Should().Contain("\"id\":1");
        json.Should().Contain("\"title\":\"Hola .NET\"");
        json.Should().Contain("\"author\":\"Antonio\"");
        json.Should().Contain("\"csharp\"");
        json.Should().Contain("\"isPublished\":true");
    }

    // ====================================================================
    // 2. Deserialize: string JSON a objeto
    // ====================================================================
    [Fact]
    public void Deserialize_DeberiaConvertirJsonStringAObjeto()
    {
        string json = """{"id":42,"title":"Test","author":"María","publishedAt":"2026-07-28T10:00:00","tags":["xunit"],"isPublished":false}""";

        BlogPost? result = JsonSerializer.Deserialize<BlogPost>(json, CamelCaseOptions);

        result.Should().NotBeNull();
        result!.Id.Should().Be(42);
        result.Title.Should().Be("Test");
        result.Author.Should().Be("María");
        result.Tags.Should().ContainSingle("xunit");
        result.IsPublished.Should().BeFalse();
    }

    // ====================================================================
    // 3. JsonSerializerOptions.PropertyNamingPolicy.CamelCase
    // ====================================================================
    [Fact]
    public void CamelCasePolicy_DeberiaConvertirNombresDePropiedadACamelCase()
    {
        var post = new BlogPost { Id = 1, Title = "Test", Author = "Ana" };

        string json = JsonSerializer.Serialize(post, CamelCaseOptions);

        // Las propiedades PascalCase (Id, Title) se convierten a camelCase.
        json.Should().Contain("\"id\":");
        json.Should().Contain("\"title\":");
        json.Should().Contain("\"author\":");
        // No deben aparecer en PascalCase.
        json.Should().NotContain("\"Id\":");
        json.Should().NotContain("\"Title\":");
    }

    [Fact]
    public void SinCamelCasePolicy_DeberiaMantenerPascalCase()
    {
        var post = new BlogPost { Id = 1, Title = "Test", Author = "Ana" };

        // Sin opciones, los nombres se serializan tal cual (PascalCase).
        string json = JsonSerializer.Serialize(post);

        json.Should().Contain("\"Id\":");
        json.Should().Contain("\"Title\":");
        json.Should().Contain("\"Author\":");
    }

    // ====================================================================
    // 4. JsonIgnore: excluye propiedad de la serialización
    // ====================================================================
    [Fact]
    public void JsonIgnore_DeberiaExcluirPropiedadDelJsonResultante()
    {
        var comment = new Comment(1, "user01", "Excelente post")
        {
            Email = "secreto@example.com"
        };

        string json = JsonSerializer.Serialize(comment, CamelCaseOptions);

        // Email tiene [JsonIgnore]: NO debe aparecer en el JSON.
        json.Should().NotContain("Email");
        json.Should().NotContain("email");
        json.Should().NotContain("secreto");
        // Las demás propiedades sí se serializan.
        json.Should().Contain("\"body\":\"Excelente post\"");
    }

    [Fact]
    public void JsonIgnore_AlDeserializar_DeberiaIgnorarPropiedadDelJson()
    {
        string json = """{"comment_id":5,"user_name":"test","body":"Hola","email":"ignorado@test.com"}""";

        Comment? result = JsonSerializer.Deserialize<Comment>(json, CamelCaseOptions);

        result.Should().NotBeNull();
        // Email se ignora al deserializar (queda null).
        result!.Email.Should().BeNull();
        result.CommentId.Should().Be(5);
        result.Body.Should().Be("Hola");
    }

    // ====================================================================
    // 5. JsonRequired: lanza excepción si falta la propiedad
    // ====================================================================
    [Fact]
    public void JsonRequired_PropiedadFalta_DeberiaLanzarJsonException()
    {
        // comment_id es [JsonRequired] pero no está en el JSON.
        string json = """{"user_name":"test","body":"Sin comment_id"}""";

        Action act = () => JsonSerializer.Deserialize<Comment>(json, CamelCaseOptions);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void JsonRequired_PropiedadPresente_DeberiaDeserializarCorrectamente()
    {
        string json = """{"comment_id":10,"user_name":"test","body":"Todo OK"}""";

        Comment? result = JsonSerializer.Deserialize<Comment>(json, CamelCaseOptions);

        result.Should().NotBeNull();
        result!.CommentId.Should().Be(10);
    }

    // ====================================================================
    // 6. JsonDocument.Parse: leer JSON sin tipos
    // ====================================================================
    [Fact]
    public void JsonDocumentParse_DeberiaLeerJsonSinDeserializarATipos()
    {
        string json = """{"status":"ok","data":{"count":42,"items":[{"name":"Alpha"},{"name":"Beta"}]}}""";

        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        root.GetProperty("status").GetString().Should().Be("ok");
        root.GetProperty("data").GetProperty("count").GetInt32().Should().Be(42);

        var items = root.GetProperty("data").GetProperty("items").EnumerateArray().ToList();
        items.Should().HaveCount(2);
        items[0].GetProperty("name").GetString().Should().Be("Alpha");
        items[1].GetProperty("name").GetString().Should().Be("Beta");
    }

    [Fact]
    public void JsonDocumentParse_TryGetProperty_NoLanzaSiNoExiste()
    {
        string json = """{"name":"Test"}""";

        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        bool exists = root.TryGetProperty("missing", out JsonElement element);

        exists.Should().BeFalse();
        element.ValueKind.Should().Be(JsonValueKind.Undefined);
    }

    // ====================================================================
    // 7. JsonNode: JSON mutable
    // ====================================================================
    [Fact]
    public void JsonNode_DeberiaCrearYModificarJsonDinamico()
    {
        var obj = new JsonObject
        {
            ["name"] = "Proyecto X",
            ["version"] = "1.0"
        };

        obj["version"] = "2.0";
        obj["description"] = "Nueva descripción";

        obj["name"]!.GetValue<string>().Should().Be("Proyecto X");
        obj["version"]!.GetValue<string>().Should().Be("2.0");
        obj["description"]!.GetValue<string>().Should().Be("Nueva descripción");
    }

    [Fact]
    public void JsonArray_DeberiaPermitirAgregarYEliminarElementos()
    {
        var array = new JsonArray("a", "b", "c");

        array.Add("d");
        array.Remove(array[0]!); // elimina "a"

        array.Should().HaveCount(3);
        array.Select(i => i!.GetValue<string>()).Should().BeEquivalentTo("b", "c", "d");
    }

    [Fact]
    public void JsonNodeParse_DeberiaLeerStringYConvertirANodo()
    {
        string json = """{"producto":"Teclado","precio":79.99,"stock":25}""";

        JsonNode? node = JsonNode.Parse(json);

        node.Should().NotBeNull();
        node!["producto"]!.GetValue<string>().Should().Be("Teclado");
        node["precio"]!.GetValue<decimal>().Should().Be(79.99m);
        node["stock"]!.GetValue<int>().Should().Be(25);
    }

    // ====================================================================
    // 8. JsonSerializerContext (source generator)
    // ====================================================================
    [Fact]
    public void JsonSerializerContext_DeberiaSerializarUsandoCodigoGeneradoEnCompilacion()
    {
        var post = new BlogPost
        {
            Id = 99,
            Title = "Source Generator Test",
            Author = "Test",
            PublishedAt = new DateTime(2026, 1, 15),
            Tags = ["sg"],
            IsPublished = true
        };

        string json = JsonSerializer.Serialize(post, BlogPostContext.Default.BlogPost);

        // WriteIndented añade espacios después de ":".
        json.Should().Contain("\"id\": 99");
        json.Should().Contain("\"title\": \"Source Generator Test\"");
    }

    [Fact]
    public void JsonSerializerContext_DeberiaDeserializarUsandoCodigoGeneradoEnCompilacion()
    {
        string json = """{"id":7,"title":"SG Test","author":"X","publishedAt":"2026-06-01T00:00:00","tags":[],"isPublished":true}""";

        BlogPost? result = JsonSerializer.Deserialize(json, BlogPostContext.Default.BlogPost);

        result.Should().NotBeNull();
        result!.Id.Should().Be(7);
        result.Title.Should().Be("SG Test");
    }

    // ====================================================================
    // 9. Round-trip: objeto → JSON → objeto preserva datos
    // ====================================================================
    [Fact]
    public void RoundTrip_ObjetoAJsonYDeVuelta_DeberiaPreservarDatos()
    {
        var original = new BlogPost
        {
            Id = 100,
            Title = "Round Trip Test",
            Author = "Test",
            PublishedAt = new DateTime(2026, 3, 15, 8, 30, 0),
            Tags = ["roundtrip", "json", "csharp"],
            IsPublished = false
        };

        string json = JsonSerializer.Serialize(original, CamelCaseOptions);
        BlogPost? result = JsonSerializer.Deserialize<BlogPost>(json, CamelCaseOptions);

        result.Should().BeEquivalentTo(original);
    }

    // ====================================================================
    // 10. HttpClient: configuración correcta sin peticiones reales
    // ====================================================================
    [Fact]
    public void HttpClient_CrearConBaseAddress_DeberiaTenerUrlBaseConfigurada()
    {
        using var client = new HttpClient
        {
            BaseAddress = new Uri("https://api.example.com/v1/")
        };

        client.BaseAddress.Should().Be(new Uri("https://api.example.com/v1/"));
    }

    [Fact]
    public void HttpClient_TimeoutPorDefecto_DeberiaSer100Segundos()
    {
        using var client = new HttpClient();

        // El timeout por defecto de HttpClient es 100 segundos.
        client.Timeout.Should().Be(TimeSpan.FromSeconds(100));
    }

    [Fact]
    public void HttpClient_ConfigurarTimeout_DeberiaReflejarValorAsignado()
    {
        using var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        client.Timeout.Should().Be(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void HttpClient_DefaultRequestHeaders_DeberianEstarConfigurados()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("X-Api-Key", "test-key-123");

        client.DefaultRequestHeaders.Contains("Accept").Should().BeTrue();
        client.DefaultRequestHeaders.GetValues("Accept").First().Should().Be("application/json");
        client.DefaultRequestHeaders.GetValues("X-Api-Key").First().Should().Be("test-key-123");
    }

    // ====================================================================
    // 11. HttpClientHelper: crear peticiones sin hacer llamadas reales
    // ====================================================================
    [Fact]
    public void CreateGetRequest_DeberiaCrearHttpRequestMessageConMetodoGet()
    {
        var request = HttpClientHelper.CreateGetRequest("https://api.example.com", "/posts/1");

        request.Method.Should().Be(HttpMethod.Get);
        request.RequestUri.Should().Be("/posts/1");
    }

    [Fact]
    public async Task CreatePostRequest_DeberiaCrearHttpRequestMessageConCuerpoJson()
    {
        var payload = new { title = "Hola", body = "Contenido", userId = 1 };

        var request = HttpClientHelper.CreatePostRequest("https://api.example.com", "/posts", payload);

        request.Method.Should().Be(HttpMethod.Post);
        request.RequestUri.Should().Be("/posts");
        request.Content.Should().NotBeNull();
        request.Content!.Headers.ContentType!.MediaType.Should().Be("application/json");

        string body = await request.Content.ReadAsStringAsync();
        body.Should().Contain("\"title\":\"Hola\"");
        body.Should().Contain("\"body\":\"Contenido\"");
        body.Should().Contain("\"userId\":1");
    }

    [Fact]
    public void CreatePutRequest_DeberiaCrearHttpRequestMessagePutConJson()
    {
        var payload = new { title = "Actualizado" };

        var request = HttpClientHelper.CreatePutRequest("/posts/1", payload);

        request.Method.Should().Be(HttpMethod.Put);
        request.RequestUri.Should().Be("/posts/1");
        request.Content!.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public void CreateDeleteRequest_DeberiaCrearHttpRequestMessageDeleteSinCuerpo()
    {
        var request = HttpClientHelper.CreateDeleteRequest("/posts/1");

        request.Method.Should().Be(HttpMethod.Delete);
        request.RequestUri.Should().Be("/posts/1");
        request.Content.Should().BeNull();
    }

    [Fact]
    public void CreateConfiguredClient_DeberiaTenerTodasLasPropiedadesCorrectas()
    {
        using var client = HttpClientHelper.CreateConfiguredClient(
            "https://api.example.com",
            TimeSpan.FromSeconds(15));

        client.BaseAddress.Should().Be(new Uri("https://api.example.com"));
        client.Timeout.Should().Be(TimeSpan.FromSeconds(15));
        client.DefaultRequestHeaders.GetValues("Accept").First().Should().Be("application/json");
        client.DefaultRequestHeaders.GetValues("User-Agent").First().Should().Be("DemoApp/1.0");
    }

    // ====================================================================
    // 12. HttpResponseMessage: verificar StatusCode sin petición real
    // ====================================================================
    [Fact]
    public void HttpResponseMessage_StatusCodeOK_DeberiaIndicarExito()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccessStatusCode.Should().BeTrue();
        ((int)response.StatusCode).Should().Be(200);
    }

    [Fact]
    public void HttpResponseMessage_StatusCodeNotFound_NoDeberiaSerExitoso()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.IsSuccessStatusCode.Should().BeFalse();
        ((int)response.StatusCode).Should().Be(404);
    }

    [Fact]
    public async Task HttpResponseMessage_DeberiaLeerContenidoComoString()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """{"id":1,"name":"Test"}""",
                Encoding.UTF8,
                "application/json")
        };

        string body = await response.Content.ReadAsStringAsync();
        body.Should().Be("""{"id":1,"name":"Test"}""");
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    // ====================================================================
    // 13. JSON con objetos anidados
    // ====================================================================
    [Fact]
    public void ObjetoAnidado_DeberiaSerializarJsonConSubobjeto()
    {
        var article = new Article
        {
            Id = 1,
            Title = "Articulo con autor",
            Author = new Author
            {
                Name = "Antonio",
                Bio = "Desarrollador .NET",
                JoinedAt = new DateTime(2025, 1, 1)
            },
            Categories = ["tech", "dotnet"]
        };

        string json = JsonSerializer.Serialize(article, IndentedOptions);

        json.Should().Contain("\"title\": \"Articulo con autor\"");
        json.Should().Contain("\"name\": \"Antonio\"");
        json.Should().Contain("\"bio\": \"Desarrollador .NET\"");
        json.Should().Contain("\"categories\": [");
        json.Should().Contain("\"tech\"");
        json.Should().Contain("\"dotnet\"");
    }

    [Fact]
    public void ObjetoAnidado_DeberiaDeserializarJsonConSubobjeto()
    {
        string json = """
        {
            "id": 10,
            "title": "Nested Test",
            "author": { "name": "Elena", "bio": "Escritora", "joinedAt": "2026-01-01T00:00:00" },
            "categories": ["news"]
        }
        """;

        Article? result = JsonSerializer.Deserialize<Article>(json, CamelCaseOptions);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Nested Test");
        result.Author.Name.Should().Be("Elena");
        result.Author.Bio.Should().Be("Escritora");
        result.Categories.Should().ContainSingle("news");
    }

    // ====================================================================
    // 14. DateTime serialization con formato custom
    // ====================================================================
    [Fact]
    public void DateTime_SerializacionPorDefecto_DeberiaUsarFormatoISO8601()
    {
        var post = new BlogPost
        {
            Id = 1,
            Title = "T",
            Author = "A",
            PublishedAt = new DateTime(2026, 7, 28, 14, 30, 45)
        };

        string json = JsonSerializer.Serialize(post, CamelCaseOptions);

        // Formato ISO 8601 por defecto.
        json.Should().Contain("2026-07-28T14:30:45");
    }

    [Fact]
    public void DateTime_ConUnixEpochConverter_DeberiaSerializarComoTimestamp()
    {
        // Crear opciones con un convertidor custom para DateTime.
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new UnixEpochDateTimeConverter() }
        };

        var date = new DateTime(2026, 7, 28, 0, 0, 0, DateTimeKind.Utc);
        var wrapper = new { timestamp = date };

        string json = JsonSerializer.Serialize(wrapper, options);

        // El timestamp Unix para 2026-07-28T00:00:00Z.
        long expectedEpoch = new DateTimeOffset(date).ToUnixTimeSeconds();
        json.Should().Contain($"\"timestamp\":{expectedEpoch}");
    }

    // ====================================================================
    // 15. WriteIndented: JSON formateado
    // ====================================================================
    [Fact]
    public void WriteIndented_DeberiaProducirJsonConSaltosDeLineaEIndentacion()
    {
        var obj = new { name = "Test", value = 42 };

        string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });

        json.Should().Contain("\n");
        json.Should().Contain("  ");
    }

    [Fact]
    public void SinWriteIndented_DeberiaProducirJsonCompacto()
    {
        var obj = new { name = "Test", value = 42 };

        string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false });

        json.Should().NotContain("\n");
    }

    // ====================================================================
    // 16. JsonConstructor: deserialización con constructor personalizado
    // ====================================================================
    [Fact]
    public void JsonConstructor_DeberiaUsarConstructorAnotadoAlDeserializar()
    {
        string json = """{"comment_id":77,"user_name":"constructor_user","body":"Creado desde JSON"}""";

        Comment? result = JsonSerializer.Deserialize<Comment>(json, CamelCaseOptions);

        result.Should().NotBeNull();
        result!.CommentId.Should().Be(77);
        result.UserName.Should().Be("constructor_user");
        result.Body.Should().Be("Creado desde JSON");
    }

    // ====================================================================
    // 17. Utf8JsonReader: lectura de tokens
    // ====================================================================
    [Fact]
    public void Utf8JsonReader_DeberiaLeerTokensDeJsonUtf8()
    {
        byte[] utf8Bytes = Encoding.UTF8.GetBytes("""{"name":"Test","age":30}""");
        var reader = new Utf8JsonReader(utf8Bytes);

        var tokens = new List<(JsonTokenType, string?)>();
        while (reader.Read())
        {
            string? value = reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Number => reader.GetInt32().ToString(),
                JsonTokenType.PropertyName => reader.GetString(),
                _ => null
            };
            tokens.Add((reader.TokenType, value));
        }

        tokens.Should().Contain(t => t.Item1 == JsonTokenType.String && t.Item2 == "Test");
        tokens.Should().Contain(t => t.Item1 == JsonTokenType.PropertyName && t.Item2 == "name");
        tokens.Should().Contain(t => t.Item1 == JsonTokenType.PropertyName && t.Item2 == "age");
    }

    // ====================================================================
    // 18. Utf8JsonWriter: escritura de tokens
    // ====================================================================
    [Fact]
    public void Utf8JsonWriter_DeberiaEscribirJsonUtf8Correctamente()
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("result", "success");
            writer.WriteNumber("code", 200);
            writer.WriteEndObject();
            writer.Flush();
        }

        string json = Encoding.UTF8.GetString(stream.ToArray());

        json.Should().Contain("\"result\":\"success\"");
        json.Should().Contain("\"code\":200");
    }
}

// ============================================================================
// Convertidor custom: DateTime como Unix Epoch (timestamp en segundos)
// ============================================================================
public class UnixEpochDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        long epoch = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeSeconds(epoch).UtcDateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        long epoch = new DateTimeOffset(value).ToUnixTimeSeconds();
        writer.WriteNumberValue(epoch);
    }
}
