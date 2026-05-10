using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.Documents;

namespace Crabshell.Sample.Singles;

[Single("site_settings", Label = "Site Settings")]
public class SiteSettings : CrabshellDocument
{
    [FieldGroup("General")]
    [TextField(Required = true, MaxLength = 100, Label = "Site Name")]
    public string SiteName { get; set; } = default!;

    [FieldGroup("General")]
    [TextField(MaxLength = 255, Label = "Tagline")]
    public string? Tagline { get; set; }

    [FieldGroup("General")]
    [TextField(MaxLength = 254, Label = "Contact Email", Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    public string? ContactEmail { get; set; }

    [FieldGroup("General")]
    [TextField(MaxLength = 20, Label = "Contact Phone")]
    public string? ContactPhone { get; set; }

    [FieldGroup("SEO", Sidebar = true)]
    [TextField(MaxLength = 160, Label = "Meta Description")]
    public string? MetaDescription { get; set; }

    [FieldGroup("SEO", Sidebar = true)]
    [TextField(MaxLength = 500, Label = "Meta Keywords")]
    public string? MetaKeywords { get; set; }
}
