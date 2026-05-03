# Bulk Actions

Bulk actions let you perform operations on multiple selected collection records at once. When rows are selected in the collection list, an **Actions (N)** split button appears in the header — clicking it runs the chosen action against all selected records.

---

## 1. Implement `IBulkAction`

Create a class in your project that implements `Crabshell.Core.BulkActions.IBulkAction`. The class must have a **public parameterless constructor**.

```csharp
using Crabshell.Core.BulkActions;

public class ExportAgenciesAction : IBulkAction
{
    // Text shown in the Actions dropdown
    public string Label => "Export Selected";

    // Unique key used to identify this action
    public string Value => "export_selected";

    // Called when the user clicks this action in the dropdown.
    // ctx.SelectedIds contains the IDs of every checked row.
    public async Task ExecuteAsync(BulkActionContext ctx)
    {
        var exportSvc = ctx.Services.GetRequiredService<IExportService>();
        await exportSvc.ExportAsync(ctx.Slug, ctx.SelectedIds);
    }
}
```

### `BulkActionContext` properties

| Property | Type | Description |
|---|---|---|
| `Slug` | `string` | The collection slug, e.g. `"agencies"` |
| `SelectedIds` | `IReadOnlyList<Guid>` | IDs of every selected record |
| `CollectionService` | `ICollectionService` | Built-in service for CRUD operations |
| `Services` | `IServiceProvider` | Full DI container — use `GetRequiredService<T>()` to resolve anything else |

---

## 2. Register the action on your collection

Pass your action type(s) to `CustomBulkOptions` on the `[Collection]` attribute:

```csharp
[Collection("agencies",
    Label = "Agencies",
    CustomBulkOptions = new[] { typeof(ExportAgenciesAction), typeof(AnotherBulkAction) }
)]
public class Agency : CrabshellDocument
{
    ...
}
```

Actions appear in the dropdown in the order they are listed.

---

## 3. Rules and constraints

- The `Value` property must be **unique** across all bulk actions on the same collection.
- The type must implement `IBulkAction`.
- The type must have a **public parameterless constructor** (constructor injection is not supported — use `ctx.Services` for dependencies instead).
- Validation is done at startup when the collection is registered, so errors surface immediately on launch rather than at runtime.

---

## 4. Full example

```csharp
// Actions/DeleteSelectedAgenciesAction.cs
public class DeleteSelectedAgenciesAction : IBulkAction
{
    public string Label => "Delete Selected";
    public string Value => "delete_selected";

    public async Task ExecuteAsync(BulkActionContext ctx)
    {
        foreach (var id in ctx.SelectedIds)
            await ctx.CollectionService.DeleteAsync(ctx.Slug, id);
    }
}

// Actions/ArchiveBulkAction.cs
public class ArchiveBulkAction : IBulkAction
{
    public string Label => "Archive Selected";
    public string Value => "archive_selected";

    public async Task ExecuteAsync(BulkActionContext ctx)
    {
        var archiveSvc = ctx.Services.GetRequiredService<IArchiveService>();
        await archiveSvc.ArchiveManyAsync(ctx.SelectedIds);
    }
}

// Collections/Agency.cs
[Collection("agencies",
    Label = "Agencies",
    CustomBulkOptions = new[] { typeof(DeleteSelectedAgenciesAction), typeof(ArchiveBulkAction) }
)]
public class Agency : CrabshellDocument
{
    [TextField(Required = true, MaxLength = 120, Label = "Name")]
    public string Name { get; set; } = default!;
}
```

The Actions button on the Agencies list page will show:

- **Delete Selected**
- **Archive Selected**
