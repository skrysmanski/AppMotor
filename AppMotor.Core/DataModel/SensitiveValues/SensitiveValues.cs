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
using System.Diagnostics.CodeAnalysis;
using System.Security;

using AppMotor.Core.Extensions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

using NotNullOnExitAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace AppMotor.Core.DataModel
{
    /// <summary>
    /// Contains methods related to sensitive values. See <see cref="ISensitiveValue"/>
    /// for more details.
    /// </summary>
    public static class SensitiveValues
    {
        /// <summary>
        /// Returns whether the specified value is a sensitive value (see <see cref="ISensitiveValue"/>).
        /// Respects <see cref="SecureString"/>, <see cref="ISensitiveValue"/>, and <see cref="SensitiveValueMarker"/>.
        /// </summary>
        /// <seealso cref="IsSensitiveValueType"/>
        [PublicAPI, Pure]
        public static bool IsSensitiveValue<T>([DisallowNull, NotNullOnExit] this T value)
        {
            Validate.Argument.IsNotNullUnconstrained(value, nameof(value));

            if (value is SecureString || value is ISensitiveValue)
            {
                return true;
            }

            if (value.GetType().IsMarkedWith<SensitiveValueMarker>())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether the specified type is a type that may contain sensitive information (see <see cref="ISensitiveValue"/>).
        /// Respects <see cref="SecureString"/>, <see cref="ISensitiveValue"/>, and <see cref="SensitiveValueMarker"/>.
        /// </summary>
        /// <seealso cref="IsSensitiveValue{T}"/>
        [PublicAPI, Pure]
        public static bool IsSensitiveValueType(Type typeToCheck)
        {
            if (typeToCheck == typeof(SecureString) || typeToCheck.Is<ISensitiveValue>())
            {
                return true;
            }

            if (typeToCheck.IsMarkedWith<SensitiveValueMarker>())
            {
                return true;
            }

            return false;
        }
    }
}
