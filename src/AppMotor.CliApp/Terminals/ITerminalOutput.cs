// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.ComponentModel;

using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// Represents the output part of a terminal. For the input part, see <see cref="ITerminalInput"/>.
/// </summary>
public interface ITerminalOutput
{
    /// <summary>
    /// The standard out stream.
    /// </summary>
    [PublicAPI]
    ITerminalWriter Out { get; }

    /// <summary>
    /// Whether <see cref="Out"/> is redirected (to a file or the input
    /// of another process).
    /// </summary>
    [PublicAPI]
    bool IsOutputRedirected { get; }

    /// <summary>
    /// The standard error output stream.
    /// </summary>
    [PublicAPI]
    ITerminalWriter Error { get; }

    /// <summary>
    /// Whether <see cref="Error"/> is redirected (to a file or the input
    /// of another process).
    /// </summary>
    [PublicAPI]
    bool IsErrorRedirected { get; }

    /// <summary>
    /// Writes the specified object to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    void Write([Localizable(true)] object? value)
    {
        this.Out.Write(value?.ToString());
    }

    /// <summary>
    /// Writes the specified string to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    void Write([Localizable(true)] string? value)
    {
        this.Out.Write(value);
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    void Write([Localizable(true)] string format, params object[] args)
    {
        Write(format.With(args));
    }

    /// <summary>
    /// Writes the specified object to the terminal's standard output
    /// and appends a line break at the end.
    /// </summary>
    [PublicAPI]
    void WriteLine([Localizable(true)] object? value)
    {
        WriteLine(value?.ToString());
    }

    /// <summary>
    /// Writes the specified string to the terminal's standard output
    /// and appends a line break at the end.
    /// </summary>
    [PublicAPI]
    void WriteLine([Localizable(true)] string? value)
    {
        this.Out.Write(value);
        this.Out.WriteLine();
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to the terminal's standard output and appends a line break at the end.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    void WriteLine([Localizable(true)] string format, params object[] args)
    {
        WriteLine(format.With(args));
    }

    /// <summary>
    /// Writes a line break to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    void WriteLine()
    {
        this.Out.WriteLine();
    }
}
