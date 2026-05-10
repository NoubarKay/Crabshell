namespace Crabshell.Core.Attributes.Fields;

/// <summary>
/// Maps to an integer (enum) or varchar(255) (string options) column.
/// When the property type is an enum, options are derived automatically from its member names.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class SelectFieldAttribute : CrabshellFieldAttribute
{
    /// <summary>Explicit string options. Ignored when the property type is an enum.</summary>
    public string[] Options { get; set; } = [];

    /// <summary>Allow multiple selections. Stored as a comma-separated string.</summary>
    public bool Multiple { get; set; } = false;
}