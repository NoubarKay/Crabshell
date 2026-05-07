using Crabshell.Core.Media;

namespace Crabshell.Media.Local;

public sealed class LocalMediaProvider : IMediaProvider
{
    private readonly string _root;
    private readonly string _baseUrl;

    public LocalMediaProvider(string root, string baseUrl)
    {
        _root = root;
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task<StorageResult> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var key = $"{Guid.NewGuid()}/{fileName}";
        var fullPath = Path.Combine(_root, key);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var file = File.Create(fullPath);
        await stream.CopyToAsync(file, ct);

        return new StorageResult(key, GetPublicUrl(key), file.Length, contentType);
    }

    public Task DeleteAsync(string fileKey, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_root, fileKey);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public string GetPublicUrl(string fileKey) => $"{_baseUrl}/{fileKey}";
}