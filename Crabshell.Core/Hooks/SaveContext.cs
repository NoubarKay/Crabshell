namespace Crabshell.Core.Hooks;

/// <summary>Context passed to every save hook.</summary>
/// <param name="Operation">Whether this is a create, update, or delete.</param>
/// <param name="Services">Scoped <see cref="IServiceProvider"/> for the current request.</param>
public record SaveContext(SaveOperation Operation, IServiceProvider Services);

public enum SaveOperation { Create, Update, Delete }