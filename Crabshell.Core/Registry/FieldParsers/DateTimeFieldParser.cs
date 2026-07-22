using System.Reflection;
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;

namespace Crabshell.Core.Registry;

internal sealed class DateTimeFieldParser : IFieldParser
{
    public Type AttributeType => typeof(DateTimeFieldAttribute);

    public FieldMeta Parse(PropertyInfo p, CrabshellFieldAttribute attr, FieldGroupAttribute? groupAttr, GridOptionsAttribute? gridAttr)
    {
        var dtAttr = (DateTimeFieldAttribute)attr;
        return FieldMetaBuilder.Create(p, attr, groupAttr, gridAttr,
            FieldMetaBuilder.BuildAccessors(p),
            v => DateTimeOffset.TryParse(v, out var dto)
                ? p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?)
                    ? (object?)dto.UtcDateTime
                    : dto
                : null,
            fieldType:        FieldType.DateTime,
            dateTimeSettings: new DateTimeFieldSettings
            {
                HasTime       = dtAttr.HasTime,
                TimeOnly      = dtAttr.TimeOnly,
                Utc           = dtAttr.Utc,
                Min           = dtAttr.Min,
                Max           = dtAttr.Max,
                ShowNowButton = dtAttr.ShowNowButton,
                HoursStep     = dtAttr.HoursStep,
                MinutesStep   = dtAttr.MinutesStep,
                SecondsStep   = dtAttr.SecondsStep,
                Immediate     = dtAttr.Immediate,
                Inline        = dtAttr.Inline,
                ShowButton    = dtAttr.ShowButton,
                YearRange     = dtAttr.YearRange,
                DateFormat    = dtAttr.DateFormat
            });
    }
}
