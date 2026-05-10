using Crabshell.Core;
using Crabshell.Core.Services;
using Crabshell.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Tests.Integration.Singles;

public class SingleTests : IntegrationTestBase
{
    private readonly ICollectionService _service;

    public SingleTests() : base(typeof(Article))
    {
        _service = Services.GetRequiredService<ICollectionService>();
        SchemaDiff.ApplyDiffAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public void Registry_lists_single_separately_from_collections()
    {
        Registry.GetAll().Should().NotContain(c => c.Slug == "site_settings");
        Registry.GetAllSingles().Should().Contain(c => c.Slug == "site_settings");
    }

    [Fact]
    public void Single_has_correct_fields()
    {
        var meta = Registry.Get("site_settings");
        meta.Should().NotBeNull();
        meta!.IsSingle.Should().BeTrue();
        meta.Fields.Should().Contain(f => f.PropertyName == "SiteName");
        meta.Fields.Should().Contain(f => f.PropertyName == "Tagline");
    }

    [Fact]
    public async Task GetSingleAsync_auto_creates_document_on_first_access()
    {
        var result = await _service.GetSingleAsync("site_settings");

        result.Should().BeOfType<Result<Core.Documents.CrabshellDocument>.Ok>();
        var doc = ((Result<Core.Documents.CrabshellDocument>.Ok)result).Value;
        doc.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetSingleAsync_returns_same_document_on_subsequent_calls()
    {
        var first  = await _service.GetSingleAsync("site_settings");
        var second = await _service.GetSingleAsync("site_settings");

        var id1 = ((Result<Core.Documents.CrabshellDocument>.Ok)first).Value.Id;
        var id2 = ((Result<Core.Documents.CrabshellDocument>.Ok)second).Value.Id;

        id1.Should().Be(id2);
    }

    [Fact]
    public async Task GetSingleAsync_returns_NotFound_for_unknown_slug()
    {
        var result = await _service.GetSingleAsync("does_not_exist");
        result.Should().BeOfType<Result<Core.Documents.CrabshellDocument>.NotFound>();
    }
}
