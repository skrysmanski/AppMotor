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

using System.Globalization;
using System.Text;

using AppMotor.Core.Extensions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Logging;

/// <summary>
/// Provides the exception extension method <see cref="ToStringExtended"/>.
/// </summary>
public static class ExtendedExceptionStringExtensions
{
    /// <summary>
    /// Returns an extended string representation of this exception (i.e. more detailed
    /// than <see cref="Exception.ToString"/>). This extended representation contains
    /// all inner exceptions with all the exception data and properties.
    /// </summary>
    /// <param name="exception">This exception</param>
    /// <param name="valueFormatter">The value formatter to use. If this is <c>null</c>,
    /// <see cref="LoggableValues.DEFAULT_VALUE_FORMATTER"/> will be used.</param>
    [PublicAPI, Pure]
    public static string ToStringExtended(this Exception exception, IValueFormatter? valueFormatter = null)
    {
        Validate.ArgumentWithName(nameof(exception)).IsNotNull(exception);

        var builder = new ExtendedStringBuilder(valueFormatter);

        builder.AppendExtendedExceptionString(new StringIndentation(), exception);

        return builder.ToString();
    }

    private sealed class ExtendedStringBuilder
    {
        private static readonly ILoggableExceptionPropertyFilter PROPERTY_FILTER = new DefaultLoggableExceptionPropertyFilter();

        private const int HEADER_WITH = 70;

        private static readonly HeaderBox EXCEPTION_SECTION_HEADER = new(HEADER_WITH, doubleBorder: false);

        private static readonly HeaderBox INNER_EXCEPTION_HEADER = new(HEADER_WITH, doubleBorder: true);

        private readonly StringBuilder _builder = new();

        private readonly IValueFormatter _valueFormatter;

        public ExtendedStringBuilder(IValueFormatter? valueFormatter)
        {
            this._valueFormatter = valueFormatter ?? LoggableValues.DEFAULT_VALUE_FORMATTER;
        }

        public void AppendExtendedExceptionString(StringIndentation indentation, Exception exception)
        {
            //
            // Information about the specified exception itself.
            //
            AppendLine(indentation, $"[{exception.GetType()}] {exception.Message}");
            AppendLine();

            AppendAdditionalDataAndProperties(indentation, exception);

            // Exceptions that were never thrown (but only newed up) don't have a stacktrace.
            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                AppendSectionHeader(indentation, EXCEPTION_SECTION_HEADER, "StackTrace");

                foreach (var line in StackTraceCleaner.CleanupStackTraceLines(exception.StackTrace.SplitLines()))
                {
                    AppendLine(indentation, line.Trim());
                }

                AppendLine();
            }

            //
            // Inner exceptions
            //
            if (exception is AggregateException aggregateException)
            {
                AppendDetailsFromAggregateException(indentation, aggregateException);
            }
            else if (exception.InnerException != null)
            {
                indentation = indentation.Increase();

                AppendSectionHeader(indentation, INNER_EXCEPTION_HEADER, "Inner Exception");

                AppendExtendedExceptionString(indentation, exception.InnerException);
            }
        }

        private void AppendAdditionalDataAndProperties(StringIndentation indentation, Exception exception)
        {
            var loggableExceptionProperties = exception.GetLoggablePropertyValuesAsStrings(this._valueFormatter, PROPERTY_FILTER).ToList();
            if (loggableExceptionProperties.Count > 0)
            {
                AppendSectionHeader(indentation, EXCEPTION_SECTION_HEADER, "Exception Properties");

                foreach (var (propertyName, propertyValue) in loggableExceptionProperties)
                {
                    AppendLine(indentation, "{0}: {1}".WithIC(propertyName, propertyValue));
                    AppendLine();
                }
            }

            var exceptionData = exception.GetData();
            if (exceptionData.Count > 0)
            {
                AppendSectionHeader(indentation, EXCEPTION_SECTION_HEADER, "Exception Data");

                foreach (var (key, value) in exceptionData)
                {
                    string? keyAsString;

                    try
                    {
                        keyAsString = this._valueFormatter.FormatValue(key);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    string? valueAsString;

                    try
                    {
                        valueAsString = this._valueFormatter.FormatValue(value);
                    }
                    catch (Exception ex)
                    {
                        valueAsString = $"Error while converting value to text: {ex.Message}";
                    }

                    AppendLine(indentation, $"{keyAsString}: {valueAsString}");
                    AppendLine();
                }
            }
        }

        private void AppendDetailsFromAggregateException(
                StringIndentation indentation,
                AggregateException aggregateException
            )
        {
            var innerExceptions = aggregateException.InnerExceptions;
            if (innerExceptions.Count == 0)
            {
                return;
            }

            string sectionHeader = "Inner Exceptions ({0})".WithIC(innerExceptions.Count);

            AppendSectionHeader(indentation, EXCEPTION_SECTION_HEADER, sectionHeader);

            indentation = indentation.Increase();

            uint count = 1;
            foreach (var innerException in innerExceptions)
            {
                AppendSectionHeader(indentation, INNER_EXCEPTION_HEADER, "Inner Exception {0}/{1}".WithIC(count, innerExceptions.Count));

                AppendExtendedExceptionString(indentation, innerException);

                count++;
            }
        }

        private void AppendSectionHeader(StringIndentation indentation, HeaderBox headerBox, string sectionName)
        {
            foreach (var line in headerBox.CreateBox(sectionName))
            {
                AppendLine(indentation, line);
            }

            AppendLine();
        }

        private void AppendLine(StringIndentation indentation, string text)
        {
            this._builder.AppendLine(CultureInfo.InvariantCulture, $"{indentation}{text}");
        }

        private void AppendLine()
        {
            this._builder.AppendLine();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this._builder.ToString();
        }
    }

    private readonly struct StringIndentation
    {
        private const char INDENT_CHAR = '\t';

        private readonly string? _indentation;

        private StringIndentation(string indentation)
        {
            this._indentation = indentation;
        }

        [Pure]
        public StringIndentation Increase()
        {
            return new StringIndentation(this._indentation + INDENT_CHAR);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this._indentation ?? "";
        }
    }

    private sealed class HeaderBox
    {
        private readonly bool _doubleBorder;

        private readonly string _horizontalBorder;

        private readonly string _textContent;

        public HeaderBox(int width, bool doubleBorder)
        {
            this._doubleBorder = doubleBorder;
            this._horizontalBorder = new string(doubleBorder ? '═' : '─', width - 2);
            this._textContent = "{0} {{0,-{1}}} {0}".WithIC(doubleBorder ? '║' : '│', width - 4);
        }

        public IEnumerable<string> CreateBox(string title)
        {
            if (this._doubleBorder)
            {
                yield return $"╔{this._horizontalBorder}╗";
                yield return this._textContent.WithIC(title);
                yield return $"╚{this._horizontalBorder}╝";
            }
            else
            {
                yield return $"┌{this._horizontalBorder}┐";
                yield return this._textContent.WithIC(title);
                yield return $"└{this._horizontalBorder}┘";
            }
        }
    }
}