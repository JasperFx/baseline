using System;
using Baseline.Conversion;
using Shouldly;
using Xunit;

namespace Baseline.Testing.Conversion
{
    public class ConversionsTester
    {
        public enum Directions
        {
            North,
            South,
            East,
            West
        }

        private readonly Conversions conversions = new Conversions();

        private void assertRoundTrip<T>(T value)
        {
            var s = value.ToString();
            conversions.Convert(typeof (T), s)
                .ShouldBe(value);
        }


        [Fact]
        public void intrinsic_types()
        {
            assertRoundTrip(true);
            assertRoundTrip<byte>(5);
            assertRoundTrip<sbyte>(6);
            assertRoundTrip('b');
            assertRoundTrip(12.34M);
            assertRoundTrip(34.56);
            assertRoundTrip<short>(24);
            assertRoundTrip<ushort>(25);
            assertRoundTrip(55);
            assertRoundTrip<uint>(56);
            assertRoundTrip<long>(55);
            assertRoundTrip<ulong>(59);
            assertRoundTrip<float>(34);
        }

        [Fact]
        public void uri()
        {
            conversions.Convert(typeof (Uri), "foo://1")
                .ShouldBeOfType<Uri>()
                .AbsoluteUri.ShouldBe("foo://1/");
        }

        [Fact]
        public void enumerations()
        {
            assertRoundTrip(UriHostNameType.Basic);
        }

        [Fact]
        public void convert_string_is_passthrough_on_value()
        {
            conversions.Convert(typeof (string), "foo").ShouldBe("foo");
        }

        [Fact]
        public void convert_string_array()
        {
            conversions.Convert(typeof (string[]), "a, b, c, d").ShouldBeOfType<string[]>()
                .ShouldHaveTheSameElementsAs("a", "b", "c", "d");
        }

        [Fact]
        public void convert_number_array()
        {
            conversions.Convert(typeof (int[]), "1,2, 3, 4").ShouldBeOfType<int[]>()
                .ShouldHaveTheSameElementsAs(1, 2, 3, 4
                );
        }

        [Fact]
        public void convert_string_as_EMPTY()
        {
            conversions.Convert(typeof (string), "EMPTY").ShouldBe(string.Empty);
        }

        [Fact]
        public void can_round_trip_enumerable_values()
        {
            assertRoundTrip(Directions.North);
        }

        [Fact]
        public void nullable_conversion()
        {
            conversions.Convert(typeof (int?), "NULL")
                .ShouldBeNull();

            conversions.Convert(typeof (int?), "123")
                .ShouldBe(123);
        }

        // SAMPLE: string-ctor-in-action
        [Fact]
        public void string_ctor_conversion()
        {
            conversions.Convert(typeof (Color), "Red")
                .ShouldBeOfType<Color>()
                .Name.ShouldBe("Red");
        }

        // ENDSAMPLE
    }

    // SAMPLE: creating-color-by-ctor
    public class Color
    {
        public Color(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    // ENDSAMPLE
}