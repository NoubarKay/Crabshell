namespace Crabshell.Data.Schema;

public interface ISchemaDiffService
{
    Task ApplyDiffAsync();
}