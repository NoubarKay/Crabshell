using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class NumberFieldMinMaxRuleTests
{
    [Fact]
    public Task Min_less_than_Max_is_valid() => RuleTest<NumberFieldMinMaxRule>.Valid("""
        class C { [NumberField(Min = 0, Max = 100)] public int Count { get; set; } }
        """);

    [Fact]
    public Task Min_equal_to_Max_is_valid() => RuleTest<NumberFieldMinMaxRule>.Valid("""
        class C { [NumberField(Min = 5, Max = 5)] public int Count { get; set; } }
        """);

    [Fact]
    public Task Only_Min_is_valid() => RuleTest<NumberFieldMinMaxRule>.Valid("""
        class C { [NumberField(Min = 0)] public int Count { get; set; } }
        """);

    [Fact]
    public Task Only_Max_is_valid() => RuleTest<NumberFieldMinMaxRule>.Valid("""
        class C { [NumberField(Max = 100)] public int Count { get; set; } }
        """);

    [Fact]
    public Task Neither_is_valid() => RuleTest<NumberFieldMinMaxRule>.Valid("""
        class C { [NumberField] public int Count { get; set; } }
        """);

    [Fact]
    public Task Min_greater_than_Max_fires() => RuleTest<NumberFieldMinMaxRule>.Invalid("""
        class C { [NumberField(Min = 100, Max = 0)] public int {|CRBSH008:Count|} { get; set; } }
        """);
}
