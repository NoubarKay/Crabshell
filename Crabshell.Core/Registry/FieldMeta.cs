using Crabshell.Core.Attributes;

namespace Crabshell.Core.Registry;

/// <summary>Runtime descriptor for a single field on a collection, built from attribute metadata at startup.</summary>
public sealed class FieldMeta
{
    /// <summary>CLR property name on the document class.</summary>
    public string PropertyName { get; init; } = default!;

    /// <summary>Database column name (snake_case of the property name).</summary>
    public string ColumnName { get; init; } = default!;

    /// <summary>Human-readable label shown in the admin UI.</summary>
    public string Label { get; init; } = default!;

    /// <summary>Whether a non-empty value is required. Validated on save.</summary>
    public bool Required { get; init; }

    /// <summary>Default value used when creating a new document.</summary>
    public object? DefaultValue { get; init; }

    /// <summary>CLR type of the property.</summary>
    public Type ClrType { get; init; } = typeof(string);

    /// <summary>The kind of field, used to select the correct admin widget and SQL column type.</summary>
    public FieldType FieldType { get; init; } = FieldType.Text;

    /// <summary>Compiled getter for reading the property value from a document instance.</summary>
    public Func<object, object?> Getter { get; init; } = default!;

    /// <summary>Compiled setter for writing a value to the property on a document instance.</summary>
    public Action<object, object?> Setter { get; init; } = default!;

    // Field-type-specific settings — null when not applicable
    /// <summary>Text field settings. Non-null only when <see cref="FieldType"/> is <see cref="FieldType.Text"/>.</summary>
    public TextFieldSettings? TextSettings { get; init; }

    /// <summary>Select field settings. Non-null only when <see cref="FieldType"/> is <see cref="FieldType.Select"/>.</summary>
    public SelectFieldSettings? SelectSettings { get; init; }

    /// <summary>Relationship field settings. Non-null only when <see cref="FieldType"/> is <see cref="FieldType.Relationship"/>.</summary>
    public RelationshipFieldSettings? RelationshipSettings { get; init; }

    /// <summary>Bool field settings. Non-null only when <see cref="FieldType"/> is <see cref="FieldType.Bool"/>.</summary>
    public BoolFieldSettings? BoolSettings { get; init; }

    /// <summary>DateTime field settings. Non-null only when <see cref="FieldType"/> is <see cref="FieldType.DateTime"/>.</summary>
    public DateTimeFieldSettings? DateTimeSettings { get; init; }

    /// <summary>Number field settings. Non-null only when <see cref="FieldType"/> is <see cref="FieldType.Number"/>.</summary>
    public NumberFieldSettings? NumberSettings { get; init; }

    /// <summary>Media field settings. Non-null only when <see cref="FieldType"/> is <see cref="FieldType.Media"/>.</summary>
    public MediaFieldSettings? MediaSettings { get; init; }

    // Layout
    /// <summary>Group settings controlling which panel this field appears in. <c>null</c> = ungrouped main area.</summary>
    public FieldGroupSettings? GroupSettings { get; init; }

    // Grid
    /// <summary>Whether this field appears as a column in the collection list view. Default true.</summary>
    public bool GridVisible { get; init; } = true;

    /// <summary>Whether the list view column can be sorted. Default true.</summary>
    public bool GridSortable { get; init; } = true;

    /// <summary>Whether the list view column exposes a filter UI. Default false.</summary>
    public bool GridFilterable { get; init; } = false;

    /// <summary>Column width in pixels. 0 = auto.</summary>
    public int GridWidth { get; init; } = 0;

    /// <summary>Column position in the list view. Lower = further left.</summary>
    public int GridOrder { get; init; } = 0;

    // Database
    /// <summary>Converts a raw form string value to the typed value expected by the property setter.</summary>
    public Func<string?, object?> FormValueParser { get; init; } = v => v;
}
