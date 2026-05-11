namespace Crabshell.Core.Hooks;

/// <summary>Returned by <see cref="IBeforeSaveHook{TDocument}.ExecuteAsync"/> to allow or cancel the save.</summary>
public sealed record HookResult
{
    public string? AbortMessage { get; init; }
    public bool IsAborted => AbortMessage is not null;

    public static HookResult Ok() => new();
    public static HookResult Abort(string message) => new() { AbortMessage = message };
}