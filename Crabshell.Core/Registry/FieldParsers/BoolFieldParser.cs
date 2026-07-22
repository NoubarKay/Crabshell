using System.Reflection;
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;

namespace Crabshell.Core.Registry;

internal sealed class BoolFieldParser : IFieldParser
{
    public Type AttributeType => typeof(BoolFieldAttribute);

    public FieldMeta Parse(PropertyInfo p, CrabshellFieldAttribute attr, FieldGroupAttribute? groupAttr, GridOptionsAttribute? gridAttr)
    {
        var boolAttr = (BoolFieldAttribute)attr;
        return FieldMetaBuilder.Create(p, attr, groupAttr, gridAttr,
            FieldMetaBuilder.BuildAccessors(p),
            v => bool.TryParse(v, out var b) ? b : null,
            fieldType:    FieldType.Bool,
            boolSettings: new BoolFieldSettings { IsSwitch = boolAttr.IsSwitch });
    }
}
