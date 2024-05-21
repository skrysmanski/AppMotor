// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Contains the various line terminator types. Use <see cref="LineTerminatorsExtensions.AsString"/> to
/// get the actual string value.
/// </summary>
public enum LineTerminators
{
    /// <summary>
    /// The line terminator depends on the operating system the current process runs on. Maps to <see cref="Environment.NewLine"/>.
    /// </summary>
    SystemDefault,

    /// <summary>
    /// Use <c>\n</c> as line terminator (Linux/Unix line endings).
    /// </summary>
    Lf,

    /// <summary>
    /// Use <c>\r\n</c> as line terminator (Windows line endings).
    /// </summary>
    CrLf,
}

/// <summary>
/// Extension methods for <see cref="LineTerminators"/>.
/// </summary>
public static class LineTerminatorsExtensions
{
    private static readonly bool IS_LF_SYSTEM_DEFAULT = "\n" == Environment.NewLine;
    private static readonly bool IS_CRLF_SYSTEM_DEFAULT = "\r\n" == Environment.NewLine;

    /// <summary>
    /// Returns the string representation (e.g. "\r\n") for this <see cref="LineTerminators"/>.
    /// </summary>
    [MustUseReturnValue]
    public static string AsString(this LineTerminators lineTerminator)
    {
        return lineTerminator switch
        {
            LineTerminators.SystemDefault => Environment.NewLine,
            LineTerminators.Lf => "\n",
            LineTerminators.CrLf => "\r\n",
            _ => throw new ArgumentOutOfRangeException(nameof(lineTerminator), lineTerminator, message: null),
        };
    }

    /// <summary>
    /// Returns whether this <see cref="LineTerminators"/> value represents the same string
    /// as <see cref="LineTerminators.SystemDefault"/> on the current system. For example,
    /// calling this method on <see cref="LineTerminators.CrLf"/> will return <c>true</c> on
    /// Windows and <c>false</c> on non-Windows systems.
    /// </summary>
    /// <remarks>
    /// Calling this method only makes sense for <see cref="LineTerminators.Lf"/> and <see cref="LineTerminators.CrLf"/>.
    /// Calling it for <see cref="LineTerminators.SystemDefault"/> will always return true.
    /// </remarks>
    [MustUseReturnValue]
    public static bool IsSystemDefault(this LineTerminators lineTerminator)
    {
        return lineTerminator switch
        {
            LineTerminators.SystemDefault => true,
            LineTerminators.Lf => IS_LF_SYSTEM_DEFAULT,
            LineTerminators.CrLf => IS_CRLF_SYSTEM_DEFAULT,
            _ => throw new ArgumentOutOfRangeException(nameof(lineTerminator), lineTerminator, message: null),
        };
    }
}
