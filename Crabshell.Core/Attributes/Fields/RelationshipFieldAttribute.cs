namespace Crabshell.Core.Attributes.Fields;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class RelationshipFieldAttribute : CrabshellFieldAttribute
{
    public Type RelatesTo { get; }

    public RelationshipFieldAttribute(Type relatesTo)
    {
        if (!relatesTo.IsSubclassOf(typeof(Documents.CrabshellDocument)))
            throw new ArgumentException(
                $"{relatesTo.Name} must extend CrabshellDocument.",
                nameof(relatesTo));

        RelatesTo = relatesTo;
    }
}