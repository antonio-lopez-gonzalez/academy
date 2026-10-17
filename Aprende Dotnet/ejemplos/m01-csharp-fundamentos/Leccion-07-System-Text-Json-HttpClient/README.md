# Lección 07 — System.Text.Json y HttpClient

## 1. Introducción

**¿Qué vamos a aprender?**
- `System.Text.Json`: serializar y deserializar JSON en .NET con alto rendimiento.
- `HttpClient`: consumir APIs HTTP de forma correcta y profesional.

**¿Por qué existen?**
En 2019, Microsoft introdujo `System.Text.Json` como reemplazo nativo de `Newtonsoft.Json` (la librería JSON dominante durante 15 años). Las razones: rendimiento (usa `Span<byte>` en lugar de strings), menor uso de memoria, compatibilidad con Native AOT y eliminación de dependencias externas.

`HttpClient` existe desde .NET Framework 4.5 y reemplaza a `WebClient`/`HttpWebRequest`. Es el cliente HTTP estándar para cualquier aplicación .NET.

**¿Qué problema resuelven?**
- `System.Text.Json`: convertir entre objetos C# y JSON sin reflexión innecesaria.
- `HttpClient`: comunicación HTTP con connection pooling, cancelación y extensibilidad.

**Errores típicos:**
- Crear un `new HttpClient()` por cada petición (Socket Exhaustion).
- Usar `Newtonsoft.Json` en proyectos nuevos sin razón.
- No configurar `Timeout` ni `CancellationToken` en peticiones HTTP.

---

## 2. Teoría

### System.Text.Json

La serialización en .NET funciona mediante **reflexión** en tiempo de ejecución o mediante **source generators** en tiempo de compilación. El flujo básico:

```
Objeto C#  ──Serialize()──▶  byte[] (UTF-8)  ──▶  string JSON
string JSON ──▶  byte[] (UTF-8)  ──Deserialize()──▶  Objeto C#
```

Internamente, el serializador usa `Utf8JsonReader`/`Utf8JsonWriter` que trabajan directamente sobre bytes UTF-8, evitando asignaciones de string intermedias. Esto es la clave de su rendimiento: `Span<byte>` en stack en lugar de `string` en heap.

**Componentes principales:**

| API | Propósito | Nivel |
|---|---|---|
| `JsonSerializer.Serialize/Deserialize` | Uso diario (90% de los casos) | Alto |
| `JsonSerializerOptions` | Configuración global | Alto |
| `JsonDocument` | Parseo read-only sin tipos | Medio |
| `JsonNode` / `JsonArray` | JSON mutable dinámico | Medio |
| `JsonSerializerContext` | Source generator (AOT-safe) | Medio |
| `Utf8JsonReader` / `Utf8JsonWriter` | Alto rendimiento, forward-only | Bajo |

### HttpClient

`HttpClient` es un wrapper sobre `SocketsHttpHandler` (en .NET Core/.NET 5+), que a su vez gestiona sockets TCP a nivel de sistema operativo.

```
Tu código ──▶ HttpClient ──▶ SocketsHttpHandler ──▶ Connection Pool ──▶ TCP Socket ──▶ Servidor
```

**Ciclo de vida de una petición:**
1. `HttpClient.SendAsync()` recibe un `HttpRequestMessage`.
2. `SocketsHttpHandler` busca un socket disponible en el pool para ese host.
3. Si no hay socket libre, abre uno nuevo (hasta el límite de `MaxConnectionsPerServer`).
4. Envía la petición y espera la respuesta.
5. Al recibir `HttpResponseMessage`, el socket vuelve al pool (NO se cierra).
6. Si haces `Dispose()` del `HttpClient`, el socket se cierra y entra en `TIME_WAIT`.

---

## 3. Ejemplo mínimo

```csharp
using System.Text.Json;

// ── Serializar ──
var obj = new { Name = "Ana", Age = 28 };
string json = JsonSerializer.Serialize(obj);
Console.WriteLine(json); // {"Name":"Ana","Age":28}

// ── Deserializar ──
string input = """{"Name":"Luis","Age":35}""";
var person = JsonSerializer.Deserialize<Person>(input);
Console.WriteLine(person.Name); // Luis

// ── HttpClient ──
using var client = new HttpClient { BaseAddress = new Uri("https://api.example.com") };
var response = await client.GetAsync("/users");
var content = await response.Content.ReadAsStringAsync();

public record Person(string Name, int Age);
```

Ejecutar con:
```bash
dotnet run --project src/Demo
```

---

## 4. Ejemplo real

El `Program.cs` de esta lección cubre todos los escenarios reales:

- **API REST**: serializar requests JSON con `camelCase`, deserializar respuestas.
- **Lectura parcial**: `JsonDocument` para extraer solo un campo de una respuesta enorme.
- **JSON dinámico**: `JsonNode` cuando la estructura no se conoce en tiempo de compilación.
- **Alto rendimiento**: `Utf8JsonReader` para procesar streams de JSON sin asignar objetos.
- **AOT**: `JsonSerializerContext` cuando compilas con Native AOT (recorte de código).
- **HTTP profesional**: `HttpClient` como singleton con `BaseAddress`, `Timeout` y `DefaultRequestHeaders`.

