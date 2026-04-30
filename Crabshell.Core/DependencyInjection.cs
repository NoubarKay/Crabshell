using System.Reflection;
using Crabshell.Core.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCrabshellCore(this IServiceCollection services, params Assembly[] assemblies)
    {
        var collectionRegistry = new CollectionRegistry();
        foreach (var assembly in assemblies)
            collectionRegistry.Register(assembly);
        services.AddSingleton(collectionRegistry);
        
        
        
        return services;
    }
}