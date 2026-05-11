using Crabshell.Core.Hooks;
using Crabshell.Sample.Collections;
using Microsoft.Extensions.Logging;

namespace Crabshell.Sample.Hooks;

/// <summary>
/// Demonstrates a post-save-only hook. Writes an immutable audit log entry
/// for every create, update, and delete on agencies.
/// </summary>
public class AgencyAuditHook(ILogger<AgencyAuditHook> logger) : IAfterSaveHook<Agency>
{
    public Task ExecuteAsync(Agency agency, SaveContext ctx, CancellationToken ct)
    {
        logger.LogInformation(
            "[Audit] Agency {Id} ({Name}) — {Op} at {At:u}",
            agency.Id, agency.Name, ctx.Operation, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}