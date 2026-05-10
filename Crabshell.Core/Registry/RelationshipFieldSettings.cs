namespace Crabshell.Core.Registry;

/// <summary>Runtime settings derived from <see cref="Attributes.Fields.RelationshipFieldAttribute"/>.</summary>
public sealed class RelationshipFieldSettings
{
    /// <summary>Slug of the related collection, used to populate the dropdown.</summary>
    public string Slug { get; init; } = default!;
}
