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

using System;
using System.Globalization;

using JetBrains.Annotations;

namespace AppMotor.Core.Globalization
{
    /// <summary>
    /// Default implementation of <see cref="IValueFormatter"/>. Supports <see cref="IConvertible"/>
    /// and <see cref="ILocalizable"/>. Does not provide any custom format strings in <see cref="FormatValue{T}"/>.
    /// </summary>
    public class DefaultValueFormatter : IValueFormatter
    {
        /// <summary>
        /// The culture to be used for formatting. If not set, <see cref="CultureInfo.CurrentUICulture"/>
        /// will be used.
        /// </summary>
        [PublicAPI]
        public CultureInfo? CultureForFormatting { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cultureForFormatting">See <see cref="CultureForFormatting"/></param>
        public DefaultValueFormatter(CultureInfo? cultureForFormatting)
        {
            this.CultureForFormatting = cultureForFormatting;
        }

        /// <inheritdoc />
        public virtual string? FormatValue<T>(T value)
        {
            switch (value)
            {
                case null:
                    return null;

                case IConvertible convertible:
                    return convertible.ToString(this.CultureForFormatting ?? CultureInfo.CurrentUICulture);

                case ILocalizable localizable:
                    return localizable.ToString(this.CultureForFormatting ?? CultureInfo.CurrentUICulture);

                default:
                    return value.ToString();
            }
        }
    }
}
