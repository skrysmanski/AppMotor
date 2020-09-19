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
using System.Security;

using AppMotor.Core.DataModel;
using AppMotor.Core.Globalization;

namespace AppMotor.Core.Logging
{
    public class DefaultLoggableValueFormatter : DefaultValueFormatter
    {
        /// <inheritdoc />
        public DefaultLoggableValueFormatter(CultureInfo? cultureForFormatting)
            // NOTE: For logging purposes it's better to use "InvariantCulture" than "CurrentUICulture".
            : base(cultureForFormatting ?? CultureInfo.InvariantCulture)
        {
        }

        /// <inheritdoc />
        public override string? FormatValue<T>(T value)
        {
            switch (value)
            {
                case DateTime dateTime:
                    // Output is similar to RFC 1123 ("r") but without day name and with timezone information.
                    return dateTime.ToString("dd MMM yyy HH:mm:ss zzz", this.CultureForFormatting);

                case DateTimeOffset dateTime:
                    // Output is similar to RFC 1123 ("r") but without day name and with timezone information.
                    return dateTime.ToString("dd MMM yyy HH:mm:ss zzz", this.CultureForFormatting);

                // NOTE: TimeSpan is not listed here as it's default format is not globalizable.

                case SecureString _:
                case ISensitiveValue _:
                    return "<hidden>";

                default:
                    return base.FormatValue(value);
            }
        }
    }
}
