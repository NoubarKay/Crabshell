namespace Crabshell.Core.Attributes.Fields;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class GridOptionsAttribute : Attribute
{
    /// <summary>Show this field as a column in the list view. Default true.</summary>
    public bool Visible { get; set; } = true;

    /// <summary>Column header label. Defaults to the field's Label.</summary>
    public string? Label { get; set; }

    /// <summary>Column width in pixels. 0 = auto.</summary>
    public int Width { get; set; } = 0;

    /// <summary>Allow sorting by this column.</summary>
    public bool Sortable { get; set; } = true;

    /// <summary>Allow filtering by this column.</summary>
    public bool Filterable { get; set; } = false;

    /// <summary>Display order in the grid. Lower = further left.</summary>
    public int Order { get; set; } = 0;
}