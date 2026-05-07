namespace Crabshell.Core.Media;

public record StorageResult(
    string FileKey,
    string PublicUrl,
    long SizeInBytes,
    string ContentType);