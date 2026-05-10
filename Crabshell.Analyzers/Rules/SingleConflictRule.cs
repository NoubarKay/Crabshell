using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SingleConflictRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH027",
        title: "[Single] and [Collection] cannot both be applied to the same class",
        messageFormat: "'{0}' has both [Single] and [Collection] attributes — a type can only be one or the other",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        var type = (INamedTypeSymbol)context.Symbol;

        var hasSingle     = type.GetAttributes().Any(a => a.AttributeClass?.Name == "SingleAttribute");
        var hasCollection = type.GetAttributes().Any(a => a.AttributeClass?.Name == "CollectionAttribute");

        if (hasSingle && hasCollection)
            context.ReportDiagnostic(Diagnostic.Create(Rule, type.Locations[0], type.Name));
    }
}
