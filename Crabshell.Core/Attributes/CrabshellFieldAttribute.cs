namespace Crabshell.Core.Attributes;

/// <summary>
/// Base for all Crabshell field attributes. Carries shared options
/// applicable to every field type.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public abstract class CrabshellFieldAttribute : Attribute
{
    /// <summary>Override the column name in Postgres. Defaults to the property name (snake_cased).</summary>
    public string? ColumnName { get; set; }
 
    /// <summary>Whether this field is required (non-nullable at the DB level).</summary>
    public bool Required { get; set; }
 
    /// <summary>Label shown in the admin UI. Defaults to the property name.</summary>
    public string? Label { get; set; }
}
