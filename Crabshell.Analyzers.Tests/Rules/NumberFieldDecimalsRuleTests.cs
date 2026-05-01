using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class NumberFieldDecimalsRuleTests
{
    [Fact]
    public Task Decimal_with_Decimals_is_valid() => RuleTest<NumberFieldDecimalsRule>.Valid("""
        class C { [NumberField(Decimals = 2)] public decimal Price { get; set; } }
        """);

    [Fact]
    public Task Int_with_zero_Decimals_is_valid() => RuleTest<NumberFieldDecimalsRule>.Valid("""
        class C { [NumberField(Decimals = 0)] public int Count { get; set; } }
        """);

    [Fact]
    public Task Int_without_Decimals_is_valid() => RuleTest<NumberFieldDecimalsRule>.Valid("""
        class C { [NumberField] public int Count { get; set; } }
        """);

    [Fact]
    public Task Int_with_Decimals_fires() => RuleTest<NumberFieldDecimalsRule>.Invalid("""
        class C { [NumberField(Decimals = 2)] public int {|CRBSH009:Count|} { get; set; } }
        """);

    [Fact]
    public Task Nullable_int_with_Decimals_fires() => RuleTest<NumberFieldDecimalsRule>.Invalid("""
        class C { [NumberField(Decimals = 2)] public int? {|CRBSH009:Count|} { get; set; } }
        """);

    [Fact]
    public Task Long_with_Decimals_fires() => RuleTest<NumberFieldDecimalsRule>.Invalid("""
        class C { [NumberField(Decimals = 2)] public long {|CRBSH009:BigCount|} { get; set; } }
        """);
}
