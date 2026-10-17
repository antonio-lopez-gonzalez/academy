// ──────────────────────────────────────────────────────────────────────────
// ServiceCollectionExtensions — Métodos de extensión adicionales para
// la configuración del contenedor de DI.
// Módulo 5: Inyección de Dependencias
// ──────────────────────────────────────────────────────────────────────────
// Esta clase contiene utilidades complementarias que ayudan a validar
// y depurar la configuración de DI, especialmente durante el desarrollo.
// ──────────────────────────────────────────────────────────────────────────

using Microsoft.Extensions.DependencyInjection;

namespace Cms.DependencyInjection;

/// <summary>
/// Extensiones adicionales para IServiceCollection
/// que complementan la configuración principal de DI del CMS.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configura la validación de scopes para detectar Captive Dependencies.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <param name="validateScopes">
    /// Si es true, el contenedor lanzará una excepción si detecta que
    /// un servicio Scoped se está resolviendo desde un Singleton.
    /// Recomendado: true en desarrollo, false en producción (por rendimiento).
    /// </param>
    /// <returns>La misma colección para encadenamiento.</returns>
    /// <remarks>
    /// CAPTIVE DEPENDENCY:
    ///   Ocurre cuando un servicio Singleton depende de un servicio Scoped.
    ///   Como el Singleton vive para siempre, el Scoped queda "atrapado"
    ///   y nunca se libera. Esto puede causar fugas de memoria y
    ///   comportamientos incorrectos (datos de una petición se mezclan
    ///   con los de otra).
    ///
    /// EJEMPLO de Captive Dependency:
    ///   services.AddSingleton<IMyService, MyService>();      // Singleton
    ///   services.AddScoped<IMyRepository, MyRepository>();   // Scoped
    ///   // MyService(IMyRepository repo) ← ¡CAPTIVE! El repo Scoped
    ///   // queda atrapado en el Singleton para siempre.
    ///
    /// Esta validación detecta estos casos en tiempo de ejecución.
    ///
    /// NOTA: ValidateScopes solo está disponible cuando se usa
    /// HostBuilder / WebApplication. En una aplicación de consola
    /// con ServiceProvider directamente, se configura con:
    ///   new ServiceCollection()
    ///       .BuildServiceProvider(new ServiceProviderOptions
    ///       {
    ///           ValidateScopes = true
    ///       });
    /// </remarks>
    public static IServiceCollection AddCmsValidation(
        this IServiceCollection services,
        bool validateScopes = true)
    {
        // ValidateScopes se configura a nivel de ServiceProvider,
        // no de IServiceCollection directamente. Este método sirve
        // como recordatorio/documentación de la buena práctica.
        // La activación real se hace en Program.cs:
        //   builder.Host.UseDefaultServiceProvider(options =>
        //   {
        //       options.ValidateScopes = true;
        //   });

        return services;
    }

    /// <summary>
    /// Registra todos los repositorios en memoria del CMS.
    /// Método helper que expone públicamente lo que DependencyInjection
    /// ya hace internamente. Útil si se quiere registrar repositorios
    /// sin los servicios o validadores (ej: en tests unitarios).
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>La misma colección para encadenamiento.</returns>
    /// <remarks>
    /// USO EN TESTS:
    ///   var services = new ServiceCollection();
    ///   services.AddCmsInMemoryRepositories();
    ///   // Solo repositorios, sin servicios ni validadores.
    ///   // Permite testear repositorios de forma aislada.
    ///
    /// NOTA: Actualmente este método delega en AddCmsServices()
    /// completo. En el futuro podría exponer solo los repositorios.
    /// </remarks>
    public static IServiceCollection AddCmsInMemoryRepositories(
        this IServiceCollection services)
    {
        // Por ahora, delegamos en la configuración completa.
        // En el futuro, este método podría registrar solo repositorios
        // para tests de integración aislados.
        return services.AddCmsServices();
    }

    /// <summary>
    /// Imprime en consola un resumen de los servicios registrados.
    /// Útil para depuración durante el desarrollo.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>La misma colección para encadenamiento.</returns>
    /// <remarks>
    /// Llama a este método antes de BuildServiceProvider() para
    /// ver exactamente qué servicios se han registrado, con qué
    /// lifetime y qué tipo de implementación.
    ///
    /// EJEMPLO DE SALIDA:
    ///   Singleton: IPostRepository → InMemoryPostRepository
    ///   Singleton: PostValidator → PostValidator
    ///   Scoped:    IPostService → PostService
    ///   ...
    /// </remarks>
    public static IServiceCollection DebugPrintRegistrations(
        this IServiceCollection services)
    {
        Console.WriteLine("=== SERVICIOS REGISTRADOS EN CMS ===");
        Console.WriteLine($"Total: {services.Count} servicios\n");

        foreach (ServiceDescriptor descriptor in services)
        {
            string lifetime = descriptor.Lifetime switch
            {
                ServiceLifetime.Singleton => "Singleton",
                ServiceLifetime.Scoped    => "Scoped   ",
                ServiceLifetime.Transient => "Transient",
                _                          => "Unknown  "
            };

            string serviceType = descriptor.ServiceType.Name;
            string implementation = descriptor.ImplementationType?.Name
                                    ?? " (factory/instance) ";

            Console.WriteLine($"  {lifetime}: {serviceType} → {implementation}");
        }

        Console.WriteLine();
        return services;
    }
}
