using Crabshell.Core.Attributes;
using Crabshell.Core.BulkActions;
using Crabshell.Core.SaveActions;

namespace Crabshell.Core.Registry;

/// <summary>Runtime descriptor for a registered collection or singleton, built from attribute metadata at startup.</summary>
public sealed class CollectionMeta
{
    /// <summary>URL-safe identifier used as the table name and admin route segment.</summary>
    public required string Slug { get; init; }

    /// <summary>Human-readable label shown in the admin sidebar.</summary>
    public required string Label { get; init; }

    /// <summary>The CLR type of the <see cref="Documents.CrabshellDocument"/> subclass.</summary>
    public required Type ClrType { get; init; }

    /// <summary>Ordered list of field descriptors for this collection.</summary>
    public required IReadOnlyList<FieldMeta> Fields { get; init; }

    /// <summary>Flags controlling which built-in save actions appear on the edit page.</summary>
    public SaveOption SaveOption { get; init; }

    /// <summary>Custom save actions added to the split button, in declared order. <c>null</c> if none.</summary>
    public IReadOnlyList<ICustomSaveAction>? CustomSaveActions { get; init; }

    /// <summary>Custom bulk actions available on the collection list. <c>null</c> if none.</summary>
    public IReadOnlyList<IBulkAction>? CustomBulkOptions { get; init; }

    /// <summary>Hook types registered for this collection, in declaration order. <c>null</c> if none.</summary>
    public IReadOnlyList<Type>? HookTypes { get; init; }

    /// <summary><c>true</c> when this collection was declared with <c>[Single]</c> and has exactly one document.</summary>
    public bool IsSingle { get; init; }
}