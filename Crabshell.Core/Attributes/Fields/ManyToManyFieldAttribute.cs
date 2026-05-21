namespace Crabshell.Core.Attributes.Fields;

/// <summary>
/// Maps to a many-to-many relationship backed by a junction table. Applied to a
/// <c>List&lt;Guid&gt;</c> (or <c>ICollection&lt;Guid&gt;</c>) property holding the related record IDs.
/// The property itself is not stored as a column; a join table linking the two collections
/// is created and managed automatically. The admin UI renders a multi-select dropdown.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ManyToManyFieldAttribute : CrabshellFieldAttribute
{
    /// <summary>The <see cref="Documents.CrabshellDocument"/> subclass this field relates to.</summary>
    public Type RelatesTo { get; }

    /// <summary>Optional override for the join table name. Defaults to <c>{sourceSlug}_{targetSlug}</c>.</summary>
    public string? JoinTableName { get; set; }

    /// <summary>
    /// Initialises the attribute with the target collection type.
    /// Throws <see cref="ArgumentException"/> if <paramref name="relatesTo"/> does not extend <see cref="Documents.CrabshellDocument"/>.
    /// </summary>
    public ManyToManyFieldAttribute(Type relatesTo)
    {
        if (!relatesTo.IsSubclassOf(typeof(Documents.CrabshellDocument)))
            throw new ArgumentException(
                $"{relatesTo.Name} must extend CrabshellDocument.",
                nameof(relatesTo));

        RelatesTo = relatesTo;
    }
}
