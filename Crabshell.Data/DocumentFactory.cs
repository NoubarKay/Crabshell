using Crabshell.Core;
using Crabshell.Core.Documents;
using Crabshell.Core.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Data;

public class DocumentFactory(IServiceProvider services) :  IDocumentFactory
{
    public CrabshellDocument Create(CollectionMeta collection)
    {
        var document = (CrabshellDocument)ActivatorUtilities.CreateInstance(services, collection.ClrType);

        foreach (var field in collection.Fields)
        {
            // Apply explicit DefaultValue if set
            if (field.DefaultValue is not null)
            {
                field.Setter(document, field.DefaultValue);
                continue;
            }

            // Seed required or non-nullable reference fields with a safe empty value
            // so the INSERT never violates a NOT NULL constraint
            var underlying = Nullable.GetUnderlyingType(field.ClrType) ?? field.ClrType;
            if (!field.ClrType.IsValueType && field.Getter(document) is null)
                field.Setter(document, underlying == typeof(string) ? "" : null);
        }

        return document;
    }
}