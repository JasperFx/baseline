using Shouldly;
using Xunit;

namespace Baseline.Testing
{
    
    public class UrlExtensionsTester
    {
        [Fact]
        public void append_url()
        {
            "foo".AppendUrl("bar").ShouldBe("foo/bar");
            "foo".AppendUrl("/bar").ShouldBe("foo/bar");
            "foo/".AppendUrl("bar").ShouldBe("foo/bar");
            "foo/".AppendUrl("/bar").ShouldBe("foo/bar");
        }

        [Fact]
        public void child_url()
        {
            "foo/bar".ChildUrl().ShouldBe("bar");
            "foo/bar/more".ChildUrl().ShouldBe("bar/more");
        }

        [Fact]
        public void parent_url()
        {
            "foo/bar".ParentUrl().ShouldBe("foo");
            "foo/bar/more".ParentUrl().ShouldBe("foo/bar");
        }

        [Fact]
        public void move_up()
        {
            "foo/bar".MoveUp().ShouldBe("bar");
            "foo/bar/more".MoveUp().ShouldBe("bar/more");

            "foo".MoveUp().ShouldBe("");
            "".MoveUp().ShouldBe("");
        }
    }
}