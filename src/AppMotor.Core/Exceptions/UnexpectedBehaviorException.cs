// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Exceptions;

/// <summary>
/// This exception is for places that code should never reach (because of the logical control flow)
/// but where the compiler can't detect this (and thus generates a compiler error).
/// </summary>
public class UnexpectedBehaviorException : Exception
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    [PublicAPI]
    public UnexpectedBehaviorException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The inner exception.</param>
    [PublicAPI]
    public UnexpectedBehaviorException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
