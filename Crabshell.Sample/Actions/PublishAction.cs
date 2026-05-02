using Crabshell.Core.SaveActions;

namespace Crabshell.Sample.Actions;

/// <summary>
/// Example custom save action that demonstrates how to add extra entries
/// to the Save split button on the collection edit page.
///
/// To create your own action:
///   1. Create a class that implements <see cref="ICustomSaveAction"/>.
///   2. Give it a public parameterless constructor.
///   3. Register it on your collection:
///      <code>
///      [Collection("articles", CustomSaveOptions = new[] { typeof(PublishAction) })]
///      </code>
///
/// <see cref="Value"/> must be unique and must not clash with the built-in
/// values: "stay", "clone", "next".
///
/// <see cref="ExecuteAsync"/> is called after the document has been saved
/// successfully. Return the URL to navigate to, or <c>null</c> to fall back
/// to the collection list page.
/// </summary>
public class PublishAction : ICustomSaveAction
{
    /// <summary>Text shown in the split button dropdown.</summary>
    public string Label => "Save and Publish";

    /// <summary>Unique key used internally to identify this action.</summary>
    public string Value => "publish";

    /// <summary>
    /// TODO: Add your publish logic here.
    /// Use <paramref name="ctx"/> to access the saved document ID, the
    /// collection slug, <see cref="SaveActionContext.CollectionService"/>,
    /// and <see cref="SaveActionContext.Services"/> for any other DI services.
    /// </summary>
    public async Task<string?> ExecuteAsync(SaveActionContext ctx)
    {
        // Example:
        // var publishSvc = ctx.Services.GetRequiredService<IPublishService>();
        // await publishSvc.PublishAsync(ctx.SavedId);

        return null; // return a URL string to navigate somewhere specific, or null for the list
    }
}
