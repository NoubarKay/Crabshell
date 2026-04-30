namespace Crabshell.Core.Documents;

/// <summary>
/// Base class for all Crabshell collection documents.
/// Every collection gets Id, CreatedAt, and UpdatedAt for free.
/// </summary>
public abstract class CrabshellDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
