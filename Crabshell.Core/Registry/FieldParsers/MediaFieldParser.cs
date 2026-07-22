using System.Reflection;
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;

namespace Crabshell.Core.Registry;

internal sealed class MediaFieldParser : IFieldParser
{
    public Type AttributeType => typeof(MediaFieldAttribute);

    public FieldMeta Parse(PropertyInfo p, CrabshellFieldAttribute attr, FieldGroupAttribute? groupAttr, GridOptionsAttribute? gridAttr)
    {
        var mediaAttr = (MediaFieldAttribute)attr;
        return FieldMetaBuilder.Create(p, attr, groupAttr, gridAttr,
            FieldMetaBuilder.BuildAccessors(p),
            v => v,
            fieldType:     FieldType.Media,
            mediaSettings: new MediaFieldSettings { Accept = mediaAttr.Accept, MaxSizeMb = mediaAttr.MaxSizeMb });
    }
}
