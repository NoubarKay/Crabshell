using Crabshell.Core.Documents;
using Crabshell.Core.Registry;

namespace Crabshell.Core;

public interface IDocumentFactory
{
    CrabshellDocument Create(CollectionMeta collection);
}