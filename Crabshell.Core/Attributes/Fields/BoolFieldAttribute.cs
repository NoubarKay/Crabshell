namespace Crabshell.Core.Attributes.Fields;

/// <summary>Maps to a boolean column. Renders a checkbox or toggle switch in the admin UI.</summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class BoolFieldAttribute : CrabshellFieldAttribute
{
    /// <summary>Render as a toggle switch instead of a checkbox. Default false.</summary>
    public bool IsSwitch { get; set; }
}