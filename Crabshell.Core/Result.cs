using Crabshell.Core.Services;

namespace Crabshell.Core;

public abstract record Result<T>
{
    public sealed record Ok(T Value) : Result<T>;
    public sealed record NotFound(string slug) : Result<T>;
    public sealed record Invalid(List<ValidationError> Errors) : Result<T>;
}