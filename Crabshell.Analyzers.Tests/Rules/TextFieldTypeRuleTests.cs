using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class TextFieldTypeRuleTests
{
    [Fact]
    public Task String_is_valid() => RuleTest<TextFieldTypeRule>.Valid("""
        class C { [TextField] public string Title { get; set; } = ""; }
        """);

    [Fact]
    public Task Nullable_string_is_valid() => RuleTest<TextFieldTypeRule>.Valid("""
        class C { [TextField] public string? Title { get; set; } }
        """);

    [Fact]
    public Task No_attribute_is_valid() => RuleTest<TextFieldTypeRule>.Valid("""
        class C { public int Count { get; set; } }
        """);

    [Fact]
    public Task Int_fires() => RuleTest<TextFieldTypeRule>.Invalid("""
        class C { [TextField] public int {|CRBSH001:Count|} { get; set; } }
        """);

    [Fact]
    public Task Bool_fires() => RuleTest<TextFieldTypeRule>.Invalid("""
        class C { [TextField] public bool {|CRBSH001:Flag|} { get; set; } }
        """);
}
