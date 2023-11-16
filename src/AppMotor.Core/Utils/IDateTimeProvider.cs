// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Provides the current time and date. This interface primarily exists for (unit) testing
/// code that relies on the current time and date (thus allowing test code to mock the current
/// time and date). For the default implementation, see <see cref="DefaultDateTimeProvider"/>.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// The current date and time in the local timezone (i.e. <see cref="DateTimeKind.Local"/>).
    /// </summary>
    /// <seealso cref="UtcNow"/>
    [PublicAPI]
    DateTime LocalNow { get; }

    /// <summary>
    /// The current date and time in the UTC timezone (i.e. <see cref="DateTimeKind.Utc"/>).
    /// </summary>
    /// <seealso cref="LocalNow"/>
    [PublicAPI]
    DateTimeUtc UtcNow { get; }
}
