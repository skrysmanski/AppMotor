// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Utils;
using AppMotor.TestCore.Logging;

using Xunit.Abstractions;

namespace AppMotor.TestCore;

/// <summary>
/// Base class for test classes.
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// The "test console" - i.e. a replacement for <see cref="Console"/> to use in test.
    /// </summary>
    /// <remarks>
    /// The reason why you should use this property over <see cref="Console"/> is that tests run in parallel.
    /// Thus, it's impossible for the test runner to determine which console output belongs to which test.
    /// With this property, this problem is fixed.
    /// </remarks>
    /// <seealso cref="XUnitLoggerProviderExtensions.AddXUnitLogger"/>
    protected ITestOutputHelper TestConsole { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <remarks>
    /// Simply implement a constructor with the same signature in your test class and XUnit will automatically
    /// provide you with an instance of <see cref="ITestOutputHelper"/>.
    /// </remarks>
    protected TestBase(ITestOutputHelper testOutputHelper)
    {
        Validate.ArgumentWithName(nameof(testOutputHelper)).IsNotNull(testOutputHelper);

        this.TestConsole = testOutputHelper;
    }
}