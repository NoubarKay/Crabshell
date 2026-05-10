namespace Crabshell.Storage.Azure;

public class AzureStorageOptions
{
    public string ConnectionString { get; set; }
    public string Container { get; set; }
    public string? CdnUrl { get; set; }
}