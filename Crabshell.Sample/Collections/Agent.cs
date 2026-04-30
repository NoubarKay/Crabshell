using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.Documents;

namespace Crabshell.Sample.Collections;

[Collection("agents", Label = "Agents")]
public class Agent : CrabshellDocument
{
    [GridOptions(Visible = true, Order = 0, Filterable = true)]
    [TextField(Required = true, MaxLength = 120, Label = "Full Name")]
    public string Name { get; set; } = default!;

    [GridOptions(Visible = false)]
    [TextField(Required = true, MaxLength = 200, Label = "Slug")]
    public string Slug { get; set; } = default!;

    // CONTACT
    [GridOptions(Visible = true, Order = 1, Width = 220)]
    [FieldGroup("Contact")]
    [TextField(Required = true, MaxLength = 254, Label = "Email", Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    public string Email { get; set; } = default!;

    [GridOptions(Visible = true, Order = 2, Width = 160)]
    [FieldGroup("Contact")]
    [TextField(MaxLength = 20, Label = "Phone")]
    public string? Phone { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Contact")]
    [TextField(MaxLength = 50, Label = "License Number")]
    public string? LicenseNumber { get; set; }

    // PROFILE
    [GridOptions(Visible = false)]
    [FieldGroup("Profile", Sidebar = true)]
    [TextField(MaxLength = -1, Label = "Bio")]
    public string? Bio { get; set; }

    [GridOptions(Visible = true, Order = 3, Width = 130, Sortable = true)]
    [FieldGroup("Profile", Sidebar = true)]
    [NumberField(Label = "Years Experience", Min = 0, Max = 60, Step = "1")]
    public int? YearsExperience { get; set; }

    [GridOptions(Visible = true, Order = 4, Width = 100, Sortable = true)]
    [FieldGroup("Profile", Sidebar = true)]
    [NumberField(Label = "Rating", Min = 0, Max = 5, Step = "0.1", Decimals = 1)]
    public decimal? Rating { get; set; }

    // AGENCY
    [GridOptions(Visible = true, Order = 5, Width = 180)]
    [FieldGroup("Agency", Sidebar = true)]
    [RelationshipField(typeof(Agency), Label = "Agency")]
    public Guid AgencyId { get; set; }

    [GridOptions(Visible = true, Order = 6, Width = 110, Sortable = true)]
    [FieldGroup("Agency", Sidebar = true)]
    [BoolField(Label = "Active", IsSwitch = true)]
    public bool IsActive { get; set; } = true;
}
