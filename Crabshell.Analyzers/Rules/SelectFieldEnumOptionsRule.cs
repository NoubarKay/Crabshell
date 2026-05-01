using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SelectFieldEnumOptionsRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH015",
        title: "SelectField Options redundant on enum property",
        messageFormat: "[SelectField] on '{0}' specifies Options but the property is an enum — options are auto-generated from the enum values and the provided Options will be ignored",
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

        // Check Options array is non-empty
        var values = optionsArg.Value.Values;
        if (values.IsEmpty) return;

        // Unwrap Nullable<T>
        var type = prop.Type;
        if (type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable)
            type = nullable.TypeArguments[0];

        if (type.TypeKind != TypeKind.Enum) return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name));
    }
}