using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class DateTimeFieldMinMaxRuleTests
{
    [Fact]
    public Task Min_before_Max_is_valid() => RuleTest<DateTimeFieldMinMaxRule>.Valid("""
        class C { [DateTimeField(Min = "2020-01-01", Max = "2025-12-31")] public System.DateTime? Date { get; set; } }
        """);

    [Fact]
    public Task Only_Min_is_valid() => RuleTest<DateTimeFieldMinMaxRule>.Valid("""
        class C { [DateTimeField(Min = "2020-01-01")] public System.DateTime? Date { get; set; } }
        """);

    [Fact]
    public Task Only_Max_is_valid() => RuleTest<DateTimeFieldMinMaxRule>.Valid("""
        class C { [DateTimeField(Max = "2025-12-31")] public System.DateTime? Date { get; set; } }
        """);

    [Fact]
    public Task No_MinMax_is_valid() => RuleTest<DateTimeFieldMinMaxRule>.Valid("""
        class C { [DateTimeField] public System.DateTime? Date { get; set; } }
        """);

    // CRBSH013: Min after Max
    [Fact]
    public Task Min_after_Max_fires() => RuleTest<DateTimeFieldMinMaxRule>.Invalid("""
        class C { [DateTimeField(Min = "2025-01-01", Max = "2020-01-01")] public System.DateTime? {|CRBSH013:Date|} { get; set; } }
        """);

    // CRBSH014: Invalid date format
    [Fact]
    public Task Invalid_Min_format_fires() => RuleTest<DateTimeFieldMinMaxRule>.Invalid("""
        class C { [DateTimeField(Min = "01/01/2020")] public System.DateTime? {|CRBSH014:Date|} { get; set; } }
        """);

    [Fact]
    public Task Invalid_Max_format_fires() => RuleTest<DateTimeFieldMinMaxRule>.Invalid("""
        class C { [DateTimeField(Max = "not-a-date")] public System.DateTime? {|CRBSH014:Date|} { get; set; } }
        """);
}
