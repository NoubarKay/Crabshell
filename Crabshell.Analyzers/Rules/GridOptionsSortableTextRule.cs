using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class GridOptionsSortableTextRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "CRBSH020",
        title: "Sortable grid column on unbounded TEXT field",
        messageFormat: "'{0}' has Sortable = true but [TextField(MaxLength = -1)] maps to an unbounded TEXT column — sorting on TEXT is inefficient and cannot be indexed with a standard B-tree index",
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

        var gridAttr = prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "GridOptionsAttribute");
        if (gridAttr == null) return;

        // Sortable defaults to true, so fire if not explicitly set to false
        var sortableArg = gridAttr.NamedArguments.FirstOrDefault(a => a.Key == "Sortable");
        var sortable = sortableArg.Key == null || sortableArg.Value.Value is true;
        if (!sortable) return;

        var textAttr = prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "TextFieldAttribute");
        if (textAttr == null) return;

        var maxLengthArg = textAttr.NamedArguments.FirstOrDefault(a => a.Key == "MaxLength");
        if (maxLengthArg.Key == null) return;
        if ((int)maxLengthArg.Value.Value! != -1) return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Locations[0], prop.Name));
    }
}
