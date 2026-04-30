using Crabshell.Core.Attributes;

namespace Crabshell.Core.Registry;

public sealed class CollectionMeta
{
    public string Slug { get; init; }
    public string Label { get; init; }
    public Type ClrType { get; init; }
    public IReadOnlyList<FieldMeta> Fields { get; init; }
    public SaveOption SaveOption { get; init; }
}