using Crabshell.Core;
using Crabshell.Core.Registry;
using Crabshell.Core.Schema;
using Crabshell.Data;
using Crabshell.Data.Schema;
using Crabshell.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Tests.Integration.Infrastructure;

public abstract class IntegrationTestBase : IAsyncDisposable
{
    protected IServiceProvider Services { get; }
    protected CrabshellDbContext DbContext => Services.GetRequiredService<CrabshellDbContext>();
    protected CollectionRegistry Registry => Services.GetRequiredService<CollectionRegistry>();
    protected SchemaDiffService SchemaDiff => (SchemaDiffService)Services.GetRequiredService<ISchemaDiffService>();

    protected IntegrationTestBase(params Type[] collectionAssemblyMarkers)
    {
        var services = new ServiceCollection();

        var assemblies = collectionAssemblyMarkers
            .Select(t => t.Assembly)
            .Distinct()
            .ToArray();

        services.AddCrabshellCore(assemblies);
        var dbName = Guid.NewGuid().ToString("N");
        services.AddCrabshellSqliteData<object>(
            $"DataSource=file:{dbName}?mode=memory&cache=shared",
            configureModel: null);

        Services = services.BuildServiceProvider();

        // Keep connection open so SQLite in-memory DB persists for the test lifetime
        var conn = DbContext.Database.GetDbConnection();
        conn.Open();
    }

    public async ValueTask DisposeAsync()
    {
        await DbContext.Database.GetDbConnection().CloseAsync();
        if (Services is IAsyncDisposable d)
            await d.DisposeAsync();
    }
}
