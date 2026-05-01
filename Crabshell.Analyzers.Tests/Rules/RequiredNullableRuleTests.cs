using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class RequiredNullableRuleTests
{
    [Fact]
    public Task NonNullable_string_with_Required_is_valid() => RuleTest<RequiredNullableRule>.Valid("""
        class C { [TextField(Required = true)] public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task NonNullable_int_with_Required_is_valid() => RuleTest<RequiredNullableRule>.Valid("""
        class C { [NumberField(Required = true)] public int Count { get; set; } }
        """);

    [Fact]
    public Task Nullable_string_without_Required_is_valid() => RuleTest<RequiredNullableRule>.Valid("""
        class C { [TextField] public string? Name { get; set; } }
        """);

    [Fact]
    public Task No_attribute_is_valid() => RuleTest<RequiredNullableRule>.Valid("""
        class C { public string? Name { get; set; } }
        """);

    [Fact]
    public Task Nullable_string_with_Required_fires() => RuleTest<RequiredNullableRule>.Invalid("""
        class C { [TextField(Required = true)] public string? {|CRBSH006:Name|} { get; set; } }
        """);

    [Fact]
    public Task Nullable_int_with_Required_fires() => RuleTest<RequiredNullableRule>.Invalid("""
        class C { [NumberField(Required = true)] public int? {|CRBSH006:Count|} { get; set; } }
        """);

    [Fact]
    public Task Nullable_Guid_with_Required_fires() => RuleTest<RequiredNullableRule>.Invalid("""
        class Doc : CrabshellDocument { }
        class C { [RelationshipField(typeof(Doc), Required = true)] public System.Guid? {|CRBSH006:OwnerId|} { get; set; } }
        """);
}
