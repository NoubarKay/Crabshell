using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.Documents;

namespace Crabshell.Sample.Collections;

[Collection("agencies", 
    Label = "Agencies",
    SaveOptions = SaveOption.Save | SaveOption.SaveAndClone |  SaveOption.SaveAndGoToNext
    )]
public class Agency : CrabshellDocument
{
    [GridOptions(Visible = true, Order = 0, Filterable = true)]
    [TextField(Required = true, MaxLength = 120, Label = "Name")]
    public string Name { get; set; } = default!;

    [GridOptions(Visible = false)]
    [TextField(Required = true, MaxLength = 200, Label = "Slug")]
    public string Slug { get; set; } = default!;

    [GridOptions(Visible = true, Order = 1, Width = 180)]
    [TextField(MaxLength = 20, Label = "Phone")]
    public string? Phone { get; set; }

    [GridOptions(Visible = true, Order = 2, Width = 220)]
    [TextField(MaxLength = 254, Label = "Email", Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    public string? Email { get; set; }

    [GridOptions(Visible = false)]
    [TextField(MaxLength = 500, Label = "Website")]
    public string? Website { get; set; }

    [GridOptions(Visible = true, Order = 3, Width = 100, Sortable = true)]
    [NumberField(Label = "Founded", Min = 1800, Max = 2100, Step = "1")]
    public int? Founded { get; set; }

    [GridOptions(Visible = false)]
    [TextField(MaxLength = -1, Label = "About")]
    public string? About { get; set; }
}
