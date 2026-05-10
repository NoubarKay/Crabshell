namespace Crabshell.Core.Registry;

/// <summary>Runtime settings derived from <see cref="Attributes.Fields.TextFieldAttribute"/>.</summary>
public sealed class TextFieldSettings
{
    /// <summary>Max character length. -1 = unlimited.</summary>
    public int MaxLength { get; init; }

    /// <summary>Minimum character length. 0 = no minimum.</summary>
    public int MinLength { get; init; }

    /// <summary>Regex pattern for validation, or <c>null</c> if not set.</summary>
    public string? Pattern { get; init; }
}
