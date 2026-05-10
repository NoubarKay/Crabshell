using Crabshell.Core.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Storage.S3;

public static class DependencyInjection
{
    public static IServiceCollection UseCrabshellS3Storage(
        this IServiceCollection services,
        Action<S3StorageOptions>? configure = null)
    {
        services.AddSingleton<IStorageProvider, S3StorageProvider>();
        services.Configure<S3StorageOptions>(options =>
        {
            configure?.Invoke(options);
        });

        return services;
    }
}
