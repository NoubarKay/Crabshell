using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TextFieldMinMaxLengthRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH010",
        title: "TextField MinLength is greater than MaxLength",
        messageFormat: "[TextField] on '{0}' has MinLength ({1}) greater than MaxLength ({2})",
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

        var attr = prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "TextFieldAttribute");
        if (attr == null) return;

        var args = attr.NamedArguments;
        var minArg = args.FirstOrDefault(a => a.Key == "MinLength");
        var maxArg = args.FirstOrDefault(a => a.Key == "MaxLength");

        if (minArg.Key == null || maxArg.Key == null) return;

        var min = (int)minArg.Value.Value!;
        var max = (int)maxArg.Value.Value!;

        // MaxLength = -1 means unlimited, so MinLength can never exceed it
        if (max == -1) return;
        if (min <= max) return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name, min, max));
    }
}