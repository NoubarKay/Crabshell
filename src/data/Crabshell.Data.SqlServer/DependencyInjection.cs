using Crabshell.Core.Schema;
using Crabshell.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Data.SqlServer;

public static class DependencyInjection
{
    public static IServiceCollection AddCrabshellSqlServerData<TDb>(
        this IServiceCollection services,
        string connectionString,
        Action<ModelBuilder>? configureModel = null) where TDb : class
    {
        services.AddSingleton<IDatabaseDialect, SqlServerDatabaseDialect>();

        return services.AddCrabshellData<TDb>(
            connectionString,
            configureModel,
            options => options.UseSqlServer(connectionString).UseSnakeCaseNamingConvention());
    }
}
