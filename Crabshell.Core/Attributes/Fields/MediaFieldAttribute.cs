namespace Crabshell.Core.Attributes.Fields;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class MediaFieldAttribute : CrabshellFieldAttribute
{
    public string Accept { get; set; } = "image/*";
    public int MaxSizeMb { get; set; } = 10;
}