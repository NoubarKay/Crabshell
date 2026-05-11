using Crabshell.Core.Documents;

namespace Crabshell.Core.Hooks;

/// <summary>
/// Runs after the database transaction commits. Failures are not rolled back — log instead of throwing.
/// </summary>
public interface IAfterSaveHook<TDocument> where TDocument : CrabshellDocument
{
    Task ExecuteAsync(TDocument document, SaveContext context, CancellationToken ct);
}