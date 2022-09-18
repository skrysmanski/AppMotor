// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Reflection;

using JetBrains.Annotations;

namespace AppMotor.Core.Logging;

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
    bool ExcludeProperty(PropertyInfo loggableProperty);

    /// <summary>
    /// Returns whether the specified loggable property value should be excluded from the result list.
    /// Note that this method won't be called for properties that have already been excluded via
    /// <see cref="ExcludeProperty"/>.
    /// </summary>
    /// <param name="propertyValue">The value to check.</param>
    /// <param name="loggableProperty">The property the value belongs to.</param>
    [Pure]
    bool ExcludePropertyValue(object? propertyValue, PropertyInfo loggableProperty);
}
