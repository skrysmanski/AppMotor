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
    /// Represents a value that can be localized into a target language/culture.
    /// </summary>
    public interface ILocalizable
    {
        /// <summary>
        /// Converts the value of this instance to an equivalent string using
        /// the specified culture-specific formatting information.
        /// </summary>
        [PublicAPI]
        string ToString(CultureInfo? culture);
    }
}
