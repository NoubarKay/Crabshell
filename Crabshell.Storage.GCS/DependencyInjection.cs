using Crabshell.Core.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Storage.GCS;

public static class DependencyInjection
{
    public static IServiceCollection UseCrabshellGcsStorage(
        this IServiceCollection services,
        Action<GcsStorageOptions>? configure = null)
    {
        services.AddSingleton<IStorageProvider, GcsStorageProvider>();
        services.Configure<GcsStorageOptions>(options =>
        {
            configure?.Invoke(options);
        });

        return services;
    }
}
