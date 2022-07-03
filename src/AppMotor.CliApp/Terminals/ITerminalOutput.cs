// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.ComponentModel;

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// Represents the output part of a terminal. For the input side, see <see cref="ITerminalInput"/>.
/// </summary>
/// <see cref="ITerminal"/>
public interface ITerminalOutput
{
    /// <summary>
    /// The standard out stream.
    /// </summary>
    [PublicAPI]
    ITerminalWriter Out { get; }

    /// <summary>
    /// Whether <see cref="Out"/> is redirected (to a file or the input of another process). If <c>false</c>,
    /// this means that <see cref="Out"/> writes to the "physical" console/terminal.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="ITerminalInput.IsInputRedirected"/>, the value of this property should not have
    /// any effects on the properties/methods in this interface.
    /// </remarks>
    /// <remarks>
    /// Note to implementers: If <see cref="Out"/> doesn't represent the "physical" console/terminal, you
    /// should return <c>true</c> here.
    /// </remarks>
    [PublicAPI]
    bool IsOutputRedirected { get; }

    /// <summary>
    /// The standard error output stream.
    /// </summary>
    [PublicAPI]
    ITerminalWriter Error { get; }

    /// <summary>
    /// Whether <see cref="Error"/> is redirected (to a file or the input of another process). If <c>false</c>,
    /// this means that <see cref="Error"/> writes to the "physical" console/terminal.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="ITerminalInput.IsInputRedirected"/>, the value of this property should not have
    /// any effects on the properties/methods in this interface.
    /// </remarks>
    /// <remarks>
    /// Note to implementers: If <see cref="Error"/> doesn't represent the "physical" console/terminal, you
    /// should return <c>true</c> here.
    /// </remarks>
    [PublicAPI]
    bool IsErrorRedirected { get; }

    /// <summary>
    /// Writes the specified object to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    void Write([Localizable(true)] object? value)
    {
        this.Out.Write(value);
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
        this.Out.Write(format, args);
    }

    /// <summary>
    /// Writes the specified object to the terminal's standard output
    /// and appends a line break at the end.
    /// </summary>
    [PublicAPI]
    void WriteLine([Localizable(true)] object? value)
    {
        this.Out.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified string to the terminal's standard output
    /// and appends a line break at the end.
    /// </summary>
    [PublicAPI]
    void WriteLine([Localizable(true)] string? value)
    {
        this.Out.WriteLine(value);
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to the terminal's standard output and appends a line break at the end.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    void WriteLine([Localizable(true)] string format, params object[] args)
    {
        this.Out.WriteLine(format, args);
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
