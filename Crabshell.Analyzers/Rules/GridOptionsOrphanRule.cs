using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class GridOptionsOrphanRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH017",
        title: "GridOptions without a field attribute",
        messageFormat: "'{0}' has [GridOptions] but no field attribute — GridOptions has no effect without a field attribute",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(Analyze, SymbolKind.Property);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        var prop = (IPropertySymbol)context.Symbol;

        var hasGridOptions = prop.GetAttributes()
            .Any(a => a.AttributeClass?.Name == "GridOptionsAttribute");

        if (!hasGridOptions) return;

        var hasFieldAttribute = prop.GetAttributes()
            .Any(a => InheritsFromCrabshellFieldAttribute(a.AttributeClass));

        if (!hasFieldAttribute)
            context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name));
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