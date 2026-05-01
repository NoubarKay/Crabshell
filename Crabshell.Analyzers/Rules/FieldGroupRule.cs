using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FieldGroupRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor RuleOrphan = new(
        id: "CRBSH021",
        title: "FieldGroup without a field attribute",
        messageFormat: "'{0}' has [FieldGroup] but no field attribute — FieldGroup has no effect without a field attribute",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor RuleSidebarConflict = new(
        id: "CRBSH022",
        title: "FieldGroup Sidebar value is inconsistent",
        messageFormat: "'{0}' uses group \"{1}\" with Sidebar = {2}, but this group is declared with Sidebar = {3} elsewhere in the collection",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [RuleOrphan, RuleSidebarConflict];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    // CRBSH021 — property level
    private static void AnalyzeProperty(SymbolAnalysisContext context)
    {
        var prop = (IPropertySymbol)context.Symbol;

        var hasFieldGroup = prop.GetAttributes().Any(a => a.AttributeClass?.Name == "FieldGroupAttribute");
        if (!hasFieldGroup) return;

        var hasFieldAttribute = prop.GetAttributes().Any(a => InheritsFromCrabshellFieldAttribute(a.AttributeClass));
        if (!hasFieldAttribute)
            context.ReportDiagnostic(Diagnostic.Create(RuleOrphan, prop.Locations[0], prop.Name));
    }

    // CRBSH022 — type level
    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        var type = (INamedTypeSymbol)context.Symbol;

        if (!type.GetAttributes().Any(a => a.AttributeClass?.Name == "CollectionAttribute")) return;

        // group name → (first sidebar value, first property that declared it)
        var groupSidebar = new Dictionary<string, (bool Sidebar, IPropertySymbol Prop)>(System.StringComparer.Ordinal);

        foreach (var prop in type.GetMembers().OfType<IPropertySymbol>())
        {
            var groupAttr = prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "FieldGroupAttribute");
            if (groupAttr == null) continue;

            var name = groupAttr.ConstructorArguments.FirstOrDefault().Value as string;
            if (string.IsNullOrEmpty(name)) continue;

            var sidebarArg = groupAttr.NamedArguments.FirstOrDefault(a => a.Key == "Sidebar");
            var sidebar = sidebarArg.Key != null && sidebarArg.Value.Value is true;

            if (!groupSidebar.TryGetValue(name, out var first))
            {
                groupSidebar[name] = (sidebar, prop);
            }
            else if (first.Sidebar != sidebar)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    RuleSidebarConflict, prop.Locations[0],
                    prop.Name, name, sidebar, first.Sidebar));
            }
        }
    }

    private static bool InheritsFromCrabshellFieldAttribute(INamedTypeSymbol? type)
    {
        while (type != null)
        {
            if (type.Name == "CrabshellFieldAttribute") return true;
            type = type.BaseType;
        }
        return false;
    }
}
