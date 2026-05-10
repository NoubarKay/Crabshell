namespace Crabshell.Core.Repository;

/// <summary>Parameters for paginating, sorting, and filtering a collection list.</summary>
/// <param name="Skip">Number of documents to skip (zero-based offset). Default 0.</param>
/// <param name="Take">Maximum number of documents to return. Default 20.</param>
/// <param name="OrderBy">Property name to sort by, or <c>null</c> for default ordering.</param>
/// <param name="Descending">Sort in descending order when <c>true</c>. Default false.</param>
/// <param name="Filters">Optional list of field filters to apply.</param>
public record CollectionQuery(
    int Skip = 0,
    int Take = 20,
    string? OrderBy = null,
    bool Descending = false,
    IReadOnlyList<FieldFilter>? Filters = null
);

/// <summary>A single filter condition applied to a collection query.</summary>
/// <param name="Property">The property name to filter on.</param>
/// <param name="Operator">The comparison operator to apply.</param>
/// <param name="Value">The value to compare against.</param>
public record FieldFilter(string Property, FieldFilterOperator Operator, object? Value);

/// <summary>Comparison operators available for field filters.</summary>
public enum FieldFilterOperator
{
    /// <summary>Property value equals the filter value.</summary>
    Equals,
    /// <summary>Property value does not equal the filter value.</summary>
    NotEquals,
    /// <summary>Property value contains the filter value as a substring.</summary>
    Contains,
    /// <summary>Property value does not contain the filter value as a substring.</summary>
    DoesNotContain,
    /// <summary>Property value starts with the filter value.</summary>
    StartsWith,
    /// <summary>Property value ends with the filter value.</summary>
    EndsWith,
    /// <summary>Property value is greater than the filter value.</summary>
    GreaterThan,
    /// <summary>Property value is greater than or equal to the filter value.</summary>
    GreaterThanOrEquals,
    /// <summary>Property value is less than the filter value.</summary>
    LessThan,
    /// <summary>Property value is less than or equal to the filter value.</summary>
    LessThanOrEquals,
}