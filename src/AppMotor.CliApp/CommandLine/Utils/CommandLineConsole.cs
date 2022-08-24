// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.CommandLine;
using System.CommandLine.IO;
using System.Diagnostics.CodeAnalysis;

using AppMotor.CliApp.Terminals;

namespace AppMotor.CliApp.CommandLine.Utils;

internal sealed class CommandLineConsole : IConsole
{
    private readonly ITerminal _terminal;

    /// <inheritdoc />
    public IStandardStreamWriter Out { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public bool IsOutputRedirected => this._terminal.IsOutputRedirected;

    /// <inheritdoc />
    public IStandardStreamWriter Error { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public bool IsErrorRedirected => this._terminal.IsErrorRedirected;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public bool IsInputRedirected => this._terminal.IsInputRedirected;

    private CommandLineConsole(ITerminal terminal)
    {
        this._terminal = terminal;

        this.Out = StandardStreamWriter.Create(terminal.Out);
        this.Error = StandardStreamWriter.Create(terminal.Error);
    }

    /// <summary>
    /// Returns the <see cref="IConsole"/> instance for <paramref name="terminal"/>.
    /// </summary>
    public static IConsole? FromTerminal(ITerminal terminal)
    {
        if (ReferenceEquals(terminal, Terminal.Instance))
        {
            // IMPORTANT: We must return "null" here so that we can get properly aligned
            //   help texts. Unfortunately, alignment is only supported if "IConsole"
            //   is "null".
            //   See: https://github.com/dotnet/command-line-api/issues/1174#issuecomment-770774549
            return null;
        }
        else
        {
            return new CommandLineConsole(terminal);
        }
    }

}