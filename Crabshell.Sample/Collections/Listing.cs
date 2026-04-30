using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.Documents;

namespace Crabshell.Sample.Collections;

public enum ListingStatus { Active, Pending, Sold, OffMarket, ComingSoon }

[Flags]
public enum PropertyType
{
    House, 
    Condo, 
    Townhouse, 
    MultiFamily, 
    Land, 
    Commercial
}

public enum PropertyCondition { Excellent, Good, Fair, NeedsWork }

[Collection("listings", Label = "Listings")]
public class Listing : CrabshellDocument
{
    [GridOptions(Visible = true, Order = 0, Filterable = true)]
    [TextField(Required = true, MaxLength = 200, Label = "Address")]
    public string Address { get; set; } = default!;

    [GridOptions(Visible = false)]
    [TextField(Required = true, MaxLength = 200, Label = "Slug")]
    public string Slug { get; set; } = default!;

    [GridOptions(Visible = false)]
    [TextField(MaxLength = 200, Label = "Headline")]
    public string? Headline { get; set; }

    // ── DETAILS ────────────────────────────────────────────
    [GridOptions(Visible = false)]
    [FieldGroup("Details")]
    [TextField(MaxLength = -1, Label = "Description")]
    public string? Description { get; set; }

    [GridOptions(Visible = true, Order = 1, Width = 140, Sortable = true, Filterable = true)]
    [FieldGroup("Details")]
    [SelectField(Label = "Property Type", Multiple = true)]
    public PropertyType PropertyType { get; set; }

    [GridOptions(Visible = true, Order = 2, Width = 100, Sortable = true)]
    [FieldGroup("Details")]
    [NumberField(Label = "Bedrooms", Min = 0, Max = 20, Step = "1")]
    public int Bedrooms { get; set; }

    [GridOptions(Visible = true, Order = 3, Width = 110, Sortable = true)]
    [FieldGroup("Details")]
    [NumberField(Label = "Bathrooms", Min = 0, Max = 20, Step = "0.5", Decimals = 1)]
    public decimal Bathrooms { get; set; }

    [GridOptions(Visible = true, Order = 4, Width = 120, Sortable = true)]
    [FieldGroup("Details")]
    [NumberField(Label = "Square Feet", Min = 0, Step = "1", Suffix = " sqft")]
    public int? SquareFeet { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Details")]
    [NumberField(Label = "Lot Size", Min = 0, Step = "0.01", Suffix = " ac", Decimals = 2)]
    public decimal? LotSizeAcres { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Details")]
    [NumberField(Label = "Year Built", Min = 1800, Max = 2026, Step = "1")]
    public int? YearBuilt { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Details")]
    [NumberField(Label = "Floors", Min = 1, Max = 10, Step = "1")]
    public int? Floors { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Details")]
    [NumberField(Label = "Parking Spaces", Min = 0, Max = 20, Step = "1")]
    public int? ParkingSpaces { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Details")]
    [SelectField(Label = "Condition")]
    public PropertyCondition? Condition { get; set; }

    // ── PRICING ────────────────────────────────────────────
    [GridOptions(Visible = true, Order = 5, Width = 160, Sortable = true)]
    [FieldGroup("Pricing", Sidebar = true)]
    [NumberField(Required = true, Label = "List Price", Min = 0, Step = "1000", Prefix = "$", Decimals = 0)]
    public decimal Price { get; set; }

    [GridOptions(Visible = true, Order = 6, Width = 140, Sortable = true, Filterable = true)]
    [FieldGroup("Pricing", Sidebar = true)]
    [SelectField(Required = true, Label = "Status")]
    public ListingStatus Status { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Pricing", Sidebar = true)]
    [NumberField(Label = "HOA Fee", Min = 0, Step = "10", Prefix = "$", Suffix = " /mo", Decimals = 0)]
    public decimal? HoaFee { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Pricing", Sidebar = true)]
    [NumberField(Label = "Annual Taxes", Min = 0, Step = "100", Prefix = "$", Decimals = 0)]
    public decimal? AnnualTaxes { get; set; }

    // ── RELATIONSHIPS ──────────────────────────────────────
    [GridOptions(Visible = true, Order = 7, Width = 180)]
    [FieldGroup("People", Sidebar = true)]
    [RelationshipField(typeof(Agent), Label = "Listing Agent")]
    public Guid AgentId { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("People", Sidebar = true)]
    [RelationshipField(typeof(Neighborhood), Label = "Neighborhood")]
    public Guid NeighborhoodId { get; set; }

    // ── DATES ──────────────────────────────────────────────
    [GridOptions(Visible = true, Order = 8, Width = 140, Sortable = true)]
    [FieldGroup("Dates", Sidebar = true)]
    [DateTimeField(Label = "Listed On", HasTime = false)]
    public DateTime? ListedAt { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Dates", Sidebar = true)]
    [DateTimeField(Label = "Open House", HasTime = true, ShowNowButton = false, ShowButton = true)]
    public DateTime? OpenHouseAt { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Dates", Sidebar = true)]
    [DateTimeField(Label = "Offer Deadline", HasTime = true, ShowButton = true)]
    public DateTime? OfferDeadline { get; set; }

    // ── FEATURES ───────────────────────────────────────────
    [GridOptions(Visible = false)]
    [FieldGroup("Features", Sidebar = true)]
    [BoolField(Label = "Pool", IsSwitch = true)]
    public bool HasPool { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Features", Sidebar = true)]
    [BoolField(Label = "Garage", IsSwitch = true)]
    public bool HasGarage { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Features", Sidebar = true)]
    [BoolField(Label = "Pet Friendly", IsSwitch = true)]
    public bool PetFriendly { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Features", Sidebar = true)]
    [BoolField(Label = "New Construction", IsSwitch = true)]
    public bool NewConstruction { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Features", Sidebar = true)]
    [BoolField(Label = "Furnished", IsSwitch = true)]
    public bool Furnished { get; set; }

    [GridOptions(Visible = true, Order = 9, Width = 130, Sortable = true)]
    [FieldGroup("Features", Sidebar = true)]
    [BoolField(Label = "Featured Listing", IsSwitch = true)]
    public bool IsFeatured { get; set; }

    // ── SEO ────────────────────────────────────────────────
    [GridOptions(Visible = false)]
    [FieldGroup("SEO")]
    [TextField(MaxLength = 200, Label = "Meta Title")]
    public string? MetaTitle { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("SEO")]
    [TextField(MaxLength = 300, Label = "Meta Description")]
    public string? MetaDescription { get; set; }
}
