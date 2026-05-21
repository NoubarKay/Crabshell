using Crabshell.Data;
using Crabshell.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Crabshell.Tests.Integration.Repository;

public class ManyToManyRepositoryTests : IntegrationTestBase
{
    private readonly CollectionRepository _repo;

    public ManyToManyRepositoryTests() : base(typeof(Article))
    {
        _repo = new CollectionRepository(DbContext);
        SchemaDiff.ApplyDiffAsync().GetAwaiter().GetResult();
    }

    private async Task<Author> CreateAuthorAsync(string name)
    {
        var author = new Author { Name = name };
        await _repo.CreateAsync(author);
        return author;
    }

    [Fact]
    public async Task Sync_and_load_round_trips_related_ids()
    {
        var a1 = await CreateAuthorAsync("Author 1");
        var a2 = await CreateAuthorAsync("Author 2");

        var meta = Registry.Get("articles")!;
        var article = new Article { Title = "M2M", AuthorIds = [a1.Id, a2.Id] };
        await _repo.CreateAsync(article);
        await _repo.SyncManyToManyAsync(meta, article);

        var loaded = await _repo.GetByIdAsync(meta, article.Id) as Article;
        loaded!.AuthorIds.Should().BeEquivalentTo(new[] { a1.Id, a2.Id });
    }

    [Fact]
    public async Task Sync_removes_unselected_relations()
    {
        var a1 = await CreateAuthorAsync("A1");
        var a2 = await CreateAuthorAsync("A2");

        var meta = Registry.Get("articles")!;
        var article = new Article { Title = "M2M", AuthorIds = [a1.Id, a2.Id] };
        await _repo.CreateAsync(article);
        await _repo.SyncManyToManyAsync(meta, article);

        article.AuthorIds = [a1.Id];
        await _repo.SyncManyToManyAsync(meta, article);

        var loaded = await _repo.GetByIdAsync(meta, article.Id) as Article;
        loaded!.AuthorIds.Should().BeEquivalentTo(new[] { a1.Id });
    }

    [Fact]
    public async Task GetPage_loads_relations_for_each_document()
    {
        var a1 = await CreateAuthorAsync("A1");
        var a2 = await CreateAuthorAsync("A2");

        var meta = Registry.Get("articles")!;

        var first = new Article { Title = "First", AuthorIds = [a1.Id] };
        await _repo.CreateAsync(first);
        await _repo.SyncManyToManyAsync(meta, first);

        var second = new Article { Title = "Second", AuthorIds = [a1.Id, a2.Id] };
        await _repo.CreateAsync(second);
        await _repo.SyncManyToManyAsync(meta, second);

        var page = await _repo.GetPageAsync(meta, new Core.Repository.CollectionQuery(Skip: 0, Take: 10));

        var firstLoaded = page.Items.Cast<Article>().Single(a => a.Title == "First");
        var secondLoaded = page.Items.Cast<Article>().Single(a => a.Title == "Second");

        firstLoaded.AuthorIds.Should().BeEquivalentTo(new[] { a1.Id });
        secondLoaded.AuthorIds.Should().BeEquivalentTo(new[] { a1.Id, a2.Id });
    }

    [Fact]
    public async Task Empty_relation_list_loads_as_empty()
    {
        var meta = Registry.Get("articles")!;
        var article = new Article { Title = "No authors" };
        await _repo.CreateAsync(article);
        await _repo.SyncManyToManyAsync(meta, article);

        var loaded = await _repo.GetByIdAsync(meta, article.Id) as Article;
        loaded!.AuthorIds.Should().BeEmpty();
    }
}
