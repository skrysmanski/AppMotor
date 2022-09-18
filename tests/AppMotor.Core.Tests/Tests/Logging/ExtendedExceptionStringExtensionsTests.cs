// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Logging;
using AppMotor.Core.TestUtils;
using AppMotor.TestCore;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace AppMotor.Core.Tests.Logging;

public sealed class ExtendedExceptionStringExtensionsTests : TestBase
{
    public ExtendedExceptionStringExtensionsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void TestSimpleException()
    {
        // setup
        var exceptionMessage = $"Some error text - {Guid.NewGuid()}";
        var exception = ExceptionCreator<InvalidOperationException>.CreateAndCatch(exceptionMessage);

        // test
        var extendedString = exception.ToStringExtended();

        // just for manual verification of the output
        this.TestConsole.WriteLine(extendedString);

        // verify
        extendedString.ShouldContain(exceptionMessage);
        extendedString.ShouldContain("CreateAndCatch");
        extendedString.ShouldContain(exception.GetType().FullName!);
    }

    [Fact]
    public void TestExceptionWithInnerException()
    {
        // setup
        var innerExceptionMessage = $"Some error text - {Guid.NewGuid()}";
        var innerException = ExceptionCreator<InvalidOperationException>.CreateAndCatch(innerExceptionMessage);
        var outerExceptionMessage = $"Some error text - {Guid.NewGuid()}";
        var outerException = ExceptionCreator<ArgumentException>.CreateAndCatch(outerExceptionMessage, innerException);

        // test
        var extendedString = outerException.ToStringExtended();

        // just for manual verification of the output
        this.TestConsole.WriteLine(extendedString);

        // verify
        extendedString.ShouldContain(innerExceptionMessage);
        extendedString.ShouldContain(outerExceptionMessage);
        extendedString.ShouldContain(innerException.GetType().FullName!);
        extendedString.ShouldContain(outerException.GetType().FullName!);
    }

    [Fact]
    public void TestAggregateException()
    {
        // setup
        var messageGuid1 = Guid.NewGuid();
        var messageGuid2 = Guid.NewGuid();

        AggregateException exception;

        try
        {
            ThrowAggregateException(messageGuid1, messageGuid2);
            throw new Exception("We should not get here.");
        }
        catch (AggregateException ex)
        {
            exception = ex;
        }

        // test
        var extendedString = exception.ToStringExtended();

        // just for manual verification of the output
        this.TestConsole.WriteLine(extendedString);

        // verify
        extendedString.ShouldContain(messageGuid1.ToString());
        extendedString.ShouldContain(messageGuid2.ToString());
        extendedString.ShouldContain(typeof(AggregateException).FullName!);
        extendedString.ShouldContain(typeof(InvalidOperationException).FullName!);
        extendedString.ShouldContain(typeof(ArgumentException).FullName!);
    }

    private static void ThrowAggregateException(Guid messageGuid1, Guid messageGuid2)
    {
        var task1 = Task.Run(
            async () =>
            {
                await Task.Delay(10);
                var exceptionMessage = $"Some error text - {messageGuid1}";
                ExceptionCreator<InvalidOperationException>.CreateAndThrow(exceptionMessage);
            }
        );

        var task2 = Task.Run(
            async () =>
            {
                await Task.Delay(10);
                var exceptionMessage = $"Some error text - {messageGuid2}";
                ExceptionCreator<ArgumentException>.CreateAndThrow(exceptionMessage);
            }
        );

        Task.WaitAll(task1, task2);
    }
}
