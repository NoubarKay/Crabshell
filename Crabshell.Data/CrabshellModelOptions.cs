using Microsoft.EntityFrameworkCore;

namespace Crabshell.Data;

internal sealed class CrabshellModelOptions
{
    public Action<ModelBuilder>? Configure { get; init; }
}