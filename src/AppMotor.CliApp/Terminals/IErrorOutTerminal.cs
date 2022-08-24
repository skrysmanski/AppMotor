// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// Represents the error output (i.e. analog to <see cref="Console.Error"/>) of a terminal.
/// </summary>
/// <seealso cref="ITerminal"/>
/// <seealso cref="IOutputTerminal"/>
/// <seealso cref="IStdOutTerminal"/>
public interface IErrorOutTerminal
{
    /// <summary>
    /// The standard error output stream.
    /// </summary>
    [PublicAPI]
    TextWriter Error { get; }

    /// <summary>
    /// Whether <see cref="Error"/> is redirected (to a file or the input
    /// of another process).
    /// </summary>
    [PublicAPI]
    bool IsErrorRedirected { get; }
}