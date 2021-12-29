#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Globalization;
using System.Threading;

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