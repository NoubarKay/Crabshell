using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class BoolFieldTypeRuleTests
{
    [Fact]
    public Task Bool_is_valid() => RuleTest<BoolFieldTypeRule>.Valid("""
        class C { [BoolField] public bool Flag { get; set; } }
        """);

    [Fact]
    public Task No_attribute_is_valid() => RuleTest<BoolFieldTypeRule>.Valid("""
        class C { public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task Int_fires() => RuleTest<BoolFieldTypeRule>.Invalid("""
        class C { [BoolField] public int {|CRBSH002:Count|} { get; set; } }
        """);

    [Fact]
    public Task String_fires() => RuleTest<BoolFieldTypeRule>.Invalid("""
        class C { [BoolField] public string {|CRBSH002:Name|} { get; set; } = ""; }
        """);
}
