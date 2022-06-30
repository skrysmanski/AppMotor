// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.ComponentModel;
using System.Globalization;

using AppMotor.Core.Globalization;

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// A writer that writes to a terminal's output stream - either stdout or stderr.
/// </summary>
public interface ITerminalWriter
{
    string NewLine => Environment.NewLine;

    CultureInfo Culture => UICulture.FormatsAndSorting;

    /// <summary>
    /// Writes the specified object to the terminal's standard output.
    /// </summary>
    void Write<T>([Localizable(true)] T? value) where T : IConvertible
    {
        Write(value?.ToString(this.Culture));
    }

    /// <summary>
    /// Writes the specified object to the terminal's standard output.
    /// </summary>
    void Write([Localizable(true)] object? value)
    {
        if (value is IConvertible convertible)
        {
            Write(convertible.ToString(this.Culture));
        }
        else
        {
            Write(value?.ToString());
        }
    }

    /// <summary>
    /// Writes the specified string to the terminal's standard output.
    /// </summary>
    void Write([Localizable(true)] string? value);

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to the terminal's standard output.
    /// </summary>
    [StringFormatMethod("format")]
    void Write([Localizable(true)] string format, params object[] args)
    {
        Write(string.Format(this.Culture, format, args));
    }

    /// <summary>
    /// Writes the specified object to the terminal's standard output
    /// and appends a line break at the end.
    /// </summary>
    void WriteLine([Localizable(true)] object? value)
    {
        WriteLine(value?.ToString());
    }

    /// <summary>
    /// Writes the specified string to the terminal's standard output
    /// and appends a line break at the end.
    /// </summary>
    void WriteLine([Localizable(true)] string? value)
    {
        Write(value);
        WriteLine();
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to the terminal's standard output and appends a line break at the end.
    /// </summary>
    [StringFormatMethod("format")]
    void WriteLine([Localizable(true)] string format, params object[] args)
    {
        Write(format, args);
        WriteLine();
    }

    /// <summary>
    /// Writes a line break to the terminal's standard output.
    /// </summary>
    void WriteLine()
    {
        Write(this.NewLine);
    }
}
