namespace Crabshell.Core.Media;

public interface IMediaProvider
{
    Task<StorageResult> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
    Task DeleteAsync(string fileName, CancellationToken ct = default);
    string GetPublicUrl(string fileKey);
}