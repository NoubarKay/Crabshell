namespace Crabshell.Core.Registry;

/// <summary>Runtime settings derived from <see cref="Attributes.Fields.ManyToManyFieldAttribute"/>.</summary>
public sealed class ManyToManyFieldSettings
{
    /// <summary>Slug of the related collection, used to populate the dropdown and validate IDs.</summary>
    public string TargetSlug { get; init; } = default!;

    /// <summary>Name of the junction table linking the two collections.</summary>
    public string JoinTableName { get; init; } = default!;

    /// <summary>Join table column holding the owning (source) document ID.</summary>
    public string SourceColumn { get; init; } = default!;

    /// <summary>Join table column holding the related (target) document ID.</summary>
    public string TargetColumn { get; init; } = default!;
}
