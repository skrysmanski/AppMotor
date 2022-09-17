// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.ComponentModel;
using System.Runtime.CompilerServices;

using AppMotor.Core.Globalization;

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// A writer that writes to a terminal's output stream - either stdout or stderr.
/// </summary>
public interface ITerminalWriter
{
    /// <summary>
    /// The <see cref="IFormatProvider"/> to use for formatting "formattable" values (like date times or numbers).
    /// </summary>
    /// <remarks>
    /// Implementers should default this to <see cref="UICulture.FormatsAndSorting"/>, unless
    /// another default value is required.
    /// </remarks>
    [PublicAPI]
    IFormatProvider Culture { get; set; }

    /// <summary>
    /// Whether colors should be supported/enabled or not. Should be <c>false</c>
    /// by default, if the environment variable "NO_COLOR" has a non empty value;
    /// see https://no-color.org/ for more details.
    /// </summary>
    [PublicAPI]
    bool EnableColors { get; set; }

    #region Write

    /// <summary>
    /// Writes the specified value to this writer's output.
    /// </summary>
    void Write([Localizable(true)] string? value);

    /// <summary>
    /// Writes the specified value to this writer's output.
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
    /// Writes the specified value to this writer's output.
    /// </summary>
    void Write<T>([Localizable(true)] T? value) where T : class, IConvertible
    {
        Write(value?.ToString(this.Culture));
    }

    /// <summary>
    /// Writes the specified value to this writer's output.
    /// </summary>
    void Write<T>([Localizable(true)] T? value) where T : struct, IConvertible
    {
        Write(value?.ToString(this.Culture));
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to this writer's output.
    /// </summary>
    [StringFormatMethod("format")]
    void Write([Localizable(true)] string format, params object?[] args)
    {
        Write(string.Format(this.Culture, format, args));
    }

    #endregion Write

    #region WriteLine

    /// <summary>
    /// Writes the specified value to this writer's output and appends a line break at the end.
    /// </summary>
    /// <remarks>
    /// This method uses <see cref="Environment.NewLine"/> as line terminator.
    /// </remarks>
    /// <remarks>
    /// While it would be nice (and possible) to make the line terminator configurable here, it's quite likely that <paramref name="value"/>
    /// may already contain line breaks. And if these line breaks are different than the ones configured, you'll end up with mixed line
    /// terminators (which may or may not be a bad thing). To avoid this problem, we always use <see cref="Environment.NewLine"/> as line
    /// terminator. First, because this is what is expected for a real terminals. And secondly, for most <c>WriteLine()/AppendLine()</c> methods
    /// the line terminator can't be configured. So they have to use some default which in most cases is <see cref="Environment.NewLine"/>
    /// - thereby resulting in consistent line terminators (as the terminal and the methods use the same line terminator by chance).
    /// </remarks>
    void WriteLine([Localizable(true)] string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Write(Environment.NewLine);
        }
        else
        {
            Write(value + Environment.NewLine);
        }
    }

    /// <summary>
    /// Writes the specified value to this writer's output and appends a line break at the end.
    /// </summary>
    /// <remarks>
    /// For details on the line terminator, see <see cref="WriteLine(string?)"/>.
    /// </remarks>
    void WriteLine([Localizable(true)] object? value)
    {
        if (value is IConvertible convertible)
        {
            WriteLine(convertible.ToString(this.Culture));
        }
        else
        {
            WriteLine(value?.ToString());
        }
    }

    /// <summary>
    /// Writes the specified value to this writer's output and appends a line break at the end.
    /// </summary>
    /// <remarks>
    /// For details on the line terminator, see <see cref="WriteLine(string?)"/>.
    /// </remarks>
    void WriteLine<T>([Localizable(true)] T? value) where T : class, IConvertible
    {
        WriteLine(value?.ToString(this.Culture));
    }

