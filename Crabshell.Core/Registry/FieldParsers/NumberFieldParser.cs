using System.Reflection;
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;

namespace Crabshell.Core.Registry;

internal sealed class NumberFieldParser : IFieldParser
{
    public Type AttributeType => typeof(NumberFieldAttribute);

    public FieldMeta Parse(PropertyInfo p, CrabshellFieldAttribute attr, FieldGroupAttribute? groupAttr, GridOptionsAttribute? gridAttr)
    {
        var numAttr    = (NumberFieldAttribute)attr;
        var underlying = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
        Func<string?, object?> parser =
            underlying == typeof(int)     ? v => int.TryParse(v, out var i) ? i : null
            : underlying == typeof(long)    ? v => long.TryParse(v, out var l) ? l : null
            : underlying == typeof(decimal) ? v => decimal.TryParse(v, out var d) ? d : null
            : v => double.TryParse(v, out var f) ? f : null;
        return FieldMetaBuilder.Create(p, attr, groupAttr, gridAttr,
            FieldMetaBuilder.BuildAccessors(p),
            parser,
            fieldType:      FieldType.Number,
            numberSettings: new NumberFieldSettings
            {
                Decimals = numAttr.Decimals,
                Min      = double.IsNaN(numAttr.Min) ? null : (decimal?)numAttr.Min,
                Max      = double.IsNaN(numAttr.Max) ? null : (decimal?)numAttr.Max,
                Step     = numAttr.Step,
                Prefix   = numAttr.Prefix,
                Suffix   = numAttr.Suffix,
                Format   = numAttr.Format,
            });
    }
}
