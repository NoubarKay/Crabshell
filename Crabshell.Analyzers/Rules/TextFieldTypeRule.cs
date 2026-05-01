using System.Collections.Immutable;
using Crabshell.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TextFieldTypeRule : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH001",
        title: "TextField on non-string property",
        messageFormat: "[TextField] can only be applied to string properties, but '{0}' is {1}",
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

    public static void Analyze(SymbolAnalysisContext context)
    {
        var prop = (IPropertySymbol)context.Symbol;
        if (!prop.HasAttribute("TextFieldAttribute")) return;
        if (prop.Type.SpecialType == SpecialType.System_String) return;
        
        context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name, prop.Type.Name));
    }
}