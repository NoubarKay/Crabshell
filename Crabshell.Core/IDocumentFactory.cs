using Crabshell.Core.Documents;
using Crabshell.Core.Registry;

namespace Crabshell.Core;

/// <summary>Creates blank document instances from collection metadata.</summary>
public interface IDocumentFactory
{
    /// <summary>Activates a new instance of the CLR type described by <paramref name="collection"/>.</summary>
    CrabshellDocument Create(CollectionMeta collection);
}