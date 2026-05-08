using Crabshell.Core.Documents;

namespace Crabshell.Core.Repository;

public record PagedResult(
    IReadOnlyList<CrabshellDocument> Items,
    int TotalCount,
    int Skip,
    int Take
);