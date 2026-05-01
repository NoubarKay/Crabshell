using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class RelationshipFieldTypeRuleTests
{
    [Fact]
    public Task Guid_is_valid() => RuleTest<RelationshipFieldTypeRule>.Valid("""
        class Doc : CrabshellDocument { }
        class C { [RelationshipField(typeof(Doc))] public System.Guid OwnerId { get; set; } }
        """);

    [Fact]
    public Task Nullable_Guid_is_valid() => RuleTest<RelationshipFieldTypeRule>.Valid("""
        class Doc : CrabshellDocument { }
        class C { [RelationshipField(typeof(Doc))] public System.Guid? OwnerId { get; set; } }
        """);

    [Fact]
    public Task No_attribute_is_valid() => RuleTest<RelationshipFieldTypeRule>.Valid("""
        class C { public System.Guid OwnerId { get; set; } }
        """);

    [Fact]
    public Task String_fires() => RuleTest<RelationshipFieldTypeRule>.Invalid("""
        class Doc : CrabshellDocument { }
        class C { [RelationshipField(typeof(Doc))] public string {|CRBSH005:OwnerId|} { get; set; } = ""; }
        """);

    [Fact]
    public Task Int_fires() => RuleTest<RelationshipFieldTypeRule>.Invalid("""
        class Doc : CrabshellDocument { }
        class C { [RelationshipField(typeof(Doc))] public int {|CRBSH005:OwnerId|} { get; set; } }
        """);
}
