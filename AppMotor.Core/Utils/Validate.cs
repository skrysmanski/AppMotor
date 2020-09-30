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

namespace AppMotor.Core.Utils
{
    /// <summary>
    /// Provides various validation methods for parameters and values in general. Raises <see cref="ArgumentException"/>s
    /// and <see cref="ValueException"/>s.
    /// </summary>
    public static class Validate
    {
        #region Argument Validation

        /// <summary>
        /// Validates arguments/parameters.
        /// </summary>
        /// <remarks>
        /// This class is called <c>Argument</c> rather than <c>Parameter</c> because all of its
        /// methods throw <see cref="ArgumentException"/>s.
        /// </remarks>
#pragma warning disable CA1034 // Nested types should not be visible
        public static class Argument
#pragma warning restore CA1034 // Nested types should not be visible
        {
            private static readonly ArgumentValidator VALIDATOR = new ArgumentValidator();

            [PublicAPI]
            [ContractAnnotation("value:null => halt")]
            public static void IsNotNull<T>(
                    [InstantHandle, NoEnumeration, ValidatedNotNull] T value,
                    [InvokerParameterName] string paramName
                )
            {
                VALIDATOR.IsNotNull(value, paramName);
            }

            [PublicAPI]
            [ContractAnnotation("value:null => halt")]
            public static void IsNotNullOrEmpty([ValidatedNotNull] string? value, [InvokerParameterName] string paramName)
            {
                VALIDATOR.IsNotNullOrEmpty(value, paramName);
            }

            [PublicAPI]
            [ContractAnnotation("value:null => halt")]
            public static void IsNotNullOrWhiteSpace([ValidatedNotNull] string? value, [InvokerParameterName] string paramName)
            {
                VALIDATOR.IsNotNullOrWhiteSpace(value, paramName);
            }

            [PublicAPI]
            [ContractAnnotation("value:null => halt")]
            public static void IsNotNullOrEmpty<T>([ValidatedNotNull] IReadOnlyCollection<T>? value, [InvokerParameterName] string paramName)
            {
                VALIDATOR.IsNotNullOrEmpty(value, paramName);
            }

            /// <summary>
            /// Verifies that the <see cref="ICollection{T}.IsReadOnly"/> property of <paramref name="value"/>
            /// is <c>true</c>; otherwise a <see cref="CollectionIsReadOnlyArgumentException"/> will be thrown.
            /// </summary>
            /// <exception cref="CollectionIsReadOnlyArgumentException">Thrown if the collection is read-only.</exception>
            [PublicAPI]
            public static void IsNotReadOnly<T>([ValidatedNotNull] ICollection<T> value, [InvokerParameterName] string paramName)
            {
                VALIDATOR.IsNotReadOnly(value, paramName);
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
        }

        #endregion Argument Validation

        #region Argument Validation Extension Methods

        /// <summary>
        /// This method does the same thing as <see cref="Argument.IsNotNull{T}"/> and primarily
        /// exists for constructor chaining where argument members are passed to another constructor
        /// and thus can't be validated with a statement.
        /// </summary>
        /// <returns>Simply returns <paramref name="value"/>.</returns>
        [PublicAPI]
        [MustUseReturnValue]
        [ContractAnnotation("value:null => halt")]
        public static T AsNotNullArgument<T>(
                [InstantHandle, NoEnumeration, ValidatedNotNull] this T value,
                [InvokerParameterName] string paramName
            )
        {
            Argument.IsNotNull(value, paramName);
            return value;
        }

        #endregion Argument Validation Extension Methods

        #region Value Validation

#pragma warning disable CA1034 // Nested types should not be visible
        public static class Value
#pragma warning restore CA1034 // Nested types should not be visible
        {
            private static readonly ValueValidator VALIDATOR = new ValueValidator();

            [PublicAPI]
            [ContractAnnotation("value:null => halt")]
            public static void IsNotNull<T>(
                    [InstantHandle, NoEnumeration, ValidatedNotNull] T value,
                    string valueName
                )
            {
                VALIDATOR.IsNotNull(value, valueName);
            }

            [PublicAPI]
            [ContractAnnotation("value:null => halt")]
            public static void IsNotNullOrEmpty([ValidatedNotNull] string? value, string valueName)
            {
                VALIDATOR.IsNotNullOrEmpty(value, valueName);
            }

            [PublicAPI]
            [ContractAnnotation("value:null => halt")]
            public static void IsNotNullOrWhiteSpace([ValidatedNotNull] string? value, string valueName)
            {
                VALIDATOR.IsNotNullOrWhiteSpace(value, valueName);
            }

            [PublicAPI]
            [ContractAnnotation("value:null => halt")]
            public static void IsNotNullOrEmpty<T>([ValidatedNotNull] IReadOnlyCollection<T>? value, string valueName)
            {
                VALIDATOR.IsNotNullOrEmpty(value, valueName);
            }

            /// <summary>
            /// Verifies that the <see cref="ICollection{T}.IsReadOnly"/> property of <paramref name="value"/>
            /// is <c>true</c>; otherwise a <see cref="CollectionIsReadOnlyValueException"/> will be thrown.
            /// </summary>
            /// <exception cref="CollectionIsReadOnlyValueException">Thrown if the collection is read-only.</exception>
            [PublicAPI]
            public static void IsNotReadOnly<T>([ValidatedNotNull] ICollection<T> value, string valueName)
            {
                VALIDATOR.IsNotReadOnly(value, valueName);
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
        }

        #endregion Value Validation

        // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

        private abstract class ValidatorBase<TBaseException> where TBaseException : Exception
        {
            protected abstract TBaseException CreateNullException(string? valueName);

            protected abstract TBaseException CreateCollectionIsReadOnlyException(string? valueName);

            protected abstract TBaseException CreateRootException(string message, string? valueName);

            [ContractAnnotation("value:null => halt")]
            public void IsNotNull<T>([InstantHandle, NoEnumeration] T value, string valueName)
            {
                if (value is null)
                {
                    throw CreateNullException(valueName);
                }
            }

            [ContractAnnotation("value:null => halt")]
            public void IsNotNullOrEmpty(string? value, string valueName)
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

            [ContractAnnotation("value:null => halt")]
            public void IsNotNullOrWhiteSpace(string? value, string valueName)
            {
                if (value is null)
                {
                    throw CreateNullException(valueName);
                }

                if (value.Length == 0)
                {
                    throw CreateRootException(ExceptionMessages.STRING_IS_EMPTY, valueName);
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw CreateRootException(ExceptionMessages.STRING_IS_WHITE_SPACES, valueName);
                }
            }

            [ContractAnnotation("value:null => halt")]
            public void IsNotNullOrEmpty<T>(IReadOnlyCollection<T>? value, string valueName)
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
            /// Verifies that the <see cref="ICollection{T}.IsReadOnly"/> property of <paramref name="value"/>
            /// is <c>true</c>; otherwise a <see cref="CollectionIsReadOnlyArgumentException"/> will be thrown.
            /// </summary>
            /// <exception cref="CollectionIsReadOnlyArgumentException">Thrown if the collection is read-only.</exception>
            public void IsNotReadOnly<T>(ICollection<T> value, string valueName)
            {
                if (value is null)
                {
                    throw CreateNullException(valueName);
                }

                if (value.IsReadOnly)
                {
                    throw CreateCollectionIsReadOnlyException(valueName);
                }
            }
        }

        // ReSharper restore ParameterOnlyUsedForPreconditionCheck.Local

        internal static class ExceptionMessages
        {
            public const string DEFAULT_VALUE_NAME = "unknown";

            public const string DEFAULT_MESSAGE = "The value is invalid.";

            public const string VALUE_IS_NULL = "The value must not be null.";

            public const string STRING_IS_EMPTY = "The string must not be empty.";

            public const string STRING_IS_WHITE_SPACES = "The string must not contain just white space characters.";

            public const string COLLECTION_IS_EMPTY = "The collection must not be empty.";
        }
    }
}
