// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Reflection;

using AppMotor.Core.Utils;

namespace AppMotor.Core.Logging;

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
        Validate.ArgumentWithName(nameof(loggableProperty)).IsNotNull(loggableProperty);

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
    public virtual bool ExcludePropertyValue(object? propertyValue, PropertyInfo loggableProperty)
    {
        Validate.ArgumentWithName(nameof(loggableProperty)).IsNotNull(loggableProperty);

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
            if (propertyValue is HResults.COR_E_EXCEPTION)
            {
                // Default HResult. No useful information here.
                return true;
            }
        }

        return false;
    }
}
