using System.Reflection;
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;

namespace Crabshell.Core.Registry;

internal interface IFieldParser
{
    Type AttributeType { get; }
    FieldMeta Parse(PropertyInfo property, CrabshellFieldAttribute attr, FieldGroupAttribute? groupAttr, GridOptionsAttribute? gridAttr);
}