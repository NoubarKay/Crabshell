using System.Reflection;
using System.Text.RegularExpressions;
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.BulkActions;
using Crabshell.Core.Documents;
using Crabshell.Core.Hooks;
using Crabshell.Core.SaveActions;

namespace Crabshell.Core.Registry;

public sealed class CollectionRegistry
{
    private readonly Dictionary<string, CollectionMeta> _collections = new();

    private static readonly HashSet<string> _baseProperties = ["Id", "CreatedAt", "UpdatedAt"];
    private static readonly Regex _safeSlug = new(@"^[a-z][a-z0-9_]*$", RegexOptions.Compiled);
    private static readonly IReadOnlyList<IFieldParser> _fieldParsers =
    [
        new TextFieldParser(),
        new SelectFieldParser(),
        new RelationshipFieldParser(),
        new BoolFieldParser(),
        new DateTimeFieldParser(),
        new NumberFieldParser(),
        new RichTextFieldParser(),
        new MediaFieldParser(),
    ];

    public void Register(Assembly assembly)
    {
        var collectionTypes = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(CrabshellDocument))
                        && t.GetCustomAttribute<CollectionAttribute>() != null);

        foreach (var type in collectionTypes)
        {
            var attribute = type.GetCustomAttribute<CollectionAttribute>()!;
            var slug = attribute.Slug ?? type.Name;

            if (!_safeSlug.IsMatch(slug))
                throw new InvalidOperationException(
                    $"Collection slug '{slug}' on '{type.Name}' is invalid. Slugs must match ^[a-z][a-z0-9_]*$.");

            var customSaveActions = BuildCustomSaveActions(attribute.CustomSaveOptions ?? [], type.Name);
            var customBulkActions = BuildBulkActions(attribute.CustomBulkOptions ?? [], type.Name);
            var hookTypes = ValidateHookTypes(attribute.Hooks ?? [], type.Name, type);

            _collections[slug] = new CollectionMeta
            {
                Slug              = slug,
                Label             = attribute.Label ?? type.Name,
                ClrType           = type,
                Fields            = ParseFields(type).AsReadOnly(),
                SaveOption        = attribute.SaveOptions,
                CustomSaveActions = customSaveActions.AsReadOnly(),
                CustomBulkOptions = customBulkActions.AsReadOnly(),
                HookTypes         = hookTypes.Count > 0 ? hookTypes.AsReadOnly() : null,
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

            if (!_safeSlug.IsMatch(slug))
                throw new InvalidOperationException(
                    $"Single slug '{slug}' on '{type.Name}' is invalid. Slugs must match ^[a-z][a-z0-9_]*$.");

            _collections[slug] = new CollectionMeta
            {
                Slug              = slug,
                Label             = attribute.Label ?? type.Name,
                ClrType           = type,
                Fields            = ParseFields(type).AsReadOnly(),
                SaveOption        = SaveOption.Save,
                CustomSaveActions = null,
                CustomBulkOptions = null,
                IsSingle          = true,
            };
        }
    }

    private static List<FieldMeta> ParseFields(Type type)
    {
        List<FieldMeta> fieldMetas = [];

        foreach (var p in type.GetProperties().Where(p => !_baseProperties.Contains(p.Name)))
        {
            var matches = _fieldParsers
                .Select(parser => (parser, attr: (CrabshellFieldAttribute?)p.GetCustomAttribute(parser.AttributeType)))
                .Where(x => x.attr is not null)
                .ToList();

            if (matches.Count > 1)
                throw new InvalidOperationException(
                    $"Property '{type.Name}.{p.Name}' has multiple field attributes. Only one is allowed.");

            if (matches.Count == 0)
                continue;

            var (parser, attr) = matches[0];
            var groupAttr = p.GetCustomAttribute<FieldGroupAttribute>();
            var gridAttr  = p.GetCustomAttribute<GridOptionsAttribute>();
            fieldMetas.Add(parser.Parse(p, attr!, groupAttr, gridAttr));
        }

        return fieldMetas;
    }

    public CollectionMeta? Get(string slug) =>
        _collections.TryGetValue(slug, out var meta) ? meta : null;

    public IReadOnlyList<CollectionMeta> GetAll() =>
        _collections.Values.Where(c => !c.IsSingle).ToList().AsReadOnly();

    public IReadOnlyList<CollectionMeta> GetAllSingles() =>
        _collections.Values.Where(c => c.IsSingle).ToList().AsReadOnly();

    public IReadOnlyList<Type> GetAllHookTypes() =>
        _collections.Values
            .SelectMany(m => m.HookTypes ?? [])
            .Distinct()
            .ToList()
            .AsReadOnly();

    private static List<Type> ValidateHookTypes(Type[] types, string collectionTypeName, Type documentType)
    {
        var beforeIface = typeof(IBeforeSaveHook<>).MakeGenericType(documentType);
        var afterIface  = typeof(IAfterSaveHook<>).MakeGenericType(documentType);

        foreach (var hookType in types)
        {
            if (!beforeIface.IsAssignableFrom(hookType) && !afterIface.IsAssignableFrom(hookType))
                throw new InvalidOperationException(
                    $"'{hookType.Name}' on '{collectionTypeName}' must implement " +
                    $"IBeforeSaveHook<{documentType.Name}> or IAfterSaveHook<{documentType.Name}>.");
        }

        return [..types];
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

            actions.Add((IBulkAction)Activator.CreateInstance(actionType)!);
        }

        return actions;
    }
}