using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.Documents;
using Crabshell.Sample.Hooks;

namespace Crabshell.Sample.Collections;

[Collection("authors", Label = "Authors", Hooks = [typeof(AuthorHooks)])]
public class Author : CrabshellDocument
{
    [GridOptions(Visible = true, Order = 0, Filterable = true)]
    [TextField(Required = true, MaxLength = 120, Label = "Full Name")]
    public string Name { get; set; } = default!;

    [GridOptions(Visible = true, Order = 1, Width = 160)]
    [TextField(Required = true, MaxLength = 200, Label = "Slug")]
    public string Slug { get; set; } = default!;

    // ── CONTACT ─────────────────────────────────────────────
    [GridOptions(Visible = true, Order = 2, Width = 220)]
    [FieldGroup("Contact")]
    [TextField(MaxLength = 254, Label = "Email", Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    public string? Email { get; set; }

    // ── PROFILE ─────────────────────────────────────────────
    [GridOptions(Visible = false, Sortable = false)]
    [FieldGroup("Profile", Sidebar = true)]
    [TextField(MaxLength = -1, Label = "Bio")]
    public string? Bio { get; set; }

    [GridOptions(Visible = true, Order = 3, Width = 100, Sortable = true)]
    [FieldGroup("Profile", Sidebar = true)]
    [BoolField(Label = "Active", IsSwitch = true)]
    public bool IsActive { get; set; } = true;

    [GridOptions(Visible = false)]
    [FieldGroup("Profile", Sidebar = true)]
    [MediaField(Label = "Avatar", Accept = "image/*", MaxSizeMb = 2)]
    public string? Avatar { get; set; }

    // ── SOCIAL ──────────────────────────────────────────────
    [GridOptions(Visible = false)]
    [FieldGroup("Social")]
    [TextField(MaxLength = 100, Label = "Twitter / X Handle")]
    public string? Twitter { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Social")]
    [TextField(MaxLength = 200, Label = "LinkedIn URL")]
    public string? LinkedIn { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Social")]
    [TextField(MaxLength = 200, Label = "Website")]
    public string? Website { get; set; }
}
