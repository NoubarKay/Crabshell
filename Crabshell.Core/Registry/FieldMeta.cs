using Crabshell.Core.Attributes;

namespace Crabshell.Core.Registry;

public sealed class FieldMeta
{
    public string PropertyName { get; init; } = default!;
    public string ColumnName { get; init; } = default!;
    public string Label { get; init; } = default!;
    public bool Required { get; init; }
    public Type ClrType { get; init; } = typeof(string);
    public FieldType FieldType { get; init; } = FieldType.Text;
    public Func<object, object?> Getter { get; init; } = default!;
    public Action<object, object?> Setter { get; init; } = default!;

    // Field-type-specific settings — null when not applicable
    public TextFieldSettings? TextSettings { get; init; }
    public SelectFieldSettings? SelectSettings { get; init; }
    public RelationshipFieldSettings? RelationshipSettings { get; init; }
    public BoolFieldSettings? BoolSettings { get; init; }
    public DateTimeFieldSettings? DateTimeSettings { get; init; }
    public NumberFieldSettings? NumberSettings { get; init; }

    // Layout
    public FieldGroupSettings? GroupSettings { get; init; }

    // Grid
    public bool GridVisible { get; init; } = true;
    public bool GridSortable { get; init; } = true;
    public bool GridFilterable { get; init; } = false;
    public int GridWidth { get; init; } = 0;
    public int GridOrder { get; init; } = 0;
    
    // Database
    public string ColumnType { get; set; }
    public Func<string?, object?> FormValueParser { get; init; } = v => v;
}
