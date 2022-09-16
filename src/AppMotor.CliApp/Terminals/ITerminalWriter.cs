// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.ComponentModel;
using System.Runtime.CompilerServices;

using AppMotor.Core.Globalization;
using AppMotor.Core.Utils;

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
    /// another default value is required/desired.
    /// </remarks>
    [PublicAPI]
    NewLineTypes NewLine { get; set; }

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
    /// Writes the specified value to this writer's output and appends a line break (see <see cref="NewLine"/>) at the end.
    /// </summary>
    void WriteLine([Localizable(true)] string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Write(this.NewLine.GetNewLineCharacters());
        }
        else
        {
            Write(value + this.NewLine.GetNewLineCharacters());
        }
    }

    /// <summary>
    /// Writes the specified value to this writer's output and appends a line break (see <see cref="NewLine"/>) at the end.
    /// </summary>
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
    /// Writes the specified value to this writer's output and appends a line break (see <see cref="NewLine"/>) at the end.
    /// </summary>
    void WriteLine<T>([Localizable(true)] T? value) where T : class, IConvertible
    {
        WriteLine(value?.ToString(this.Culture));
    }

    /// <summary>
    /// Writes the specified value to this writer's output and appends a line break (see <see cref="NewLine"/>) at the end.
    /// </summary>
    void WriteLine<T>([Localizable(true)] T? value) where T : struct, IConvertible
    {
        WriteLine(value?.ToString(this.Culture));
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/>, writes the result
    /// to this writer's output, and appends a line break (see <see cref="NewLine"/>) at the end.
    /// </summary>
    [StringFormatMethod("format")]
    void WriteLine([Localizable(true)] string format, params object?[] args)
    {
        WriteLine(string.Format(this.Culture, format, args));
    }

    /// <summary>
    /// Writes a line break (see <see cref="NewLine"/>) to this writer's output.
    /// </summary>
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
    /// Writes the specified value to this writer's output and appends a line break (see <see cref="ITerminalWriter.NewLine"/>) at the end.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(this ITerminalWriter terminalWriter, [Localizable(true)] string? value)
    {
        terminalWriter.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified value to this writer's output and appends a line break (see <see cref="ITerminalWriter.NewLine"/>) at the end.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(this ITerminalWriter terminalWriter, [Localizable(true)] object? value)
    {
        terminalWriter.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified value to this writer's output and appends a line break (see <see cref="ITerminalWriter.NewLine"/>) at the end.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine<T>(this ITerminalWriter terminalWriter, [Localizable(true)] T? value) where T : class, IConvertible
    {
        terminalWriter.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified value to this writer's output and appends a line break (see <see cref="ITerminalWriter.NewLine"/>) at the end.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine<T>(this ITerminalWriter terminalWriter, [Localizable(true)] T? value) where T : struct, IConvertible
    {
        terminalWriter.WriteLine(value);
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/>, writes the result
    /// to this writer's output, and appends a line break (see <see cref="ITerminalWriter.NewLine"/>) at the end.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [StringFormatMethod("format")]
    public static void WriteLine(this ITerminalWriter terminalWriter, [Localizable(true)] string format, params object?[] args)
    {
        terminalWriter.WriteLine(format, args);
    }

    /// <summary>
    /// Writes a line break (see <see cref="ITerminalWriter.NewLine"/>) to this writer's output.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(this ITerminalWriter terminalWriter)
    {
        terminalWriter.WriteLine();
    }

    #endregion WriteLine
}