```bash
dotnet run --project src/Demo
```

---

## 5. Buenas prácticas

### System.Text.Json

✅ **Usar `PropertyNamingPolicy.CamelCase`** siempre en APIs (estándar JSON).
✅ **Usar `JsonSerializerContext` (source generator)** en aplicaciones AOT o de alto rendimiento.
✅ **Usar `JsonDocument`** para leer campos puntuales de JSON grandes sin deserializar todo.
✅ **Usar `JsonIgnore`** en propiedades sensibles (contraseñas, tokens).
✅ **Usar `JsonRequired`** para campos obligatorios: detección temprana de errores.
✅ **Configurar `DefaultIgnoreCondition.WhenWritingNull`** para reducir tamaño de respuestas.

❌ **No usar `Newtonsoft.Json`** en proyectos nuevos (solo si migras legacy).
❌ **No deserializar a `object`/`dynamic`** con `JsonSerializer.Deserialize<object>()` — devuelve `JsonElement`.
❌ **No ignorar `JsonException`** — contiene detalles de qué falló y en qué propiedad.

### HttpClient

✅ **Usar una única instancia estática o `IHttpClientFactory`** (nunca `new HttpClient()` por petición).
✅ **Configurar `BaseAddress`** en lugar de concatenar URLs manualmente.
✅ **Configurar `Timeout`** (el default son 100s, excesivo para la mayoría de casos).
✅ **Usar `CancellationToken`** para cancelar peticiones que ya no son necesarias.
✅ **Usar `PostAsJsonAsync` / `GetFromJsonAsync`** (métodos de extensión en `System.Net.Http.Json`).

❌ **No crear `HttpClient` dentro de un bucle o método llamado frecuentemente.**
❌ **No ignorar `IsSuccessStatusCode`** — una respuesta 404 o 500 no lanza excepción automáticamente.
❌ **No usar `HttpClient` en aplicaciones de larga duración sin `IHttpClientFactory`** (problema de rotación de DNS).

---

## 6. Errores habituales

### Error 1: Crear HttpClient por cada petición

```csharp
// ❌ INCORRECTO — Socket Exhaustion en segundos
for (int i = 0; i < 10000; i++)
{
    using var client = new HttpClient();
    await client.GetAsync("https://api.example.com/data");
}
```

```csharp
// ✅ CORRECTO — Singleton o IHttpClientFactory
private static readonly HttpClient _client = new() { BaseAddress = new Uri("https://api.example.com") };

for (int i = 0; i < 10000; i++)
{
    await _client.GetAsync("/data");
}
```

### Error 2: Olvidar `PropertyNamingPolicy` al consumir APIs

```csharp
// ❌ La API espera {"userName":"Ana"} pero envías {"UserName":"Ana"}
var request = new { UserName = "Ana" };
var json = JsonSerializer.Serialize(request); // {"UserName":"Ana"} ← PascalCase
```

```csharp
// ✅ Añadir CamelCase a las opciones
var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
var json = JsonSerializer.Serialize(request, options); // {"userName":"Ana"} ← camelCase
```

### Error 3: No validar `IsSuccessStatusCode`

```csharp
// ❌ Asume que 200 OK siempre
var response = await client.GetAsync("/users/999");
var user = await response.Content.ReadFromJsonAsync<User>(); // Null si 404
```

```csharp
// ✅ Validar el código de estado
var response = await client.GetAsync("/users/999");
response.EnsureSuccessStatusCode(); // Lanza HttpRequestException si no es 2xx
var user = await response.Content.ReadFromJsonAsync<User>();
```

### Error 4: Usar `Newtonsoft.Json` sin necesitarlo

```csharp
// ❌ Dependencia externa innecesaria en proyecto nuevo
using Newtonsoft.Json;
var json = JsonConvert.SerializeObject(obj);
```

```csharp
// ✅ Usar la biblioteca incluida en el SDK
using System.Text.Json;
var json = JsonSerializer.Serialize(obj);
```

**Solo justifica Newtonsoft.Json si:**
- Migras una app legacy de .NET Framework que ya lo usa intensivamente.
- Necesitas polimorfismo complejo con `TypeNameHandling` (y no puedes usar `JsonDerivedType` de .NET 7+).

---

## 7. Ejercicio guiado

**Objetivo:** Crear un servicio que consuma una API REST y deserialice la respuesta.

### Paso 1: Definir los modelos

```csharp
public class ApiUser
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public ApiAddress? Address { get; set; }
}

public class ApiAddress
{
    public string Street { get; set; } = "";
    public string City { get; set; } = "";
}
```

### Paso 2: Configurar HttpClient y JsonSerializerOptions

```csharp
private static readonly HttpClient _client = new()
{
    BaseAddress = new Uri("https://jsonplaceholder.typicode.com"),
    Timeout = TimeSpan.FromSeconds(10)
};

private static readonly JsonSerializerOptions _jsonOptions = new()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
```

