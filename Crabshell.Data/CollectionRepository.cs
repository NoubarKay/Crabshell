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

    protected IQueryable<CrabshellDocument> GetQueryable(CollectionMeta collection) =>
        (IQueryable<CrabshellDocument>)_setMethod
            .MakeGenericMethod(collection.ClrType)
            .Invoke(DbContext, null)!;

    public virtual async Task<List<CrabshellDocument>> GetAllAsync(
        CollectionMeta collection, int page = 1, int pageSize = 50) =>
        await GetQueryable(collection)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

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
        DbContext.Remove(document);
        await DbContext.SaveChangesAsync();
    }
}