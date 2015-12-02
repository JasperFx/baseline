using System;
using System.Globalization;
using System.IO;
using Rhino.Mocks;
using Shouldly;
using Xunit;

namespace Baseline.Testing
{
    public class StringExtensionsFacter
    {

        [Fact]
        public void parent_directory()
        {
            var path = ".".ToFullPath();
            path.ParentDirectory().ShouldBe(Path.GetDirectoryName(path));
        }

        [Fact]
        public void parent_directory_ending_with_directory_seperator()
        {
            var path = ".".ToFullPath();
            (path + Path.DirectorySeparatorChar).ParentDirectory().ShouldBe(Path.GetDirectoryName(path));
        }

        [Fact]
        public void if_not_null_positive()
        {
            var action = MockRepository.GenerateMock<Action<string>>();

            "a".IfNotNull(action);

            action.AssertWasCalled(x => x.Invoke("a"));
        }

        [Fact]
        public void if_not_null_negative()
        {
            var action = MockRepository.GenerateMock<Action<string>>();
            string a = null;

            a.IfNotNull(action);

            action.AssertWasNotCalled(x => x.Invoke(null), x => x.IgnoreArguments());
        }

        [Fact]
        public void combine_to_path_when_rooted()
        {
            var rooted = Path.Combine(Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory), "here");
            rooted.CombineToPath("there").ShouldBe(rooted);
        }

        [Fact]
        public void combine_to_path_when_not_rooted()
        {
            "here".CombineToPath("there").ShouldBe(Path.Combine("there", "here"));
        }

        [Fact]
        public void is_empty()
        {
            string.Empty.IsEmpty().ShouldBeTrue();

            string nullString = null;
            nullString.IsEmpty().ShouldBeTrue();
        
            " ".IsEmpty().ShouldBeFalse();
            "something".IsEmpty().ShouldBeFalse();
        }

        [Fact]
        public void is_not_empty()
        {
            string.Empty.IsNotEmpty().ShouldBeFalse();

            string nullString = null;
            nullString.IsNotEmpty().ShouldBeFalse();

            " ".IsNotEmpty().ShouldBeTrue();
            "something".IsNotEmpty().ShouldBeTrue();
        }


        [Fact]
        public void converting_plain_text_line_returns_to_line_breaks()
        {
            const string plainText = "before\nmiddle\r\nafter";

            var textWithBreaks = plainText.ConvertCRLFToBreaks();

            textWithBreaks.ShouldBe(@"before<br/>middle<br/>after");
        }

        [Fact]
        public void numbers_with_commas_and_periods_should_be_valid()
        {
            var numbers = new[]
                              {
                                  "1,000",
                                  "100.1",
                                  "1000.1",
                                  "1,000.1",
                                  "10,000.1",
                                  "100,000.1",
                              };

            numbers.Each(x => x.IsValidNumber(CultureInfo.CreateSpecificCulture("en-us")).ShouldBeTrue());
          
        }

        [Fact]
        public void numbers_with_commas_and_periods_should_be_valid_in_european_culture()
        {
            var numbers = new[]
                              {
                                  "1.000",
                                  "100,1",
                                  "1000,1",
                                  "1.000,1",
                                  "10.000,1",
                                  "100.000,1",
                              };

            numbers.Each(x => x.IsValidNumber(CultureInfo.CreateSpecificCulture("de-DE")).ShouldBeTrue());
          
        }

        [Fact]
        public void numbers_should_be_invalid()
        {
            var numbers = new[]
                              {
                                  "1,00",
                                  "100,1",
                                  "100,1.01",
                                  "A,Jun.K",
                              };

            numbers.Each(x => x.IsValidNumber(CultureInfo.CreateSpecificCulture("en-us")).ShouldBeFalse());
        }

        [Fact]
        public void to_bool()
        {
            "true".ToBool().ShouldBeTrue();
            "True".ToBool().ShouldBeTrue();
        
            "false".ToBool().ShouldBeFalse();
            "False".ToBool().ShouldBeFalse();
        
            "".ToBool().ShouldBeFalse();

            string nullString = null;
            nullString.ToBool().ShouldBeFalse();
        }

        [Fact]
        public void to_format()
        {
            "My name is {0} and I was born in {1}, {2}".ToFormat("Jeremy", "Carthage", "Missouri")
                .ShouldBe("My name is Jeremy and I was born in Carthage, Missouri");
        }

        [Fact]
        public void read_lines_to_an_enumerable()
        {
            var text = @"a
b
c
d
e
";

            text.ReadLines().ShouldHaveTheSameElementsAs("a", "b", "c", "d", "e");
            
        }

        [Fact]
        public void read_lines_to_an_action()
        {
            var action = MockRepository.GenerateMock<Action<string>>();

            var text = @"a
b
c
d
e
";


            text.ReadLines(action);

            action.AssertWasCalled(x => x.Invoke("a"));
            action.AssertWasCalled(x => x.Invoke("b"));
            action.AssertWasCalled(x => x.Invoke("c"));
            action.AssertWasCalled(x => x.Invoke("d"));
            action.AssertWasCalled(x => x.Invoke("e"));

            
        }

        [Fact]
        public void to_hash_is_repeatable()
        {
            "something".ToHash().ShouldBe("something".ToHash());
            "else".ToHash().ShouldNotBe("something".ToHash());
        }


        [Fact]
        public void to_enum()
        {
            var x = "Machine".ToEnum<EnvironmentVariableTarget>();
            x.ShouldBe(EnvironmentVariableTarget.Machine);
        }

        [Fact]
        public void to_enum_should_ignore_case()
        {
            var x = "machine".ToEnum<EnvironmentVariableTarget>();
            x.ShouldBe(EnvironmentVariableTarget.Machine);
        }

        [Fact]
        public void to_enum_should_throw_if_not_enum()
        {
            Exception<ArgumentException>.ShouldBeThrownBy(() =>
            {
                "a".ToEnum<NotAnEnum>();
            });


        }

        public struct NotAnEnum
        {
             
        }

        [Fact]
        public void should_split_on_camel_case()
        {
            "camelCaseString".SplitCamelCase().ShouldBe("camel Case String");
        }

        [Fact]
        public void should_split_on_pascal_case()
        {
            "PascalCaseString".SplitPascalCase().ShouldBe("Pascal Case String");
        }

        [Fact]
        public void file_escape()
        {
            "my file".FileEscape().ShouldBe("\"my file\"");
        }
    }
}