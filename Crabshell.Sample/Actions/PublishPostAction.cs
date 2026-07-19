using Crabshell.Core.SaveActions;

namespace Crabshell.Sample.Actions;

/// <summary>
/// Custom save action that publishes a blog post immediately after saving.
///
/// To create your own action:
///   1. Create a class that implements <see cref="ICustomSaveAction"/>.
///   2. Give it a public parameterless constructor.
///   3. Register it on your collection:
///      <code>
///      [Collection("blog_posts", CustomSaveOptions = new[] { typeof(PublishPostAction) })]
///      </code>
///
/// <see cref="Value"/> must be unique and must not clash with the built-in
/// values: "stay", "clone", "next".
///
/// <see cref="ExecuteAsync"/> is called after the document has been saved
/// successfully. Return a URL to navigate to, or <c>null</c> to fall back
/// to the collection list page.
/// </summary>
public class PublishPostAction : ICustomSaveAction
{
    public string Label => "Save and Publish";
    public string Value => "publish";

    public async Task<string?> ExecuteAsync(SaveActionContext ctx)
    {
        // TODO: update the post status to Published via CollectionService
        // Example:
        // var svc = ctx.CollectionService;
        // await svc.UpdateAsync("blog_posts", ctx.SavedId, new Dictionary<string, string?>
        // {
        //     [nameof(BlogPost.Status)]      = PostStatus.Published.ToString(),
        //     [nameof(BlogPost.PublishedAt)] = DateTime.UtcNow.ToString("O"),
        // });

        return null;
    }
}
