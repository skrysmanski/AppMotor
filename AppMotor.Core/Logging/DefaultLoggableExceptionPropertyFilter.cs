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
using System.Reflection;

namespace AppMotor.Core.Logging
{
    /// <summary>
    /// The property filter used by <see cref="ExtendedExceptionStringExtensions.ToStringExtended"/>.
    /// You may use it for when you want to mirror <c>ToStringExtended()</c>. However, the filtered
    /// properties may change at any time without warning (i.e. the list of filtered properties
    /// is NOT part of the class' contract).
    /// </summary>
    public class DefaultLoggableExceptionPropertyFilter : ILoggableExceptionPropertyFilter
    {
        /// <inheritdoc />
        public virtual bool ExcludeProperty(PropertyInfo loggableProperty)
        {
            switch (loggableProperty.Name)
            {
                // These properties are usually already present somewhere else in the exception log output.
                case nameof(Exception.StackTrace):
                case nameof(Exception.Message):
                case nameof(Exception.TargetSite): // <-- already present in the stack trace
                    return true;

                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public virtual bool ExcludePropertyValue(object propertyValue, PropertyInfo loggableProperty)
        {
            if (propertyValue is null)
            {
                // No need to put empty values in the output.
                return true;
            }

            if (propertyValue is string stringValue && string.IsNullOrWhiteSpace(stringValue))
            {
                // No need to put empty values in the output.
                return true;
            }

            if (loggableProperty.Name == nameof(Exception.HResult))
            {
                if (propertyValue is int hResult && hResult == HResults.COR_E_EXCEPTION)
                {
                    // Default HResult. No useful information here.
                    return true;
                }
            }

            return false;
        }
    }
}
