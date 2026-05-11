using Crabshell.Core.Documents;
using Crabshell.Core.Hooks;
using Crabshell.Core.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Data;

public sealed class HookDispatcher(IServiceProvider services)
{
    private static readonly Type BeforeHookDef = typeof(IBeforeSaveHook<>);
    private static readonly Type AfterHookDef  = typeof(IAfterSaveHook<>);

    public async Task<HookResult> RunBeforeAsync(
        CollectionMeta collection, CrabshellDocument document, SaveContext context, CancellationToken ct)
    {
        if (collection.HookTypes is null) return HookResult.Ok();

        foreach (var hookType in collection.HookTypes)
        {
            var iface = GetInterface(hookType, BeforeHookDef);
            if (iface is null) continue;

            var hook = services.GetRequiredService(hookType);
            var method = iface.GetMethod(nameof(IBeforeSaveHook<CrabshellDocument>.ExecuteAsync))!;
            var result = await (Task<HookResult>)method.Invoke(hook, [document, context, ct])!;
            if (result.IsAborted) return result;
        }

        return HookResult.Ok();
    }

    public async Task RunAfterAsync(
        CollectionMeta collection, CrabshellDocument document, SaveContext context, CancellationToken ct)
    {
        if (collection.HookTypes is null) return;

        foreach (var hookType in collection.HookTypes)
        {
            var iface = GetInterface(hookType, AfterHookDef);
            if (iface is null) continue;

            var hook = services.GetRequiredService(hookType);
            var method = iface.GetMethod(nameof(IAfterSaveHook<CrabshellDocument>.ExecuteAsync))!;
            await (Task)method.Invoke(hook, [document, context, ct])!;
        }
    }

    private static Type? GetInterface(Type hookType, Type genericDef) =>
        hookType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericDef);
}