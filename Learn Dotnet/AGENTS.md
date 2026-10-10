# Curso Completo de .NET 8 con C#

## De Cero a Arquitecto usando .NET 8 + C# 12

---

# Objetivo

Actúa como un profesor senior de .NET con muchos años de experiencia formando desarrolladores.

El objetivo es crear **un curso completo de .NET 8**, pensado para un desarrollador que ya sabe programar (por ejemplo JavaScript, Python, Java, etc.) pero que **nunca ha aprendido .NET correctamente**.

El curso debe ser extremadamente didáctico.

No quiero únicamente aprender a escribir C#; quiero entender **por qué funciona**, cómo está diseñado el runtime, el CLR, el GC, los principios SOLID aplicados, la inyección de dependencias por dentro, y cuáles son las mejores prácticas profesionales.

---

# Tecnologías

Utilizar SIEMPRE:

- .NET 8 (LTS) + C# 12
- ASP.NET Core (Minimal APIs y Controller-based con `ControllerBase`)
- Entity Framework Core 8
- PostgreSQL + Npgsql
- xUnit + Moq + FluentAssertions + Coverlet + FsCheck
- Serilog + OpenTelemetry
- FluentValidation
- MediatR + Scrutor
- AutoMapper (o mapeo manual)
- Docker / Docker Compose
- JWT / Identity
- SignalR + Redis Backplane
- Redis (caching distribuido)
- Swagger / Scalar
- Polly (resiliencia)
- BenchmarkDotNet
- Hangfire / Quartz.NET
- Visual Studio Code / Rider
- `dotnet` CLI

No utilizar:

- .NET Framework (solo .NET 8+)
- Web Forms
- WCF
- NancyFX
- Newtonsoft.Json (usar System.Text.Json)
- Base de datos SQLite para producción
- Windows-only features
- Blazor (ni server ni wasm)
- .NET MAUI
- Razor Pages / MVC con vistas `.cshtml` (SÍ se usa `ControllerBase` para API, NO vistas Razor)

---

# Forma de enseñar

Cada lección debe seguir exactamente esta estructura.

## 1. Introducción

Explicar qué vamos a aprender.

Por qué existe.

Qué problema resuelve.

Cuándo se utiliza.

Errores típicos.

---

## 2. Teoría

Explicar el concepto con mucho detalle.

NO asumir conocimientos previos de .NET.

Utilizar comparaciones del mundo real (Java, Node, Python).

Explicar el porqué de cada decisión de diseño en el runtime/clase.

Incluir diagramas ASCII cuando sea útil.

---

## 3. Ejemplo mínimo

Mostrar el ejemplo más pequeño posible.

Explicar línea por línea.

Ejecutable con `dotnet run`.

---

## 4. Ejemplo real

Crear un ejemplo más cercano a una aplicación profesional.

Explicar todas las decisiones.

---

## 5. Buenas prácticas

Explicar:

- qué hacer
- qué evitar
- por qué

---

## 6. Errores habituales

Mostrar ejemplos incorrectos.

Explicar por qué están mal.

Mostrar la versión correcta.

---

## 7. Ejercicio guiado

Crear un ejercicio.

No demasiado difícil.

Resolverlo paso a paso.

---

## 8. Reto

Proponer un reto adicional.

NO resolverlo inmediatamente.

Esperar a que el alumno lo intente.

---

## 9. Resumen

Resumen de la lección.

Conceptos importantes.

---

## 10. Preguntas frecuentes

Añadir preguntas frecuentes relacionadas con la lección.

---

# Código

Todo el código debe estar completamente comentado.

Cada línea importante debe explicarse.

Los identificadores de código en inglés (estándar profesional).

Comentarios y UI en español.

---

# Organización

Cada módulo debe estar dividido en lecciones.

Cada lección debe ser independiente.

---

# Proyecto durante el curso

Durante todo el curso construiremos una aplicación profesional.

Será un **Sistema de Gestión de Contenidos (CMS)** similar a WordPress pero como API headless.

La iremos construyendo poco a poco.

Cada módulo añadirá funcionalidades nuevas.

Al terminar el curso tendremos una aplicación completa.

Frontend: solo API + Swagger (sin SPA, sin Blazor, sin vistas Razor).

---

# Índice del curso

## Módulo 0 — Preparación

