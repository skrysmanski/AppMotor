// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Utils;

/// <summary>
/// Provides a method to convert an arbitrary value into a string.
/// </summary>
/// <remarks>
/// This interface is basically a simplified version of <see cref="ICustomFormatter"/>.
/// </remarks>
public interface IValueFormatter
{
    /// <summary>
    /// Converts the specified value into a string. The exact format of the
    /// return value is up to the implementation.
    ///
    /// <para>It's up to the implementation what to return if <paramref name="value"/>
    /// is <c>null</c>. Both <c>null</c> and empty string are allowed.</para>
    /// </summary>
    /// <remarks>
    /// This method is generic so that boxing can be avoid for value types.
    /// </remarks>
    string? FormatValue<T>(T value);
}