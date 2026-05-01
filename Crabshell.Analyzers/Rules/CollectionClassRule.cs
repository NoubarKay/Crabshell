using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CollectionClassRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor RuleNoDocument = new(
        id: "CRBSH023",
        title: "Collection class does not extend CrabshellDocument",
        messageFormat: "'{0}' is marked [Collection] but does not extend CrabshellDocument",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor RuleUnmappedProperty = new(
        id: "CRBSH024",
        title: "Public property in Collection class has no field attribute",
        messageFormat: "'{0}' is a public property in a [Collection] class but has no field attribute — it will not be mapped",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [RuleNoDocument, RuleUnmappedProperty];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        var type = (INamedTypeSymbol)context.Symbol;

        if (!type.GetAttributes().Any(a => a.AttributeClass?.Name == "CollectionAttribute")) return;

        // CRBSH023 — must extend CrabshellDocument
        if (!InheritsFrom(type, "CrabshellDocument"))
        {
            context.ReportDiagnostic(Diagnostic.Create(RuleNoDocument, type.Locations[0], type.Name));
            return;
        }

        // CRBSH024 — public properties with no field attribute
        // Collect base class property names to exclude (Id, CreatedAt, UpdatedAt)
        var baseProps = GetBasePropertyNames(type);

        foreach (var prop in type.GetMembers().OfType<IPropertySymbol>())
        {
            if (prop.DeclaredAccessibility != Accessibility.Public) continue;
            if (prop.IsStatic) continue;
            if (baseProps.Contains(prop.Name)) continue;

            var hasFieldAttribute = prop.GetAttributes()
                .Any(a => InheritsFromCrabshellFieldAttribute(a.AttributeClass));

            if (!hasFieldAttribute)
                context.ReportDiagnostic(Diagnostic.Create(RuleUnmappedProperty, prop.Locations[0], prop.Name));
        }
    }

    private static bool InheritsFrom(INamedTypeSymbol type, string baseName)
    {
        var current = type.BaseType;
        while (current != null)
        {
            if (current.Name == baseName) return true;
            current = current.BaseType;
        }
        return false;
    }

    private static System.Collections.Generic.HashSet<string> GetBasePropertyNames(INamedTypeSymbol type)
    {
        var names = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);
        var current = type.BaseType;
        while (current != null)
        {
            foreach (var m in current.GetMembers().OfType<IPropertySymbol>())
                names.Add(m.Name);
            current = current.BaseType;
        }
        return names;
    }

    private static bool InheritsFromCrabshellFieldAttribute(INamedTypeSymbol? type)
    {
        while (type != null)
        {
            if (type.Name == "CrabshellFieldAttribute") return true;
            type = type.BaseType;
        }
        return false;
    }
}
