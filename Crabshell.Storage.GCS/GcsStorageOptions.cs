namespace Crabshell.Storage.GCS;

public class GcsStorageOptions
{
    public required string BucketName { get; set; }
    public string? CredentialsJson { get; set; }
    public string? CdnUrl { get; set; }
}
