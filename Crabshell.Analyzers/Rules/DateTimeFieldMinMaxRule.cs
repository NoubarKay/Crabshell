using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crabshell.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DateTimeFieldMinMaxRule : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor RuleMinMaxOrder = new(
        id: "CRBSH013",
        title: "DateTimeField Min is after Max",
        messageFormat: "[DateTimeField] on '{0}' has Min \"{1}\" after Max \"{2}\"",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor RuleInvalidFormat = new(
        id: "CRBSH014",
        title: "DateTimeField Min/Max has invalid date format",
        messageFormat: "[DateTimeField] on '{0}' has an invalid date string \"{1}\" — expected format is yyyy-MM-dd",
        category: "Crabshell",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [RuleMinMaxOrder, RuleInvalidFormat];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(Analyze, SymbolKind.Property);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        var prop = (IPropertySymbol)context.Symbol;

        var attr = prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "DateTimeFieldAttribute");
        if (attr == null) return;

        var args = attr.NamedArguments;
        var minArg = args.FirstOrDefault(a => a.Key == "Min");
        var maxArg = args.FirstOrDefault(a => a.Key == "Max");

        if (minArg.Key == null && maxArg.Key == null) return;

        DateTime? min = null;
        DateTime? max = null;

        if (minArg.Key != null && minArg.Value.Value is string minStr)
        {
            if (!DateTime.TryParseExact(minStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                context.ReportDiagnostic(Diagnostic.Create(RuleInvalidFormat, prop.Locations[0], prop.Name, minStr));
                return;
            }
            min = parsed;
        }

        if (maxArg.Key != null && maxArg.Value.Value is string maxStr)
        {
            if (!DateTime.TryParseExact(maxStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                context.ReportDiagnostic(Diagnostic.Create(RuleInvalidFormat, prop.Locations[0], prop.Name, maxStr));
                return;
            }
            max = parsed;
        }

        if (min.HasValue && max.HasValue && min.Value > max.Value)
            context.ReportDiagnostic(Diagnostic.Create(RuleMinMaxOrder, prop.Locations[0], prop.Name, minArg.Value.Value, maxArg.Value.Value));
    }
}