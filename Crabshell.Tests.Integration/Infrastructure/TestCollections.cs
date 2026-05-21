using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.Documents;
using SingleAttribute = Crabshell.Core.Attributes.SingleAttribute;

namespace Crabshell.Tests.Integration.Infrastructure;

[Crabshell.Core.Attributes.Collection("articles")]
public class Article : CrabshellDocument
{
    [TextField(Label = "Title", MaxLength = 255)]
    public string Title { get; set; } = default!;

    [TextField(Label = "Body", MaxLength = -1)]
    public string? Body { get; set; }

    [NumberField(Label = "Views")]
    public int Views { get; set; }

    [BoolField(Label = "Published")]
    public bool Published { get; set; }

    [DateTimeField(Label = "Published At")]
    public DateTime? PublishedAt { get; set; }

    [ManyToManyField(typeof(Author), Label = "Authors")]
    public List<Guid> AuthorIds { get; set; } = [];
}

[Crabshell.Core.Attributes.Collection("authors")]
public class Author : CrabshellDocument
{
    [TextField(Label = "Name", MaxLength = 255)]
    public string Name { get; set; } = default!;

    [TextField(Label = "Bio", MaxLength = -1)]
    public string? Bio { get; set; }
}

[Single("site_settings", Label = "Site Settings")]
public class SiteSettings : CrabshellDocument
{
    [TextField(Label = "Site Name", MaxLength = 255)]
    public string SiteName { get; set; } = default!;

    [TextField(Label = "Tagline", MaxLength = 255)]
    public string? Tagline { get; set; }
}
