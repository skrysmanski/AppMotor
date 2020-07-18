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

using System.Reflection;

using JetBrains.Annotations;

namespace AppMotor.Core.Logging
{
    /// <summary>
    /// Filter for <see cref="ExceptionLogExtensions.GetLoggablePropertyValues"/> and
    /// <see cref="ExceptionLogExtensions.GetLoggablePropertyValuesAsStrings"/>.
    /// </summary>
    public interface ILoggableExceptionPropertyFilter
    {
        /// <summary>
        /// Returns whether the specified property should be excluded from the result.
        /// </summary>
        [Pure]
        bool ExcludeProperty([NotNull] PropertyInfo loggableProperty);

        /// <summary>
        /// Returns whether the specified loggable property value should be excluded from the result list.
        /// Note that this method won't be called for properties that have already been excluded via
        /// <see cref="ExcludeProperty"/>.
        /// </summary>
        /// <param name="propertyValue">The value to check.</param>
        /// <param name="loggableProperty">The property the value belongs to.</param>
        [Pure]
        bool ExcludePropertyValue([CanBeNull] object propertyValue, [NotNull] PropertyInfo loggableProperty);
    }
}
