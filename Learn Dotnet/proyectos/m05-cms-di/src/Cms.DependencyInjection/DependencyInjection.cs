// ──────────────────────────────────────────────────────────────────────────
// DependencyInjection — Método de extensión central para registrar todos
// los servicios del CMS en el contenedor de DI de .NET.
// Módulo 5: Inyección de Dependencias
// ──────────────────────────────────────────────────────────────────────────
// Este archivo es el punto de entrada de la configuración de DI del CMS.
// Contiene el método AddCmsServices(), que registra repositorios,
// validadores y servicios de aplicación con los lifetimes adecuados.
//
// PATRÓN: Método de extensión sobre IServiceCollection.
//   Ventaja: quien consume esta biblioteca solo necesita llamar
//   builder.Services.AddCmsServices() y todo queda configurado.
//   Esto evita que cada proyecto tenga que conocer los detalles
//   internos de qué implementaciones concretas usar.
//
// LIFETIMES USADOS:
//   - Singleton: repositorios en memoria y validadores.
//     Los repositorios en memoria mantienen estado (la lista de
//     entidades). Si fueran Scoped, cada petición HTTP tendría su
//     propia lista vacía. Singleton garantiza que los datos persistan
//     entre peticiones mientras la aplicación esté corriendo.
//     Los validadores son stateless, así que Singleton es correcto.
//
//   - Scoped: servicios de aplicación.
//     En ASP.NET Core, Scoped significa "una instancia por petición HTTP".
//     Aunque ahora mismo los servicios no tienen estado por petición,
//     en el futuro usarán DbContext (que ES Scoped). Mantener Scoped
//     desde el principio evita problemas de Captive Dependencies.
//
// FUTURO (Módulo 7 - EF Core):
//   Los repositorios en memoria se reemplazarán por DbContext,
//   que se registra como Scoped. La configuración quedará:
//     services.AddDbContext<CmsDbContext>(...);  // Scoped
//     services.AddScoped<IPostRepository, PostRepository>(); // Scoped
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;
using Cms.Services;
using Cms.Solid.Repositories;
using Cms.Solid.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cms.DependencyInjection;

/// <summary>
/// Clase estática que expone métodos de extensión para configurar
/// la inyección de dependencias de todo el CMS.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra todos los servicios del CMS en el contenedor de DI.
    /// Método principal. Llama a los submétodos para organizar el registro.
    /// </summary>
    /// <param name="services">Colección de servicios de la aplicación.</param>
    /// <returns>La misma colección para permitir encadenamiento (fluent API).</returns>
    public static IServiceCollection AddCmsServices(this IServiceCollection services)
    {
        // ── Paso 1: Registrar repositorios (Singleton porque son en memoria) ──
        // Singleton: una instancia compartida por toda la aplicación.
        // Necesario porque los repositorios en memoria mantienen el estado
        // (la lista de entidades). Si fueran Scoped, cada petición HTTP
        // empezaría con una lista vacía y los datos no se compartirían.
        services.AddCmsRepositories();

        // ── Paso 2: Registrar validadores (Singleton porque no tienen estado) ──
        // Los validadores son objetos puros: no guardan datos entre
        // llamadas. Son thread-safe por naturaleza. Singleton evita
        // crear instancias nuevas en cada petición.
        services.AddCmsValidators();

        // ── Paso 3: Registrar servicios de aplicación (Scoped por convención) ──
        // Scoped: una instancia nueva por cada petición HTTP (o por scope).
        // En el futuro, cuando usemos DbContext (que es Scoped), los
        // servicios que dependen de DbContext también deben ser Scoped
        // para evitar Captive Dependencies.
        services.AddCmsApplicationServices();

        return services;
    }

    /// <summary>
    /// Registra los repositorios en memoria.
    /// </summary>
    private static IServiceCollection AddCmsRepositories(this IServiceCollection services)
    {
        // ── Posts ──
        // Registramos InMemoryPostRepository como la implementación concreta
        // de IPostRepository. Cuando alguien pida IPostRepository, el
        // contenedor devolverá la misma instancia de InMemoryPostRepository.
        services.AddSingleton<IPostRepository, InMemoryPostRepository>();

        // ── Categorías ──
        services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();

        // ── Etiquetas ──
        services.AddSingleton<ITagRepository, InMemoryTagRepository>();

        // ── Usuarios ──
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();

        return services;
    }

    /// <summary>
    /// Registra los validadores de entidades.
    /// </summary>
    private static IServiceCollection AddCmsValidators(this IServiceCollection services)
    {
        // Los validadores son stateless: no almacenan datos entre
        // validaciones. Cada método Validate recibe la entidad y
        // devuelve errores sin modificar estado interno.
        // Singleton es el lifetime correcto aquí.

        services.AddSingleton<PostValidator>();
        services.AddSingleton<CategoryValidator>();
        services.AddSingleton<TagValidator>();
        services.AddSingleton<UserValidator>();

        return services;
    }

    /// <summary>
    /// Registra los servicios de aplicación.
    /// </summary>
    private static IServiceCollection AddCmsApplicationServices(this IServiceCollection services)
    {
        // Scoped: una instancia por petición HTTP.
        // Los servicios dependen de repositorios (Singleton) y
        // validadores (Singleton). Esto NO es un Captive Dependency
        // porque Scoped puede depender de Singleton sin problema.
        // El problema sería al revés: Singleton dependiendo de Scoped.

        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
