namespace Crabshell.Core.Attributes;

/// <summary>
/// Marks a CrabshellDocument subclass as a managed collection.
/// The slug becomes the API route segment and the Postgres table name.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CollectionAttribute : Attribute
{
    /// <summary>
    /// URL-safe slug for this collection, e.g. "articles", "authors".
    /// Used as the table name and REST route segment.
    /// </summary>
    public string Slug { get; }
 
    /// <summary>
    /// Human-readable label shown in the admin UI. Defaults to the slug if not set.
    /// </summary>
    public string? Label { get; set; }
    
    /// <summary>Flags controlling which built-in save actions appear in the edit page split button. Default <see cref="SaveOption.Save"/>.</summary>
    public SaveOption SaveOptions { get; set; } = SaveOption.Save;

    /// <summary>
    /// Custom save action types added to the split button after built-in entries.
    /// Each type must implement <see cref="SaveActions.ICustomSaveAction"/> and have a public parameterless constructor.
    /// </summary>
    public Type[] CustomSaveOptions { get; set; } = [];

    /// <summary>
    /// Custom bulk action types shown in the Actions button on the collection list.
    /// Each type must implement <see cref="BulkActions.IBulkAction"/> and have a public parameterless constructor.
    /// </summary>
    public Type[] CustomBulkOptions { get; set; } = [];
 
    public CollectionAttribute(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Collection slug must not be empty.", nameof(slug));
 
        Slug = slug.ToLowerInvariant().Trim();
    }
}

/// <summary>Flags controlling which save actions appear in the edit page split button.</summary>
[Flags]
public enum SaveOption
{
    /// <summary>Navigate to the collection list after saving.</summary>
    Save,
    /// <summary>Stay on the edit page after saving (updates the URL to the new ID when creating).</summary>
    SaveAndStayHere,
    /// <summary>Duplicate the saved document and open the clone for editing.</summary>
    SaveAndClone,
    /// <summary>Navigate to the next document in the list after saving.</summary>
    SaveAndGoToNext
}