using System.Linq.Expressions;
using Crabshell.Core.Attributes;
using Crabshell.Core.Documents;
using Crabshell.Core.Registry;
using Crabshell.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace Crabshell.Data;

public class CollectionRepository : ICollectionRepository
{
    protected readonly CrabshellDbContext DbContext;

    private static readonly System.Reflection.MethodInfo _setMethod =
        typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!;

    public CollectionRepository(CrabshellDbContext dbContext)
    {
        DbContext = dbContext;
    }

    private IQueryable<CrabshellDocument> GetQueryable(CollectionMeta collection) =>
        (IQueryable<CrabshellDocument>)_setMethod
            .MakeGenericMethod(collection.ClrType)
            .Invoke(DbContext, null)!;

    public virtual async Task<PagedResult> GetPageAsync(CollectionMeta collection, CollectionQuery query)
    {
        var q = GetQueryable(collection);

        // Filtering
        foreach (var filter in query.Filters ?? [])
        {
            var fieldMeta = collection.Fields.FirstOrDefault(f => f.PropertyName == filter.Property);
            if (fieldMeta is null || filter.Value is null) continue;

            var param = Expression.Parameter(typeof(CrabshellDocument), "doc");
            var prop = Expression.Property(Expression.Convert(param, collection.ClrType), filter.Property);
            var underlyingType = Nullable.GetUnderlyingType(fieldMeta.ClrType) ?? fieldMeta.ClrType;
            var converted = Convert.ChangeType(filter.Value, underlyingType);
            var constant = Expression.Constant(converted, fieldMeta.ClrType);

            Expression? body = filter.Operator switch
            {
                FieldFilterOperator.Contains            => Expression.Call(prop, nameof(string.Contains),   null, constant),
                FieldFilterOperator.DoesNotContain      => Expression.Not(Expression.Call(prop, nameof(string.Contains), null, constant)),
                FieldFilterOperator.StartsWith          => Expression.Call(prop, nameof(string.StartsWith), null, constant),
                FieldFilterOperator.EndsWith            => Expression.Call(prop, nameof(string.EndsWith),   null, constant),
                FieldFilterOperator.Equals              => Expression.Equal(prop, constant),
                FieldFilterOperator.NotEquals           => Expression.NotEqual(prop, constant),
                FieldFilterOperator.GreaterThan         => Expression.GreaterThan(prop, constant),
                FieldFilterOperator.GreaterThanOrEquals => Expression.GreaterThanOrEqual(prop, constant),
                FieldFilterOperator.LessThan            => Expression.LessThan(prop, constant),
                FieldFilterOperator.LessThanOrEquals    => Expression.LessThanOrEqual(prop, constant),
                _ => null
            };

            if (body is null) continue;
            q = q.Where(Expression.Lambda<Func<CrabshellDocument, bool>>(body, param));
        }

        // Sorting
        if (!string.IsNullOrEmpty(query.OrderBy))
        {
            var sortField = collection.Fields.FirstOrDefault(f => f.PropertyName == query.OrderBy);
            if (sortField is not null)
            {
                var param = Expression.Parameter(typeof(CrabshellDocument), "doc");
                var keySelector = Expression.Lambda(
                    Expression.Property(Expression.Convert(param, collection.ClrType), query.OrderBy),
                    param);
                var orderMethod = query.Descending ? "OrderByDescending" : "OrderBy";
                var method = typeof(Queryable)
                    .GetMethods()
                    .First(m => m.Name == orderMethod && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(CrabshellDocument), sortField.ClrType);
                q = (IQueryable<CrabshellDocument>)method.Invoke(null, [q, keySelector])!;
            }
        }

        var totalCount = await q.CountAsync();
        var items = await q.Skip(query.Skip).Take(query.Take).ToListAsync();

        await LoadManyToManyAsync(collection, items);

        return new PagedResult(items, totalCount, query.Skip, query.Take);
    }

    public virtual async Task<CrabshellDocument?> GetByIdAsync(
        CollectionMeta collection, Guid id)
    {
        var doc = await GetQueryable(collection).FirstOrDefaultAsync(x => x.Id == id);
        if (doc is not null)
            await LoadManyToManyAsync(collection, [doc]);
        return doc;
    }

    public virtual async Task<CrabshellDocument> CreateAsync(CrabshellDocument document)
    {
        document.CreatedAt = DateTime.UtcNow;
        document.UpdatedAt = DateTime.UtcNow;
        DbContext.Add(document);
        await DbContext.SaveChangesAsync();
        return document;
    }

    public virtual async Task<CrabshellDocument> UpdateAsync(CrabshellDocument document)
    {
        document.UpdatedAt = DateTime.UtcNow;
        DbContext.Update(document);
        await DbContext.SaveChangesAsync();
        return document;
    }

    public virtual async Task DeleteAsync(CrabshellDocument document)
    {
        document.IsDeleted = true;
        document.DeletedAt = DateTime.UtcNow;
        DbContext.Update(document);
        await DbContext.SaveChangesAsync();
    }

    public virtual async Task SyncManyToManyAsync(CollectionMeta collection, CrabshellDocument document)
    {
        var m2mFields = collection.Fields
            .Where(f => f.FieldType == FieldType.ManyToMany && f.ManyToManySettings is not null)
            .ToList();
        if (m2mFields.Count == 0) return;

        foreach (var field in m2mFields)
        {
            var settings  = field.ManyToManySettings!;
            var sourceCol = settings.SourceColumn;
            var targetCol = settings.TargetColumn;
            var set       = JoinSet(settings.JoinTableName);

            var desired = (field.Getter(document) as IEnumerable<Guid>)?.Distinct().ToHashSet() ?? [];

            var existingRows = await set
                .Where(r => EF.Property<Guid>(r, sourceCol) == document.Id)
                .ToListAsync();
            var existingTargets = existingRows.Select(r => (Guid)r[targetCol]).ToHashSet();

            foreach (var row in existingRows)
                if (!desired.Contains((Guid)row[targetCol]))
                    set.Remove(row);

            foreach (var targetId in desired)
                if (!existingTargets.Contains(targetId))
                    set.Add(new Dictionary<string, object>
                    {
                        [sourceCol] = document.Id,
                        [targetCol] = targetId,
                    });
        }

        await DbContext.SaveChangesAsync();
    }

    private DbSet<Dictionary<string, object>> JoinSet(string joinTable) =>
        DbContext.Set<Dictionary<string, object>>(joinTable);

    private async Task LoadManyToManyAsync(CollectionMeta collection, IReadOnlyList<CrabshellDocument> documents)
    {
        var m2mFields = collection.Fields
            .Where(f => f.FieldType == FieldType.ManyToMany && f.ManyToManySettings is not null)
            .ToList();
        if (m2mFields.Count == 0 || documents.Count == 0) return;

        var ids = documents.Select(d => d.Id).ToList();

        foreach (var field in m2mFields)
        {
            var settings  = field.ManyToManySettings!;
            var sourceCol = settings.SourceColumn;
            var targetCol = settings.TargetColumn;

            var rows = await JoinSet(settings.JoinTableName)
                .Where(r => ids.Contains(EF.Property<Guid>(r, sourceCol)))
                .Select(r => new
                {
                    Source = EF.Property<Guid>(r, sourceCol),
                    Target = EF.Property<Guid>(r, targetCol),
                })
                .ToListAsync();

            var bySource = rows
                .GroupBy(r => r.Source)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Target).ToList());

            foreach (var doc in documents)
                field.Setter(doc, bySource.TryGetValue(doc.Id, out var list) ? list : new List<Guid>());
        }
    }
}