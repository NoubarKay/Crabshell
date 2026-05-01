using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class NumberFieldTypeRuleTests
{
    [Fact
    ]
    public Task Int_is_valid() => RuleTest<NumberFieldTypeRule>.Valid("""
        class C { [NumberField] public int Count { get; set; } }
        """);

    [Fact]
    public Task Nullable_int_is_valid() => RuleTest<NumberFieldTypeRule>.Valid("""
        class C { [NumberField] public int? Count { get; set; } }
        """);

    [Fact]
    public Task Decimal_is_valid() => RuleTest<NumberFieldTypeRule>.Valid("""
        class C { [NumberField] public decimal Price { get; set; } }
        """);

    [Fact]
    public Task Nullable_decimal_is_valid() => RuleTest<NumberFieldTypeRule>.Valid("""
        class C { [NumberField] public decimal? Price { get; set; } }
        """);

    [Fact]
    public Task No_attribute_is_valid() => RuleTest<NumberFieldTypeRule>.Valid("""
        class C { public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task String_fires() => RuleTest<NumberFieldTypeRule>.Invalid("""
        class C { [NumberField] public string {|CRBSH003:Name|} { get; set; } = ""; }
        """);

    [Fact]
    public Task Bool_fires() => RuleTest<NumberFieldTypeRule>.Invalid("""
        class C { [NumberField] public bool {|CRBSH003:Flag|} { get; set; } }
        """);
}
