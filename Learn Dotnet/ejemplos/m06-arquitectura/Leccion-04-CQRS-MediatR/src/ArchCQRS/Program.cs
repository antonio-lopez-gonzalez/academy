// =============================================================================
// Módulo 06 — Arquitectura y organización
// Lección 04 — CQRS con MediatR
// =============================================================================
// CQRS = Command Query Responsibility Segregation
//
// Separa las operaciones de LECTURA (Queries) de las de ESCRITURA (Commands).
//
// ┌─────────────────────────────────────────────────────────────────┐
// │                       SIN CQRS                                  │
// │                                                                 │
// │  PostService                                                    │
// │  ├── GetPost()       ← lectura                                  │
// │  ├── ListPosts()     ← lectura                                  │
// │  ├── CreatePost()    ← escritura                                │
// │  ├── UpdatePost()    ← escritura                                │
// │  └── DeletePost()    ← escritura                                │
// │                                                                 │
// │  ❌ Misma clase para leer y escribir                            │
// │  ❌ Mismo modelo para entrada y salida                          │
// │  ❌ Difícil optimizar lecturas por separado                     │
// └─────────────────────────────────────────────────────────────────┘
//
// ┌─────────────────────────────────────────────────────────────────┐
// │                       CON CQRS                                  │
// │                                                                 │
// │  COMMANDS (escritura)              QUERIES (lectura)             │
// │  ├── CreatePostCommand             ├── GetPostQuery              │
// │  │   + CreatePostHandler           │   + GetPostHandler          │
// │  ├── UpdatePostCommand             ├── ListPostsQuery            │
// │  │   + UpdatePostHandler           │   + ListPostsHandler        │
// │  └── DeletePostCommand             └── SearchPostsQuery          │
// │      + DeletePostHandler               + SearchPostsHandler      │
// │                                                                 │
// │  ✅ Separación clara lectura/escritura                          │
// │  ✅ Cada handler tiene su propio modelo de entrada/salida       │
// │  ✅ Puedes escalar/optimizar lecturas y escrituras por separado │
// │  ✅ Ideal con MediatR (patrón Mediator)                         │
// └─────────────────────────────────────────────────────────────────┘
//
// ─── MEDIATR ───
// MediatR implementa el patrón Mediator (NO confundir con Mediator de GoF).
// El Mediator actúa como un bus de mensajes interno:
//
//   Cliente → IMediator.Send(command) → Pipeline → Handler → Respuesta
//
// Pipelines (Behaviors): se ejecutan alrededor de cada handler:
//   Request → LoggingBehavior → ValidationBehavior → Handler → Respuesta
// =============================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// ═══════════════════════════════════════════════════════════════════
// PARTE 1: MEDIATOR MANUAL (sin librerías, para entender el concepto)
// ═══════════════════════════════════════════════════════════════════
namespace ArchCQRS.ManualMediator
{
    // ─── Interfaces del Mediator ───

    /// <summary>
    /// Marca un mensaje que devuelve una respuesta.
    /// </summary>
    public interface IRequest<TResponse> { }

