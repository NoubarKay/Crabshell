namespace Crabshell.Core.Registry;

/// <summary>Runtime settings derived from <see cref="Attributes.Fields.BoolFieldAttribute"/>.</summary>
public sealed class BoolFieldSettings
{
    /// <summary>Render as a toggle switch instead of a checkbox.</summary>
    public bool IsSwitch { get; init; }
}