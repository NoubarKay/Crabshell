namespace Crabshell.Core.Documents;

/// <summary>
/// Base class for all Crabshell collection documents.
/// Every collection gets Id, CreatedAt, and UpdatedAt for free.
/// </summary>
public abstract class CrabshellDocument
{
    /// <summary>Unique identifier for this document. Defaults to a new <see cref="Guid"/>.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>UTC timestamp when this document was first created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp when this document was last updated.</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Soft-delete flag. When <c>true</c> the document is excluded from normal queries.</summary>
    public bool IsDeleted { get; set; }

    /// <summary>UTC timestamp when this document was soft-deleted, or <c>null</c> if not deleted.</summary>
    public DateTime? DeletedAt { get; set; }
}
