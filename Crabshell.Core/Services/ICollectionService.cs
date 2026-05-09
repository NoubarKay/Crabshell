using Crabshell.Core.Documents;
using Crabshell.Core.Repository;

namespace Crabshell.Core.Services;

public interface ICollectionService
{
    Task<Result<PagedResult>> GetPageAsync(string slug, CollectionQuery query);
    Task<Result<CrabshellDocument?>> GetByIdAsync(string slug, Guid id);
    Task<Result<CrabshellDocument>> NewAsync(string slug);
    Task<Result<(Guid Id, List<ValidationError> Errors)>> CreateAsync(string slug, Dictionary<string, string?> formValues);
    Task<Result<List<ValidationError>>> UpdateAsync(string slug, Guid id, Dictionary<string, string?> formValues);
    Task DeleteAsync(string slug, Guid id);
}