    /// <summary>
    /// Writes the specified value to this writer's output and appends a line break at the end.
    /// </summary>
    /// <remarks>
    /// For details on the line terminator, see <see cref="WriteLine(string?)"/>.
    /// </remarks>
    void WriteLine<T>([Localizable(true)] T? value) where T : struct, IConvertible
    {
        WriteLine(value?.ToString(this.Culture));
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/>, writes the result
    /// to this writer's output, and appends a line break at the end.
    /// </summary>
    /// <remarks>
    /// For details on the line terminator, see <see cref="WriteLine(string?)"/>.
    /// </remarks>
    [StringFormatMethod("format")]
    void WriteLine([Localizable(true)] string format, params object?[] args)
    {
        WriteLine(string.Format(this.Culture, format, args));
    }

    /// <summary>
    /// Writes a line break to this writer's output.
    /// </summary>
    /// <remarks>
    /// For details on the line terminator, see <see cref="WriteLine(string?)"/>.
    /// </remarks>
    void WriteLine()
    {
        WriteLine(null);
    }

    #endregion WriteLine
}

/// <summary>
/// Extension methods for <see cref="ITerminalWriter"/>.
/// </summary>
/// <remarks>
/// The primary goal of this class it to give users of concrete implementations of <see cref="ITerminalWriter"/>
/// access to the default interface implementations (without the need to cast to the interface first).
/// </remarks>
public static class ITerminalWriterExtensions
{
    #region Write

    /// <summary>
    /// Writes the specified value to this writer's output.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this ITerminalWriter terminalWriter, [Localizable(true)] object? value)
    {
        terminalWriter.Write(value);
    }

    /// <summary>
    /// Writes the specified value to this writer's output.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(this ITerminalWriter terminalWriter, [Localizable(true)] T? value) where T : class, IConvertible
    {
        terminalWriter.Write(value);
    }

    /// <summary>
    /// Writes the specified value to this writer's output.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(this ITerminalWriter terminalWriter, [Localizable(true)] T? value) where T : struct, IConvertible
    {
        terminalWriter.Write(value);
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to this writer's output.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [StringFormatMethod("format")]
    public static void Write(this ITerminalWriter terminalWriter, [Localizable(true)] string format, params object?[] args)
    {
        terminalWriter.Write(format, args);
    }

    #endregion Write

    #region WriteLine

    /// <summary>
    /// Writes the specified value to this writer's output and appends a line break at the end.
    /// </summary>
    /// <remarks>
    /// For details on the line terminator, see <see cref="ITerminalWriter.WriteLine(string?)"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(this ITerminalWriter terminalWriter, [Localizable(true)] string? value)
    {
        terminalWriter.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified value to this writer's output and appends a line break at the end.
    /// </summary>
    /// <remarks>
    /// For details on the line terminator, see <see cref="ITerminalWriter.WriteLine(string?)"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(this ITerminalWriter terminalWriter, [Localizable(true)] object? value)
    {
        terminalWriter.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified value to this writer's output and appends a line break at the end.
    /// </summary>
    /// <remarks>
    /// For details on the line terminator, see <see cref="ITerminalWriter.WriteLine(string?)"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine<T>(this ITerminalWriter terminalWriter, [Localizable(true)] T? value) where T : class, IConvertible
    {
        terminalWriter.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified value to this writer's output and appends a line break at the end.
    /// </summary>
    /// <remarks>
    /// For details on the line terminator, see <see cref="ITerminalWriter.WriteLine(string?)"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine<T>(this ITerminalWriter terminalWriter, [Localizable(true)] T? value) where T : struct, IConvertible
    {
        terminalWriter.WriteLine(value);
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/>, writes the result
    /// to this writer's output, and appends a line break at the end.
    /// </summary>
    /// <remarks>
    /// For details on the line terminator, see <see cref="ITerminalWriter.WriteLine(string?)"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [StringFormatMethod("format")]
    public static void WriteLine(this ITerminalWriter terminalWriter, [Localizable(true)] string format, params object?[] args)
    {
        terminalWriter.WriteLine(format, args);
    }

    /// <summary>
    /// Writes a line break to this writer's output.
    /// </summary>
    /// <remarks>
    /// For details on the line terminator, see <see cref="ITerminalWriter.WriteLine(string?)"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(this ITerminalWriter terminalWriter)
    {
        terminalWriter.WriteLine();
    }

    #endregion WriteLine
}
