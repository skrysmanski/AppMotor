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

        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ParamNotNull<T>([CanBeNull, InstantHandle, NoEnumeration] T obj, [InvokerParameterName, NotNull] string paramName) where T : class
        {
            ArgumentVerifier.INSTANCE.NotNull(obj, paramName);
        }

        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ParamNotNull<T>([CanBeNull] T? obj, [InvokerParameterName, NotNull] string paramName) where T : struct
        {
            ArgumentVerifier.INSTANCE.NotNull(obj, paramName);
        }

        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ParamNotNullOrEmpty([CanBeNull] string obj, [InvokerParameterName, NotNull] string paramName)
        {
            ArgumentVerifier.INSTANCE.NotNullOrEmpty(obj, paramName);
        }

        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ParamNotNullOrWhiteSpace([CanBeNull] string obj, [InvokerParameterName, NotNull] string paramName)
        {
            ArgumentVerifier.INSTANCE.NotNullOrWhiteSpace(obj, paramName);
        }

        /// <summary>
        /// Verifies that the <see cref="ICollection{T}.IsReadOnly"/> property of <paramref name="collectionToCheck"/>
        /// is <c>true</c>; otherwise a <see cref="CollectionIsReadOnlyArgumentException"/> will be thrown.
        /// </summary>
        /// <exception cref="CollectionIsReadOnlyArgumentException">Thrown if the collection is read-only.</exception>
        [PublicAPI]
        public static void ParamNotReadOnly<T>([NotNull] ICollection<T> collectionToCheck, [InvokerParameterName, NotNull] string paramName)
        {
            ArgumentVerifier.INSTANCE.NotReadOnly(collectionToCheck, paramName);
        }

        #endregion Argument Verification

        #region Value Verification

        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ValueNotNull<T>([CanBeNull, InstantHandle, NoEnumeration] T obj, [NotNull] string valueName) where T : class
        {
            ValueVerifier.INSTANCE.NotNull(obj, valueName);
        }

        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ValueNotNull<T>([CanBeNull] T? obj, [NotNull] string valueName) where T : struct
        {
            ValueVerifier.INSTANCE.NotNull(obj, valueName);
        }

        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ValueNotNullOrEmpty([CanBeNull] string obj, [NotNull] string valueName)
        {
            ValueVerifier.INSTANCE.NotNullOrEmpty(obj, valueName);
        }

        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ValueNotNullOrWhiteSpace([CanBeNull] string obj, [NotNull] string valueName)
        {
            ValueVerifier.INSTANCE.NotNullOrWhiteSpace(obj, valueName);
        }

        /// <summary>
        /// Verifies that the <see cref="ICollection{T}.IsReadOnly"/> property of <paramref name="collectionToCheck"/>
        /// is <c>true</c>; otherwise a <see cref="CollectionIsReadOnlyValueException"/> will be thrown.
        /// </summary>
        /// <exception cref="CollectionIsReadOnlyValueException">Thrown if the collection is read-only.</exception>
        [PublicAPI]
        public static void ValueNotReadOnly<T>([NotNull] ICollection<T> collectionToCheck, [NotNull] string valueName)
        {
            ValueVerifier.INSTANCE.NotReadOnly(collectionToCheck, valueName);
        }

        #endregion Value Verification

        // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

        private abstract class VerifierBase<TBaseException> where TBaseException : Exception
        {
            protected const string DEFAULT_VALUE_NAME = "unknown";

            [NotNull]
            protected abstract TBaseException CreateNullException([CanBeNull] string valueName);

            [NotNull]
            protected abstract TBaseException CreateCollectionIsReadOnlyException([CanBeNull] string valueName);

            [NotNull]
            protected abstract TBaseException CreateRootException([NotNull] string message, [CanBeNull] string valueName);

            [ContractAnnotation("obj:null => halt")]
            public void NotNull<T>([CanBeNull, InstantHandle, NoEnumeration] T obj, [NotNull] string valueName) where T : class
            {
                if (obj is null)
                {
                    throw CreateNullException(valueName);
                }
            }

            [ContractAnnotation("obj:null => halt")]
            public void NotNull<T>([CanBeNull] T? obj, [NotNull] string valueName) where T : struct
            {
                if (!obj.HasValue)
                {
                    throw CreateNullException(valueName);
                }
            }

            [ContractAnnotation("obj:null => halt")]
            public void NotNullOrEmpty([CanBeNull] string obj, [NotNull] string valueName)
            {
                if (obj is null)
                {
                    throw CreateNullException(valueName);
                }

                if (obj.Length == 0)
                {
                    throw CreateRootException("String is empty.", valueName);
                }
            }

            [ContractAnnotation("obj:null => halt")]
            public void NotNullOrWhiteSpace([CanBeNull] string obj, [NotNull] string valueName)
            {
                if (obj is null)
                {
                    throw CreateNullException(valueName);
                }

                if (obj.Length == 0)
                {
                    throw CreateRootException("String is empty.", valueName);
                }

                if (string.IsNullOrWhiteSpace(obj))
                {
                    throw CreateRootException("String just contains white space characters.", valueName);
                }
            }

            /// <summary>
            /// Verifies that the <see cref="ICollection{T}.IsReadOnly"/> property of <paramref name="collectionToCheck"/>
            /// is <c>true</c>; otherwise a <see cref="CollectionIsReadOnlyArgumentException"/> will be thrown.
            /// </summary>
            /// <exception cref="CollectionIsReadOnlyArgumentException">Thrown if the collection is read-only.</exception>
            public void NotReadOnly<T>([NotNull] ICollection<T> collectionToCheck, [NotNull] string valueName)
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

        private sealed class ArgumentVerifier : VerifierBase<ArgumentException>
        {
            [NotNull]
            public static readonly ArgumentVerifier INSTANCE = new ArgumentVerifier();

            /// <inheritdoc />
            protected override ArgumentException CreateNullException(string valueName)
            {
                return new ArgumentNullException(paramName: valueName ?? DEFAULT_VALUE_NAME);
            }

            /// <inheritdoc />
            protected override ArgumentException CreateCollectionIsReadOnlyException(string valueName)
            {
                return new CollectionIsReadOnlyArgumentException(paramName: valueName ?? DEFAULT_VALUE_NAME);
            }

            /// <inheritdoc />
            protected override ArgumentException CreateRootException(string message, string valueName)
            {
                return new ArgumentException(message: message, paramName: valueName ?? DEFAULT_VALUE_NAME);
            }
        }

        private sealed class ValueVerifier : VerifierBase<ValueException>
        {
            [NotNull]
            public static readonly ValueVerifier INSTANCE = new ValueVerifier();

            /// <inheritdoc />
            protected override ValueException CreateNullException(string valueName)
            {
                return new ValueNullException(message: null, valueName: valueName ?? DEFAULT_VALUE_NAME);
            }

            /// <inheritdoc />
            protected override ValueException CreateCollectionIsReadOnlyException(string valueName)
            {
                return new CollectionIsReadOnlyValueException(valueName: valueName ?? DEFAULT_VALUE_NAME);
            }

            /// <inheritdoc />
            protected override ValueException CreateRootException(string message, string valueName)
            {
                return new ValueException(message: message, valueName: valueName ?? DEFAULT_VALUE_NAME);
            }
        }
    }
}
