using Crabshell.Core.Documents;
using Crabshell.Core.Repository;

namespace Crabshell.Core.Services;

/// <summary>High-level service for CRUD operations on Crabshell collections and singletons.</summary>
public interface ICollectionService
{
    /// <summary>Returns a paged, filtered, and sorted list of documents for the given collection.</summary>
    Task<Result<PagedResult>> GetPageAsync(string slug, CollectionQuery query);

    /// <summary>Returns a single document by ID, or <c>null</c> if not found.</summary>
    Task<Result<CrabshellDocument?>> GetByIdAsync(string slug, Guid id);

    /// <summary>Creates a new empty document instance (not persisted) for the given collection.</summary>
    Task<Result<CrabshellDocument>> NewAsync(string slug);

    /// <summary>
    /// Returns the sole document for a singleton collection (<c>[Single]</c>).
    /// Auto-creates the document on first call if it does not exist.
    /// </summary>
    Task<Result<CrabshellDocument>> GetSingleAsync(string slug);

    /// <summary>
    /// Validates and persists a new document from the given form values.
    /// Returns the new document's ID on success, or a list of validation errors.
    /// </summary>
    Task<Result<(Guid Id, List<ValidationError> Errors)>> CreateAsync(string slug, Dictionary<string, string?> formValues);

    /// <summary>
    /// Validates and applies form values to an existing document.
    /// Returns an empty list on success, or a list of validation errors.
    /// </summary>
    Task<Result<List<ValidationError>>> UpdateAsync(string slug, Guid id, Dictionary<string, string?> formValues);

    /// <summary>Soft-deletes the document with the given ID.</summary>
    Task DeleteAsync(string slug, Guid id);
}