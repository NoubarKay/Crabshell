using Crabshell.Core.Attributes;
using Crabshell.Core.Registry;
using Crabshell.Data.Sqlite;
using Crabshell.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Crabshell.Tests.Integration.Dialect;

public class SqliteDatabaseDialectTests
{
    private readonly SqliteDatabaseDialect _dialect = new();
    private readonly CollectionRegistry _registry;

    public SqliteDatabaseDialectTests()
    {
        _registry = new CollectionRegistry();
        _registry.Register(typeof(Article).Assembly);
    }


    [Fact]
    public void UuidType_is_TEXT() => _dialect.UuidType.Should().Be("TEXT");

    [Fact]
    public void TimestampType_is_TEXT() => _dialect.TimestampType.Should().Be("TEXT");

    [Fact]
    public void BoolType_is_INTEGER() => _dialect.BoolType.Should().Be("INTEGER");

    [Fact]
    public void Now_returns_sqlite_datetime() => _dialect.Now().Should().Contain("datetime");

    [Fact]
    public void AddColumnIfNotExists_generates_alter_statement()
    {
        var sql = _dialect.AddColumnIfNotExists("articles", "\"title\" TEXT NOT NULL");
        sql.Should().Contain("ALTER TABLE");
        sql.Should().Contain("articles");
        sql.Should().Contain("title");
    }

    [Fact]
    public void GetColumnType_text_limited_returns_TEXT()
    {
        var field = _registry.Get("articles")!.Fields.First(f => f.PropertyName == "Title");
        _dialect.GetColumnType(field).Should().Be("TEXT");
    }

    [Fact]
    public void GetColumnType_bool_returns_INTEGER()
    {
        var field = _registry.Get("articles")!.Fields.First(f => f.PropertyName == "Published");
        _dialect.GetColumnType(field).Should().Be("INTEGER");
    }

    [Fact]
    public void GetColumnType_number_int_returns_INTEGER()
    {
        var field = _registry.Get("articles")!.Fields.First(f => f.PropertyName == "Views");
        _dialect.GetColumnType(field).Should().Be("INTEGER");
    }

    [Fact]
    public void GetColumnType_datetime_returns_TEXT()
    {
        var field = _registry.Get("articles")!.Fields.First(f => f.PropertyName == "PublishedAt");
        _dialect.GetColumnType(field).Should().Be("TEXT");
    }
}
