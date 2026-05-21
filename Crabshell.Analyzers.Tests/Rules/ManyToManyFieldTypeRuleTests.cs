using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class ManyToManyFieldTypeRuleTests
{
    [Fact]
    public Task List_of_Guid_is_valid() => RuleTest<ManyToManyFieldTypeRule>.Valid("""
        class Doc : CrabshellDocument { }
        class C { [ManyToManyField(typeof(Doc))] public System.Collections.Generic.List<System.Guid> Ids { get; set; } = new(); }
        """);

    [Fact]
    public Task ICollection_of_Guid_is_valid() => RuleTest<ManyToManyFieldTypeRule>.Valid("""
        class Doc : CrabshellDocument { }
        class C { [ManyToManyField(typeof(Doc))] public System.Collections.Generic.ICollection<System.Guid> Ids { get; set; } = new System.Collections.Generic.List<System.Guid>(); }
        """);

    [Fact]
    public Task Guid_array_is_valid() => RuleTest<ManyToManyFieldTypeRule>.Valid("""
        class Doc : CrabshellDocument { }
        class C { [ManyToManyField(typeof(Doc))] public System.Guid[] Ids { get; set; } = System.Array.Empty<System.Guid>(); }
        """);

    [Fact]
    public Task No_attribute_is_valid() => RuleTest<ManyToManyFieldTypeRule>.Valid("""
        class C { public System.Collections.Generic.List<System.Guid> Ids { get; set; } = new(); }
        """);

    [Fact]
    public Task Single_Guid_fires() => RuleTest<ManyToManyFieldTypeRule>.Invalid("""
        class Doc : CrabshellDocument { }
        class C { [ManyToManyField(typeof(Doc))] public System.Guid {|CRBSH028:Ids|} { get; set; } }
        """);

    [Fact]
    public Task List_of_int_fires() => RuleTest<ManyToManyFieldTypeRule>.Invalid("""
        class Doc : CrabshellDocument { }
        class C { [ManyToManyField(typeof(Doc))] public System.Collections.Generic.List<int> {|CRBSH028:Ids|} { get; set; } = new(); }
        """);

    [Fact]
    public Task String_fires() => RuleTest<ManyToManyFieldTypeRule>.Invalid("""
        class Doc : CrabshellDocument { }
        class C { [ManyToManyField(typeof(Doc))] public string {|CRBSH028:Ids|} { get; set; } = ""; }
        """);
}
