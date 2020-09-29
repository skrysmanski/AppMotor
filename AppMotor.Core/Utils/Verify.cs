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
    /// Provides various verification methods for parameters and values in general. Raises <see cref="ArgumentException"/>s
    /// and <see cref="ValueException"/>s.
    /// </summary>
    public static class Verify
    {
        #region Argument Verification

        public static class Argument
        {
            private static readonly ArgumentVerifier VERIFIER = new ArgumentVerifier();

            [PublicAPI]
            [ContractAnnotation("obj:null => halt")]
            public static void IsNotNull<T>([InstantHandle, NoEnumeration, ValidatedNotNull] T obj, [InvokerParameterName] string paramName)
            {
                VERIFIER.IsNotNull(obj, paramName);
            }

            [PublicAPI]
            [ContractAnnotation("obj:null => halt")]
            public static void IsNotNullOrEmpty([ValidatedNotNull] string? obj, [InvokerParameterName] string paramName)
            {
                VERIFIER.IsNotNullOrEmpty(obj, paramName);
            }

            [PublicAPI]
            [ContractAnnotation("obj:null => halt")]
            public static void IsNotNullOrWhiteSpace([ValidatedNotNull] string? obj, [InvokerParameterName] string paramName)
            {
                VERIFIER.IsNotNullOrWhiteSpace(obj, paramName);
            }

            [PublicAPI]
            [ContractAnnotation("obj:null => halt")]
            public static void IsNotNullOrEmpty<T>([ValidatedNotNull] IReadOnlyCollection<T>? obj, [InvokerParameterName] string paramName)
            {
                VERIFIER.IsNotNullOrEmpty(obj, paramName);
            }

            /// <summary>
            /// Verifies that the <see cref="ICollection{T}.IsReadOnly"/> property of <paramref name="collectionToCheck"/>
            /// is <c>true</c>; otherwise a <see cref="CollectionIsReadOnlyArgumentException"/> will be thrown.
            /// </summary>
            /// <exception cref="CollectionIsReadOnlyArgumentException">Thrown if the collection is read-only.</exception>
            [PublicAPI]
            public static void IsNotReadOnly<T>([ValidatedNotNull] ICollection<T> collectionToCheck, [InvokerParameterName] string paramName)
            {
                VERIFIER.IsNotReadOnly(collectionToCheck, paramName);
            }

            private sealed class ArgumentVerifier : VerifierBase<ArgumentException>
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

        #endregion Argument Verification

        #region Value Verification

        public static class Value
        {
            private static readonly ValueVerifier VERIFIER = new ValueVerifier();

            [PublicAPI]
            [ContractAnnotation("obj:null => halt")]
            public static void IsNotNull<T>([InstantHandle, NoEnumeration, ValidatedNotNull] T obj, string valueName)
            {
                VERIFIER.IsNotNull(obj, valueName);
            }

            [PublicAPI]
            [ContractAnnotation("obj:null => halt")]
            public static void IsNotNullOrEmpty([ValidatedNotNull] string? obj, string valueName)
            {
                VERIFIER.IsNotNullOrEmpty(obj, valueName);
            }

            [PublicAPI]
            [ContractAnnotation("obj:null => halt")]
            public static void IsNotNullOrWhiteSpace([ValidatedNotNull] string? obj, string valueName)
            {
                VERIFIER.IsNotNullOrWhiteSpace(obj, valueName);
            }

            [PublicAPI]
            [ContractAnnotation("obj:null => halt")]
            public static void IsNotNullOrEmpty<T>([ValidatedNotNull] IReadOnlyCollection<T>? obj, string valueName)
            {
                VERIFIER.IsNotNullOrEmpty(obj, valueName);
            }

            /// <summary>
            /// Verifies that the <see cref="ICollection{T}.IsReadOnly"/> property of <paramref name="collectionToCheck"/>
            /// is <c>true</c>; otherwise a <see cref="CollectionIsReadOnlyValueException"/> will be thrown.
            /// </summary>
            /// <exception cref="CollectionIsReadOnlyValueException">Thrown if the collection is read-only.</exception>
            [PublicAPI]
            public static void IsNotReadOnly<T>([ValidatedNotNull] ICollection<T> collectionToCheck, string valueName)
            {
                VERIFIER.IsNotReadOnly(collectionToCheck, valueName);
            }

            private sealed class ValueVerifier : VerifierBase<ValueException>
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

        #endregion Value Verification

        // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

        private abstract class VerifierBase<TBaseException> where TBaseException : Exception
        {
            protected abstract TBaseException CreateNullException(string? valueName);

            protected abstract TBaseException CreateCollectionIsReadOnlyException(string? valueName);

            protected abstract TBaseException CreateRootException(string message, string? valueName);

            [ContractAnnotation("obj:null => halt")]
            public void IsNotNull<T>([InstantHandle, NoEnumeration] T obj, string valueName)
            {
                if (obj is null)
                {
                    throw CreateNullException(valueName);
                }
            }

            [ContractAnnotation("obj:null => halt")]
            public void IsNotNullOrEmpty(string? obj, string valueName)
            {
                if (obj is null)
                {
                    throw CreateNullException(valueName);
                }

                if (obj.Length == 0)
                {
                    throw CreateRootException(ExceptionMessages.STRING_IS_EMPTY, valueName);
                }
            }

            [ContractAnnotation("obj:null => halt")]
            public void IsNotNullOrWhiteSpace(string? obj, string valueName)
            {
                if (obj is null)
                {
                    throw CreateNullException(valueName);
                }

                if (obj.Length == 0)
                {
                    throw CreateRootException(ExceptionMessages.STRING_IS_EMPTY, valueName);
                }

                if (string.IsNullOrWhiteSpace(obj))
                {
                    throw CreateRootException(ExceptionMessages.STRING_IS_WHITE_SPACES, valueName);
                }
            }

            [ContractAnnotation("obj:null => halt")]
            public void IsNotNullOrEmpty<T>(IReadOnlyCollection<T>? obj, string valueName)
            {
                if (obj is null)
                {
                    throw CreateNullException(valueName);
                }

                if (obj.Count == 0)
                {
                    throw CreateRootException(ExceptionMessages.COLLECTION_IS_EMPTY, valueName);
                }
            }

            /// <summary>
            /// Verifies that the <see cref="ICollection{T}.IsReadOnly"/> property of <paramref name="collectionToCheck"/>
            /// is <c>true</c>; otherwise a <see cref="CollectionIsReadOnlyArgumentException"/> will be thrown.
            /// </summary>
            /// <exception cref="CollectionIsReadOnlyArgumentException">Thrown if the collection is read-only.</exception>
            public void IsNotReadOnly<T>(ICollection<T> collectionToCheck, string valueName)
            {
                if (collectionToCheck is null)
                {
                    throw CreateNullException(valueName);
                }

                if (collectionToCheck.IsReadOnly)
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
