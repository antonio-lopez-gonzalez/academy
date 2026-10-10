// =============================================================================
// Módulo 06 — Arquitectura y organización
// Lección 03 — Vertical Slices
// =============================================================================
// Dos formas de organizar una aplicación:
//
// ┌──────────────────────────────────────────────────────────────────┐
// │  CLEAN ARCHITECTURE (horizontal)        VERTICAL SLICES          │
// │                                      (vertical por feature)      │
// │                                                                  │
// │  Presentation/                      Features/                    │
// │    Controllers/                     ├── Posts/                   │
// │  Application/                       │   ├── CreatePost.cs        │
// │    Services/                        │   │   (Command + Handler   │
// │    DTOs/                            │   │    + Validator + Resp) │
// │  Domain/                            │   ├── GetPost.cs           │
// │    Entities/                        │   └── ListPosts.cs         │
// │  Infrastructure/                    ├── Comments/                │
// │    Repositories/                    │   ├── CreateComment.cs     │
// │                                     │   └── DeleteComment.cs     │
// │  Cada capa es una carpeta.          └── Users/                   │
// │  Cada feature toca todas las capas.     ├── RegisterUser.cs      │
// │                                         └── LoginUser.cs         │
// │                                                                  │
// │  Acoplamiento:                     Acoplamiento:                 │
// │   Alto entre capas, bajo           Alto dentro del slice, bajo   │
// │   entre features                   entre features                │
// └──────────────────────────────────────────────────────────────────┘
//
// CUÁNDO USAR CADA UNA:
//
// Clean Architecture (horizontal):
//   ✅ Aplicaciones grandes con dominio complejo y compartido
//   ✅ Múltiples equipos tocando las mismas entidades
//   ✅ Necesitas reutilizar la lógica de dominio en varios sitios
//   ✅ Ej: sistema bancario, ERP, plataforma e-commerce compleja
//
// Vertical Slices:
//   ✅ Microservicios o aplicaciones modulares
//   ✅ Equipos autónomos dueños de features completas
//   ✅ Quieres poder borrar/mover una feature sin afectar al resto
//   ✅ Las features son independientes entre sí
//   ✅ Ej: SaaS con módulos, API con endpoints independientes
//
// AMBAS pueden coexistir: Vertical Slices dentro de Clean Architecture.
// Dentro de cada slice puedes usar capas si la feature es compleja.
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// ═══════════════════════════════════════════════════════════════════
// DOMINIO COMPARTIDO (lo que se reutiliza entre features)
// En Vertical Slices, el dominio compartido es mínimo.
// ═══════════════════════════════════════════════════════════════════
namespace ArchVertical.Shared
{
    /// <summary>
    /// Modelo de dominio compartido: Post.
    /// Solo contiene los datos y comportamiento esencial.
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
    /// "Base de datos" simulada en memoria (en producción sería EF Core).
    /// </summary>
    public static class Database
    {
        public static readonly List<Post> Posts = new();
    }
}

// ═══════════════════════════════════════════════════════════════════
// FEATURE: CreatePost
// Todo lo que necesita esta feature está AQUÍ, en UN SOLO SITIO.
// ───────────────────────────────────────────────────────────────────
// Contiene: Request (Command), Response, Validator, Handler.
// Si un día quieres borrar "CreatePost", borras esta clase y ya está.
// No hay que buscar en Services/, Validators/, DTOs/, etc.
// ═══════════════════════════════════════════════════════════════════
namespace ArchVertical.Features.Posts
{
    using ArchVertical.Shared;

    public static class CreatePost
    {
        /// <summary>
        /// Request: lo que el cliente envía (el comando).
        /// </summary>
        public sealed record Request(string Title, string Content, Guid AuthorId, List<string> Tags);

        /// <summary>
        /// Response: lo que devolvemos al cliente.
        /// </summary>
        public sealed record Response(Guid Id, string Title, DateTime CreatedAt);

        /// <summary>
        /// Validator: reglas de validación específicas de esta feature.
        /// En producción usarías FluentValidation, aquí va manual para el ejemplo.
        /// </summary>
        public sealed class Validator
        {
            public List<string> Validate(Request request)
            {
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(request.Title))
                    errors.Add("El título es obligatorio.");

                if (request.Title.Length < 3)
                    errors.Add("El título debe tener al menos 3 caracteres.");

                if (request.Title.Length > 200)
                    errors.Add("El título no puede superar los 200 caracteres.");

                if (string.IsNullOrWhiteSpace(request.Content))
                    errors.Add("El contenido es obligatorio.");

                if (request.AuthorId == Guid.Empty)
                    errors.Add("El AuthorId no puede ser vacío.");

                return errors;
            }
        }

        /// <summary>
        /// Handler: la lógica de negocio de esta feature.
        /// Orquesta: valida → transforma → persiste → devuelve.
        /// </summary>
        public static class Handler
        {
            private static readonly Validator _validator = new();

