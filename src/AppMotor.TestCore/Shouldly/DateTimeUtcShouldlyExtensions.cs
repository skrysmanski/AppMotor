// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.DateAndTime;

using JetBrains.Annotations;

using Shouldly;

namespace AppMotor.TestCore.Shouldly;

/// <summary>
/// <see cref="Should"/>ly extensions for <see cref="DateTimeUtc"/>.
/// </summary>
[ShouldlyMethods]
public static class DateTimeUtcShouldlyExtensions
{
    /// <inheritdoc cref="ShouldBeTestExtensions.ShouldBe(DateTime,DateTime,TimeSpan,string)"/>
    [PublicAPI]
    public static void ShouldBe(this DateTimeUtc actual, DateTimeUtc expected, TimeSpan tolerance, string? customMessage = null)
    {
        actual.ToDateTime().ShouldBe(expected.ToDateTime(), tolerance, customMessage);
    }

    /// <inheritdoc cref="ShouldBeTestExtensions.ShouldNotBe(DateTime,DateTime,TimeSpan,string)"/>
    [PublicAPI]
    public static void ShouldNotBe(this DateTimeUtc actual, DateTimeUtc expected, TimeSpan tolerance, string? customMessage = null)
    {
        actual.ToDateTime().ShouldNotBe(expected.ToDateTime(), tolerance, customMessage);
    }
}
