using System.Collections.Immutable;
using System.Linq;
using Crabshell.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NumberFieldMinMaxRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH008",
        title: "NumberField Min is greater than Max",
        messageFormat: "[NumberField] on '{0}' has Min ({1}) greater than Max ({2})",
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

        var attr = prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "NumberFieldAttribute");
        if (attr == null) return;

        var args = attr.NamedArguments;
        var minArg = args.FirstOrDefault(a => a.Key == "Min");
        var maxArg = args.FirstOrDefault(a => a.Key == "Max");

        if (minArg.Key == null || maxArg.Key == null) return;

        var min = (double)minArg.Value.Value!;
        var max = (double)maxArg.Value.Value!;

        if (double.IsNaN(min) || double.IsNaN(max)) return;
        if (min <= max) return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name, min, max));
    }
}