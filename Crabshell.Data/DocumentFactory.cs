using Crabshell.Core;
using Crabshell.Core.Documents;
using Crabshell.Core.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Data;

public class DocumentFactory(IServiceProvider services) :  IDocumentFactory
{
    public CrabshellDocument Create(CollectionMeta collection)
        => (CrabshellDocument)ActivatorUtilities.CreateInstance(services, collection.ClrType);
}