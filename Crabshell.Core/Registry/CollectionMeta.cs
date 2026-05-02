using Crabshell.Core.Attributes;
using Crabshell.Core.SaveActions;

namespace Crabshell.Core.Registry;

public sealed class CollectionMeta
{
    public string Slug { get; init; }
    public string Label { get; init; }
    public Type ClrType { get; init; }
    public IReadOnlyList<FieldMeta> Fields { get; init; }
    public SaveOption SaveOption { get; init; }
    public IReadOnlyList<ICustomSaveAction> CustomSaveActions { get; init; }
}