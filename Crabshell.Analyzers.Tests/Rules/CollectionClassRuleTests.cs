using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class CollectionClassRuleTests
{
    // CRBSH023: [Collection] class not extending CrabshellDocument
    [Fact]
    public Task Collection_extending_CrabshellDocument_is_valid() => RuleTest<CollectionClassRule>.Valid("""
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [TextField] public string Name { get; set; } = "";
        }
        """);

    [Fact]
    public Task Non_collection_class_is_valid() => RuleTest<CollectionClassRule>.Valid("""
        class NotADoc { public string Name { get; set; } = ""; }
        """);

    [Fact]
    public Task Collection_without_CrabshellDocument_fires() => RuleTest<CollectionClassRule>.Invalid("""
        [Collection("docs")]
        class {|CRBSH023:Doc|}
        {
            [TextField] public string Name { get; set; } = "";
        }
        """);

    // CRBSH024: public property with no field attribute in a collection class
    [Fact]
    public Task All_mapped_properties_is_valid() => RuleTest<CollectionClassRule>.Valid("""
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [TextField] public string Name { get; set; } = "";
            [NumberField] public int Count { get; set; }
        }
        """);

    [Fact]
    public Task Unmapped_public_property_fires() => RuleTest<CollectionClassRule>.Invalid("""
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [TextField] public string Name { get; set; } = "";
            public int {|CRBSH024:UnmappedCount|} { get; set; }
        }
        """);

    [Fact]
    public Task Private_property_without_attribute_is_valid() => RuleTest<CollectionClassRule>.Valid("""
        [Collection("docs")]
        class Doc : CrabshellDocument
        {
            [TextField] public string Name { get; set; } = "";
            private int Internal { get; set; }
        }
        """);
}
