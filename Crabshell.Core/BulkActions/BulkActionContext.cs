using Crabshell.Core.Services;

namespace Crabshell.Core.BulkActions;

/// <summary>Context passed to a bulk action when it is executed from the collection list.</summary>
/// <param name="Slug">The collection slug, e.g. <c>"articles"</c>.</param>
/// <param name="SelectedIds">IDs of the documents selected by the user.</param>
/// <param name="CollectionService">Built-in service for CRUD operations on the collection.</param>
/// <param name="Services">Full DI container — use <c>GetRequiredService&lt;T&gt;()</c> to resolve dependencies.</param>
public record BulkActionContext(
    string Slug,
    IReadOnlyList<Guid> SelectedIds,
    ICollectionService CollectionService,
    IServiceProvider Services
);

/// <summary>
/// An action that operates on multiple selected documents from the collection list.
/// Implement this interface and register the type via <c>CustomBulkOptions</c> on <c>[Collection]</c>.
/// The type must have a public parameterless constructor.
/// </summary>
public interface IBulkAction
{
    /// <summary>Text shown in the bulk Actions dropdown.</summary>
    string Label { get; }

    /// <summary>Unique key identifying this action. Must not conflict with reserved keys.</summary>
    string Value { get; }

    /// <summary>Invoked after the user confirms the bulk action. Receives context for the selected documents.</summary>
    Task ExecuteAsync(BulkActionContext context);
}
