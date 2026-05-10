using Crabshell.Core.Documents;

namespace Crabshell.Core.Repository;

/// <summary>A page of documents returned by a collection query.</summary>
/// <param name="Items">The documents in this page.</param>
/// <param name="TotalCount">Total number of documents matching the query (before pagination).</param>
/// <param name="Skip">Number of documents skipped.</param>
/// <param name="Take">Maximum documents requested per page.</param>
public record PagedResult(
    IReadOnlyList<CrabshellDocument> Items,
    int TotalCount,
    int Skip,
    int Take
);