using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class GridOptionsSortableTextRuleTests
{
    [Fact]
    public Task TextField_with_bounded_MaxLength_is_valid() => RuleTest<GridOptionsSortableTextRule>.Valid("""
        class C { [GridOptions(Sortable = true)] [TextField(MaxLength = 200)] public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task TextField_unbounded_with_Sortable_false_is_valid() => RuleTest<GridOptionsSortableTextRule>.Valid("""
        class C { [GridOptions(Sortable = false)] [TextField(MaxLength = -1)] public string Body { get; set; } = ""; }
        """);

    [Fact]
    public Task NumberField_unbounded_is_valid() => RuleTest<GridOptionsSortableTextRule>.Valid("""
        class C { [GridOptions] [NumberField] public int Count { get; set; } }
        """);

    [Fact]
    public Task TextField_unbounded_with_default_Sortable_fires() => RuleTest<GridOptionsSortableTextRule>.Invalid("""
        class C { [GridOptions] [TextField(MaxLength = -1)] public string {|CRBSH020:Body|} { get; set; } = ""; }
        """);

    [Fact]
    public Task TextField_unbounded_with_explicit_Sortable_true_fires() => RuleTest<GridOptionsSortableTextRule>.Invalid("""
        class C { [GridOptions(Sortable = true)] [TextField(MaxLength = -1)] public string {|CRBSH020:Body|} { get; set; } = ""; }
        """);
}
