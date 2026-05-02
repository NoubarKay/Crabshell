# Crabshell

A headless CMS framework for .NET Blazor. Define collections as C# classes with attributes and get a full admin UI and PostgreSQL-backed data layer automatically.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL instance

## Setup

**1. Configure the connection string**

Edit `Crabshell.Sample/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=crabshell;Username=postgres;Password=yourpassword"
}
```

Also update the hardcoded connection string in `Crabshell.Sample/CrabshellDbContextFactory.cs` to match.

**2. Run migrations**

```bash
cd Crabshell.Sample
dotnet ef database update
```

**3. Register Crabshell in `Program.cs`**

```csharp
builder.Services.AddCrabshellCore(typeof(Program).Assembly);
builder.Services.AddCrabshellData<CrabshellDb>(
    builder.Configuration.GetConnectionString("DefaultConnection")!);

// ...

app.MapCrabshellAdmin();
await app.UseCrabshellDataAsync();
```

**4. Run the app**

```bash
dotnet run
```

The admin UI is available at `/admin`.

---

## Defining a collection

Extend `CrabshellDocument` and annotate with `[Collection]`:

```csharp
using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.Documents;

[Collection("articles", Label = "Articles")]
public class Article : CrabshellDocument
{
    [GridOptions(Visible = true, Order = 0)]
    [TextField(Required = true, MaxLength = 200, Label = "Title")]
    public string Title { get; set; } = default!;

    [TextField(MaxLength = -1, Label = "Body")]
    public string? Body { get; set; }

    [SelectField(Required = true, Label = "Status")]
    public ArticleStatus Status { get; set; }
}

public enum ArticleStatus { Draft, Published, Archived }
```

Collections are picked up automatically from the assembly passed to `AddCrabshellCore`. After adding a new collection, create and apply a migration:

```bash
dotnet ef migrations add AddArticles
dotnet ef database update
```

### `[Collection]` options

| Property | Type | Description |
|---|---|---|
| `Slug` *(required)* | `string` | URL-safe identifier used as the table name and route segment |
| `Label` | `string?` | Human-readable name shown in the admin UI. Defaults to the slug |
| `SaveOptions` | `SaveOption` | Flags controlling which save actions appear in the edit page button |
| `CustomSaveOptions` | `Type[]` | Custom save action types added to the split button — see [Custom Save Actions](docs/custom-save-actions.md) |

---

## Field attributes

Only one field attribute may be applied per property. All field attributes share these base options:

| Option | Type | Description |
|---|---|---|
| `Label` | `string?` | Field label shown in the admin UI. Defaults to the property name |
| `Required` | `bool` | Marks the field as required. Validated on save |

### `[TextField]`

Maps to a `varchar` or `text` column.

| Option | Default | Description |
|---|---|---|
| `MaxLength` | `255` | Max character length. `-1` = unlimited (`text` column) |
| `MinLength` | `0` | Minimum character length |
| `Pattern` | `null` | Regex pattern validated on save |

### `[NumberField]`

Maps to `integer`, `bigint`, `numeric`, or `double precision` based on the CLR type.

| Option | Default | Description |
|---|---|---|
| `Decimals` | `2` | Decimal places (only for `decimal` properties) |
| `Min` | — | Minimum allowed value |
| `Max` | — | Maximum allowed value |
| `Step` | `"1"` | Spinner increment in the admin UI |
| `Prefix` | `null` | Text shown before the input, e.g. `"$"` |
| `Suffix` | `null` | Text shown after the input, e.g. `"%"` |
| `Format` | `null` | Display format string |

### `[BoolField]`

Maps to a `boolean` column.

| Option | Default | Description |
|---|---|---|
| `IsSwitch` | `false` | Renders as a toggle switch instead of a checkbox |

### `[SelectField]`

Maps to an `integer` (enum) or `varchar(255)` (string options) column.

| Option | Default | Description |
|---|---|---|
| `Options` | `[]` | Explicit string options. Ignored if the property type is an enum |
| `Multiple` | `false` | Allow multiple selections |

When the property type is an enum, options are derived automatically from the enum member names.

### `[DateTimeField]`

Maps to `date`, `timestamptz`, or `timetz` depending on the options.

| Option | Default | Description |
|---|---|---|
| `HasTime` | `true` | Include a time component (`timestamptz`) |
| `TimeOnly` | `false` | Show only a time picker (`timetz`) |
| `Utc` | `true` | Store as UTC |
| `Min` / `Max` | `null` | Allowed date range, format `"yyyy-MM-dd"` |
| `ShowNowButton` | `false` | Add a "Now" shortcut button |
| `HoursStep` | `1` | Hour increment in the time picker |
| `MinutesStep` | `1` | Minute increment |
| `SecondsStep` | `1` | Second increment |
| `Immediate` | `false` | Update value on every keystroke |
| `Inline` | `false` | Show the calendar inline (always visible) |
| `ShowButton` | `true` | Show the calendar icon button |
| `YearRange` | `"1950:2056"` | Range shown in the year dropdown |
| `DateFormat` | `null` | Display format, e.g. `"MM/dd/yyyy"` |

### `[RelationshipField(typeof(T))]`

Maps to a `uuid` foreign key column pointing to another collection.

```csharp
[RelationshipField(typeof(Agency), Label = "Agency")]
public Guid? AgencyId { get; set; }
```

The dropdown in the admin UI is populated from the related collection and filtered by the first `[TextField]` on that type.

---

## Grid options

Use `[GridOptions]` alongside a field attribute to control how the column appears in the collection list view.

| Option | Default | Description |
|---|---|---|
| `Visible` | `true` | Show this field as a column |
| `Label` | field label | Column header override |
| `Order` | `0` | Column position — lower = further left |
| `Width` | `0` | Column width in pixels. `0` = auto |
| `Sortable` | `true` | Allow sorting by this column |
| `Filterable` | `false` | Allow filtering by this column |

---

## Field groups

Use `[FieldGroup]` to group related fields together in the edit page. Groups can be placed in the main content area or the sidebar.

```csharp
[FieldGroup("SEO", Sidebar = true)]
[TextField(MaxLength = 160, Label = "Meta Description")]
public string? MetaDescription { get; set; }
```

| Option | Default | Description |
|---|---|---|
| `Name` *(required)* | — | Group heading shown in the UI |
| `Sidebar` | `false` | Place the group in the right sidebar instead of the main area |

---

## Save options

Control which actions appear on the edit page Save button via the `SaveOptions` flag:

```csharp
[Collection("articles",
    SaveOptions = SaveOption.Save | SaveOption.SaveAndStayHere | SaveOption.SaveAndClone)]
```

| Flag | Behaviour |
|---|---|
| `SaveOption.Save` | Navigate to the collection list after saving |
| `SaveOption.SaveAndStayHere` | Stay on the edit page (updates the URL to the new ID if creating) |
| `SaveOption.SaveAndClone` | Duplicate the document and open the clone for editing |
| `SaveOption.SaveAndGoToNext` | Navigate to the next document in the list |

For custom save behaviour (e.g. "Save and Publish"), see [Custom Save Actions](docs/custom-save-actions.md).

---

## Feature Requests

- Open an issue — changes are made when someone volunteers or when time allows.
- Submit a pull request.
- Hire someone to implement it.

## Support

The support you get is the support you pay for.