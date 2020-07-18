#region License
// Copyright 2020 - 2020 AppWeave.Core (https://github.com/skrysmanski/AppWeave.Core)
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

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions
{
    /// <summary>
    /// Various extension methods related to strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Convenience method for calling <c>string.Format()</c> with <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        [StringFormatMethod("message")]
        [PublicAPI, NotNull]
        public static string WithIC([NotNull, Localizable(false)] this string message, [NotNull] params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, message, args);
        }

        /// <summary>
        /// Splits the input string into individual lines.
        ///
        /// <para>Note: Lines are returned without the trailing end-of-line characters (i.e. <c>\r</c> and <c>\n</c>).</para>
        /// </summary>
        [PublicAPI, NotNull, ItemNotNull, Pure]
        public static IEnumerable<string> SplitLines([NotNull] this string input)
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
