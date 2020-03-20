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
