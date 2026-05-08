using Crabshell.Core.Documents;
using Crabshell.Core.Repository;

namespace Crabshell.Core.Services;

public interface ICollectionService
{
    Task<PagedResult> GetPageAsync(string slug, CollectionQuery query);
    Task<CrabshellDocument?> GetByIdAsync(string slug, Guid id);
    Task<(Guid Id, List<ValidationError> Errors)> CreateAsync(string slug, Dictionary<string, string?> formValues);
    Task<List<ValidationError>> UpdateAsync(string slug, Guid id, Dictionary<string, string?> formValues);
    Task DeleteAsync(string slug, Guid id);
}