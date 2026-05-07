namespace Crabshell.Core.Attributes.Fields;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class MediaFieldAttribute : CrabshellFieldAttribute
{
    /// <summary>Max allowed file size in bytes. Null = no limit.</summary>
    public int? MaxSizeBytes { get; set; }
}