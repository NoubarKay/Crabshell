using Crabshell.Core.Repository;
using Crabshell.Data;
using Crabshell.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Tests.Integration.Repository;

public class CollectionRepositoryTests : IntegrationTestBase
{
    private readonly CollectionRepository _repo;

    public CollectionRepositoryTests() : base(typeof(Article))
    {
        _repo = new CollectionRepository(DbContext);
        SchemaDiff.ApplyDiffAsync().GetAwaiter().GetResult();
    }

    private Article NewArticle(string title = "Test Article") => new()
    {
        Title = title,
        Views = 0,
        Published = false
    };

    [Fact]
    public async Task Create_persists_document()
    {
        var article = NewArticle();
        var created = await _repo.CreateAsync(article);

        created.Id.Should().NotBe(Guid.Empty);
        created.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetById_returns_created_document()
    {
        var article = NewArticle("Hello World");
        await _repo.CreateAsync(article);

        var meta = Registry.Get("articles")!;
        var found = await _repo.GetByIdAsync(meta, article.Id) as Article;

        found.Should().NotBeNull();
        found!.Title.Should().Be("Hello World");
    }

    [Fact]
    public async Task GetById_returns_null_for_missing_id()
    {
        var meta = Registry.Get("articles")!;
        var found = await _repo.GetByIdAsync(meta, Guid.NewGuid());
        found.Should().BeNull();
    }

    [Fact]
    public async Task Update_persists_changes()
    {
        var article = NewArticle();
        await _repo.CreateAsync(article);

        article.Title = "Updated Title";
        await _repo.UpdateAsync(article);

        var meta = Registry.Get("articles")!;
        var found = await _repo.GetByIdAsync(meta, article.Id) as Article;
        found!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task Delete_soft_deletes_document()
    {
        var article = NewArticle();
        await _repo.CreateAsync(article);
        await _repo.DeleteAsync(article);

        var meta = Registry.Get("articles")!;
        var found = await _repo.GetByIdAsync(meta, article.Id);
        found.Should().BeNull(); // query filter excludes soft-deleted
    }

    [Fact]
    public async Task GetPage_returns_paged_results()
    {
        for (var i = 1; i <= 5; i++)
            await _repo.CreateAsync(NewArticle($"Article {i}"));

        var meta = Registry.Get("articles")!;
        var result = await _repo.GetPageAsync(meta, new CollectionQuery(Skip: 0, Take: 3));

        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(5);
    }

    [Fact]
    public async Task GetPage_filters_by_field()
    {
        await _repo.CreateAsync(NewArticle("Findable"));
        await _repo.CreateAsync(NewArticle("Other"));

        var meta = Registry.Get("articles")!;
        var result = await _repo.GetPageAsync(meta, new CollectionQuery(
            Skip: 0,
            Take: 10,
            Filters: [new FieldFilter("Title", FieldFilterOperator.Equals, "Findable")]
        ));

        result.Items.Should().HaveCount(1);
        (result.Items[0] as Article)!.Title.Should().Be("Findable");
    }

    [Fact]
    public async Task GetPage_sorts_by_field()
    {
        await _repo.CreateAsync(NewArticle("B Article"));
        await _repo.CreateAsync(NewArticle("A Article"));

        var meta = Registry.Get("articles")!;
        var result = await _repo.GetPageAsync(meta, new CollectionQuery(
            Skip: 0,
            Take: 10,
            OrderBy: "Title",
            Descending: false
        ));

        var titles = result.Items.Cast<Article>().Select(a => a.Title).ToList();
        titles.Should().BeInAscendingOrder();
    }
}
