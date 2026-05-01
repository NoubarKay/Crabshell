using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class SelectFieldEnumOptionsRuleTests
{
    [Fact]
    public Task Enum_without_Options_is_valid() => RuleTest<SelectFieldEnumOptionsRule>.Valid("""
        enum Status { Active, Inactive }
        class C { [SelectField] public Status Value { get; set; } }
        """);

    [Fact]
    public Task String_with_Options_is_valid() => RuleTest<SelectFieldEnumOptionsRule>.Valid("""
        class C { [SelectField(Options = new[] { "A", "B" })] public string Value { get; set; } = ""; }
        """);

    [Fact]
    public Task Enum_with_empty_Options_is_valid() => RuleTest<SelectFieldEnumOptionsRule>.Valid("""
        enum Status { Active, Inactive }
        class C { [SelectField(Options = new string[0])] public Status Value { get; set; } }
        """);

    [Fact]
    public Task Enum_with_Options_fires() => RuleTest<SelectFieldEnumOptionsRule>.Invalid("""
        enum Status { Active, Inactive }
        class C { [SelectField(Options = new[] { "Active" })] public Status {|CRBSH015:Value|} { get; set; } }
        """);

    [Fact]
    public Task Nullable_enum_with_Options_fires() => RuleTest<SelectFieldEnumOptionsRule>.Invalid("""
        enum Status { Active, Inactive }
        class C { [SelectField(Options = new[] { "Active" })] public Status? {|CRBSH015:Value|} { get; set; } }
        """);
}
