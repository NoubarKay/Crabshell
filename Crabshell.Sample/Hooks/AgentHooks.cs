using Crabshell.Core.Hooks;
using Crabshell.Sample.Collections;
using Microsoft.Extensions.Logging;

namespace Crabshell.Sample.Hooks;

/// <summary>
/// Before: blocks activating an agent who has no license number.
/// After:  writes an audit log entry — demonstrates fire-and-forget post-commit work.
/// </summary>
public class AgentHooks(ILogger<AgentHooks> logger)
    : IBeforeSaveHook<Agent>, IAfterSaveHook<Agent>
{
    Task<HookResult> IBeforeSaveHook<Agent>.ExecuteAsync(Agent agent, SaveContext ctx, CancellationToken ct)
    {
        if (agent.IsActive && string.IsNullOrWhiteSpace(agent.LicenseNumber))
            return Task.FromResult(HookResult.Abort("Active agents must have a license number."));

        return Task.FromResult(HookResult.Ok());
    }

    Task IAfterSaveHook<Agent>.ExecuteAsync(Agent agent, SaveContext ctx, CancellationToken ct)
    {
        logger.LogInformation(
            "[{Op}] Agent {Id} ({Name}) saved — active: {Active}",
            ctx.Operation, agent.Id, agent.Name, agent.IsActive);

        return Task.CompletedTask;
    }
}