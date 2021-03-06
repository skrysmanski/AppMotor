﻿#region License
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;

using AppMotor.Core.Globalization;

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions
{
    /// <summary>
    /// Various extension methods related to strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Convenience method for calling <c>string.Format()</c> with the current UI culture
        /// (<see cref="UICulture.CurrentFormatProvider"/>) as format provider.
        /// </summary>
        /// <seealso cref="WithIC"/>
        [StringFormatMethod("message")]
        [PublicAPI]
        public static string With([Localizable(true)] this string message, params object[] args)
        {
            return string.Format(UICulture.CurrentFormatProvider, message, args);
        }

        /// <summary>
        /// Convenience method for calling <c>string.Format()</c> with <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        /// <seealso cref="With"/>
        [StringFormatMethod("message")]
        [PublicAPI]
        public static string WithIC([Localizable(false)] this string message, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, message, args);
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
}
