namespace Crabshell.Core.Attributes.Fields;

/// <summary>
/// Maps to a text column. Renders as a rich text (HTML) editor in the admin UI.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class RichTextFieldAttribute : CrabshellFieldAttribute
{
}