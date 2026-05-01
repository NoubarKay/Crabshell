using System.Collections.Immutable;
using Crabshell.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BoolFieldTypeRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH002",
        title: "BoolField on non-bool property",
        messageFormat: "[BoolField] can only be applied to boolean properties, but '{0}' is {1}",
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
        if (!prop.HasAttribute("BoolFieldAttribute")) return;
        if (prop.Type.SpecialType == SpecialType.System_Boolean) return;
        
        context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name, prop.Type.Name));
    }
}