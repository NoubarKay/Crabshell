using Crabshell.Core.Documents;
using Crabshell.Core.Registry;

namespace Crabshell.Core.Repository;

/// <summary>Low-level data-access interface for a Crabshell collection.</summary>
public interface ICollectionRepository
{
    /// <summary>Returns a paged, filtered, and sorted result set for the given collection.</summary>
    Task<PagedResult> GetPageAsync(CollectionMeta collection, CollectionQuery query);

    /// <summary>Returns the document with the given ID, or <c>null</c> if not found.</summary>
    Task<CrabshellDocument?> GetByIdAsync(CollectionMeta collection, Guid id);

    /// <summary>Persists a new document and returns it with any database-assigned values applied.</summary>
    Task<CrabshellDocument> CreateAsync(CrabshellDocument document);

    /// <summary>Persists changes to an existing document and returns the updated instance.</summary>
    Task<CrabshellDocument> UpdateAsync(CrabshellDocument document);

    /// <summary>Soft-deletes the given document.</summary>
    Task DeleteAsync(CrabshellDocument document);

    /// <summary>Synchronises many-to-many join rows for the document to match its current relation lists.</summary>
    Task SyncManyToManyAsync(CollectionMeta collection, CrabshellDocument document);
}

/// <summary>Strongly-typed variant of <see cref="ICollectionRepository"/> for use with DI.</summary>
public interface ICollectionRepository<T> : ICollectionRepository
    where T : CrabshellDocument
{
}