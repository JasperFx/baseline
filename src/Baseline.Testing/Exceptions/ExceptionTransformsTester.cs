using System;
using System.Net;
using Baseline.Exceptions;
using Shouldly;
using Xunit;

namespace Baseline.Testing.Exceptions
{
    public class ExceptionTransformsTester
    {
        private readonly ExceptionTransforms theTransforms = new ExceptionTransforms();
        
        public ExceptionTransformsTester()
        {
            theTransforms.AddTransform<CustomTransform>();
            theTransforms.IfExceptionIs<InvalidOperationException>()
                .If(e => e.Message.Contains("Bad"))
                .ThenTransformTo(e => new CustomException2("It was bad", e))
                .If(e => e.Message.Contains("Wrong"))
                .ThenTransformTo(e => new WrongException("It was wrong", e))
                .IfInnerIs<WrongException>().ThenTransformTo(e => new CustomException3("It was wrong", e))
                ;
        }

        [Fact]
        public void passthrough_if_no_matches()
        {
            Should.Throw<BadImageFormatException>(() =>
            {
                theTransforms.TransformAndThrow(new BadImageFormatException());
            });
        }

        [Fact]
        public void use_inner_exception()
        {
            Should.Throw<CustomException3>(() =>
            {
                theTransforms.TransformAndThrow(new InvalidOperationException("some message",
                    new WrongException("Bad", new Exception())));
            });
        }

        [Fact]
        public void simple_transformation()
        {
            var transformed = Should.Throw<CustomException1>(() =>
            {
                theTransforms.TransformAndThrow(new DivideByZeroException());
            });
            
            transformed.Message.ShouldBe("Got it");
        }

        [Fact]
        public void match_by_message_name_and_type()
        {
            Should.Throw<CustomException2>(() =>
            {
                theTransforms.TransformAndThrow(new InvalidOperationException("It was Bad"));
            });
            
            Should.Throw<WrongException>(() =>
            {
                theTransforms.TransformAndThrow(new InvalidOperationException("It was Wrong"));
            });
        }
        
    }

    public class CustomTransform : ExceptionTransform<DivideByZeroException>
    {
        public CustomTransform()
        {
            TransformTo(i => new CustomException1("Got it", i));
        }
    }

    public class WrongException : Exception
    {
        public WrongException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class CustomException1 : Exception
    {
        public CustomException1(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    
    public class CustomException2 : Exception
    {
        public CustomException2(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    
    public class CustomException3 : Exception
    {
        public CustomException3(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}