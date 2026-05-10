using Crabshell.Data.Schema;
using Crabshell.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Crabshell.Tests.Integration.Schema;

public class SchemaDiffServiceTests : IntegrationTestBase
{
    public SchemaDiffServiceTests() : base(typeof(Article)) { }

    private async Task<IReadOnlyList<string>> GetColumnsAsync(string table)
    {
        var conn = DbContext.Database.GetDbConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"PRAGMA table_info({table})";
        using var reader = await cmd.ExecuteReaderAsync();
        var columns = new List<string>();
        while (await reader.ReadAsync())
            columns.Add(reader.GetString(1)); // column name is index 1
        return columns;
    }

    [Fact]
    public async Task Creates_table_with_all_columns_on_first_run()
    {
        await SchemaDiff.ApplyDiffAsync();

        var columns = await GetColumnsAsync("articles");
        columns.Should().Contain("id");
        columns.Should().Contain("created_at");
        columns.Should().Contain("updated_at");
        columns.Should().Contain("is_deleted");
        columns.Should().Contain("deleted_at");
        columns.Should().Contain("title");
        columns.Should().Contain("body");
        columns.Should().Contain("views");
        columns.Should().Contain("published");
        columns.Should().Contain("published_at");
    }

    [Fact]
    public async Task ApplyDiff_is_idempotent()
    {
        await SchemaDiff.ApplyDiffAsync();
        var act = async () => await SchemaDiff.ApplyDiffAsync();
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Creates_both_collection_tables()
    {
        await SchemaDiff.ApplyDiffAsync();

        var articleColumns = await GetColumnsAsync("articles");
        var authorColumns = await GetColumnsAsync("authors");

        articleColumns.Should().NotBeEmpty();
        authorColumns.Should().NotBeEmpty();
        authorColumns.Should().Contain("name");
        authorColumns.Should().Contain("bio");
    }
}
