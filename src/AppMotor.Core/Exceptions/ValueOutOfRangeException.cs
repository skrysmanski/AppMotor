// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Exceptions;

/// <summary>
/// Equivalent to <see cref="ArgumentOutOfRangeException"/> - just for values that are not arguments/parameters.
/// </summary>
[PublicAPI]
public class ValueOutOfRangeException : ValueException
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public ValueOutOfRangeException()
        : this(message: null, valueName: null, innerException: null)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ValueOutOfRangeException(string? message)
        : this(message: message, valueName: null, innerException: null)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="valueName">The name of the value this exception applies to.</param>
    public ValueOutOfRangeException(string? message, string? valueName)
        : this(message: message, valueName: valueName, innerException: null)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The inner exception</param>
    public ValueOutOfRangeException(string? message, Exception? innerException)
        : this(message: message, valueName: null, innerException: innerException)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="valueName">The name of the value this exception applies to.</param>
    /// <param name="innerException">The inner exception</param>
    public ValueOutOfRangeException(string? message, string? valueName, Exception? innerException)
        : base(message ?? "The specified value is outside of the range of valid values.", valueName, innerException)
    {
    }
}
