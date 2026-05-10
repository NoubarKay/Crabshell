using Crabshell.Core;
using Crabshell.Core.Documents;
using Crabshell.Core.Registry;
using Crabshell.Core.Repository;
using Crabshell.Core.Services;

namespace Crabshell.Data;

public class CollectionService(CollectionRepositoryResolver resolver, CollectionRegistry registry, IDocumentFactory factory) : ICollectionService
{
    public async Task<Result<PagedResult>> GetPageAsync(string slug, CollectionQuery query)
    {
        var collection = registry.Get(slug);

        if (collection is null)
            return new Result<PagedResult>.NotFound(slug);

        var pagedResult = await resolver
            .Resolve(collection)
            .GetPageAsync(collection, query);
        
        return new Result<PagedResult>.Ok(pagedResult);
    }

    public async Task<Result<CrabshellDocument?>> GetByIdAsync(string slug, Guid id)
    {
        var collection = registry.Get(slug);

        if (collection is null)
            return new Result<CrabshellDocument?>.NotFound(slug);

        var doc = await resolver
            .Resolve(collection)
            .GetByIdAsync(collection, id);
        
        return new Result<CrabshellDocument?>.Ok(doc);
    }

    public async Task<Result<CrabshellDocument>> GetSingleAsync(string slug)
    {
        var collection = registry.Get(slug);
        if (collection is null) return new Result<CrabshellDocument>.NotFound(slug);

        var paged = await resolver.Resolve(collection).GetPageAsync(collection, new CollectionQuery(Skip: 0, Take: 1));

        if (paged.Items.Count > 0)
            return new Result<CrabshellDocument>.Ok(paged.Items[0]);

        // Auto-create the singleton document on first access
        var document = factory.Create(collection);
        await resolver.Resolve(collection).CreateAsync(document);
        return new Result<CrabshellDocument>.Ok(document);
    }

    public Task<Result<CrabshellDocument>> NewAsync(string slug)
    {
        var collection = registry.Get(slug);
        if (collection is null) return Task.FromResult<Result<CrabshellDocument>>(new Result<CrabshellDocument>.NotFound(slug));
        return Task.FromResult<Result<CrabshellDocument>>(new Result<CrabshellDocument>.Ok(factory.Create(collection!)));
    }

    public async Task<Result<(Guid Id, List<ValidationError> Errors)>> CreateAsync(string slug, Dictionary<string, string?> formValues)
    {
        var collection = registry.Get(slug);
        if (collection is null) return new Result<(Guid Id, List<ValidationError> Errors)>.NotFound(slug);

        var errors = CollectionValidator.Validate(collection, formValues);
        if (errors.Count > 0) return new Result<(Guid Id, List<ValidationError> Errors)>.Invalid(errors);

        var relationshipErrors = await ValidateRelationshipsAsync(collection, formValues);
        if (relationshipErrors.Count > 0) return new Result<(Guid Id, List<ValidationError> Errors)>.Ok((Guid.Empty, relationshipErrors));

        var document = factory.Create(collection);
        MapFormValues(collection, document, formValues);

        await resolver.Resolve(collection).CreateAsync(document);
        return new Result<(Guid Id, List<ValidationError> Errors)>.Ok((document.Id, []));
    }

    public async Task<Result<List<ValidationError>>> UpdateAsync(string slug, Guid id, Dictionary<string, string?> formValues)
    {
        var collection = registry.Get(slug);
        if (collection is null) return new Result<List<ValidationError>>.NotFound(slug);
            
        var errors = CollectionValidator.Validate(collection, formValues);
        if (errors.Count > 0) return new Result<List<ValidationError>>.Invalid(errors);

        var relationshipErrors = await ValidateRelationshipsAsync(collection, formValues);
        if (relationshipErrors.Count > 0) return new Result<List<ValidationError>>.Invalid(errors);

        var document = await resolver.Resolve(collection).GetByIdAsync(collection, id);
        if (document is null) return new Result<List<ValidationError>>.Ok([]);

        MapFormValues(collection, document, formValues);

        await resolver.Resolve(collection).UpdateAsync(document);
        return new Result<List<ValidationError>>.Ok([]);
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
