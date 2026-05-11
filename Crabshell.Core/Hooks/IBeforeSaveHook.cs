using Crabshell.Core.Documents;

namespace Crabshell.Core.Hooks;

/// <summary>
/// Runs before the document is written to the database.
/// Return <see cref="HookResult.Abort"/> to cancel the save and surface a validation error.
/// </summary>
public interface IBeforeSaveHook<TDocument> where TDocument : CrabshellDocument
{
    Task<HookResult> ExecuteAsync(TDocument document, SaveContext context, CancellationToken ct);
}