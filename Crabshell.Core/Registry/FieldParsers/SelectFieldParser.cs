using System.Reflection;
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;

namespace Crabshell.Core.Registry;

internal sealed class SelectFieldParser : IFieldParser
{
    public Type AttributeType => typeof(SelectFieldAttribute);

    public FieldMeta Parse(PropertyInfo p, CrabshellFieldAttribute attr, FieldGroupAttribute? groupAttr, GridOptionsAttribute? gridAttr)
    {
        var selectAttr = (SelectFieldAttribute)attr;
        var enumType   = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
        var options    = enumType.IsEnum ? Enum.GetNames(enumType) : selectAttr.Options ?? [];
        return FieldMetaBuilder.Create(p, attr, groupAttr, gridAttr,
            FieldMetaBuilder.BuildAccessors(p),
            enumType.IsEnum
                ? v => Enum.TryParse(enumType, v, ignoreCase: true, out var e) ? e : null
                : v => v,
            fieldType:      FieldType.Select,
            selectSettings: new SelectFieldSettings { Options = options });
    }
}