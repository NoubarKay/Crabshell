using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class FieldGroupRuleTests
{
    // CRBSH021: FieldGroup without field attribute
    [Fact]
    public Task FieldGroup_with_TextField_is_valid() => RuleTest<FieldGroupRule>.Valid("""
        class C { [FieldGroup("Details")] [TextField] public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task No_FieldGroup_is_valid() => RuleTest<FieldGroupRule>.Valid("""
        class C { [TextField] public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task FieldGroup_without_field_attr_fires() => RuleTest<FieldGroupRule>.Invalid("""
        class C { [FieldGroup("Details")] public string {|CRBSH021:Name|} { get; set; } = ""; }
        """);

    // CRBSH022: same group name with conflicting Sidebar values
    [Fact]
    public Task Consistent_Sidebar_is_valid() => RuleTest<FieldGroupRule>.Valid("""
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [FieldGroup("Details", Sidebar = true)] [TextField] public string Name { get; set; } = "";
            [FieldGroup("Details", Sidebar = true)] [NumberField] public int Count { get; set; }
        }
        """);

    [Fact]
    public Task Conflicting_Sidebar_fires() => RuleTest<FieldGroupRule>.Invalid("""
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [FieldGroup("Details")] [TextField] public string Name { get; set; } = "";
            [FieldGroup("Details", Sidebar = true)] [NumberField] public int {|CRBSH022:Count|} { get; set; }
        }
        """);
}
