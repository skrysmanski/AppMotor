// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Exceptions;

/// <summary>
/// Equivalent to <see cref="ArgumentNullException"/> - just for values that are not arguments/parameters.
/// </summary>
[PublicAPI]
public class ValueNullException : ValueException
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public ValueNullException()
        : this(message: null, valueName: null, innerException: null)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ValueNullException(string? message)
        : this(message: message, valueName: null, innerException: null)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="valueName">The name of the value this exception applies to.</param>
    public ValueNullException(string? message, string? valueName)
        : this(message: message, valueName: valueName, innerException: null)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The inner exception</param>
    public ValueNullException(string? message, Exception? innerException)
        : this(message: message, valueName: null, innerException: innerException)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="valueName">The name of the value this exception applies to.</param>
    /// <param name="innerException">The inner exception</param>
    public ValueNullException(string? message, string? valueName, Exception? innerException)
        : base(message ?? Validate.ExceptionMessages.VALUE_IS_NULL, valueName, innerException)
    {
    }
}
