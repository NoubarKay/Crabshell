using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;

namespace Crabshell.Core.Registry;

internal static class FieldMetaBuilder
{
    internal static FieldMeta Create(
        PropertyInfo p,
        CrabshellFieldAttribute attr,
        FieldGroupAttribute? groupAttr,
        GridOptionsAttribute? gridAttr,
        (Func<object, object?> getter, Action<object, object?> setter) accessors,
        Func<string?, object?> parser,
        FieldType fieldType = FieldType.Text,
        TextFieldSettings? textSettings = null,
        SelectFieldSettings? selectSettings = null,
        RelationshipFieldSettings? relationshipSettings = null,
        BoolFieldSettings? boolSettings = null,
        DateTimeFieldSettings? dateTimeSettings = null,
        NumberFieldSettings? numberSettings = null,
        MediaFieldSettings? mediaSettings = null) => new()
    {
        PropertyName         = p.Name,
        ColumnName           = ToSnakeCase(p.Name),
        Label                = attr.Label ?? p.Name,
        Required             = attr.Required,
        DefaultValue         = attr.DefaultValue is null ? null : parser(attr.DefaultValue),
        ClrType              = p.PropertyType,
        FieldType            = fieldType,
        TextSettings         = textSettings,
        SelectSettings       = selectSettings,
        RelationshipSettings = relationshipSettings,
        BoolSettings         = boolSettings,
        DateTimeSettings     = dateTimeSettings,
        NumberSettings       = numberSettings,
        MediaSettings        = mediaSettings,
        GroupSettings        = groupAttr is null ? null : new FieldGroupSettings { Name = groupAttr.Name, Sidebar = groupAttr.Sidebar },
        Getter               = accessors.getter,
        Setter               = accessors.setter,
        GridVisible          = gridAttr?.Visible ?? true,
        GridSortable         = gridAttr?.Sortable ?? true,
        GridFilterable       = gridAttr?.Filterable ?? false,
        GridWidth            = gridAttr?.Width ?? 0,
        GridOrder            = gridAttr?.Order ?? 0,
        FormValueParser      = parser,
    };

    internal static (Func<object, object?> getter, Action<object, object?> setter) BuildAccessors(PropertyInfo property)
    {
        var getParam    = Expression.Parameter(typeof(object), "obj");
        var getCast     = Expression.Convert(getParam, property.DeclaringType!);
        var getProp     = Expression.Property(getCast, property);
        var getConvert  = Expression.Convert(getProp, typeof(object));
        var getter      = Expression.Lambda<Func<object, object?>>(getConvert, getParam).Compile();

        var setParam    = Expression.Parameter(typeof(object), "obj");
        var setVal      = Expression.Parameter(typeof(object), "val");
        var setCast     = Expression.Convert(setParam, property.DeclaringType!);
        var setProp     = Expression.Property(setCast, property);
        var setConvert  = Expression.Convert(setVal, property.PropertyType);
        var assign      = Expression.Assign(setProp, setConvert);
        var setter      = Expression.Lambda<Action<object, object?>>(assign, setParam, setVal).Compile();

        return (getter, setter);
    }

    private static string ToSnakeCase(string name) =>
        Regex.Replace(name, @"([a-z0-9])([A-Z])|([A-Z]+)([A-Z][a-z])", "$1$3_$2$4").ToLowerInvariant();
}