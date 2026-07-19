using Crabshell.Core.Hooks;
using Crabshell.Sample.Collections;
using Microsoft.Extensions.Logging;

namespace Crabshell.Sample.Hooks;

/// <summary>
/// Before: blocks activating an author who has no email address.
/// After:  writes an audit log entry.
/// </summary>
public class AuthorHooks(ILogger<AuthorHooks> logger)
    : IBeforeSaveHook<Author>, IAfterSaveHook<Author>
{
    Task<HookResult> IBeforeSaveHook<Author>.ExecuteAsync(Author author, SaveContext ctx, CancellationToken ct)
    {
        if (author.IsActive && string.IsNullOrWhiteSpace(author.Email))
            return Task.FromResult(HookResult.Abort("Active authors must have an email address."));

        return Task.FromResult(HookResult.Ok());
    }

    Task IAfterSaveHook<Author>.ExecuteAsync(Author author, SaveContext ctx, CancellationToken ct)
    {
        logger.LogInformation(
            "[{Op}] Author {Id} ({Name}) saved — active: {Active}",
            ctx.Operation, author.Id, author.Name, author.IsActive);

        return Task.CompletedTask;
    }
}
