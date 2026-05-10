using Crabshell.Core.Schema;
using Crabshell.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Data.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddCrabshellPostgresData<TDb>(
        this IServiceCollection services,
        string connectionString,
        Action<ModelBuilder>? configureModel = null) where TDb : class
    {
        services.AddSingleton<IDatabaseDialect, PostgresDatabaseDialect>();

        return services.AddCrabshellData<TDb>(
            connectionString,
            configureModel,
            options => options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
    }
}
