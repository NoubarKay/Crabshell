using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SelectFieldDuplicateOptionsRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH016",
        title: "SelectField Options contains duplicate entries",
        messageFormat: "[SelectField] on '{0}' has duplicate option \"{1}\"",
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

        var attr = prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "SelectFieldAttribute");
        if (attr == null) return;

        var optionsArg = attr.NamedArguments.FirstOrDefault(a => a.Key == "Options");
        if (optionsArg.Key == null) return;

        var values = optionsArg.Value.Values
            .Select(v => v.Value as string)
            .Where(v => v != null)
            .ToList();

        var seen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);
        foreach (var value in values)
        {
            if (!seen.Add(value!))
                context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name, value));
        }
    }
}