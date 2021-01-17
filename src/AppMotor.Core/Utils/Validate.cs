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
using System.Collections.Generic;

using AppMotor.Core.Exceptions;

using JetBrains.Annotations;

using NotNullOnExitAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace AppMotor.Core.Utils
{
    /// <summary>
    /// Provides various validation methods for parameters and values in general. Raises <see cref="ArgumentException"/>s
    /// and <see cref="ValueException"/>s.
    /// </summary>
    public static class Validate
    {
        /// <summary>
        /// Validates arguments/parameters. Throws <see cref="ArgumentException"/> (or one of its child classes) in
        /// case the validation fails.
        /// </summary>
        /// <remarks>
        /// This class is called <c>Argument</c> rather than <c>Parameter</c> because all of its
        /// methods throw <see cref="ArgumentException"/>s.
        /// </remarks>
        /// <seealso cref="Value"/>
#pragma warning disable CA1034 // Nested types should not be visible
        public static partial class Argument
#pragma warning restore CA1034 // Nested types should not be visible
        {
            private static readonly ArgumentValidator VALIDATOR = new();
        }

        /// <summary>
        /// Validates regular values (i.e. everything that's not an argument/parameter). Throws <see cref="ValueException"/>
        /// (or one of its child classes) in case the validation fails.
        /// </summary>
        /// <seealso cref="Argument"/>
#pragma warning disable CA1034 // Nested types should not be visible
        public static partial class Value
#pragma warning restore CA1034 // Nested types should not be visible
        {
            private static readonly ValueValidator VALIDATOR = new();
        }

        private sealed class ArgumentValidator : ValidatorBase<ArgumentException>
        {
            /// <inheritdoc />
            protected override ArgumentException CreateNullException(string? valueName)
            {
                return new ArgumentNullException(
                    message: ExceptionMessages.VALUE_IS_NULL, // <-- we specify this here so that the message doesn't get translated
                    paramName: valueName ?? ExceptionMessages.DEFAULT_VALUE_NAME
                );
            }

            /// <inheritdoc />
            protected override ArgumentException CreateCollectionIsReadOnlyException(string? valueName)
            {
                return new CollectionIsReadOnlyArgumentException(paramName: valueName ?? ExceptionMessages.DEFAULT_VALUE_NAME);
            }

            /// <inheritdoc />
            protected override ArgumentException CreateRootException(string message, string? valueName)
            {
                return new ArgumentException(message: message, paramName: valueName ?? ExceptionMessages.DEFAULT_VALUE_NAME);
            }
        }

        private sealed class ValueValidator : ValidatorBase<ValueException>
        {
            /// <inheritdoc />
            protected override ValueException CreateNullException(string? valueName)
            {
                return new ValueNullException(message: null, valueName: valueName ?? ExceptionMessages.DEFAULT_VALUE_NAME);
            }

            /// <inheritdoc />
            protected override ValueException CreateCollectionIsReadOnlyException(string? valueName)
            {
                return new CollectionIsReadOnlyValueException(valueName: valueName ?? ExceptionMessages.DEFAULT_VALUE_NAME);
            }

            /// <inheritdoc />
            protected override ValueException CreateRootException(string message, string? valueName)
            {
                return new ValueException(message: message, valueName: valueName ?? ExceptionMessages.DEFAULT_VALUE_NAME);
            }
        }

        private abstract partial class ValidatorBase<TBaseException> where TBaseException : Exception
        {
            protected abstract TBaseException CreateNullException(string? valueName);

            protected abstract TBaseException CreateCollectionIsReadOnlyException(string? valueName);

            protected abstract TBaseException CreateRootException(string message, string? valueName);
        }

        internal static class ExceptionMessages
        {
            public const string DEFAULT_VALUE_NAME = "unknown";

            public const string DEFAULT_MESSAGE = "The value is invalid.";

            public const string VALUE_IS_NULL = "The value must not be null.";

            public const string STRING_IS_EMPTY = "The string must not be empty.";

            public const string STRING_IS_WHITE_SPACES = "The string must not contain just white space characters.";

            public const string COLLECTION_IS_EMPTY = "The collection must not be empty.";
        }

        #region Argument Validation

        public static partial class Argument
        {
            [PublicAPI]
            public static void IsNotNull<T>(
                    [InstantHandle, NoEnumeration, NotNullOnExit] T? value,
                    [InvokerParameterName] string paramName
                )
                    where T : class
            {
                VALIDATOR.IsNotNull(value, paramName);
            }

            [PublicAPI]
            public static void IsNotNull<T>(
                    [InstantHandle, NoEnumeration, NotNullOnExit] T? value,
                    [InvokerParameterName] string paramName
                )
                    where T : struct
            {
                VALIDATOR.IsNotNull(value, paramName);
            }

            [PublicAPI]
            public static void IsNotNullUnconstrained<T>(
                    [InstantHandle, NoEnumeration, NotNullOnExit] T value,
                    [InvokerParameterName] string paramName
                )
            {
                VALIDATOR.IsNotNullUnconstrained(value, paramName);
            }

            [PublicAPI]
            public static void IsNotNullOrEmpty([NotNullOnExit] string? value, [InvokerParameterName] string paramName)
            {
                VALIDATOR.IsNotNullOrEmpty(value, paramName);
            }

            [PublicAPI]
            public static void IsNotNullOrWhiteSpace([NotNullOnExit] string? value, [InvokerParameterName] string paramName)
            {
                VALIDATOR.IsNotNullOrWhiteSpace(value, paramName);
            }

            [PublicAPI]
            public static void IsNotNullOrEmpty<T>([NotNullOnExit] IReadOnlyCollection<T>? value, [InvokerParameterName] string paramName)
            {
                VALIDATOR.IsNotNullOrEmpty(value, paramName);
            }

            [PublicAPI]
            public static void IsNotReadOnly<T>([NotNullOnExit] ICollection<T> value, [InvokerParameterName] string paramName)
            {
                VALIDATOR.IsNotReadOnly(value, paramName);
            }
        }

        #endregion Argument Validation

        #region Value Validation

        public static partial class Value
        {
            [PublicAPI]
            public static void IsNotNull<T>(
                    [InstantHandle, NoEnumeration, NotNullOnExit] T? value,
                    string valueName
                )
                    where T : class
            {
                VALIDATOR.IsNotNull(value, valueName);
            }

            [PublicAPI]
            public static void IsNotNull<T>(
                    [InstantHandle, NoEnumeration, NotNullOnExit] T? value,
                    string valueName
                )
                    where T : struct
            {
                VALIDATOR.IsNotNull(value, valueName);
            }

            [PublicAPI]
            public static void IsNotNullUnconstrained<T>(
                    [InstantHandle, NoEnumeration, NotNullOnExit] T value,
                    string valueName
                )
            {
                VALIDATOR.IsNotNullUnconstrained(value, valueName);
            }

            [PublicAPI]
            public static void IsNotNullOrEmpty([NotNullOnExit] string? value, string valueName)
            {
                VALIDATOR.IsNotNullOrEmpty(value, valueName);
            }

            [PublicAPI]
            public static void IsNotNullOrWhiteSpace([NotNullOnExit] string? value, string valueName)
            {
                VALIDATOR.IsNotNullOrWhiteSpace(value, valueName);
            }

            [PublicAPI]
            public static void IsNotNullOrEmpty<T>([NotNullOnExit] IReadOnlyCollection<T>? value, string valueName)
            {
                VALIDATOR.IsNotNullOrEmpty(value, valueName);
            }

            [PublicAPI]
            public static void IsNotReadOnly<T>([NotNullOnExit] ICollection<T> value, string valueName)
            {
                VALIDATOR.IsNotReadOnly(value, valueName);
            }
        }

        #endregion Value Validation

        // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

        private abstract partial class ValidatorBase<TBaseException> where TBaseException : Exception
        {
            /// <summary>
            /// Validates that the given (reference type) value is not null.
            /// </summary>
            /// <remarks>
            /// This check is split into this method and the other overload so that it can't be
            /// accidentally called for non-nullable value types. Especially structs may be misinterpreted
            /// as classes and thus unnecessarily checked for null. (This is why this method needs the
            /// "where T : class" constraint.)
            ///
            /// <para>Also, the overload for value types becomes faster this way as it avoids any boxing. See:
            /// https://sharplab.io/#v2:EYLgtghgzgLgpgJwDQxASwDYB8ACAmARgFgAoHAZgAJ9KBhSgb1MpcuAHt2NKBZAgHgAqAPgAUgygA8AlJQDuAC0RxKEkNTyUAvMKmU0USgDsArhgwBuUgEgOXXniFjBAfimzFy1ZXWwEJgGMYbV1JfUNTcysSAF8gA=
            /// </para>
            ///
            /// <para>Only downside is that we can no longer use this to check values with unconstrained generic types.
            /// Instead, <see cref="IsNotNullUnconstrained{T}"/> must be used.</para>
            /// </remarks>
            /// <seealso cref="IsNotNullUnconstrained{T}"/>
            public void IsNotNull<T>([InstantHandle, NoEnumeration, NotNullOnExit] T? value, string valueName) where T : class
            {
                if (value is null)
                {
                    throw CreateNullException(valueName);
                }
            }

            /// <summary>
            /// Validates that the given (value type) value is not null.
            /// </summary>
            /// <remarks>
            /// This check is split into this method and the other <c>IsNotNull</c> overload. See other overload for more details.
            /// </remarks>
            /// <seealso cref="IsNotNullUnconstrained{T}"/>
            public void IsNotNull<T>([InstantHandle, NoEnumeration, NotNullOnExit] T? value, string valueName) where T : struct
            {
                if (!value.HasValue)
                {
                    throw CreateNullException(valueName);
                }
            }

            /// <summary>
            /// Validates that the given (unconstrained generic type) value is not null.
            ///
            /// <para>Note: You should prefer <c>IsNotNull()</c> instead. Only use this method
            /// if you have a value with a generic type that is unconstrained.</para>
            /// </summary>
            public void IsNotNullUnconstrained<T>([InstantHandle, NoEnumeration, NotNullOnExit] T value, string valueName)
            {
                if (value is null)
                {
                    throw CreateNullException(valueName);
                }
            }

            /// <summary>
            /// Validates that the the given string is neither <c>null</c> nor empty.
            /// </summary>
            public void IsNotNullOrEmpty([NotNullOnExit] string? value, string valueName)
            {
                if (value is null)
                {
                    throw CreateNullException(valueName);
                }

                if (value.Length == 0)
                {
                    throw CreateRootException(ExceptionMessages.STRING_IS_EMPTY, valueName);
                }
            }

            /// <summary>
            /// Validates that the the given string is neither <c>null</c> nor empty nor only white space characters.
            /// </summary>
            public void IsNotNullOrWhiteSpace([NotNullOnExit] string? value, string valueName)
            {
                // NOTE: For performance reasons (in case this check passes), we use this check first. If it fails,
                //   we're already in an exception situation and then performance is no longer this important.
                if (string.IsNullOrWhiteSpace(value))
                {
                    if (value is null)
                    {
                        throw CreateNullException(valueName);
                    }

                    if (value.Length == 0)
                    {
                        throw CreateRootException(ExceptionMessages.STRING_IS_EMPTY, valueName);
                    }

                    throw CreateRootException(ExceptionMessages.STRING_IS_WHITE_SPACES, valueName);
                }
            }

            /// <summary>
            /// Validates that the the given collection is neither <c>null</c> nor empty.
            /// </summary>
            public void IsNotNullOrEmpty<T>([NotNullOnExit] IReadOnlyCollection<T>? value, string valueName)
            {
                if (value is null)
                {
                    throw CreateNullException(valueName);
                }

                if (value.Count == 0)
                {
                    throw CreateRootException(ExceptionMessages.COLLECTION_IS_EMPTY, valueName);
                }
            }

            /// <summary>
            /// Verifies that the <see cref="ICollection{T}.IsReadOnly"/> property of the specified value
            /// is <c>false</c>; otherwise a <see cref="CollectionIsReadOnlyArgumentException"/> or
            /// <see cref="CollectionIsReadOnlyValueException"/> is thrown (depending on the value type).
            /// </summary>
            public void IsNotReadOnly<T>([NotNullOnExit] ICollection<T> value, string valueName)
            {
                if (value.IsReadOnly)
                {
                    throw CreateCollectionIsReadOnlyException(valueName);
                }
            }
        }

        // ReSharper restore ParameterOnlyUsedForPreconditionCheck.Local
    }

    /// <summary>
    /// Contains extension methods that use <see cref="Validate"/>.
    /// </summary>
    /// <remarks>
    /// These methods are not defined directly in <see cref="Validate"/> so that the don't appear in
    /// the code completion list of <see cref="Validate"/>.
    /// </remarks>
    public static class ValidationExtensionMethods
    {
        /// <summary>
        /// This method does the same thing as <see cref="Validate.Argument.IsNotNull{T}(T,string)"/> and primarily
        /// exists for constructor chaining where argument members are passed to another constructor
        /// and thus can't be validated with a statement.
        /// </summary>
        /// <returns>Simply returns <paramref name="value"/>.</returns>
        [PublicAPI]
        [MustUseReturnValue]
        public static T AsNotNullArgument<T>(
                [InstantHandle, NoEnumeration, NotNullOnExit] this T? value,
                [InvokerParameterName] string paramName
            )
                where T : class
        {
            Validate.Argument.IsNotNull(value, paramName);

            return value;
        }

        /// <summary>
        /// This method does the same thing as <see cref="Validate.Argument.IsNotNull{T}(T?,string)"/> and primarily
        /// exists for constructor chaining where argument members are passed to another constructor
        /// and thus can't be validated with a statement.
        /// </summary>
        /// <returns>Simply returns <paramref name="value"/>.</returns>
        [PublicAPI]
        [MustUseReturnValue]
        public static T AsNotNullArgument<T>(
                [InstantHandle, NoEnumeration, NotNullOnExit] this T? value,
                [InvokerParameterName] string paramName
            )
                where T : struct
        {
            Validate.Argument.IsNotNull(value, paramName);

            return value.Value;
        }

        /// <summary>
        /// This method does the same thing as <see cref="Validate.Argument.IsNotNullUnconstrained{T}"/> and primarily
        /// exists for constructor chaining where argument members are passed to another constructor
        /// and thus can't be validated with a statement.
        /// </summary>
        /// <returns>Simply returns <paramref name="value"/>.</returns>
        [PublicAPI]
        [MustUseReturnValue]
        [return: NotNullOnExit]
        public static T AsNotNullArgumentUnconstrained<T>(
                [InstantHandle, NoEnumeration, NotNullOnExit] this T value,
                [InvokerParameterName] string paramName
            )
        {
            Validate.Argument.IsNotNullUnconstrained(value, paramName);

            return value;
        }
    }
}
