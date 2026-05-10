using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Crabshell.Core.Storage;
using Microsoft.Extensions.Options;

namespace Crabshell.Storage.S3;

public class S3StorageProvider : IStorageProvider
{
    private readonly IAmazonS3 _client;
    private readonly S3StorageOptions _options;

    public S3StorageProvider(IOptions<S3StorageOptions> options)
    {
        _options = options.Value;

        var region = RegionEndpoint.GetBySystemName(_options.Region);

        _client = _options.AccessKey is not null && _options.SecretKey is not null
            ? new AmazonS3Client(new BasicAWSCredentials(_options.AccessKey, _options.SecretKey), region)
            : new AmazonS3Client(region);
    }

    public async Task<StorageResult> UploadAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = path,
            InputStream = content,
            ContentType = contentType,
            AutoCloseStream = false
        };

        await _client.PutObjectAsync(request, cancellationToken);

        return new StorageResult(path, GetPublicUrl(path), content.Length);
    }

    public async Task DeleteAsync(string path, CancellationToken ct = default)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = path
        };

        await _client.DeleteObjectAsync(request, ct);
    }

    public async Task<bool> ExistsAsync(string path, CancellationToken ct = default)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _options.BucketName,
                Key = path
            };

            await _client.GetObjectMetadataAsync(request, ct);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public string GetPublicUrl(string path)
    {
        return _options.CloudFrontUrl is not null
            ? $"{_options.CloudFrontUrl}/{path}"
            : $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com/{path}";
    }
}
