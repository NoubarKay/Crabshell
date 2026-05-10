namespace Crabshell.Core.Attributes.Fields;

/// <summary>
/// Stores a relative file path as a varchar column.
/// Renders a drag-and-drop file upload widget with preview and clear button in the admin UI.
/// Requires an <see cref="Storage.IStorageProvider"/> to be registered.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class MediaFieldAttribute : CrabshellFieldAttribute
{
    /// <summary>HTML <c>accept</c> attribute passed to the file input, e.g. <c>"image/*"</c> or <c>".pdf,.docx"</c>. Default <c>"image/*"</c>.</summary>
    public string Accept { get; set; } = "image/*";

    /// <summary>Maximum upload size in megabytes. Enforced client-side and server-side. Default 10.</summary>
    public int MaxSizeMb { get; set; } = 10;
}