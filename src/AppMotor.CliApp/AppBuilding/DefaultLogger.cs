// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Logging;
using AppMotor.CliApp.Terminals;

using Microsoft.Extensions.Logging;

namespace AppMotor.CliApp.AppBuilding;

/// <summary>
/// The logger to configure in <see cref="DefaultHostBuilderFactory"/>.
/// </summary>
public enum DefaultLogger
{
    /// <summary>
    /// No logging is configured.
    /// </summary>
    /// <remarks>
    /// Use this to do the whole logging configuration yourself.
    /// </remarks>
    None,

    /// <summary>
    /// Use a logger that writes to <see cref="System.Console"/>.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="ConsoleLoggerExtensions.AddConsole(ILoggingBuilder)"/>.
    /// </remarks>
    Console,

    /// <summary>
    /// Use a logger that write to <see cref="ITerminal"/> (or <see cref="ITerminalOutput"/>
    /// to be more precise).
    /// </summary>
    /// <remarks>
    /// Uses <see cref="TerminalLoggerExtensions.AddTerminalLogger"/>.
    /// </remarks>
    Terminal,
}
