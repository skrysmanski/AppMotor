// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections;
using System.Collections.ObjectModel;

using AppMotor.Core.Extensions;
using AppMotor.Core.TestUtils;
using AppMotor.TestCore;

using JetBrains.Annotations;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

using ExceptionExtensions = AppMotor.Core.Extensions.ExceptionExtensions;

namespace AppMotor.Core.Tests.Extensions;

/// <summary>
/// Tests for <see cref="ExceptionExtensions"/>.
/// </summary>
public sealed class ExceptionExtensionsTests : TestBase
{
    public ExceptionExtensionsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    #region AddData()

    [Fact]
    public void TestAddData()
    {
        // setup
        var exception = new Exception();

        // test
        exception.AddData("abc", 42);

        // verify
        exception.GetData()["abc"].ShouldBe(42);
    }

    /// <summary>
    /// Verifies that <see cref="ExceptionExtensions.AddData"/> does not
    /// throw an exception if the exception data dictionary is read-only.
    /// </summary>
    [Fact]
    public void TestAddData_ReadOnlyData()
    {
        // setup
        var exception = new ExceptionWithReadOnlyData();

        // test our assumptions
        exception.Data.IsReadOnly.ShouldBe(true);

        // tests
        exception.AddData("abc", 42); // should not do anything
        exception.GetData().Count.ShouldBe(0);
    }

    /// <summary>
    /// Tests adding a non-serializable type as exception data. This tests is just for compatibility
    /// reasons as this was a requirement in the .NET Framework - but this requirement has been
    /// dropped with .NET Core.
    /// </summary>
    [Fact]
    public void TestAddData_NonSerializableData()
    {
        // setup
        var exception = new Exception();

        // test
        exception.AddData("abc", new SomeDataClass()); // must not throw
    }

    private sealed class ExceptionWithReadOnlyData : Exception
    {
        /// <inheritdoc />
        public override IDictionary Data { get; } = new ReadOnlyDictionary<object, object>(new Dictionary<object, object>());
    }

    private sealed class SomeDataClass
    {
        [UsedImplicitly]
        public int SomeProperty { get; } = 42;
    }

    #endregion AddData()

    [Fact]
    public void TestRethrow()
    {
        // setup
        var caughtException = ExceptionCreator<MySpecialException>.CreateAndCatch();

        // test
        var rethrownException = Should.Throw<MySpecialException>(() => SomeOtherMethod(caughtException));

        // for manual verification
        this.TestConsole.WriteLine(rethrownException.ToString());

        // verify
        rethrownException.StackTrace!.ShouldContain(nameof(ExceptionCreator<MySpecialException>.CreateAndCatch));
    }

    private static void SomeOtherMethod(Exception caughtException)
    {
        throw caughtException.Rethrow();
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class MySpecialException : InvalidOperationException
    {
        /// <inheritdoc />
        public MySpecialException(string? message) : base(message)
        {
        }
    }
}