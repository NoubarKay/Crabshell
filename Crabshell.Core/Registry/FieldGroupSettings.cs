namespace Crabshell.Core.Registry;

/// <summary>Runtime settings derived from <see cref="Attributes.Fields.FieldGroupAttribute"/>.</summary>
public sealed class FieldGroupSettings
{
    /// <summary>Group heading shown in the admin UI.</summary>
    public string Name { get; init; } = default!;

    /// <summary>When <c>true</c>, the group is placed in the right sidebar instead of the main area.</summary>
    public bool Sidebar { get; init; }
}
