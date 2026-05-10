namespace Crabshell.Core.Storage;

public interface IStorageProvider
{
    Task<StorageResult> UploadAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string path, CancellationToken ct = default);
    Task DeleteAsync(string path, CancellationToken ct = default);
    Task<bool> ExistsAsync(string path, CancellationToken ct = default);
    string GetPublicUrl(string path);
}

public record StorageResult(string Path, string PublicUrl, long SizeBytes);