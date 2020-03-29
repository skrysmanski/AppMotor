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

using System;

using AppWeave.Core.DataModel;
using AppWeave.Core.Extensions;
using AppWeave.Core.Utils;

using JetBrains.Annotations;

namespace AppWeave.Core.Logging
{
    public static class LoggableValues
    {
        /// <summary>
        /// Whether this type is a type can be (easily) logged. This includes all numeric
        /// types, primitive types as well as some basic .NET types (like <c>string</c>
        /// or <see cref="DateTime"/>). This explicitly excludes any form of collection.
        /// It also excludes exceptions as they can't be logged with one line.
        ///
        /// <para>Also respects <see cref="ISensitiveValue"/>, <see cref="SensitiveValueMarker"/>,
        /// <see cref="ISimpleLoggableValue"/>, and <see cref="SimpleLoggableValueMarker"/>.</para>
        /// </summary>
        [PublicAPI, Pure]
        public static bool IsSimpleLoggableType([NotNull] this Type typeToCheck)
        {
            Verify.ParamNotNull(typeToCheck, nameof(typeToCheck));

            // Sensitive values must never be logged.
            // NOTE: This also takes precedence before "ISimpleLoggableValue"/"SimpleLoggableValueMarker".
            if (typeToCheck.Is<ISensitiveValue>() || typeToCheck.IsMarkedWith<SensitiveValueMarker>())
            {
                return false;
            }

            if (typeToCheck.IsValueType)
            {
                var underlyingType = Nullable.GetUnderlyingType(typeToCheck);
                if (underlyingType != null)
                {
                    typeToCheck = underlyingType;
                }

                if (typeToCheck.IsNumericType())
                {
                    return true;
                }
                else if (typeToCheck.IsEnum)
                {
                    return true;
                }
                else if (   typeToCheck == typeof(DateTime)
                         || typeToCheck == typeof(TimeSpan)
                         || typeToCheck == typeof(DateTimeOffset)
                         || typeToCheck == typeof(bool)
                         || typeToCheck == typeof(char)
                         || typeToCheck == typeof(Guid)
                        )
                {
                    return true;
                }
            }
            else
            {
                // Reference types
                if (typeToCheck == typeof(string))
                {
                    return true;
                }
            }

            if (typeToCheck.Is<ISimpleLoggableValue>() || typeToCheck.IsMarkedWith<SimpleLoggableValueMarker>())
            {
                return true;
            }

            return false;
        }
    }
}
