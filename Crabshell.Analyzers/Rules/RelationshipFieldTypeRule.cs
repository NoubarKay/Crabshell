using System.Collections.Immutable;
using Crabshell.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RelationshipFieldTypeRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH005",
        title: "RelationshipField on non-Guid property",
        messageFormat: "[RelationshipField] can only be applied to Guid properties, but '{0}' is {1}",
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
        if (!prop.HasAttribute("RelationshipFieldAttribute")) return;
        var type = prop.Type;

        if (type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable)
            type = nullable.TypeArguments[0];

        if (type.ToDisplayString() == "System.Guid") return;
        
        context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name, prop.Type.Name));
    }
}