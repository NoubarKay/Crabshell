using Crabshell.Analyzers.Rules;
using Crabshell.Analyzers.Tests.Infrastructure;
using Xunit;

namespace Crabshell.Analyzers.Tests.Rules;

public class SingleConflictRuleTests
{
    [Fact]
    public Task Single_alone_is_valid() => RuleTest<SingleConflictRule>.Valid("""
        [Single("site_settings")]
        public class SiteSettings : CrabshellDocument { }
        """);

    [Fact]
    public Task Collection_alone_is_valid() => RuleTest<SingleConflictRule>.Valid("""
        [Collection("articles")]
        public class Article : CrabshellDocument { }
        """);

    [Fact]
    public Task Single_and_Collection_together_is_error() => RuleTest<SingleConflictRule>.Invalid("""
        [Single("site_settings"), Collection("site_settings")]
        public class {|CRBSH027:SiteSettings|} : CrabshellDocument { }
        """);
}
