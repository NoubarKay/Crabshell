using Microsoft.EntityFrameworkCore;

namespace Crabshell.Data;

public sealed class CrabshellModelOptions
{
    public Action<ModelBuilder>? Configure { get; init; }
}