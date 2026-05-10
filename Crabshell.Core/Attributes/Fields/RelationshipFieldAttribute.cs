namespace Crabshell.Core.Attributes.Fields;

/// <summary>
/// Maps to a UUID foreign key column pointing to another collection.
/// The admin UI renders a dropdown populated from the related collection,
/// filtered by the first <see cref="TextFieldAttribute"/> on that type.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class RelationshipFieldAttribute : CrabshellFieldAttribute
{
    /// <summary>The <see cref="Documents.CrabshellDocument"/> subclass this field relates to.</summary>
    public Type RelatesTo { get; }

    /// <summary>
    /// Initialises the attribute with the target collection type.
    /// Throws <see cref="ArgumentException"/> if <paramref name="relatesTo"/> does not extend <see cref="Documents.CrabshellDocument"/>.
    /// </summary>
    public RelationshipFieldAttribute(Type relatesTo)
    {
        if (!relatesTo.IsSubclassOf(typeof(Documents.CrabshellDocument)))
            throw new ArgumentException(
                $"{relatesTo.Name} must extend CrabshellDocument.",
                nameof(relatesTo));

        RelatesTo = relatesTo;
    }
}