using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class GridOptionsOrderRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor RuleNoOrder = new(
        id: "CRBSH018",
        title: "Visible GridOptions column has no explicit Order",
        messageFormat: "'{0}' is visible in the grid but has no explicit Order — column position may be unpredictable",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor RuleDuplicateOrder = new(
        id: "CRBSH019",
        title: "Duplicate GridOptions Order value",
        messageFormat: "'{0}' shares Order = {1} with another visible grid column in this collection",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [RuleNoOrder, RuleDuplicateOrder];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        var type = (INamedTypeSymbol)context.Symbol;

        // Only analyse collection document types (have [Collection] attribute)
        if (!type.GetAttributes().Any(a => a.AttributeClass?.Name == "CollectionAttribute")) return;

        var orderCounts = new Dictionary<int, List<IPropertySymbol>>();

        foreach (var prop in type.GetMembers().OfType<IPropertySymbol>())
        {
            var attr = prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "GridOptionsAttribute");
            if (attr == null) continue;

            if (!IsVisible(attr)) continue;

            var hasExplicitOrder = attr.NamedArguments.Any(a => a.Key == "Order");

            if (!hasExplicitOrder)
            {
                context.ReportDiagnostic(Diagnostic.Create(RuleNoOrder, prop.Locations[0], prop.Name));
                continue; // don't also check duplicate order since Order isn't set
            }

            var order = (int)attr.NamedArguments.First(a => a.Key == "Order").Value.Value!;

            if (!orderCounts.TryGetValue(order, out var list))
                orderCounts[order] = list = new List<IPropertySymbol>();

            list.Add(prop);
        }

        foreach (var kvp in orderCounts)
        {
            if (kvp.Value.Count <= 1) continue;
            foreach (var prop in kvp.Value)
                context.ReportDiagnostic(Diagnostic.Create(RuleDuplicateOrder, prop.Locations[0], prop.Name, kvp.Key));
        }
    }

    private static bool IsVisible(AttributeData attr)
    {
        var arg = attr.NamedArguments.FirstOrDefault(a => a.Key == "Visible");
        return arg.Key == null || arg.Value.Value is true;
    }
}