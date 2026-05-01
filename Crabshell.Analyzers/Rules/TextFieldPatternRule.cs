using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TextFieldPatternRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH011",
        title: "TextField Pattern is an invalid regex",
        messageFormat: "[TextField] on '{0}' has an invalid Pattern: {1}",
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

        var patternArg = attr.NamedArguments.FirstOrDefault(a => a.Key == "Pattern");
        if (patternArg.Key == null) return;

        var pattern = patternArg.Value.Value as string;
        if (string.IsNullOrEmpty(pattern)) return;

        try
        {
            _ = new Regex(pattern, RegexOptions.None, TimeSpan.FromSeconds(1));
        }
        catch (ArgumentException ex)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name, ex.Message));
        }
    }
}