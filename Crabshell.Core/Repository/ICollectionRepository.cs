using Crabshell.Core.Documents;
using Crabshell.Core.Registry;

namespace Crabshell.Core.Repository;

public interface ICollectionRepository
{
    Task<IQueryable<CrabshellDocument>> GetAllAsync(CollectionMeta collection);
    Task<CrabshellDocument?> GetByIdAsync(CollectionMeta collection, Guid id);
    Task<CrabshellDocument> CreateAsync(CrabshellDocument document);
    Task<CrabshellDocument> UpdateAsync(CrabshellDocument document);
    Task DeleteAsync(CrabshellDocument document);
}

public interface ICollectionRepository<T> : ICollectionRepository
    where T : CrabshellDocument
{
}