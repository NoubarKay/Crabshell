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

**3. Run the app**

```bash
dotnet run
```

The admin UI is available at `/admin`.

## Defining a collection

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

Collections are picked up automatically from the assembly registered in `Program.cs`:

```csharp
builder.Services.AddCrabshellCore(typeof(Program).Assembly);
builder.Services.AddCrabshellData<CrabshellDb>(builder.Configuration.GetConnectionString("DefaultConnection")!);
```

After adding a new collection, create and apply a new migration:

```bash
dotnet ef migrations add AddArticles
dotnet ef database update
```

## Available field attributes

| Attribute | Description |
|---|---|
| `[TextField]` | Single or multiline text |
| `[NumberField]` | Numeric input with optional prefix/suffix/decimals |
| `[BoolField]` | Checkbox or toggle switch |
| `[SelectField]` | Dropdown from an enum (supports `Multiple = true`) |
| `[DateTimeField]` | Date/time picker |
| `[RelationshipField]` | Foreign key to another collection |

Use `[GridOptions]` on any field to control column visibility, order, width, sorting, and filtering in the list view.

# Feature Requests

When it comes to feature requests you have a few options

* Make an issue in the repo requesting the change. The change will be made if someone wants to make it or when I have time.
* Make your own pull request.
* Pay someone to do it for you.

## Support

The support you get it the support you pay for.