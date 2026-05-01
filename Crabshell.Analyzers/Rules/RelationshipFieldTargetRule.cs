using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RelationshipFieldTargetRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor RuleNotDocument = new(
        id: "CRBSH025",
        title: "RelationshipField target does not extend CrabshellDocument",
        messageFormat: "[RelationshipField] on '{0}' points to '{1}' which does not extend CrabshellDocument",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor RuleDuplicateTarget = new(
        id: "CRBSH026",
        title: "Duplicate RelationshipField target type",
        messageFormat: "'{0}' is a duplicate [RelationshipField] pointing to '{1}' — another property in this collection already targets the same type",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [RuleNotDocument, RuleDuplicateTarget];

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

        // target type name → first property that used it
        var seen = new Dictionary<string, IPropertySymbol>(System.StringComparer.Ordinal);

        foreach (var prop in type.GetMembers().OfType<IPropertySymbol>())
        {
            var attr = prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "RelationshipFieldAttribute");
            if (attr == null) continue;

            if (attr.ConstructorArguments.Length == 0) continue;

            var targetType = attr.ConstructorArguments[0].Value as INamedTypeSymbol;
            if (targetType == null) continue;

            // CRBSH025 — target must extend CrabshellDocument
            if (!InheritsFrom(targetType, "CrabshellDocument"))
            {
                context.ReportDiagnostic(Diagnostic.Create(RuleNotDocument, prop.Locations[0], prop.Name, targetType.Name));
                continue;
            }

            // CRBSH026 — duplicate target type
            var key = targetType.ToDisplayString();
            if (!seen.TryGetValue(key, out _))
                seen[key] = prop;
            else
                context.ReportDiagnostic(Diagnostic.Create(RuleDuplicateTarget, prop.Locations[0], prop.Name, targetType.Name));
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
}
