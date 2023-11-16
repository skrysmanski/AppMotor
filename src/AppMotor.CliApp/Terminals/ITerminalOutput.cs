// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.ComponentModel;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// Represents the output part of a terminal. For the input side, see <see cref="ITerminalInput"/>.
/// </summary>
/// <remarks>
/// This interface exists primarily for unit testing purposes. See <see cref="ITerminal"/> for
/// more details.
/// </remarks>
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

    #region Write

    /// <summary>
    /// Writes the specified value to the terminal's standard output.
    /// </summary>
    void Write([Localizable(true)] object? value)
    {
        this.Out.Write(value);
    }

    /// <summary>
    /// Writes the specified value to the terminal's standard output.
    /// </summary>
    void Write([Localizable(true)] string? value)
    {
        this.Out.Write(value);
    }

    /// <summary>
    /// Writes the specified value to the terminal's standard output.
    /// </summary>
    void Write<T>([Localizable(true)] T? value) where T : class, IConvertible
    {
        this.Out.Write(value);
    }

    /// <summary>
    /// Writes the specified value to the terminal's standard output.
    /// </summary>
    void Write<T>([Localizable(true)] T? value) where T : struct, IConvertible
    {
        this.Out.Write(value);
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to the terminal's standard output.
    /// </summary>
    [StringFormatMethod("format")]
    void Write([Localizable(true)] string format, params object?[] args)
    {
        this.Out.Write(format, args);
    }

    #endregion Write

    #region WriteLine

    /// <summary>
    /// Writes the specified value to the terminal's standard output and appends a line break at the end.
    /// </summary>
    void WriteLine([Localizable(true)] object? value)
    {
        this.Out.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified value to the terminal's standard output and appends a line break at the end.
    /// </summary>
    void WriteLine([Localizable(true)] string? value)
    {
        this.Out.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified value to the terminal's standard output and appends a line break at the end.
    /// </summary>
    void WriteLine<T>([Localizable(true)] T? value) where T : class, IConvertible
    {
        this.Out.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified value to the terminal's standard output and appends a line break at the end.
    /// </summary>
    void WriteLine<T>([Localizable(true)] T? value) where T : struct, IConvertible
    {
        this.Out.WriteLine(value);
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/>, writes the result
    /// to the terminal's standard output, and appends a line break at the end.
    /// </summary>
    [StringFormatMethod("format")]
    void WriteLine([Localizable(true)] string format, params object?[] args)
    {
        this.Out.WriteLine(format, args);
    }

    /// <summary>
    /// Writes a line break to the terminal's standard output.
    /// </summary>
    void WriteLine()
    {
        this.Out.WriteLine();
    }

    #endregion WriteLine
}

/// <summary>
/// Extension methods for <see cref="ITerminalOutput"/>.
/// </summary>
/// <remarks>
/// The primary goal of this class it to give users of concrete implementations of <see cref="ITerminalOutput"/>
/// access to the default interface implementations (without the need to cast to the interface first).
/// </remarks>
public static class ITerminalOutputExtensions
{
    #region Write

    /// <summary>
    /// Writes the specified value to the terminal's standard output.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this ITerminalOutput terminalOutput, [Localizable(true)] object? value)
    {
        terminalOutput.Write(value);
    }

    /// <summary>
    /// Writes the specified value to the terminal's standard output.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this ITerminalOutput terminalOutput, [Localizable(true)] string? value)
    {
        terminalOutput.Write(value);
    }

    /// <summary>
    /// Writes the specified value to the terminal's standard output.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(this ITerminalOutput terminalOutput, [Localizable(true)] T? value) where T : class, IConvertible
    {
        terminalOutput.Write(value);
    }

    /// <summary>
    /// Writes the specified value to the terminal's standard output.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(this ITerminalOutput terminalOutput, [Localizable(true)] T? value) where T : struct, IConvertible
    {
        terminalOutput.Write(value);
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to the terminal's standard output.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [StringFormatMethod("format")]
    public static void Write(this ITerminalOutput terminalOutput, [Localizable(true)] string format, params object?[] args)
    {
        terminalOutput.Write(format, args);
    }

    #endregion Write

    #region WriteLine

    /// <summary>
    /// Writes the specified value to the terminal's standard output and appends a line break at the end.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(this ITerminalOutput terminalOutput, [Localizable(true)] object? value)
    {
        terminalOutput.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified value to the terminal's standard output and appends a line break at the end.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(this ITerminalOutput terminalOutput, [Localizable(true)] string? value)
    {
        terminalOutput.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified value to the terminal's standard output and appends a line break at the end.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine<T>(this ITerminalOutput terminalOutput, [Localizable(true)] T? value) where T : class, IConvertible
    {
        terminalOutput.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified value to the terminal's standard output and appends a line break at the end.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine<T>(this ITerminalOutput terminalOutput, [Localizable(true)] T? value) where T : struct, IConvertible
    {
        terminalOutput.WriteLine(value);
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/>, writes the result
    /// to the terminal's standard output, and appends a line break at the end.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [StringFormatMethod("format")]
    public static void WriteLine(this ITerminalOutput terminalOutput, [Localizable(true)] string format, params object?[] args)
    {
        terminalOutput.WriteLine(format, args);
    }

    /// <summary>
    /// Writes a line break to the terminal's standard output.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(this ITerminalOutput terminalOutput)
    {
        terminalOutput.WriteLine();
    }

    #endregion WriteLine
}
