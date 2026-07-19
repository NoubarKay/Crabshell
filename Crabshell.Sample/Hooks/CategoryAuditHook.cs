using Crabshell.Core.Hooks;
using Crabshell.Sample.Collections;
using Microsoft.Extensions.Logging;

namespace Crabshell.Sample.Hooks;

/// <summary>
/// Demonstrates a post-save-only hook. Writes an immutable audit log entry
/// for every create, update, and delete on categories.
/// </summary>
public class CategoryAuditHook(ILogger<CategoryAuditHook> logger) : IAfterSaveHook<Category>
{
    public Task ExecuteAsync(Category category, SaveContext ctx, CancellationToken ct)
    {
        logger.LogInformation(
            "[Audit] Category {Id} ({Name}) — {Op} at {At:u}",
            category.Id, category.Name, ctx.Operation, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
