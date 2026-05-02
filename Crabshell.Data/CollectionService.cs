using Crabshell.Core.Documents;
using Crabshell.Core.Registry;
using Crabshell.Core.Services;

namespace Crabshell.Data;

public class CollectionService(CollectionRepositoryResolver resolver, CollectionRegistry registry) : ICollectionService
{
    public async Task<List<CrabshellDocument>> GetAllAsync(string slug, int page = 1, int pageSize = 50)
    {
        var collection = registry.Get(slug);
        if (collection is null) return await Task.FromResult(new List<CrabshellDocument>());
        return await resolver.Resolve(collection).GetAllAsync(collection, page, pageSize);
    }

    public async Task<CrabshellDocument?> GetByIdAsync(string slug, Guid id)
    {
        var collection = registry.Get(slug);
        if (collection is null) return await Task.FromResult<CrabshellDocument?>(null);
        return await resolver.Resolve(collection).GetByIdAsync(collection, id);
    }


    public async Task<(Guid Id, List<ValidationError> Errors)> CreateAsync(string slug, Dictionary<string, string?> formValues)
    {
        var collection = registry.Get(slug);
        if (collection is null) return (Guid.Empty, []);

        var errors = CollectionValidator.Validate(collection, formValues);
        if (errors.Count > 0) return (Guid.Empty, errors);

        var relationshipErrors = await ValidateRelationshipsAsync(collection, formValues);
        if (relationshipErrors.Count > 0) return (Guid.Empty, relationshipErrors);

        var document = (CrabshellDocument)Activator.CreateInstance(collection.ClrType)!;
        MapFormValues(collection, document, formValues);

        await resolver.Resolve(collection).CreateAsync(document);
        return (document.Id, []);
    }

    public async Task<List<ValidationError>> UpdateAsync(string slug, Guid id, Dictionary<string, string?> formValues)
    {
        var collection = registry.Get(slug);
        if (collection is null) return [];

        var errors = CollectionValidator.Validate(collection, formValues);
        if (errors.Count > 0) return errors;

        var relationshipErrors = await ValidateRelationshipsAsync(collection, formValues);
        if (relationshipErrors.Count > 0) return relationshipErrors;

        var document = await resolver.Resolve(collection).GetByIdAsync(collection, id);
        if (document is null) return [];

        MapFormValues(collection, document, formValues);

        await resolver.Resolve(collection).UpdateAsync(document);
        return [];
    }

    public async Task DeleteAsync(string slug, Guid id)
    {
        var collection = registry.Get(slug);
        if (collection is null) return;

        var document = await resolver.Resolve(collection).GetByIdAsync(collection, id);
        if (document is null) return;

        await resolver.Resolve(collection).DeleteAsync(document);
    }

    private async Task<List<ValidationError>> ValidateRelationshipsAsync(
        CollectionMeta collection,
        Dictionary<string, string?> formValues)
    {
        var errors = new List<ValidationError>();

        foreach (var field in collection.Fields.Where(f =>
            f.FieldType == Core.Attributes.FieldType.Relationship && f.RelationshipSettings is not null))
        {
            if (!formValues.TryGetValue(field.PropertyName, out var value)) continue;
            if (string.IsNullOrWhiteSpace(value)) continue;
            if (!Guid.TryParse(value, out var guid)) continue; // format already caught by CollectionValidator

            var relatedCollection = registry.Get(field.RelationshipSettings!.Slug);
            if (relatedCollection is null) continue;

            var exists = await resolver.Resolve(relatedCollection).GetByIdAsync(relatedCollection, guid);
            if (exists is null)
                errors.Add(new ValidationError(field.PropertyName, $"{field.Label} references a record that does not exist."));
        }

        return errors;
    }

    private static void MapFormValues(
        CollectionMeta collection,
        CrabshellDocument document,
        Dictionary<string, string?> formValues)
    {
        foreach (var field in collection.Fields)
        {
            if (!formValues.TryGetValue(field.PropertyName, out var value)) continue;

            field.Setter(document, field.FormValueParser(value));
        }
    }
}
