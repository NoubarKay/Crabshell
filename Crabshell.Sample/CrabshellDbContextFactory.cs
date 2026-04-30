using Crabshell.Core.Registry;
using Crabshell.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Crabshell.Sample;

public class CrabshellDbContextFactory : IDesignTimeDbContextFactory<CrabshellDbContext>
{
    public CrabshellDbContext CreateDbContext(string[] args)
    {
        var registry = new CollectionRegistry();
        registry.Register(typeof(CrabshellDbContextFactory).Assembly);

        var options = new DbContextOptionsBuilder<CrabshellDbContext>()
            .UseNpgsql("Host=localhost;Database=crabshell-test;Username=postgres;Password=123123123")
            .UseSnakeCaseNamingConvention()
            .Options;

        return new CrabshellDbContext(options, registry);
    }
}   