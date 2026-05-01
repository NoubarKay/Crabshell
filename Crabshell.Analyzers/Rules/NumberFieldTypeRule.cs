using System.Collections.Generic;
using System.Collections.Immutable;
using Crabshell.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NumberFieldTypeRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH003",
        title: "NumberField on non-numeric property",
        messageFormat: "[NumberField] can only be applied to numeric properties, but '{0}' is {1}",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    
    private static readonly HashSet<SpecialType> NumericTypes =
    [
        SpecialType.System_Byte,
        SpecialType.System_SByte,
        SpecialType.System_Int16,
        SpecialType.System_UInt16,
        SpecialType.System_Int32,
        SpecialType.System_UInt32,
        SpecialType.System_Int64,
        SpecialType.System_UInt64,
        SpecialType.System_Single,
        SpecialType.System_Double,
        SpecialType.System_Decimal,
    ];

    
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
        if (!prop.HasAttribute("NumberFieldAttribute")) return;

        var type = prop.Type;
        
        if (type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable)
            type = nullable.TypeArguments[0];

        if (NumericTypes.Contains(type.SpecialType)) return;
        
        context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name, prop.Type.Name));
    }
}