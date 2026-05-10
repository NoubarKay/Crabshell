# Singleton Documents

A singleton is a collection that always has exactly one document — site settings, homepage content, global navigation, feature flags, etc. Unlike regular collections, there is no list page; the admin sidebar links directly to the edit page.

---

## Defining a singleton

Use `[Single]` instead of `[Collection]`:

```csharp
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.Documents;

[Single("site_settings", Label = "Site Settings")]
public class SiteSettings : CrabshellDocument
{
    [TextField(MaxLength = 100, Label = "Site Name")]
    public string? SiteName { get; set; }

    [TextField(MaxLength = 160, Label = "Meta Description")]
    public string? MetaDescription { get; set; }

    [BoolField(IsSwitch = true, Label = "Maintenance Mode")]
    public bool MaintenanceMode { get; set; }
}
```

The document is created automatically on first access — you never need to seed it manually.

### `[Single]` options

| Property | Type | Description |
|---|---|---|
| `Slug` *(required)* | `string` | URL-safe identifier used as the table name and route segment |
| `Label` | `string?` | Human-readable name shown in the admin sidebar. Defaults to the slug |

---

## What's different from `[Collection]`

| | `[Collection]` | `[Single]` |
|---|---|---|
| Admin list page | Yes | No — sidebar links directly to edit |
| Number of documents | Many | Exactly one |
| Auto-created on first access | No | Yes |
| Save options / custom save actions | Yes | No |
| Bulk actions | Yes | No |
| All field types | Yes | Yes |
| Field groups | Yes | Yes |

---

## Reading a singleton in code

Use `ICollectionService.GetSingleAsync`:

```csharp
var result = await collectionService.GetSingleAsync("site_settings");

if (result is Result<CrabshellDocument>.Ok { Value: var doc })
{
    var settings = (SiteSettings)doc;
    // use settings.SiteName, settings.MaintenanceMode, etc.
}
```

---

## All field types are supported

Singletons support the same field attributes as regular collections: `[TextField]`, `[RichTextField]`, `[NumberField]`, `[BoolField]`, `[SelectField]`, `[DateTimeField]`, `[RelationshipField]`, and `[MediaField]`. Field groups and `[GridOptions]` are accepted but grid options have no effect (there is no list view).
