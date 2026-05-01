using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class RelationshipFieldTargetRuleTests
{
    // CRBSH025: target type doesn't extend CrabshellDocument
    [Fact]
    public Task Target_extending_CrabshellDocument_is_valid() => RuleTest<RelationshipFieldTargetRule>.Valid("""
        class Agent : CrabshellDocument { }
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [RelationshipField(typeof(Agent))] public System.Guid AgentId { get; set; }
        }
        """);

    [Fact]
    public Task Target_not_extending_CrabshellDocument_fires() => RuleTest<RelationshipFieldTargetRule>.Invalid("""
        class NotADoc { }
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [RelationshipField(typeof(NotADoc))] public System.Guid {|CRBSH025:OwnerId|} { get; set; }
        }
        """);

    // CRBSH026: two properties pointing to the same target type
    [Fact]
    public Task Different_targets_are_valid() => RuleTest<RelationshipFieldTargetRule>.Valid("""
        class Agent : CrabshellDocument { }
        class Owner : CrabshellDocument { }
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [RelationshipField(typeof(Agent))] public System.Guid AgentId { get; set; }
            [RelationshipField(typeof(Owner))] public System.Guid OwnerId { get; set; }
        }
        """);

    [Fact]
    public Task Duplicate_target_fires_on_second() => RuleTest<RelationshipFieldTargetRule>.Invalid("""
        class Agent : CrabshellDocument { }
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [RelationshipField(typeof(Agent))] public System.Guid AgentId { get; set; }
            [RelationshipField(typeof(Agent))] public System.Guid {|CRBSH026:AgentId2|} { get; set; }
        }
        """);
}
