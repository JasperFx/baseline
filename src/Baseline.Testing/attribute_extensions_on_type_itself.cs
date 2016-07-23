using System;
using Shouldly;
using Xunit;

namespace Baseline.Testing
{
    public class attribute_extensions_on_type_itself
    {
        [Fact]
        public void for_attribute_miss()
        {
            typeof(NoAttributeGuy).ForAttribute<TagAttribute>(att =>
            {
                throw new Exception("Shouldn't be called");
            });

            bool wasMissed = false;
            typeof(NoAttributeGuy).ForAttribute<TagAttribute>(att =>
            {
                throw new Exception("Shouldn't be called");
            }, () => wasMissed = true);

            wasMissed.ShouldBeTrue();
        }

        [Fact]
        public void for_attribute_hit()
        {
            string text = null;

            typeof(MyGuy).ForAttribute<TagAttribute>(att => text = att.Text);
            text.ShouldBe("Blue");

            typeof(OtherGuy).ForAttribute<TagAttribute>(att => text = att.Text, () =>
            {
                throw new Exception("Shouldn't be called");
            });

            text.ShouldBe("Green");
        }

        [Fact]
        public void has_attribute()
        {
            typeof(MyGuy).HasAttribute<TagAttribute>().ShouldBeTrue();
            typeof(MyGuy).HasAttribute<OtherAttribute>().ShouldBeFalse();
        }

        [Fact]
        public void get_attribute()
        {
            typeof(MyGuy).GetAttribute<TagAttribute>().Text.ShouldBe("Blue");
        }


        [Tag("Blue")]
        public class MyGuy
        {
            
        }

        [Tag("Green")]
        public class OtherGuy
        {

        }

        public class NoAttributeGuy { }

        public class TagAttribute : Attribute
        {
            public string Text { get; }

            public TagAttribute(string text)
            {
                Text = text;
            }
        }

        public class OtherAttribute : Attribute
        {
            public string Text { get; }

            public OtherAttribute(string text)
            {
                Text = text;
            }
        }
    }
}