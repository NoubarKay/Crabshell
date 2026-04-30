namespace Crabshell.Core.Attributes.Fields;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
public sealed class FieldGroupAttribute : Attribute
{
    public string Name { get; }
    public bool Sidebar { get; set; } = false;
    public FieldGroupAttribute(string name) => Name = name;
    
}