# Model Configuration

Crabshell automatically generates EF Core entity mappings from your collection classes — table names, column types, required constraints, max lengths, foreign keys, and soft-delete query filters. The `configureModel` callback lets you layer additional EF Core configuration on top without touching Crabshell internals.

---

## Registering a callback

Pass an `Action<ModelBuilder>` as the third argument to `AddCrabshellData`:

```csharp
builder.Services.AddCrabshellData<CrabshellDb>(
    builder.Configuration.GetConnectionString("DefaultConnection")!,
    model =>
    {
        // Your EF Core configuration here
    });
```

The callback is optional — omitting it leaves Crabshell's default behaviour unchanged. When provided it runs **after** all Crabshell-managed configuration, so anything you configure here takes precedence over the defaults.

---

## What you can configure

### Indexes

Crabshell does not add indexes beyond the primary key. Add them here:

```csharp
model =>
{
    model.Entity<Listing>()
        .HasIndex(l => l.Price);

    model.Entity<Listing>()
        .HasIndex(l => new { l.NeighborhoodId, l.Price })
        .HasDatabaseName("ix_listing_neighborhood_price");

    model.Entity<Article>()
        .HasIndex(a => a.Slug)
        .IsUnique();
}
```

### Column overrides

Override column names, types, or defaults that Crabshell has already set:

```csharp
model =>
{
    model.Entity<Agent>()
        .Property(a => a.Email)
        .HasColumnName("email_address")
        .HasMaxLength(320);

    model.Entity<Listing>()
        .Property(l => l.Price)
        .HasPrecision(12, 2);
}
```

### Relationship delete behaviour

Crabshell defaults all foreign keys to `ON DELETE RESTRICT`. Override a specific relationship here:

```csharp
model =>
{
    model.Entity<Listing>()
        .HasOne<Neighborhood>()
        .WithMany()
        .HasForeignKey(nameof(Listing.NeighborhoodId))
        .OnDelete(DeleteBehavior.SetNull);
}
```

### Check constraints

```csharp
model =>
{
    model.Entity<Listing>()
        .ToTable(t => t.HasCheckConstraint("ck_listing_price_positive", "price > 0"));
}
```

### Computed columns

```csharp
model =>
{
    model.Entity<Listing>()
        .Property<string>("FullAddress")
        .HasComputedColumnSql("street || ', ' || city", stored: true);
}
```

### Owned types / value objects

If you have a CLR type that is not a Crabshell collection but should be owned by one:

```csharp
model =>
{
    model.Entity<Agent>()
        .OwnsOne(a => a.Address, addr =>
        {
            addr.Property(x => x.Street).HasColumnName("address_street");
            addr.Property(x => x.City).HasColumnName("address_city");
        });
}
```

---

## What you should not override

Avoid reconfiguring the things Crabshell manages internally, as the results are undefined:

- The primary key (`Id`)
- The base timestamp columns (`CreatedAt`, `UpdatedAt`, `DeletedAt`)
- The soft-delete query filter (`IsDeleted == false`)
- The table name (`ToTable(collection.Slug)`)
- Foreign key mappings that match a `[RelationshipField]` — override the delete behaviour only if needed, and use the exact `HasForeignKey` name that Crabshell generated (the property name)

---

## Migrations

The callback participates in the EF Core model the same way any `OnModelCreating` configuration does. After adding or changing configuration, create and apply a migration:

```bash
dotnet ef migrations add AddListingPriceIndex
dotnet ef database update
```

In development, `SchemaDiffService` handles table and column creation automatically at startup. Index and constraint changes from the callback require a migration in all environments.