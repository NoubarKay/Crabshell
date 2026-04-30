using Crabshell.Core.Attributes;
using Crabshell.Core.Registry;
using Microsoft.EntityFrameworkCore;

namespace Crabshell.Data;

public partial class CrabshellDbContext(DbContextOptions<CrabshellDbContext> options, CollectionRegistry registry) : DbContext(options)
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var collections = registry.GetAll();

        foreach (var collection in collections)
        {
            var entityBuilder = modelBuilder.Entity(collection.ClrType);
            entityBuilder.ToTable(collection.Slug);

            foreach (var field in collection.Fields)
            {
                var property = entityBuilder.Property(field.ClrType, field.PropertyName);

                if (field.Required)
                    property.IsRequired();

                if (field.TextSettings is { MaxLength: not -1 } text)
                    property.HasMaxLength(text.MaxLength);
            }

            foreach (var field in collection.Fields.Where(f =>
                f.FieldType == FieldType.Relationship && f.RelationshipSettings is not null))
            {
                var related = collections.FirstOrDefault(c => c.Slug == field.RelationshipSettings!.Slug);
                if (related is null) continue;

                entityBuilder
                    .HasOne(related.ClrType, null)
                    .WithMany()
                    .HasForeignKey(field.PropertyName)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}