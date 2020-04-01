﻿#region License
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AppWeave.Core.Extensions;

using JetBrains.Annotations;

namespace AppWeave.Core.Logging
{
    /// <summary>
    /// Provies information about <see cref="Exception.HResult"/>.
    /// </summary>
    public static class HResultInfo
    {
        [NotNull]
        private static readonly Dictionary<int, string> s_hResultNames;

        static HResultInfo()
        {
            s_hResultNames = GetHResultNames();
        }

        [NotNull, Pure]
        private static Dictionary<int, string> GetHResultNames()
        {
            var hResultNames = new Dictionary<int, string>();

            var fieldInfos = typeof(HResults).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var fieldInfo in fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly))
            {
                int hResult = (int)fieldInfo.GetRawConstantValue();
                hResultNames[hResult] = fieldInfo.Name;
            }

            return hResultNames;
        }

        /// <summary>
        /// Returns the name for the specified value, if it has a name. Otherwise
        /// <c>null</c> will be returned.
        /// </summary>
        [PublicAPI, CanBeNull, Pure]
        public static string GetHResultName(int hResult)
        {
            s_hResultNames.TryGetValue(hResult, out var hResultName);
            return hResultName;
        }

        /// <summary>
        /// Formats the specified value into a string.
        /// </summary>
        /// <param name="hResult">The value to format</param>
        /// <param name="includeName">Whether to include the value's name, if
        /// it has one (see <see cref="GetHResultName"/>).</param>
        /// <returns></returns>
        [PublicAPI, NotNull, Pure]
        public static string FormatHResult(int hResult, bool includeName)
        {
            // Format the HResult property in hexadecimal notation.
            var formattedHResult = "0x{0:X8}".WithIC(hResult);

            if (includeName && s_hResultNames.TryGetValue(hResult, out var hResultName))
            {
                formattedHResult += $" ({hResultName})";
            }

            return formattedHResult;
        }
    }
}
