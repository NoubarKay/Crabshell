namespace Crabshell.Core.Attributes.Fields;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class MediaField : CrabshellFieldAttribute
{
    public string Accepts { get; set; }
    public string MaxSizeMb { get; set; }
}