// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections.Immutable;
using System.Reflection;

using AppMotor.Core.ComponentModel;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Logging;

/// <summary>
/// Extension methods related to logging of exceptions.
/// </summary>
public static class ExceptionLogExtensions
{
    private static readonly Dictionary<Type, LoggablePropertiesList> s_loggablePropertiesCache = new();

    /// <summary>
    /// Returns all (simple) loggable properties for the specified exception. The properties will be returned
    /// in alphabetical order. Only properties whose type matches <see cref="LoggableValues.IsSimpleLoggableType"/>
    /// will be returned.
    ///
    /// <para>Note: The list also includes all properties that have the type <c>object</c>. You'll need
    /// to filter them based on their actual value.</para>
    /// </summary>
    /// <seealso cref="GetLoggablePropertyValues"/>
    /// <seealso cref="GetLoggablePropertyValuesAsStrings"/>
    [PublicAPI]
    public static ImmutableArray<PropertyInfo> GetLoggableProperties(this Exception exception)
    {
        Validate.ArgumentWithName(nameof(exception)).IsNotNull(exception);

        var exceptionType = exception.GetType();

        LoggablePropertiesList? loggableProperties;

        lock (s_loggablePropertiesCache)
        {
            if (!s_loggablePropertiesCache.TryGetValue(exceptionType, out loggableProperties))
            {
                loggableProperties = new LoggablePropertiesList(exceptionType);
                s_loggablePropertiesCache[exceptionType] = loggableProperties;
            }
        }

        return loggableProperties.Value;
    }

    /// <summary>
    /// Returns all (simple) loggable values for the specified exception. The values will be returned
    /// sorted by property name.
    /// </summary>
    /// <param name="exception">This exception.</param>
    /// <param name="filter">The filter to use (optional).</param>
    /// <seealso cref="GetLoggableProperties"/>
    /// <seealso cref="GetLoggablePropertyValuesAsStrings"/>
    [PublicAPI]
    public static IEnumerable<KeyValuePair<string, object?>> GetLoggablePropertyValues(
            this Exception exception,
            ILoggableExceptionPropertyFilter? filter = null
        )
    {
        var loggableProperties = exception.GetLoggableProperties();

        foreach (var loggableProperty in loggableProperties)
        {
            if (filter?.ExcludeProperty(loggableProperty) == true)
            {
                continue;
            }

            object? loggableValue;

            try
            {
                loggableValue = loggableProperty.GetValue(exception);
            }
            catch (Exception ex)
            {
                loggableValue = $"Error while retrieving value for property '{loggableProperty.Name}': {ex.Message}";
            }

            if (filter?.ExcludePropertyValue(loggableValue, loggableProperty) == true)
            {
                continue;
            }

            if (loggableValue != null)
            {
                if (loggableProperty.PropertyType == typeof(object))
                {
                    // Check if the actual value is loggable.
                    if (!LoggableValues.IsSimpleLoggableType(loggableValue.GetType()))
                    {
                        continue;
                    }
                }
                else
                {
                    // Check if the actual value is sensitive.
                    if (loggableValue.IsSensitiveValue())
                    {
                        continue;
                    }
                }
            }

            yield return new KeyValuePair<string, object?>(loggableProperty.Name, loggableValue);
        }
    }

    /// <summary>
    /// Returns all (simple) loggable values for the specified exception as strings. The values
    /// will be returned sorted by property name. Value to text conversion is done via
    /// <see cref="LoggableValues.GetLoggableText"/>. See there for more details.
    /// </summary>
    /// <param name="exception">This exception.</param>
    /// <param name="valueFormatter">The formatter to use for converting the property values into
    /// strings.</param>
    /// <param name="filter">The filter to use (optional).</param>
    /// <seealso cref="GetLoggableProperties"/>
    /// <seealso cref="GetLoggablePropertyValues"/>
    [PublicAPI]
    public static IEnumerable<KeyValuePair<string, string>> GetLoggablePropertyValuesAsStrings(
            this Exception exception,
            IValueFormatter? valueFormatter = null,
            ILoggableExceptionPropertyFilter? filter = null
        )
    {
        var loggableValues = exception.GetLoggablePropertyValues(filter);

        foreach (var (propertyName, loggableValue) in loggableValues)
        {
            string loggableText;

            if (propertyName == nameof(Exception.HResult) && loggableValue is int hResult)
            {
                loggableText = HResultInfo.FormatHResult(hResult, includeName: true);
            }
            else
            {
                try
                {
                    loggableText = LoggableValues.GetLoggableText(loggableValue, valueFormatter);
                }
                catch (Exception ex)
                {
                    loggableText = $"Error while convert value of property '{propertyName}' to text: {ex.Message}";
                }
            }

            yield return new KeyValuePair<string, string>(propertyName, loggableText);
        }
    }

    private sealed class LoggablePropertiesList
    {
        private readonly PropertyInfo[] _allPropertiesOrderedByName;

        private readonly HashSet<Type> _allPropertyTypes = [];

        public ImmutableArray<PropertyInfo> Value => this._valueLazy.Value;

        private Lazy<ImmutableArray<PropertyInfo>> _valueLazy;

        public LoggablePropertiesList(Type exceptionType)
        {
            // NOTE: This list will only contain properties that are "visible" for the
            //   exception type. This especially excludes properties that hide properties
            //   from the base class with the same name. But that's ok. We don't need
            //   to return two properties with the same name.
            this._allPropertiesOrderedByName = exceptionType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                            .OrderBy(prop => prop.Name)
                                                            .ToArray();

            this._valueLazy = CreateValueLazy();

            foreach (var propertyInfo in this._allPropertiesOrderedByName)
            {
                this._allPropertyTypes.Add(propertyInfo.PropertyType);
            }

            LoggableValues.LoggabilityChanged += OnLoggabilityChanged;
        }

        private void OnLoggabilityChanged(object? sender, LoggabilityChangedEventArgs e)
        {
            // If any of our types changed loggability, re-create the list of loggable properties.
            if (this._allPropertyTypes.Contains(e.Type))
            {
                this._valueLazy = CreateValueLazy();
            }
        }

        private Lazy<ImmutableArray<PropertyInfo>> CreateValueLazy()
        {
            return new Lazy<ImmutableArray<PropertyInfo>>(
                CollectProperties,
                LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        private ImmutableArray<PropertyInfo> CollectProperties()
        {
            var loggablePropertiesBuilder = ImmutableArray.CreateBuilder<PropertyInfo>();

            foreach (var propertyInfo in this._allPropertiesOrderedByName)
            {
                if (!propertyInfo.CanRead)
                {
                    continue;
                }

                // NOTE: Since "object" may be anything, we include it here. We'll then check
                //   the actual value when having a concrete exception (see above).
                if (   LoggableValues.IsSimpleLoggableType(propertyInfo.PropertyType)
                    || propertyInfo.PropertyType == typeof(object))
                {
                    loggablePropertiesBuilder.Add(propertyInfo);
                }
            }

            return loggablePropertiesBuilder.ToImmutable();
        }
    }
}
