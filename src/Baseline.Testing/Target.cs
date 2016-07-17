using System;
using System.Collections.Generic;

namespace Baseline.Testing
{
    public enum Colors
    {
        Red,
        Blue,
        Green
    }

    public class Target
    {
        private static readonly Random _random = new Random(67);

        private static readonly string[] _strings =
        {
            "Red", "Orange", "Yellow", "Green", "Blue", "Purple", "Violet",
            "Pink", "Gray", "Black"
        };

        private static readonly string[] _otherStrings =
        {
            "one", "two", "three", "four", "five", "six", "seven", "eight",
            "nine", "ten"
        };

        public static IEnumerable<Target> GenerateRandomData(int number)
        {
            var i = 0;
            while (i < number)
            {
                yield return Random(true);

                i++;
            }
        }

        public static Target Random(bool deep = false)
        {
            var target = new Target();
            target.String = _strings[_random.Next(0, 10)];
            target.AnotherString = _otherStrings[_random.Next(0, 10)];
            target.Number = _random.Next();

            target.Float = float.Parse(_random.NextDouble().ToString());

            target.NumberArray = new[] { _random.Next(0, 10), _random.Next(0, 10), _random.Next(0, 10) };

            switch (_random.Next(0, 2))
            {
                case 0:
                    target.Color = Colors.Blue;
                    break;

                case 1:
                    target.Color = Colors.Green;
                    break;

                default:
                    target.Color = Colors.Red;
                    break;
            }

            target.Long = 100 * _random.Next();
            target.Double = _random.NextDouble();
            target.Long = _random.Next() * 10000;

            target.Date = DateTime.Today.AddDays(_random.Next(-10000, 10000)).ToUniversalTime();


            return target;
        }

        public Target()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public int Number { get; set; }
        public long Long { get; set; }
        public string String { get; set; }
        public string AnotherString { get; set; }

        public Guid OtherGuid { get; set; }

        public Child Inner { get; set; }

        public Child InnerField { get; set; }

        public Colors Color { get; set; }

        public bool Flag { get; set; }

        public string StringField;

        public double Double { get; set; }
        public decimal Decimal { get; set; }
        public DateTime Date { get; set; }
        public DateTimeOffset DateOffset { get; set; }

        public float Float;

        public int[] NumberArray { get; set; }


        public Child[] Children { get; set; }

        public int? NullableNumber { get; set; }
        public DateTime? NullableDateTime { get; set; }
        public bool? NullableBoolean { get; set; }
    }

    public class Child
    {
        public string Name;
        public Colors Color;

        public GrandChild GrandChild;
    }

    public class GrandChild
    {
        public string Name;
        public int Age;
    }

}