### Paso 3: Implementar GET y POST

```csharp
public async Task<List<ApiUser>> GetUsersAsync()
{
    var response = await _client.GetAsync("/users");
    response.EnsureSuccessStatusCode();
    var json = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<List<ApiUser>>(json, _jsonOptions) ?? [];
}

public async Task<ApiUser?> CreateUserAsync(ApiUser user)
{
    var json = JsonSerializer.Serialize(user, _jsonOptions);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var response = await _client.PostAsync("/users", content);
    response.EnsureSuccessStatusCode();
    var responseJson = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<ApiUser>(responseJson, _jsonOptions);
}
```

### Ejecutar:

```bash
dotnet run --project src/Demo
```

---

## 8. Reto

**Reto:** Modifica `Program.cs` para añadir una función que use `Utf8JsonReader` para contar cuántas veces aparece una clave concreta en un JSON de 10.000 líneas, SIN deserializar todo el documento. Compara el rendimiento con `JsonDocument.Parse().RootElement.GetProperty()` usando `Stopwatch`.

Pistas:
- `Utf8JsonReader` es forward-only y no asigna memoria en heap.
- `JsonDocument` sí parsea todo el documento a `JsonElement`.
- Usa `File.ReadAllBytes()` para cargar el archivo y `Stopwatch` para medir.

---

## 9. Resumen

| Concepto | Punto clave |
|---|---|
| `JsonSerializer` | Serializar/deserializar con opciones de naming y formato |
| `JsonSerializerOptions` | Configurar `PropertyNamingPolicy`, `WriteIndented`, `DefaultIgnoreCondition` |
| `JsonPropertyName` | Renombrar propiedades en JSON |
| `JsonIgnore` | Excluir propiedades (datos sensibles, calculados) |
| `JsonRequired` | Exigir que una propiedad exista en el JSON |
| `JsonConstructor` | Elegir qué constructor usar al deserializar |
| `JsonDocument` | Leer JSON sin tipos (read-only, eficiente) |
| `JsonNode` / `JsonArray` | JSON mutable dinámico (equivalente a JObject/JArray) |
| `JsonSerializerContext` | Source generator: serialización compilada, AOT-safe |
| `Utf8JsonReader/Writer` | Alto rendimiento, forward-only, sin heap allocations |
| `HttpClient` | Singleton o `IHttpClientFactory` (nunca por petición) |
| `BaseAddress` | URL base para peticiones relativas |
| `Timeout` | Tiempo máximo de espera (default 100s, ajustar siempre) |
| `CancellationToken` | Cancelar peticiones en curso |
| `StringContent` | Enviar cuerpo JSON con `application/json` |
| `IHttpClientFactory` | Gestión profesional de HttpClient (Módulo 4) |

---

## 10. Preguntas frecuentes

**P: ¿System.Text.Json o Newtonsoft.Json?**
R: System.Text.Json para todo proyecto nuevo en .NET. Solo usa Newtonsoft.Json si migras código legacy que depende de características no soportadas (polimorfismo con `TypeNameHandling`, `JObject` dinámico profundo). Para JSON dinámico básico, `JsonNode` cubre el 95% de los casos.

**P: ¿Por qué mi JSON se ve con `Id` en mayúscula si la API espera `id`?**
R: Por defecto, `JsonSerializer` usa el nombre exacto de la propiedad C#. Configura `PropertyNamingPolicy = JsonNamingPolicy.CamelCase` en las opciones o usa `[JsonPropertyName("id")]`.

**P: ¿Cada cuánto debo hacer `Dispose()` de `HttpClient`?**
R: **Nunca** (o casi nunca). Mantén una instancia estática durante toda la vida de la aplicación. Si necesitas rotación de DNS o configuraciones diferentes, usa `IHttpClientFactory`.

**P: ¿Qué pasa si no pongo `CancellationToken`?**
R: La petición seguirá ejecutándose aunque el usuario cierre la página o cancele la operación. Siempre pasa `CancellationToken` desde el controller/handler hasta `HttpClient`.

**P: ¿Puedo serializar un `record` con System.Text.Json?**
R: Sí, perfectamente. Los `record` se serializan por propiedades igual que las `class`. Con `record` posicional (`record Person(string Name, int Age)`) se usan las propiedades init-only generadas automáticamente.

**P: ¿`JsonSerializerContext` es obligatorio?**
R: No para aplicaciones normales. Es obligatorio para Native AOT y recomendado para alto rendimiento (evita reflexión). Para el 90% de aplicaciones web, la serialización por reflexión es suficientemente rápida.

**P: ¿Cómo envío un formulario o archivos con HttpClient?**
R: Usa `MultipartFormDataContent` para formularios y subida de archivos:
```csharp
using var content = new MultipartFormDataContent();
content.Add(new StringContent("valor"), "campo");
content.Add(new ByteArrayContent(bytes), "archivo", "nombre.pdf");
await client.PostAsync("/upload", content);
```
