using Crabshell.Core.Attributes;
using Crabshell.Core.Registry;
using Crabshell.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Crabshell.Tests.Integration.Registry;

public class CollectionRegistryTests : IntegrationTestBase
{
    public CollectionRegistryTests() : base(typeof(Article)) { }

    [Fact]
    public void Registers_collections_from_assembly()
    {
        var collections = Registry.GetAll();
        collections.Should().Contain(c => c.Slug == "articles");
        collections.Should().Contain(c => c.Slug == "authors");
    }

    [Fact]
    public void Collection_has_correct_fields()
    {
        var meta = Registry.Get("articles");
        meta.Should().NotBeNull();
        meta!.Fields.Should().Contain(f => f.PropertyName == "Title");
        meta.Fields.Should().Contain(f => f.PropertyName == "Body");
        meta.Fields.Should().Contain(f => f.PropertyName == "Views");
        meta.Fields.Should().Contain(f => f.PropertyName == "Published");
        meta.Fields.Should().Contain(f => f.PropertyName == "PublishedAt");
    }

    [Fact]
    public void Field_types_are_correct()
    {
        var meta = Registry.Get("articles")!;
        meta.Fields.First(f => f.PropertyName == "Title").FieldType.Should().Be(FieldType.Text);
        meta.Fields.First(f => f.PropertyName == "Views").FieldType.Should().Be(FieldType.Number);
        meta.Fields.First(f => f.PropertyName == "Published").FieldType.Should().Be(FieldType.Bool);
        meta.Fields.First(f => f.PropertyName == "PublishedAt").FieldType.Should().Be(FieldType.DateTime);
    }

    [Fact]
    public void TextField_settings_are_mapped()
    {
        var meta = Registry.Get("articles")!;
        var title = meta.Fields.First(f => f.PropertyName == "Title");
        title.TextSettings.Should().NotBeNull();
        title.TextSettings!.MaxLength.Should().Be(255);

        var body = meta.Fields.First(f => f.PropertyName == "Body");
        body.TextSettings!.MaxLength.Should().Be(-1);
    }

    [Fact]
    public void CollectionAttribute_throws_on_empty_slug()
    {
        var act = () => new Crabshell.Core.Attributes.CollectionAttribute("");
        act.Should().Throw<ArgumentException>();
    }
}
