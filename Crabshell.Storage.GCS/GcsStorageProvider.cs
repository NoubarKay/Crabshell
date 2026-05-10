using Crabshell.Core.Storage;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace Crabshell.Storage.GCS;

public class GcsStorageProvider : IStorageProvider
{
    private readonly StorageClient _client;
    private readonly GcsStorageOptions _options;

    public GcsStorageProvider(IOptions<GcsStorageOptions> options)
    {
        _options = options.Value;

        _client = _options.CredentialsJson is not null
            ? StorageClient.Create(GoogleCredential.FromJson(_options.CredentialsJson))
            : StorageClient.Create();
    }

    public async Task<StorageResult> UploadAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        await _client.UploadObjectAsync(_options.BucketName, path, contentType, content, cancellationToken: cancellationToken);

        return new StorageResult(path, GetPublicUrl(path), content.Length);
    }

    public async Task DeleteAsync(string path, CancellationToken ct = default)
    {
        await _client.DeleteObjectAsync(_options.BucketName, path, cancellationToken: ct);
    }

    public async Task<bool> ExistsAsync(string path, CancellationToken ct = default)
    {
        try
        {
            await _client.GetObjectAsync(_options.BucketName, path, cancellationToken: ct);
            return true;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public string GetPublicUrl(string path)
    {
        return _options.CdnUrl is not null
            ? $"{_options.CdnUrl}/{path}"
            : $"https://storage.googleapis.com/{_options.BucketName}/{path}";
    }
}
