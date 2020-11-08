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

using AppMotor.Core.Globalization;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils
{
    /// <summary>
    /// Default implementation of <see cref="IValueFormatter"/>. Supports <see cref="IConvertible"/>
    /// and <see cref="IFormattable"/>. Does not provide any custom format strings in <see cref="FormatValue{T}"/>.
    /// </summary>
    public class DefaultValueFormatter : IValueFormatter
    {
        private readonly IFormatProvider? m_formatProvider;

        /// <summary>
        /// The format provider to be used for formatting values.
        /// </summary>
        [PublicAPI]
        public IFormatProvider FormatProvider => this.m_formatProvider ?? UICulture.CurrentFormatProvider;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="formatProvider">The format provider to be used for formatting values. If <c>null</c>,
        /// <see cref="UICulture.CurrentFormatProvider"/> will be used.</param>
        public DefaultValueFormatter(IFormatProvider? formatProvider)
        {
            this.m_formatProvider = formatProvider;
        }

        /// <inheritdoc />
        public virtual string? FormatValue<T>(T value)
        {
            switch (value)
            {
                case null:
                    return null;

                case IFormattable formattable:
                    return formattable.ToString(format: null, this.FormatProvider);

                case IConvertible convertible:
                    return convertible.ToString(this.FormatProvider);

                default:
                    return value.ToString();
            }
        }
    }
}
