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
        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ParamNotNull<T>(
                [CanBeNull, InstantHandle, NoEnumeration] T obj,
                [InvokerParameterName] string paramName
            ) where T : class
        {
            if (obj is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ParamNotNull<T>([CanBeNull] T? obj, [InvokerParameterName] string paramName) where T : struct
        {
            if (!obj.HasValue)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ParamNotNullOrEmpty([CanBeNull] string obj, [InvokerParameterName] string paramName)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (obj == "")
            {
                throw new ArgumentException("String is empty.", paramName);
            }
        }

        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ParamNotNullOrWhiteSpace([CanBeNull] string obj, [InvokerParameterName] string paramName)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (string.IsNullOrWhiteSpace(obj))
            {
                throw new ArgumentException("String is empty or just contains white space characters.", paramName);
            }
        }

        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ValueNotNull<T>(
                [CanBeNull, InstantHandle, NoEnumeration] T obj,
                string valueName
            )
                where T : class
        {
            if (obj is null)
            {
                throw new ValueNullException(valueName);
            }
        }

        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ValueNotNull<T>([CanBeNull] T? obj, string valueName) where T : struct
        {
            if (!obj.HasValue)
            {
                throw new ValueNullException(valueName);
            }
        }

        /// <summary>
        /// Verifies that the <see cref="ICollection{T}.IsReadOnly"/> property of <paramref name="collectionToCheck"/>
        /// is <c>true</c>; otherwise a <see cref="CollectionIsReadOnlyArgumentException"/> will be thrown.
        /// </summary>
        /// <exception cref="CollectionIsReadOnlyArgumentException">Thrown if the collection is read-only.</exception>
        [PublicAPI]
        public static void ParamNotReadOnly<T>([NotNull] ICollection<T> collectionToCheck, [InvokerParameterName] string paramName)
        {
            ParamNotNull(collectionToCheck, nameof(collectionToCheck));

            if (collectionToCheck.IsReadOnly)
            {
                throw new CollectionIsReadOnlyArgumentException(paramName: paramName);
            }
        }

        /// <summary>
        /// Verifies that the <see cref="ICollection{T}.IsReadOnly"/> property of <paramref name="collectionToCheck"/>
        /// is <c>true</c>; otherwise a <see cref="CollectionIsReadOnlyValueException"/> will be thrown.
        /// </summary>
        /// <exception cref="CollectionIsReadOnlyValueException">Thrown if the collection is read-only.</exception>
        [PublicAPI]
        public static void ValueNotReadOnly<T>([NotNull] ICollection<T> collectionToCheck, [NotNull] string valueName)
        {
            ParamNotNull(collectionToCheck, nameof(collectionToCheck));

            if (collectionToCheck.IsReadOnly)
            {
                throw new CollectionIsReadOnlyValueException(valueName: valueName);
            }
        }
    }
}
