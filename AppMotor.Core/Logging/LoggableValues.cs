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

using AppMotor.Core.DataModel;
using AppMotor.Core.Extensions;
using AppMotor.Core.Globalization;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Logging
{
    public static class LoggableValues
    {
        /// <summary>
        /// The default value formatter used by this class.
        /// </summary>
        [NotNull]
        public static readonly IValueFormatter DEFAULT_VALUE_FORMATTER = new DefaultLoggableValueFormatter(CultureInfo.InvariantCulture);

        /// <summary>
        /// This event is raised whenever the "loggability" of a type changes (or may have changed).
        /// The event args contain the new valid values.
        /// </summary>
        public static event EventHandler<LoggabilityChangedEventArgs> LoggabilityChanged;

        static LoggableValues()
        {
            TypeMarkers.TypeMarkerAdded += OnTypeMarkerAdded;
        }

        private static void OnTypeMarkerAdded(object sender, [NotNull] TypeMarkerAddedEventArgs e)
        {
            if (e.TypeMarkerType == typeof(SimpleLoggableValueMarker) || e.TypeMarkerType == typeof(SensitiveValueMarker))
            {
                LoggabilityChanged?.Invoke(null, new LoggabilityChangedEventArgs(e.MarkedType));
            }
        }

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
        public static bool IsSimpleLoggableType([NotNull] Type typeToCheck)
        {
            Verify.ParamNotNull(typeToCheck, nameof(typeToCheck));

            // Sensitive values must never be logged.
            // NOTE: This also takes precedence before "ISimpleLoggableValue"/"SimpleLoggableValueMarker".
            if (SensitiveValues.IsSensitiveValueType(typeToCheck))
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

        /// <summary>
        /// Returns the loggable text representation for the specified value. If no value formatter is
        /// specified, an instance of <see cref="DefaultLoggableValueFormatter"/> will be used.
        /// </summary>
        [PublicAPI, NotNull]
        public static string GetLoggableText([CanBeNull] object loggableValue, [CanBeNull] IValueFormatter valueFormatter = null)
        {
            return (valueFormatter ?? DEFAULT_VALUE_FORMATTER).FormatValue(loggableValue) ?? "";
        }
    }
}
