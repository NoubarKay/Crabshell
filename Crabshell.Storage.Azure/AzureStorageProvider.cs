using Azure.Storage.Blobs;
using Crabshell.Core.Storage;
using Microsoft.Extensions.Options;

namespace Crabshell.Storage.Azure;

public class AzureStorageProvider(IOptions<AzureStorageOptions> options) : IStorageProvider
{
    private readonly BlobContainerClient _blobContainerClient = new(options.Value.ConnectionString, options.Value.Container);

    public async Task<StorageResult> UploadAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        await _blobContainerClient.UploadBlobAsync(path, content, cancellationToken);
        return new StorageResult(path, GetPublicUrl(path), content.Length);
    }

    public async Task DeleteAsync(string path, CancellationToken ct = default)
    {
        var blob = _blobContainerClient.GetBlobClient(path);
        if (await blob.ExistsAsync(ct))
        {
            await blob.DeleteAsync(cancellationToken: ct);
        }
    }

    public async Task<bool> ExistsAsync(string path, CancellationToken ct = default)
    {
        var blob = _blobContainerClient.GetBlobClient(path);
        return await blob.ExistsAsync(ct);
    }

    public string GetPublicUrl(string path)
    {
        return options.Value.CdnUrl is not null
            ? $"{options.Value.CdnUrl}/{path}"
            : _blobContainerClient.GetBlobClient(path).Uri.ToString();
    }
}