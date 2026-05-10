namespace Crabshell.Storage.S3;

public class S3StorageOptions
{
    public required string BucketName { get; set; }
    public required string Region { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? CloudFrontUrl { get; set; }
}
