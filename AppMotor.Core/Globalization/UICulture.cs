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

using JetBrains.Annotations;

namespace AppMotor.Core.Globalization
{
    /// <summary>
    /// Provides access to information about the UI culture for the application.
    /// </summary>
    /// <remarks>
    /// This class' primary purpose is to make it easier to differentiate between
    /// <see cref="CultureInfo.CurrentCulture"/> and <see cref="CultureInfo.CurrentUICulture"/>.
    /// For more details, see: https://stackoverflow.com/a/329041/614177
    /// </remarks>
    public static class UICulture
    {
        /// <summary>
        /// This culture represents the formats for number, dates, and the like.
        ///
        /// <para>This may not be the user's display language. For this, use
        /// <see cref="CurrentLanguageCulture"/> instead.</para>
        /// </summary>
        [PublicAPI]
        public static CultureInfo CurrentFormatsCulture => CultureInfo.CurrentCulture;

        /// <summary>
        /// The display language used for translating texts in applications.
        ///
        /// <para>When formatting numbers, data, and the like, use <see cref="CurrentFormatsCulture"/>
        /// instead.</para>
        /// </summary>
        [PublicAPI]
        public static CultureInfo CurrentLanguageCulture => CultureInfo.CurrentUICulture;
    }
}
