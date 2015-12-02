using System;
using Shouldly;

namespace Baseline.Testing
{
    public static class Assert
    {
        public static void IsTrue(bool value)
        {
            value.ShouldBeTrue();
        }

        public static void IsFalse(bool value)
        {
            value.ShouldBeFalse();
        }

        public static void Fail(string message)
        {
            throw new Exception(message);
        }
    }
}