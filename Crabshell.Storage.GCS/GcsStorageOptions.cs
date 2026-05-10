namespace Crabshell.Storage.GCS;

public class GcsStorageOptions
{
    public string BucketName { get; set; } = default!;
    public string? CredentialsJson { get; set; }
    public string? CdnUrl { get; set; }
}
