using Crabshell.Core.Schema;
using Crabshell.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Data.Sqlite;

public static class DependencyInjection
{
    public static IServiceCollection AddCrabshellSqliteData<TDb>(
        this IServiceCollection services,
        string connectionString,
        Action<ModelBuilder>? configureModel = null) where TDb : class
    {
        services.AddSingleton<IDatabaseDialect, SqliteDatabaseDialect>();

        return services.AddCrabshellData<TDb>(
            connectionString,
            configureModel,
            options => options.UseSqlite(connectionString).UseSnakeCaseNamingConvention());
    }
}
