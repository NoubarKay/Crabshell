using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class GridOptionsOrphanRuleTests
{
    [Fact]
    public Task GridOptions_with_TextField_is_valid() => RuleTest<GridOptionsOrphanRule>.Valid("""
        class C { [GridOptions(Visible = true)] [TextField] public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task GridOptions_with_NumberField_is_valid() => RuleTest<GridOptionsOrphanRule>.Valid("""
        class C { [GridOptions] [NumberField] public int Count { get; set; } }
        """);

    [Fact]
    public Task No_GridOptions_is_valid() => RuleTest<GridOptionsOrphanRule>.Valid("""
        class C { [TextField] public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task GridOptions_without_field_attr_fires() => RuleTest<GridOptionsOrphanRule>.Invalid("""
        class C { [GridOptions(Visible = true)] public string {|CRBSH017:Name|} { get; set; } = ""; }
        """);
}
