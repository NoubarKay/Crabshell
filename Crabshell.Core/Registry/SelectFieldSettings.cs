namespace Crabshell.Core.Registry;

/// <summary>Runtime settings derived from <see cref="Attributes.Fields.SelectFieldAttribute"/>.</summary>
public sealed class SelectFieldSettings
{
    /// <summary>
    /// Allowed string options. Empty when the field is backed by an enum
    /// (options are resolved at render time from enum members).
    /// </summary>
    public string[] Options { get; init; } = [];
}
