// ─── MÓDULO 17 ─── Lección 3: gRPC — Servidor del CMS ───
// Puntos clave de este arranque:
//   1. Kestrel configurado por CÓDIGO en http://127.0.0.1:5299 (sin HTTPS).
//      ¿Por qué sin TLS aquí? Es un entorno LOCAL de aprendizaje: evitamos generar/aceptar
//      el cert dev de ASP.NET Core para simplificar. gRPC nativo sobre texto claro (h2c,
//      "HTTP/2 cleartext") funciona perfectamente entre procesos .NET.
//      ⚠ EN PRODUCCIÓN gRPC DEBE ir detrás de TLS (seguridad + proxies).
//
//   2. Protocolos HTTP/1.1 + HTTP/2 en el MISMO puerto:
//      - Los clientes NATIVOS (.NET, Go...) usan h2c (prefacio HTTP/2 sin ALPN) → requieren Http2.
//      - Los NAVEGADORES solo hablan HTTP/1.1 en claro → necesitan Http1.
//      Con Http1AndHttp2 + middleware gRPC-Web servimos a ambos en un solo endpoint.
//
//   3. gRPC-Web (UseGrpcWeb): traduce llamadas de navegador a formato gRPC.
//      Un navegador NO soporta gRPC nativo porque no puede enviar trailers HTTP
//      ni iniciar h2c; gRPC-Web resuelve eso desde el servidor.

using System.Net;
using CmsGrpcServer.Services;
using Grpc.AspNetCore.Web;

var builder = WebApplication.CreateBuilder(args);

// ── Kestrel: escucha explícita en 127.0.0.1:5299 ──────────────────
// ConfigureKestrel por código tiene prioridad sobre appsettings.json y launchSettings:
// así `dotnet run` SIEMPRE queda en la misma URL, con o sin perfil de lanzamiento.
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Loopback, 5299, listenOptions =>
    {
        // Http1AndHttp2: navegadores (gRPC-Web, HTTP/1.1) + clientes nativos (h2c, HTTP/2).
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});

// Registro del servicio gRPC (el codegen generó ContentServiceBase).
builder.Services.AddGrpc();

var app = builder.Build();

// Middleware gRPC-Web: DEBE ir ANTES de MapGrpcService.
// DefaultEnabled = true activa gRPC-Web para TODOS los servicios mapeados
// (alternativa fina: [EnableGrpcWeb] solo en servicios concretos).
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

// Mapea el contrato content.proto al servicio C#.
app.MapGrpcService<ContentService>();

// Endpoint de cortesía para comprobar con el navegador que el proceso vive.
app.MapGet("/", () => "CmsGrpcServer vivo. Usa el cliente gRPC en src/CmsGrpcClient.");

app.Run();
