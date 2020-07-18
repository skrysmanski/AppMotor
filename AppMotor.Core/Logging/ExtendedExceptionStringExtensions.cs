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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AppMotor.Core.Extensions;
using AppMotor.Core.Globalization;

using JetBrains.Annotations;

namespace AppMotor.Core.Logging
{
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
        [PublicAPI, NotNull, Pure]
        public static string ToStringExtended([NotNull] this Exception exception, [CanBeNull] IValueFormatter valueFormatter = null)
        {
            var builder = new ExtendedStringBuilder(valueFormatter);

            builder.AppendExtendedExceptionString(new StringIndentation(), exception);

            return builder.ToString();
        }

        private sealed class ExtendedStringBuilder
        {
            [NotNull]
            private static readonly ILoggableExceptionPropertyFilter PROPERTY_FILTER = new DefaultLoggableExceptionPropertyFilter();

            private const int HEADER_WITH = 70;

            [NotNull]
            private static readonly HeaderBox EXCEPTION_SECTION_HEADER = new HeaderBox(HEADER_WITH, doubleBorder: false);

            [NotNull]
            private static readonly HeaderBox INNER_EXCEPTION_HEADER = new HeaderBox(HEADER_WITH, doubleBorder: true);

            [NotNull]
            private readonly StringBuilder m_builder = new StringBuilder();

            [NotNull]
            private readonly IValueFormatter m_valueFormatter;

            public ExtendedStringBuilder([CanBeNull] IValueFormatter valueFormatter)
            {
                this.m_valueFormatter = valueFormatter ?? LoggableValues.DEFAULT_VALUE_FORMATTER;
            }

            public void AppendExtendedExceptionString(StringIndentation indentation, [NotNull] Exception exception)
            {
                //
                // Information about the specified exception itself.
                //
                AppendLine(indentation, $"Exception Type: {exception.GetType()}");
                AppendLine();

                AppendLine(indentation, $"Message: {exception.Message}");
                AppendLine();

                AppendAdditionalDataAndProperties(indentation, exception);

                // Exceptions that were never thrown (but only newed up) don't have a stacktrace.
                if (!string.IsNullOrEmpty(exception.StackTrace))
                {
                    AppendSectionHeader(indentation, EXCEPTION_SECTION_HEADER, "StackTrace");

                    foreach (var line in exception.StackTrace.SplitLines())
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

            private void AppendAdditionalDataAndProperties(StringIndentation indentation, [NotNull] Exception exception)
            {
                var loggableExceptionProperties = exception.GetLoggablePropertyValuesAsStrings(this.m_valueFormatter, PROPERTY_FILTER).ToList();
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
                        string keyAsString;

                        try
                        {
                            keyAsString = this.m_valueFormatter.FormatValue(key);
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        string valueAsString;

                        try
                        {
                            valueAsString = this.m_valueFormatter.FormatValue(value);
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
                    [NotNull] AggregateException aggregateException
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

            private void AppendSectionHeader(StringIndentation indentation, [NotNull] HeaderBox headerBox, [NotNull] string sectionName)
            {
                foreach (var line in headerBox.CreateBox(sectionName))
                {
                    AppendLine(indentation, line);
                }

                AppendLine();
            }

            private void AppendLine(StringIndentation indentation, [NotNull] string text)
            {
                this.m_builder.AppendLine($"{indentation}{text}");
            }

            private void AppendLine()
            {
                this.m_builder.AppendLine();
            }

            /// <inheritdoc />
            [NotNull]
            public override string ToString()
            {
                return this.m_builder.ToString();
            }
        }

        private readonly struct StringIndentation
        {
            private const char INDENT_CHAR = '\t';

            [CanBeNull]
            private readonly string m_indentation;

            private StringIndentation([NotNull] string indentation)
            {
                this.m_indentation = indentation;
            }

            [Pure]
            public StringIndentation Increase()
            {
                return new StringIndentation(this.m_indentation + INDENT_CHAR);
            }

            /// <inheritdoc />
            [NotNull]
            public override string ToString()
            {
                return this.m_indentation ?? "";
            }
        }

        private sealed class HeaderBox
        {
            private readonly bool m_doubleBorder;

            [NotNull]
            private readonly string m_horizontalBorder;

            [NotNull]
            private readonly string m_textContent;

            public HeaderBox(int width, bool doubleBorder)
            {
                this.m_doubleBorder = doubleBorder;
                this.m_horizontalBorder = new string(doubleBorder ? '═' : '─', width - 2);
                this.m_textContent = "{0} {{0,-{1}}} {0}".WithIC(doubleBorder ? '║' : '│', width - 4);
            }

            [NotNull, ItemNotNull]
            public IEnumerable<string> CreateBox([NotNull] string title)
            {
                if (this.m_doubleBorder)
                {
                    yield return $"╔{this.m_horizontalBorder}╗";
                    yield return this.m_textContent.WithIC(title);
                    yield return $"╚{this.m_horizontalBorder}╝";
                }
                else
                {
                    yield return $"┌{this.m_horizontalBorder}┐";
                    yield return this.m_textContent.WithIC(title);
                    yield return $"└{this.m_horizontalBorder}┘";
                }
            }
        }
    }
}
