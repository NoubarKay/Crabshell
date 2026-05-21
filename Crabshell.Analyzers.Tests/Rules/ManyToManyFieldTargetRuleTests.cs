using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class ManyToManyFieldTargetRuleTests
{
    // CRBSH029: target type doesn't extend CrabshellDocument
    [Fact]
    public Task Target_extending_CrabshellDocument_is_valid() => RuleTest<ManyToManyFieldTargetRule>.Valid("""
        class Tag : CrabshellDocument { }
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [ManyToManyField(typeof(Tag))] public System.Collections.Generic.List<System.Guid> TagIds { get; set; } = new();
        }
        """);

    [Fact]
    public Task Target_not_extending_CrabshellDocument_fires() => RuleTest<ManyToManyFieldTargetRule>.Invalid("""
        class NotADoc { }
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [ManyToManyField(typeof(NotADoc))] public System.Collections.Generic.List<System.Guid> {|CRBSH029:TagIds|} { get; set; } = new();
        }
        """);

    // CRBSH030: two properties pointing to the same target type
    [Fact]
    public Task Different_targets_are_valid() => RuleTest<ManyToManyFieldTargetRule>.Valid("""
        class Tag : CrabshellDocument { }
        class Author : CrabshellDocument { }
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [ManyToManyField(typeof(Tag))] public System.Collections.Generic.List<System.Guid> TagIds { get; set; } = new();
            [ManyToManyField(typeof(Author))] public System.Collections.Generic.List<System.Guid> AuthorIds { get; set; } = new();
        }
        """);

    [Fact]
    public Task Duplicate_target_fires_on_second() => RuleTest<ManyToManyFieldTargetRule>.Invalid("""
        class Tag : CrabshellDocument { }
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [ManyToManyField(typeof(Tag))] public System.Collections.Generic.List<System.Guid> TagIds { get; set; } = new();
            [ManyToManyField(typeof(Tag))] public System.Collections.Generic.List<System.Guid> {|CRBSH030:MoreTagIds|} { get; set; } = new();
        }
        """);
}
