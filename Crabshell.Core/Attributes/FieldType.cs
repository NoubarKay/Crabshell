namespace Crabshell.Core.Attributes;

/// <summary>Identifies the kind of field, determining the SQL column type and admin UI widget used.</summary>
public enum FieldType
{
    /// <summary>varchar or text column. Rendered as a text input.</summary>
    Text,
    /// <summary>Integer (enum) or varchar(255) (string options) column. Rendered as a dropdown.</summary>
    Select,
    /// <summary>UUID foreign key column pointing to another collection. Rendered as a searchable dropdown.</summary>
    Relationship,
    /// <summary>Boolean column. Rendered as a checkbox or toggle switch.</summary>
    Bool,
    /// <summary>date, timestamptz, or timetz column. Rendered as a date/time picker.</summary>
    DateTime,
    /// <summary>integer, bigint, numeric, or double precision column. Rendered as a number input.</summary>
    Number,
    /// <summary>text column. Rendered as a rich text (HTML) editor.</summary>
    RichText,
    /// <summary>varchar column storing a relative file path. Rendered as a file upload widget.</summary>
    Media
}