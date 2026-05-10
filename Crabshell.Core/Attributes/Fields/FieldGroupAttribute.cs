namespace Crabshell.Core.Attributes.Fields;

/// <summary>
/// Groups fields together under a named heading in the admin edit page.
/// Apply to a property alongside any field attribute to assign it to a group.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
public sealed class FieldGroupAttribute : Attribute
{
    /// <summary>Group heading shown in the admin UI.</summary>
    public string Name { get; }

    /// <summary>Place the group in the right sidebar instead of the main content area. Default false.</summary>
    public bool Sidebar { get; set; } = false;

    /// <summary>Initialises the attribute with the required group name.</summary>
    public FieldGroupAttribute(string name) => Name = name;
}