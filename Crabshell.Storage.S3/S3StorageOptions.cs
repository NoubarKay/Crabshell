namespace Crabshell.Storage.S3;

public class S3StorageOptions
{
    public string BucketName { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? CloudFrontUrl { get; set; }
}
