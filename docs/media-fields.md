# Media Fields & Storage

`[MediaField]` stores a file path in the database and renders a file upload widget in the admin — drag-and-drop, image preview, and a clear button. Uploaded files are handled by a pluggable `IStorageProvider`.

---

## 1. Register a storage provider

Media fields require a storage provider. Register one in `Program.cs` **before** `app.MapCrabshellAdmin()`.

### Local filesystem

Suitable for development. Files are served directly from `wwwroot`.

```csharp
builder.Services.UseCrabshellLocalStorage(opts =>
{
    opts.RootPath = "wwwroot/uploads"; // path relative to the project root
});
```

### Azure Blob Storage

```csharp
builder.Services.UseCrabshellAzureStorage(opts =>
{
    opts.ConnectionString = builder.Configuration["Azure:StorageConnectionString"]!;
    opts.Container = "media";
    opts.CdnUrl = "https://cdn.example.com"; // optional — prepended to public URLs
});
```

### AWS S3

```csharp
builder.Services.UseCrabshellS3Storage(opts =>
{
    opts.BucketName = "my-bucket";
    opts.Region = "us-east-1";
    opts.AccessKey = builder.Configuration["AWS:AccessKey"];   // optional — uses IAM role if omitted
    opts.SecretKey = builder.Configuration["AWS:SecretKey"];   // optional
    opts.CloudFrontUrl = "https://cdn.example.com";            // optional
});
```

### Google Cloud Storage

```csharp
builder.Services.UseCrabshellGcsStorage(opts =>
{
    opts.BucketName = "my-bucket";
    opts.CredentialsJson = builder.Configuration["GCS:CredentialsJson"]; // optional — uses ADC if omitted
    opts.CdnUrl = "https://cdn.example.com"; // optional
});
```

---

## 2. Add a media field to a collection

```csharp
[MediaField(Label = "Hero Image", Accept = "image/*", MaxSizeMb = 10)]
public string? HeroImagePath { get; set; }
```

The property must be `string?`. The stored value is the relative path returned by the storage provider.

### `[MediaField]` options

| Option | Default | Description |
|---|---|---|
| `Accept` | `"image/*"` | HTML `accept` attribute passed to the file input — e.g. `"image/*"`, `".pdf,.docx"`, `"video/*"` |
| `MaxSizeMb` | `10` | Maximum upload size in MB. Enforced client-side before upload and server-side before calling the provider |
| `Label` | property name | Field label shown in the admin UI |
| `Required` | `false` | Marks the field as required. Validated on save |

---

## 3. How uploads work

When a file is selected in the admin:

1. The file size is checked against `MaxSizeMb` — oversized files are rejected immediately with an error notification.
2. The file is uploaded via `IStorageProvider.UploadAsync` with a path of `{collection-slug}/{guid}{extension}`.
3. The returned path is stored in the form and saved to the database when the document is saved.
4. The admin renders an image preview using `IStorageProvider.GetPublicUrl(path)`.

Clicking **Clear** sets the field to `null`. The old file is not deleted from storage automatically — clean-up is your responsibility (or use a lifecycle hook).

---

## 4. Implementing a custom storage provider

If none of the built-in providers fit, implement `IStorageProvider`:

```csharp
public interface IStorageProvider
{
    Task<StorageResult> UploadAsync(string path, Stream content, string contentType, CancellationToken ct = default);
    Task DeleteAsync(string path, CancellationToken ct = default);
    Task<bool> ExistsAsync(string path, CancellationToken ct = default);
    string GetPublicUrl(string path);
}

public record StorageResult(string Path, string PublicUrl, long SizeBytes);
```

Register it as a singleton:

```csharp
builder.Services.AddSingleton<IStorageProvider, MyCustomStorageProvider>();
```

---

## 5. Accessing the stored path in code

The field stores a relative path. Call `GetPublicUrl` on `IStorageProvider` to convert it to a URL:

```csharp
public class ArticleService(IStorageProvider storage)
{
    public string GetHeroImageUrl(Article article)
        => article.HeroImagePath is not null
            ? storage.GetPublicUrl(article.HeroImagePath)
            : "/images/placeholder.png";
}
```
