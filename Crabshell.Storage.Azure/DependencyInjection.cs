using Crabshell.Core.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Storage.Azure;

public static class DependencyInjection
{
    public static IServiceCollection UseCrabshellAzureStorage(
        this IServiceCollection services,
        Action<AzureStorageOptions>? configure = null)
    {
        services.AddSingleton<IStorageProvider, AzureStorageProvider>();
        services.Configure<AzureStorageOptions>(options =>
        {
            configure?.Invoke(options);
        });

        return services;
    }
}