using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class TextFieldPatternRuleTests
{
    [Fact]
    public Task Valid_regex_is_valid() => RuleTest<TextFieldPatternRule>.Valid("""
        class C { [TextField(Pattern = "^[a-zA-Z0-9]+$")] public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task No_pattern_is_valid() => RuleTest<TextFieldPatternRule>.Valid("""
        class C { [TextField] public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task Invalid_regex_fires() => RuleTest<TextFieldPatternRule>.Invalid("""
        class C { [TextField(Pattern = "[invalid")] public string {|CRBSH011:Name|} { get; set; } = ""; }
        """);

    [Fact]
    public Task Unclosed_group_fires() => RuleTest<TextFieldPatternRule>.Invalid("""
        class C { [TextField(Pattern = "(unclosed")] public string {|CRBSH011:Name|} { get; set; } = ""; }
        """);
}
