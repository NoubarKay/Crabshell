using Crabshell.Core.Repository;
using Crabshell.Core.Services;
using Crabshell.Data.Schema;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Crabshell.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddCrabshellData<TDb>(
        this IServiceCollection services,
        string connectionString) where TDb : class
    {
        services.AddDbContext<CrabshellDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        services.AddScoped<TDb>();
        services.AddScoped<ICollectionRepository, CollectionRepository>();
        services.AddScoped<CollectionRepositoryResolver>();
        services.AddScoped<ICollectionService, CollectionService>();
        services.AddScoped<ISchemaDiffService, SchemaDiffService>();

        return services;
    }
    
    public static async Task UseCrabshellDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrabshellDbContext>();
        var schemaDiff = scope.ServiceProvider.GetRequiredService<ISchemaDiffService>();

        if (app.Environment.IsDevelopment())
        {
            await schemaDiff.ApplyDiffAsync();
        }
        else
        {
            await db.Database.MigrateAsync();
        }
    }
}