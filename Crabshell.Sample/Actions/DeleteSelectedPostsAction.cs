using Crabshell.Core.BulkActions;

namespace Crabshell.Sample.Actions;

/// <summary>
/// Bulk action that deletes all selected blog posts.
///
/// To create your own bulk action:
///   1. Implement <see cref="IBulkAction"/> with a public parameterless constructor.
///   2. Register it on your collection:
///      <code>
///      [Collection("blog_posts", CustomBulkOptions = new[] { typeof(DeleteSelectedPostsAction) })]
///      </code>
/// </summary>
public class DeleteSelectedPostsAction : IBulkAction
{
    public string Label => "Delete Selected";
    public string Value => "delete_selected";

    public async Task ExecuteAsync(BulkActionContext ctx)
    {
        foreach (var id in ctx.SelectedIds)
            await ctx.CollectionService.DeleteAsync(ctx.Slug, id);
    }
}
