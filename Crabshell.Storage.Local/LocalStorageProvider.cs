using Crabshell.Core.Storage;
using Microsoft.Extensions.Options;

namespace Crabshell.Storage.Local;

public class LocalStorageProvider(IOptions<LocalStorageOptions> options) : IStorageProvider
{
    public async Task<StorageResult> UploadAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        
        var extension = Path.GetExtension(path);
        var fileName = Path.GetFileNameWithoutExtension(path);
        var rootPath = Path.GetFullPath(options.Value.RootPath);

        var uniqueName = $"{fileName}_{Guid.NewGuid():N}{extension}";
        var relativePath = Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, uniqueName);

        var uploadPath = Path.GetFullPath(Path.Combine(rootPath, relativePath));

        if (!uploadPath.StartsWith(rootPath))
        {
            throw new InvalidOperationException("Invalid path.");
        }

        var directory = Path.GetDirectoryName(uploadPath)!;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var fileStream = File.Create(uploadPath);
        await content.CopyToAsync(fileStream, cancellationToken);

        return new StorageResult(relativePath, GetPublicUrl(relativePath), fileStream.Length);
    }

    public async Task<Stream> DownloadAsync(string path, CancellationToken ct = default)
    {
        var exists = await ExistsAsync(path, ct);

        if (!exists)
        {
            throw new InvalidOperationException("Invalid path.");
        }
        
        var rootPath = Path.GetFullPath(options.Value.RootPath);                                                                                                                                                
        var uploadPath = Path.GetFullPath(Path.Combine(rootPath, path));      
        
        return File.OpenRead(uploadPath);
    }

    public async Task DeleteAsync(string path, CancellationToken ct = default)
    {
        var exists = await ExistsAsync(path, ct);

        if (!exists)
        {
            throw new InvalidOperationException("Invalid path.");
        }
        
        var rootPath = Path.GetFullPath(options.Value.RootPath);                                                                                                                                                
        var uploadPath = Path.GetFullPath(Path.Combine(rootPath, path));
        
        File.Delete(uploadPath);
    }

    public Task<bool> ExistsAsync(string path, CancellationToken ct = default)
    {
        var rootPath = Path.GetFullPath(options.Value.RootPath);                                                                                                                                                
        var uploadPath = Path.GetFullPath(Path.Combine(rootPath, path));

        return Task.FromResult(File.Exists(uploadPath));
    }

    public string GetPublicUrl(string path)
    {
        return $"/uploads/{path}"; 
    }
}