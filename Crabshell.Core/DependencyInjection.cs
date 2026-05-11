using System.Reflection;
using Crabshell.Core.Registry;
using Crabshell.Core.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Core;

/// <summary>Extension methods for registering Crabshell core services.</summary>
public static class DependencyInjection
{
    /// <summary>
    /// Scans <paramref name="assemblies"/> for <c>[Collection]</c> and <c>[Single]</c> types,
    /// builds the <see cref="CollectionRegistry"/>, and registers it as a singleton.
    /// Call this before <c>AddCrabshellData</c>.
    /// </summary>
    public static IServiceCollection AddCrabshellCore(this IServiceCollection services, params Assembly[] assemblies)
    {
        var collectionRegistry = new CollectionRegistry();
        foreach (var assembly in assemblies)
            collectionRegistry.Register(assembly);
        services.AddSingleton(collectionRegistry);

        foreach (var hookType in collectionRegistry.GetAllHookTypes())
            services.AddScoped(hookType);

        return services;
    }

    /// <summary>
    /// Registers a custom <see cref="IStorageProvider"/> implementation as a singleton.
    /// Use this when none of the built-in provider extension methods fit your needs.
    /// </summary>
    public static IServiceCollection UseCrabshellStorage<TProvider>(this IServiceCollection services) where TProvider : class, IStorageProvider
    {
        services.AddSingleton<IStorageProvider, TProvider>();
        return services;
    }
}