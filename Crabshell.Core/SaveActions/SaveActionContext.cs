using Crabshell.Core.Services;

namespace Crabshell.Core.SaveActions;

public record SaveActionContext(
    string Slug,
    Guid SaveId,
    ICollectionService CollectionService,
    IServiceProvider Services
);

public interface ICustomSaveAction
{
    string Label { get; }
    string Value { get; }
    
    Task<string?> ExecuteAsync(SaveActionContext context);
}