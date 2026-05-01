using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class GridOptionsOrderRuleTests
{
    // CRBSH018: visible column with no explicit Order
    [Fact]
    public Task Visible_with_Order_is_valid() => RuleTest<GridOptionsOrderRule>.Valid("""
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [GridOptions(Visible = true, Order = 0)] [TextField] public string Name { get; set; } = "";
        }
        """);

    [Fact]
    public Task Hidden_column_without_Order_is_valid() => RuleTest<GridOptionsOrderRule>.Valid("""
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [GridOptions(Visible = false)] [TextField] public string Name { get; set; } = "";
        }
        """);

    [Fact]
    public Task Visible_without_Order_fires_CRBSH018() => RuleTest<GridOptionsOrderRule>.Invalid("""
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [GridOptions(Visible = true)] [TextField] public string {|CRBSH018:Name|} { get; set; } = "";
        }
        """);

    // CRBSH019: duplicate Order values among visible columns
    [Fact]
    public Task Unique_Orders_are_valid() => RuleTest<GridOptionsOrderRule>.Valid("""
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [GridOptions(Visible = true, Order = 0)] [TextField] public string Name { get; set; } = "";
            [GridOptions(Visible = true, Order = 1)] [NumberField] public int Count { get; set; }
        }
        """);

    [Fact]
    public Task Duplicate_Orders_fire_CRBSH019() => RuleTest<GridOptionsOrderRule>.Invalid("""
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [GridOptions(Visible = true, Order = 1)] [TextField] public string {|CRBSH019:Name|} { get; set; } = "";
            [GridOptions(Visible = true, Order = 1)] [NumberField] public int {|CRBSH019:Count|} { get; set; }
        }
        """);
}
