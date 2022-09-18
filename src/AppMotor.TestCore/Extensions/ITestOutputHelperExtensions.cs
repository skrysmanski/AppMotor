// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

using Xunit.Abstractions;

namespace AppMotor.TestCore.Extensions;

/// <summary>
/// Extension methods for <see cref="ITestOutputHelper"/>
/// </summary>
public static class ITestOutputHelperExtensions
{
    /// <summary>
    /// Writes a blank line.
    /// </summary>
    [PublicAPI]
    public static void WriteLine(this ITestOutputHelper testOutputHelper)
    {
        testOutputHelper.WriteLine("");
    }
}
