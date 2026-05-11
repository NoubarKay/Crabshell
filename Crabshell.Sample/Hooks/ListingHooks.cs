using Crabshell.Core.Hooks;
using Crabshell.Sample.Collections;
using Microsoft.Extensions.Logging;

namespace Crabshell.Sample.Hooks;

/// <summary>
/// Demonstrates a single class implementing both IBeforeSaveHook and IAfterSaveHook.
///
/// Before: rejects active/pending listings with no price.
/// After:  simulates a search-index refresh (logs the operation).
/// </summary>
public class ListingHooks(ILogger<ListingHooks> logger)
    : IBeforeSaveHook<Listing>, IAfterSaveHook<Listing>
{
    Task<HookResult> IBeforeSaveHook<Listing>.ExecuteAsync(Listing listing, SaveContext ctx, CancellationToken ct)
    {
        if (listing.Status is ListingStatus.Active or ListingStatus.Pending && listing.Price <= 0)
            return Task.FromResult(HookResult.Abort("Active and pending listings must have a price greater than zero."));

        return Task.FromResult(HookResult.Ok());
    }

    Task IAfterSaveHook<Listing>.ExecuteAsync(Listing listing, SaveContext ctx, CancellationToken ct)
    {
        logger.LogInformation(
            "[{Op}] Listing {Id} — queuing search re-index for \"{Address}\"",
            ctx.Operation, listing.Id, listing.Address);

        return Task.CompletedTask;
    }
}