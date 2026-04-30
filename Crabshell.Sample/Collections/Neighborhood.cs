using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.Documents;

namespace Crabshell.Sample.Collections;

[Collection("neighborhoods", Label = "Neighborhoods")]
public class Neighborhood : CrabshellDocument
{
    [GridOptions(Visible = true, Order = 0, Filterable = true)]
    [TextField(Required = true, MaxLength = 120, Label = "Name")]
    public string Name { get; set; } = default!;

    [GridOptions(Visible = false)]
    [TextField(Required = true, MaxLength = 200, Label = "Slug")]
    public string Slug { get; set; } = default!;

    [GridOptions(Visible = true, Order = 1, Width = 160, Filterable = true)]
    [TextField(Required = true, MaxLength = 100, Label = "City")]
    public string City { get; set; } = default!;

    [GridOptions(Visible = true, Order = 2, Width = 80)]
    [TextField(Required = true, MaxLength = 2, Label = "State")]
    public string State { get; set; } = default!;

    [GridOptions(Visible = false)]
    [TextField(MaxLength = -1, Label = "Description")]
    public string? Description { get; set; }

    // STATS
    [GridOptions(Visible = true, Order = 3, Width = 140, Sortable = true)]
    [FieldGroup("Stats", Sidebar = true)]
    [NumberField(Label = "Walkability Score", Min = 0, Max = 100, Step = "1", Suffix = " / 100")]
    public int? WalkabilityScore { get; set; }

    [GridOptions(Visible = true, Order = 4, Width = 160, Sortable = true)]
    [FieldGroup("Stats", Sidebar = true)]
    [NumberField(Label = "Median Home Price", Min = 0, Step = "1000", Prefix = "$", Decimals = 0, Format = "c")]
    public decimal? MedianPrice { get; set; }

    [GridOptions(Visible = true, Order = 5, Width = 120, Sortable = true)]
    [FieldGroup("Stats", Sidebar = true)]
    [NumberField(Label = "Avg Days on Market", Min = 0, Step = "1")]
    public int? AvgDaysOnMarket { get; set; }

    // FLAGS
    [GridOptions(Visible = false)]
    [FieldGroup("Features", Sidebar = true)]
    [BoolField(Label = "Top Rated Schools", IsSwitch = true)]
    public bool TopRatedSchools { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Features", Sidebar = true)]
    [BoolField(Label = "Gated Community")]
    public bool GatedCommunity { get; set; }
}
