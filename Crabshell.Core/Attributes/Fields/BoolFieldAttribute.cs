namespace Crabshell.Core.Attributes.Fields;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class BoolFieldAttribute : CrabshellFieldAttribute
{
    public bool IsSwitch { get; set; }
}