using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class DateTimeFieldTypeRuleTests
{
    [Fact]
    public Task DateTime_is_valid() => RuleTest<DateTimeFieldTypeRule>.Valid("""
        class C { [DateTimeField] public System.DateTime CreatedAt { get; set; } }
        """);

    [Fact]
    public Task Nullable_DateTime_is_valid() => RuleTest<DateTimeFieldTypeRule>.Valid("""
        class C { [DateTimeField] public System.DateTime? CreatedAt { get; set; } }
        """);

    [Fact]
    public Task DateTimeOffset_is_valid() => RuleTest<DateTimeFieldTypeRule>.Valid("""
        class C { [DateTimeField] public System.DateTimeOffset CreatedAt { get; set; } }
        """);

    [Fact]
    public Task No_attribute_is_valid() => RuleTest<DateTimeFieldTypeRule>.Valid("""
        class C { public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task String_fires() => RuleTest<DateTimeFieldTypeRule>.Invalid("""
        class C { [DateTimeField] public string {|CRBSH004:CreatedAt|} { get; set; } = ""; }
        """);

    [Fact]
    public Task Int_fires() => RuleTest<DateTimeFieldTypeRule>.Invalid("""
        class C { [DateTimeField] public int {|CRBSH004:Timestamp|} { get; set; } }
        """);
}
