using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Crabshell.Analyzers.Tests.Infrastructure;

public static class RuleTest<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
{
    // Minimal stubs matching the real Crabshell.Core attribute shapes.
    // The analyzers match by attribute class name, so these stubs are sufficient.
    internal const string Stubs = """
        #nullable enable
        using System;

        [AttributeUsage(AttributeTargets.Property, Inherited = true)]
        public abstract class CrabshellFieldAttribute : Attribute
        {
            public string? ColumnName { get; set; }
            public bool Required { get; set; }
            public string? Label { get; set; }
        }

        [AttributeUsage(AttributeTargets.Property)]
        public sealed class TextFieldAttribute : CrabshellFieldAttribute
        {
            public int MaxLength { get; set; } = 255;
            public int MinLength { get; set; } = 0;
            public string? Pattern { get; set; }
        }

        [AttributeUsage(AttributeTargets.Property)]
        public sealed class NumberFieldAttribute : CrabshellFieldAttribute
        {
            public int Decimals { get; set; } = 2;
            public double Min { get; set; } = double.NaN;
            public double Max { get; set; } = double.NaN;
            public string Step { get; set; } = "1";
        }

        [AttributeUsage(AttributeTargets.Property)]
        public sealed class BoolFieldAttribute : CrabshellFieldAttribute
        {
            public bool IsSwitch { get; set; }
        }

        [AttributeUsage(AttributeTargets.Property)]
        public sealed class DateTimeFieldAttribute : CrabshellFieldAttribute
        {
            public bool HasTime { get; set; } = true;
            public bool TimeOnly { get; set; } = false;
            public string? Min { get; set; }
            public string? Max { get; set; }
        }

        [AttributeUsage(AttributeTargets.Property)]
        public sealed class RelationshipFieldAttribute : CrabshellFieldAttribute
        {
            public Type RelatesTo { get; }
            public RelationshipFieldAttribute(Type relatesTo) => RelatesTo = relatesTo;
        }

        [AttributeUsage(AttributeTargets.Property)]
        public sealed class SelectFieldAttribute : CrabshellFieldAttribute
        {
            public string[] Options { get; set; } = Array.Empty<string>();
            public bool Multiple { get; set; } = false;
        }

        [AttributeUsage(AttributeTargets.Property)]
        public sealed class GridOptionsAttribute : Attribute
        {
            public bool Visible { get; set; } = true;
            public string? Label { get; set; }
            public int Width { get; set; } = 0;
            public bool Sortable { get; set; } = true;
            public bool Filterable { get; set; } = false;
            public int Order { get; set; } = 0;
        }

        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
        public sealed class FieldGroupAttribute : Attribute
        {
            public string Name { get; }
            public bool Sidebar { get; set; } = false;
            public FieldGroupAttribute(string name) => Name = name;
        }

        [AttributeUsage(AttributeTargets.Class)]
        public sealed class CollectionAttribute : Attribute
        {
            public string Name { get; }
            public string? Label { get; set; }
            public CollectionAttribute(string name) => Name = name;
        }

        public abstract class CrabshellDocument
        {
            public Guid Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        """;

    public static Task Valid(string code) =>
        new CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
        {
            TestCode = Stubs + code,
        }.RunAsync();

    public static Task Invalid(string code) =>
        new CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
        {
            TestCode = Stubs + code,
        }.RunAsync();
}
