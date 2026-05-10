namespace Crabshell.Storage.Azure;

public class AzureStorageOptions
{
    public required string ConnectionString { get; set; }
    public required string Container { get; set; }
    public string? CdnUrl { get; set; }
}