using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.Documents;
using Crabshell.Sample.Hooks;

namespace Crabshell.Sample.Collections;

[Collection("categories", Label = "Categories", Hooks = [typeof(CategoryAuditHook)])]
public class Category : CrabshellDocument
{
    [GridOptions(Visible = true, Order = 0, Filterable = true)]
    [TextField(Required = true, MaxLength = 100, Label = "Name")]
    public string Name { get; set; } = default!;

    [GridOptions(Visible = true, Order = 1, Width = 160)]
    [TextField(Required = true, MaxLength = 100, Label = "Slug")]
    public string Slug { get; set; } = default!;

    [GridOptions(Visible = false)]
    [TextField(MaxLength = 500, Label = "Description")]
    public string? Description { get; set; }

    [GridOptions(Visible = false)]
    [TextField(MaxLength = 20, Label = "Color (hex, e.g. #3b82f6)")]
    public string? Color { get; set; }
}
