using System.Linq.Expressions;
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

        return new PagedResult(items, totalCount, query.Skip, query.Take);
    }

    public virtual async Task<CrabshellDocument?> GetByIdAsync(
        CollectionMeta collection, Guid id) =>
        await GetQueryable(collection)
            .FirstOrDefaultAsync(x => x.Id == id);

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
}