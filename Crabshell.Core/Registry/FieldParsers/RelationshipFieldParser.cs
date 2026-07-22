using System.Reflection;
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;

namespace Crabshell.Core.Registry;

internal sealed class RelationshipFieldParser : IFieldParser
{
    public Type AttributeType => typeof(RelationshipFieldAttribute);

    public FieldMeta Parse(PropertyInfo p, CrabshellFieldAttribute attr, FieldGroupAttribute? groupAttr, GridOptionsAttribute? gridAttr)
    {
        var relAttr     = (RelationshipFieldAttribute)attr;
        var relatedSlug = relAttr.RelatesTo.GetCustomAttribute<CollectionAttribute>()?.Slug;
        return FieldMetaBuilder.Create(p, attr, groupAttr, gridAttr,
            FieldMetaBuilder.BuildAccessors(p),
            v => Guid.TryParse(v, out var g) ? g : null,
            fieldType:            FieldType.Relationship,
            relationshipSettings: relatedSlug is null ? null : new RelationshipFieldSettings { Slug = relatedSlug });
    }
}
