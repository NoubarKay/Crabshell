using Crabshell.Core.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Storage.Local;

public static class DependencyInjection
{
    public static IServiceCollection UseCrabshellLocalStorage(
        this IServiceCollection services,
        Action<LocalStorageOptions>? configure = null)
    {
        services.AddSingleton<IStorageProvider, LocalStorageProvider>();
        services.Configure<LocalStorageOptions>(options =>
        {
            options.RootPath = "wwwroot/uploads";
            configure?.Invoke(options);
        });

        return services;
    }
}