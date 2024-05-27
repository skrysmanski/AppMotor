// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.DateAndTime;

/// <summary>
/// The default implementation of <see cref="IDateTimeProvider"/>. All properties
/// are "redirected" to <see cref="DateTime"/>.
/// </summary>
public sealed class DefaultDateTimeProvider : IDateTimeProvider
{
    /// <summary>
    /// The instance of this class.
    /// </summary>
    public static DefaultDateTimeProvider Instance { get; } = new();

    /// <inheritdoc />
    public DateTime LocalNow => DateTime.Now;

    /// <inheritdoc />
    public DateTimeUtc UtcNow => DateTimeUtc.Now;
}
