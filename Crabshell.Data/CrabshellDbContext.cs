using System.Linq.Expressions;
using Crabshell.Core.Attributes;
using Crabshell.Core.Documents;
using Crabshell.Core.Registry;
using Microsoft.EntityFrameworkCore;

namespace Crabshell.Data;

public partial class CrabshellDbContext(DbContextOptions<CrabshellDbContext> options, CollectionRegistry registry, CrabshellModelOptions? modelOptions = null) : DbContext(options)
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var collections = registry.GetAll().Concat(registry.GetAllSingles()).ToList();
        var configuredJoinTables = new HashSet<string>();

        foreach (var collection in collections)
        {
            var entityBuilder = modelBuilder.Entity(collection.ClrType);
            entityBuilder.ToTable(collection.Slug);

            foreach (var field in collection.Fields)
            {
                // Many-to-many fields are backed by a join table, not a column on this entity.
                if (field.FieldType == FieldType.ManyToMany)
                {
                    entityBuilder.Ignore(field.PropertyName);
                    continue;
                }

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

            foreach (var field in collection.Fields.Where(f =>
                f.FieldType == FieldType.ManyToMany && f.ManyToManySettings is not null))
            {
                var settings = field.ManyToManySettings!;
                if (!configuredJoinTables.Add(settings.JoinTableName)) continue;

                var target = collections.FirstOrDefault(c => c.Slug == settings.TargetSlug);
                if (target is null) continue;

                var sourceClr = collection.ClrType;
                var targetClr = target.ClrType;
                var sourceCol = settings.SourceColumn;
                var targetCol = settings.TargetColumn;
                var joinTable = settings.JoinTableName;

                modelBuilder.SharedTypeEntity<Dictionary<string, object>>(joinTable, b =>
                {
                    b.ToTable(joinTable);
                    b.Property<Guid>(sourceCol);
                    b.Property<Guid>(targetCol);
                    b.HasKey(sourceCol, targetCol);
                    b.HasOne(sourceClr, null).WithMany().HasForeignKey(sourceCol).OnDelete(DeleteBehavior.Cascade);
                    b.HasOne(targetClr, null).WithMany().HasForeignKey(targetCol).OnDelete(DeleteBehavior.Cascade);
                });
            }

            var param = Expression.Parameter(collection.ClrType, "e");
            var isDeleted = Expression.Property(param, nameof(CrabshellDocument.IsDeleted));
            var notDeleted = Expression.Equal(isDeleted, Expression.Constant(false));
            entityBuilder.HasQueryFilter(Expression.Lambda(notDeleted, param));
        }

        modelOptions?.Configure?.Invoke(modelBuilder);
    }
}