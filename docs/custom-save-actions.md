# Custom Save Actions

By default the collection edit page offers built-in save behaviours controlled by the `SaveOption` flags enum:

| Flag | Split button entry |
|---|---|
| `SaveOption.Save` | Simple "Save" button (no dropdown) |
| `SaveOption.SaveAndStayHere` | "Save and Stay Here" |
| `SaveOption.SaveAndClone` | "Save and Clone" |
| `SaveOption.SaveAndGoToNext` | "Save and Go to Next" |

If you need behaviour beyond these — e.g. "Save and Publish", "Save and Send Email", "Save and Sync to API" — you can register **custom save actions** that appear automatically in the same split button.

---

## 1. Implement `ICustomSaveAction`

Create a class in your project that implements `Crabshell.Core.SaveActions.ICustomSaveAction`. The class must have a **public parameterless constructor**.

```csharp
using Crabshell.Core.SaveActions;

public class PublishAction : ICustomSaveAction
{
    // Text shown in the split button dropdown
    public string Label => "Save and Publish";

    // Unique key — must not be "stay", "clone", or "next" (reserved)
    public string Value => "publish";

    // Called after the document has been saved successfully.
    // Return a URL to navigate to, or null to go to the collection list.
    public async Task<string?> ExecuteAsync(SaveActionContext ctx)
    {
        var publishSvc = ctx.Services.GetRequiredService<IPublishService>();
        await publishSvc.PublishAsync(ctx.Slug, ctx.SavedId);

        return null; // null → navigate to collection list
    }
}
```

### `SaveActionContext` properties

| Property | Type | Description |
|---|---|---|
| `Slug` | `string` | The collection slug, e.g. `"articles"` |
| `SavedId` | `Guid` | ID of the document that was just saved |
| `CollectionService` | `ICollectionService` | Built-in service for CRUD operations |
| `Services` | `IServiceProvider` | Full DI container — use `GetRequiredService<T>()` to resolve anything else |

---

## 2. Register the action on your collection

Pass your action type(s) to `CustomSaveOptions` on the `[Collection]` attribute:

```csharp
[Collection("articles",
    Label = "Articles",
    SaveOptions = SaveOption.Save | SaveOption.SaveAndStayHere,
    CustomSaveOptions = new[] { typeof(PublishAction), typeof(AnotherAction) }
)]
public class Article : CrabshellDocument
{
    ...
}
```

Custom actions appear **after** the built-in entries in the dropdown, in the order they are listed.

---

## 3. Rules and constraints

- The `Value` property must be **unique** across all custom actions on the same collection.
- The following values are **reserved** by built-in actions and cannot be used: `stay`, `clone`, `next`.
- The type must implement `ICustomSaveAction`.
- The type must have a **public parameterless constructor** (constructor injection is not supported — use `ctx.Services` for dependencies instead).
- Validation is done at startup when the collection is registered, so errors surface immediately on launch rather than at runtime.

---

## 4. Full example

```csharp
// Actions/PublishAction.cs
public class PublishAction : ICustomSaveAction
{
    public string Label => "Save and Publish";
    public string Value => "publish";

    public async Task<string?> ExecuteAsync(SaveActionContext ctx)
    {
        var svc = ctx.Services.GetRequiredService<IPublishService>();
        await svc.PublishAsync(ctx.SavedId);
        return null;
    }
}

// Actions/ArchiveAction.cs
public class ArchiveAction : ICustomSaveAction
{
    public string Label => "Save and Archive";
    public string Value => "archive";

    public async Task<string?> ExecuteAsync(SaveActionContext ctx)
    {
        var svc = ctx.Services.GetRequiredService<IArchiveService>();
        await svc.ArchiveAsync(ctx.SavedId);
        // Navigate to a specific page after archiving
        return $"/admin/collections/{ctx.Slug}?filter=archived";
    }
}

// Collections/Article.cs
[Collection("articles",
    Label = "Articles",
    SaveOptions = SaveOption.Save | SaveOption.SaveAndStayHere,
    CustomSaveOptions = new[] { typeof(PublishAction), typeof(ArchiveAction) }
)]
public class Article : CrabshellDocument
{
    [TextField(Required = true, MaxLength = 200, Label = "Title")]
    public string Title { get; set; } = default!;
}
```

The split button on the Article edit page will show:

- **Save** (primary button)
- Save and Stay Here
- Save and Publish
- Save and Archive