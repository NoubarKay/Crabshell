using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class TextFieldMinMaxLengthRuleTests
{
    [Fact]
    public Task MinLength_less_than_MaxLength_is_valid() => RuleTest<TextFieldMinMaxLengthRule>.Valid("""
        class C { [TextField(MinLength = 5, MaxLength = 200)] public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task MinLength_with_unlimited_MaxLength_is_valid() => RuleTest<TextFieldMinMaxLengthRule>.Valid("""
        class C { [TextField(MinLength = 100, MaxLength = -1)] public string Body { get; set; } = ""; }
        """);

    [Fact]
    public Task Only_MinLength_is_valid() => RuleTest<TextFieldMinMaxLengthRule>.Valid("""
        class C { [TextField(MinLength = 5)] public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task Only_MaxLength_is_valid() => RuleTest<TextFieldMinMaxLengthRule>.Valid("""
        class C { [TextField(MaxLength = 200)] public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task MinLength_greater_than_MaxLength_fires() => RuleTest<TextFieldMinMaxLengthRule>.Invalid("""
        class C { [TextField(MinLength = 200, MaxLength = 10)] public string {|CRBSH010:Name|} { get; set; } = ""; }
        """);
}
