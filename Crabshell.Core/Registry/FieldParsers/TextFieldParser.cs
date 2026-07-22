using System.Reflection;
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;

namespace Crabshell.Core.Registry;

internal sealed class TextFieldParser : IFieldParser
{
    public Type AttributeType => typeof(TextFieldAttribute);

    public FieldMeta Parse(PropertyInfo p, CrabshellFieldAttribute attr, FieldGroupAttribute? groupAttr, GridOptionsAttribute? gridAttr)
    {
        var textAttr = (TextFieldAttribute)attr;
        return FieldMetaBuilder.Create(p, attr, groupAttr, gridAttr,
            FieldMetaBuilder.BuildAccessors(p),
            v => v,
            textSettings: new TextFieldSettings { MaxLength = textAttr.MaxLength, MinLength = textAttr.MinLength, Pattern = textAttr.Pattern });
    }
}