- ¿Qué es .NET y C#?
- .NET SDK vs .NET Runtime
- CLR, IL y JIT (C# → IL → JIT → máquina)
- Versiones de .NET: .NET moderno vs .NET Framework
- Instalación del SDK y verificación
- `dotnet` CLI: `new`, `build`, `run`, `restore`
- Estructura de un proyecto `.csproj`
- Primer programa: `Program.cs`, top-level statements, `Console.WriteLine`
- Comentarios, sintaxis y convenciones de nombres

---

## Módulo 1 — Fundamentos de C#

- Variables y tipos de datos (`var`, `const`, tipos numéricos, `bool`, `char`, `string`)
- Conversión de tipos (`Convert`, `Parse`, `TryParse`, casting)
- Operadores (aritméticos, lógicos, ternario, `??`, `??=`, `?.`)
- Control de flujo (`if/else`, `switch`, switch expressions, bucles, `break`/`continue`)
- Métodos (parámetros, retorno, opcionales, con nombre, sobrecarga, `ref`/`out`/`in`)
- Strings (interpolación, verbatim, raw strings, inmutabilidad, `StringBuilder`)
- Arrays (índices, multidimensionales, jagged)
- Colecciones (`List`, `Dictionary`, `HashSet`, `Queue`, `Stack`)
- Enumeraciones (`enum`, flags)
- Structs y tipos por valor (`readonly struct`, `record struct`)
- Nullable (`int?`, `string?`, `#nullable`)
- Excepciones (`try`/`catch`/`finally`, `throw`)
- Genéricos (`T`, restricciones `where`)
- Delegados y funciones (`Action`, `Func`, `Predicate`, lambdas)
- `IEnumerable` y enumeración (`yield return`, ejecución diferida)
- LINQ (operadores, deferred execution)
- Archivos y directorios + JSON (`File`, `Directory`, `Stream`, `System.Text.Json`)
- Fechas y horas (`DateTime`, `DateOnly`, `TimeOnly`, `TimeSpan`)
- Pattern Matching (patrones de tipo, propiedad, relacionales, lógicos)
- Memoria y GC (stack/heap, boxing, generaciones, `IDisposable`/`using`)
- Asincronía (`Task`, `async`/`await`, `WhenAll`, `CancellationToken`)
- Ecosistema .NET (projects, NuGet, configuración, logging con `ILogger`)

---

## Módulo 2 — POO en C#

- Introducción a la POO (objetos, clases, estado/comportamiento, los 4 pilares)
- Clases y objetos (`new`, campos, métodos, propiedades, referencias)
- Constructores (por defecto, personalizados, sobrecarga, `this(...)`, primarios)
- Encapsulación (modificadores de acceso, setters privados, APIs de clase)
- Propiedades (automáticas, completas, calculadas, `get`/`set`/`init`, backing fields)
- Miembros y clases `static`
- Herencia (clase base/derivada, `protected`, `base`, relación "is-a")
- Polimorfismo (`virtual`, `override`, dynamic dispatch, `abstract`, `new`)
- Clases abstractas (métodos/propiedades abstractas, constructores)
- Interfaces (contratos, implementación múltiple, explícita, default methods)
- Composición (relación "has-a", delegación, composición sobre herencia)
- Sobrecarga (overload) vs sobrescritura (override)
- Records (`record`, `record struct`, inmutabilidad, `with`, DTOs, value objects)
- Igualdad y objetos (`Equals`, `GetHashCode`, `IEquatable<T>`, records)
- Características avanzadas (nested, partial, sealed, extension methods)

Proyecto:
Sistema de pedidos (bloque final) con clases, encapsulación, composición e interfaces.

---

## Módulo 3 — SOLID en C#

- Introducción a SOLID (cohesión, acoplamiento, code smells, refactoring)
- S — SRP (una clase, una razón de cambio; separar responsabilidades)
- O — OCP (abierto a extensión, cerrado a modificación; Strategy Pattern)
- L — LSP (derivadas sustituibles; contrato, pre/post condiciones; Square/Rectangle)
- I — ISP (interfaces pequeñas y cohesivas; no depender de lo que no se usa)
- D — DIP (depender de abstracciones; DI, Constructor Injection, IoC, Service Locator)
- SOLID en conjunto (relación entre principios, qué NO significa, KISS/DRY/YAGNI)
- Refactoring aplicando SOLID (de una god class a un diseño desacoplado)
- Proyecto final: sistema de pedidos con POO + SOLID

---

## Módulo 4 — C# Avanzado

- `Span<T>`, `ReadOnlySpan<T>`, `Memory<T>` (stackalloc, slicing sin copia)
- `unsafe` y punteros (cuándo tiene sentido, marshalling)
- `ref struct`, `ref return`, `in` parameters
- `IAsyncEnumerable<T>` y streaming asíncrono
- Source Generators (cómo funcionan, ejemplo real que genera DTOs)
- `InterpolatedStringHandler` (C# 10+)
- `CallerArgumentExpression`, `CallerMemberName`, `CallerFilePath`
- `Regex` Source Generator (C# 11+)
- `file` scoped types (C# 11+)
- `required` members y `init` accessors
- `FrozenSet<T>`, `FrozenDictionary<TKey, TValue>` (colecciones inmutables de alto rendimiento, .NET 8)
- `TimeProvider` (abstracción del tiempo testeable, .NET 8)
- `Keyed Services` en DI (`[FromKeyedServices]`, .NET 8)
- `HttpClientFactory` + `IHttpClientFactory` (manejo de sockets, pooling, named/typed clients)

Proyecto:
Source Generator que genera DTOs a partir de entidades del CMS.

---

## Módulo 5 — Inyección de Dependencias

- ¿Qué es DI? El problema que resuelve (acoplamiento, testabilidad)
- DI sin contenedor: Poor Man's DI (factory manual)
- Service Locator (anti-patrón: por qué evitarlo)
- El contenedor de DI de .NET: cómo funciona por dentro
- `IServiceCollection`, `ServiceDescriptor`, `ServiceProvider`
- Lifetime scopes: Singleton, Scoped, Transient (cuándo cada uno + peligros)
- Captive Dependencies (el error más común con Scoped dentro de Singleton)
- Options Pattern (`IOptions<T>`, `IOptionsSnapshot<T>`, `IOptionsMonitor<T>`)
- Registerar decorators con DI (`Scrutor`)
- Open Generics en DI
- Comparación con contenedores externos: Autofac, Castle Windsor

Proyecto:
Registro y configuración de todos los servicios del CMS con DI.

---

## Módulo 6 — Arquitectura y organización

- Clean Architecture (capas, regla de dependencia)
- Domain-Driven Design táctico: Entities, Value Objects, Aggregates, Repositories
- Vertical Slices (alternativa a Clean Architecture, cuándo usar cada una)
- Separación física en proyectos (`.csproj` vs carpetas)
- CQRS con MediatR (comandos y queries separados)
- `MediatR` pipelines: logging, validación, transacciones
- `Result<T>` pattern vs excepciones para flujo normal
- Error handling consistente

Proyecto:
Estructura final del CMS con Clean Architecture + CQRS + MediatR.

---

## Módulo 7 — Entity Framework Core

- ¿Qué es un ORM? ¿Por qué existe?
- DbContext, DbSet, lifetime en DI
- Code-First vs Database-First
- Migraciones (`add`, `update`, `remove`, `script`, `bundle`)
- Fluent API vs Data Annotations
- Relaciones (1:1, 1:N, N:N) y cascading deletes
- Propiedades de navegación
- Owned Entities (Value Objects en BD)
- Table Per Hierarchy (TPH) vs Table Per Type (TPT) vs Table Per Concrete (TPC)
- Shadow Properties y `PropertyBuilder.ValueGeneratedOnAdd`
- Lazy Loading vs Eager Loading vs Explicit Loading
- `AsNoTracking`, `AsSplitQuery`, `AsNoTrackingWithIdentityResolution`
- `AsAsyncEnumerable` para streaming
- Transacciones y `SaveChanges`
- `SaveChangesInterceptor` y `IDbContextInterceptor`
- Ejecución diferida vs ejecución inmediata (`IQueryable` vs `IEnumerable`)
- Compiled Queries (`EF.CompileQuery`)
- Raw SQL y Stored Procedures
- Migraciones en producción (seguro vs peligroso, `dotnet ef migrations bundle`)
- Performance: N+1 query, selección de columnas, paginación, `ToQueryString()`
- Concurrencia optimista con `[ConcurrencyCheck]` / `IsRowVersion`
- `EnumToStringConverter` / `ValueConverter<T>`

Proyecto:
DbContext + repositorios del CMS con todas las entidades.

---

## Módulo 8 — ASP.NET Core Web API

- Minimal APIs vs Controller-based (cuándo cada una)
- Routing (convencional y atributos)
- Model Binding y Validation
- `[FromQuery]`, `[FromRoute]`, `[FromBody]`, `[FromServices]`
- Status Codes HTTP (200, 201, 204, 400, 401, 403, 404, 409, 422, 500)
- `IResult` / `TypedResults` vs `IActionResult` vs `IActionResult<T>`
- Content Negotiation (JSON, XML)
- Global Exception Handling con `IExceptionHandler`
- Problem Details (RFC 9457 / RFC 7807): `ProblemDetails`, `ValidationProblemDetails`
- `System.Text.Json` en profundidad:
  - Custom `JsonConverter<T>`
  - `JsonSerializerContext` (source generation)
  - Polimorfismo: `[JsonPolymorphic]`, `[JsonDerivedType]`
  - `JsonIgnore`, `JsonPropertyName`, `JsonRequired`
  - `ReferenceHandler.IgnoreCycles` (evitar ciclos en JSON)
- Logging con `ILogger<T>` y Serilog (structured logging, enrichment, sinks)
- Swagger / Scalar (personalizar, anotaciones, ejemplos)
- API Versioning:
  - URL path versioning (`/api/v1/posts`, `/api/v2/posts`)
  - Query string versioning
  - Header versioning
  - Media type versioning
- `ApiVersionSet`, `MapApiVersion`, `[ApiVersion]`, `[MapToApiVersion]`
- Deprecación de versiones

Proyecto:
Endpoints CRUD del CMS (`POST /api/v1/posts`, `GET /api/v1/posts/{id}`, etc.).

---

## Módulo 9 — Validación y mapeo

- FluentValidation
- Validación en capa de aplicación vs en API
- Custom validators
- Validación condicional
- AutoMapper vs mapeo manual (comparación honesta, cuándo cada uno)
- Mapping con extension methods
- `Result<T>` con errores de validación
- `OneOf` / `FluentResults` / `ErrorOr`

Proyecto:
Validación de todas las requests del CMS con FluentValidation.

---

## Módulo 10 — Middleware y pipelines

- ¿Qué es un middleware?
- Pipeline HTTP de ASP.NET Core
- Middleware personalizado con `IMiddleware`
- `app.Use()`, `app.Map()`, `app.When()`
- Rate Limiting (`AddRateLimiter`, `SlidingWindowLimiter`)
- CORS
- Request/Response logging middleware
- Correlation ID (trazabilidad entre servicios)
- Health Checks (`MapHealthChecks`)
- `UseExceptionHandler` vs `IExceptionHandler`

Proyecto:
Middleware de logging, correlation ID y rate limiting para el CMS.

---

## Módulo 11 — Autenticación y autorización

- JWT: estructura (header, payload, signature), firma, expiración
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- Identity vs custom JWT (cuándo Identity, cuándo custom)
- `[Authorize]`, `[AllowAnonymous]`
- Policies y Claims (claims personalizados, policies basadas en claims)
- Roles vs Claims vs Policies
- Refresh Tokens (rotation, revocación)
- Hash de contraseñas con `BCrypt.Net`
- `PasswordHasher<TUser>` de Identity

Proyecto:
Autenticación del CMS con JWT + roles (Admin, Editor, Author).

---

## Módulo 12 — Testing

- xUnit: `[Fact]`, `[Theory]`, `[InlineData]`, `[ClassData]`, `[MemberData]`
- Moq: `Mock<T>`, `Setup`, `Verify`, `Callback`, `ReturnsAsync`, `Protected`
- FluentAssertions: `Should()`, `BeEquivalentTo`, `HaveXxx`
- Test de servicios con DI mockeada
- Test de endpoints Minimal API / Controllers
- Integration Tests con `WebApplicationFactory` + `TestContainers`
- `ITestOutputHelper` e integración con Serilog
- Snapshot Testing con `Verify` (aprobación visual de objetos complejos)
- Property-Based Testing con `FsCheck` (generar casos aleatorios)
- Contract Testing con `Pact` (compatibilidad entre API cliente/servidor)
- Code Coverage con `coverlet` + `reportgenerator`
- Naming conventions: `[Method]_[Scenario]_[ExpectedResult]`
- `TimeProvider` en tests (controlar el tiempo sin `Thread.Sleep`)
- `HttpClient` testing con `MockHttp` / `WireMock.Net`
- Health Checks testing

Proyecto:
Suite completa de tests del CMS (unitarios + integración + contract).

---

## Módulo 13 — Caching y rendimiento

- Caching en memoria (`IMemoryCache`, `MemoryCache`)
- Caching distribuido con Redis (`IDistributedCache`)
- Output Caching (`.NET 8`)
- Response Caching
- ETags y `If-None-Match`
- Compresión de respuestas
- Connection Pooling (Npgsql)
- Query Optimization con EF Core (analizar SQL generado)
- Benchmarking con BenchmarkDotNet
- `dotnet counters` y `dotnet trace`

Proyecto:
Caching de posts y categorías en Redis + Output Caching.

---

## Módulo 14 — Tiempo real

- SignalR: WebSockets en .NET
- Hubs, Groups, Connections
- Cliente JavaScript (SignalR @microsoft/signalr)
- Escenarios: notificaciones, chat, tableros colaborativos
- Escalar SignalR con Redis Backplane
- Comparación: WebSockets vanilla vs SignalR vs SSE

Proyecto:
Notificaciones en tiempo real para el CMS (nuevo post, comentario).

---

## Módulo 15 — Background Jobs

- `IHostedService`, `BackgroundService`
- Hangfire (`AddHangfireServer`, `RecurringJob`)
- Quartz.NET (`IJob`, `ISchedulerFactory`)
- Escenarios: enviar emails, limpiar archivos, generar sitemaps
- `Channel<T>` para productor/consumidor en memoria
- Integración con el DI container

Proyecto:
Job programado que publica posts automáticos + envío de emails.

---

## Módulo 16 — Docker y despliegue

- Dockerfile multi-stage para .NET (Build + Publish + Runtime)
- `docker-compose.yml` con API + PostgreSQL + Redis
- Variables de entorno y `appsettings.Docker.json`
- `.dockerignore`
- Publicación: `dotnet publish`, `docker build`, `docker push`
- CI/CD básico con GitHub Actions
- Despliegue en Railway / Azure Container Apps / AWS ECS
- HTTPS en producción con reverse proxy (nginx / Caddy)
- `dotnet monitor` y métricas en contenedor

Proyecto:
Docker Compose + CI/CD del CMS.

---

## Módulo 17 — Temas avanzados y producción

- Resileriencia con Polly:
  - `RetryPolicy` (retry con backoff exponencial)
  - `CircuitBreakerPolicy` (cortar llamadas cuando el servicio falla)
  - `TimeoutPolicy` (evitar llamadas que cuelgan)
  - `BulkheadPolicy` (limitar concurrencia)
  - `PolicyHttpMessageHandler` + `HttpClientFactory`
- Configuration en profundidad:
  - Jerarquía: `appsettings.json` → User Secrets → Environment Variables → Azure App Configuration
  - `IConfiguration`, `IConfigurationSection`, `IConfigurationRoot`
  - Options pattern: `IOptions<T>`, `IOptionsSnapshot<T>`, `IOptionsMonitor<T>`
  - Named options con `Configure<T>(name, …)`
  - `PostConfigure`, `ValidateOnStart`, `ValidateDataAnnotations`
- Feature Flags (`Microsoft.FeatureManagement`)
- OpenTelemetry en profundidad:
  - Trazas distribuidas (`ActivitySource`, `ActivityListener`)
  - Métricas (`Meter`, `Counter`, `Histogram`)
  - Exportadores: `OpenTelemetry.Exporter.Console`, `OpenTelemetry.Exporter.OpenTelemetryProtocol`
  - Integración con ASP.NET Core, EF Core, HttpClient
- System.Text.Json avanzado (ya cubierto en módulo 8, profundizar)
- gRPC en .NET:
  - Protobuf, servicios unarios/server streaming/client streaming/bidi streaming
  - gRPC-Web para clientes JS
  - gRPC vs SignalR (cuándo cada uno)
- Minimal APIs avanzadas:
  - Grupos de rutas (`MapGroup`)
  - Filtros (`IEndpointFilter`)
  - Bind personalizados (`BindAsync`)
  - `TypedResults` vs `Results`

Proyecto:
Endpoints gRPC + telemetría + Polly + Feature Flags para el CMS.

---

## Módulo 18 — Seguridad en APIs (OWASP Top 10)

- A01: Broken Object Level Authorization — validar que el usuario accede a SU recurso
- A02: Broken Authentication — JWT bien configurado, rotación de tokens
- A03: Broken Object Property Level Authorization — no exponer campos internos
- A04: Unrestricted Resource Consumption — rate limiting, paginación obligatoria
- A05: Broken Function Level Authorization — policies por rol/claim
- A06: Unrestricted Access to Sensitive Business Flows — anti-automation
- A07: Server Side Request Forgery (SSRF) — no permitir URLs arbitrarias
- A08: Security Misconfiguration — CORS, headers HTTP (CSP, HSTS, X-Content-Type-Options)
- A09: Improper Inventory Management — versionado, deprecación
- A10: Unsafe Consumption of APIs — validar todo input, nunca confiar en el cliente
- SQL Injection: cómo EF Core la previene automáticamente (parameters) y cómo romperla con Raw SQL
- XSS: cómo se previene en APIs (JSON no ejecuta scripts)
- CSRF: por qué las APIs con JWT (stateless) no necesitan anti-CSRF
- Rate Limiting en profundidad: `SlidingWindow`, `Concurrency`, `TokenBucket`
- Helmet.js equivalente en .NET: cabeceras de seguridad con middleware
- Secret scanning: `dotnet user-secrets`, Azure Key Vault, GitHub Secrets

Proyecto:
Auditoría de seguridad del CMS + fix de todas las vulnerabilidades encontradas.

---

# Estructura del repositorio

```text
curso-dotnet/
│
├── AGENTS.md
├── README.md
│
├── web/                       ← Curso estático (HTML + CSS + JS)
│   ├── index.html
│   ├── assets/css/
│   ├── assets/js/
│   ├── modulos/
│   │   ├── 00-preparacion/ … 17-avanzados/
│   └── recursos/
│
├── ejemplos/                  ← mini-proyectos `dotnet new` por módulo
│   ├── m01-csharp-fundamentos/
│   ├── m02-poo/
│   └── ...
│
├── proyectos/                 ← proyecto práctico de cada módulo
│   ├── m01-cms-models/
│   ├── m02-cms-services/
│   └── ...
│
└── proyecto-final/
    ├── src/
    │   ├── Cms.Domain/
    │   ├── Cms.Application/
    │   ├── Cms.Infrastructure/
    │   └── Cms.Api/
    ├── tests/
    ├── docker-compose.yml
    └── .github/workflows/
```

---

# Buenas prácticas

Explicar siempre:

- cuándo utilizar cada solución
- ventajas
- inconvenientes
- alternativas
- rendimiento
- mantenibilidad
- escalabilidad

---

# Convenciones del curso

- Idioma: contenido, comentarios y UI en español.
- Identificadores de código en inglés.
- Versiones fijadas con caret: .NET 8, C# 12.
- Cada lección sigue SIEMPRE las 10 secciones definidas en "Forma de enseñar".
- Usar únicamente `dotnet` CLI en los comandos.
- Todo el código debe ejecutarse.

---

# Reglas importantes

1. Nunca des por sentado un concepto.
2. Explica siempre el "por qué".
3. Todo el código debe compilar y ejecutarse.
4. Sigue las buenas prácticas actuales de .NET 8.
5. No omitas pasos de configuración.
6. Mantén una progresión lógica entre las lecciones.
7. Cada módulo debe terminar con un proyecto práctico.
8. Reutiliza el proyecto CMS a lo largo del curso.
9. Incluye consejos de debugging (`dotnet counters`, VS Debugger, logging, `ILogger`).
10. Prioriza la comprensión profunda frente a la velocidad.

---

# Comandos del proyecto

```bash
# Build de todos los proyectos de ejemplo
dotnet build ejemplos/**/*.csproj

# Build de todos los proyectos del CMS
dotnet build proyectos/**/*.csproj

# Build del proyecto final
dotnet build proyecto-final/**/*.csproj

# Test de todos los proyectos
dotnet test ejemplos/**/*.csproj
dotnet test proyectos/**/*.csproj
dotnet test proyecto-final/**/*.csproj

# Formatear código automáticamente
dotnet format

# Verificar formato sin aplicar cambios
dotnet format --verify-no-changes
```

Antes de dar una lección por terminada, ejecutar siempre `dotnet build` sobre el proyecto correspondiente. Si hay tests, ejecutar `dotnet test`.

---

# Workflow para crear lecciones

Cada vez que se crea una nueva lección, seguir este orden:

## 1. Verificar estructura de directorios
Asegurarse de que existe la carpeta del módulo en `ejemplos/` y/o `proyectos/`.

## 2. Crear proyecto con `dotnet new`
Siempre usar la CLI, nunca crear `.csproj` manualmente:

```bash
dotnet new console -n MiLeccion -o ejemplos/mXX-tema/Leccion-NN-Nombre/src/MiLeccion
dotnet new xunit -n MiLeccion.Tests -o ejemplos/mXX-tema/Leccion-NN-Nombre/tests/MiLeccion.Tests
```

Agregar el proyecto a una solución si el módulo la tiene:

```bash
dotnet sln ejemplos/mXX-tema/mXX.sln add ejemplos/mXX-tema/Leccion-NN-Nombre/src/MiLeccion
```

## 3. Implementar la lección
Seguir las 10 secciones definidas en "Forma de enseñar":
- Introducción, Teoría, Ejemplo mínimo, Ejemplo real, Buenas prácticas
- Errores habituales, Ejercicio guiado, Reto, Resumen, Preguntas frecuentes

Si la lección es larga, el README.md contiene el texto y el proyecto de consola contiene el código ejecutable.

## 4. Verificar que compila

```bash
dotnet build ejemplos/mXX-tema/Leccion-NN-Nombre
```

## 5. Ejecutar el ejemplo

```bash
dotnet run --project ejemplos/mXX-tema/Leccion-NN-Nombre/src/MiLeccion
```

## 6. Ejecutar tests (si existen)

```bash
dotnet test ejemplos/mXX-tema/Leccion-NN-Nombre/tests/MiLeccion.Tests
```

## 7. Marcar progreso
Actualizar la sección "Módulo actual y progreso" de este mismo archivo.

---

# Reglas para el asistente AI

- **Verificar siempre** el estado actual del repo (`ls`, `git status`) antes de actuar. No asumir nada.
- **No asumir proyectos existentes.** Si una lección necesita un proyecto y no existe, crearlo con `dotnet new`.
- **Proyectos independientes por lección.** Cada lección tiene su propio `.csproj` dentro de `ejemplos/mXX-tema/Leccion-NN/`. No compartir proyectos entre lecciones.
- **Usar `ejemplos/`** para ejemplos mínimos y demostraciones de conceptos.
- **Usar `proyectos/`** para el proyecto CMS incremental (el que se construye a lo largo del curso).
- **Idioma:** los mensajes al usuario (y los comentarios en código) van en español. El código (identificadores, clases, métodos, variables) va en inglés.
- **Explicar cada comando `dotnet`** que se ejecute. Decir qué hace y por qué.
- **No borrar código existente** a menos que sea necesario para la corrección. Preguntar antes.
- **Preferir `dotnet run --project`** sobre `cd` + `dotnet run`.
- **No mezclar módulos.** Cada módulo es autocontenido en su carpeta.
- **No modificar el índice del curso** (la lista de módulos) sin preguntar.

---

# Estructura de una lección en disco

```text
ejemplos/mXX-tema/
├── mXX.sln                              ← Solución del módulo (opcional)
│
├── Leccion-01-Nombre-del-Tema/
│   ├── src/
│   │   └── MiProyecto/
│   │       ├── MiProyecto.csproj
│   │       └── Program.cs
│   ├── tests/                           ← Opcional
│   │   └── MiProyecto.Tests/
│   │       ├── MiProyecto.Tests.csproj
│   │       └── UnitTest1.cs
│   └── README.md                        ← Texto de la lección (10 secciones)
│
├── Leccion-02-Otro-Tema/
│   └── ...
│
└── recursos/                            ← Imágenes, diagramas, etc.
```

Para el proyecto del CMS:

```text
proyectos/mXX-cms-algo/
├── src/
│   ├── Cms.XXX/                        ← Proyecto de la capa correspondiente
│   │   ├── Cms.XXX.csproj
│   │   └── ...
│   └── ...
├── tests/
│   └── Cms.XXX.Tests/
│       ├── Cms.XXX.Tests.csproj
│       └── ...
└── README.md
```

---

# Convenciones de código para la IA

| Regla | Valor |
|---|---|
| **Lenguaje de código** | Inglés (clases, métodos, variables) |
| **Lenguaje de comentarios** | Español |
| **Nullables** | Activado (`<Nullable>enable</Nullable>`) |
| **ImplicitUsings** | Activado (`<ImplicitUsings>enable</ImplicitUsings>`) |
| **Namespaces** | File-scoped (`namespace X.Y;`) |
| **Constructores** | Primary constructors (C# 12) cuando sea posible |
| **`var`** | Solo cuando el tipo es obvio (`new`, cast, LINQ) |
| **Tests** | xUnit + FluentAssertions + Moq |
| **Logging** | `ILogger<T>`, nunca `Console.WriteLine` |
| **JSON** | System.Text.Json, nunca Newtonsoft.Json |
| **HTTP** | `HttpClientFactory`, nunca `new HttpClient()` |
| **Strings** | `string` interpolation sobre concatenación |
| **Patrones** | Pattern matching (`is`, `switch`) preferido sobre `if` anidados |
| **Async** | Async/Await con `Task<T>`, evitar `.Result` o `.Wait()` |
| **Dispose** | `using` declaration (`using var x = new();`) |

---

# Módulo actual y progreso

<!-- Marcador de progreso del curso. La IA actualiza esto al completar cada módulo. -->

| Módulo | Estado | Lecciones HTML |
|---|---|---|
| M00 — Preparación | ✅ Completado | 01-que-es-dotnet-csharp, 02-instalacion-primer-proyecto, 03-primer-programa |
| M01 — Fundamentos de C# | ✅ Completado | 01-variables-tipos, 02-operadores, 03-control-de-flujo, 04-metodos, 05-strings, 06-arrays, 07-colecciones, 08-enumeraciones, 09-structs-tipos-valor, 10-nullable, 11-excepciones, 12-genericos, 13-delegados-funciones, 14-ienumerable, 15-linq, 16-archivos-json, 17-fechas-horas, 18-pattern-matching, 19-memoria-gc, 20-asincronia, 21-ecosistema-dotnet |
| M02 — POO en C# | ✅ Completado | 01-introduccion-poo, 02-clases-objetos, 03-constructores, 04-encapsulacion, 05-propiedades, 06-static, 07-herencia, 08-polimorfismo, 09-clases-abstractas, 10-interfaces, 11-composicion, 12-sobrecarga-sobrescritura, 13-records, 14-igualdad, 15-clases-avanzadas |
| M03 — SOLID en C# | ✅ Completado | 01-introduccion-solid, 02-srp, 03-ocp, 04-lsp, 05-isp, 06-dip, 07-solid-conjunto, 08-refactoring, 09-proyecto-final |
| M04 — C# Avanzado | ✅ Completado | 01-span-memory, 02-unsafe-punteros, 03-source-generators, 04-frozenset-timerprovider, 05-httpclient-factory, 06-proyecto-cms-dto-generator |
| M05 — Inyección de Dependencias | ✅ Completado | 01-que-es-di, 02-di-container, 03-lifetimes, 04-options-pattern, 05-scrutor-decorators, 06-proyecto-cms-di |
| M06 — Arquitectura y organización | ✅ Completado | 01-clean-architecture, 02-ddd-tactico, 03-vertical-slices, 04-cqrs-mediatr, 05-result-pattern, 06-proyecto-cms-arquitectura |
| M07 — Entity Framework Core | ✅ Completado | 01-que-es-orm, 02-code-first-migrations, 03-relaciones, 04-consultas-rendimiento, 05-transacciones-concurrencia, 06-proyecto-cms-dbcontext |
| M08 — ASP.NET Core Web API | ✅ Completado | 01-minimal-apis-vs-controllers, 02-routing-binding, 03-status-codes-results, 04-exception-handling, 05-system-text-json, 06-swagger-versioning, 07-proyecto-cms-endpoints |
| M09 — Validación y mapeo | ✅ Completado | 01-fluent-validation, 02-automapper-vs-manual, 03-result-pattern, 04-proyecto-cms-validacion |
| M10 — Middleware y pipelines | ✅ Completado | 01-pipeline-http, 02-middleware-personalizado, 03-rate-limiting-cors, 04-correlation-id-health, 05-proyecto-cms-middleware |
| M11 — Autenticación y autorización | ✅ Completado | 01-jwt-estructura, 02-identity-vs-custom, 03-policies-roles, 04-refresh-tokens, 05-proyecto-cms-auth |
| M12 — Testing | ✅ Completado | 01-xunit-fundamentos, 02-integration-tests, 03-property-snapshot-testing, 04-code-coverage, 05-proyecto-cms-testing |
| M13 — Caching y rendimiento | ✅ Completado | 01-memory-cache, 02-redis-distributed-cache, 03-output-caching, 04-benchmarking, 05-proyecto-cms-caching |
| M14 — Tiempo real | ✅ Completado | 01-signalr-fundamentos, 02-hubs-grupos, 03-redis-backplane, 04-proyecto-cms-signalr |
| M15 — Background Jobs | ✅ Completado | 01-hosted-service, 02-hangfire, 03-quartz-net, 04-channel, 05-proyecto-cms-jobs |
| M16 — Docker y despliegue | ✅ Completado | 01-dockerfile-multistage, 02-docker-compose, 03-github-actions, 04-proyecto-cms-docker |
| M17 — Temas avanzados y producción | ✅ Completado | 01-polly-resiliencia, 02-opentelemetry, 03-grpc, 04-feature-flags, 05-minimal-apis-avanzadas, 06-proyecto-cms-avanzado |
| M18 — Seguridad en APIs (OWASP) | ✅ Completado | 01-owasp-top10, 02-sql-injection, 03-xss-csrf, 04-rate-limiting-profundo, 05-secret-scanning, 06-proyecto-cms-seguridad |

**Recursos web:** ✅ Completados (cheatsheet, faq, glosario)

**Proyecto CMS (web):** ✅ Completado — 14 lecciones de proyecto repartidas por módulos M00-M18

<!-- Cuando un módulo se completa, cambiar ❌ a ✅ y añadir las lecciones impartidas. Ejemplo:
| M00 — Preparación | ✅ Completado | 01-Instalacion, 02-CLI, 03-Estructura |
-->
