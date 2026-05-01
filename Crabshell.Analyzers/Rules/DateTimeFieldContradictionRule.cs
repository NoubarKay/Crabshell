using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DateTimeFieldContradictionRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH012",
        title: "DateTimeField has contradictory options",
        messageFormat: "[DateTimeField] on '{0}' has TimeOnly = true but HasTime = false — TimeOnly requires a time component",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Error,
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

        var attr = prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "DateTimeFieldAttribute");
        if (attr == null) return;

        var args = attr.NamedArguments;

        var timeOnly = args.FirstOrDefault(a => a.Key == "TimeOnly").Value.Value is true;
        var hasTime = args.FirstOrDefault(a => a.Key == "HasTime").Value.Value;

        // TimeOnly = true with HasTime explicitly set to false is contradictory
        if (timeOnly && hasTime is false)
            context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name));
    }
}