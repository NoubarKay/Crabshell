namespace Crabshell.Core.Services;

/// <summary>A field-level validation failure returned by <see cref="ICollectionService"/> operations.</summary>
/// <param name="PropertyName">The property that failed validation.</param>
/// <param name="Message">Human-readable description of the failure.</param>
public record ValidationError(string PropertyName, string Message);
