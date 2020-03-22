/*
Copyright 2020 AppWeave.Core (https://github.com/skrysmanski/AppWeave.Core)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;

using AppWeave.Core.Exceptions;

using JetBrains.Annotations;

namespace AppWeave.Core.Utils
{
    /// <summary>
    /// Provides various verification methods for parameters and values in general. Raises <see cref="ArgumentException"/>s
    /// and <see cref="ValueException"/>s.
    /// </summary>
    public static class Verify
    {
        [PublicAPI]
        [ContractAnnotation("obj:null => halt")]
        public static void ParamNotNull<T>([CanBeNull, InstantHandle] T obj, [InvokerParameterName] string paramName) where T : class
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
        public static void ValueNotNull<T>([CanBeNull, InstantHandle] T obj, string valueName) where T : class
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
    }
}
