using Crabshell.Core.Services;

namespace Crabshell.Core;

/// <summary>Discriminated union returned by service operations. Match on the nested records to handle each case.</summary>
public abstract record Result<T>
{
    /// <summary>The operation succeeded. <see cref="Value"/> contains the result.</summary>
    public sealed record Ok(T Value) : Result<T>;

    /// <summary>No collection or document was found for the given slug/ID.</summary>
    public sealed record NotFound(string slug) : Result<T>;

    /// <summary>Validation failed. <see cref="Errors"/> lists each field-level problem.</summary>
    public sealed record Invalid(List<ValidationError> Errors) : Result<T>;
}