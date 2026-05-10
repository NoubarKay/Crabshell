namespace Crabshell.Core.Registry;

/// <summary>Runtime settings derived from <see cref="Attributes.Fields.MediaFieldAttribute"/>.</summary>
public sealed class MediaFieldSettings
{
    /// <summary>HTML <c>accept</c> attribute for the file input, e.g. <c>"image/*"</c>.</summary>
    public required string Accept { get; init; }

    /// <summary>Maximum upload size in megabytes. Enforced client-side and server-side.</summary>
    public required int MaxSizeMb { get; init; }
}