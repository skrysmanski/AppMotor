// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.ComponentModel;

using AppMotor.Core.Globalization;

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions;

/// <summary>
/// Various extension methods related to strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Convenience method for calling <c>string.Format()</c> with the current UI culture
    /// (<see cref="UICulture.FormatsAndSorting"/>) as format provider.
    /// </summary>
    /// <seealso cref="InvariantCultureExtensions.WithIC"/>
    [StringFormatMethod("message")]
    [MustUseReturnValue]
    [PublicAPI]
    public static string With([Localizable(true)] this string message, params object[] args)
    {
        return string.Format(UICulture.FormatsAndSorting, message, args);
    }

    /// <summary>
    /// Splits the input string into individual lines.
    ///
    /// <para>Note: Lines are returned without the trailing end-of-line characters (i.e. <c>\r</c> and <c>\n</c>).</para>
    /// </summary>
    [PublicAPI, Pure]
    public static IEnumerable<string> SplitLines(this string input)
    {
        using var reader = new StringReader(input);

        while (true)
        {
            var line = reader.ReadLine();
            if (line == null)
            {
                break;
            }

            yield return line;
        }
    }
}
