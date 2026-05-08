namespace Crabshell.Core.Repository;

public record CollectionQuery(
    int Skip = 0,
    int Take = 20,
    string? OrderBy = null,
    bool Descending = false,
    IReadOnlyList<FieldFilter>? Filters = null
);

public record FieldFilter(string Property, FieldFilterOperator Operator, object? Value);

public enum FieldFilterOperator
{
    Equals,
    NotEquals,
    Contains,
    DoesNotContain,
    StartsWith,
    EndsWith,
    GreaterThan,
    GreaterThanOrEquals,
    LessThan,
    LessThanOrEquals,
}