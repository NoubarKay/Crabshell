using Crabshell.Core.Services;

namespace Crabshell.Core.BulkActions;

public record BulkActionContext(
    string Slug,
    IReadOnlyList<Guid> SelectedIds,
    ICollectionService CollectionService,
    IServiceProvider Services
);

public interface IBulkAction
{
    string Label { get; }
    string Value { get; }

    Task ExecuteAsync(BulkActionContext context);
}
