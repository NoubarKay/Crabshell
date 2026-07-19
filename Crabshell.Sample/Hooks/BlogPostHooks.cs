using Crabshell.Core.Hooks;
using Crabshell.Sample.Collections;
using Microsoft.Extensions.Logging;

namespace Crabshell.Sample.Hooks;

/// <summary>
/// Before: blocks publishing a post with no content, and auto-sets PublishedAt when first published.
/// After:  logs the save operation (simulates a search re-index or CDN purge).
/// </summary>
public class BlogPostHooks(ILogger<BlogPostHooks> logger)
    : IBeforeSaveHook<BlogPost>, IAfterSaveHook<BlogPost>
{
    Task<HookResult> IBeforeSaveHook<BlogPost>.ExecuteAsync(BlogPost post, SaveContext ctx, CancellationToken ct)
    {
        if (post.Status == PostStatus.Published && string.IsNullOrWhiteSpace(post.Content))
            return Task.FromResult(HookResult.Abort("Published posts must have content."));

        if (post.Status == PostStatus.Published && post.PublishedAt is null)
            post.PublishedAt = DateTime.UtcNow;

        return Task.FromResult(HookResult.Ok());
    }

    Task IAfterSaveHook<BlogPost>.ExecuteAsync(BlogPost post, SaveContext ctx, CancellationToken ct)
    {
        logger.LogInformation(
            "[{Op}] BlogPost {Id} — queuing cache purge for \"{Title}\" ({Status})",
            ctx.Operation, post.Id, post.Title, post.Status);

        return Task.CompletedTask;
    }
}