            public static Response Handle(Request request)
            {
                // 1. Validar
                List<string> errors = _validator.Validate(request);
                if (errors.Count > 0)
                {
                    throw new ValidationException(errors);
                }

                // 2. Crear la entidad de dominio
                Post post = new()
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title.Trim(),
                    Content = request.Content.Trim(),
                    AuthorId = request.AuthorId,
                    CreatedAt = DateTime.UtcNow,
                    Tags = request.Tags.Select(t => t.Trim().ToLowerInvariant()).ToList()
                };

                // 3. Persistir (aquí iría el repositorio, EF Core, etc.)
                Database.Posts.Add(post);

                // 4. Devolver respuesta
                return new Response(post.Id, post.Title, post.CreatedAt);
            }
        }
    }

    /// <summary>
    /// Excepción de validación específica.
    /// </summary>
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
// FEATURE: GetPost
// Otra feature completamente independiente.
// Si necesitas cambiar cómo se obtiene un post, solo tocas este archivo.
// ═══════════════════════════════════════════════════════════════════
namespace ArchVertical.Features.Posts
{
    using ArchVertical.Shared;

    public static class GetPost
    {
        public sealed record Request(Guid Id);

        public sealed record Response(
            Guid Id,
            string Title,
            string Content,
            Guid AuthorId,
            DateTime CreatedAt,
            DateTime? PublishedAt,
            List<string> Tags
        );

        public static class Handler
        {
            public static Response? Handle(Request request)
            {
                Post? post = Database.Posts.FirstOrDefault(p => p.Id == request.Id);
                if (post is null) return null;

                return new Response(
                    post.Id,
                    post.Title,
                    post.Content,
                    post.AuthorId,
                    post.CreatedAt,
                    post.PublishedAt,
                    post.Tags
                );
            }
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// FEATURE: ListPosts
// Listado con filtro opcional por tag.
// ═══════════════════════════════════════════════════════════════════
namespace ArchVertical.Features.Posts
{
    using ArchVertical.Shared;

    public static class ListPosts
    {
        public sealed record Request(string? TagFilter = null);

        public sealed record PostSummary(Guid Id, string Title, DateTime CreatedAt, List<string> Tags);

        public static class Handler
        {
            public static List<PostSummary> Handle(Request request)
            {
                IEnumerable<Post> query = Database.Posts;

                if (!string.IsNullOrWhiteSpace(request.TagFilter))
                {
                    string tag = request.TagFilter.Trim().ToLowerInvariant();
                    query = query.Where(p => p.Tags.Contains(tag));
                }

                return query
                    .Select(p => new PostSummary(p.Id, p.Title, p.CreatedAt, p.Tags))
                    .ToList();
            }
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// FEATURE: PublishPost
// Caso de uso: publicar un post (cambia el estado).
// ═══════════════════════════════════════════════════════════════════
namespace ArchVertical.Features.Posts
{
    using ArchVertical.Shared;

    public static class PublishPost
    {
        public sealed record Request(Guid Id);

        public sealed record Response(Guid Id, string Title, DateTime PublishedAt);

        public static class Handler
        {
            public static Response Handle(Request request)
            {
                Post? post = Database.Posts.FirstOrDefault(p => p.Id == request.Id);
                if (post is null)
                    throw new InvalidOperationException($"Post con ID {request.Id} no encontrado.");

                if (post.PublishedAt is not null)
                    throw new InvalidOperationException($"El post '{post.Title}' ya está publicado.");

                post.PublishedAt = DateTime.UtcNow;

                return new Response(post.Id, post.Title, post.PublishedAt.Value);
            }
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// DEMO: Programa principal
// ═══════════════════════════════════════════════════════════════════
namespace ArchVertical
{
    using ArchVertical.Features.Posts;

    public static class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║  Módulo 06 — Lección 03: Vertical Slices    ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.WriteLine();

            Guid authorId = Guid.NewGuid();

            // ─── CREAR POSTS (feature: CreatePost) ───
            Console.WriteLine("─── FEATURE: CreatePost ───");
            Console.WriteLine();

            CreatePost.Response post1 = CreatePost.Handler.Handle(
                new CreatePost.Request(
                    "Guía de Clean Architecture en .NET 8",
                    "Clean Architecture organiza el código en capas concéntricas...",
                    authorId,
                    new List<string> { "arquitectura", "dotnet", "csharp" }
                )
            );
            Console.WriteLine($"  ✅ Post creado: [{post1.Id}] {post1.Title}");

            CreatePost.Response post2 = CreatePost.Handler.Handle(
                new CreatePost.Request(
                    "Introducción a DDD Táctico",
                    "Entities, Value Objects, Aggregates y más...",
                    authorId,
                    new List<string> { "ddd", "domain-driven-design", "arquitectura" }
                )
            );
            Console.WriteLine($"  ✅ Post creado: [{post2.Id}] {post2.Title}");

            CreatePost.Response post3 = CreatePost.Handler.Handle(
                new CreatePost.Request(
                    "Vertical Slices vs Clean Architecture",
                    "Comparativa entre organización horizontal y vertical...",
                    authorId,
                    new List<string> { "arquitectura", "vertical-slices" }
                )
            );
            Console.WriteLine($"  ✅ Post creado: [{post3.Id}] {post3.Title}");

