﻿// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.ComponentModel;
using AppMotor.Core.Extensions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Logging;

/// <summary>
/// Provides utility methods for handling values for logging purposes.
/// </summary>
public static class LoggableValues
{
    /// <summary>
    /// The default value formatter used by this class.
    /// </summary>
    public static readonly IValueFormatter DEFAULT_VALUE_FORMATTER = new DefaultLoggableValueFormatter();

    /// <summary>
    /// This event is raised whenever the "loggability" of a type changes (or may have changed).
    /// The event args contain the new valid values.
    /// </summary>
    public static event EventHandler<LoggabilityChangedEventArgs>? LoggabilityChanged;

    static LoggableValues()
    {
        TypeMarkers.TypeMarkerAdded += OnTypeMarkerAdded;
    }

    private static void OnTypeMarkerAdded(object? sender, TypeMarkerAddedEventArgs e)
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
    public static bool IsSimpleLoggableType(Type typeToCheck)
    {
        Validate.ArgumentWithName(nameof(typeToCheck)).IsNotNull(typeToCheck);

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

            if (typeToCheck.IsNumericType(includeNullables: true))
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
    [PublicAPI]
    public static string GetLoggableText(object? loggableValue, IValueFormatter? valueFormatter = null)
    {
        return (valueFormatter ?? DEFAULT_VALUE_FORMATTER).FormatValue(loggableValue) ?? "";
    }
}
