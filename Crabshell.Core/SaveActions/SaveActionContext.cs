using Crabshell.Core.Services;

namespace Crabshell.Core.SaveActions;

/// <summary>Context passed to a custom save action after the document has been successfully saved.</summary>
/// <param name="Slug">The collection slug, e.g. <c>"articles"</c>.</param>
/// <param name="SaveId">ID of the document that was just saved.</param>
/// <param name="CollectionService">Built-in service for CRUD operations on the collection.</param>
/// <param name="Services">Full DI container — use <c>GetRequiredService&lt;T&gt;()</c> to resolve dependencies.</param>
public record SaveActionContext(
    string Slug,
    Guid SaveId,
    ICollectionService CollectionService,
    IServiceProvider Services
);

/// <summary>
/// A custom entry in the Save split button on the collection edit page.
/// Implement this interface and register the type via <c>CustomSaveOptions</c> on <c>[Collection]</c>.
/// The type must have a public parameterless constructor; resolve dependencies via <c>ctx.Services</c>.
/// </summary>
public interface ICustomSaveAction
{
    /// <summary>Text shown in the split button dropdown.</summary>
    string Label { get; }

    /// <summary>
    /// Unique key identifying this action on the collection.
    /// Must not be <c>"stay"</c>, <c>"clone"</c>, or <c>"next"</c> (reserved by built-in actions).
    /// </summary>
    string Value { get; }

    /// <summary>
    /// Invoked after the document has been saved successfully.
    /// Return a URL to navigate to, or <c>null</c> to navigate to the collection list.
    /// </summary>
    Task<string?> ExecuteAsync(SaveActionContext context);
}