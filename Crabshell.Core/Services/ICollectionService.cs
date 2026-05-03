using Crabshell.Core.Documents;

namespace Crabshell.Core.Services;

public interface ICollectionService
{
    Task<IQueryable<CrabshellDocument>> GetAllAsync(string slug);
    Task<CrabshellDocument?> GetByIdAsync(string slug, Guid id);
    Task<(Guid Id, List<ValidationError> Errors)> CreateAsync(string slug, Dictionary<string, string?> formValues);
    Task<List<ValidationError>> UpdateAsync(string slug, Guid id, Dictionary<string, string?> formValues);
    Task DeleteAsync(string slug, Guid id);
}