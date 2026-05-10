using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.Documents;

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
}

[Crabshell.Core.Attributes.Collection("authors")]
public class Author : CrabshellDocument
{
    [TextField(Label = "Name", MaxLength = 255)]
    public string Name { get; set; } = default!;

    [TextField(Label = "Bio", MaxLength = -1)]
    public string? Bio { get; set; }
}
