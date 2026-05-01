using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RequiredNullableRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH006",
        title: "Required field on nullable property",
        messageFormat: "'{0}' is marked Required but its type is nullable",
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

        var hasRequired = prop.GetAttributes().Any(a =>
            a.NamedArguments.Any(n => n.Key == "Required" && n.Value.Value is true));

        if (!hasRequired) return;

        var isNullable =
            prop.Type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T }
            || prop.Type.NullableAnnotation == NullableAnnotation.Annotated;

        if (!isNullable) return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name));
    }
}