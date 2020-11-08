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

using JetBrains.Annotations;

namespace AppMotor.Core.Utils
{
    /// <summary>
    /// Convenience base class for <see cref="IFormatProvider"/> implementations.
    /// </summary>
    [PublicAPI]
    public abstract class FormatProviderBase : IFormatProvider
    {
        private readonly Formatter m_formatter;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected FormatProviderBase()
        {
            this.m_formatter = new Formatter(this);
        }

        /// <inheritdoc />
        public object? GetFormat(Type? formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this.m_formatter;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Formats the specified value with the specified format string.
        /// </summary>
        protected abstract string Format(string? format, object? value);

        private sealed class Formatter : ICustomFormatter
        {
            private readonly FormatProviderBase m_outerFormatProvider;

            public Formatter(FormatProviderBase outerFormatProvider)
            {
                this.m_outerFormatProvider = outerFormatProvider;
            }

            /// <inheritdoc />
            public string Format(string? format, object? value, IFormatProvider? formatProvider)
            {
                return this.m_outerFormatProvider.Format(format, value);
            }
        }
    }
}
