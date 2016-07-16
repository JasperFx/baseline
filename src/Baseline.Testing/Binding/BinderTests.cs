using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using Baseline.Conversion;
using Baseline.Binding;
using Shouldly;

namespace Baseline.Testing.Binding
{
    public class BinderTests
    {
        readonly Binder<Target> binder = new Binder<Target>();
        readonly Dictionary<string, string> theData = new Dictionary<string, string>();
        private DictionaryDataSource theSource;

        public BinderTests()
        {
            theSource = new DictionaryDataSource(new Dictionary<string, string>());
        }


        [Fact]
        public void can_build_a_binder()
        {
            binder.ShouldNotBeNull();
        }

        [Fact]
        public void can_bind_string_property()
        {
            theData.Add(nameof(Target.String), "somebody");

            binder.Build(theSource)
                .String.ShouldBe("somebody");
        }
    }
}