using Crabshell.Core.Media;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Media.Local;

public static class DependencyInjection
{
    public static IServiceCollection AddLocalMedia(this IServiceCollection services, string rootPath, string baseUrl)
    {
        services.AddSingleton<IMediaProvider>(new LocalMediaProvider(rootPath, baseUrl));
        return services;
    }
}