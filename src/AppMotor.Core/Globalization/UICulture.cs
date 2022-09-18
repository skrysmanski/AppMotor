// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Globalization;

using JetBrains.Annotations;

namespace AppMotor.Core.Globalization;

/// <summary>
/// Provides access to information about the UI culture for the application - or for the current thread, to
/// be more precise - via <see cref="FormatsAndSorting"/> and <see cref="Language"/>.
/// </summary>
/// <remarks>
/// This class' primary purpose is to make it easier to differentiate between
/// <see cref="CultureInfo.CurrentCulture"/> and <see cref="CultureInfo.CurrentUICulture"/>.
/// For more details, see: https://stackoverflow.com/a/329041/614177
/// </remarks>
public static class UICulture
{
    /// <summary>
    /// This property defines (for the current thread) the default format for dates, times, numbers, currency values,
    /// the sorting order of text, casing conventions, and string comparisons.
    ///
    /// <para>This may not(!) be the user's display language. For this, use <see cref="Language"/> instead.</para>
    /// </summary>
    /// <remarks>
    /// Setting this property changes the value for the current thread - or for an <see cref="AsyncLocal{T}"/>
    /// to be precise.
    /// </remarks>
    [PublicAPI]
    public static CultureInfo FormatsAndSorting
    {
        get => CultureInfo.CurrentCulture;
        set => CultureInfo.CurrentCulture = value;
    }

    /// <summary>
    /// The display language (of the current thread) used for translating texts in applications.
    ///
    /// <para>When formatting numbers, data, currency, sorting and the like, use
    /// <see cref="FormatsAndSorting"/> instead.</para>
    /// </summary>
    /// <remarks>
    /// Setting this property changes the value for the current thread - or for an <see cref="AsyncLocal{T}"/>
    /// to be precise.
    /// </remarks>
    [PublicAPI]
    public static CultureInfo Language
    {
        get => CultureInfo.CurrentUICulture;
        set => CultureInfo.CurrentUICulture = value;
    }
}
