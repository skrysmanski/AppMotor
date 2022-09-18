// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.ComponentModel;

using JetBrains.Annotations;

namespace AppMotor.Core.Exceptions;

/// <summary>
/// An exception whose message can/should be displayed to the user - but not the stacktrace.
/// </summary>
public class ErrorMessageException : Exception
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message to display to the user.</param>
    [PublicAPI]
    public ErrorMessageException([Localizable(true)] string message)
        : base(message)
    {
    }

    /// <summary>
    /// Only writes the message; no type nor stacktrace.
    /// </summary>
    public override string ToString()
    {
        return this.Message;
    }
}
