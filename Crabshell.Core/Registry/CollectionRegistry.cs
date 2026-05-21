using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.BulkActions;
using Crabshell.Core.Documents;
using Crabshell.Core.SaveActions;

namespace Crabshell.Core.Registry;

public sealed class CollectionRegistry
{
    private readonly Dictionary<string, CollectionMeta> _collections = new();

    private static readonly HashSet<string> _baseProperties = ["Id", "CreatedAt", "UpdatedAt"];
    private static readonly Regex _safeSlug = new(@"^[a-z][a-z0-9_]*$", RegexOptions.Compiled);

    
    public void Register(Assembly assembly)
    {
        var collectionTypes = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(CrabshellDocument))
                        && t.GetCustomAttribute<CollectionAttribute>() != null);

        foreach (var type in collectionTypes)
        {
            var attribute = type.GetCustomAttribute<CollectionAttribute>();

            if (attribute is null) continue;

            var slug = attribute.Slug ?? type.Name;
            var label = attribute?.Label ?? type.Name;
            var saveOptions = attribute.SaveOptions;

            //Check for custom save actions
            var customSaveActions = BuildCustomSaveActions(attribute?.CustomSaveOptions ?? [], type.Name);
            var customBulkActions = BuildBulkActions(attribute?.CustomBulkOptions ?? [], type.Name);


            if (!_safeSlug.IsMatch(slug))
                throw new InvalidOperationException(
                    $"Collection slug '{slug}' on '{type.Name}' is invalid. Slugs must match ^[a-z][a-z0-9_]*$.");

            List<FieldMeta> fieldMetas = [];

            foreach (var p in type.GetProperties().Where(p => !_baseProperties.Contains(p.Name)))
            {
                
                var fieldAttrCount = new[]
                {
                    p.GetCustomAttribute<TextFieldAttribute>() is not null,
                    p.GetCustomAttribute<SelectFieldAttribute>() is not null,
                    p.GetCustomAttribute<RelationshipFieldAttribute>() is not null,
                    p.GetCustomAttribute<BoolFieldAttribute>() is not null,
                    p.GetCustomAttribute<DateTimeFieldAttribute>() is not null,
                    p.GetCustomAttribute<NumberFieldAttribute>() is not null,
                    p.GetCustomAttribute<RichTextFieldAttribute>() is not null,
                    p.GetCustomAttribute<MediaFieldAttribute>() is not null,
                    p.GetCustomAttribute<ManyToManyFieldAttribute>() is not null,
                }.Count(x => x);

                if (fieldAttrCount > 1)
                    throw new InvalidOperationException(
                        $"Property '{type.Name}.{p.Name}' has multiple field attributes. Only one is allowed.");

                
                var groupAttr = p.GetCustomAttribute<FieldGroupAttribute>();
                var gridAttr  = p.GetCustomAttribute<GridOptionsAttribute>();
                var accessors = BuildAccessors(p);

                var textAttr = p.GetCustomAttribute<TextFieldAttribute>();
                if (textAttr is not null)
                    fieldMetas.Add(CreateFieldMeta(p,
                        textAttr,
                        groupAttr,
                        gridAttr,
                        accessors,
                        v=> v,
                        textSettings: new TextFieldSettings { MaxLength = textAttr.MaxLength, MinLength = textAttr.MinLength, Pattern = textAttr.Pattern }));

                var selectAttr = p.GetCustomAttribute<SelectFieldAttribute>();
                if (selectAttr is not null)
                {
                    var enumType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    var options = enumType.IsEnum
                        ? Enum.GetNames(enumType)
                        : selectAttr.Options ?? [];
                    
                    fieldMetas.Add(CreateFieldMeta(p,
                        selectAttr,
                        groupAttr,
                        gridAttr,
                        accessors,
                        enumType.IsEnum
                            ? v => Enum.TryParse(enumType, v, ignoreCase: true, out var e) ? e : null
                            : v => v,
                        fieldType: FieldType.Select,
                        selectSettings: new SelectFieldSettings { Options = options }));
                }


                var relAttr = p.GetCustomAttribute<RelationshipFieldAttribute>();
                if (relAttr is not null)
                {
                    var relatedSlug = relAttr.RelatesTo.GetCustomAttribute<CollectionAttribute>()?.Slug;
                    fieldMetas.Add(CreateFieldMeta(p,
                        relAttr,
                        groupAttr,
                        gridAttr,
                        accessors,
                        v => Guid.TryParse(v, out var g) ? g : null,
                        fieldType: FieldType.Relationship,
                        relationshipSettings: relatedSlug is null ? null : new RelationshipFieldSettings { Slug = relatedSlug }));
                }

                var boolAttr = p.GetCustomAttribute<BoolFieldAttribute>();
                if (boolAttr is not null)
                    fieldMetas.Add(CreateFieldMeta(p,
                        boolAttr,
                        groupAttr,
                        gridAttr,
                        accessors,
                        v => bool.TryParse(v, out var b) ? b : null,
                        fieldType: FieldType.Bool,
                        boolSettings: new BoolFieldSettings { IsSwitch = boolAttr.IsSwitch }));

                var dateTimeAttr = p.GetCustomAttribute<DateTimeFieldAttribute>();
                if (dateTimeAttr is not null)
                    fieldMetas.Add(CreateFieldMeta(p, dateTimeAttr, groupAttr, gridAttr, accessors,
                        v => DateTimeOffset.TryParse(v, out var dto)
                            ? p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?)
                                ? (object?)dto.UtcDateTime
                                : dto
                            : null,
                        fieldType: FieldType.DateTime,
                        dateTimeSettings: new DateTimeFieldSettings
                        {
                            HasTime      = dateTimeAttr.HasTime,
                            TimeOnly     = dateTimeAttr.TimeOnly,
                            Utc          = dateTimeAttr.Utc,
                            Min          = dateTimeAttr.Min,
                            Max          = dateTimeAttr.Max,
                            ShowNowButton = dateTimeAttr.ShowNowButton,
                            HoursStep    = dateTimeAttr.HoursStep,
                            MinutesStep  = dateTimeAttr.MinutesStep,
                            SecondsStep  = dateTimeAttr.SecondsStep,
                            Immediate    = dateTimeAttr.Immediate,
                            Inline       = dateTimeAttr.Inline,
                            ShowButton   = dateTimeAttr.ShowButton,
                            YearRange    = dateTimeAttr.YearRange,
                            DateFormat   = dateTimeAttr.DateFormat
                        }));

                var numberAttr = p.GetCustomAttribute<NumberFieldAttribute>();
                if (numberAttr is not null)
                {
                    var underlying = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

                    Func<string?, object?> parser = underlying == typeof(int)     ? v => int.TryParse(v, out var i) ? i : null
                                                  : underlying == typeof(long)    ? v => long.TryParse(v, out var l) ? l : null
                                                  : underlying == typeof(decimal) ? v => decimal.TryParse(v, out var d) ? d : null
                                                  : v => double.TryParse(v, out var f) ? f : null;

                    fieldMetas.Add(CreateFieldMeta(p,
                        numberAttr,
                        groupAttr,
                        gridAttr,
                        accessors,
                        parser,
                        fieldType: FieldType.Number,
                        numberSettings: new NumberFieldSettings
                        {
                            Decimals = numberAttr.Decimals,
                            Min      = double.IsNaN(numberAttr.Min) ? null : (decimal?)numberAttr.Min,
                            Max      = double.IsNaN(numberAttr.Max) ? null : (decimal?)numberAttr.Max,
                            Step     = numberAttr.Step,
                            Prefix   = numberAttr.Prefix,
                            Suffix   = numberAttr.Suffix,
                            Format   = numberAttr.Format,
                        }));
                }

                var richTextAttr = p.GetCustomAttribute<RichTextFieldAttribute>();
                if (richTextAttr is not null)
                    fieldMetas.Add(CreateFieldMeta(p,
                        richTextAttr,
                        groupAttr,
                        gridAttr,
                        accessors,
                        v => v,
                        fieldType: FieldType.RichText));

                var mediaAttr = p.GetCustomAttribute<MediaFieldAttribute>();
                if (mediaAttr is not null)
                    fieldMetas.Add(CreateFieldMeta(p,
                        mediaAttr,
                        groupAttr,
                        gridAttr,
                        accessors,
                        v => v,
                        fieldType: FieldType.Media,
                        mediaSettings: new MediaFieldSettings { Accept = mediaAttr.Accept, MaxSizeMb = mediaAttr.MaxSizeMb }));

                var m2mAttr = p.GetCustomAttribute<ManyToManyFieldAttribute>();
                if (m2mAttr is not null)
                    fieldMetas.Add(CreateFieldMeta(p,
                        m2mAttr,
                        groupAttr,
                        gridAttr,
                        accessors,
                        ParseGuidCsv,
                        fieldType: FieldType.ManyToMany,
                        manyToManySettings: BuildManyToManySettings(m2mAttr, slug)));
            }
            _collections[slug] = new CollectionMeta
            {
                Slug              = slug,
                Label             = label,
                ClrType           = type,
                Fields            = fieldMetas.AsReadOnly(),
                SaveOption        = saveOptions,
                CustomSaveActions = customSaveActions.AsReadOnly(),
                CustomBulkOptions = customBulkActions.AsReadOnly(),
                IsSingle          = false,
            };
        }

        var singleTypes = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(CrabshellDocument))
                        && t.GetCustomAttribute<SingleAttribute>() != null);

        foreach (var type in singleTypes)
        {
            var attribute = type.GetCustomAttribute<SingleAttribute>()!;
            var slug      = attribute.Slug;
            var label     = attribute.Label ?? type.Name;

            if (!_safeSlug.IsMatch(slug))
                throw new InvalidOperationException(
                    $"Single slug '{slug}' on '{type.Name}' is invalid. Slugs must match ^[a-z][a-z0-9_]*$.");

            List<FieldMeta> fieldMetas = [];

            foreach (var p in type.GetProperties().Where(p => !_baseProperties.Contains(p.Name)))
            {
                var fieldAttrCount = new[]
                {
                    p.GetCustomAttribute<TextFieldAttribute>() is not null,
                    p.GetCustomAttribute<SelectFieldAttribute>() is not null,
                    p.GetCustomAttribute<RelationshipFieldAttribute>() is not null,
                    p.GetCustomAttribute<BoolFieldAttribute>() is not null,
                    p.GetCustomAttribute<DateTimeFieldAttribute>() is not null,
                    p.GetCustomAttribute<NumberFieldAttribute>() is not null,
                    p.GetCustomAttribute<RichTextFieldAttribute>() is not null,
                    p.GetCustomAttribute<MediaFieldAttribute>() is not null,
                    p.GetCustomAttribute<ManyToManyFieldAttribute>() is not null,
                }.Count(x => x);

                if (fieldAttrCount > 1)
                    throw new InvalidOperationException(
                        $"Property '{type.Name}.{p.Name}' has multiple field attributes. Only one is allowed.");

                var groupAttr = p.GetCustomAttribute<FieldGroupAttribute>();
                var gridAttr  = p.GetCustomAttribute<GridOptionsAttribute>();
                var accessors = BuildAccessors(p);

                var textAttr = p.GetCustomAttribute<TextFieldAttribute>();
                if (textAttr is not null)
                    fieldMetas.Add(CreateFieldMeta(p, textAttr, groupAttr, gridAttr, accessors,
                        v => v,
                        textSettings: new TextFieldSettings { MaxLength = textAttr.MaxLength, MinLength = textAttr.MinLength, Pattern = textAttr.Pattern }));

                var selectAttr = p.GetCustomAttribute<SelectFieldAttribute>();
                if (selectAttr is not null)
                {
                    var enumType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    var options  = enumType.IsEnum ? Enum.GetNames(enumType) : selectAttr.Options ?? [];
                    fieldMetas.Add(CreateFieldMeta(p, selectAttr, groupAttr, gridAttr, accessors,
                        enumType.IsEnum
                            ? v => Enum.TryParse(enumType, v, ignoreCase: true, out var e) ? e : null
                            : v => v,
                        fieldType:       FieldType.Select,
                        selectSettings:  new SelectFieldSettings { Options = options }));
                }

                var relAttr = p.GetCustomAttribute<RelationshipFieldAttribute>();
                if (relAttr is not null)
                {
                    var relatedSlug = relAttr.RelatesTo.GetCustomAttribute<CollectionAttribute>()?.Slug;
                    fieldMetas.Add(CreateFieldMeta(p, relAttr, groupAttr, gridAttr, accessors,
                        v => Guid.TryParse(v, out var g) ? g : null,
                        fieldType:             FieldType.Relationship,
                        relationshipSettings:  relatedSlug is null ? null : new RelationshipFieldSettings { Slug = relatedSlug }));
                }

                var boolAttr = p.GetCustomAttribute<BoolFieldAttribute>();
                if (boolAttr is not null)
                    fieldMetas.Add(CreateFieldMeta(p, boolAttr, groupAttr, gridAttr, accessors,
                        v => bool.TryParse(v, out var b) ? b : null,
                        fieldType:    FieldType.Bool,
                        boolSettings: new BoolFieldSettings { IsSwitch = boolAttr.IsSwitch }));

                var dateTimeAttr = p.GetCustomAttribute<DateTimeFieldAttribute>();
                if (dateTimeAttr is not null)
                    fieldMetas.Add(CreateFieldMeta(p, dateTimeAttr, groupAttr, gridAttr, accessors,
                        v => DateTimeOffset.TryParse(v, out var dto)
                            ? p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?)
                                ? (object?)dto.UtcDateTime
                                : dto
                            : null,
                        fieldType:        FieldType.DateTime,
                        dateTimeSettings: new DateTimeFieldSettings
                        {
                            HasTime       = dateTimeAttr.HasTime,
                            TimeOnly      = dateTimeAttr.TimeOnly,
                            Utc           = dateTimeAttr.Utc,
                            Min           = dateTimeAttr.Min,
                            Max           = dateTimeAttr.Max,
                            ShowNowButton = dateTimeAttr.ShowNowButton,
                            HoursStep     = dateTimeAttr.HoursStep,
                            MinutesStep   = dateTimeAttr.MinutesStep,
                            SecondsStep   = dateTimeAttr.SecondsStep,
                            Immediate     = dateTimeAttr.Immediate,
                            Inline        = dateTimeAttr.Inline,
                            ShowButton    = dateTimeAttr.ShowButton,
                            YearRange     = dateTimeAttr.YearRange,
                            DateFormat    = dateTimeAttr.DateFormat
                        }));

                var numberAttr = p.GetCustomAttribute<NumberFieldAttribute>();
                if (numberAttr is not null)
                {
                    var underlying = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    Func<string?, object?> parser = underlying == typeof(int)     ? v => int.TryParse(v, out var i) ? i : null
                                                  : underlying == typeof(long)    ? v => long.TryParse(v, out var l) ? l : null
                                                  : underlying == typeof(decimal) ? v => decimal.TryParse(v, out var d) ? d : null
                                                  : v => double.TryParse(v, out var f) ? f : null;
                    fieldMetas.Add(CreateFieldMeta(p, numberAttr, groupAttr, gridAttr, accessors,
                        parser,
                        fieldType:      FieldType.Number,
                        numberSettings: new NumberFieldSettings
                        {
                            Decimals = numberAttr.Decimals,
                            Min      = double.IsNaN(numberAttr.Min) ? null : (decimal?)numberAttr.Min,
                            Max      = double.IsNaN(numberAttr.Max) ? null : (decimal?)numberAttr.Max,
                            Step     = numberAttr.Step,
                            Prefix   = numberAttr.Prefix,
                            Suffix   = numberAttr.Suffix,
                            Format   = numberAttr.Format,
                        }));
                }

                var richTextAttr = p.GetCustomAttribute<RichTextFieldAttribute>();
                if (richTextAttr is not null)
                    fieldMetas.Add(CreateFieldMeta(p, richTextAttr, groupAttr, gridAttr, accessors,
                        v => v,
                        fieldType: FieldType.RichText));

                var mediaAttr = p.GetCustomAttribute<MediaFieldAttribute>();
                if (mediaAttr is not null)
                    fieldMetas.Add(CreateFieldMeta(p, mediaAttr, groupAttr, gridAttr, accessors,
                        v => v,
                        fieldType: FieldType.Media,
                        mediaSettings: new MediaFieldSettings { Accept = mediaAttr.Accept, MaxSizeMb = mediaAttr.MaxSizeMb }));

                var m2mAttr = p.GetCustomAttribute<ManyToManyFieldAttribute>();
                if (m2mAttr is not null)
                    fieldMetas.Add(CreateFieldMeta(p, m2mAttr, groupAttr, gridAttr, accessors,
                        ParseGuidCsv,
                        fieldType: FieldType.ManyToMany,
                        manyToManySettings: BuildManyToManySettings(m2mAttr, slug)));
            }

            _collections[slug] = new CollectionMeta
            {
                Slug              = slug,
                Label             = label,
                ClrType           = type,
                Fields            = fieldMetas.AsReadOnly(),
                SaveOption        = SaveOption.Save,
                CustomSaveActions = null,
                CustomBulkOptions = null,
                IsSingle          = true,
            };
        }
    }

    public CollectionMeta? Get(string slug)
    {
        return _collections.TryGetValue(slug, out var meta) ? meta : null;
    }

    public IReadOnlyList<CollectionMeta> GetAll() =>
        _collections.Values.Where(c => !c.IsSingle).ToList().AsReadOnly();

    public IReadOnlyList<CollectionMeta> GetAllSingles() =>
        _collections.Values.Where(c => c.IsSingle).ToList().AsReadOnly();
    
    private static FieldMeta CreateFieldMeta(
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
        MediaFieldSettings? mediaSettings = null,
        ManyToManyFieldSettings? manyToManySettings = null) => new()
    {
        PropertyName = p.Name,
        ColumnName = ToSnakeCase(p.Name),
        Label = attr.Label ?? p.Name,
        Required = attr.Required,
        DefaultValue = attr.DefaultValue is null ? null : parser(attr.DefaultValue),
        ClrType = p.PropertyType,
        FieldType = fieldType,
        TextSettings = textSettings,
        SelectSettings = selectSettings,
        RelationshipSettings = relationshipSettings,
        BoolSettings = boolSettings,
        DateTimeSettings = dateTimeSettings,
        NumberSettings = numberSettings,
        MediaSettings = mediaSettings,
        ManyToManySettings = manyToManySettings,
        GroupSettings = groupAttr is null ? null : new FieldGroupSettings { Name = groupAttr.Name, Sidebar = groupAttr.Sidebar },
        Getter = accessors.getter,
        Setter = accessors.setter,
        GridVisible = gridAttr?.Visible ?? true,
        GridSortable = gridAttr?.Sortable ?? true,
        GridFilterable = gridAttr?.Filterable ?? false,
        GridWidth = gridAttr?.Width ?? 0,
        GridOrder = gridAttr?.Order ?? 0,
        FormValueParser = parser,
    };

    private static ManyToManyFieldSettings BuildManyToManySettings(ManyToManyFieldAttribute attr, string sourceSlug)
    {
        var targetSlug = attr.RelatesTo.GetCustomAttribute<CollectionAttribute>()?.Slug
                         ?? attr.RelatesTo.GetCustomAttribute<SingleAttribute>()?.Slug
                         ?? attr.RelatesTo.Name;

        var joinTable = attr.JoinTableName ?? $"{sourceSlug}_{targetSlug}";

        // Distinguish columns for self-referential relationships where the slugs match.
        var sourceColumn = $"{sourceSlug}_id";
        var targetColumn = $"{targetSlug}_id";
        if (sourceColumn == targetColumn)
        {
            sourceColumn = "source_id";
            targetColumn = "target_id";
        }

        return new ManyToManyFieldSettings
        {
            TargetSlug    = targetSlug,
            JoinTableName = joinTable,
            SourceColumn  = sourceColumn,
            TargetColumn  = targetColumn,
        };
    }

    private static List<Guid> ParseGuidCsv(string? value) =>
        (value ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => Guid.TryParse(s, out var g) ? (Guid?)g : null)
            .Where(g => g.HasValue)
            .Select(g => g!.Value)
            .ToList();

    private static string ToSnakeCase(string name) =>
        Regex.Replace(name, @"([a-z0-9])([A-Z])|([A-Z]+)([A-Z][a-z])", "$1$3_$2$4").ToLowerInvariant()
            .ToLowerInvariant();
    
    private static (Func<object, object?> getter, Action<object, object?> setter) BuildAccessors(
        PropertyInfo property)
    {
        // getter
        var getParam = Expression.Parameter(typeof(object), "obj");
        var getCast = Expression.Convert(getParam, property.DeclaringType!);
        var getProp = Expression.Property(getCast, property);
        var getConvert = Expression.Convert(getProp, typeof(object));
        var getter = Expression.Lambda<Func<object, object?>>(getConvert, getParam).Compile();

        // setter
        var setParam = Expression.Parameter(typeof(object), "obj");
        var setVal = Expression.Parameter(typeof(object), "val");
        var setCast = Expression.Convert(setParam, property.DeclaringType!);
        var setProp = Expression.Property(setCast, property);
        var setConvert = Expression.Convert(setVal, property.PropertyType);
        var assign = Expression.Assign(setProp, setConvert);
        var setter = Expression.Lambda<Action<object, object?>>(assign, setParam, setVal).Compile();

        return (getter, setter);
    }
    
    private static readonly HashSet<string> _reservedSaveValues = ["stay", "clone", "next"];

    private static List<ICustomSaveAction> BuildCustomSaveActions(Type[] types, string collectionTypeName)
    {
        var actions = new List<ICustomSaveAction>();

        foreach (var actionType in types)
        {
            if (!typeof(ICustomSaveAction).IsAssignableFrom(actionType))
                throw new InvalidOperationException(
                    $"'{actionType.Name}' on '{collectionTypeName}' must implement ICustomSaveAction.");

            if (actionType.GetConstructor(Type.EmptyTypes) is null)
                throw new InvalidOperationException(
                    $"'{actionType.Name}' on '{collectionTypeName}' must have a public parameterless constructor.");

            var instance = (ICustomSaveAction)Activator.CreateInstance(actionType)!;

            if (_reservedSaveValues.Contains(instance.Value))
                throw new InvalidOperationException(
                    $"'{actionType.Name}' uses reserved Value '{instance.Value}'. Reserved: stay, clone, next.");

            actions.Add(instance);
        }

        return actions;
    }
    
    private static List<IBulkAction> BuildBulkActions(Type[] types, string collectionTypeName)
    {
        var actions = new List<IBulkAction>();

        foreach (var actionType in types)
        {
            if (!typeof(IBulkAction).IsAssignableFrom(actionType))
                throw new InvalidOperationException(
                    $"'{actionType.Name}' on '{collectionTypeName}' must implement IBulkAction.");

            if (actionType.GetConstructor(Type.EmptyTypes) is null)
                throw new InvalidOperationException(
                    $"'{actionType.Name}' on '{collectionTypeName}' must have a public parameterless constructor.");

            var instance = (IBulkAction)Activator.CreateInstance(actionType)!;

            actions.Add(instance);
        }

        return actions;
    }

}