namespace Crabshell.Core.Attributes;

/// <summary>
/// Marks a CrabshellDocument subclass as a singleton — exactly one document exists.
/// The admin UI bypasses the list page and goes directly to the edit page.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SingleAttribute : Attribute
{
    /// <summary>
    /// URL-safe slug, e.g. "site-settings". Used as the table name and route segment.
    /// </summary>
    public string Slug { get; }

    /// <summary>
    /// Human-readable label shown in the admin sidebar. Defaults to the slug if not set.
    /// </summary>
    public string? Label { get; set; }

    public SingleAttribute(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Single slug must not be empty.", nameof(slug));

        Slug = slug.ToLowerInvariant().Trim();
    }
}
