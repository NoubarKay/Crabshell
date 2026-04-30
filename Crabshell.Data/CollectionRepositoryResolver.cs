using Crabshell.Core.Registry;
using Crabshell.Core.Repository;

namespace Crabshell.Data;

public class CollectionRepositoryResolver(IServiceProvider provider, ICollectionRepository repo)
{
    public ICollectionRepository Resolve(CollectionMeta collection)
    {
        var genericType = typeof(ICollectionRepository<>)
            .MakeGenericType(collection.ClrType);

        var specific = provider.GetService(genericType) as ICollectionRepository;

        return specific ?? repo;
    }
}