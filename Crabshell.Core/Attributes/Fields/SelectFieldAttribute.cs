namespace Crabshell.Core.Attributes.Fields;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class SelectFieldAttribute : CrabshellFieldAttribute
{
    public string[] Options { get; set; } = [];
    public bool Multiple { get; set; } = false;
}