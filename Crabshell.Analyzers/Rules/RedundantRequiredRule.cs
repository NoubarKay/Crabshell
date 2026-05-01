using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RedundantRequiredRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH007",
        title: "Redundant Required on non-nullable value type",
        messageFormat: "'{0}' is a non-nullable value type and is always required — setting Required = true is redundant, or consider making it nullable",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Info,
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

        // Only care about non-nullable value types (bool, int, Guid, decimal, etc.)
        var isNonNullableValueType =
            prop.Type.IsValueType &&
            prop.Type is not INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T };

        if (!isNonNullableValueType) return;

        // Flag if any field attribute has Required = false or omits Required entirely
        var fieldAttr = prop.GetAttributes().FirstOrDefault(a =>
            a.AttributeClass?.Name.EndsWith("FieldAttribute") == true);

        if (fieldAttr == null) return;

        var requiredArg = fieldAttr.NamedArguments.FirstOrDefault(n => n.Key == "Required");
        // If Required is omitted or explicitly false, it's misleading on a value type
        if (requiredArg.Key == "Required" && requiredArg.Value.Value is true) return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name));
    }
}