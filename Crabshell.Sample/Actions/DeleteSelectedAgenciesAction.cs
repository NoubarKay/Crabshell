using Crabshell.Core.BulkActions;

namespace Crabshell.Sample.Actions;

/// <summary>
/// Example bulk action that deletes all selected agencies.
///
/// To create your own bulk action:
///   1. Implement <see cref="IBulkAction"/> with a public parameterless constructor.
///   2. Register it on your collection:
///      <code>
///      [Collection("agencies", CustomBulkOptions = new[] { typeof(DeleteSelectedAgenciesAction) })]
///      </code>
///
/// <see cref="ExecuteAsync"/> receives a <see cref="BulkActionContext"/> with the
/// selected IDs, slug, collection service, and full DI service provider.
/// </summary>
public class DeleteSelectedAgenciesAction : IBulkAction
{
    public string Label => "Delete Selected";
    public string Value => "delete_selected";

    public async Task ExecuteAsync(BulkActionContext ctx)
    {
        foreach (var id in ctx.SelectedIds)
            await ctx.CollectionService.DeleteAsync(ctx.Slug, id);
    }
}
