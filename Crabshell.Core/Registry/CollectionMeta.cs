using Crabshell.Core.Attributes;
using Crabshell.Core.BulkActions;
using Crabshell.Core.SaveActions;

namespace Crabshell.Core.Registry;

public sealed class CollectionMeta
{
    public required string Slug { get; init; }
    public required string Label { get; init; }
    public required Type ClrType { get; init; }
    public required IReadOnlyList<FieldMeta> Fields { get; init; }
    public SaveOption SaveOption { get; init; }
    public IReadOnlyList<ICustomSaveAction>? CustomSaveActions { get; init; }
    public IReadOnlyList<IBulkAction>? CustomBulkOptions { get; init; }
    public bool IsSingle { get; init; }
}