// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// The various new line types. Use <see cref="NewLineTypesExtensions.GetNewLineCharacters"/> to
/// get the actual string value.
/// </summary>
public enum NewLineTypes
{
    /// <summary>
    /// The newline depends on the operating system the current process runs on. Maps to <see cref="Environment.NewLine"/>.
    /// </summary>
    SystemDefault,

    /// <summary>
    /// Use <c>\n</c> as newline (Linux/Unix line endings).
    /// </summary>
    Lf,

    /// <summary>
    /// Use <c>\r\n</c> as newline (Windows line endings).
    /// </summary>
    CrLf,
}

/// <summary>
/// Extension methods for <see cref="NewLineTypes"/>.
/// </summary>
public static class NewLineTypesExtensions
{
    /// <summary>
    /// Returns the string representation (e.g. "\r\n") for this <see cref="NewLineTypes"/>.
    /// </summary>
    [MustUseReturnValue]
    public static string GetNewLineCharacters(this NewLineTypes newLineType)
    {
        return newLineType switch
        {
            NewLineTypes.SystemDefault => Environment.NewLine,
            NewLineTypes.Lf => "\n",
            NewLineTypes.CrLf => "\r\n",
            _ => throw new ArgumentOutOfRangeException(nameof(newLineType), newLineType, message: null),
        };
    }
}
