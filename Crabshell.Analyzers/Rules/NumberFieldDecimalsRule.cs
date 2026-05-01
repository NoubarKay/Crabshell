using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NumberFieldDecimalsRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH009",
        title: "NumberField Decimals on integer property",
        messageFormat: "[NumberField] on '{0}' specifies Decimals = {1} but '{2}' is an integer type and cannot store decimals",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly ImmutableHashSet<SpecialType> IntegerTypes = ImmutableHashSet.Create(
        SpecialType.System_Byte,
        SpecialType.System_SByte,
        SpecialType.System_Int16,
        SpecialType.System_UInt16,
        SpecialType.System_Int32,
        SpecialType.System_UInt32,
        SpecialType.System_Int64,
        SpecialType.System_UInt64);

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

        var decimalsArg = attr.NamedArguments.FirstOrDefault(a => a.Key == "Decimals");
        if (decimalsArg.Key == null) return;

        var decimals = (int)decimalsArg.Value.Value!;
        if (decimals <= 0) return;

        // Unwrap Nullable<T>
        var type = prop.Type;
        if (type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable)
            type = nullable.TypeArguments[0];

        if (!IntegerTypes.Contains(type.SpecialType)) return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name, decimals, type.Name));
    }
}