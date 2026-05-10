namespace Crabshell.Core.Storage;

/// <summary>
/// Pluggable file storage abstraction used by media fields.
/// Register a built-in provider (local, Azure, S3, GCS) or implement this interface for a custom backend.
/// </summary>
public interface IStorageProvider
{
    /// <summary>Uploads a file stream to the given path and returns the stored path and public URL.</summary>
    Task<StorageResult> UploadAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default);

    /// <summary>Deletes the file at the given path. No-ops if the file does not exist.</summary>
    Task DeleteAsync(string path, CancellationToken ct = default);

    /// <summary>Returns <c>true</c> if a file exists at the given path.</summary>
    Task<bool> ExistsAsync(string path, CancellationToken ct = default);

    /// <summary>Converts a stored relative path to a publicly accessible URL.</summary>
    string GetPublicUrl(string path);
}

/// <summary>Result returned by <see cref="IStorageProvider.UploadAsync"/>.</summary>
/// <param name="Path">The relative path the file was stored at.</param>
/// <param name="PublicUrl">The publicly accessible URL for the file.</param>
/// <param name="SizeBytes">Size of the uploaded file in bytes.</param>
public record StorageResult(string Path, string PublicUrl, long SizeBytes);