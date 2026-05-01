using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class SelectFieldDuplicateOptionsRuleTests
{
    [Fact]
    public Task Unique_Options_are_valid() => RuleTest<SelectFieldDuplicateOptionsRule>.Valid("""
        class C { [SelectField(Options = new[] { "A", "B", "C" })] public string Value { get; set; } = ""; }
        """);

    [Fact]
    public Task Empty_Options_is_valid() => RuleTest<SelectFieldDuplicateOptionsRule>.Valid("""
        class C { [SelectField] public string Value { get; set; } = ""; }
        """);

    [Fact]
    public Task Duplicate_Option_fires() => RuleTest<SelectFieldDuplicateOptionsRule>.Invalid("""
        class C { [SelectField(Options = new[] { "A", "B", "A" })] public string {|CRBSH016:Value|} { get; set; } = ""; }
        """);
}
