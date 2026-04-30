using Crabshell.Core.Documents;
using Crabshell.Core.Registry;

namespace Crabshell.Core.Repository;

public interface ICollectionRepository
{
    Task<List<CrabshellDocument>> GetAllAsync(CollectionMeta collection, int page = 1, int pageSize = 50);
    Task<CrabshellDocument?> GetByIdAsync(CollectionMeta collection, Guid id);
    Task<CrabshellDocument> CreateAsync(CrabshellDocument document);
    Task<CrabshellDocument> UpdateAsync(CrabshellDocument document);
    Task DeleteAsync(CrabshellDocument document);
}

public interface ICollectionRepository<T> : ICollectionRepository
    where T : CrabshellDocument
{
}