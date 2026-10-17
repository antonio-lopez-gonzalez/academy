// ─── MÓDULO 17 ─── Lección 3: gRPC — ContentService del CMS ───
// Implementación del contrato definido en Protos/content.proto.
//
// Cómo funciona el codegen:
//   Grpc.Tools generó (en obj/) la clase abstracta `ContentService.ContentServiceBase`
//   con un método virtual por cada rpc. Esta clase la hereda y sobrescribe.
//   ASP.NET Core la registra con MapGrpcService<ContentService>() y cada llamada
//   entra por DI como un servicio SCOPED (nueva instancia por RPC).

using Google.Protobuf.WellKnownTypes; // Timestamp
using Grpc.Core;                      // RpcException, StatusCode, ServerCallContext
using Cms.Grpc.V1;

namespace CmsGrpcServer.Services;

// Modelo interno de dominio (no es el mensaje protobuf: separación contrato/dominio).
public sealed class Post
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string Excerpt { get; init; } = string.Empty;
    public List<string> Tags { get; init; } = [];
    public DateTime? PublishedAtUtc { get; set; }
}

public sealed class ContentService : ContentService.ContentServiceBase
{
    // Almacén en memoria para la lección (en el curso real sería EF Core + PostgreSQL).
    private readonly ConcurrentDictionary<string, Post> _posts = new();
    private int _sequence;

    public ContentService()
    {
        // Datos semilla deterministas para que el cliente siempre tenga qué leer.
        _posts.TryAdd("post-1", new Post
        {
            Id = "post-1",
            Title = "Introducción a .NET 8",
            Excerpt = "CLR, BCL y compilación.",
            Tags = ["dotnet", "basics"],
            PublishedAtUtc = new DateTime(2026, 1, 10, 9, 0, 0, DateTimeKind.Utc)
        });
        _posts.TryAdd("post-2", new Post
        {
            Id = "post-2",
            Title = "gRPC vs REST",
            Excerpt = "Cuándo elegir binario sobre JSON.",
            Tags = ["grpc", "api"],
            PublishedAtUtc = new DateTime(2026, 2, 3, 15, 30, 0, DateTimeKind.Utc)
        });
        _posts.TryAdd("post-3", new Post
        {
            Id = "post-3",
            Title = "Borrador: Minimal APIs avanzadas",
            Excerpt = "Filtros, BindAsync y TypedResults.",
            Tags = ["aspnet"],
            PublishedAtUtc = null // aún no publicado
        });
    }

    // ── RPC UNARIO: GetPost ─────────────────────────────────────────
    // En gRPC los ERRORES no son excepciones "normales": se comunican con
    // StatusCode del protocolo (NOT_FOUND, INVALID_ARGUMENT, ...).
    // Lanzamos RpcException; el runtime la convierte en status gRPC hacia el cliente.
    public override Task<PostReply> GetPost(GetPostRequest request, ServerCallContext context)
    {
        if (!_posts.TryGetValue(request.Id, out Post? post))
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"No existe un post con id '{request.Id}'"));
        }

        return Task.FromResult(ToReply(post));
    }

    // ── RPC STREAMING SERVIDOR: ListPosts ───────────────────────────
    // En vez de devolver UNA respuesta, escribimos MUCHAS respuestas con
    // responseStream.WriteAsync(). El cliente las recibe en cuanto salen
    // (sin esperar a que termine el stream): ideal para feeds grandes.
    public override async Task ListPosts(
        ListPostsRequest request,
        IServerStreamWriter<PostReply> responseStream,
        ServerCallContext context)
    {
        var query = _posts.Values.OrderBy(p => p.Id, StringComparer.Ordinal).AsEnumerable();

        if (request.PublishedOnly)
        {
            query = query.Where(p => p.PublishedAtUtc is not null);
        }

        if (request.Limit > 0)
        {
            query = query.Take(request.Limit);
        }

        foreach (Post post in query)
        {
            // context.CancellationToken se cancela si el cliente corta la conexión
            // o si expira el deadline: comprobarlo evita seguir escribiendo al vacío.
            context.CancellationToken.ThrowIfCancellationRequested();

            await responseStream.WriteAsync(ToReply(post));
        }
    }

    // ── RPC UNARIO: CreatePost ──────────────────────────────────────
    public override Task<PostReply> CreatePost(CreatePostRequest request, ServerCallContext context)
    {
        // Validación básica → status INVALID_ARGUMENT (equivalente a 400 en REST).
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                "El título no puede estar vacío"));
        }

        int number = Interlocked.Increment(ref _sequence);
        var post = new Post
        {
            Id = $"post-{number}",
            Title = request.Title.Trim(),
            Excerpt = request.Excerpt.Trim(),
            Tags = [.. request.Tags]
        };

        _posts[post.Id] = post;
        return Task.FromResult(ToReply(post));
    }

    // Mapeo dominio → mensaje protobuf.
    private static PostReply ToReply(Post post)
    {
        var reply = new PostReply
        {
            Id = post.Id,
            Title = post.Title,
            Excerpt = post.Excerpt
        };

        // `repeated string tags` se expone como RepeatedField<string> ( IList<string> ):
        // se rellena con AddRange, igual que una lista normal.
        reply.Tags.AddRange(post.Tags);

        // DateTime? → google.protobuf.Timestamp (null si no está publicado).
        if (post.PublishedAtUtc is not null)
        {
            reply.PublishedAt = Timestamp.FromDateTime(post.PublishedAtUtc.Value);
        }

        return reply;
    }
}
