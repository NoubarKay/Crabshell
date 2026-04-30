namespace Crabshell.Core.Attributes.Fields;

/// <summary>
/// Maps to a varchar or text column. Use MaxLength = -1 for unbounded text.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class TextFieldAttribute : CrabshellFieldAttribute
{
    /// <summary>Max character length. -1 = unlimited (TEXT column). Default 255.</summary>
    public int MaxLength { get; set; } = 255;
 
    public int MinLength { get; set; } = 0;
 
    /// <summary>Regex pattern for validation (applied in CollectionValidator).</summary>
    public string? Pattern { get; set; }
}