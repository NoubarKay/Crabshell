using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class DateTimeFieldContradictionRuleTests
{
    [Fact]
    public Task TimeOnly_with_default_HasTime_is_valid() => RuleTest<DateTimeFieldContradictionRule>.Valid("""
        class C { [DateTimeField(TimeOnly = true)] public System.DateTime? ApptTime { get; set; } }
        """);

    [Fact]
    public Task TimeOnly_with_explicit_HasTime_true_is_valid() => RuleTest<DateTimeFieldContradictionRule>.Valid("""
        class C { [DateTimeField(TimeOnly = true, HasTime = true)] public System.DateTime? ApptTime { get; set; } }
        """);

    [Fact]
    public Task HasTime_false_without_TimeOnly_is_valid() => RuleTest<DateTimeFieldContradictionRule>.Valid("""
        class C { [DateTimeField(HasTime = false)] public System.DateTime? DateOnly { get; set; } }
        """);

    [Fact]
    public Task No_options_is_valid() => RuleTest<DateTimeFieldContradictionRule>.Valid("""
        class C { [DateTimeField] public System.DateTime? CreatedAt { get; set; } }
        """);

    [Fact]
    public Task TimeOnly_true_with_HasTime_false_fires() => RuleTest<DateTimeFieldContradictionRule>.Invalid("""
        class C { [DateTimeField(TimeOnly = true, HasTime = false)] public System.DateTime? {|CRBSH012:ApptTime|} { get; set; } }
        """);
}
