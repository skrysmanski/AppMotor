// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.ComponentModel;

using AppMotor.Core.Globalization;

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// A writer that writes to a terminal's output stream - either stdout or stderr.
/// </summary>
public interface ITerminalWriter
{
    /// <summary>
    /// The newline characters to use.
    /// </summary>
    /// <remarks>
    /// Implementers should default this to <see cref="NewLineTypes.SystemDefault"/>, unless
    /// another default value is required.
    /// </remarks>
    NewLineTypes NewLine { get; set; }

    /// <summary>
    /// The <see cref="IFormatProvider"/> to use for formatting "formattable" values (like date times or numbers).
    /// </summary>
    /// <remarks>
    /// Implementers should default this to <see cref="UICulture.FormatsAndSorting"/>, unless
    /// another default value is required.
    /// </remarks>
    IFormatProvider Culture { get; set; }

    /// <summary>
    /// Whether colors should be supported/enabled or not. Should be <c>false</c>
    /// by default, if the environment variable "NO_COLOR" has a non empty value;
    /// see https://no-color.org/ for more details.
    /// </summary>
    bool EnableColors { get; set; }

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
    void WriteLine([Localizable(true)] string? value);

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to the terminal's standard output and appends a line break at the end.
    /// </summary>
    [StringFormatMethod("format")]
    void WriteLine([Localizable(true)] string format, params object[] args)
    {
        WriteLine(string.Format(this.Culture, format, args));
    }

    /// <summary>
    /// Writes a line break to the terminal's standard output.
    /// </summary>
    void WriteLine()
    {
        WriteLine(null);
    }
}