    /// <summary>
    /// Handler que procesa un mensaje y devuelve una respuesta.
    /// </summary>
    public interface IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Pipeline behavior: se ejecuta alrededor del handler.
    /// </summary>
    public interface IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            Func<Task<TResponse>> next);
    }

    /// <summary>
    /// Mediador manual: registra handlers y behaviors, y enruta mensajes.
    /// </summary>
    public sealed class Mediator
    {
        // Diccionario: tipo de request → función que crea el handler y ejecuta
        private readonly Dictionary<Type, Func<object, CancellationToken, Task<object>>> _handlers = new();

        // Lista de factories de behaviors (se ejecutan en orden)
        private readonly List<Func<object, object>> _behaviorFactories = new();

        /// <summary>
        /// Registra un handler para un tipo de request.
        /// </summary>
        public void Register<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : IRequest<TResponse>
        {
            _handlers[typeof(TRequest)] = async (request, ct) =>
            {
                TResponse result = await handler.Handle((TRequest)request, ct);
                return result!;
            };
        }

        /// <summary>
        /// Añade un pipeline behavior.
        /// </summary>
        public void AddPipeline<TRequest, TResponse>(
            IPipelineBehavior<TRequest, TResponse> behavior)
            where TRequest : IRequest<TResponse>
        {
            _behaviorFactories.Add(request =>
            {
                // Creamos un wrapper que ejecuta el behavior
                return (Func<object, CancellationToken, Task<object>>)(async (req, ct) =>
                {
                    // Encontramos el handler original
                    Func<object, CancellationToken, Task<object>>? handlerFunc = null;
                    if (_handlers.TryGetValue(typeof(TRequest), out Func<object, CancellationToken, Task<object>>? h))
                    {
                        handlerFunc = h;
                    }

                    if (handlerFunc is null)
                        throw new InvalidOperationException($"No handler registered for {typeof(TRequest).Name}");

                    TResponse result = await behavior.Handle(
                        (TRequest)req,
                        ct,
                        async () =>
                        {
                            object innerResult = await handlerFunc(req, ct);
                            return (TResponse)innerResult;
                        });

                    return result!;
                });
            });
        }

        /// <summary>
        /// Envía un request y obtiene la respuesta. Los behaviors se ejecutan en orden.
        /// </summary>
        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            Type requestType = request.GetType();

            // Construir la cadena de ejecución: behaviors → handler
            // Partimos del handler base
            if (!_handlers.TryGetValue(requestType, out Func<object, CancellationToken, Task<object>>? execute))
            {
                throw new InvalidOperationException(
                    $"No handler registered for request type {requestType.Name}");
            }

            // Envolvemos con behaviors en orden inverso (el último se ejecuta primero)
            // En esta versión simplificada, los behaviors se registran pero el orden
            // depende de cómo los encadenemos. Para simplificar, ejecutamos el handler
            // directamente y luego mostramos cómo funcionaría con MediatR real.
            object result = await execute(request, cancellationToken);
            return (TResponse)result;
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// PARTE 2: DOMINIO Y MODELOS
// ═══════════════════════════════════════════════════════════════════
namespace ArchCQRS.Domain
{
    /// <summary>
    /// Entidad de dominio: Post.
    /// </summary>
    public sealed class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// Base de datos simulada.
    /// </summary>
    public static class InMemoryDb
    {
        public static readonly List<Post> Posts = new();
    }
}

// ═══════════════════════════════════════════════════════════════════
// PARTE 3: COMMANDS (escritura)
// ═══════════════════════════════════════════════════════════════════
namespace ArchCQRS.Features.Posts.Commands
{
    using ArchCQRS.Domain;
    using ArchCQRS.ManualMediator;

    // ─── CreatePostCommand ───

    /// <summary>
    /// Command: Crear un nuevo post.
    /// Los comandos modifican estado. Normalmente devuelven solo el ID.
    /// </summary>
    public sealed record CreatePostCommand(
        string Title,
        string Content,
        Guid AuthorId,
        List<string> Tags
    ) : IRequest<Guid>; // El comando devuelve el ID del post creado

    /// <summary>
    /// Handler del comando CreatePost.
    /// Contiene TODA la lógica de creación: validación, mapeo, persistencia.
    /// </summary>
    public sealed class CreatePostHandler : IRequestHandler<CreatePostCommand, Guid>
    {
        public Task<Guid> Handle(CreatePostCommand command, CancellationToken cancellationToken)
        {
            // 1. Validación (en producción iría en un Pipeline Behavior)
            List<string> errors = new();
            if (string.IsNullOrWhiteSpace(command.Title))
                errors.Add("El título es obligatorio.");
            if (command.Title?.Length < 3)
                errors.Add("El título debe tener al menos 3 caracteres.");
            if (command.AuthorId == Guid.Empty)
                errors.Add("El AuthorId no puede ser vacío.");

            if (errors.Count > 0)
                throw new ValidationException(errors);

            // 2. Crear entidad de dominio
            Post post = new()
            {
                Id = Guid.NewGuid(),
                Title = command.Title?.Trim() ?? string.Empty,
                Content = command.Content?.Trim() ?? string.Empty,
                AuthorId = command.AuthorId,
                CreatedAt = DateTime.UtcNow,
                Tags = command.Tags?.Select(t => t.Trim().ToLowerInvariant()).ToList() ?? new()
            };

            // 3. Persistir
            InMemoryDb.Posts.Add(post);

            // 4. Devolver ID
            return Task.FromResult(post.Id);
        }
    }

    // ─── PublishPostCommand ───

    public sealed record PublishPostCommand(Guid PostId) : IRequest<bool>;

    public sealed class PublishPostHandler : IRequestHandler<PublishPostCommand, bool>
    {
        public Task<bool> Handle(PublishPostCommand command, CancellationToken cancellationToken)
        {
            Post? post = InMemoryDb.Posts.FirstOrDefault(p => p.Id == command.PostId);
            if (post is null)
                return Task.FromResult(false);

            post.PublishedAt = DateTime.UtcNow;
            return Task.FromResult(true);
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// PARTE 4: QUERIES (lectura)
// ═══════════════════════════════════════════════════════════════════
namespace ArchCQRS.Features.Posts.Queries
{
    using ArchCQRS.Domain;
    using ArchCQRS.ManualMediator;

    // ─── DTOs de respuesta ───

    /// <summary>
    /// DTO para devolver datos de un post (nunca exponemos la entidad directamente).
    /// </summary>
    public sealed record PostDto(
        Guid Id,
        string Title,
        string Content,
        Guid AuthorId,
        DateTime CreatedAt,
        DateTime? PublishedAt,
        List<string> Tags
    );

    public sealed record PostSummaryDto(
        Guid Id,
        string Title,
        DateTime CreatedAt,
        DateTime? PublishedAt,
        int TagCount
    );

    // ─── GetPostQuery ───

    /// <summary>
    /// Query: obtener un post por ID.
    /// Las queries son solo lectura, no modifican estado.
    /// </summary>
    public sealed record GetPostQuery(Guid Id) : IRequest<PostDto?>;

    public sealed class GetPostHandler : IRequestHandler<GetPostQuery, PostDto?>
    {
        public Task<PostDto?> Handle(GetPostQuery query, CancellationToken cancellationToken)
        {
            Post? post = InMemoryDb.Posts.FirstOrDefault(p => p.Id == query.Id);
            if (post is null)
                return Task.FromResult<PostDto?>(null);

            PostDto dto = new(
                post.Id,
                post.Title,
                post.Content,
                post.AuthorId,
                post.CreatedAt,
                post.PublishedAt,
                post.Tags
            );

            return Task.FromResult<PostDto?>(dto);
        }
    }

    // ─── ListPostsQuery ───

    /// <summary>
    /// Query: listar posts con filtro opcional por tag.
    /// </summary>
    public sealed record ListPostsQuery(string? TagFilter = null) : IRequest<List<PostSummaryDto>>;

    public sealed class ListPostsHandler : IRequestHandler<ListPostsQuery, List<PostSummaryDto>>
    {
        public Task<List<PostSummaryDto>> Handle(
            ListPostsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<Post> posts = InMemoryDb.Posts;

            if (!string.IsNullOrWhiteSpace(query.TagFilter))
            {
                string tag = query.TagFilter.Trim().ToLowerInvariant();
                posts = posts.Where(p => p.Tags.Contains(tag));
            }

            List<PostSummaryDto> result = posts
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PostSummaryDto(
                    p.Id,
                    p.Title,
                    p.CreatedAt,
                    p.PublishedAt,
                    p.Tags.Count
                ))
                .ToList();

            return Task.FromResult(result);
        }
    }

    // ─── SearchPostsQuery (ejemplo con más parámetros) ───

    public sealed record SearchPostsQuery(
        string? SearchTerm = null,
        Guid? AuthorId = null,
        DateTime? FromDate = null,
        DateTime? ToDate = null
    ) : IRequest<List<PostSummaryDto>>;

    public sealed class SearchPostsHandler : IRequestHandler<SearchPostsQuery, List<PostSummaryDto>>
    {
        public Task<List<PostSummaryDto>> Handle(
            SearchPostsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<Post> posts = InMemoryDb.Posts;

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                string term = query.SearchTerm.ToLowerInvariant();
                posts = posts.Where(p =>
                    p.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    p.Content.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            if (query.AuthorId.HasValue)
            {
                posts = posts.Where(p => p.AuthorId == query.AuthorId.Value);
            }

            if (query.FromDate.HasValue)
            {
                posts = posts.Where(p => p.CreatedAt >= query.FromDate.Value);
            }

            if (query.ToDate.HasValue)
            {
                posts = posts.Where(p => p.CreatedAt <= query.ToDate.Value);
            }

            List<PostSummaryDto> result = posts
                .Select(p => new PostSummaryDto(
                    p.Id, p.Title, p.CreatedAt, p.PublishedAt, p.Tags.Count))
                .ToList();

            return Task.FromResult(result);
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// PARTE 5: EXCEPCIÓN DE VALIDACIÓN
// ═══════════════════════════════════════════════════════════════════
namespace ArchCQRS.Features.Posts
{
    public sealed class ValidationException : Exception
    {
        public List<string> Errors { get; }

        public ValidationException(List<string> errors)
            : base($"Errores de validación: {string.Join("; ", errors)}")
        {
            Errors = errors;
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// PARTE 6: DEMO PRINCIPAL
// ═══════════════════════════════════════════════════════════════════
namespace ArchCQRS
{
    using ArchCQRS.Domain;
    using ArchCQRS.Features.Posts.Commands;
    using ArchCQRS.Features.Posts.Queries;
    using ArchCQRS.ManualMediator;

    public static class Program
    {
        public static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║  Módulo 06 — Lección 04: CQRS con MediatR   ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.WriteLine();

            // ─── CONFIGURACIÓN DEL MEDIATOR MANUAL ───
            Mediator mediator = new();

            // Registrar handlers de comandos
            mediator.Register<CreatePostCommand, Guid>(new CreatePostHandler());
            mediator.Register<PublishPostCommand, bool>(new PublishPostHandler());

            // Registrar handlers de queries
            mediator.Register<GetPostQuery, PostDto?>(new GetPostHandler());
            mediator.Register<ListPostsQuery, List<PostSummaryDto>>(new ListPostsHandler());
            mediator.Register<SearchPostsQuery, List<PostSummaryDto>>(new SearchPostsHandler());

            Guid authorId = Guid.NewGuid();

            // ─── COMMANDS: Crear posts ───
            Console.WriteLine("─── COMMANDS (escritura) ───");
            Console.WriteLine();

            Guid postId1 = await mediator.Send(
                new CreatePostCommand(
                    "Guía de Clean Architecture",
                    "Las dependencias apuntan hacia el centro del círculo...",
                    authorId,
                    new List<string> { "arquitectura", "clean-code" }
                )
            );
            Console.WriteLine($"  ✅ Post creado: {postId1}");

            Guid postId2 = await mediator.Send(
                new CreatePostCommand(
                    "CQRS con MediatR en .NET 8",
                    "Separar comandos y queries mejora el diseño...",
                    authorId,
                    new List<string> { "cqrs", "mediatr", "arquitectura" }
                )
            );
            Console.WriteLine($"  ✅ Post creado: {postId2}");

            Guid postId3 = await mediator.Send(
                new CreatePostCommand(
                    "Domain-Driven Design Táctico",
                    "Entities, Value Objects y Aggregates...",
                    Guid.NewGuid(), // Otro autor
                    new List<string> { "ddd", "domain" }
                )
            );
            Console.WriteLine($"  ✅ Post creado: {postId3}");

            // ─── COMMANDS: Publicar ───
            Console.WriteLine();
            Console.WriteLine("─── COMMAND: PublishPost ───");
            Console.WriteLine();

            bool published = await mediator.Send(new PublishPostCommand(postId1));
            Console.WriteLine($"  📢 Post {postId1} publicado: {published}");

            // ─── QUERIES: Obtener post ───
            Console.WriteLine();
            Console.WriteLine("─── QUERIES (lectura) ───");
            Console.WriteLine();

            PostDto? postDto = await mediator.Send(new GetPostQuery(postId1));
            if (postDto is not null)
            {
                Console.WriteLine($"  📄 GetPost: {postDto.Title}");
                Console.WriteLine($"     Publicado: {postDto.PublishedAt:u}");
                Console.WriteLine($"     Tags: {string.Join(", ", postDto.Tags)}");
            }

            // ─── QUERIES: Listar posts ───
            Console.WriteLine();
            Console.WriteLine("─── QUERY: ListPosts ───");
            Console.WriteLine();

            List<PostSummaryDto> allPosts = await mediator.Send(new ListPostsQuery());
            Console.WriteLine($"  Total posts: {allPosts.Count}");
            foreach (PostSummaryDto p in allPosts)
            {
                string status = p.PublishedAt is not null ? "📢" : "📝";
                Console.WriteLine($"    {status} {p.Title} ({p.TagCount} tags)");
            }

            // Filtrar por tag
            Console.WriteLine();
            Console.WriteLine("  Filtrando por tag 'arquitectura':");
            List<PostSummaryDto> filtered = await mediator.Send(
                new ListPostsQuery(TagFilter: "arquitectura")
            );
            foreach (PostSummaryDto p in filtered)
            {
                Console.WriteLine($"    • {p.Title}");
            }

            // ─── QUERY: SearchPosts ───
            Console.WriteLine();
            Console.WriteLine("─── QUERY: SearchPosts ───");
            Console.WriteLine();

            List<PostSummaryDto> searchResults = await mediator.Send(
                new SearchPostsQuery(SearchTerm: "DDD")
            );
            Console.WriteLine($"  Búsqueda 'DDD': {searchResults.Count} resultado(s)");
            foreach (PostSummaryDto p in searchResults)
            {
                Console.WriteLine($"    • {p.Title}");
            }

            // ─── VALIDACIÓN ───
            Console.WriteLine();
            Console.WriteLine("─── VALIDACIÓN EN COMMAND ───");
            Console.WriteLine();

            try
            {
                await mediator.Send(
                    new CreatePostCommand("", "", Guid.Empty, new List<string>())
                );
            }
            catch (ArchCQRS.Features.Posts.ValidationException ex)
            {
                Console.WriteLine("  ❌ Errores de validación:");
                foreach (string error in ex.Errors)
                {
                    Console.WriteLine($"     {error}");
                }
            }

            // =========================================================================
            // COMPARACIÓN: CÓMO SERÍA CON MEDIATR REAL + DI
            // =========================================================================
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("  CON MEDIATR REAL + DEPENDENCY INJECTION");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("  // Program.cs con Minimal API + MediatR real:");
            Console.WriteLine("  var builder = WebApplication.CreateBuilder(args);");
            Console.WriteLine();
            Console.WriteLine("  // Registrar MediatR (escanea el assembly en busca de handlers)");
            Console.WriteLine("  builder.Services.AddMediatR(cfg => {");
            Console.WriteLine("      cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());");
            Console.WriteLine("  });");
            Console.WriteLine();
            Console.WriteLine("  // Registrar pipeline behaviors (se ejecutan en orden)");
            Console.WriteLine("  builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));");
            Console.WriteLine("  builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));");
            Console.WriteLine();
            Console.WriteLine("  var app = builder.Build();");
            Console.WriteLine();
            Console.WriteLine("  // Minimal API endpoint con CQRS:");
            Console.WriteLine("  app.MapPost(\"/api/posts\", async (CreatePostCommand command, IMediator mediator) =>");
            Console.WriteLine("  {");
            Console.WriteLine("      Guid id = await mediator.Send(command);");
            Console.WriteLine("      return Results.Created($\"/api/posts/{id}\", new { Id = id });");
            Console.WriteLine("  });");
            Console.WriteLine();
            Console.WriteLine("  app.MapGet(\"/api/posts/{id:guid}\", async (Guid id, IMediator mediator) =>");
            Console.WriteLine("  {");
            Console.WriteLine("      PostDto? post = await mediator.Send(new GetPostQuery(id));");
            Console.WriteLine("      return post is not null ? Results.Ok(post) : Results.NotFound();");
            Console.WriteLine("  });");
            Console.WriteLine();

            // =========================================================================
            // PIPELINE BEHAVIORS (ejemplo teórico de cómo se verían con MediatR real)
            // =========================================================================
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("  PIPELINE BEHAVIORS (con MediatR real)");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("  Orden de ejecución de un comando:");
            Console.WriteLine("  ┌─────────────────────────────────────────┐");
            Console.WriteLine("  │ mediator.Send(command)                  │");
            Console.WriteLine("  │   └→ LoggingBehavior                     │");
            Console.WriteLine("  │       └→ ValidationBehavior               │");
            Console.WriteLine("  │           └→ TransactionBehavior           │");
            Console.WriteLine("  │               └→ CreatePostHandler.Handle() │");
            Console.WriteLine("  └─────────────────────────────────────────┘");
            Console.WriteLine();
            Console.WriteLine("  // Ejemplo de LoggingBehavior con MediatR real:");
            Console.WriteLine("  public class LoggingBehavior<TRequest, TResponse>");
            Console.WriteLine("      : IPipelineBehavior<TRequest, TResponse>");
            Console.WriteLine("      where TRequest : IRequest<TResponse>");
            Console.WriteLine("  {");
            Console.WriteLine("      private readonly ILogger<TRequest> _logger;");
            Console.WriteLine();
            Console.WriteLine("      public async Task<TResponse> Handle(");
            Console.WriteLine("          TRequest request,");
            Console.WriteLine("          RequestHandlerDelegate<TResponse> next,");
            Console.WriteLine("          CancellationToken cancellationToken)");
            Console.WriteLine("      {");
            Console.WriteLine("          _logger.LogInformation(\"Handling {Request}\", typeof(TRequest).Name);");
            Console.WriteLine("          var sw = Stopwatch.StartNew();");
            Console.WriteLine("          TResponse response = await next();");
            Console.WriteLine("          sw.Stop();");
            Console.WriteLine("          _logger.LogInformation(");
            Console.WriteLine("              \"Handled {Request} in {Elapsed}ms\",");
            Console.WriteLine("              typeof(TRequest).Name, sw.ElapsedMilliseconds);");
            Console.WriteLine("          return response;");
            Console.WriteLine("      }");
            Console.WriteLine("  }");
            Console.WriteLine();

            // =========================================================================
            // RESUMEN
            // =========================================================================
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("  VENTAJAS DE CQRS + MEDIATR");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("  1. Separación clara lectura/escritura");
            Console.WriteLine("  2. Cada caso de uso es una clase independiente (SRP)");
            Console.WriteLine("  3. Pipelines reutilizables (logging, validation, transaction)");
            Console.WriteLine("  4. Fácil de testear (mock de IMediator o test del handler)");
            Console.WriteLine("  5. Encaja perfectamente con Vertical Slices");
            Console.WriteLine("  6. Minimal APIs pueden recibir IMediator directamente");
            Console.WriteLine();
            Console.WriteLine("  CUÁNDO USAR CQRS:");
            Console.WriteLine("  ✅ APIs con operaciones CRUD complejas");
            Console.WriteLine("  ✅ Necesitas diferentes modelos de lectura/escritura");
            Console.WriteLine("  ✅ Quieres añadir cross-cutting concerns (logging, validation)");
            Console.WriteLine();
            Console.WriteLine("  CUÁNDO NO USAR CQRS:");
            Console.WriteLine("  ❌ CRUD simple sin lógica de negocio");
            Console.WriteLine("  ❌ Aplicación muy pequeña (over-engineering)");
            Console.WriteLine("  ❌ No necesitas separar lecturas de escrituras");
            Console.WriteLine("═══════════════════════════════════════════════");
        }
    }
}
