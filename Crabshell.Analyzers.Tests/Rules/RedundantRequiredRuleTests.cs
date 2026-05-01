using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class RedundantRequiredRuleTests
{
    // Value types without Required=true on a field attr → fires (always implicitly required)
    [Fact]
    public Task Int_without_Required_fires() => RuleTest<RedundantRequiredRule>.Invalid("""
        class C { [NumberField] public int {|CRBSH007:Count|} { get; set; } }
        """);

    [Fact]
    public Task Bool_without_Required_fires() => RuleTest<RedundantRequiredRule>.Invalid("""
        class C { [BoolField] public bool {|CRBSH007:Flag|} { get; set; } }
        """);

    [Fact]
    public Task Int_with_Required_false_fires() => RuleTest<RedundantRequiredRule>.Invalid("""
        class C { [NumberField(Required = false)] public int {|CRBSH007:Count|} { get; set; } }
        """);

    // Required=true is set explicitly → no fire
    [Fact]
    public Task Int_with_Required_true_is_valid() => RuleTest<RedundantRequiredRule>.Valid("""
        class C { [NumberField(Required = true)] public int Count { get; set; } }
        """);

    // Nullable value type → not subject to this rule
    [Fact]
    public Task Nullable_int_is_valid() => RuleTest<RedundantRequiredRule>.Valid("""
        class C { [NumberField] public int? Count { get; set; } }
        """);

    // Reference type → not a value type, doesn't apply
    [Fact]
    public Task String_is_valid() => RuleTest<RedundantRequiredRule>.Valid("""
        class C { [TextField] public string Name { get; set; } = ""; }
        """);

    // No field attribute → doesn't apply
    [Fact]
    public Task No_attribute_is_valid() => RuleTest<RedundantRequiredRule>.Valid("""
        class C { public int Count { get; set; } }
        """);
}