            // ─── VALIDACIÓN (feature: CreatePost.Validator) ───
            Console.WriteLine();
            Console.WriteLine("─── VALIDACIÓN EN CreatePost ───");
            Console.WriteLine();

            try
            {
                CreatePost.Handler.Handle(
                    new CreatePost.Request("", "", Guid.Empty, new List<string>())
                );
            }
            catch (ValidationException ex)
            {
                Console.WriteLine("  Errores de validación:");
                foreach (string error in ex.Errors)
                {
                    Console.WriteLine($"    ❌ {error}");
                }
            }

            // ─── OBTENER POST (feature: GetPost) ───
            Console.WriteLine();
            Console.WriteLine("─── FEATURE: GetPost ───");
            Console.WriteLine();

            GetPost.Response? fetched = GetPost.Handler.Handle(new GetPost.Request(post1.Id));
            if (fetched is not null)
            {
                Console.WriteLine($"  📄 {fetched.Title}");
                Console.WriteLine($"     Autor: {fetched.AuthorId}");
                Console.WriteLine($"     Tags:  {string.Join(", ", fetched.Tags)}");
                Console.WriteLine($"     Creado: {fetched.CreatedAt:u}");
            }

            // ─── LISTAR POSTS (feature: ListPosts) ───
            Console.WriteLine();
            Console.WriteLine("─── FEATURE: ListPosts ───");
            Console.WriteLine();

            List<ListPosts.PostSummary> allPosts = ListPosts.Handler.Handle(
                new ListPosts.Request()
            );
            Console.WriteLine("  Todos los posts:");
            foreach (ListPosts.PostSummary p in allPosts)
            {
                Console.WriteLine($"    • [{p.Id.ToString()[..8]}] {p.Title}");
            }

            // Filtrar por tag
            Console.WriteLine();
            Console.WriteLine("  Filtrando por tag 'arquitectura':");
            List<ListPosts.PostSummary> filtered = ListPosts.Handler.Handle(
                new ListPosts.Request(TagFilter: "arquitectura")
            );
            foreach (ListPosts.PostSummary p in filtered)
            {
                Console.WriteLine($"    • {p.Title}");
            }

            // ─── PUBLICAR POST (feature: PublishPost) ───
            Console.WriteLine();
            Console.WriteLine("─── FEATURE: PublishPost ───");
            Console.WriteLine();

            try
            {
                PublishPost.Response published = PublishPost.Handler.Handle(
                    new PublishPost.Request(post1.Id)
                );
                Console.WriteLine($"  ✅ '{published.Title}' publicado el {published.PublishedAt:u}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"  ❌ Error: {ex.Message}");
            }

            // Intentar publicar de nuevo (debe fallar)
            try
            {
                PublishPost.Handler.Handle(new PublishPost.Request(post1.Id));
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"  ❌ Error esperado: {ex.Message}");
            }

            // ─── COMPARATIVA DE ESTRUCTURAS ───
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("  COMPARATIVA: HORIZONTAL vs VERTICAL");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("  HORIZONTAL (Clean Architecture):");
            Console.WriteLine("  ─────────────────────────────────");
            Console.WriteLine("  src/");
            Console.WriteLine("  ├── Services/");
            Console.WriteLine("  │   ├── PostService.cs      ← TODOS los métodos de Post aquí");
            Console.WriteLine("  │   └── CommentService.cs");
            Console.WriteLine("  ├── Validators/");
            Console.WriteLine("  │   ├── CreatePostValidator.cs");
            Console.WriteLine("  │   └── UpdatePostValidator.cs");
            Console.WriteLine("  ├── DTOs/");
            Console.WriteLine("  │   ├── CreatePostDto.cs");
            Console.WriteLine("  │   └── PostResponseDto.cs");
            Console.WriteLine("  └── Repositories/");
            Console.WriteLine("      └── PostRepository.cs");
            Console.WriteLine();
            Console.WriteLine("  ❌ Para añadir un campo a CreatePost tocas 3-4 carpetas.");
            Console.WriteLine("  ❌ Para borrar CreatePost buscas en 3-4 carpetas.");
            Console.WriteLine();
            Console.WriteLine("  VERTICAL (Vertical Slices):");
            Console.WriteLine("  ────────────────────────────");
            Console.WriteLine("  src/Features/Posts/");
            Console.WriteLine("  ├── CreatePost.cs         ← Request + Validator + Handler + Response");
            Console.WriteLine("  ├── GetPost.cs            ← Query + Handler + Response");
            Console.WriteLine("  ├── ListPosts.cs          ← Query + Handler + Response (con filtro)");
            Console.WriteLine("  ├── UpdatePost.cs");
            Console.WriteLine("  └── DeletePost.cs");
            Console.WriteLine();
            Console.WriteLine("  ✅ Todo lo de CreatePost está en UN archivo.");
            Console.WriteLine("  ✅ Borrar CreatePost = borrar un archivo.");
            Console.WriteLine("  ✅ Cada feature es independiente y autónoma.");
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════");
        }
    }
}
