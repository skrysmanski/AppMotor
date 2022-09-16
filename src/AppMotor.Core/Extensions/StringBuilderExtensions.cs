// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

using AppMotor.Core.Utils;

namespace AppMotor.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="StringBuilder"/>.
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    /// Appends the specified line terminator to the end of this string builder.
    /// </summary>
    /// <seealso cref="AppendLineLf"/>
    /// <see cref="AppendLineCrLf"/>
    public static StringBuilder AppendLine(this StringBuilder stringBuilder, LineTerminators lineTerminator)
    {
        return stringBuilder.Append(lineTerminator.AsString());
    }

    /// <summary>
    /// Appends the Linux/Unix line terminator to the end of this string builder.
    /// </summary>
    /// <seealso cref="AppendLineCrLf"/>
    /// <see cref="AppendLine"/>
    public static StringBuilder AppendLineLf(this StringBuilder stringBuilder)
    {
        return stringBuilder.Append('\n');
    }

    /// <summary>
    /// Appends the Windows line terminator to the end of this string builder.
    /// </summary>
    /// <seealso cref="AppendLineLf"/>
    /// <seealso cref="AppendLine"/>
    public static StringBuilder AppendLineCrLf(this StringBuilder stringBuilder)
    {
        return stringBuilder.Append("\r\n");
    }
}
